using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Donteco.Security
{
	public static class StringCipher
	{
		public static string Encrypt(string plainText, string passPhrase)
		{
			byte[] array = Generate256BitsOfRandomEntropy();
			byte[] array2 = Generate256BitsOfRandomEntropy();
			byte[] bytes = Encoding.UTF8.GetBytes(plainText);
			string result;
			using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passPhrase, array, 1000))
			{
				byte[] bytes2 = rfc2898DeriveBytes.GetBytes(16);
				using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
				{
					rijndaelManaged.BlockSize = 128;
					rijndaelManaged.Mode = CipherMode.CBC;
					rijndaelManaged.Padding = PaddingMode.PKCS7;
					using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor(bytes2, array2))
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
							{
								cryptoStream.Write(bytes, 0, bytes.Length);
								cryptoStream.FlushFinalBlock();
								byte[] inArray = array.Concat(array2).ToArray().Concat(memoryStream.ToArray()).ToArray();
								memoryStream.Close();
								cryptoStream.Close();
								result = Convert.ToBase64String(inArray);
							}
						}
					}
				}
			}
			return result;
		}

		public static string Decrypt(string cipherText, string passPhrase)
		{
			byte[] array = Convert.FromBase64String(cipherText);
			byte[] salt = array.Take(16).ToArray();
			byte[] rgbIV = array.Skip(16).Take(16).ToArray();
			byte[] array2 = array.Skip(32).Take(array.Length - 32).ToArray();
			string @string;
			using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passPhrase, salt, 1000))
			{
				byte[] bytes = rfc2898DeriveBytes.GetBytes(16);
				using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
				{
					rijndaelManaged.BlockSize = 128;
					rijndaelManaged.Mode = CipherMode.CBC;
					rijndaelManaged.Padding = PaddingMode.PKCS7;
					using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(bytes, rgbIV))
					{
						using (MemoryStream memoryStream = new MemoryStream(array2))
						{
							using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
							{
								byte[] array3 = new byte[array2.Length];
								int count = cryptoStream.Read(array3, 0, array3.Length);
								memoryStream.Close();
								cryptoStream.Close();
								@string = Encoding.UTF8.GetString(array3, 0, count);
							}
						}
					}
				}
			}
			return @string;
		}

		private static byte[] Generate256BitsOfRandomEntropy()
		{
			byte[] array = new byte[16];
			using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				rngcryptoServiceProvider.GetBytes(array);
			}
			return array;
		}

		private const int Keysize = 128;

		private const int DerivationIterations = 1000;
	}
}
