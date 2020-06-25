using System;
using System.Linq;
using System.Reflection;
using BookYourShow.Data.EFDatabase;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookYourShow.BusinessLogic.StartupMethods
{
    public class Startuphelper {
        public static void InjectDependency (IServiceCollection services) {
            
            string interfaceName = string.Empty;
            Type classType, interfaceType;
            var assembly = Assembly.GetExecutingAssembly ();

            var listClasses = assembly.GetTypes ()
                .Where (t => t.Name.EndsWith ("Service")).Select (t => new {
                    Name = t.Name,
                        FullName = t.FullName,
                        IsClass = t.IsClass,
                        Type = t.GetType()
                }).ToList ();
            var classListOnly = listClasses.Where (c => c.IsClass);

            foreach (var className in classListOnly) {
                classType = Type.GetType (className.FullName);
                interfaceName = listClasses.Where (t => t.Name.Equals ($"I{className.Name}"))
                    .FirstOrDefault ()?.FullName;

                if (!string.IsNullOrWhiteSpace (interfaceName)) {
                    interfaceType = Type.GetType (interfaceName);
                    services.AddTransient (interfaceType, classType);
                }
            }

        }
    }
}