using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

/// <summary>
/// Summary description for RecentJobs
/// </summary>
public class RecentJobs
{
	private const string Name = "RecentJobs";
	public const int MaxSize = 5;
	private ArrayList Jobs = new ArrayList();
	//private Int32[] Jobs = new Int32[MaxSize];

	public RecentJobs()
	{
		Jobs = (ArrayList)HttpContext.Current.Session[Name];
		if (Jobs == null)
		{
			Jobs = new ArrayList();
		}
	}

	public void Push(Int32 JobRno)
	{
		// look for a previous copy
		bool fFound = false;
		foreach (Int32 Job in Jobs)
		{
			if (Job == JobRno)
			{
				Jobs.Remove(Job);
				fFound = true;
				break;
			}
		}

		if (!fFound && Jobs.Count == MaxSize)
		{
			Jobs.RemoveAt(0);
		}

		Jobs.Add(JobRno);

		HttpContext.Current.Session[Name] = Jobs;
	}

	public static string Html()
	{
		StringBuilder sbHtml = new StringBuilder();
		
		RecentJobs RecentJobs = new RecentJobs();

		int cJobs = 0;
		if (RecentJobs.Jobs != null && RecentJobs.Jobs.Count > 0)
		{
			ArrayList Reverse = (ArrayList)RecentJobs.Jobs.Clone();
			Reverse.Reverse();

			DB db = new DB();

			sbHtml.Append(
				"<i class=\"fa fa-bookmark-o bookmark\" title=\"Recent Jobs\"></i>\n" +
				"<div id=\"JobToolbar\" class=\"ui-widget-header ui-corner-all\">\n");
			
			foreach (Int32 JobRno in Reverse)
			{
				string Sql = string.Format(
                    "Select Coalesce(cu.Name, c.Name) as Customer, JobDate " +
                    "From mcJobs j " +
                    "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                    "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
                    "Where JobRno = {0}", JobRno);
				DataRow dr = db.DataRow(Sql);
				if (dr != null)
				{
					cJobs++;
					sbHtml.AppendFormat(
						"<div><button class=\"JobOnBar\" data-job=\"{0}\">#{0} - {1} {2}</button></div>\n",
						JobRno,
						DB.DtTm(dr["JobDate"]).ToString("M/d/yyyy"),
						DB.Str(dr["Customer"]));
				}
			}
			sbHtml.Append("</div>\n");
			db.Close();
		}

		if (cJobs == 0)
		{
			sbHtml = new StringBuilder();
		}

		return sbHtml.ToString();
	}
}