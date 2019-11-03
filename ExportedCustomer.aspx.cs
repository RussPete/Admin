using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class ExportedCustomer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Int32 CustomerRno = Parm.Int("Customer");
        string ExportId = Parm.Str("ExportId");

        if (CustomerRno == 0) // || ExportId.Length == 0)
        {
            Response.Clear();
            Response.ContentType = "text/plain";
            if (CustomerRno == 0)
            {
                Response.Write("Missing customer number: ExportedCustomer.asx?Customer=nnn\n");
            }
            //if (ExportId.Length == 0)
            //{
            //    Response.Write("Missing export ID: ExportedCustomer.asx?ExportId=xxx\n");
            //}
            Response.End();
        }
        else
        {
            string Sql = string.Format("Select Count(*) From mcJobs Where JobRno = {0}", CustomerRno);
            try
            {
                using (DB db = new DB())
                {
                    int Count = db.SqlNum(Sql);
                    if (Count > 0)
                    {
                        //Sql = string.Format("Update mcJobs Set CustomerExportId = '{1}', CustomerExportDtTm = GetDate() Where JobRno = {0}", CustomerRno, ExportId);
                        Sql = string.Format("Update mcJobs Set CustomerExportId = '{1}' Where JobRno = {0}", CustomerRno, ExportId);
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
                        Response.Write("Customer not found");
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