using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Medicloud.BLL.Utils
{
	public class HashHelper
	{
		public string HashOtp(string otp)
		{
			using var sha256 = SHA256.Create();
			var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(otp));
			return Convert.ToBase64String(bytes);
		}

		public string GenerateOtp()
		{
			var otp = new Random().Next(1000, 10000).ToString();
			return otp;
		}
		public string sha256(string randomString)
		{
			//var crypt = new System.Security.Cryptography.SHA256Managed();
			//var hash = new System.Text.StringBuilder();
			//byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
			//foreach (byte theByte in crypto)
			//{
			//	hash.Append(theByte.ToString("x2"));
			//}
			//return hash.ToString();

			using var sha256 = SHA256.Create();
			var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(randomString));

			// Hexadecimal formatında döndürme
			StringBuilder hexString = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes)
			{
				hexString.AppendFormat("{0:x2}", b);
			}

			return hexString.ToString();
		}



	}
}
