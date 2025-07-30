using System.Text.RegularExpressions;

namespace HotelManagementSite.Helpers
{
	public static class HelperClass
	{
		// return safe name by removing special characters, replacing spaces with underscores, and ensuring it is not empty
		// if the name is null or empty, generate a unique name using a GUID

		public static string CreateSafeUserName(string? name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return "user_" + Guid.NewGuid().ToString("N").Substring(0, 6);

			name = name.Trim().Replace(" ", "_");
			name = Regex.Replace(name, @"[^a-zA-Z0-9\-._@]", "");

			if (string.IsNullOrWhiteSpace(name))
				return "user_" + Guid.NewGuid().ToString("N").Substring(0, 8);

			return name;
		}
		// generate a unique username by appending a random string to the safe name
		// the random string is 6 characters long and consists of alphanumeric characters
		public static string GenerateUniqueUserName(string? name)
		{
			var safeName = CreateSafeUserName(name);
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var random = new Random();
			return $"{ safeName}_" + new string([.. Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)])]);
		}
	}
}
