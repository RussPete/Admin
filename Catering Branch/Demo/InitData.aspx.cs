using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;
using YamlDotNet.RepresentationModel;

public partial class Demo_InitData : System.Web.UI.Page
{
    DB db = new DB();

    protected void Page_Load(object sender, EventArgs e)
    {
        ImportData();
    }

    protected void btnInitData_Click(object sender, EventArgs e)
    {
        ImportData();
    }

    protected void ImportData()
    {
        /*
        ClearMenuCategories();
        InsertMenuCategory("Hamburgers", 1);
        InsertMenuCategory("Side Dish", 2);
        InsertMenuCategory("Drinks", 3);

        ClearMenuItems();
        InsertMenuItem("Hamburgers", "Hamburger", 1);
        InsertMenuItem("Hamburgers", "Cheese Hamburger", 2);
        InsertMenuItem("Hamburgers", "Bacon Hamburger", 3);
        InsertMenuItem("Side Dish", "French Fries", 1);
        InsertMenuItem("Side Dish", "Onion Rings", 2);
        InsertMenuItem("Drinks", "Water", 1);
        InsertMenuItem("Drinks", "Root Beer", 2);
        InsertMenuItem("Drinks", "Coke", 3);

        ClearUsers();
        InsertUser("admin", "E7D1ABDEAC670D6A10E36BEF5D2E77ADC89F372F809098C0F4F7CC0E7BA9E227", "abc1f0ed1df4663b248ea20e2e7b8d93ba0047bcffb4ef168c59ac60b7f98e3a", 4);
        InsertUser("user1", "4D22B7A69663E995FD2EB5F5B653FEB97781959E138C4A4F59FFCB7972A20E97", "09ab9c2fa297ff966deb57314d94bf52eaa785825933212e2345576e3689d7ea", 3);
        InsertUser("user2", "F9A60435428EED136AB88A0731B60E1AAD1136C92263BED22F58B6EC9F8448F2", "829d86543ddf38af1bdd92a5934b4ea034a040e1184d257c0af758fd0e5506fb", 3);
        InsertUser("user3", "3F08A242F5426D60CA288751D1AF0A49B994BD75A6F4097BC3FFC9A929CFCDFB", "fe17943539f10d9669478cdba0d5fc9ee98654b3734a508efd20ac6b7167c6a6", 3);
        */

        const int NumCustomers = 50;
        const int MaxNumContacts = 3;
        const int MaxNumJobs = 5;
        const int MaxNumDays = 60;

        ClearCustomers();
        ClearContacts();
        ClearJobs();

        for (int iCustomer = 0; iCustomer < NumCustomers; iCustomer++)
        {
            string CustomerName = Faker.Company.Name();
            int CustomerRno = InsertCustomer(CustomerName);

            Random Rnd = new Random();
            int NumContacts = Rnd.Next(MaxNumContacts) + 1;
            for (int iContact = 0; iContact < NumContacts; iContact++)
            {
                int ContactRno = InsertContact(CustomerRno, Faker.Name.FullName(), Faker.Phone.Number(), Faker.Internet.Email());

                int NumJobs = Rnd.Next(MaxNumJobs) + 1;
                for (int iJob = 0; iJob < NumJobs; iJob++)
                {
                    DateTime JobDate = DateTime.Today.AddDays(Rnd.Next(MaxNumDays));
                    string[] Words = Faker.Lorem.Words(6).ToArray();
                    int JobRno = InsertJob(ContactRno, CustomerName, Words[0], JobDate, JobDate.AddMinutes(Rnd.Next(9 * 60)), Rnd.Next(2000) + 1);
                }
            }
        }

        //        using (StreamReader sr = new StreamReader(Server.MapPath("~\\Demo\\demodata.yaml")))
        //        {
        //            YamlStream Yaml = new YamlStream();
        //            Yaml.Load(sr);
        //            foreach (YamlDocument YamlDoc in Yaml.Documents)
        //            {
        //                ExploreNode(YamlDoc.RootNode);
        //            }
        //        }
    }

    protected void ClearMenuCategories()
    {
        string Sql = string.Format("delete from mcJobMenuCategories");
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

    protected void InsertMenuCategory(string Category, int SortOrder)
    {
        string Sql = string.Format("insert into mcJobMenuCategories (Category, SortOrder, CreatedDtTm, CreatedUser) values ({0}, {1}, GetDate(), 'Sys')", DB.Put(Category, 50), DB.Put(SortOrder));
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

    protected void ClearMenuItems()
    {
        string Sql = string.Format("delete from mcJobMenuItems");
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

    protected void InsertMenuItem(string Category, string MenuItem, int CategorySortOrder)
    {
        string Sql = string.Format("insert into mcJobMenuItems (Category, MenuItem, CategorySortOrder, CreatedDtTm, CreatedUser) values ({0}, {1}, {2}, GetDate(), 'Sys')", DB.Put(Category, 50), DB.Put(MenuItem, 50), DB.Put(CategorySortOrder));
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

    protected void ExploreNode(YamlNode Node, int Depth = 0, string Label = "")
    {
        string Padding = "";
        for (int i = 0; i < Depth * 4; i++)
        {
            Padding += ".";
        }

        switch (Node.NodeType)
        {
            case YamlNodeType.Alias:
                break;
            case YamlNodeType.Mapping:
                YamlMappingNode MapNode = Node as YamlMappingNode;
                foreach (KeyValuePair<YamlNode, YamlNode> Child in MapNode.Children)
                {
                    //Response.Write(Padding + Child.Key + "<br/>");
                    YamlScalarNode KeyNode = Child.Key as YamlScalarNode;
                    ExploreNode(Child.Value, Depth, KeyNode.Value);
                }
                break;
            case YamlNodeType.Scalar:
                YamlScalarNode ScalarNode = Node as YamlScalarNode;
                Response.Write(Padding + Label + ": " + ScalarNode.Value + "<br/>");
                break;
            case YamlNodeType.Sequence:
                YamlSequenceNode SeqNode = Node as YamlSequenceNode;
                foreach (YamlNode Child in SeqNode.Children)
                {
                    Response.Write(Padding + Label + "<br/>");
                    ExploreNode(Child, Depth + 1, Label);
                    Response.Write("Write " + Label + "<br/>");

                    switch (Label)
                    {
                        case "mcJobMenuItems":
                            break;
                        default:
                            break;
                    }

                }
                break;
            default:
                break;
        }
    }

    protected void ClearUsers()
    {
        string Sql = string.Format("delete from Users");
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

    protected void InsertUser(string User, String PasswordSalt, String Password, int AccessLevel)
    {
        string Sql = string.Format(
            "insert into Users (UserName, UserCommonName, PasswordSalt, Password, AccessLevel, CreatedDtTm, CreatedUser) values ({0}, {0}, {1}, {2}, {3}, GetDate(), 'Sys')", 
            DB.Put(User, 20), DB.Put(PasswordSalt, 64), DB.Put(Password, 64), AccessLevel);
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

    protected void ClearCustomers()
    {
        string Sql = string.Format("delete from Customers");
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

    protected int InsertCustomer(string Name)
    {
        int Rno = 0;

        string Sql = string.Format(
            "insert into Customers (Name, CreatedDtTm, CreatedUser) values ({0}, GetDate(), 'Sys'); select SCOPE_IDENTITY()", DB.Put(Name, 50));
        try
        {
            Rno = db.SqlNum(Sql);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return Rno;
    }

    protected void ClearContacts()
    {
        string Sql = string.Format("delete from Contacts");
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

    protected int InsertContact(int CustomerRno, string Name, string Cell, string Email)
    {
        int Rno = 0;

        string Sql = string.Format(
            "insert into Contacts (CustomerRno, Name, Cell, Email, CreatedDtTm, CreatedUser) values ({0}, {1}, {2}, {3}, GetDate(), 'Sys'); select SCOPE_IDENTITY()", CustomerRno, DB.Put(Name, 50), DB.Put(Cell, 20), DB.Put(Email, 80));
        try
        {
            Rno = db.SqlNum(Sql);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return Rno;
    }

    protected void ClearJobs()
    {
        string Sql = string.Format("delete from mcJobs");
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

    protected int InsertJob(int ContactRno, string CustomerName, string EventType, DateTime JobDate, DateTime MealTime, int NumServings)
    {
        int Rno = 0;

        Random Rnd = new Random();

        string Sql = string.Format(
            "insert into mcJobs (JobRno, JobDesc, ContactRno, JobType, EventType, JobDate, MealTime, NumMenServing, CreatedDtTm, CreatedUser) " +
            "values ((select IsNull(Max(JobRno), 0) + 1 From mcJobs), {0}, {1}, {2}, {3}, {4}, {5}, {6}, GetDate(), 'Sys'); select Max(JobRno) + 1 From mcJobs",
            DB.Put(CustomerName + " - " + EventType, 80),
            ContactRno,
            DB.Put((Rnd.Next(2) == 1 ? "Corporate" : "Private"), 20),
            DB.Put(EventType, 50),
            DB.Put(JobDate),
            DB.Put(MealTime),
            NumServings);
        try
        {
            Rno = db.SqlNum(Sql);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return Rno;
    }

}