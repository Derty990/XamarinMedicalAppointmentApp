
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
            if (sourceObject == null) return targetObject;

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
                if (property.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    try
                    { 
                        property.SetValue(targetObject, sourceProperty.GetValue(sourceObject), null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error copying property {property.Name}: {ex.Message}");
                    }
                }
            }
        }
        private static IEnumerable<PropertyInfo> GetTypeProperties<T2>(this T2 sourceObject)
            => sourceObject.GetType().GetProperties();

        private static bool CheckIfPropertyExistInSource(PropertyInfo prop, PropertyInfo property)
            => string.Equals(property.Name, prop.Name, StringComparison.InvariantCultureIgnoreCase);

    }
}