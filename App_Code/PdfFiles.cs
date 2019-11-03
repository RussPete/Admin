using Globals;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
    /// <summary>
    /// Summary description for PdfFiles
    /// </summary>
    public class PdfFiles
    {
        public PdfFiles()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Proposal

        public static MemoryStream Proposal(Page Page, int JobRno)
        {
            MemoryStream MemoryStream = new MemoryStream();
            DB db = null;

            string Sql = string.Empty;
            try
            {
                //Create PDF document 
                Document Doc = new Document(PageSize.LETTER, 75, 95, 45, 60);
                PdfWriter PdfWriter = PdfWriter.GetInstance(Doc, MemoryStream);
                Doc.Open();

                string ImagePath = Page.Server.MapPath(@"~/Images");

                // fonts
                //BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                //BaseFont bfBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false);
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                BaseFont bfBold = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);                
                Font fntPhone = new Font(bf, 8, Font.NORMAL, BaseColor.GRAY);
                Font fntNormal = new Font(bf, 10, Font.NORMAL, BaseColor.BLACK);
                Font fntNormalBold = new Font(bfBold, 10, Font.NORMAL, BaseColor.BLACK);

                // logo
                //------------------------------------------------------------
                Image imgLogo = Image.GetInstance(ImagePath + "/MarvellLogoTrans.png");
                imgLogo.Alignment = Image.ALIGN_RIGHT;
                imgLogo.ScaleToFit(100f, 123f);

                PdfPTable tbl = new PdfPTable(1);
                tbl.DefaultCell.BorderWidth = 0;
                tbl.SetWidths(new float[] { 1 });

                PdfPCell cell = new PdfPCell(imgLogo);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                tbl.AddCell(cell);

                tbl.AddCell(new PdfPCell(new Paragraph(new Chunk("801-374-0879\n ", fntPhone))) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });

                Doc.Add(tbl);

                // job data
                //------------------------------------------------------------

                db = new DB();

                Sql = string.Format(
                    "Select c.Name, c.Phone, c.Email, cu.Name as Customer, j.ProposalFlg, j.JobDate, j.GuestArrivalTime, j.MealTime, j.EndTime, " +
                    "j.NumMenServing, j.NumWomenServing, j.NumChildServing, j.ServiceType, j.Location, j.PONumber, " +
                    "j.ServicePropDesc, j.DeliveryPropDesc, j.ChinaPropDesc, " +
                    "j.AddServicePropDesc, j.FuelTravelPropDesc, j.FacilityPropDesc, j.RentalsPropDesc, j.Adj1PropDesc, j.Adj2PropDesc, " +
                    "cu.TaxExemptFlg, j.SubTotTaxPct, j.EstTotPropDesc, j.DepositPropDesc, j.JobType, j.PmtTerms, j.PropCreatedDtTm " +
                    "From mcJobs j " +
                    "Left Join Contacts c on j.ContactRno = c.ContactRno " +
                    "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
                    "Where j.JobRno = {0}", 
                    JobRno);
                DataRow dr = db.DataRow(Sql);
                if (dr == null)
                {
                    // job not found
                    tbl = new PdfPTable(1);
                    tbl.DefaultCell.BorderWidth = 0;
                    tbl.WidthPercentage = 100;
                    tbl.SetWidths(new float[] { 1 });

                    Paragraph p = new Paragraph();
                    p.Add(new Chunk(string.Format("Job #{0} was not found in the database.", JobRno), fntNormalBold));
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(cell);

                    Doc.Add(tbl);
                }
                else
                {
                    tbl = new PdfPTable(1);
                    tbl.DefaultCell.BorderWidth = 0;
                    tbl.WidthPercentage = 100;
                    tbl.SetWidths(new float[] { 1 });

                    // contact information
                    //------------------------------------------------------------
                    string Contact = Str.Append(DB.Str(dr["Name"]), ", ", DB.Str(dr["Phone"]));
                    Contact = Str.Append(Contact, ", ", DB.Str(dr["Email"]));

                    Paragraph p = new Paragraph();
                    //p.Add(new Chunk("\n\n", fntNormal));
                    p.Add(new Chunk(string.Format("{0}\n", DB.Str(dr["Customer"])), fntNormal));
                    p.Add(new Chunk(string.Format("{0}\n", Contact), fntNormal));
                    p.Add(new Chunk(string.Format("Catering {0} from " + g.Company.Name + "\n", (DB.Bool(dr["ProposalFlg"]) ? "Proposal" : "Confirmation")), fntNormal));
                    p.Add(new Chunk(string.Format("Event #{0}\n", JobRno), fntNormal));
                    cell = new PdfPCell(p);
                    cell.SetLeading(12, 0);
                    cell.Border = 0;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthBottom = 0.5f;
                    cell.BorderColorBottom = BaseColor.GRAY;
                    tbl.AddCell(cell);

                    // time and palce
                    //------------------------------------------------------------
                    string[] Nth = { "th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th" };
                    DateTime JobDate = DB.DtTm(dr["JobDate"]);
                    DateTime Guests = DB.DtTm(dr["GuestArrivalTime"]);
                    DateTime Start = DB.DtTm(dr["MealTime"]);
                    DateTime End = DB.DtTm(dr["EndTime"]);

                    if (Guests != DateTime.MinValue && Guests < Start)
                    {
                        Start = Guests;
                    }

                    string EventTime;
                    if (End == DateTime.MinValue)
                    {
                        EventTime = Start.ToString("h:mm tt").ToLower();
                    }
                    else
                    {
                        EventTime = string.Format("{0} - {1}", Start.ToString("h:mm tt"), End.ToString("h:mm tt")).ToLower();
                    }

                    int NumServings = DB.Int32(dr["NumMenServing"]) + DB.Int32(dr["NumWomenServing"]) + DB.Int32(dr["NumChildServing"]);
                    string ServiceType = DB.Str(dr["ServiceType"]);
                    string sCell = string.Format(
                        "Date: {0}\n" +
                        "Location: {1}\n" +
                        "Event Time: {2}\n" +
                        "Guest Count: {3}\n" +
                        "Service Type: {4}\n",
                        Fmt.DtNth(JobDate),
                        DB.Str(dr["Location"]),
                        EventTime,
                        NumServings,
                        ServiceType);
                    string PONumber = DB.Str(dr["PONumber"]);
                    if (PONumber.Length > 0)
                    {
                        sCell += "PO #: " + PONumber + "\n";
                    }
                    sCell += "\n";
                    cell = new PdfPCell(new Paragraph(new Chunk(sCell, fntNormal)));

                    cell.SetLeading(12, 0);
                    cell.Border = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(cell);

                    // menu 
                    //------------------------------------------------------------
                    Sql = string.Format(
                        "Select * " +
                        "From mcJobFood " +
                        //"Where JobRno = {0} And (MenuItem <> '' Or MenuItem Is Null) And ProposalHideFlg = 0 " +
                        "Where JobRno = {0} And ProposalHideFlg = 0 " +
                        "Order By ProposalSeq, FoodSeq",
                        JobRno);
                    DataTable dt = db.DataTable(Sql);
                    foreach (DataRow drMenu in dt.Rows)
                    {
                        string ProposalMenuItem = DB.Str(drMenu["ProposalMenuItem"]);
                        bool fProposalTitle = DB.Bool(drMenu["ProposalTitleFlg"]);

                        if (fProposalTitle)
                        {
                            cell = new PdfPCell(new Paragraph(new Chunk(string.Format("\n{0}", ProposalMenuItem), fntNormalBold)));
                        }
                        else
                        {
                            cell = new PdfPCell(new Paragraph(new Chunk((ProposalMenuItem.Length > 0 ? ProposalMenuItem : "\n"), fntNormal)));
                            cell.PaddingLeft = 40;
                        }
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // prices
                    //------------------------------------------------------------
                    Sql = string.Format("Select * From JobInvoicePrices Where JobRno = {0} Order By Seq", JobRno);
                    dt = db.DataTable(Sql);
                    foreach (DataRow drPrice in dt.Rows)
                    {
                        string PriceType = DB.Str(drPrice["PriceType"]);
                        string Desc = string.Empty;

                        switch (PriceType)
                        {
                            case Misc.cnPerPerson:
                                Desc = DB.Str(drPrice["PropPerPersonDesc"]);
                                break;

                            case Misc.cnPerItem:
                                Desc = DB.Str(drPrice["PropPerItemDesc"]);
                                break;

                            case Misc.cnAllIncl:
                                Desc = DB.Str(drPrice["PropAllInclDesc"]);
                                break;
                        }

                        if (Desc.Length > 0)
                        {
                            cell = new PdfPCell(new Paragraph(new Chunk(string.Format("\n{0}", Desc), fntNormalBold)));
                            cell.Border = 0;
                            tbl.AddCell(cell);
                        }
                    }

                    // fees
                    //------------------------------------------------------------

                    string Fee;
                    string LF = "\n";

                    // services
                    Fee = DB.Str(dr["ServicePropDesc"]);
                    if (Fee.Length > 0)
                    {
                        cell = new PdfPCell(new Paragraph(new Chunk(LF + Fee, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // delivery
                    Fee = DB.Str(dr["DeliveryPropDesc"]);
                    if (Fee.Length > 0)
                    {
                        cell = new PdfPCell(new Paragraph(new Chunk(LF + Fee, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // china
                    Fee = DB.Str(dr["ChinaPropDesc"]);
                    if (Fee.Length > 0)
                    {
                        cell = new PdfPCell(new Paragraph(new Chunk(LF + Fee, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // additional services
                    Fee = DB.Str(dr["AddServicePropDesc"]);
                    if (Fee.Length > 0)
                    {
                        cell = new PdfPCell(new Paragraph(new Chunk(LF + Fee, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // fuel & travel
                    Fee = DB.Str(dr["FuelTravelPropDesc"]);
                    if (Fee.Length > 0)
                    {
                        cell = new PdfPCell(new Paragraph(new Chunk(LF + Fee, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // facility
                    Fee = DB.Str(dr["FacilityPropDesc"]);
                    if (Fee.Length > 0)
                    {
                        cell = new PdfPCell(new Paragraph(new Chunk(LF + Fee, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // rentals
                    Fee = DB.Str(dr["RentalsPropDesc"]);
                    if (Fee.Length > 0)
                    {
                        cell = new PdfPCell(new Paragraph(new Chunk(LF + Fee, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // adjustment 1
                    Fee = DB.Str(dr["Adj1PropDesc"]);
                    if (Fee.Length > 0)
                    {
                        cell = new PdfPCell(new Paragraph(new Chunk(LF + Fee, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // adjustment 2
                    Fee = DB.Str(dr["Adj2PropDesc"]);
                    if (Fee.Length > 0)
                    {
                        cell = new PdfPCell(new Paragraph(new Chunk(LF + Fee, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // sales tax
                    //------------------------------------------------------------
                    bool fTaxExempt = DB.Bool(dr["TaxExemptFlg"]);
                    if (!fTaxExempt)
                    {
                        string Tax = string.Format("\nUtah Food Sales Tax {0:0.00}%", DB.Dec(dr["SubTotTaxPct"]));
                        cell = new PdfPCell(new Paragraph(new Chunk(Tax, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // total
                    //------------------------------------------------------------
                    string TotalDesc = string.Format("\n{0}", DB.Str(dr["EstTotPropDesc"]));
                    cell = new PdfPCell(new Paragraph(new Chunk(TotalDesc, fntNormalBold)));
                    cell.Border = 0;
                    tbl.AddCell(cell);

                    // deposit
                    //------------------------------------------------------------
                    string DepositDesc = DB.Str(dr["DepositPropDesc"]);
                    if (DepositDesc.Length > 0)
                    {
                        cell = new PdfPCell(new Paragraph(new Chunk(LF + DepositDesc, fntNormalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                    }

                    // payment options
                    //------------------------------------------------------------
                    string JobType = DB.Str(dr["JobType"]);
                    string PaymentTerms = (JobType == "Corporate" ? DB.Str(dr["PmtTerms"]) : string.Empty).ToUpper();
                    string[] PaymentOptions = (DB.Str(dr["JobType"]) == "Private" ? Misc.PrivatePaymentOptions : Misc.CorporatePaymentOptions);
                    string Seperator = LF + LF;
                    foreach (string Msg in PaymentOptions)
                    {
                        if (Msg.Length > 0)
                        {
                            string FmtMsg = string.Format(Msg, PaymentTerms);
                            cell = new PdfPCell(new Paragraph(new Chunk(Seperator + FmtMsg, fntNormalBold)));
                            cell.Border = 0;
                            tbl.AddCell(cell);
                            Seperator = LF;
                        }
                    }

                    // phone #
                    //------------------------------------------------------------
                    //cell = new PdfPCell(new Paragraph(new Chunk(LF + "Phone #: 801-374-0879", fntNormal)));
                    //cell.Border = 0;
                    //tbl.AddCell(cell);

                    Doc.Add(tbl);

                    if (DB.DtTm(dr["PropCreatedDtTm"]) != DateTime.MinValue)
                    {
                        Sql = string.Format("Update mcJobs Set PropGeneratedDtTm = GetDate(), PropGeneratedUser = '{1}' Where JobRno = {0}", JobRno, g.User);
                        db.Exec(Sql);
                    }
                }

                //------------------------------------------------------------

                PdfWriter.CloseStream = false;
                Doc.Close();
                MemoryStream.Position = 0;
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex, Sql);
                //HttpContext.Current.Response.Write(Err.Html());
            }

            return MemoryStream;
        }

        #endregion Proposal

        #region Invoice

        public static MemoryStream Invoice(Page Page, int JobRno, bool fDepositInvoice = false, decimal DepositAmt = 0)
        {
            MemoryStream MemoryStream = new MemoryStream();
            string Sql = string.Empty;

            try
            {
                //Create PDF document 
                Document Doc = new Document(PageSize.LETTER, 30, 30, 30, 30);
                PdfWriter PdfWriter = PdfWriter.GetInstance(Doc, MemoryStream);
                Doc.Open();

                string ImagePath = Page.Server.MapPath(@"~/Images");

                // fonts
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                Font fntLargeLabel = new Font(bf, 14, Font.NORMAL, BaseColor.GRAY);
                Font fntLargeNormal = new Font(bf, 14, Font.NORMAL, BaseColor.BLACK);
                Font fntLargeBold = new Font(bf, 14, Font.BOLD, BaseColor.BLACK);
                Font fntLabel = new Font(bf, 11, Font.NORMAL, BaseColor.GRAY);
                Font fntNormal = new Font(bf, 11, Font.NORMAL, BaseColor.BLACK);
                Font fntNormalBold = new Font(bf, 11, Font.BOLD, BaseColor.BLACK);
                Font fntTotalBold = new Font(bf, 12, Font.BOLD, BaseColor.BLACK);
                Font fntPayableNote = new Font(bf, 11, Font.BOLD, BaseColor.BLACK);
                Font fntPayableBold = new Font(bf, 9, Font.BOLD, BaseColor.BLACK);
                Font fntPayable = new Font(bf, 9, Font.NORMAL, BaseColor.BLACK);
                Font fntNote = new Font(bf, 13, Font.BOLD, BaseColor.WHITE);

                BaseColor LineColor = new BaseColor(0x667B3E);

                // table
                PdfPTable tbl = new PdfPTable(4);
                tbl.DefaultCell.BorderWidth = 0;
                tbl.WidthPercentage = 100;
                tbl.SetWidths(new float[] { 1, 3, 1, 1 });
                //------------------------------------------------------------

                DB db = new DB();

                Sql = "Select cu.Name as Customer, c.Name, c.Phone, c.Cell, c.Fax, c.Email, " +
                    "j.Location, j.JobDate, j.PONumber, cu.TaxExemptFlg, j.SubTotTaxPct, j.ServiceTaxPct, j.DeliveryTaxPct, j.ChinaTaxPct, j.AddServiceTaxPct, " +
                    "j.FuelTravelTaxPct, j.FacilityTaxPct, j.GratuityTaxPct, j.RentalsTaxPct, j.Adj1TaxPct, j.Adj2TaxPct, j.CCFeeTaxPct, j.CCFeePct, " +
                    "j.InvEventCount, j.InvPricePerPerson, j.SubTotAmt, j.ServiceAmt, j.DeliveryAmt, j.ChinaAmt, j.AddServiceAmt, j.FuelTravelAmt, " +
                    "j.FacilityAmt, j.GratuityAmt, j.VoluntaryGratuityAmt, j.RentalsAmt, j.RentalsDesc, j.Adj1Desc, j.Adj2Desc, j.Adj1Amt, j.Adj2Amt, j.CCFeeAmt, j.InvBalAmt, " +
                    "j.ServiceSubTotPctFlg, j.ServiceSubTotPct, j.DeliverySubTotPctFlg, j.DeliverySubTotPct, j.DeliverySubTotPct, " +
                    "j.ChinaSubTotPctFlg, j.ChinaSubTotPct, j.AddServiceSubTotPctFlg, j.AddServiceSubTotPct, j.FuelTravelSubTotPctFlg, j.FuelTravelSubTotPct, " +
                    "j.FacilitySubTotPctFlg, j.FacilitySubTotPct, j.GratuitySubTotPctFlg, j.GratuitySubTotPct, j.VoluntaryGratuitySubTotPctFlg, j.VoluntaryGratuitySubTotPct, j.RentalsSubTotPctFlg, j.RentalsSubTotPct, " +
                    "j.Adj1SubTotPctFlg, j.Adj1SubTotPct, j.Adj2SubTotPctFlg, j.Adj2SubTotPct, j.CCPmtFeeFlg, j.CCFeePct, j.PreTaxSubTotAmt, j.SalesTaxTotAmt, " +
                    "j.InvTotAmt, j.JobType, j.PmtTerms, j.JobRno " +
                    "From mcJobs j " +
                    "Left Join Contacts c on j.ContactRno = c.ContactRno " +
                    "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
                    "Where j.JobRno = " + 
                    JobRno;
                DataTable dt = db.DataTable(Sql);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    DataTable dtPrices = db.DataTable("Select * From JobInvoicePrices Where JobRno = " + JobRno + " Order By Seq");

                    string sCustomer = DB.Str(dr["Customer"]);
                    string sName = DB.Str(dr["Name"]);
                    string sPhone = DB.Str(dr["Phone"]);
                    string sCell = DB.Str(dr["Cell"]);
                    string sFax = DB.Str(dr["Fax"]);
                    string sEmail = DB.Str(dr["Email"]);
                    string sContactInfo = sName +
                        (sPhone.Length > 0 ? ", " + sPhone : "") +
                        (sCell.Length > 0 ? ", " + sCell : "") +
                        (sEmail.Length > 0 ? ", " + sEmail : "");
                    string sLocation = DB.Str(dr["Location"]);
                    string sEventDate = Fmt.Dt(DB.DtTm(dr["JobDate"]));
                    string sPONumber = DB.Str(dr["PONumber"]);

                    bool fTaxExempt = DB.Bool(dr["TaxExemptFlg"]);
                    decimal SubtotalTaxPct = DB.Dec(dr["SubTotTaxPct"]);
                    decimal ServiceTaxPct = DB.Dec(dr["ServiceTaxPct"]);
                    decimal DeliveryTaxPct = DB.Dec(dr["DeliveryTaxPct"]);
                    decimal ChinaTaxPct = DB.Dec(dr["ChinaTaxPct"]);
                    decimal AddServiceTaxPct = DB.Dec(dr["AddServiceTaxPct"]);
                    decimal FuelTravelTaxPct = DB.Dec(dr["FuelTravelTaxPct"]);
                    decimal FacilityTaxPct = DB.Dec(dr["FacilityTaxPct"]);
                    decimal GratuityTaxPct = DB.Dec(dr["GratuityTaxPct"]);
                    decimal RentalsTaxPct = DB.Dec(dr["RentalsTaxPct"]);
                    decimal Adj1TaxPct = DB.Dec(dr["Adj1TaxPct"]);
                    decimal Adj2TaxPct = DB.Dec(dr["Adj2TaxPct"]);
                    decimal CCFeeTaxPct = DB.Dec(dr["CCFeeTaxPct"]);
                    decimal CCFeePct = DB.Dec(dr["CCFeePct"]);

                    Int32 EventCount = DB.Int32(dr["InvEventCount"]);
                    decimal EventPrice = DB.Dec(dr["InvPricePerPerson"]);
                    //decimal Subtotal = EventCount * EventPrice;

                    if (dtPrices.Rows.Count > 0)
                    {
                        //Subtotal = 0;

                        foreach (DataRow drPrice in dtPrices.Rows)
                        {
                            string PriceType = DB.Str(drPrice["PriceType"]);
                            int Count = 0;
                            decimal Price = 0;
                            decimal ExtPrice = 0;

                            switch (PriceType)
                            {
                                case Misc.cnPerPerson:
                                    Count = DB.Int32(drPrice["InvPerPersonCount"]);
                                    Price = DB.Dec(drPrice["InvPerPersonPrice"]);
                                    //ExtPrice = Count * Price;
                                    ExtPrice = DB.Dec(drPrice["InvPerPersonExtPrice"]);
                                    break;

                                case Misc.cnPerItem:
                                    Count = DB.Int32(drPrice["InvPerItemCount"]);
                                    Price = DB.Dec(drPrice["InvPerItemPrice"]);
                                    //ExtPrice = Count * Price;
                                    ExtPrice = DB.Dec(drPrice["InvPerItemExtPrice"]);
                                    break;

                                case Misc.cnAllIncl:
                                    Count = 1;
                                    Price = DB.Dec(drPrice["InvAllInclPrice"]);
                                    //ExtPrice = Price - Price * (1 - 1 / (1 + (fTaxExempt ? 0 : SubtotalTaxPct) / 100));
                                    ExtPrice = DB.Dec(drPrice["InvAllInclExtPrice"]);
                                    break;

                                default:
                                    break;
                            }
                            //Subtotal += ExtPrice; ;
                        }
                    }
                    decimal Subtotal = DB.Dec(dr["SubTotAmt"]);
                    decimal OrigSubtotal = Subtotal;
                    string sSubtotal = Fmt.Dollar(Subtotal);

                    decimal Service           = DB.Dec(dr["ServiceAmt"]);
                    decimal Delivery          = DB.Dec(dr["DeliveryAmt"]);
                    decimal China             = DB.Dec(dr["ChinaAmt"]);
                    decimal AddService        = DB.Dec(dr["AddServiceAmt"]);
                    decimal FuelTravel        = DB.Dec(dr["FuelTravelAmt"]);
                    decimal Facility          = DB.Dec(dr["FacilityAmt"]);
                    decimal Gratuity          = DB.Dec(dr["GratuityAmt"]);
                    decimal VoluntaryGratuity = DB.Dec(dr["VoluntaryGratuityAmt"]);
                    decimal Rentals           = DB.Dec(dr["RentalsAmt"]);
                    string RentalsDesc        = DB.Str(dr["RentalsDesc"]);
                    string Adj1Desc           = DB.Str(dr["Adj1Desc"]);
                    string Adj2Desc           = DB.Str(dr["Adj2Desc"]);
                    decimal Adj1Amt           = DB.Dec(dr["Adj1Amt"]);
                    decimal Adj2Amt           = DB.Dec(dr["Adj2Amt"]);
                    decimal CCFee             = DB.Dec(dr["CCFeeAmt"]);
                    decimal Balance           = DB.Dec(dr["InvBalAmt"]);

                    //string PmtType1 = DB.Str(dr["PmtType1"]);
                    //string PmtType2 = DB.Str(dr["PmtType2"]);
                    //string PmtType3 = DB.Str(dr["PmtType3"]);
                    //decimal PmtAmt1 = DB.Dec(dr["PmtAmt1"]);
                    //decimal PmtAmt2 = DB.Dec(dr["PmtAmt2"]);
                    //decimal PmtAmt3 = DB.Dec(dr["PmtAmt3"]);
                    //string PmtRef1 = DB.Str(dr["PmtRef1"]);
                    //string PmtRef2 = DB.Str(dr["PmtRef2"]);
                    //string PmtRef3 = DB.Str(dr["PmtRef3"]);
                    //string PmtMethod1 = DB.Str(dr["PmtMethod1"]);
                    //string PmtMethod2 = DB.Str(dr["PmtMethod2"]);
                    //string PmtMethod3 = DB.Str(dr["PmtMethod3"]);
                    //decimal CCReqAmt1 = DB.Dec(dr["CCReqAmt1"]);
                    //decimal CCReqAmt2 = DB.Dec(dr["CCReqAmt2"]);
                    //decimal CCReqAmt3 = DB.Dec(dr["CCReqAmt3"]);

                    //decimal CCFee1 = 0;
                    //decimal CCFee2 = 0;
                    //decimal CCFee3 = 0;
                    //decimal CCFeeBalance = 0;

                    //if (PmtMethod1 == "CreditCard")
                    //{
                    //	CCFee1 = Math.Round(CCReqAmt1 * CCFeePct / 100, 2);
                    //}
                    //if (PmtMethod2 == "CreditCard")
                    //{
                    //	CCFee2 = Math.Round(CCReqAmt2 * CCFeePct / 100, 2);
                    //}
                    //if (PmtMethod3 == "CreditCard")
                    //{
                    //	CCFee3 = Math.Round(CCReqAmt3 * CCFeePct / 100, 2);
                    //}

                    //decimal DepositAmt = DB.Dec(dr["DepAmt"]);
                    //string sDepositDesc = DB.Str(dr["DepRef"]);
                    //sDepositDesc = (sDepositDesc.Length > 0 ? string.Format("Deposit: {0}", sDepositDesc) : string.Empty);

                    //decimal SalesTax = Subtotal   * SubtotalTaxPct
                    //				 + Service    * ServiceTaxPct
                    //				 + Delivery   * DeliveryTaxPct
                    //				 + China      * ChinaTaxPct
                    //				 + AddService * AddServiceTaxPct
                    //				 + FuelTravel * FuelTravelTaxPct
                    //				 + Facility	  * FacilityTaxPct
                    //				 + Gratuity	  * GratuityTaxPct
                    //				 + Rentals	  * RentalsTaxPct
                    //				 + Adj1Amt	  * Adj1TaxPct
                    //				 + Adj2Amt	  * Adj2TaxPct
                    //				 + CCFee1	  * CCFeeTaxPct
                    //				 + CCFee2	  * CCFeeTaxPct
                    //				 + CCFee3	  * CCFeeTaxPct;
                    //SalesTax /= 100;
                    //if (fTaxExempt)
                    //{
                    //	SalesTax = 0;
                    //}

                    //Subtotal += Service
                    //		 + Delivery
                    //		 + China
                    //		 + AddService
                    //		 + FuelTravel
                    //		 + Facility
                    //		 + Gratuity
                    //		 + Rentals
                    //		 + Adj1Amt
                    //		 + Adj2Amt
                    //		 + CCFee1
                    //		 + CCFee2
                    //		 + CCFee3;

                    //decimal Total = Subtotal + SalesTax;
                    //decimal Balance = Total - (PmtAmt1 + PmtAmt2 + PmtAmt3);

                    //bool fChargeCCFee = DB.Bool(dr["CCPmtFeeFlg"]);
                    //if (fChargeCCFee)
                    //{
                    //	decimal Fee = Balance * CCFeePct / 100;
                    //	decimal Tax = Fee * CCFeeTaxPct / 100;

                    //	if (fTaxExempt)
                    //	{
                    //		Tax = 0;
                    //	}

                    //	CCFeeBalance += Fee;
                    //	Subtotal += Fee;
                    //	SalesTax += Tax;
                    //	Total += Fee + Tax;
                    //	Balance += Fee + Tax;
                    //}

                    string sServiceSubTotPct    = (DB.Bool(dr["ServiceSubTotPctFlg"])    ? Fmt.Pct(DB.Dec(dr["ServiceSubTotPct"]),    3, false) : "");
                    string sDeliverySubTotPct   = (DB.Bool(dr["DeliverySubTotPctFlg"])   ? Fmt.Pct(DB.Dec(dr["DeliverySubTotPct"]),   3, false) : "");
                    string sChinaSubTotPct      = (DB.Bool(dr["ChinaSubTotPctFlg"])      ? Fmt.Pct(DB.Dec(dr["ChinaSubTotPct"]),      3, false) : "");
                    string sAddServiceSubTotPct = (DB.Bool(dr["AddServiceSubTotPctFlg"]) ? Fmt.Pct(DB.Dec(dr["AddServiceSubTotPct"]), 3, false) : "");
                    string sFuelTravelSubTotPct = (DB.Bool(dr["FuelTravelSubTotPctFlg"]) ? Fmt.Pct(DB.Dec(dr["FuelTravelSubTotPct"]), 3, false) : "");
                    string sFacilitySubTotPct   = (DB.Bool(dr["FacilitySubTotPctFlg"])   ? Fmt.Pct(DB.Dec(dr["FacilitySubTotPct"]),   3, false) : "");
                    string sGratuitySubTotPct   = (DB.Bool(dr["GratuitySubTotPctFlg"])   ? Fmt.Pct(DB.Dec(dr["GratuitySubTotPct"]),   3, false) : "");
                    string sRentalsSubTotPct    = (DB.Bool(dr["RentalsSubTotPctFlg"])    ? Fmt.Pct(DB.Dec(dr["RentalsSubTotPct"]),    3, false) : "");
                    string sAdj1SubTotPct       = (DB.Bool(dr["Adj1SubTotPctFlg"])       ? Fmt.Pct(DB.Dec(dr["Adj1SubTotPct"]),       3, false) : "");
                    string sAdj2SubTotPct       = (DB.Bool(dr["Adj2SubTotPctFlg"])       ? Fmt.Pct(DB.Dec(dr["Adj2SubTotPct"]),       3, false) : "");
                    string sCCFeePct            = (DB.Bool(dr["CCPmtFeeFlg"])            ? Fmt.Pct(DB.Dec(dr["CCFeePct"]),            3, false) : "");

                    string sServiceAmt           = Fmt.Dollar(Service,           false);
                    string sDeliveryAmt          = Fmt.Dollar(Delivery,          false);
                    string sChinaAmt             = Fmt.Dollar(China,             false);
                    string sAddServiceAmt        = Fmt.Dollar(AddService,        false);
                    string sFuelTravelAmt        = Fmt.Dollar(FuelTravel,        false);
                    string sFacilityAmt          = Fmt.Dollar(Facility,          false);
                    string sGratuityAmt          = Fmt.Dollar(Gratuity,          false);
                    string sVoluntaryGratuityAmt = Fmt.Dollar(VoluntaryGratuity, false);
                    string sRentalsAmt           = Fmt.Dollar(Rentals,           false);
                    string sAdj1Desc             = Adj1Desc;
                    string sAdj2Desc             = Adj2Desc;
                    string sAdj1Amt              = Fmt.Dollar(Adj1Amt, false);
                    string sAdj2Amt              = Fmt.Dollar(Adj2Amt, false);
                    //string sCCFeeAmt = Fmt.Dollar(CCFee1 + CCFee2 + CCFee3 + CCFeeBalance, false);
                    string sCCFeeAmt = (CCFee != 0 ? Fmt.Dollar(CCFee) : string.Empty);

                    string sSubtotalTaxPct   = Fmt.Pct(SubtotalTaxPct,   3, false);
                    string sServiceTaxPct    = Fmt.Pct(ServiceTaxPct,    3, false);
                    string sDeliveryTaxPct   = Fmt.Pct(DeliveryTaxPct,   3, false);
                    string sChinaTaxPct      = Fmt.Pct(ChinaTaxPct,      3, false);
                    string sAddServiceTaxPct = Fmt.Pct(AddServiceTaxPct, 3, false);
                    string sFuelTravelTaxPct = Fmt.Pct(FuelTravelTaxPct, 3, false);
                    string sFacilityTaxPct   = Fmt.Pct(FacilityTaxPct,   3, false);
                    string sGratuityTaxPct   = Fmt.Pct(GratuityTaxPct,   3, false);
                    string sRentalsTaxPct    = Fmt.Pct(RentalsTaxPct,    3, false);
                    string sAdj1TaxPct       = Fmt.Pct(Adj1TaxPct,       3, false);
                    string sAdj2TaxPct       = Fmt.Pct(Adj2TaxPct,       3, false);
                    string sCCFeeTaxPct      = Fmt.Pct(CCFeeTaxPct,      3, false);

                    decimal PreTaxSubtotal = DB.Dec(dr["PreTaxSubTotAmt"]);
                    string sPreTaxSubtotal = Fmt.Dollar(PreTaxSubtotal, false);
                    decimal SalesTax       = DB.Dec(dr["SalesTaxTotAmt"]);
                    string sSalesTax       = Fmt.Dollar(SalesTax);
                    decimal Total          = DB.Dec(dr["InvTotAmt"]);
                    string sTotal          = Fmt.Dollar(Total);
                    string sDeposit        = Fmt.Dollar(DepositAmt);
                    string sBalance        = Fmt.Dollar(Balance);

                    //------------------------------------------------------------
                    // logo
                    Image imgPot = Image.GetInstance(ImagePath + "/Pot3.gif");
                    imgPot.Alignment = Image.ALIGN_RIGHT;
                    imgPot.ScaleToFit(57f, 70f);

                    Image imgTitle = Image.GetInstance(ImagePath + "/HeaderWhtDrkInv.gif");
                    imgTitle.Alignment = Image.ALIGN_LEFT;
                    imgTitle.ScaleToFit(335f, 70f);

                    PdfPCell cell = (new PdfPCell(imgPot));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Border = 0;
                    tbl.AddCell(cell);
                    cell = new PdfPCell(imgTitle);
                    cell.Colspan = 3;
                    cell.Border = 0;
                    tbl.AddCell(cell);

                    // customer
                    tbl.AddCell(string.Empty);
                    Paragraph p = new Paragraph();
                    p.Add(new Chunk("Catering Services For: ", fntLargeLabel));
                    p.Add(new Chunk(sCustomer, fntLargeBold));
                    cell = new PdfPCell(p);
                    cell.Colspan = 2;
                    cell.Border = 0;
                    cell.MinimumHeight = 30;
                    cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    tbl.AddCell(cell);
                    tbl.AddCell(string.Empty);

                    // contact info
                    tbl.AddCell(string.Empty);
                    p = new Paragraph();
                    p.Add(new Chunk("Contact Info: ", fntLargeLabel));
                    p.Add(new Chunk(sContactInfo, fntLargeNormal));
                    cell = new PdfPCell(p);
                    cell.Colspan = 2;
                    cell.Border = 0;
                    cell.MinimumHeight = 25;
                    cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    tbl.AddCell(cell);
                    tbl.AddCell(string.Empty);

                    // event location
                    tbl.AddCell(string.Empty);
                    p = new Paragraph();
                    p.Add(new Chunk("Event Location: ", fntLabel));
                    p.Add(new Chunk(sLocation, fntNormal));
                    cell = new PdfPCell(p);
                    cell.Colspan = 2;
                    cell.Border = 0;
                    cell.MinimumHeight = 30;
                    cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    tbl.AddCell(cell);
                    tbl.AddCell(string.Empty);

                    // event date
                    tbl.AddCell(string.Empty);
                    p = new Paragraph();
                    p.Add(new Chunk("Event Date: ", fntLabel));
                    p.Add(new Chunk(sEventDate, fntNormal));
                    cell = new PdfPCell(p);
                    cell.Colspan = 2;
                    cell.Border = 0;
                    cell.MinimumHeight = 20;
                    cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    tbl.AddCell(cell);
                    tbl.AddCell(string.Empty);

                    // PO Number
                    if (sPONumber.Length > 0)
                    {
                        tbl.AddCell(string.Empty);
                        p = new Paragraph();
                        p.Add(new Chunk("PO #: ", fntLabel));
                        p.Add(new Chunk(sPONumber, fntNormal));
                        cell = new PdfPCell(p);
                        cell.Colspan = 2;
                        cell.Border = 0;
                        cell.MinimumHeight = 20;
                        cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                        tbl.AddCell(cell);
                        tbl.AddCell(string.Empty);
                    }

                    // space
                    cell = new PdfPCell();
                    cell.AddElement(new Chunk(string.Empty));
                    cell.Colspan = 4;
                    cell.Border = 0;
                    cell.MinimumHeight = 54;
                    tbl.AddCell(cell);

                    if (dtPrices.Rows.Count > 0)
                    {
                        Subtotal = 0;
                    }

                    if ((EventCount != 0 || EventPrice != 0) && dtPrices.Rows.Count == 0)
                    {
                        // single event price
                        tbl.AddCell(string.Empty);
                        tbl.AddCell(string.Format("{0} servings @ {1} per person", Fmt.Num(EventCount), Fmt.Dollar(EventPrice)));
                        cell = new PdfPCell(new Phrase(Fmt.Dollar(EventCount * EventPrice)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbl.AddCell(cell);
                        tbl.AddCell(string.Empty);
                    }

                    // multiple prices
                    foreach (DataRow drPrice in dtPrices.Rows)
                    {
                        string PriceType = DB.Str(drPrice["PriceType"]);
                        int Count = 0;
                        decimal Price = 0;
                        decimal ExtPrice = 0;
                        string Desc = "";
                        string ExtDesc = "";

                        switch (PriceType)
                        {
                            case Misc.cnPerPerson:
                                Count = DB.Int32(drPrice["InvPerPersonCount"]);
                                Price = DB.Dec(drPrice["InvPerPersonPrice"]);
                                ExtPrice = Count * Price;
                                Desc = DB.Str(drPrice["InvPerPersonDesc"]);
                                ExtDesc = string.Format("{0} {1} @ {2} per person",
                                    Fmt.Num(Count),
                                    Desc,
                                    Fmt.Dollar(Price));
                                break;

                            case Misc.cnPerItem:
                                Count = DB.Int32(drPrice["InvPerItemCount"]);
                                Price = DB.Dec(drPrice["InvPerItemPrice"]);
                                ExtPrice = Count * Price;
                                Desc = DB.Str(drPrice["InvPerItemDesc"]);
                                ExtDesc = string.Format("{0} {1} @ {2} per item",
                                    Fmt.Num(Count),
                                    Desc,
                                    Fmt.Dollar(Price));
                                break;

                            case Misc.cnAllIncl:
                                Count = 1;
                                Price = DB.Dec(drPrice["InvAllInclPrice"]);
                                ExtPrice = Price - Price * (1 - 1 / (1 + (fTaxExempt ? 0 : SubtotalTaxPct) / 100));
                                Desc = DB.Str(drPrice["InvAllInclDesc"]);
                                ExtDesc = string.Format("All inclusive {0} @ {1}",
                                    Desc,
                                    Fmt.Dollar(Price));
                                break;

                            default:
                                break;
                        }

                        tbl.AddCell(string.Empty);
                        tbl.AddCell(ExtDesc);
                        cell = new PdfPCell(new Phrase(Fmt.Dollar(ExtPrice)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Border = 0;
                        tbl.AddCell(cell);
                        tbl.AddCell(string.Empty);
                    }

                    InvoiceHorizontalLine(tbl, LineColor);

                    // subtotal
                    tbl.AddCell(string.Empty);
                    cell = new PdfPCell(new Phrase(new Chunk("Subtotal", fntNormalBold)));
                    cell.Border = 0;
                    tbl.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(sSubtotal, fntNormalBold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Border = 0;
                    tbl.AddCell(cell);
                    tbl.AddCell(string.Empty);

                    // little space after subtotal
                    if (sGratuityAmt.Length > 0 ||
                        sVoluntaryGratuityAmt.Length > 0 ||
                        sDeliveryAmt.Length > 0 ||
                        sRentalsAmt.Length > 0 ||
                        sAdj1Desc.Length > 0 || sAdj1Amt.Length > 0 ||
                        sAdj2Desc.Length > 0 || sAdj2Amt.Length > 0)
                    {
                        InvoiceSpace(tbl, 6);
                    }

                    // on-site service fee
                    if (sServiceAmt.Length > 0)
                    {
                        InvoiceItem(tbl, "On-Site Service Fee", sServiceAmt);
                    }

                    // delivery fee
                    if (sDeliveryAmt.Length > 0)
                    {
                        InvoiceItem(tbl, "Delivery", sDeliveryAmt);
                    }

                    // china
                    if (sChinaAmt.Length > 0)
                    {
                        InvoiceItem(tbl, "China", sChinaAmt);
                    }

                    // additional service fee
                    if (sAddServiceAmt.Length > 0)
                    {
                        InvoiceItem(tbl, "Additional Service Fee", sAddServiceAmt);
                    }

                    // fuel & travel
                    if (sFuelTravelAmt.Length > 0)
                    {
                        InvoiceItem(tbl, "Fuel & Travel", sFuelTravelAmt);
                    }

                    // venue fee
                    if (sFacilityAmt.Length > 0)
                    {
                        InvoiceItem(tbl, "Venue Fee", sFacilityAmt);
                    }

                    // rental
                    if (sRentalsAmt.Length > 0)
                    {
                        InvoiceItem(tbl, "Rentals " + (RentalsDesc.Length > 0 ? string.Format(" ({0})", RentalsDesc) : ""), sRentalsAmt);
                    }

                    // gratuity
                    if (sGratuityAmt.Length > 0 && !fDepositInvoice)
                    {
                        InvoiceItem(tbl, "Gratuity (for employee service, $25-50 standard per employee)", sGratuityAmt);
                    }
                    if (sVoluntaryGratuityAmt.Length > 0 && !fDepositInvoice)
                    {
                        InvoiceItem(tbl, "Voluntary Gratuity (optional for employee service, $25-50 standard per employee)", sVoluntaryGratuityAmt);
                    }

                    // adjustment 1
                    if (sAdj1Amt.Length > 0)
                    {
                        InvoiceItem(tbl, sAdj1Desc, sAdj1Amt);
                    }

                    // adjustment 2
                    if (sAdj2Amt.Length > 0)
                    {
                        InvoiceItem(tbl, sAdj2Desc, sAdj2Amt);
                    }

                    // credit card fee
                    if (sCCFeeAmt.Length > 0)
                    {
                        InvoiceItem(tbl, "Credit Card Fee", sCCFeeAmt);
                    }

                    InvoiceHorizontalLine(tbl, LineColor);

                    InvoiceItem(tbl, "Pre-Tax Total", sPreTaxSubtotal);
                    InvoiceItem(tbl, "Utah State Food Tax" + (fTaxExempt ? " (Tax Exempt)" : ""), sSalesTax);
                    InvoiceSpace(tbl, 5);

                    string PmtTerms = (DB.Str(dr["JobType"]) == "Corporate" ? DB.Str(dr["PmtTerms"]) : string.Empty);

                    // total
                    if (fDepositInvoice)
                    {
                        tbl.AddCell(string.Empty);
                    }
                    else
                    {
                        p = new Paragraph();
                        p.Add(new Chunk(string.Format("Invoice #{0}", DB.Int32(dr["JobRno"])), fntNormalBold));
                        cell = new PdfPCell(p);
                        cell.Border = 0;
                        cell.PaddingTop = 3;
                        tbl.AddCell(cell);
                    }
                    cell = new PdfPCell(new Phrase(new Chunk("Invoice Total", fntTotalBold)));
                    cell.Border = 0;
                    tbl.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(sTotal, fntTotalBold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Border = 0;
                    tbl.AddCell(cell);
                    tbl.AddCell(string.Empty);

                    if (fDepositInvoice)
                    {
                        InvoiceHorizontalLine(tbl, LineColor);

                        // deposit amount
                        p = new Paragraph();
                        p.Add(new Chunk(string.Format("Invoice #{0}", DB.Int32(dr["JobRno"])), fntNormalBold));
                        cell = new PdfPCell(p);
                        cell.Border = 0;
                        cell.PaddingTop = 3;
                        tbl.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Deposit Payment Due", fntTotalBold)));
                        cell.Border = 0;
                        tbl.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(sDeposit, fntTotalBold)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Border = 0;
                        tbl.AddCell(cell);
                        tbl.AddCell(string.Empty);
                    }

                    string PmtSql = string.Format("Select Type, Amount, Reference From Payments Where JobRno = {0} Order By Seq", JobRno);
                    DataTable dtPmt = null;
                    try
                    {
                        dtPmt = db.DataTable(PmtSql);
                    }
                    catch (Exception Ex)
                    {
                        Err Err = new Err(Ex);
                        HttpContext.Current.Response.Write(Err.Html());
                    }

                    //if (PmtAmt1 != 0 || PmtAmt2 != 0 || PmtAmt3 != 0)
                    if (dtPmt != null && dtPmt.Rows.Count > 0)
                    {
                        cell = new PdfPCell();
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(PmtTerms, fntNormalBold)));
                    }

                    cell.Border = 0;
                    tbl.AddCell(cell);
                    int iRowTotal = tbl.getLastCompletedRowIndex();

                    //if (DepositAmt != 0 || sDepositDesc.Length > 0)
                    //{
                    //	InvoiceItem(tbl, sDepositDesc, sDeposit);
                    //	InvoiceSpace(tbl, 5);

                    //if (PmtAmt1 != 0 || PmtAmt2 != 0 || PmtAmt3 != 0)
                    if (dtPmt != null && dtPmt.Rows.Count > 0)
                    {
                        InvoiceSpace(tbl, 5);
                    }

                    foreach (DataRow drPmt in dtPmt.Rows)
                    {
                        string PmtType = DB.Str(drPmt["Type"]);
                        decimal PmtAmt = DB.Dec(drPmt["Amount"]);
                        string PmtRef = DB.Str(drPmt["Reference"]);

                        if (PmtAmt != 0)
                        {
                            InvoicePayment(tbl, PmtType, PmtRef, Fmt.Dollar(PmtAmt), fntPayable);
                        }
                    }

                    //if (PmtAmt1 != 0)
                    //{
                    //    InvoicePayment(tbl, PmtType1, PmtRef1, Fmt.Dollar(PmtAmt1), fntPayable);
                    //}
                    //if (PmtAmt2 != 0)
                    //{
                    //    InvoicePayment(tbl, PmtType2, PmtRef2, Fmt.Dollar(PmtAmt2), fntPayable);
                    //}
                    //if (PmtAmt3 != 0)
                    //{
                    //    InvoicePayment(tbl, PmtType3, PmtRef3, Fmt.Dollar(PmtAmt3), fntPayable);
                    //}

                    //if (PmtAmt1 != 0 || PmtAmt2 != 0 || PmtAmt3 != 0)
                    if (dtPmt != null && dtPmt.Rows.Count > 0)
                    {
                        InvoiceSpace(tbl, 5);

                        // balance
                        tbl.AddCell(string.Empty);
                        cell = new PdfPCell(new Phrase(new Chunk("Invoice Balance", fntTotalBold)));
                        tbl.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(sBalance, fntTotalBold)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbl.AddCell(cell);

                        cell = new PdfPCell(new Phrase(new Chunk(PmtTerms, fntNormalBold)));
                        tbl.AddCell(cell);
                        iRowTotal = tbl.getLastCompletedRowIndex();
                    }

                    InvoiceMarkBalance(tbl.Rows[iRowTotal], LineColor);

                    if (sGratuityAmt.Length == 0 && sVoluntaryGratuityAmt.Length == 0 && !fDepositInvoice)
                    {
                        InvoiceSpace(tbl, 12);
                        InvoiceGratuity(tbl, 0.15M, OrigSubtotal, Balance);
                        InvoiceSpace(tbl, 2);
                        InvoiceGratuity(tbl, 0.2M, OrigSubtotal, Balance);
                    }

                    Doc.Add(tbl);

                    // payment info box
                    const float PaymentBorderWidth = 0.85f;
                    tbl = new PdfPTable(1);
                    tbl.TotalWidth = 505f;
                    tbl.SetWidths(new float[] { 1f });

                    cell = new PdfPCell();
                    cell.MinimumHeight = 10;
                    tbl.AddCell(cell);
                    cell = new PdfPCell(new Phrase("For paying by check, please make payable to", fntPayable));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Border = 0;
                    tbl.AddCell(cell);
                    cell = new PdfPCell(new Phrase(string.Format(g.Company.Name + " and reference Invoice #{0}.", DB.Int32(dr["JobRno"])), fntPayableNote));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell.MinimumHeight = 20;
                    tbl.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Mail to: " + g.Company.Address, fntPayable));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.MinimumHeight = 20;
                    tbl.AddCell(cell);
                    cell = new PdfPCell(new Phrase("For credit card payments please call " + g.Company.Phone + ".", fntPayableNote));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(cell);
                    cell = new PdfPCell(new Phrase(Globals.g.Company.CreditCardPmt, fntPayable));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.MinimumHeight = 20;
                    tbl.AddCell(cell);
                    cell = new PdfPCell(new Phrase("For billing inquiries please call " + g.Company.Phone, fntPayable));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(cell);

                    // set border color
                    for (int i = 0; i <= tbl.getLastCompletedRowIndex(); i++)
                    {
                        PdfPRow Row = tbl.GetRow(i);
                        cell = Row.GetCells()[0];
                        cell.PaddingRight = 10;
                        cell.Border = 0;
                        cell.BorderColor = LineColor;
                        cell.BorderWidthLeft =
                        cell.BorderWidthRight = PaymentBorderWidth;
                    }

                    cell = tbl.GetRow(0).GetCells()[0];
                    cell.BorderWidthTop = PaymentBorderWidth;
                    cell = tbl.GetRow(tbl.getLastCompletedRowIndex()).GetCells()[0];
                    cell.BorderWidthBottom = PaymentBorderWidth;

                    PdfContentByte cb = PdfWriter.DirectContent;
                    tbl.WriteSelectedRows(0, -1, 50f, 130f, cb);


                    // Take Note
                    tbl = new PdfPTable(1);
                    tbl.TotalWidth = 505f;
                    tbl.SetWidths(new float[] { 1f });

                    cell = new PdfPCell(new Phrase("PLEASE TAKE NOTE OF OUR NEW ADDRESS", fntNote));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell.MinimumHeight = 30;
                    cell.BackgroundColor = LineColor;
                    cell.Border = 0;
                    cell.PaddingTop = 0.5f;
                    cell.PaddingBottom = 4f;
                    tbl.AddCell(cell);
                    tbl.WriteSelectedRows(0, -1, 50f, 150f, cb);

                    Image imgLeftArrow = Image.GetInstance(ImagePath + "/red-arrow-left.png");
                    imgLeftArrow.ScalePercent(30.0f);
                    //imgLeftArrow.SetAbsolutePosition(130, 68);
                    imgLeftArrow.SetAbsolutePosition(110, 85);
                    Doc.Add(imgLeftArrow);

                    Image imgRightArrow = Image.GetInstance(ImagePath + "/red-arrow-right.png");
                    imgRightArrow.ScalePercent(30.0f);
                    //imgRightArrow.SetAbsolutePosition(405, 68);
                    imgRightArrow.SetAbsolutePosition(425, 85);
                    Doc.Add(imgRightArrow);
                }

                //------------------------------------------------------------

                PdfWriter.CloseStream = false;
                Doc.Close();
                MemoryStream.Position = 0;
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex, Sql);
                HttpContext.Current.Response.Write(Err.Html());
            }

            return MemoryStream;
        }

        protected static void InvoiceItem(PdfPTable tbl, string Desc, string Amt)
        {
            tbl.AddCell(string.Empty);
            tbl.AddCell(Desc);
            PdfPCell cell = new PdfPCell(new Phrase(Amt));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = 0;
            tbl.AddCell(cell);
            tbl.AddCell(string.Empty);
        }

        protected static void InvoiceHorizontalLine(PdfPTable tbl, BaseColor Color)
        {
            // horizontal line
            tbl.AddCell(string.Empty);
            PdfPCell cell = new PdfPCell();
            cell.Colspan = 2;
            cell.Border = 0;
            cell.BorderWidthBottom = 1;
            cell.BorderColorBottom = Color;
            cell.MinimumHeight = 10;
            tbl.AddCell(cell);
            tbl.AddCell(string.Empty);

            InvoiceSpace(tbl, 6);
        }

        protected static void InvoiceGratuity(PdfPTable tbl, decimal Percent, decimal Amount, decimal Balance)
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            Font fntNormal = new Font(bf, 10, Font.NORMAL, BaseColor.BLACK);
            Font fntNormalBold = new Font(bf, 9, Font.BOLD, BaseColor.BLACK);

            decimal Gratuity = Math.Round(Amount * Percent, 2);
            string sGratuity = Fmt.Dollar(Gratuity, true);
            string sGratuityBalance = Fmt.Dollar(Balance + Gratuity);

            tbl.AddCell(string.Empty);
            Paragraph p = new Paragraph();
            p.Add(new Chunk("Adjusted Total with ", fntNormal));
            p.Add(new Chunk(string.Format("{0} gratuity", Fmt.Pct(Percent * 100), fntNormalBold)));
            p.Add(new Chunk(string.Format(" ({0}):", sGratuity), fntNormal));
            PdfPCell cell = new PdfPCell(p);
            cell.Border = 0;
            tbl.AddCell(cell);
            cell = new PdfPCell(new Phrase(sGratuityBalance, fntNormal));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = 0;
            tbl.AddCell(cell);
            tbl.AddCell(string.Empty);
        }

        protected static void InvoiceSpace(PdfPTable tbl, float Height)
        {
            PdfPCell cell = new PdfPCell();
            cell.Colspan = 4;
            cell.Border = 0;
            cell.MinimumHeight = Height;
            tbl.AddCell(cell);
        }

        protected static void InvoiceMarkBalance(PdfPRow Row, BaseColor LineColor)
        {
            const float BoxWidth = 1.5f;
            PdfPCell[] Cells = Row.GetCells();
            PdfPCell Cell;

            Cell = Cells[0];
            Cell.BorderWidth = 0;
            Cell.BorderColor = LineColor;
            Cell.BorderWidthTop =
            Cell.BorderWidthBottom =
            Cell.BorderWidthLeft = BoxWidth;
            Cell.PaddingLeft = 5;

            Cell = Cells[Cells.Length - 1];
            Cell.BorderWidth = 0;
            Cell.BorderColor = LineColor;
            Cell.BorderWidthTop =
            Cell.BorderWidthRight =
            Cell.BorderWidthBottom = BoxWidth;
            Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Cell.PaddingRight = 10;
            Cell.PaddingTop = 3;

            for (int i = 1; i < Cells.Length - 1; i++)
            {
                Cell = Cells[i];
                Cell.BorderWidth = 0;
                Cell.BorderColor = LineColor;
                Cell.BorderWidthTop =
                Cell.BorderWidthBottom = BoxWidth;
                Cell.PaddingBottom = 7;
            }
        }

        protected static void InvoicePayment(PdfPTable tbl, string Type, string Desc, string Amt, Font DescFont)
        {
            tbl.AddCell(string.Empty);

            PdfPCell cell = new PdfPCell(new Phrase(Type + ": " + Desc, DescFont));
            cell.Border = 0;
            tbl.AddCell(cell);

            cell = new PdfPCell(new Phrase(Amt));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = 0;
            tbl.AddCell(cell);

            tbl.AddCell(string.Empty);
        }

        #endregion Invoice
    }
}
