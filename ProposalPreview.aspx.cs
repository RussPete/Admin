using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class ProposalPreview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		int JobRno = Utils.Parm.Int("Rno");

		if (JobRno != 0)
		{
			MemoryStream msProposal = Utils.PdfFiles.Proposal(Page, JobRno);

			// view test result
			Response.Clear();
			Response.ClearContent();
			Response.ClearHeaders();
			Response.Buffer = true;

			// Prevent this page from being cached.
			//  NOTE: we cannot use the CacheControl property, or set the PRAGMA header value due to a flaw re: PDF/SSL/IE
			Response.Expires = -1;

			Response.ContentType = "application/pdf";
			Response.AppendHeader("content-length", msProposal.Length.ToString());
			Response.BinaryWrite(msProposal.ToArray());
			Response.Flush();
			Response.Close();
		}
    }
}