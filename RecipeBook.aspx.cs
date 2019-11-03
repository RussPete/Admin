using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class RecipeBook : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
    protected bool fGenerating;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.Recipes.Title;
            Setup();
		}

        ListRecipes();
	}

    private void Setup()
    {
        string Sql = "Select KitchenLocRno, Name From KitchenLocations Order By SortOrder";

		try 
		{
            DataTable dt = db.DataTable(Sql);
            ddlLocation.Items.Clear();
            ddlLocation.Items.Add(new ListItem() { Text = "All", Value = "0" });
            foreach (DataRow dr in dt.Rows)
            {
                ddlLocation.Items.Add(new ListItem() { Text = DB.Str(dr["Name"]), Value = DB.Int32(dr["KitchenLocRno"]).ToString() });
            }
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
    }

    protected void ListRecipes()
    {
        string Sql =
            "Select r.Category, r.Name " +
            "From Recipes r " +
            "Where IsNull(r.HideFlg, 0) = 0 " +
            Filter() + 
            "Order By r.Category, r.Name";
        try
        {
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                HtmlTableRow tr = new HtmlTableRow();
                tr.Cells.Add(new HtmlTableCell() { InnerHtml = DB.Str(dr["Category"]) });
                tr.Cells.Add(new HtmlTableCell() { InnerHtml = DB.Str(dr["Name"]) });
                tbl.Rows.Add(tr);
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    protected string Filter()
    {
        string Where = string.Empty;

        if (ddlLocation.SelectedValue != "0")
        {
            Where += string.Format("And m.KitchenLocRno = {0} ", ddlLocation.SelectedValue);
        }

        switch (ddlInclude.SelectedValue)
        {
            case "R":
                Where += "And IsNull(r.IncludeInBookFlg, 0) = 1 ";
                break;

            case "A":
                break;
        }

        return Where;
    }

    protected void btnCreate_Click(object sender, EventArgs e)
	{
        fGenerating = true;
		CreateRecipeBook();
	}

    protected void CreateRecipeBook()
    {
        Byte[] data = (new PdfRecipeBook(Page)).Create(Filter());
        if (data != null)
        {
            // view result
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            //Response.Buffer = true;

            // Prevent this page from being cached.
            //  NOTE: we cannot use the CacheControl property, or set the PRAGMA header value due to a flaw re: PDF/SSL/IE
            Response.Expires = -1;

            Response.ContentType = "application/pdf";
            Response.AppendHeader("content-length", data.Length.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename='RecipeBook.pdf'");
            Response.BinaryWrite(data);
            Response.Flush();
            Response.Close();
        }
    }
}