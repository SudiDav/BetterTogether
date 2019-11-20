using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BetterTogether.Data;
using BetterTogether.Models;
using BetterTogether.Models.ViewModels;
using BetterTogether.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetterTogether.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.SuperAdminUser + "," + StaticDetails.AdminUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private int PageSize = 3;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int productPage = 1,string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewModel appointmentViewModel = new AppointmentViewModel()
            {
                Appointments = new List<Models.Appointments>()
            };

            StringBuilder param = new StringBuilder();

            param.Append("/Admin/Appointments?productPage=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchDate=");
            if (searchDate != null)
            {
                param.Append(searchDate);
            }




            appointmentViewModel.Appointments = _context.Appointments.Include(a => a.SalesPerson).ToList();
            if (User.IsInRole(StaticDetails.AdminUser))
            {
                appointmentViewModel.Appointments = appointmentViewModel.Appointments
                                                                        .Where(a => a.SalesPersonId == claim.Value)
                                                                        .ToList();
            } 

            if (searchName != null)
            {
                appointmentViewModel.Appointments = appointmentViewModel.Appointments
                                                                        .Where(a => a.CustName.ToLower()
                                                                        .Contains(searchName.ToLower()))
                                                                        .ToList();
            }
            if (searchEmail != null)
            {
                appointmentViewModel.Appointments = appointmentViewModel.Appointments
                                                                        .Where(a => a.CustEmail.ToLower()
                                                                        .Contains(searchEmail.ToLower()))
                                                                        .ToList();
            }
            if (searchPhone != null)
            {
                appointmentViewModel.Appointments = appointmentViewModel.Appointments
                                                                        .Where(a => a.CustPhoneNumber.ToLower()
                                                                        .Contains(searchPhone.ToLower()))
                                                                        .ToList();
            }
            if (searchDate != null)
            {
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);
                    appointmentViewModel.Appointments = appointmentViewModel.Appointments
                                                                            .Where(a => a.AppointmentDate.ToShortDateString()
                                                                            .Equals(appDate.ToShortDateString()))
                                                                            .ToList();
                }
                catch (Exception ex)
                {

                }

            }

            var count = appointmentViewModel.Appointments.Count;

            //appointmentViewModel.Appointments = appointmentViewModel.Appointments.OrderBy(p => p.AppointmentDate)
            //    .Skip((productPage - 1) * PageSize)
            //    .Take(PageSize).ToList();


            //appointmentViewModel.PagingInfo = new PagingInfo
            //{
            //    CurrentPage = productPage,
            //    ItemsPerPage = PageSize,
            //    TotalItems = count,
            //    urlParam = param.ToString()
            //};


            return View(appointmentViewModel);
           
        }

        //Get action method for appointment
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _context.Products
                                                      join x in _context.ProductsAppointments
                                                      on p.Id equals x.ProductId
                                                      where x.AppointmentId  == id
                                                      select p).Include("ProductTypes");
            var appointmentDetailsViewModel = new AppointmentDetailsViewModel
            {

                Appointment = _context.Appointments.Include(a => a.SalesPerson).Where(z => z.Id == id).FirstOrDefault(),
                SalesPerson = _context.ApplicationUser.ToList(),
                Products = productList.ToList()
            };
            return View(appointmentDetailsViewModel);
        }
    }
}