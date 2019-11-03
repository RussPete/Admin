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

public partial class DailyCCPmts : System.Web.UI.Page
{
    protected WebPage Pg;
    protected DB db;

    protected void Page_Load(object sender, EventArgs e)
    {
        db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
        {
            Session["Menu"] = WebPage.Accounting.Title;
            Setup();
        }
        else
        {
        }
    }

    protected void Setup()
    {
        txtStartDate.Text = DateTime.Today.AddDays(-1).ToString("M/d/yyyy");
    }

    protected string CCPayments()
    {
        string Html = string.Empty;
        string Sql = string.Empty;

        try
        {
            decimal RptTotal = 0;
            decimal DailyTotal = 0;
            int DailyCount = 0;

            DateTime PrevDay = DateTime.MinValue;
            bool fFirstDay = true;

            Sql = string.Format(
                "Select Cast(p.PaymentDt as Date) as PaymentDt, p.Amount, p.Type, p.Reference, p.CCStatus, p.CCResult, " +
                "p.CCPmtDtTm, p.CCPmtUser, p.CCPrintDtTm, p.CCPrintUser, p.JobRno, p.Seq, " +
                "Coalesce(cu.Name, c.Name) as Customer, j.JobDate, j.Location, j.InvTotAmt " +
                "From Payments p " +
                "Inner Join mcJobs j on p.JobRno = j.JobRno " +
                "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
                "Where PaymentDt Between {0} And GetDate() And Method = 'CreditCard' " +
                "Order By Cast(p.PaymentDt as Date), p.JobRno, p.Seq",
                DB.PutDtTm(Str.DtTm(txtStartDate.Text)));

            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                DateTime PmtDate        = DB.DtTm(dr["PaymentDt"]);
                decimal PmtAmt          = DB.Dec(dr["Amount"]);
                string sPmtAmt          = Fmt.Dollar(PmtAmt);
                string PmtType          = DB.Str(dr["Type"]);
                string PmtRef           = DB.Str(dr["Reference"]);
                string CCStatus         = DB.Str(dr["CCStatus"]);
                string CCResult         = DB.Str(dr["CCResult"]);
                DateTime CCPmtDtTm      = DB.DtTm(dr["CCPmtDtTm"]);
                string CCPmtUser        = DB.Str(dr["CCPmtUser"]);
                DateTime CCPrintDtTm    = DB.DtTm(dr["CCPrintDtTm"]);
                String CCPrintUser      = DB.Str(dr["CCPrintUser"]);
                Int32 JobRno            = DB.Int32(dr["JobRno"]);
                string Customer         = DB.Str(dr["Customer"]);
                DateTime JobDate        = DB.DtTm(dr["JobDate"]);        
                string Location         = DB.Str(dr["Location"]);
                decimal Total           = DB.Dec(dr["InvTotAmt"]);
                bool fDeclined          = (CCStatus.ToLower() == "declined");

                if (fDeclined)
                {
                    sPmtAmt = "** " + sPmtAmt;
                    CCStatus = "<b>" + CCStatus + "</b>";
                }

                if (PmtDate != PrevDay)
                {
                    if (!fFirstDay)
                    {
                        //Html += "\t\t</tbody>\n\t</table>\n";
                        Html += string.Format(
                            "<tr>\n" +
                            "<td align=\"right\"><b>{0}</b></td>\n" +
                            "<td><b>Total</b></td>\n" +
                            "<td align=\"right\"><b>{1}</b></td>\n" +
                            "<td><b>Count</b></td>\n" +
                            "<td colspan=\"7\"></td>\n" +
                            "</tr>\n",
                            Fmt.Dollar(DailyTotal),
                            DailyCount);
                        Html += "\t\t</tbody>\n";
                        DailyCount = 0;
                        DailyTotal = 0;
                    }
                    Html += string.Format(
                        //"<div class=\"FeatureSub\">{0:M/d/yyyy}</div>\n" +
                        //"\t<table class=\"Acct\">\n" +
                        "\t\t<thead>\n" +
                        "\t\t\t<tr><td colspan=\"11\"><div class=\"FeatureSub\">{0:M/d/yyyy}</div></td></tr>\n" +
                        "\t\t\t<tr>\n" +
                        "\t\t\t\t<th>Amount</th>\n" +
                        "\t\t\t\t<th>Type</th>\n" +
                        "\t\t\t\t<th>Reference</th>\n" +
                        "\t\t\t\t<th>Status</th>\n" +
                        "\t\t\t\t<th>Paid</th>\n" +
                        "\t\t\t\t<th>Printed</th>\n" +
                        "\t\t\t\t<th style=\"white-space: nowrap;\">Inv #</th>\n" +
                        "\t\t\t\t<th>Name</th>\n" +
                        "\t\t\t\t<th>Job Date</th>\n" +
                        "\t\t\t\t<th>Location</th>\n" +
                        "\t\t\t\t<th>Total</th>\n" +
                        "\t\t\t</tr>\n" +
                        "\t\t</thead>\n" +
                        "\t\t</tbody>\n",
                        PmtDate
                    );
                    PrevDay = PmtDate;
                    fFirstDay = false;
                }

                Html += string.Format(
                    "<tr class=\"{9}\">\n" +
                    "<td class=\"Right\">{0}</td>\n" +
                    "<td>{1}</td>\n" +
                    "<td>{2}</td>\n" +
                    "<td>{3}</td>\n" +
                    "<td>{4}</td>\n" +
                    "<td>{5}</td>\n" +
                    "<td class=\"Right\"><a href=\"Invoice.aspx?JobRno={6}\" target=\"invoice\">{6}</a></td>\n" +
                    "<td>{7}</td>\n" +
                    "<td class=\"Center\">{8}</td>\n" +
                    "<td>{9}</td>\n" +
                    "<td class=\"Right\">{10}</td>\n" +
                    "</tr>\n",
                    sPmtAmt,
                    PmtType,
                    PmtRef,
                    CCStatus,
                    string.Format("{0} - {1}", Fmt.Tm(CCPmtDtTm), CCPmtUser),
                    string.Format("{0} - {1}", Fmt.Tm(CCPrintDtTm), CCPrintUser),
                    JobRno,
                    Customer,
                    Fmt.Dt(JobDate),
                    Location,
                    Fmt.Dollar(Total),
                    (fDeclined ? "Declined" : string.Empty));

                DailyCount++;
                DailyTotal += Math.Round(PmtAmt, 2);
                RptTotal += Math.Round(PmtAmt, 2);
            }

            Html += string.Format(
                "<tr>\n" +
                "<td align=\"right\"><b>{0}</b></td>\n" +
                "<td><b>Total</b></td>\n" +
                "<td align=\"right\"><b>{1}</b></td>\n" +
                "<td><b>Count</b></td>\n" +
                "<td colspan=\"7\"></td>\n" +
                "</tr>\n",
                Fmt.Dollar(DailyTotal),
                DailyCount);
            Html += "\t\t</tbody>\n";

            Html += string.Format(
                "<tr><td colspan=\"11\">&nbsp;</td></tr>\n" +
                "<tr>\n" +
                "<td align=\"right\"><b>{0}</b></td>\n" +
                "<td><b>Total</b></td>\n" +
                "<td align=\"right\"><b>{1}</b></td>\n" +
                "<td><b>Count&nbsp;&nbsp;Report Totals</b></td>\n" +
                "<td colspan=\"7\"></td>\n" +
                "</tr>\n",
                Fmt.Dollar(RptTotal),
                dt.Rows.Count);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return Html;
    }
}
