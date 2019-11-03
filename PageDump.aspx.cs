using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class PageDump : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        using (StreamWriter sw = new StreamWriter(Server.MapPath("~/output/PageDump.txt"), true))
        {
            sw.WriteLine(DateTime.Now.ToString());
            sw.WriteLine("Request Type: {0}", Request.RequestType);
            sw.WriteLine("URL: {0}", Request.Url);
            sw.WriteLine("URL Referrer: {0}", Request.UrlReferrer);


            sw.WriteLine();
            sw.WriteLine("Headers");
            sw.WriteLine("-------------------------------------------------------------------");
            foreach (string Key in Request.Headers.AllKeys)
            {
                sw.WriteLine("{0}: {1}", Key, Request.Headers[Key]);
            }

            sw.WriteLine();
            sw.WriteLine("Params");
            sw.WriteLine("-------------------------------------------------------------------");
            foreach (string Key in Request.Params.AllKeys)
            {
                sw.WriteLine("{0}: {1}", Key, Request.Params[Key]);
            }

            sw.WriteLine();
            sw.WriteLine("Form");
            sw.WriteLine("-------------------------------------------------------------------");
            foreach (string Key in Request.Form.AllKeys)
            {
                sw.WriteLine("{0}: {1}", Key, Request.Form[Key]);
            }

            sw.WriteLine();
            sw.WriteLine("Body");
            sw.WriteLine("-------------------------------------------------------------------");
            Request.InputStream.CopyTo(sw.BaseStream);

            sw.WriteLine();
            sw.WriteLine("===========================================================================================");
            sw.WriteLine();
        }
    }
}