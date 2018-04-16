using BeitechWebApi.App_Start;
using BeitechWebApi.Jwt;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BeitechWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // Rutas de API web
            config.MapHttpAttributeRoutes();

            /* config.Routes.MapHttpRoute(
                 name: "DefaultApi",
                 routeTemplate: "api/{controller}/{id}",
                 defaults: new { id = RouteParameter.Optional }
             );*/

            // Limpiar todos los formato de salida para el "API"
            config.Formatters.Clear();

            // Agregar formato de salida "JSON"
            config.Formatters.Add(new JsonMediaTypeFormatter());

            // Modificar formato de salida "JSON"
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
 

            //config.MessageHandlers.Add(new ApiAuthHandler());
            config.MessageHandlers.Add(new JjwtAuthMessageHandler());



        }

        #region ApplicationInfo

        public static class ApplicationInfo
        {
            private static Assembly ThisAssembly
            {
                get
                {
                    return Assembly.GetExecutingAssembly();
                }
            }

            public static string Title
            {
                get
                {
                    object[] attributes = ThisAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    NameValueCollection appSettings = System.Configuration.ConfigurationManager.AppSettings;

                    bool isManualActive = false;
                    string appManualActive = appSettings.Get("AppManual_Active");
                    string appManualName = appSettings.Get("AppManual_Name");

                    if (!string.IsNullOrWhiteSpace(appManualActive))
                    {
                        isManualActive = bool.Parse(appManualActive);
                    }
                    if (isManualActive && !string.IsNullOrWhiteSpace(appManualName))
                    {
                        return appManualName;
                    }

                    if (attributes.Length > 0)
                    {
                        AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                        if (titleAttribute.Title.Length > 0) return titleAttribute.Title;
                    }
                    return System.IO.Path.GetFileNameWithoutExtension(ThisAssembly.CodeBase);
                }
            }

            public static string ProductName
            {
                get
                {
                    object[] attributes = ThisAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    return attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
                }
            }

            public static string Description
            {
                get
                {
                    object[] attributes = ThisAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                    return attributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute)attributes[0]).Description;
                }
            }

            public static string Copyright
            {
                get
                {
                    object[] attributes = ThisAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    return attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                }
            }

            public static string CompanyName
            {
                get
                {
                    object[] attributes = ThisAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    return attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company;
                }
            }

            public static Version BuildVersionFull
            {
                get
                {
                    return ThisAssembly.GetName().Version;
                }
            }

            public static string BuildVersion
            {
                get
                {
                    Version curVersion = BuildVersionFull;
                    var version = string.Format("{0}.{1}.{2}", curVersion.Major, curVersion.Minor, curVersion.Build.ToString());
                    //Application.Add("AppVersionBuild2", string.Join(".", Application.Get("AppVersionBuild").ToString().Split(char.Parse(".")).Take(2)));
                    return version;
                }
            }

            public static string BuildFramework
            {
                get
                {
                    return System.AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;
                }
            }

            public static DateTime BuildDateTime
            {
                get
                {
                    Version versionBuild = BuildVersionFull;
                    DateTime versionBuildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
                        TimeSpan.TicksPerDay * versionBuild.Build + // days since 1 January 2000
                        TimeSpan.TicksPerSecond * 2 * versionBuild.Revision // seconds since midnight, (multiply by 2 to get original)
                    ));
                    return versionBuildDateTime;
                }
            }

            public static string BuildYear
            {
                get
                {
                    return BuildDateTime.Year.ToString();
                }
            }

            public static string BuildConfiguration
            {
                get
                {
                    object[] attributes = ThisAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
                    return attributes.Length == 0 ? "" : ((AssemblyConfigurationAttribute)attributes[0]).Configuration;
                }
            }

            public static Version CommercialVersion
            {
                get
                {
                    Version version = new Version("0.0.0.19000101");
                    AssemblyFileVersionAttribute fileVersion = ThisAssembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute;
                    return (fileVersion == null) ? version : new Version(fileVersion.Version);
                }
            }

            public static string CommercialConfiguration
            {
                get
                {
                    string configuration = BuildConfiguration;
                    NameValueCollection appSettings = System.Configuration.ConfigurationManager.AppSettings;
                    string configurationApp = string.Format("{0} ({1})", appSettings.Get("App_Configuration"), configuration);
                    if (string.IsNullOrWhiteSpace(configuration) || configuration != "Debug")
                    {
                        configurationApp = "Desarrollo";
                    }
                    return configurationApp;
                }
            }
        }

        #endregion ApplicationInfo
    }
}
