using System;
using System.Data;
using System.Web.UI;
using Utils;
using Globals;
using System.Web.UI.WebControls;
using System.IO;

public partial class Users : Page
{
	protected WebPage Pg;
	protected DB db;

    protected string FocusField = "";

    private string StartHere;
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
        btnUpdate.Enabled =
        btnNext.Enabled = true;

        LoadList();
    }

    protected void ClearData()
    {
        lstList.SelectedIndex = -1;
        txtCurrUser.Value =
        txtUserCommonName.Text = 
        txtUserName.Text =
        txtPassword.Text =
        txtRetypePassword.Text = string.Empty;
        rdoGeneral.Checked = 
        rdoOfficeStaff.Checked =
        rdoEverything.Checked =
        rdoAdmin.Checked =
        chkDisabled.Checked = false;
        btnDelete.Enabled = false;
        btnDelete.ToolTip = "New user record";
    }

    protected void GetData(string userNameLabel)
    {
        string userName = getUserNameFromListFormatStyle(userNameLabel);
        string Sql = string.Format("Select * From Users Where UserName = {0}", DB.PutStr(userName));

        ClearData();

        try
        {
            DataRow dr = db.DataRow(Sql);
            if (dr != null)
            {
                txtCurrUser.Value = listFormatStyle(DB.Str(dr["UserCommonName"]), DB.Str(dr["UserName"]));
                txtUserCommonName.Text = DB.Str(dr["UserCommonName"]);
                txtUserName.Text = DB.Str(dr["UserName"]);
                PageAccess.AccessLevels AccessLevel = (PageAccess.AccessLevels)DB.Int32(dr["AccessLevel"]);
                switch (AccessLevel)
                {
                    case PageAccess.AccessLevels.General:
                        rdoGeneral.Checked = true;
                        break;

                    case PageAccess.AccessLevels.OfficeStaff:
                        rdoOfficeStaff.Checked = true;
                        break;

                    case PageAccess.AccessLevels.Everything:
                        rdoEverything.Checked = true;
                        break;

                    case PageAccess.AccessLevels.Admin:
                        rdoAdmin.Checked = true;
                        break;
                }
                chkDisabled.Checked = DB.Bool(dr["DisabledFlg"]);
                txtCreatedDt.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
                txtCreatedUser.Text = DB.Str(dr["CreatedUser"]);
                txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
                txtUpdatedUser.Text = DB.Str(dr["UpdatedUser"]);
            }

            btnDelete.Enabled = true;
            btnDelete.ToolTip = "";
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    protected void SaveData()
    {
        string listUserFormat = txtCurrUser.Value;
        string userName = string.IsNullOrEmpty(listUserFormat) ? "" : getUserNameFromListFormatStyle(listUserFormat);
        string Sql = string.Empty;

        bool editingSelf = userName.Equals(g.User) ? true : false;

        try
        {
            Sql = string.Format("Select Count(*) From Users Where UserName = {0}", DB.PutStr(txtUserName.Text));

            DateTime Tm = DateTime.Now;

            if (txtUserName.Text != userName)
            {
                if (db.SqlNum(Sql) > 0)
                {
                    fError = true;
                    ErrorMsg = string.Format("There is already a user named <b>{0}</b>. Please choose another name.", txtUserName.Text);
                }
                else
                {
                    if (userName.Length == 0)
                    {
                        string salt = UserHelper.GeneratePasswordSalt();
                        string passwordHashed = UserHelper.Generate256HashOnString(txtPassword.Text + salt);

                        Sql = string.Format(
                            "Insert Into Users (UserName, PasswordSalt, Password, CreatedDtTm, CreatedUser) Values ({0}, {1}, {2}, {3}, {4})",
                            DB.PutStr(txtUserName.Text),
                            DB.PutStr(salt),
                            DB.PutStr(passwordHashed),
                            DB.PutDtTm(Tm),
                            DB.PutStr(g.User));
                        db.SqlNum(Sql);

                        userName = txtUserName.Text;
                    }
                }
            }

            if (chkChangePassword.Checked)
            {
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    fError = true;
                    ErrorMsg = "New password cannot be blank.";
                }
                else if (txtPassword.Text != txtRetypePassword.Text)
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

                string passwordValue = string.IsNullOrEmpty(txtPassword.Text) ? passwordKey : UserHelper.Generate256HashOnString(txtPassword.Text + salt);

                PageAccess.AccessLevels AccessLevel = PageAccess.AccessLevels.General;
                if (rdoGeneral.Checked)
                {
                    AccessLevel = PageAccess.AccessLevels.General;
                }
                else if (rdoOfficeStaff.Checked)
                {
                    AccessLevel = PageAccess.AccessLevels.OfficeStaff;
                }
                else if (rdoEverything.Checked)
                {
                    AccessLevel = PageAccess.AccessLevels.Everything;
                }
                else if (rdoAdmin.Checked)
                {
                    AccessLevel = PageAccess.AccessLevels.Admin;
                }

                // update the values
                Sql = string.Format(
                    "Begin Transaction\n" +
                    "Update Users Set " +
                    "UserName = {1}, " +
                    "UserCommonName = {2}, " +
                    "Password = {3}, " +
                    "AccessLevel = {4}, " +
                    "DisabledFlg = {5}, " +
                    "UpdatedDtTm = {6}, " +
                    "UpdatedUser = {7} " +
                    "Where UserName = {0}\n",
                    DB.PutStr(userName),
                    DB.PutStr(txtUserName.Text),
                    DB.PutStr(txtUserCommonName.Text),
                    DB.PutStr(passwordValue),
                    (Int32)AccessLevel, 
                    DB.PutBool(chkDisabled.Checked),
                    DB.PutDtTm(Tm),
                    DB.PutStr(g.User));

                Sql += "Commit Transaction";
                db.Exec(Sql);

                if (editingSelf)
                {
                    Session["User"] =
                    g.User = txtUserName.Text;
                    //Response.Cookies["User"].Value = txtUserName.Text;
                }

                chkChangePassword.Checked = false;
                txtPassword.Text =
                txtRetypePassword.Text = string.Empty;

                StartHere = listFormatStyle(txtUserCommonName.Text, txtUserName.Text);
            }
        //}
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

    protected void btnUpdateNext_Click(object sender, EventArgs e)
    {
        Update(true);
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        ClearData();
        LoadList();
        btnUpdate.Enabled =
        btnNext.Enabled = true;
        FocusField = "txtUserCommonName";
		chkChangePassword.Checked = true;
		chkChangePassword.Enabled = false;
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string Sql = String.Format("Delete From Users Where UserName = {0}", DB.PutStr(txtUserName.Text));
        try
        {
            db.Exec(Sql);
            ClearData();
            LoadList(true);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    protected void Update(bool fNext)
    {
        SaveData();
        if (!fError)
        {
            //ClearData();
            LoadList(fNext);
        }
    }

    protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        string user = lstList.SelectedItem.Value;
        //GetData(user);
        StartHere = user;
        LoadList();
        btnUpdate.Enabled =
        btnNext.Enabled = true;
		chkChangePassword.Checked = false;
		chkChangePassword.Enabled = true;
	}

    protected void UpdateList(object sender, System.EventArgs e)
    {
        LoadList();
    }

    private void LoadList()
    {
        LoadList(false);
    }

    private void LoadList(bool fNext)
    {
        string Sql = "Select UserCommonName, UserName, DisabledFlg From Users";
        string Disabled = chkShowDisabled.Checked ? string.Empty : " (DisabledFlg Is Null Or DisabledFlg = 0) ";
        string Where = "";
        if (Disabled.Length > 0) Where += (Where.Length > 0 ? " And" : string.Empty) + Disabled;

        string CurrCategory = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : null);
        string NextCurrCategory = (lstList.SelectedIndex >= 0 && lstList.SelectedIndex + 1 < lstList.Items.Count ? lstList.Items[lstList.SelectedIndex + 1].Value : null);
        if (!fNext && CurrCategory != null && CurrCategory.Length > 0)
        {
            NextCurrCategory = CurrCategory;
        }

        lstList.Items.Clear();

        Sql += (Where.Length > 0 ? " Where" + Where : "");

        LoadListSql(Sql, NextCurrCategory);
    }

    private void LoadListSql(string Sql, string NextCurrItem)
    {
        try
        {
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow r in dt.Rows)
            {
                string userName = DB.Str(r["userName"]);
                string userCommonName = DB.Str(r["UserCommonName"]);
                bool fHidden = DB.Bool(r["DisabledFlg"]);
                ListItem Item = new ListItem(listFormatStyle(userCommonName, userName));
                if (fHidden) Item.Attributes.Add("class", "Hidden");

                if (Item.Value == NextCurrItem)
                {
                    Item.Selected = true;

                    userName = Item.Value;
                    GetData(userName);
                }

                lstList.Items.Add(Item);
            }

            lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Users";
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    private string listFormatStyle(string commonName, string userName)
    {
        if (string.IsNullOrEmpty(commonName)) commonName = "Undefined";
        return string.Format("{0} ({1})", commonName, userName);
    }

    private string getUserNameFromListFormatStyle(string listStyle)
    {
        return listStyle.Split('(')[1].Replace(")", "");
    }
}
