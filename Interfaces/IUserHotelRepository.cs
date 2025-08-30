using HotelManagementSite.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace HotelManagementSite.Interfaces
{
    public interface IUserHotelRepository
    {
         Task AddExternalUserAsync(ExternalLoginInfo info, string IdentityId);
         Task<User?> GetUserByIdentityIdAsync(string identityId);
         Task CreateUserAsync(IdentityUser user);
         Task CreateUserAsync(User user);
        

        
        
        // Add new method for admin user creation
        Task<User> AddUserAsync(string identityId, string name, string email, string? nid = null,
            DateOnly? dateOfBirth = null, string? phoneNumber = null, string? address = null,
            string? about = null, byte[]? profileImage = null, string? profileImageType = null);
    }
}