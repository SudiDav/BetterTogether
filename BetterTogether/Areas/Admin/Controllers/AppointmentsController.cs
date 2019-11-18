using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterTogether.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterTogether.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.SuperAdminUser + "," + StaticDetails.AdminUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}