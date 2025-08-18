using HotelManagementSite.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace HotelManagementSite.Interfaces
{
    public interface IUserRepository
    {
        public Task<IEnumerable<IdentityUser>> GetAllIdentityUser();
        public Task AddExternalUserAsync(ExternalLoginInfo info, string IdentityId);
        public Task<User?> GetUserByIdentityIdAsync(string identityId);
        
        // Add new method for admin user creation
        public Task<User> AddUserAsync(string identityId, string name, string email, string? nid = null,
            DateOnly? dateOfBirth = null, string? phoneNumber = null, string? address = null,
            string? about = null, byte[]? profileImage = null, string? profileImageType = null);
        public Task<string> GetUserRole(string identityId);
    }
}