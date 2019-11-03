using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Utils;
using System.Data;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class ExportData : System.Web.UI.Page
{
    public class Customer
    {
        public string ExportId;
        public Int32 id;
        public string Name;
        public string Contact;
        public string Phone;
        public string Cell;
        public string Email;
        public bool Company;
        public bool TaxExempt;
    }

    public class Invoice
    {
        public string ExportId;
        public Int32 id;
        public Int32 Number;
        public decimal SubTotal;
        public decimal Gratuity;
        public decimal SalesTaxRate;
        public decimal SalesTax;
        public decimal Total;
        public DateTime Date;
        public bool TaxExempt;
        public string CustomerExportId;
    }

    public class Payment
    {
        public string ExportId;
        public Int32 id;
        public Int32 InvoiceNumber;
        public Int32 Seq;
        public decimal Amount;
        public DateTime Date;
        public string Reference;
        public string Type;
        public string Method;
        public string CCType;
        public string CCAuth;
        public string CCReference;
        public string InvoiceExportId;
        public string CustomerExportId;
    }

    public class Data
    {
        public List<Customer> Customers;
        public List<Invoice> Invoices;
        public List<Payment> Payments;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Data Data = new Data()
        {
            Customers = new List<Customer>(),
            Invoices = new List<Invoice>(),
            Payments = new List<Payment>()
        };

        using (DB db = new DB())
        {
            string Sql =
                "Select c.ExportId, c.ContactRno as id, cu.Name as Customer, c.Name, " +
                "c.Phone, c.Cell, c.Email, " +
                "cu.CompanyFlg, cu.TaxExemptFlg " +
                //"From mcJobs Where (ExportDtTm Is Null Or InvUpdatedDtTm > ExportDtTm) and FinalDtTm Is Not Null Order By JobRno";
                "From Contacts c " +
                "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
                "Where ExportDtTm Is Null Order By ContactRno";

            try
            {
                DataTable dt = db.DataTable(Sql);
                foreach (DataRow dr in dt.Rows)
                {
                    Data.Customers.Add(new Customer()
                    {
                        ExportId    = DB.Str(dr["ExportId"]),
                        id          = DB.Int32(dr["id"]),
                        Name        = DB.Str(dr["Customer"]),
                        Contact     = DB.Str(dr["Name"]),
                        Phone       = DB.Str(dr["Phone"]),
                        Cell        = DB.Str(dr["Cell"]),
                        Email       = DB.Str(dr["Email"]),
                        Company     = DB.Bool(dr["CompanyFlg"]),
                        TaxExempt   = DB.Bool(dr["TaxExemptFlg"])
                    });
                }

                Sql =
                    "Select ExportId, JobRno as id, JobRno as Number, PreTaxSubTotAmt as SubTotal, GratuityAmt as Gratuity, " +
                    "SubTotTaxPct as SalesTaxRate, SalesTaxTotAmt as SalesTax, InvTotAmt as Total, IsNull(InvoicedDtTm, InvCreatedDtTm) as InvoicedDtTm, " +
                    "TaxExemptFlg, CustomerExportId " +
                    //"From mcJobs Where (ExportDtTm Is Null Or InvUpdatedDtTm > ExportDtTm) and FinalDtTm Is Not Null Order By JobRno";
                    "From mcJobs Where ExportDtTm Is Null and FinalDtTm Is Not Null Order By JobRno";

                dt = db.DataTable(Sql);
                foreach (DataRow dr in dt.Rows)
                {
                    Data.Invoices.Add(new Invoice()
                    {
                        ExportId            = DB.Str(dr["ExportId"]),
                        id                  = DB.Int32(dr["id"]),
                        Number              = DB.Int32(dr["Number"]),
                        SubTotal            = DB.Dec(dr["SubTotal"]),
                        Gratuity            = DB.Dec(dr["Gratuity"]),
                        SalesTaxRate        = DB.Dec(dr["SalesTaxRate"]),
                        SalesTax            = DB.Dec(dr["SalesTax"]),
                        Total               = DB.Dec(dr["Total"]),
                        Date                = DB.DtTm(dr["InvoicedDtTm"]),
                        TaxExempt           = DB.Bool(dr["TaxExemptFlg"]),
                        CustomerExportId    = DB.Str(dr["CustomerExportId"])
                    });
                }

                Sql =
                    "Select p.ExportId, p.PaymentRno as id, p.JobRno as InvoiceNumber, p.Seq, p.Amount, p.PaymentDt, p.Reference, p.Type, p.Method, p.CCType, " +
                    "CCAuth, CCReference, j.ExportId as InvoiceExportId, j.CustomerExportId " +
                    "From Payments p Inner Join mcJobs j on p.JobRno = j.JobRno " +
                    //"Where (p.ExportDtTm Is Null Or p.UpdatedDtTm > p.ExportDtTm) and p.Amount <> 0 Order By p.PaymentRno";
                    "Where p.ExportDtTm Is Null and p.Amount <> 0 Order By p.PaymentRno";

                dt = db.DataTable(Sql);
                foreach (DataRow dr in dt.Rows)
                {
                    Data.Payments.Add(new Payment() {
                        ExportId            = DB.Str(dr["ExportId"]),
                        id                  = DB.Int32(dr["id"]),
                        InvoiceNumber       = DB.Int32(dr["InvoiceNumber"]),
                        Seq                 = DB.Int32(dr["Seq"]),
                        Amount              = DB.Dec(dr["Amount"]),
                        Date                = DB.DtTm(dr["PaymentDt"]),
                        Reference           = DB.Str(dr["Reference"]),
                        Type                = DB.Str(dr["Type"]),
                        Method              = DB.Str(dr["Method"]),
                        CCType              = DB.Str(dr["CCType"]),
                        CCAuth              = DB.Str(dr["CCAuth"]),
                        CCReference         = DB.Str(dr["CCReference"]),
                        InvoiceExportId     = DB.Str(dr["InvoiceExportId"]),
                        CustomerExportId    = DB.Str(dr["CustomerExportId"])
                    });
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
        }

        Response.Clear();
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write(JsonConvert.SerializeObject(Data));
        Response.End();
    }
}