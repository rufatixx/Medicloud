using System.Security.Cryptography;
using System.Text;

namespace Medicloud.BLL.Utils;

public class HMACUtils
{
	public static string ComputeHMACSHA256(string message, string secretKey)
	{
		byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
		byte[] messageBytes = Encoding.UTF8.GetBytes(message);

		using HMACSHA256 hmac = new HMACSHA256(keyBytes);
		byte[] hashBytes = hmac.ComputeHash(messageBytes);
		return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
	}
}
