using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentApp.WebApi.Models 
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }

        [Required]
        public int PatientId { get; set; } 

        [Required]
        public int DoctorId { get; set; } 

        [Required]
        public int ClinicId { get; set; } 

        [Required]
        [Column(TypeName = "date")] // Mapowanie na typ DATE w SQL
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Column(TypeName = "time")] // Mapowanie na typ TIME w SQL
        public TimeSpan StartTime { get; set; } 

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        public int StatusId { get; set; } 

        [ForeignKey("PatientId")]
        public virtual User Patient { get; set; } 

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [ForeignKey("ClinicId")]
        public virtual Clinic Clinic { get; set; }

        [ForeignKey("StatusId")]
        public virtual AppointmentStatus Status { get; set; }
    }
}