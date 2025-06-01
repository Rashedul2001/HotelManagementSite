using System.ComponentModel.DataAnnotations;

namespace HotelManagementSite.Models.Domain
{
    public class Amenity
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public byte[]? Icon { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<HotelRoom> HotelRooms { get; set; } 
    }
}