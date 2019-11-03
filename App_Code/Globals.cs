using System;
using System.Diagnostics;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Globals
{
	/// <summary>
	/// Summary description for Globals.
	/// </summary>
	public class g
	{
		//static public DB db = new DB();
		static public String User = "";
        static public PageAccess.AccessLevels UserAccessLevel;
        static public Company Company = new Company();

		public g()
		{
			Debug.WriteLine("Global g");
		}
		~g()
		{
			Debug.WriteLine("Global g~");
		}
	}
}
