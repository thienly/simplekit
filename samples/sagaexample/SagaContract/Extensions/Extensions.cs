using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SagaContract.Extensions
{
    public static class Extensions
    {
        public static T ToObject<T>(this IDictionary<string, object> dictionary) where T: class, new()
        {
            Type t = typeof(T);
            var instance = Activator.CreateInstance<T>();
            var propertyInfos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            propertyInfos.ForEach(x =>
            {
                try
                {
                    var value = dictionary[x.Name];
                    if (value != null)
                    {
                        var s = Encoding.UTF8.GetString((byte[]) value);
                        if (!string.IsNullOrEmpty(s))
                        {
                            var convertFrom = TypeDescriptor.GetConverter(x.PropertyType).ConvertFrom(s);
                            x.SetValue(instance, convertFrom);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            });
            return instance;
        }
    }
}