using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebTelegramBotsBuilder.Models.Helpers
{
    public static class MD5Helper
    {
        public static string ToMD5Hash(string Input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(Input));
                return Encoding.UTF8.GetString(result);

            }
        }
    }
}
