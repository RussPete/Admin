using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

/// <summary>
/// Summary description for PageAccess
/// </summary>
public class PageAccess
{
    public enum AccessLevels
    {
        General = 1,
        OfficeStaff,
        Everything,
        Admin
    };

    protected Dictionary<String, AccessLevels> Pages;

    public PageAccess()
    {
        Pages = new Dictionary<String, AccessLevels>()
        {
            // Jobs
            { "Job",                    AccessLevels.General },
            { "Food",                   AccessLevels.OfficeStaff },
            { "Services",               AccessLevels.OfficeStaff },
            { "Tables",                 AccessLevels.OfficeStaff },
            { "Linens",                 AccessLevels.OfficeStaff },
            { "Dishes",                 AccessLevels.OfficeStaff },
            { "Supplies",               AccessLevels.OfficeStaff },
            { "Invoice",                AccessLevels.OfficeStaff },
            { "ProposalMenu",           AccessLevels.OfficeStaff },
            { "ProposalPricing",        AccessLevels.OfficeStaff },

            // Reports
            { "DailyJobs",              AccessLevels.General },
            { "JobLeads",               AccessLevels.General },
            { "ServingCounts",          AccessLevels.General },
            { "JobCalendar",            AccessLevels.General },
            { "JobSheets",              AccessLevels.General },
            { "Invoices",               AccessLevels.OfficeStaff },
            { "Confirmations",          AccessLevels.OfficeStaff },
            { "JobInfo",                AccessLevels.OfficeStaff },

            // Shopping
            { "StockedItems",           AccessLevels.OfficeStaff },
            { "ShoppingList",           AccessLevels.OfficeStaff },
            { "Purchases",              AccessLevels.OfficeStaff },
            { "Vendors",                AccessLevels.OfficeStaff },
            { "CleanupShopping",        AccessLevels.OfficeStaff },

            // Contacts
            { "Contacts",               AccessLevels.OfficeStaff },
            { "Customers",              AccessLevels.OfficeStaff },
            { "MergeContacts",          AccessLevels.OfficeStaff },
            { "MergeCustomers",         AccessLevels.OfficeStaff },

            // Menu Setup
            { "SetupMenuCategories",    AccessLevels.OfficeStaff },
            { "SetupMenuItems",         AccessLevels.OfficeStaff },
            { "MenuItemQuotes",         AccessLevels.OfficeStaff },
            { "SetupInvoice",           AccessLevels.OfficeStaff },

            // Recipes
            { "Recipes",                AccessLevels.OfficeStaff },
            { "RecipeBook",             AccessLevels.OfficeStaff },
            { "Ingredients",            AccessLevels.OfficeStaff },
            { "Units",                  AccessLevels.OfficeStaff },
            { "CleanupRecipes",         AccessLevels.OfficeStaff },
            { "SetupRecipes",           AccessLevels.OfficeStaff },

            // Job Setup
            { "OrderServices",          AccessLevels.OfficeStaff },
            { "OrderTables",            AccessLevels.OfficeStaff },
            { "OrderLinens",            AccessLevels.OfficeStaff },
            { "OrderDishes",            AccessLevels.OfficeStaff },
            { "OrderSupplies",          AccessLevels.OfficeStaff },

            // Accounting
            { "Accounting",             AccessLevels.Everything },
            { "CheckSearch",            AccessLevels.Everything },
            { "PaymentSearch",          AccessLevels.Everything },
            { "UnpaidJobs",             AccessLevels.OfficeStaff },
            { "DailyCCPmts",            AccessLevels.Everything },

            // Edit Profile
            { "EditProfile",            AccessLevels.General },

            // Users
            { "Users",                  AccessLevels.Admin },
            { "SlingUser",              AccessLevels.Admin },

            // Sling
            { "Availability",           AccessLevels.OfficeStaff },
            { "CheckSling",             AccessLevels.OfficeStaff },
            { "ResetSling",             AccessLevels.OfficeStaff },

            // Logout
            { "Default",                AccessLevels.General }
        };
    }

    public void Add(string Name, AccessLevels AccessLevel = AccessLevels.General)
    {
        Pages.Add(Name, AccessLevel);
    }

    public AccessLevels AccessLevel(string PageName)
    {
        return Pages[PageName];
    }
}