using HotelManagementSite.Helpers;
using HotelManagementSite.Interfaces;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelManagementSite.Models.Temp;

// fix the Actions Those are not mine to use they are here for using the Views for room user list view

namespace HotelManagementSite.Controllers
{
    public class AdminController(IAuthAccountRepository authRepo) : Controller
    {
        private static List<User> _users = GetInitialUsers();
        private static List<Room> _rooms = GetInitialRooms();


        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Index()
        {
            ViewBag.CurrentAction = "Index";
            return View();
        }

        public IActionResult Users(string searchTerm = "", string sortBy = "name", int page = 1)
        {

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

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                user.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
                user.JoinDate = DateTime.Now;
                _users.Add(user);

                return Json(new { success = true, message = "User created successfully!" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = errors });
        }

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
    }

}