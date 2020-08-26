using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ben_Project.Utils
{
    public class Crypto
    {
        static public string Sha256(string str)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
            return Convert.ToBase64String(hash);
            //return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
