﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BetterTogether.Models.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public Appointments Appointment { get; set; }
        public List<ApplicationUser> SalesPerson { get; set; }
        public List<Products> Products { get; set; }
    }
}
