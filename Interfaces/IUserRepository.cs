using Microsoft.AspNetCore.Identity;

namespace HotelManagementSite.Interfaces
{
    public interface IUserRepository
    {
        public Task AddExternalUserAsync(ExternalLoginInfo info , string IdentityId);
        
    }
    
}