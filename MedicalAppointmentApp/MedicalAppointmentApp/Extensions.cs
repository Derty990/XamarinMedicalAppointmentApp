using System;
using Xamarin.Forms;

namespace MedicalAppointmentApp
{
    // Klasa musi być static
    public static class Extensions
    {
        // Metoda musi być static
        // this Page page - oznacza, że rozszerzamy klasę Page
        public static void AddMenuButton(this Page page)
        {
            if (page.ToolbarItems.Count == 0)
            {
                page.ToolbarItems.Add(new ToolbarItem
                {
                    Text = "≡", // Symbol menu hamburger
                    Order = ToolbarItemOrder.Primary,
                    Command = new Command(() =>
                    {
                        if (Application.Current.MainPage is FlyoutPage flyoutPage)
                        {
                            flyoutPage.IsPresented = true;
                        }
                    })
                });
            }
        }
    }
}