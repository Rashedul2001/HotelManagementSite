using System.Text.RegularExpressions;

namespace HotelManagementSite.Helpers
{
	public static class HelperClass
	{
		public static string CreateSafeUserName(string? name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return "user_" + Guid.NewGuid().ToString("N").Substring(0, 2);

			name = name.Trim().Replace(" ", "_");
			name = Regex.Replace(name, @"[^a-zA-Z0-9\-._@]", "");

			if (string.IsNullOrWhiteSpace(name))
				return "user_" + Guid.NewGuid().ToString("N").Substring(0, 8);

			return name;
		}
		public static string GenerateUniqueUserName(string? name)
		{
			var safeName = CreateSafeUserName(name);
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var random = new Random();
			return $"{ safeName}_" + new string([.. Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)])]);
		}
	}
}
