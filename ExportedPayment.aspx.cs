using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class ExportedPayment : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Int32 PaymentRno = Parm.Int("Payment");
        string ExportId = Parm.Str("ExportId");

        if (PaymentRno == 0) // || ExportId.Length == 0)
        {
            Response.Clear();
            Response.ContentType = "text/plain";
            if (PaymentRno == 0)
            {
                Response.Write("Missing payment number: ExportedPayment.asx?Payment=nnn\n");
            }
            //if (ExportId.Length == 0)
            //{
            //    Response.Write("Missing export ID: ExportedPayment.asx?ExportId=xxx\n");
            //}
            Response.End();
        }
        else
        {
            string Sql = string.Format("Select Count(*) From Payments Where PaymentRno = {0}", PaymentRno);
            try
            {
                using (DB db = new DB())
                {
                    int Count = db.SqlNum(Sql);
                    if (Count > 0)
                    {

                        Sql = string.Format("Update Payments Set ExportId = '{1}',  ExportDtTm = GetDate() Where PaymentRno = {0}", PaymentRno, ExportId);
                        db.Exec(Sql);

                        Response.Clear();
                        Response.ContentType = "text/plain";
                        Response.Write("OK");
                        Response.Flush();
                    }
                    else
                    {
                        Response.Clear();
                        Response.ContentType = "text/plain";
                        Response.Write("Payment not found");
                        Response.Flush();
                    }
                }
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex, Sql);
                Response.Clear();
                Response.ContentType = "text/plain";
                Response.Write(Ex.ToString());
                Response.End();
            }
            Response.End();
        }
    }
}