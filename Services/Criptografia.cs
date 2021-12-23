using System;
using System.Security.Cryptography;
using System.Text;

namespace ApiUser.Services
{
    public static class Criptografia
    {
        // c# string to sha256
        // https://www.codegrepper.com/code-examples/csharp/c%23+string+to+sha256
        public static string criptografarSenha(string password)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(password));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}