using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSite.ViewComponents
{
    public class LogInModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new LogInModel();
            return View(model);
        }
    }
}