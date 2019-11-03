using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utils;

/// <summary>
/// Summary description for Company
/// </summary>
public class Company
{
    private static string mCompanyLegalName;
    private static string mCompanyName;
    private static string mCompanyPhone;
    private static string mCompanyAddr1;
    private static string mCompanyAddr2;
    private static string mCompanyAddr3;
    private static string mCompanyInitials;
    private static string mInfoEmail;
    private static string mCreditCardPmt;
    private static string mRecipeBookCopyright;
    private static string mCompanyWebsite;
    private static string mCompanyWebsiteUrl;
    private static string mCustomerComfirmationLogoBackground;

    public Company()
    {
        using (WebConfig wc = new WebConfig())
        {
            mCompanyLegalName = wc.Str("Company Legal Name");
            mCompanyName = wc.Str("Company Name");
            mCompanyPhone = wc.Str("Company Phone");
            mCompanyAddr1 = wc.Str("Company Addr1");
            mCompanyAddr2 = wc.Str("Company Addr2");
            mCompanyAddr3 = wc.Str("Company Addr3");
            mCompanyInitials = wc.Str("Company Initials");
            mInfoEmail = wc.Str("Info Email");
            mCreditCardPmt = wc.Str("Credit Card Payment");
            mRecipeBookCopyright = wc.Str("Recipe Book Copyright");
            mCompanyWebsite = wc.Str("Company Website");
            mCompanyWebsiteUrl = wc.Str("Company Website URL");
            mCustomerComfirmationLogoBackground = wc.Str("CustomerComfirmationLogoBackground");
        }
    }

    public string LegalName { get { return mCompanyLegalName; } }

    public string Name { get { return mCompanyName; } }
 
    public string Phone { get { return mCompanyPhone; } }

    public string Address { get { return Addr1 + (Addr2.Length > 0 ? ", " : "") + Addr2 + (Addr3.Length > 0 ? ", " : "") + Addr3; } }

    public string Addr1 { get { return mCompanyAddr1; } }

    public string Addr2 { get { return mCompanyAddr2; } }

    public string Addr3 { get { return mCompanyAddr3; } }

    public string Initials { get { return mCompanyInitials; } }

    public string InfoEmail { get { return mInfoEmail; } }

    public string CreditCardPmt { get { return mCreditCardPmt; } }

    public string RecipeBookCopyright { get { return mRecipeBookCopyright; } }

    public string Website { get { return mCompanyWebsite; } }

    public string WebsiteUrl { get { return mCompanyWebsiteUrl; } }

    public string CustomerComfirmationLogoBackground { get { return mCustomerComfirmationLogoBackground; } }
}