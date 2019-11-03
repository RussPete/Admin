using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Functions to encrypt and decrypt data
	/// </summary>
	public class Crypt
	{
		/// <summary>
		/// Creates a new password for data encryption.
		/// </summary>
		/// <returns>The new password.</returns>
		public static string NewPassword()
		{
			RijndaelManaged RijndaelCipher = new RijndaelManaged();
			return Convert.ToBase64String(RijndaelCipher.Key);
		}

		/// <summary>
		/// Encrypt a text string.
		/// </summary>
		/// <param name="StringToBeEncrypted">Text to be encrypted.</param>
		/// <param name="Password">Password for the encryption.</param>
		/// <returns>The encrypted data string.</returns>
		public static string Encrypt(string StringToBeEncrypted, string Password)
		{

		    RijndaelManaged RijndaelCipher = new RijndaelManaged();
		    RijndaelCipher.Padding = PaddingMode.PKCS7;
			byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(StringToBeEncrypted);
		    byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

		    PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

		    //Creates a symmetric encryptor object.
		    ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

			//Defines a stream that links data streams to cryptographic transformations
			MemoryStream msData = new MemoryStream();
		    CryptoStream csData = new CryptoStream(msData, Encryptor, CryptoStreamMode.Write);
		    csData.Write(PlainText, 0, PlainText.Length);

		    //Writes the final state and clears the buffer
		    csData.FlushFinalBlock();
		    byte[] CipherBytes = msData.ToArray();

		    msData.Close();
		    csData.Close();

            csData.Dispose();
            csData = null;

            msData.Dispose();
            msData = null;

            Encryptor.Dispose();
            Encryptor = null;

            SecretKey = null;
            RijndaelCipher = null;

			string EncryptedData = Convert.ToBase64String(CipherBytes);

		    return EncryptedData;
		}

		/// <summary>
		/// Decrypt data to a text string.
		/// </summary>
		/// <param name="DataToBeDecrypted">Encrypted data to be decrypted.</param>
		/// <param name="Password">Password for decrypting the data, same password used to encrypt it.</param>
		/// <returns>The decrypted string.</returns>
		public static string Decrypt(string DataToBeDecrypted, string Password)
		{

			RijndaelManaged RijndaelCipher = new RijndaelManaged();
			RijndaelCipher.Padding = PaddingMode.PKCS7;
			byte[] EncryptedData = Convert.FromBase64String(DataToBeDecrypted);
			byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

			//Making of the key for decryption
			PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

			//Creates a symmetric Rijndael decryptor object.
			ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

			//Defines the cryptographics stream for decryption.THe stream contains decrpted data
			MemoryStream msData = new MemoryStream(EncryptedData);
			CryptoStream csData = new CryptoStream(msData, Decryptor, CryptoStreamMode.Read);

			byte[] PlainText = new byte[EncryptedData.Length];
			int DecryptedCount = csData.Read(PlainText, 0, PlainText.Length);

			msData.Close();
			csData.Close();

            csData.Dispose();
            csData = null;

            msData.Dispose();
            msData = null;

            Decryptor.Dispose();
            Decryptor = null;

            SecretKey = null;
            RijndaelCipher = null;

            //Converting to string
			string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

		    return DecryptedData;
		}
	}
}
