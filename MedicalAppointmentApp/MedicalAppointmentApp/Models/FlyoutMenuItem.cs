﻿using System;

namespace MedicalAppointmentApp.Models
{
    public class FlyoutMenuItem
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public Type TargetType { get; set; }
        
        // public string IconSource { get; set; }
    }
}