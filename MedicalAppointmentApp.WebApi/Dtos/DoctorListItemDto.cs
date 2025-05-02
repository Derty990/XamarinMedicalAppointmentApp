namespace MedicalAppointmentApp.WebApi.Dtos
{
    // Prostsze DTO do wyświetlania lekarzy na liście
    public class DoctorListItemDto
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; } // Połączone FirstName + LastName z User
        public string SpecializationName { get; set; } 
    }
}