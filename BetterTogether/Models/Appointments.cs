using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BetterTogether.Models
{
    public class Appointments
    {
        public int Id { get; set; }

        [Display(Name = "Sales Person")]
        public string SalesPersonId { get; set; }

        [ForeignKey("SalesPersonId")]
        public virtual ApplicationUser SalesPerson { get; set; }

        public DateTime AppointmentDate { get; set; }

        [NotMapped]
        public DateTime AppointmentTime { get; set; }

        public string CustName { get; set; }
        public string CustPhoneNumber { get; set; }
        public string CustEmail { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
