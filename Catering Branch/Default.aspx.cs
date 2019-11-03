using Globals;
using System;
using System.Data;
using System.Web.UI;
using Utils;

public partial class Default : Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";
	protected SelectList slUser;

	private void Page_Init(object sender, System.EventArgs e)
	{
	}

	protected void Page_Load(object sender, EventArgs e)
	{
        db = new DB();

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.Jobs.Title;
            Setup();
		}

        Pg = new WebPage("Images");
    }

    private void Setup()
	{
		//Response.Cookies["User"].Value = "";
		Session["User"] = null;
        Session["AccessLevel"] = null;
		Session["JobRno"] = "";
		Session["Warn"] = "";
        g.UserAccessLevel = 0;
        new Sling().Clear();

		//SetupSelectLists();
		//LoadSelectLists();

		FocusField = "txtUser";
		lblBadPassword.Visible = false;
	}

	//private void SetupSelectLists()
	//{
	//	SelectList.Clear();

	//	slUser = new SelectList("User", ref txtUser);
	//	slUser.ImageButton(ddUser);
	//}

	//private void LoadSelectLists()
	//{
	//	String Sql;

	//	Sql = 
	//		"Select CreatedUser As Usr From mcJobs " +
	//		"Union " +
	//		"Select UpdatedUser As Usr From mcJobs " +
	//		"Union " +
	//		"Select PrintedUser As Usr From mcJobs " +
	//		"Union " +
	//		"Select CreatedUser As Usr From mcJobFood " +
	//		"Union " +
	//		"Select UpdatedUser As Usr From mcJobFood " +
	//		"Union " +
	//		"Select CreatedUser As Usr From mcJobServices " +
	//		"Union " +
	//		"Select UpdatedUser As Usr From mcJobServices " +
	//		"Union " +
	//		"Select CreatedUser As Usr From mcJobSupplies " +
	//		"Union " +
	//		"Select UpdatedUser As Usr From mcJobSupplies " +
	//		"Union " +
	//		"Select CreatedUser As Usr From mcJobCrew " +
	//		"Union " +
	//		"Select UpdatedUser As Usr From mcJobCrew " +
	//		"Order By Usr";

	//	try
	//	{
	//		slUser.ClearValues();
	//		slUser.AddDBValues(db, Sql);
	//	}
	//	catch (Exception Ex)
	//	{
	//		Err Err = new Err(Ex, Sql);
	//		Response.Write(Err.Html());
	//	}
	//}

	protected void btnLogin_Click(object sender, System.EventArgs e)
	{
        //WebConfig Cfg = new WebConfig();
        string userName = txtUser.Text.Trim();
        string password = txtPassword.Text;

        DB db = new DB();

        string sql = string.Format("SELECT PasswordSalt, Password, AccessLevel FROM Users WHERE UserName COLLATE Latin1_General_CS_AS = {0} AND DisabledFlg = 0", DB.PutStr(userName));
        DataRow dr = db.DataRow(sql);

        if (dr != null)
        {
            string salt = DB.Str(dr["PasswordSalt"]);
            string key = DB.Str(dr["Password"]);

            string passwordHashed = UserHelper.Generate256HashOnString(password + salt);

            if (passwordHashed == key)
            {
                Session["User"] = userName;
                Session["AccessLevel"] = DB.Int32(dr["AccessLevel"]);
                //Response.Cookies["User"].Value = userName;
                //Response.Cookies["User"].Expires = DateTime.Now.AddDays(1);
                Response.Redirect("~/Job.aspx");
            }
            else
            {
                lblBadPassword.Visible = true;
            }
        }
        else
        {
            lblBadPassword.Visible = true;
        }

        ////			if (txtPassword.Text.Length > 0)
        //		{
        //			if (txtPassword.Text == Cfg.Str("mcUserPassword"))
        //			{
        //				if (txtUser.Text.Trim().Length > 0)
        //				{

        //					Session["User"] = txtUser.Text.Trim();
        //					Response.Cookies["User"].Value = txtUser.Text.Trim();
        //					Response.Cookies["User"].Expires = DateTime.Now.AddDays(1);
        //					//Response.Redirect("http://" + Request.Url.Host + "/Job.aspx");
        //					Response.Redirect("~/Job.aspx");
        //				}
        //			}
        //			else
        //			{
        //				lblBadPassword.Visible = true;
        //			}
        //		}
    }
}
