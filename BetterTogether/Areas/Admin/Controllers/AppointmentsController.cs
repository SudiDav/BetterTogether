using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterTogether.Data;
using BetterTogether.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterTogether.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.SuperAdminUser + "," + StaticDetails.AdminUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}