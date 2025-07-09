using System.Security.Claims;
using HotelManagementSite.Data;
using HotelManagementSite.Interfaces;
using HotelManagementSite.Models.Domain;
using Microsoft.AspNetCore.Identity;
namespace HotelManagementSite.Repositories
{
    public class UserRepository(HotelDbContext hotelContext) : IUserRepository
    {
        public async Task AddExternalUserAsync(ExternalLoginInfo info, string IdentityId)
        {
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


    }
}