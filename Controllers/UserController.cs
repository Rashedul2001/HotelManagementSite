using HotelManagementSite.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSite.Controllers
{
    public class UserController(IUserHotelRepository userRepo , IAuthAccountRepository authAcRepo) :Controller 
    {


    }
}