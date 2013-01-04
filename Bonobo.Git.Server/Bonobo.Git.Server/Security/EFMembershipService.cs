using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Bonobo.Git.Server.Data;
using System.Data;
using Bonobo.Git.Server.Models;
using BCrypt.Net;
using System.Security.Cryptography;
using System.DirectoryServices;
using System.Text;

namespace Bonobo.Git.Server.Security
{
    public class EFMembershipService : IMembershipService
    {
        public bool ValidateUser(string username, string password)
        {
            if (String.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            User user = GetUserObject(username);

            if (user == null)
                return false;

            if (UserConfigurationManager.UseADAuthentication)
            {
                if (String.IsNullOrEmpty(UserConfigurationManager.DomainName)) throw new ArgumentException("Value cannot be null or empty.", "DomainName");
                if (String.IsNullOrEmpty(UserConfigurationManager.DomainPath)) throw new ArgumentException("Value cannot be null or empty.", "DomainPath");

                LdapAuthentication adAuth = new LdapAuthentication(UserConfigurationManager.DomainPath);

                if (adAuth.IsAuthenticated(UserConfigurationManager.DomainName, username, password))
                    return true;
                else // fallback to local authentication
                    return user != null && (CompareMD5Password(password, user.Password) || ComparePassword(password, user.Password));
            }
            else
            {
                return user != null && (CompareMD5Password(password, user.Password) || ComparePassword(password, user.Password));
            }
        }

        public User GetUserObject(string username)
        {
            if (String.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            using (var database = new DataEntities())
            {
                return database.User.FirstOrDefault(i => i.Username == username);
            }
        }

        public bool UserExists(string username)
        {
            return GetUserObject(username) != null;
        }

        public bool CreateUser(string username, string password, string name, string surname, string email)
        {
            if (String.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", "name");
            if (String.IsNullOrEmpty(surname)) throw new ArgumentException("Value cannot be null or empty.", "surname");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

            using (var database = new DataEntities())
            {
                var user = new User
                {
                    Username = username,
                    Password = EncryptPassword(password),
                    Name = name,
                    Surname = surname,
                    Email = email,
                };
                database.AddToUser(user);
                try
                {
                    database.SaveChanges();
                }
                catch (UpdateException)
                {
                    return false;
                }
            }

            return true;
        }

        public IList<UserModel> GetAllUsers()
        {
            using (var db = new DataEntities())
            {
                return db.User.Include("Roles")
                    .Select(this.CreateViewModel)
                    .ToList();
            }
        }

        public UserModel GetUser(string username)
        {
            if (String.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            using (var db = new DataEntities())
            {
                return db.User.Include("Roles")
                    .Where(user => user.Username == username)
                    .Select(this.CreateViewModel)
                    .FirstOrDefault();
            }
        }

        public void UpdateUser(string username, string name, string surname, string email, string password)
        {
            using (var database = new DataEntities())
            {
                var user = database.User.FirstOrDefault(i => i.Username == username);
                if (user != null)
                {
                    user.Name = name ?? user.Name;
                    user.Surname = surname ?? user.Surname;
                    user.Email = email ?? user.Email;
                    user.Password = password != null ? EncryptPassword(password) : user.Password;
                    database.SaveChanges();
                }
            }
        }

        public void DeleteUser(string username)
        {
            using (var database = new DataEntities())
            {
                var user = database.User.FirstOrDefault(i => i.Username == username);
                if (user != null)
                {
                    user.AdministratedRepositories.Clear();
                    user.Roles.Clear();
                    user.Repositories.Clear();
                    user.Teams.Clear();
                    database.DeleteObject(user);
                    database.SaveChanges();
                }
            }
        }

        private bool CompareMD5Password(string password, string hash)
        {
            return EncryptPasswordMD5(password) == hash;
        }

        private bool ComparePassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        private string EncryptPasswordMD5(string password)
        {
            var x = new MD5CryptoServiceProvider();
            var data = System.Text.Encoding.ASCII.GetBytes(password);
            data = x.ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }

        private string EncryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(15));
        }

        private UserModel CreateViewModel(User user)
        {
            return new UserModel
            {
                Username = user.Username,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Roles = user.Roles.Select(i => i.Name).ToArray(),
            };
        }
    }

    public class LdapAuthentication
    {
        private string _path;
        private string _filterAttribute;

        public LdapAuthentication(string path)
        {
            _path = path;
        }

        public bool IsAuthenticated(string domain, string username, string pwd)
        {
            string domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);

            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    return false;
                }

                //Update the new path to the user in the directory.
                _path = result.Path;
                _filterAttribute = (string)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                //throw new Exception("Error authenticating user. " + ex.Message);
                return false;
            }

            return true;
        }
    }
}