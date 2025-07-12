namespace HotelManagementSite.Models.ViewModels
{
    public class UserIconViewModel
    {
        public string Name { get; set; } = string.Empty;
        public byte[]? ProfileImage { get; set; }
        public string? ProfileImageType { get; set; }
    }
}