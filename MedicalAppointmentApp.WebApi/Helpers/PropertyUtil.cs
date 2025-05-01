
using System;
using System.Collections.Generic; 
using System.Linq; 
using System.Reflection;

namespace MedicalAppointmentApp.WebApi.Helpers 
{
    public static class PropertyUtil
    {
        public static T CopyProperties<T, T2>(this T targetObject, T2 sourceObject)
        {
            if (sourceObject == null) return targetObject; // Dodano zabezpieczenie przed nullem

            targetObject.GetTypeProperties().Where(p => p.CanWrite).ToList()
                .ForEach(property => FindAndReplaceProperty(targetObject, sourceObject, property));
            return targetObject;
        }

        private static void FindAndReplaceProperty<T, T2>(T targetObject, T2 sourceObject, PropertyInfo property)
        {
            var sourceProperties = sourceObject.GetTypeProperties();
            var sourceProperty = sourceProperties
                                    .FirstOrDefault(prop => CheckIfPropertyExistInSource(prop, property));

            if (sourceProperty != null)
            {
                // Sprawdź, czy typy są przypisywalne (obsługuje nullable itp.)
                if (property.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    try
                    { // Dodano try-catch na wypadek problemów z GetValue
                        property.SetValue(targetObject, sourceProperty.GetValue(sourceObject), null);
                    }
                    catch (Exception ex)
                    {
                        // Można dodać logowanie błędu kopiowania konkretnej właściwości
                        Console.WriteLine($"Error copying property {property.Name}: {ex.Message}");
                    }
                }
            }
        }
        private static IEnumerable<PropertyInfo> GetTypeProperties<T2>(this T2 sourceObject)
            => sourceObject.GetType().GetProperties();

        private static bool CheckIfPropertyExistInSource(PropertyInfo prop, PropertyInfo property)
            // Porównanie nazw jest już odporne na wielkość liter, ale zostawmy dla pewności
            => string.Equals(property.Name, prop.Name, StringComparison.InvariantCultureIgnoreCase);
        // Usunięto sprawdzanie typu - CopyProperties sprawdzi przypisywalność przy SetValue

        // Usunięto GetPropertyValue - nie jest potrzebne, można użyć sourceProperty.GetValue(sourceObject)
        // private static object GetPropertyValue<T>(this T source, string propertyName)
        //    => source.GetType().GetProperty(propertyName).GetValue(source, null);
    }
}