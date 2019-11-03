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

public partial class Tables : System.Web.UI.Page
{
    protected WebPage Pg;
    private DB db;
    protected String FocusField = "";
    protected Int32 JobRno;

    private void Page_Load(object sender, System.EventArgs e)
    {
        db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        JobRno = Pg.JobRno();

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
        {
            Session["Menu"] = WebPage.Jobs.Title;
            Setup();
        }
    }

    private void Page_PreRender(object sender, System.EventArgs e)
    {
        LoadTables();

        FocusField = "txtCaterer1";
    }

    private void Setup()
    {
        btnUpdate.Attributes.Add("onClick", "ClearDirty();");
    }

    private void LoadTables()
    {
        if (JobRno > 0)
        {
            String Sql = "";

            while (tblTables.Rows.Count > 1)
            {
                tblTables.Rows.RemoveAt(1);
            }

            try
            {
                Sql = String.Format(
                    "Select Seq, Description, Caterer, Other " +
                    "From JobTables " +
                    "Where JobRno = {0} Order By Seq", JobRno);
                DataTable dt = db.DataTable(Sql);
                hfNewRecords.Value = "false";

                if (dt.Rows.Count == 0)
                {
                    Sql = String.Format(
                        "Select Seq, Description, Caterer, Other " +
                        "From JobTables " +
                        "Where JobRno = {0} Order By Seq", 0);
                    dt = db.DataTable(Sql);
                    hfNewRecords.Value = "true";
                }

                int iRow = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    iRow++;

                    Label lbl = new Label() { ID = "lblDescription" + iRow, Text = DB.Str(dr["Description"]) };
                    HiddenField hf = new HiddenField() { ID = "hfDescription" + iRow, Value = lbl.Text };
                    TableCell tc = new TableCell() { CssClass = "Field" };
                    tc.Controls.Add(lbl);
                    tc.Controls.Add(hf);
                    TableRow tr = new TableRow();
                    tr.Cells.Add(tc);

                    hf = new HiddenField() { ID = "hfSeq" + iRow, Value = DB.Int32(dr["Seq"]).ToString() };
                    tc = new TableCell();
                    tc.Controls.Add(hf);
                    tr.Cells.Add(tc);

                    TextBox txt = new TextBox() { ID = "txtCaterer" + iRow, CssClass = "Tables", Text = DB.Str(dr["Caterer"]) };
                    tc = new TableCell();
                    tc.Controls.Add(txt);
                    tr.Cells.Add(tc);

                    txt = new TextBox() { ID = "txtOther" + iRow, CssClass = "Tables", Text = DB.Str(dr["Other"]) };
                    tc = new TableCell();
                    tc.Controls.Add(txt);
                    tr.Cells.Add(tc);

                    tblTables.Rows.Add(tr);
                }
                hfCount.Value = iRow.ToString();
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex, Sql);
                Response.Write(Err.Html());
            }
        }
    }

    protected void btnUpdate_Click(object sender, System.EventArgs e)
    {
        Update();
        LoadTables();
    }

    private void Update()
    {
        bool fNew = (hfNewRecords.Value == "true");
        DateTime Tm = DateTime.Now;

        int Count = (int)Str.Int64(hfCount.Value);
        for (int iRow = 1; iRow <= Count; iRow++)
        {
            int Seq = Parm.Int("hfSeq" + iRow);
            string sDescription = Parm.Str("hfDescription" + iRow);
            string sCaterer = Parm.Str("txtCaterer" + iRow);
            string sOther = Parm.Str("txtOther" + iRow);

            string Sql = string.Format(fNew ? 
                "Insert Into JobTables (JobRno, Seq, Description, Caterer, Other, CreatedDtTm, CreatedUser) Values ({0}, {1}, {2}, {3}, {4}, {5}, {6})" : 
                "Update JobTables Set Caterer = {3}, Other = {4}, UpdatedDtTm = {5}, UpdatedUser = {6} Where JobRno = {0} and Seq = {1}",
                JobRno,
                Seq,
                DB.Put(sDescription, 20), 
                DB.Put(sCaterer, 60),
                DB.Put(sOther, 60),
                DB.PutDtTm(Tm),
                DB.PutStr(g.User));

            try
            {
                db.Exec(Sql);
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex, Sql);
                Response.Write(Err.Html());
            }
        }
    }
}