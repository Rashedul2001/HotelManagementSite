using System.Text.RegularExpressions;

namespace HotelManagementSite.Helpers
{
	public static class HelperClass
	{
		public static string CreateSafeUserName(string? name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return "user_" + Guid.NewGuid().ToString("N").Substring(0, 8);

			name = name.Trim().Replace(" ", "_");
			name = Regex.Replace(name, @"[^a-zA-Z0-9\-._@]", "");

			if (string.IsNullOrWhiteSpace(name))
				return "user_" + Guid.NewGuid().ToString("N").Substring(0, 8);

			return name;
		}
	}
}
