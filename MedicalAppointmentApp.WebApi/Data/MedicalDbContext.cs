using MedicalAppointmentApp.WebApi.Entities; 
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MedicalAppointmentApp.WebApi.Data 
{
    public class MedicalDbContext : DbContext
    {
        // Konstruktor wymagany do konfiguracji przez Dependency Injection
        public MedicalDbContext(DbContextOptions<MedicalDbContext> options) : base(options)
        {
        }

        // DbSet dla każdej encji - reprezentują tabele w bazie danych
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<DoctorClinic> DoctorClinics { get; set; } // Tabela łącząca, przykład wiele do wielu

        
    }
}