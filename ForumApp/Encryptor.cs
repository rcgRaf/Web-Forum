using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace customEncrypt
{
    public static class CustomEncryptor
    {
        private static string Step1(string input)
        {
        var byteinput = System.Text.Encoding.UTF8.GetBytes(input);
        var sha=new System.Security.Cryptography.SHA1Managed().ComputeHash(byteinput);
        return sha.ToString();
            
        }
        public static async Task<byte[]> EncryptAsync(string input) 
        {
            input = input.Insert(input.Length / 2, Step1(input));

        var byteinput = System.Text.Encoding.UTF8.GetBytes(input);
        var sha=new System.Security.Cryptography.SHA1Managed().ComputeHash(byteinput);
        return sha;
        }
    }
}