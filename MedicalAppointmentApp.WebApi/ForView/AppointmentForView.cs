using MedicalAppointmentApp.WebApi.Models;
using MedicalAppointmentApp.WebApi.Helpers;
using System;

namespace MedicalAppointmentApp.WebApi.ForView
{
    public class AppointmentForView
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public int StatusId { get; set; }

        public string PatientFullName { get; set; }
        public string DoctorFullName { get; set; }
        public string ClinicName { get; set; }
        public string StatusName { get; set; }

        public static implicit operator AppointmentForView(Appointment app)
        {
            if (app == null) return null;

            var forView = new AppointmentForView().CopyProperties(app);

            forView.PatientFullName = $"{app.Patient?.FirstName} {app.Patient?.LastName}".Trim();
            forView.DoctorFullName = $"{app.Doctor?.User?.FirstName} {app.Doctor?.User?.LastName}".Trim(); 
            forView.ClinicName = app.Clinic?.Name;
            forView.StatusName = app.Status?.StatusName;

            return forView;
        }
    }
}