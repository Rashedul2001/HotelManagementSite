using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSite.ViewComponents
{
    public class RegisterModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(new RegisterModel());
        }
    }
}