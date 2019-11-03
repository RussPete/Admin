using Globals;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class UnpaidJobs : System.Web.UI.Page
{
    protected WebPage Pg;
    protected DB db;

    protected decimal Subtotal = 0;
    protected decimal OtherTotal = 0;
    protected decimal SalesTaxTotal = 0;
    protected decimal Total = 0;
    protected decimal Balance = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
        {
            Session["Menu"] = WebPage.Accounting.Title;
            //Setup();
        }
        else
        {
            DateTime Tm = DateTime.Now;

            // Paid In Full jobs
            int Count = Str.Num(Request.Params["Count"]);
            for (int i = 1; i <= Count; i++)
            {
                int JobRno = Str.Num(Request.Params["chkProc" + i]);
                if (JobRno > 0)
                {
                    string Sql = "Update mcJobs Set " +
                        "PaidInFullDtTm = " + DB.PutDtTm(Tm) + ", " +
                        "PaidInFullUser = " + DB.PutStr(g.User) + ", " +
                        "UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
                        "UpdatedUser = " + DB.PutStr(g.User) + " " +
                        "Where JobRno = " + JobRno;
                    db.Exec(Sql);
                }
            }
        }
    }

    protected string Jobs()
    {
        string Html = string.Empty;
        string Sql = string.Empty;

        try
        {
            decimal RptTotal = 0;
            decimal RptTotalBalance = 0;

            int iRow = 0;

            DateTime PrevDay = DateTime.MinValue;
            bool fFirstDay = true;

            string Where = "JobDate <= GetDate() and InvBalAmt > 0 and PaidInFullDtTm is null and IsNull(ProposalFlg, 0) = 0 and CancelledDtTm is null";
            Sql = "Select *, Coalesce(cu.Name, c.Name) as Customer " +
                "From mcJobs j " +
                "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " + 
                "Where " + Where + " " +
                "Order By JobDate, JobRno";
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                InvoiceAmounts(dr);

                DateTime JobDate        = DB.DtTm(dr["JobDate"]);        
                string Customer         = DB.Str(dr["Customer"]);
                DateTime? Final         = DB.DtTm(dr["FinalDtTm"]);
                DateTime? Processed     = DB.DtTm(dr["ProcessedDtTm"]);
                DateTime? EditProcessed = DB.DtTm(dr["EditProcessedDtTm"]);
                DateTime? Updated       = DB.DtTm(dr["InvUpdatedDtTm"]);

                if (Updated.HasValue && Updated.Value == DateTime.MinValue)
                    Updated = null;

                bool fUpdated = false;
                string Title = string.Empty;

                if (JobDate != PrevDay)
                {
                    if (!fFirstDay)
                    {
                        //Html += "\t\t</tbody>\n\t</table>\n";
                        Html += "\t\t</tbody>\n";
                    }
                    Html += string.Format(
                        //"<div class=\"FeatureSub\">{0:M/d/yyyy}</div>\n" +
                        //"\t<table class=\"Acct\">\n" +
                        "\t\t<thead>\n" +
                        "\t\t\t<tr><td colspan=\"10\"><div class=\"FeatureSub\">{0:M/d/yyyy}</div></td></tr>\n" +
                        "\t\t\t<tr>\n" +
                        "\t\t\t\t<th>Paid</th>\n" +
                        "\t\t\t\t<th style=\"white-space: nowrap;\">Inv #</th>\n" +
                        "\t\t\t\t<th>Name</th>\n" +
                        "\t\t\t\t<th>Date</th>\n" +
                        "\t\t\t\t<th>Location</th>\n" +
                        "\t\t\t\t<th>Subtotal</th>\n" +
                        "\t\t\t\t<th>Other</th>\n" +
                        "\t\t\t\t<th>Tax</th>\n" +
                        "\t\t\t\t<th>Total</th>\n" +
                        "\t\t\t\t<th>Balance</th>\n" +
                        "\t\t\t</tr>\n" +
                        "\t\t</thead>\n" +
                        "\t\t</tbody>\n",
                        JobDate
                    );
                    PrevDay = JobDate;
                    fFirstDay = false;
                }

                Html += string.Format(
                    "<tr class=\"{12}\">\n" +
                    "<td class=\"Center\"><input type=\"checkbox\" name=\"chkProc{0}\" value=\"{2}\" /></td>\n" +
                    "<td class=\"Right\" title=\"{13}\">{11}<a href=\"Invoice.aspx?JobRno={2}\" target=\"invoice\">{2}</a></td>\n" +
                    "<td>{3}</td>\n" +
                    "<td class=\"Center\">{4}</td>\n" +
                    "<td>{5}</td>\n" +
                    "<td class=\"Right\">{6}</td>\n" +
                    "<td class=\"Right\">{7}</td>\n" +
                    "<td class=\"Right\">{8}</td>\n" +
                    "<td class=\"Right\">{9}</td>\n" +
                    "<td class=\"Right\">{10}</td>\n" +
                    "</tr>\n",
                    ++iRow,
                    "",
                    DB.Int32(dr["JobRno"]),
                    DB.Str(dr["Customer"]),
                    Fmt.Dt(DB.DtTm(dr["JobDate"])),
                    DB.Str(dr["Location"]),
                    Fmt.Dollar(Subtotal),
                    Fmt.Dollar(OtherTotal),
                    Fmt.Dollar(SalesTaxTotal),
                    Fmt.Dollar(Total),
                    Fmt.Dollar(Balance),
                    (fUpdated ? "*" : ""),
                    (fUpdated ? "Updated" : ""),
                    Title);
                RptTotal += Math.Round(Total, 2);
                RptTotalBalance += Math.Round(Balance, 2);
            }

            Html += string.Format(
                "<tr>\n" +
                "<td><input type=\"hidden\" name=\"{0}Count\" value=\"{1}\"/></td>\n" +
                //"<td colspan=\"2\"><span style='color: Red;'>*</span><span style='font-size: 80%'>Updated</span></td>\n" +
                "<td colspan=\"2\"></td>\n" +
                "<td colspan=\"2\"></td>\n" +
                "<td align=\"right\"><b>Count</b></td>\n" +
                "<td align=\"right\"><b>{2}</b></td>\n" +
                "<td align=\"right\"><b>Total</b></td>\n" +
                "<td align=\"right\"><b>{3}</b></td>\n" +
                "<td align=\"right\"><b>{4}</b></td>\n" +
                "</tr>\n",
                "",
                iRow,
                dt.Rows.Count,
                Fmt.Dollar(RptTotal),
                Fmt.Dollar(RptTotalBalance));
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return Html;
    }

    protected void InvoiceAmounts(DataRow dr)
    {
        Subtotal        = DB.Dec(dr["SubTotAmt"]);
        OtherTotal      = DB.Dec(dr["PreTaxSubTotAmt"]) - Subtotal;
        SalesTaxTotal   = DB.Dec(dr["SalesTaxTotAmt"]);
        Total           = DB.Dec(dr["InvTotAmt"]);
        Balance         = DB.Dec(dr["InvBalAmt"]);
    }
}
