using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UIOMatic.Front.API.Models;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace UIOMatic.Front.API.Services
{
    public class UserService
    {
        private readonly IUIOMaticObjectService _objectService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUIOMaticObjectService objectService, ILogger<UserService> logger)
        {
            _objectService = objectService;
            _logger = logger;
        }

        public User CreateUser(string username, string password, string email)
        {
            var existingUser = GetUserByUsernameOrEmail(username, email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username or email already exists");
            }

            var user = new User
            {
                Username = username,
                Email = email,
                HashedPassword = HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var values = UserToDictionary(user);
            var created = _objectService.Create(typeof(User), values);
            return created as User;
        }

        public User AuthenticateUser(string username, string password)
        {
            var user = GetUserByUsernameOrEmail(username, username);
            if (user == null)
            {
                return null;
            }

            if (!VerifyPassword(password, user.HashedPassword))
            {
                return null;
            }

            user.LastLoginAt = DateTime.UtcNow;
            var values = UserToDictionary(user);
            _objectService.Update(typeof(User), values);

            return user;
        }

        public User GetUserByUsernameOrEmail(string username, string email)
        {
            var users = _objectService.GetAll(typeof(User), string.Empty, string.Empty);
            return users
                .Select(u => u as User)
                .FirstOrDefault(u =>
                    u != null &&
                    (u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) ||
                     u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

        private IDictionary<string, object> UserToDictionary(User user)
        {
            var dict = new Dictionary<string, object>();
            foreach (var prop in typeof(User).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                dict[prop.Name] = prop.GetValue(user);
            }
            return dict;
        }
    }
} 