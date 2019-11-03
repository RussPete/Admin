using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;
using Globals;

public partial class CCPrint : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        g.User = (string)Session["User"];
        if (g.User != null && g.User != "")
        {
            Int32 PaymentRno = Parm.Int("PaymentRno");
            DateTime Tm = DateTime.Now;

            string Sql = 
                "Update Payments Set " +
                "CCPrintDtTm = " + DB.PutDtTm(Tm) + ", " +
                "CCPrintUser = " + DB.PutStr(g.User) + ", " +
                "UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
                "UpdatedUser = " + DB.PutStr(g.User) + " " +
                "Where PaymentRno = " + PaymentRno;

            try
            {
                DB.DBExec(Sql);
            }
            catch (Exception)
            {
            }
        }
    }
}