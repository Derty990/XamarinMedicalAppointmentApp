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

        // Dodatkowe, "spłaszczone" pola dla wygody klienta
        public string PatientFullName { get; set; }
        public string DoctorFullName { get; set; }
        public string ClinicName { get; set; }
        public string StatusName { get; set; }

        // Operator konwersji z Encji Appointment
        // Wymaga ręcznego mapowania pól spłaszczonych!
        public static implicit operator AppointmentForView(Appointment app)
        {
            if (app == null) return null;

            // Najpierw skopiuj pasujące pola
            var forView = new AppointmentForView().CopyProperties(app);

            // Potem ręcznie wypełnij spłaszczone dane
            // WAŻNE: To zadziała tylko, jeśli w kontrolerze załadowano powiązane encje przez .Include()!
            forView.PatientFullName = $"{app.Patient?.FirstName} {app.Patient?.LastName}".Trim();
            forView.DoctorFullName = $"{app.Doctor?.User?.FirstName} {app.Doctor?.User?.LastName}".Trim(); // Zagnieżdżenie User->Doctor
            forView.ClinicName = app.Clinic?.Name;
            forView.StatusName = app.Status?.StatusName;

            return forView;
        }

        // Konwersja w drugą stronę (AppointmentForView -> Appointment) jest trudniejsza
        // z powodu spłaszczonych pól i zazwyczaj nie jest potrzebna/zalecana.
        // Do tworzenia/aktualizacji użyj dedykowanych DTO lub mapuj ręcznie w kontrolerze.
    }
}