using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace NSE.WebApp.MVC.Extensions
{
    public static class RazorHelpers
    {
        public static string StockMessage(this RazorPage page, int quantity)
        {
            return quantity > 0 ? $"In stock {quantity} only!" : "Out of stock";
        }

        public static string Formatcurrency(this RazorPage page, decimal value)
        {
            return value > 0 ? string.Format(Thread.CurrentThread.CurrentCulture, "{0:C}", value) : "free";
        }

        public static string HashEmailForGravatar(this RazorPage page, string email)
        {
            var md5Hasher = MD5.Create();
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));
            var sBuilder = new StringBuilder();
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
