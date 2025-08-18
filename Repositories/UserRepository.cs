using System.Security.Claims;
using HotelManagementSite.Data;
using HotelManagementSite.Interfaces;
using HotelManagementSite.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace HotelManagementSite.Repositories
{
    
    public class UserRepository(HotelDbContext hotelContext,UserManager<IdentityUser> userManager) : IUserRepository
    {
        
        public async Task<IEnumerable<IdentityUser>> GetAllIdentityUser(){
           return await userManager.Users.ToListAsync();
        }
        
        public async Task AddExternalUserAsync(ExternalLoginInfo info, string IdentityId)
        {
            var existingUser = await hotelContext.Users.FirstOrDefaultAsync(u => u.IdentityId == IdentityId);
            if (existingUser != null)
                return;
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            DateOnly? dob = null;
            if (info.LoginProvider.Equals("Facebook", StringComparison.OrdinalIgnoreCase))
            {
                var dobString = info.Principal.FindFirstValue("birthday");
                if (!string.IsNullOrEmpty(dobString) && DateOnly.TryParse(dobString, out var parsedDob))
                {
                    dob = parsedDob;
                }
            }

            string[] picKeys = ["picture", "profilePicture", "urn:github:avatarurl"];
            string? pictureUrl = null;
            //currently this only works for Google and GitHub 
            // Facebook and LinkedIn do not provide the picture url in the claims
            // this need to be fixed later
            foreach (var key in picKeys)
            {
                pictureUrl = info.Principal.FindFirstValue(key);
                if (!string.IsNullOrEmpty(pictureUrl))
                    break;
            }


            byte[]? profileImage = null;
            string? profileImageType = null;
            if (!string.IsNullOrEmpty(pictureUrl))
            {
                using var httpClient = new HttpClient();
                try
                {
                    var response = await httpClient.GetAsync(pictureUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        profileImage = await response.Content.ReadAsByteArrayAsync();
                        profileImageType = response.Content.Headers.ContentType?.MediaType;
                    }
                }
                catch
                {
                    profileImage = null;
                    profileImageType = null;
                }
            }
            var hotelUser = new User
            {

                IdentityId = IdentityId,
                Name = name,
                Email = email,
                DateOfBirth = dob,
                ProfileImage = profileImage,
                ProfileImageType = profileImageType,
            };
            await hotelContext.Users.AddAsync(hotelUser);
            await hotelContext.SaveChangesAsync();
        }
        public async Task<User?> GetUserByIdentityIdAsync(string identityId)
        {
            return await hotelContext.Users.FirstOrDefaultAsync(u => u.IdentityId == identityId);
        }

        // Add new method for admin user creation
        public async Task<User> AddUserAsync(string identityId, string name, string email, string? nid = null,
            DateOnly? dateOfBirth = null, string? phoneNumber = null, string? address = null,
            string? about = null, byte[]? profileImage = null, string? profileImageType = null)
        {
            var hotelUser = new User
            {
                IdentityId = identityId,
                Name = name,
                Email = email,
                NID = nid,
                DateOfBirth = dateOfBirth,
                PhoneNumber = phoneNumber,
                Address = address,
                About = about,
                ProfileImage = profileImage,
                ProfileImageType = profileImageType
            };

            await hotelContext.Users.AddAsync(hotelUser);
            await hotelContext.SaveChangesAsync();
            
            return hotelUser;
        }
        public async Task<string> GetUserRole(string identityId)
        {
                    var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == identityId);
                    if (user == null)
                    {
                        return "Unknown";
			        }
            var roles = await userManager.GetRolesAsync(user);
            return  roles.FirstOrDefault() ?? "User";

		}
	}



}