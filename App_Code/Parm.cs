using System;
using System.Collections.Generic;
using System.Web;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Summary description for Parm
	/// </summary>
	public class Parm
	{
		public Parm()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static bool Bool(string Name)
		{
			bool Value = false;

			object objParm = System.Web.HttpContext.Current.Request.Params[Name];
			if (objParm != null)
			{
				string sParm = objParm.ToString().ToLower();
				Value = (sParm == "on" || sParm == "1" || sParm == "true");
			}

			return Value;
		}

		public static int Int(string Name)
		{
			int Value = 0;

			object objParm = System.Web.HttpContext.Current.Request.Params[Name];
			if (objParm != null)
			{
				string sParm = objParm.ToString();
				Value = Utils.Str.Num(sParm);
			}

			return Value;
		}

		public static decimal Dec(string Name)
		{
			decimal Value = 0;

			object objParm = System.Web.HttpContext.Current.Request.Params[Name];
			if (objParm != null)
			{
				string sParm = objParm.ToString();
				Value = Utils.Str.Dec(sParm);
			}

			return Value;
		}

		public static DateTime DtTm(string Name)
		{
			DateTime Value = DateTime.MinValue;

			object objParm = System.Web.HttpContext.Current.Request.Params[Name];
			if (objParm != null)
			{
				string sParm = objParm.ToString();
				Value = Utils.Str.DtTm(sParm);
			}

			return Value;
		}

		public static string Str(string Name)
		{
			string Value = string.Empty;

			object objParm = System.Web.HttpContext.Current.Request.Params[Name];
			if (objParm != null)
			{
				Value = objParm.ToString();
			}

			return Value;
		}
	}
}