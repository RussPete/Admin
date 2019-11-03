using Globals;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

/// <summary>
/// Summary description for RecipeBookPdf
/// </summary>
public class PdfRecipeBook
{
    protected Page Page;
    protected float Margin = 18;
    protected BaseFont BaseFont;

    protected string RecipeName;
    protected string Category;
    protected string PrevCategory;

    protected Font fntMainTitle;
    protected Font fntText;
    protected DataTable dt;
    protected int iRow;
    protected LinkedList<int> PageOrder = new LinkedList<int>();
    protected LinkedList<PageInfo> CategoryPages = new LinkedList<PageInfo>();
    protected int PageNum;
    protected int CategoryPageNum;
    protected int NextPageNum;

    public struct PageInfo
    {
        public string Name;
        public int PageNum;
    }

    public PdfRecipeBook(Page Page)
    {
        this.Page = Page;
    }

    public byte[] Create(string Where)
    {
        byte[] data = null;

        try
        {
            int Border = 0;

            using (MemoryStream ms = new MemoryStream())
            {
                // create PDF document 
                // margins on first page (page 2, first recipe, shifted to the left)
                using (Document doc = new Document(PageSize.HALFLETTER, Margin, 2 * Margin, 2 * Margin, 2 * Margin))        // 0.25" margins on sides + gutter of 0.25", 0.5" top & bottom  72 units per inch
                {
                    using (PdfWriter pw = PdfWriter.GetInstance(doc, ms))
                    {
                        try
                        {
                            // gutter margins
                            //doc.SetMarginMirroring(true);   // has to be called after PdfWriter.GetInstance

                            pw.PageEvent = new PageEvents(this);

                            string ImagePath = this.Page.Server.MapPath(@"~/Images");

                            BaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                            fntMainTitle = new Font(BaseFont, 14, Font.BOLD, BaseColor.BLACK);
                            Font fntServings = new Font(BaseFont, 10, Font.ITALIC, BaseColor.DARK_GRAY);
                            Font fntTitle = new Font(BaseFont, 11, Font.BOLD, BaseColor.BLACK);
                            fntText = new Font(BaseFont, 11, Font.NORMAL, BaseColor.BLACK);
                            Font fntNote = new Font(BaseFont, 9, Font.ITALIC, BaseColor.BLACK);
                            Font fntDirections = new Font(BaseFont, 11, Font.ITALIC, BaseColor.DARK_GRAY);
                            Font fntCategory = new Font(BaseFont, 9, Font.NORMAL, BaseColor.DARK_GRAY);
                            Font fntPageNum = new Font(BaseFont, 9, Font.NORMAL, BaseColor.DARK_GRAY);

                            DB db = new DB();
                            string Sql =
                                "Select r.Category, r.Name, r.RecipeRno, r.NumServings, r.Instructions, " +
                                "xr.RecipeSeq, xr.UnitQty, u.UnitSingle, u.UnitPlural, " +
                                "Coalesce(i.Name, sr.Name, xr.Title) Ingred, xr.Note " +
                                "From Recipes r " +
                                "Inner Join RecipeIngredXref xr On r.RecipeRno = xr.RecipeRno " +
                                "Left Join Ingredients i On xr.IngredRno = i.IngredRno " +
                                "Left Join Recipes sr On xr.SubrecipeRno = sr.RecipeRno " +
                                "Left Join Units u On xr.UnitRno = u.UnitRno " +
                                "Where IsNull(r.HideFlg, 0) = 0 " +
                                //"And r.RecipeRno in (1485, 1508) " +
                                Where + 
                                "Order By r.Category, r.Name, r.RecipeRno, xr.RecipeSeq";
                            dt = db.DataTable(Sql);

                            PrevCategory = string.Empty;
                            Int32 PrevRecipeRno = 0;
                            PdfPTable t = null;
                            string Instructions = string.Empty;
                            PageNum = 1;
                            CategoryPageNum = 1;
                            NextPageNum = 0;

                            for (iRow = 0; iRow < dt.Rows.Count; iRow++)
                            {
                                DataRow dr = dt.Rows[iRow];
                                RecipeName = DB.Str(dr["Name"]);
                                Category = DB.Str(dr["Category"]);
                                Int32 RecipeRno = DB.Int32(dr["RecipeRno"]);

                                if (RecipeRno != PrevRecipeRno) // || Category != PrevCategory)
                                {
                                    if (PrevRecipeRno == 0)
                                    {
                                        PrevCategory = Category;    // special case, show title on first page
                                        doc.Open();     // open here so the category is defined for OnStartPage
                                        pw.SetLinearPageMode();
                                    }
                                    else
                                    {
                                        FinishPage(pw, doc, t, Instructions, BaseFont, fntDirections, fntText);
                                        NewPage(doc, (Category != PrevCategory));
                                    }

                                    PrevCategory = Category;
                                    PrevRecipeRno = RecipeRno;

                                    decimal NumServings = DB.Dec(dr["NumServings"]);
                                    Instructions = DB.Str(dr["Instructions"]);

                                    // Main Title
                                    t = new PdfPTable(1);
                                    t.DefaultCell.Border = Border;
                                    t.WidthPercentage = 100;
                                    t.SetWidths(new float[] { 1 });

                                    t.AddCell(new PdfPCell(new Paragraph(new Chunk(RecipeName, fntMainTitle))) { Border = Border, HorizontalAlignment = Element.ALIGN_CENTER });
                                    t.AddCell(new PdfPCell(new Paragraph(new Chunk(string.Format("{0:F0} Servings", NumServings), fntServings))) { Border = Border, HorizontalAlignment = Element.ALIGN_CENTER });

                                    doc.Add(t);

                                    CategoryPages.AddLast(new PageInfo() { Name = RecipeName, PageNum = PageNum });

                                    t = new PdfPTable(4);
                                    t.DefaultCell.Border = Border;
                                    t.SpacingBefore = 15;
                                    t.SpacingAfter = 15;
                                    t.WidthPercentage = 100;
                                    t.SetWidths(new float[] { 1.2F, 2.4F, 5, 4 });
                                }

                                decimal Qty = DB.Dec(dr["UnitQty"]);
                                string Unit = DB.Str(dr["Unit" + (Qty <= 1 ? "Single" : "Plural")]);
                                string Ingred = DB.Str(dr["Ingred"]);
                                string Note = DB.Str(dr["Note"]);

                                if (Qty > 0)
                                {
                                    t.AddCell(new PdfPCell(new Paragraph(new Chunk(Str.ShowFract(Qty), fntText))) { Border = Border, HorizontalAlignment = Element.ALIGN_RIGHT });
                                    t.AddCell(new PdfPCell(new Paragraph(new Chunk(Unit, fntText))) { Border = Border });
                                    t.AddCell(new PdfPCell(new Paragraph(new Chunk(Ingred, fntText))) { Border = Border });
                                    t.AddCell(new PdfPCell(new Paragraph(new Chunk(Note, fntNote))) { Border = Border });
                                }
                                else
                                {
                                    t.AddCell(new PdfPCell(new Paragraph(new Chunk(Ingred, fntTitle))) { Border = Border, Colspan = 4, PaddingTop = 10 });
                                    t.CompleteRow();
                                }
                            }

                            FinishPage(pw, doc, t, Instructions, BaseFont, fntDirections, fntText);

                            // push last category
                            Category = string.Empty;
                            NewPage(doc, true);   // create page for category toc

                            int NumPages = pw.ReorderPages(null);

                            int[] Pages = PageOrder.ToArray<int>();
                            int[] Order = new int[NumPages];
                            for (int iOrder = 0; iOrder < Order.Length; iOrder++)
                            {
                                for (int iPages = 0; iPages < Pages.Length; iPages++)
                                {
                                    if (Pages[iPages] == iOrder + 1)
                                    {
                                        Order[iOrder] = iPages + 1;
                                        break;
                                    }
                                }
                            }
                            pw.ReorderPages(Order);

                            doc.Close();

                            data = ms.ToArray();
                        }
                        catch (Exception Ex)
                        {
                            Err Err = new Err(Ex);
                            HttpContext.Current.Response.Write(Err.Html());
                        }
                    }
                }
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex);
            HttpContext.Current.Response.Write(Err.Html());
        }

        return data;
    }

    private void FinishPage(PdfWriter pw, Document doc, PdfPTable t, string Instructions, BaseFont bf, Font fntDirections, Font fntText)
    {
        doc.Add(t);
        if (Instructions.Length > 0)
        {
            doc.Add(new Paragraph(new Chunk("Directions", fntDirections)));

            // Instructions contains html formatting, convert from html to pdf
            using (MemoryStream msCss = new MemoryStream(Encoding.UTF8.GetBytes(".Instructions {font-size: 10pt;} p {margin: 4px 0px;} ul {list-style-type: disc;}")))
            {
                using (MemoryStream msHtml = new MemoryStream(Encoding.UTF8.GetBytes(string.Format("<div class='Instructions'>{0}</div>", Instructions.Replace("<br>", "<br/>")))))
                {
                    try
                    { 
                        XMLWorkerHelper.GetInstance().ParseXHtml(pw, doc, msHtml, msCss);
                    }
                    catch (RuntimeWorkerException Ex)
                    {
                        Err Err = new Err(Ex);
                        HttpContext.Current.Response.Write(Err.Html());
                    }
                    catch (Exception Ex)
                    {
                        Err Err = new Err(Ex);
                        HttpContext.Current.Response.Write(Err.Html());
                    }
                }
            }
        }
    }

    private void NewPage(Document doc, bool fToc)
    {
        int PageNum = (NextPageNum > 0 ? NextPageNum : this.PageNum) + 1;

        if (fToc)
        {
            PageNum = CategoryPageNum;
        }

        doc.SetMargins((PageNum % 2 == 1 ? 2 : 1) * Margin, (PageNum % 2 == 1 ? 1 : 2) * Margin, 2 * Margin, 2 * Margin);
        doc.NewPage();
    }

    public class PageEvents : PdfPageEventHelper
    {
        PdfRecipeBook RecipeBook;

        public PageEvents(PdfRecipeBook RecipeBook)
        {
            this.RecipeBook = RecipeBook;
        }

        public override void OnStartPage(PdfWriter pw, Document doc)
        {
            float FontSize = 9;
            RecipeBook.PageNum++;

            // prep margins for the next page if we overflow
            int NextPageNum = RecipeBook.PageNum + 1;
            doc.SetMargins((NextPageNum % 2 == 1 ? 2 : 1) * RecipeBook.Margin, (NextPageNum % 2 == 1 ? 1 : 2) * RecipeBook.Margin, 2 * RecipeBook.Margin, 2 * RecipeBook.Margin);

            if (RecipeBook.Category != RecipeBook.PrevCategory)
            {
                NewCategory(pw, doc);
            }

            if (RecipeBook.Category.Length > 0)
            {
                PdfContentByte cb = pw.DirectContent;
                cb.SetColorFill(BaseColor.DARK_GRAY);
                cb.BeginText();
                cb.SetFontAndSize(RecipeBook.BaseFont, FontSize);

                // category
                float len = RecipeBook.BaseFont.GetWidthPoint(RecipeBook.Category, FontSize);
                if (RecipeBook.PageNum % 2 == 1)
                {
                    cb.SetTextMatrix(doc.GetRight(len), doc.GetTop(-RecipeBook.Margin + FontSize));
                }
                else
                {
                    cb.SetTextMatrix(doc.GetLeft(0), doc.GetTop(-RecipeBook.Margin + FontSize));
                }
                cb.ShowText(RecipeBook.Category);

                // certified
                const string Certified = "[  ] Certified";
                len = RecipeBook.BaseFont.GetWidthPoint(Certified, FontSize);
                if (RecipeBook.PageNum % 2 == 0)
                {
                    cb.SetTextMatrix(doc.GetRight(len), doc.GetTop(-RecipeBook.Margin + FontSize));
                }
                else
                {
                    cb.SetTextMatrix(doc.GetLeft(0), doc.GetTop(-RecipeBook.Margin + FontSize));
                }
                cb.ShowText(Certified);

                cb.EndText();
            }
        }

        public override void OnEndPage(PdfWriter pw, Document doc)
        {
            float FontSize = 9;

            RecipeBook.PageOrder.AddLast(RecipeBook.PageNum);
            string PageNum = RecipeBook.PageNum.ToString();
            float len = RecipeBook.BaseFont.GetWidthPoint(PageNum, FontSize);

            PdfContentByte cb = pw.DirectContent;
            cb.SetColorFill(BaseColor.DARK_GRAY);
            cb.BeginText();
            cb.SetFontAndSize(RecipeBook.BaseFont, FontSize);
            if (RecipeBook.PageNum % 2 == 1)
            {
                cb.SetTextMatrix(doc.GetRight(len), doc.GetBottom(-RecipeBook.Margin));
            }
            else
            {
                cb.SetTextMatrix(doc.GetLeft(0), doc.GetBottom(-RecipeBook.Margin));
            }
            cb.ShowText(PageNum);

            string Copyright = string.Format("Copyright © {0}-{1} {2}", g.Company.RecipeBookCopyright, DateTime.Today.Year, g.Company.LegalName);
            len = RecipeBook.BaseFont.GetWidthPoint(Copyright, FontSize);
            float pos = (doc.Right - doc.Left - len) / 2;
            cb.SetTextMatrix(doc.GetLeft(pos), doc.GetBottom(-RecipeBook.Margin));
            cb.ShowText(Copyright);

            cb.EndText();

            if (RecipeBook.NextPageNum > 0)
            {
                RecipeBook.PageNum = RecipeBook.NextPageNum;
                RecipeBook.NextPageNum = 0;
            }
        }

        private void NewCategory(PdfWriter pw, Document doc)
        {
            int Border = 0;

            try
            {
                // create previous category table of contents
                if (RecipeBook.CategoryPages.Count > 0)
                {
                    PdfPTable t = new PdfPTable(1);
                    t.DefaultCell.Border = Border;
                    t.WidthPercentage = 100;
                    t.SetWidths(new float[] { 1 });

                    t.AddCell(new PdfPCell(new Paragraph(new Chunk(RecipeBook.PrevCategory, RecipeBook.fntMainTitle))) { Border = Border, HorizontalAlignment = Element.ALIGN_CENTER });

                    doc.Add(t);

                    // table for 2 columns of table of contents
                    t = new PdfPTable(3);
                    t.DefaultCell.Border = Border;
                    t.SpacingBefore = 15;
                    t.SpacingAfter = 15;
                    t.WidthPercentage = 100;
                    t.SetWidths(new float[] { 4, 1, 4 });

                    int iCol = -1;
                    int MaxCols = 2;
                    int MaxRecipesPerCol = 20;
                    int iRecipeRow = MaxRecipesPerCol;
                    PdfPTable toc = null;
                    string PrevName = string.Empty;
                    int cCols = 0;

                    while (RecipeBook.CategoryPages.Count > 0 && iCol < MaxCols)
                    {
                        if (iRecipeRow >= MaxRecipesPerCol)
                        {
                            iRecipeRow = 0;
                            if (++iCol >= MaxCols)
                            {
                                break;
                            }

                            if (toc != null)
                            {
                                t.AddCell(toc);
                                t.AddCell("");
                                cCols++;
                            }

                            // prep column for recipe table of contents
                            toc = new PdfPTable(2);
                            toc.DefaultCell.Border = Border;
                            toc.WidthPercentage = 100;
                            toc.SetWidths(new float[] { 4, 1 });
                        }

                        PageInfo Page = RecipeBook.CategoryPages.First();
                        RecipeBook.CategoryPages.RemoveFirst();
                        toc.AddCell(new PdfPCell(new Paragraph(new Chunk(Page.Name, RecipeBook.fntText))) { Border = Border });
                        toc.AddCell(new PdfPCell(new Paragraph(new Chunk(Page.PageNum.ToString(), RecipeBook.fntText))) { Border = Border, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_BOTTOM });

                    }
                    t.AddCell(toc);
                    cCols++;

                    while (cCols % MaxCols != 0)
                    {
                        t.AddCell("");
                        t.AddCell("");
                        cCols++;
                    }

                    doc.Add(t);

                    RecipeBook.NextPageNum = RecipeBook.PageNum;
                    RecipeBook.PageNum = RecipeBook.CategoryPageNum;
                    RecipeBook.CategoryPageNum = RecipeBook.NextPageNum;

                    RecipeBook.NewPage(doc, false);
                }

                RecipeBook.CategoryPages.Clear();
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex);
                HttpContext.Current.Response.Write(Err.Html());
            }
        }
    }
}