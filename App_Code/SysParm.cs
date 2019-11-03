using System;
using System.Data;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Summary description for Parm
	/// </summary>
	public class SysParm
	{
		public SysParm()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public static string Get(string Name)
        {
            string Value = null;

            string Sql = string.Format("Select SysParmValue From SysParms Where SysParmName = {0}", DB.Put(Name));
            try
            {
                DB db = new Utils.DB();
                DataRow dr = db.DataRow(Sql);
                Value = DB.Str(dr["SysParmValue"]);
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex, Sql);
                System.Web.HttpContext.Current.Response.Write(Err.Html());
            }

            return Value;
        }
	}
}