// need to add the history later
using HotelManagementSite.Models.Domain;

namespace HotelManagementSite.Models.ViewModels
{
    public class ProfileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? NID { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? About { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string? ProfileImageType { get; set; }
        public string? Role { get; set; }
        public ICollection<Account>? Accounts { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool MobileNumberVerified { get; set; }
        public bool EmailVerified { get; set; }


    }
}
