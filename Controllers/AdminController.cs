using System.Globalization;
using HotelManagementSite.Helpers;
using HotelManagementSite.Interfaces;
using HotelManagementSite.Models.Temp;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// fix the Actions Those are not mine to use they are here for using the Views for room user list view

namespace HotelManagementSite.Controllers
{
    public class AdminController(IAuthAccountRepository authRepo, IUserRepository userRepo) : Controller
    {
        private readonly IAuthAccountRepository authRepo = authRepo;
        private readonly IUserRepository userRepo = userRepo;

		[Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Index()
        {
            ViewBag.CurrentAction = "Index";
            return View();
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateUser([FromForm] UserCreationModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values
						.SelectMany(v => v.Errors)
						.Select(e => e.ErrorMessage)
						.ToList();

					return Json(new
					{
						success = false,
						message = "Validation failed",
						errors
					});
				}

				// Check if user already exists
				var existingUser = await  authRepo.FindUserByEmailAsync(model.Email);
				if (existingUser != null)
				{
					return Json(new
					{
						success = false,
						message = "User with this email already exists"
					});
				}

				// Handle profile image
				byte[]? profileImage = null;
				string? profileImageType = null;

				if (model.ProfileImage != null && model.ProfileImage.Length > 0)
				{
					// Validate image file
					var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
					if (!allowedTypes.Contains(model.ProfileImage.ContentType.ToLower()))
					{
						return Json(new
						{
							success = false,
							message = "Invalid image format. Only JPEG, PNG, and GIF are allowed."
						});
					}

					// Check file size (limit to 5MB)
					if (model.ProfileImage.Length > 5 * 1024 * 1024)
					{
						return Json(new
						{
							success = false,
							message = "Image file size cannot exceed 5MB."
						});
					}

					using var memoryStream = new MemoryStream();
					await model.ProfileImage.CopyToAsync(memoryStream);
					profileImage = memoryStream.ToArray();
					profileImageType = model.ProfileImage.ContentType;
				}

				// Create Identity user
				var identityResult = await authRepo.CreateUserAsync(model.Email, model.Password, model.Name, model.Role);

				if (!identityResult.Succeeded)
				{
					var errors = identityResult.Errors.Select(e => e.Description).ToList();
					return Json(new
					{
						success = false,
						message = "Failed to create user account",
						errors
					});
				}

				// Get the created identity user
				var identityUser = await authRepo.FindUserByEmailAsync(model.Email);
				if (identityUser == null)
				{
					return Json(new
					{
						success = false,
						message = "User creation failed"
					});
				}

				// Create hotel user
				var hotelUser = await userRepo.AddUserAsync(
					identityUser.Id,
					model.Name,
					model.Email,
					model.NID,
					model.DateOfBirth,
					model.PhoneNumber,
					model.Address,
					model.About,
					profileImage,
					profileImageType
				);

				return Json(new
				{
					success = true,
					message = $"User '{model.Name}' created successfully!",
					user = new
					{
						id = hotelUser.Id,
						name = hotelUser.Name,
						email = hotelUser.Email,
						role = model.Role
					}
				});
			}
			catch (Exception ex)
			{
				// Log the exception here
				return Json(new
				{
					success = false,
					message = "An error occurred while creating the user. Please try again."
				});
			}
		}
		public async Task<IActionResult> Users()
		{
			ViewBag.CurrentAction = "Users";
			var allIdentityUser =await	userRepo.GetAllIdentityUser();
			var users = new List<UserModel>();
			foreach( var identityUser in allIdentityUser)
			{
				var user = await userRepo.GetUserByIdentityIdAsync(identityUser.Id);
				if (user != null)
				{
					users.Add(new UserModel
					{
						Id = user.Id,
						Name = user.Name,
						Email = user.Email,
						Role = await userRepo.GetUserRole(user.IdentityId),
						PhoneNumber = user.PhoneNumber ?? "Not Provided",
						Address = user.Address ?? "No Address Provided",
					});
				}
			}



			return View(users);
		}



		// Temporary storage for users and rooms 
		// In a real application, this data would come from a database
		private static List<User> _users = GetInitialUsers();
        private static List<Room> _rooms = GetInitialRooms();


     

        public IActionResult Rooms(string searchTerm = "", string sortBy = "number", int page = 1)
        {
            var rooms = _rooms.AsQueryable();

            // Filter rooms
            if (!string.IsNullOrEmpty(searchTerm))
            {
                rooms = rooms.Where(r =>
                    r.Number.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    r.Type.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    r.Status.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Sort rooms
            rooms = sortBy.ToLower() switch
            {
                "type" => rooms.OrderBy(r => r.Type),
                "status" => rooms.OrderBy(r => r.Status),
                "price" => rooms.OrderBy(r => r.Price),
                _ => rooms.OrderBy(r => r.Number)
            };

            var roomsList = rooms.ToList();
            var itemsPerPage = 4;
            var totalRooms = roomsList.Count;
            var totalPages = (int)Math.Ceiling((double)totalRooms / itemsPerPage);
            var paginatedRooms = roomsList.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            var viewModel = new RoomViewModel
            {
                Rooms = paginatedRooms,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalRooms = totalRooms,
                ItemsPerPage = itemsPerPage
            };

            ViewBag.CurrentAction = "Rooms";
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateRoom([FromBody] Room room)
        {
            if (ModelState.IsValid)
            {
                room.Id = _rooms.Count > 0 ? _rooms.Max(r => r.Id) + 1 : 1;

                // Parse amenities from string
                if (!string.IsNullOrEmpty(room.AmenitiesString))
                {
                    room.Amenities = room.AmenitiesString.Split(',')
                        .Select(a => a.Trim())
                        .Where(a => !string.IsNullOrEmpty(a))
                        .ToList();
                }

                _rooms.Add(room);

                return Json(new { success = true, message = "Room created successfully!" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = errors });
        }

        private static List<User> GetInitialUsers()
        {
            return new List<User>
            {
                new User { Id = 1, Name = "John Doe", Email = "john@example.com", Role = "Manager", Status = "Active", JoinDate = new DateTime(2024, 1, 15) },
                new User { Id = 2, Name = "Sarah Wilson", Email = "sarah@example.com", Role = "Receptionist", Status = "Active", JoinDate = new DateTime(2024, 2, 20) },
                new User { Id = 3, Name = "Mike Johnson", Email = "mike@example.com", Role = "Housekeeping", Status = "Inactive", JoinDate = new DateTime(2024, 1, 10) },
                new User { Id = 4, Name = "Emily Davis", Email = "emily@example.com", Role = "Manager", Status = "Active", JoinDate = new DateTime(2024, 3, 5) },
                new User { Id = 5, Name = "David Brown", Email = "david@example.com", Role = "Security", Status = "Active", JoinDate = new DateTime(2024, 2, 28) }
            };
        }

        private static List<Room> GetInitialRooms()
        {
            return new List<Room>
            {
                new Room { Id = 101, Number = "101", Type = "Standard", Status = "Available", Price = 120, Capacity = 2, Floor = 1, Amenities = new List<string> { "WiFi", "AC", "TV" } },
                new Room { Id = 102, Number = "102", Type = "Deluxe", Status = "Occupied", Price = 180, Capacity = 3, Floor = 1, Amenities = new List<string> { "WiFi", "AC", "TV", "Mini Bar" } },
                new Room { Id = 201, Number = "201", Type = "Suite", Status = "Available", Price = 300, Capacity = 4, Floor = 2, Amenities = new List<string> { "WiFi", "AC", "TV", "Mini Bar", "Balcony" } },
                new Room { Id = 202, Number = "202", Type = "Standard", Status = "Maintenance", Price = 120, Capacity = 2, Floor = 2, Amenities = new List<string> { "WiFi", "AC", "TV" } },
                new Room { Id = 301, Number = "301", Type = "Presidential", Status = "Available", Price = 500, Capacity = 6, Floor = 3, Amenities = new List<string> { "WiFi", "AC", "TV", "Mini Bar", "Balcony", "Jacuzzi" } }
            };
        }
        public IActionResult TestPage(string searchTerm = "", string sortBy = "name", int page = 1){
			var users = _users.AsQueryable();

			// Filter users
			if (!string.IsNullOrEmpty(searchTerm))
			{
				users = users.Where(u =>
					u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
					u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
					u.Role.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
				);
			}

			// Sort users
			users = sortBy.ToLower() switch
			{
				"role" => users.OrderBy(u => u.Role),
				"status" => users.OrderBy(u => u.Status),
				"joindate" => users.OrderBy(u => u.JoinDate),
				_ => users.OrderBy(u => u.Name)
			};

			var usersList = users.ToList();
			var itemsPerPage = 4;
			var totalUsers = usersList.Count;
			var totalPages = (int)Math.Ceiling((double)totalUsers / itemsPerPage);
			var paginatedUsers = usersList.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var viewModel = new UserViewModel
			{
				Users = paginatedUsers,
				SearchTerm = searchTerm,
				SortBy = sortBy,
				CurrentPage = page,
				TotalPages = totalPages,
				TotalUsers = totalUsers,
				ItemsPerPage = itemsPerPage
			};

			ViewBag.CurrentAction = "Users";
			return View(viewModel);
		}
    }

}