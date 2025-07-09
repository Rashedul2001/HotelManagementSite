using HotelManagementSite.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSite.Controllers
{
    public class UserController(IUserRepository userRepo , IAuthAccountRepository authAcRepo) :Controller 
    {


    }
}