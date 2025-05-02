using MedicalAppointmentApp.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointmentApp.WebApi.Data 
{
   
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

       
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorClinic> DoctorClinics { get; set; } // Tabela łącząca

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Konfiguracja indeksów unikalnych ---
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Specialization>() // Dodano dla pewności
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<AppointmentStatus>() // Dodano dla pewności
                .HasIndex(aps => aps.StatusName)
                .IsUnique();

            // --- Konfiguracja relacji jeden-do-jeden (User <-> Doctor) ---
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor) // Zakłada istnienie właściwości 'Doctor' w klasie User
                .HasForeignKey<Doctor>(d => d.UserId);

            // --- Konfiguracja relacji wiele-do-wielu (Doctor <-> Clinic przez DoctorClinic) ---
            // Używamy klucza głównego surrogate DoctorClinicId, więc nie konfigurujemy klucza złożonego tutaj.
            // Relacje FK są konfigurowane poniżej:
            modelBuilder.Entity<DoctorClinic>()
                .HasOne(dc => dc.Doctor)
                .WithMany(d => d.DoctorClinics) // Zakłada ICollection<DoctorClinic> w Doctor
                .HasForeignKey(dc => dc.DoctorId);

            modelBuilder.Entity<DoctorClinic>()
                .HasOne(dc => dc.Clinic)
                .WithMany(c => c.DoctorClinics) // Zakłada ICollection<DoctorClinic> w Clinic
                .HasForeignKey(dc => dc.ClinicId);

            // --- Konfiguracja unikalności par w DoctorClinic (ważne dla tabeli łączącej) ---
            modelBuilder.Entity<DoctorClinic>()
               .HasIndex(dc => new { dc.DoctorId, dc.ClinicId })
               .IsUnique();

            // --- Konfiguracja zachowania przy usuwaniu (OnDelete) dla Appointment ---
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(u => u.PatientAppointments) // Zakłada ICollection<Appointment> PatientAppointments w User
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // Zapobiega kaskadowemu usuwaniu wizyt przy usuwaniu pacjenta

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments) // Zakłada ICollection<Appointment> Appointments w Doctor
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict); // Zapobiega kaskadowemu usuwaniu wizyt przy usuwaniu lekarza

            modelBuilder.Entity<Appointment>()
               .HasOne(a => a.Clinic)
               .WithMany(c => c.Appointments) // Zakłada ICollection<Appointment> Appointments w Clinic
               .HasForeignKey(a => a.ClinicId)
               .OnDelete(DeleteBehavior.Restrict); // Zapobiega kaskadowemu usuwaniu wizyt przy usuwaniu kliniki

            modelBuilder.Entity<Appointment>()
               .HasOne(a => a.Status)
               .WithMany(aps => aps.Appointments) // Zakłada ICollection<Appointment> Appointments w AppointmentStatus
               .HasForeignKey(a => a.StatusId)
               .OnDelete(DeleteBehavior.Restrict); // Zazwyczaj statusów się nie usuwa, ale na wszelki wypadek

            // --- Konfiguracja typów danych dla SQL Server ---
            modelBuilder.Entity<Appointment>()
               .Property(a => a.StartTime)
               .HasColumnType("time");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.EndTime)
                .HasColumnType("time");

            modelBuilder.Entity<Appointment>()
               .Property(a => a.AppointmentDate)
               .HasColumnType("date");

            // Można tu dodać więcej konfiguracji Fluent API w miarę potrzeb
            // np. precyzję dla typów decimal, wartości domyślne, etc.
        }
    }
}