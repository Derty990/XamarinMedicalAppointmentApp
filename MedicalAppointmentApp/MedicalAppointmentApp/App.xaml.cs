using MedicalAppointmentApp.XamarinApp.ApiClient; 
using MedicalAppointmentApp.XamarinApp.Services; 
using MedicalAppointmentApp.XamarinApp.Services.Abstract; 
using MedicalAppointmentApp.Views; 
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using MedicalAppointmentApp.Services.Abstract;
using System.Net.Http;

namespace MedicalAppointmentApp
{
    public partial class App : Application
    {
        public static MedicalApiClient ApiClient { get; private set; }

        public App()
        {
            InitializeComponent();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5268")

            };

            ApiClient = new MedicalApiClient("http://localhost:5268", httpClient);

            DependencyService.RegisterSingleton<MedicalApiClient>(ApiClient);
            DependencyService.Register<IUserService, UserDataStore>();
            DependencyService.Register<IAppointmentService, AppointmentDataStore>();
            DependencyService.Register<IDoctorService, DoctorDataStore>();
            DependencyService.Register<IClinicService, ClinicDataStore>();
            DependencyService.Register<IAddressService, AddressDataStore>();
            DependencyService.Register<ISpecializationService, SpecializationDataStore>();
            DependencyService.Register<IAppointmentStatusService, AppointmentStatusDataStore>();

            var loginPage = new LoginPage();

            var navPage = new NavigationPage(loginPage);

            MainPage = navPage;

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }

}