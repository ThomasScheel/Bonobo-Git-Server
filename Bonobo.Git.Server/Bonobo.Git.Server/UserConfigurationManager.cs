﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;

namespace Bonobo.Git.Server
{
    public static class UserConfigurationManager
    {
        private static XmlSerializer _serializer = new XmlSerializer(typeof(UserConfiguration));

        private static UserConfiguration _config;
        private static string _configPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["UserConfiguration"]);

        private static UserConfiguration Configuration
        {
            get
            {
                if (_config == null)
                {
                    using (var file = new FileStream(_configPath, FileMode.Open))
                    {
                        _config = (UserConfiguration)_serializer.Deserialize(file);
                    }
                }

                return _config;
            }
        }

        public static bool AllowAnonymousPush
        {
            get
            {
                return Configuration.AllowAnonymousPush;
            }
            set
            {
                Configuration.AllowAnonymousPush = value;
            }
        }

        public static bool AllowAnonymousRegistration
        {
            get
            {
                return Configuration.AllowAnonymousRegistration;
            }
            set
            {
                Configuration.AllowAnonymousRegistration = value;
            }
        }

        public static bool AllowUserRepositoryCreation
        {
            get
            {
                return Configuration.AllowUserRepositoryCreation;
            }
            set
            {
                Configuration.AllowUserRepositoryCreation = value;
            }
        }


        public static string Repositories
        {
            get
            {
                return Configuration.Repositories;
            }
            set
            {
                Configuration.Repositories = value;
            }
        }


        public static string DomainName
        {
            get
            {
                return Configuration.DomainName;
            }
            set
            {
                Configuration.DomainName = value;
            }
        }


        public static string DomainPath
        {
            get
            {
                return Configuration.DomainPath;
            }
            set
            {
                Configuration.DomainPath = value;
            }
        }


        public static bool UseADAuthentication
        {
            get
            {
                return Configuration.UseADAuthentication;
            }
            set
            {
                Configuration.UseADAuthentication = value;
            }
        }


        public static void Save()
        {
            using (var file = new FileStream(_configPath, FileMode.Create))
            {
                _serializer.Serialize(file, Configuration);
            }
        }
    }


}