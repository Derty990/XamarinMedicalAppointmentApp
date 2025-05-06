using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalAppointmentApp.Models 
{
    public class UserSelectItem
    {
        public int UserId { get; set; }
        public string FullName { get; set; } // e.g., "Jan Kowalski (ID: 5)"
    }
}