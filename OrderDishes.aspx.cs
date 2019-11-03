using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using Utils;
using Globals;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class OrderDishes : System.Web.UI.Page
{
	protected WebPage Pg;
	protected String FocusField = string.Empty;
	protected string NewHtml = string.Empty;

	private void Page_Load(object sender, System.EventArgs e)
	{
		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.JobSetup.Title;
		}
		else
		{
			SaveUpdates();
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
        LoadValues();

        NewHtml = Misc.ToString(EditValue("xxxx", 0, "New Dish"));
	}

    private void LoadValues()
    {
        string Sql = "Select DishRno, DishItem From mcJobDishes Where JobRno = 0 Order By DishSeq";
        try
        {
            DB db = new DB();
            DataTable dt = db.DataTable(Sql);

            int cRows = dt.Rows.Count;
            hfNumItems.Value = cRows.ToString();
            int i = 0;

            foreach (DataRow dr in dt.Rows)
            {
                int DishRno = DB.Int32(dr["DishRno"]);
                string Value = DB.Str(dr["DishItem"]);

                HtmlGenericControl ulItems = (i < (cRows + 1) / 2 ? ulItems1 : ulItems2);
                ulItems.Controls.Add(EditValue(i.ToString(), DishRno, Value));
                i++;
            }
            db.Close();
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    private HtmlGenericControl EditValue(string i, int Rno, string Value)
	{
		HtmlGenericControl li = new HtmlGenericControl("li");

		HtmlGenericControl sp = new HtmlGenericControl("span");
		sp.Attributes.Add("class", "ui-icon ui-icon-arrowthick-2-n-s");
		li.Controls.Add(sp);

		HtmlInputHidden hf = new HtmlInputHidden();
        hf.ID = string.Format("hfRno{0}", i);
        hf.Value = Rno.ToString();
        li.Controls.Add(hf);

        hf = new HtmlInputHidden();
        hf.ID = string.Format("hfValue{0}", i);
		hf.Value = Value;
		li.Controls.Add(hf);

		hf = new HtmlInputHidden();
		hf.ID = string.Format("hfSeq{0}", i);
		hf.Value = i.ToString();
		hf.Attributes.Add("class", "Seq");
		li.Controls.Add(hf);

		HtmlGenericControl lbl = new HtmlGenericControl("label");
		lbl.InnerText = Value;
        lbl.Attributes.Add("class", "Value");
		li.Controls.Add(lbl);

        HtmlInputText txt = new HtmlInputText();
        txt.ID = string.Format("txtValue{0}", i);
        txt.Value = Value;
        txt.Style.Add("display", "none");
        li.Controls.Add(txt);

        HtmlGenericControl img = new HtmlGenericControl("img");
        img.Attributes.Add("src", "Images\\Add16.png");
        img.Attributes.Add("class", "Show");
        li.Controls.Add(img);

        img = new HtmlGenericControl("img");
        img.Attributes.Add("src", "Images\\Delete16.png");
        img.Attributes.Add("class", "NotShow");
        li.Controls.Add(img);

        hf = new HtmlInputHidden();
        hf.ID = string.Format("hfDelete{0}", i);
        hf.Value = "false";
        li.Controls.Add(hf);

        return li;
	}

	private void SaveUpdates()
	{
		string Sql = string.Empty;
		try
		{
			DB db = new DB();

			int cItems = Str.Num(hfNumItems.Value);
			for (int i = 0; i < cItems; i++)
			{
                int Rno = Parm.Int(string.Format("hfRno{0}", i));
				string Value = Parm.Str(string.Format("hfValue{0}", i));
				string NewValue = Parm.Str(string.Format("txtValue{0}", i));
                int Seq = Parm.Int(string.Format("hfSeq{0}", i));
                bool fDelete = Parm.Bool(string.Format("hfDelete{0}", i));

                if (Rno == 0)
                {
                    if (!fDelete)
                    {
                        Sql = string.Format("Insert Into mcJobDishes (JobRno, DishItem, DishSeq, CreatedDtTm, CreatedUser) Values (0, {0}, {1}, GetDate(), {2})",
                            DB.PutStr(NewValue),
                            Seq,
                            DB.PutStr(g.User));
                        db.Exec(Sql);
                    }
                }
                else
				{
                    if (!fDelete)
                    {
                        Sql = string.Format("Update mcJobDishes Set DishSeq = {1}, UpdatedDtTm = GetDate(), UpdatedUser = {2} Where DishRno = {0}",
                            Rno,
                            Seq,
                            DB.PutStr(g.User));
                        db.Exec(Sql);

                        // has the Value text itself changed?
                        if (NewValue != Value)
                        {
                            // has changed, update Value values
                            Sql = string.Format("Update mcJobDishes Set DishItem = {1} Where DishRno = {0}",
                                Rno,
                                DB.PutStr(NewValue));
                            db.Exec(Sql);
                        }
                    }
                    else
                    {
                        Sql = string.Format("Delete From mcJobDishes Where DishRno = {0}", Rno);
                        db.Exec(Sql);
                    }
				}
			}
			db.Close();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}
}