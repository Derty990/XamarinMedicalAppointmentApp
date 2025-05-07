using MedicalAppointmentApp.XamarinApp.ApiClient; 
using MedicalAppointmentApp.XamarinApp.Services.Abstract;

namespace MedicalAppointmentApp.XamarinApp.Services.Abstract
{
   
    public interface IAppointmentStatusService : IDataStore<AppointmentStatusForView> 
    {
       
    }
}