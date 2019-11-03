using System;
using System.Data;
using System.Web.UI;
using Utils;
using Globals;
using System.Web.UI.WebControls;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class SlingUser : Page
{
	protected WebPage Pg;
	protected DB db;

    protected string FocusField = "";

    protected string ErrorMsg = string.Empty;
    protected bool fError = false;

    private void Page_Load(object sender, System.EventArgs e)
    {
        db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
        {
            Session["Menu"] = WebPage.Users.Title;
            ClearData();
            Setup();
        }
    }

    private void Setup()
    {
        btnUpdate.Enabled = true;

        GetData();
    }

    protected void ClearData()
    {
        txtUser.Text = 
        txtPassword.Text = string.Empty;
        hfEmailRno.Value =
        hfPasswordRno.Value = string.Empty;
    }

    protected void GetData()
    {
        string Sql = string.Format("Select * From SysParms Where SysParmName = {0}", DB.PutStr(Misc.cnSlingUserEmail));

        ClearData();

        try
        {
            DataRow dr = db.DataRow(Sql);
            if (dr != null)
            {
                hfEmailRno.Value = DB.Int32(dr["SysParmRno"]).ToString();
                txtUser.Text = DB.Str(dr["SysParmValue"]);
                txtCreatedDt.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
                txtCreatedUser.Text = DB.Str(dr["CreatedUser"]);
                txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
                txtUpdatedUser.Text = DB.Str(dr["UpdatedUser"]);
            }

            Sql = string.Format("Select * From SysParms Where SysParmName = {0}", DB.PutStr(Misc.cnSlingUserPassword));
            dr = db.DataRow(Sql);
            if (dr != null)
            {
                hfPasswordRno.Value = DB.Int32(dr["SysParmRno"]).ToString();
                txtPassword.Text = DB.Str(dr["SysParmValue"]);
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    protected void SaveData()
    {
        string Sql = string.Empty;
        DateTime tm = DateTime.Now;

        try
        {
            Int32 Rno = Str.Num(hfEmailRno.Value);
            if (Rno == 0)
            {
                Sql = string.Format(
                    "Insert Into SysParms (SysParmName, CreatedDtTm, CreatedUser) Values ({0}, {1}, {2});" + 
                    "Select @@Identity;",
                    DB.Put(Misc.cnSlingUserEmail),
                    DB.Put(tm),
                    DB.Put(g.User));
                Rno = db.SqlNum(Sql);
            }

            Sql = string.Format(
                "Update SysParms Set " + 
                "SysParmValue = {1}, " +
                "UpdatedDtTm = {2}, " +
                "UpdatedUser = {3} " +
                "Where SysParmRno = {0}",
                Rno,
                DB.Put(txtUser.Text), 
                DB.Put(tm),
                DB.Put(g.User));
            db.Exec(Sql);

            Rno = Str.Num(hfPasswordRno.Value);
            if (Rno == 0)
            {
                Sql = string.Format(
                    "Insert Into SysParms (SysParmName, CreatedDtTm, CreatedUser) Values ({0}, {1}, {2});" +
                    "Select @@Identity;",
                    DB.Put(Misc.cnSlingUserPassword),
                    DB.Put(tm),
                    DB.Put(g.User));
                Rno = db.SqlNum(Sql);
            }

            Sql = string.Format(
                "Update SysParms Set " +
                "SysParmValue = {1}, " +
                "UpdatedDtTm = {2}, " +
                "UpdatedUser = {3} " +
                "Where SysParmRno = {0}",
                Rno,
                DB.Put(txtPassword.Text),
                DB.Put(tm),
                DB.Put(g.User));
            db.Exec(Sql);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        Update(false);
    }

    protected void Update(bool fNext)
    {
        SaveData();
    }
}
