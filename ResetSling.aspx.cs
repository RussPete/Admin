using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class ResetSling : System.Web.UI.Page
{
    protected WebPage Pg;
    protected DB db;
    protected Sling Sling;
    protected Dictionary<Int32, string> MarvellJobs = new Dictionary<Int32, string>();

    private void Page_Init(object sender, System.EventArgs e)
    {
    }

    private void Page_Load(object sender, System.EventArgs e)
    {
        db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
        {
            Session["Menu"] = WebPage.Sling.Title;

        }
    }

    protected void btnReset_Click(object sender, System.EventArgs e)
    {
        Sling Sling = new Sling();
        Sling.Reset();
    }
}