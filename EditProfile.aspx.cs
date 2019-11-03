using System;
using System.Data;
using System.Web.UI;
using Utils;
using Globals;
using System.Web.UI.WebControls;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class EditProfile : Page
{
    protected WebPage Pg;
    protected DB db;

    protected String FocusField = "";

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
            Session["Menu"] = WebPage.EditProfile.Title;
            GetData(g.User);
        }
    }

    protected void GetData(string userName)
    {
        string Sql = string.Format("Select * From Users Where UserName = {0}", DB.PutStr(userName));

        try
        {
            DataRow dr = db.DataRow(Sql);
            if (dr != null)
            {
                txtUserCommonName.Text = DB.Str(dr["UserCommonName"]);
                txtUserName.Text = DB.Str(dr["UserName"]);
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
        string userName = g.User;
        string Sql = string.Empty;

        try
        {
            Sql = string.Format("Select Count(*) From Users Where UserName = {0}", DB.PutStr(txtUserName.Text));

            DateTime Tm = DateTime.Now;

            if (userName != txtUserName.Text)
            {
                // if user already exists
                if (db.SqlNum(Sql) > 0)
                {
                    fError = true;
                    ErrorMsg = string.Format("There is already a user named <b>{0}</b>. Please choose another name.", txtUserName.Text);
                }
            }

            // if changing password
            if (chkChangePassword.Checked)
            {
                string salt = UserHelper.GetSaltForUser(userName);
                string passwordKey = UserHelper.GetPasswordKeyForUser(userName);

                string passwordValue = UserHelper.Generate256HashOnString(txtCurrentPassword.Text + salt);

                if (passwordKey != passwordValue)
                {
                    fError = true;
                    ErrorMsg = "The password you typed does not match your current password.";
                }
                else if (string.IsNullOrEmpty(txtNewPassword.Text))
                {
                    fError = true;
                    ErrorMsg = "New password cannot be blank.";
                }
                else if (txtNewPassword.Text != txtRetypePassword.Text)
                {
                    fError = true;
                    ErrorMsg = "The new passwords do not match.";
                }
            }

            // if there wasn't an error
            if (!fError)
            {
                string salt = UserHelper.GetSaltForUser(userName);
                string passwordKey = UserHelper.GetPasswordKeyForUser(userName);

                string passwordValue = string.IsNullOrEmpty(txtNewPassword.Text) ? passwordKey : UserHelper.Generate256HashOnString(txtNewPassword.Text + salt);

                // update the values
                Sql = string.Format(
                    "Begin Transaction\n" +
                    "Update Users Set " +
                    "UserName = {1}, " +
                    "UserCommonName = {2}, " +
                    "Password = {3}, " +
                    "UpdatedDtTm = {4}, " +
                    "UpdatedUser = {5} " +
                    "Where UserName = {0}\n",
                    DB.PutStr(userName),
                    DB.PutStr(txtUserName.Text),
                    DB.PutStr(txtUserCommonName.Text),
                    DB.PutStr(passwordValue),
                    DB.PutDtTm(Tm),
                    DB.PutStr(g.User));

                Sql += "Commit Transaction";
                db.Exec(Sql);

                Session["User"] =
                g.User = txtUserName.Text;

                pnlChangedPassword.Visible = chkChangePassword.Checked ? true : false;
                chkChangePassword.Checked = false;
                txtCurrentPassword.Text =
                txtNewPassword.Text =
                txtRetypePassword.Text = string.Empty;
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        SaveData();
    }
}
