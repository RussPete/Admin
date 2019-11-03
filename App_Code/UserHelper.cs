using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{ 
    public static class UserHelper
    {
        public static string Generate256HashOnString(string str)
        {
            string passwordHashed = "";
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            SHA256Managed sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(bytes);
            foreach (byte x in hash)
            {
                passwordHashed += string.Format("{0:x2}", x);
            }

            return passwordHashed;
        }

        public static string GeneratePasswordSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[32];
            rng.GetBytes(buffer);
            string salt = BitConverter.ToString(buffer).Replace("-", "");

            return salt;
        }

        public static string GetSaltForUser(string userName)
        {
            DB db = new DB();
            string salt = string.Empty;

            string sql = string.Format("SELECT PasswordSalt FROM Users WHERE UserName COLLATE Latin1_General_CS_AS = {0}", DB.PutStr(userName));
            DataRow dr = db.DataRow(sql);

            if (dr != null)
            {
                salt = (string)dr[0];
            }

            return salt;
        }

        public static string GetPasswordKeyForUser(string userName)
        {
            DB db = new DB();
            string key = string.Empty;

            string sql = string.Format("SELECT Password FROM Users WHERE UserName COLLATE Latin1_General_CS_AS = {0}", DB.PutStr(userName));
            DataRow dr = db.DataRow(sql);

            if (dr != null)
            {
                key = (string)dr[0];
            }

            return key;
        }

        public static Boolean IsAdminUser(string userName)
        {
            DB db = new DB();
            bool isAdmin = false;

            string sql = string.Format("SELECT AdminFlg FROM Users WHERE UserName COLLATE Latin1_General_CS_AS = {0}", DB.PutStr(userName));
            DataRow dr = db.DataRow(sql);

            if (dr != null)
            {
                isAdmin = (bool)dr[0];
            }

            return isAdmin;
        }

        public static string getDisplayNameForUser(string userName)
        {
            DB db = new DB();
            string displayName = string.Empty;

            string sql = string.Format("SELECT UserCommonName FROM Users WHERE UserName COLLATE Latin1_General_CS_AS = {0}", DB.PutStr(userName));
            DataRow dr = db.DataRow(sql);

            if (dr != null)
            {
                displayName = (string)dr[0];
            }

            return displayName;
        }
    }
}