using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsPreferred { get; set; }

        public Customer()
        {
            // Пустой конструктор для сериализации
        }

        public Customer(string login, string password)
        {
            Login = login;
            PasswordHash = HashPassword(password);
            RegistrationDate = DateTime.Now;
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
