using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class KeepSessionAlive : System.Web.UI.Page
{
	protected string WindowStatusText = "";

	protected void Page_Load(object sender, EventArgs e)
    {
		string User = (string)Session["User"];
		if (User != null && User != "")
		{
			// Refresh this page 60 seconds before session timeout, effectively resetting the session timeout counter.
			MetaRefresh.Attributes["content"] = Convert.ToString((Session.Timeout * 60) - 60) + ";url=KeepSessionAlive.aspx?q=" + DateTime.Now.Ticks;
			//MetaRefresh.Attributes["content"] = Convert.ToString(30) + ";url=KeepSessionAlive.aspx?q=" + DateTime.Now.Ticks;

			WindowStatusText = "Last refresh " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
		}
	}
}
