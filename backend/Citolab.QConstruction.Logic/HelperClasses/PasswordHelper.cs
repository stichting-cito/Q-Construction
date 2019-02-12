using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Citolab.QConstruction.Logic.HelperClasses
{
    public static class PasswordHelper
    {
        private static readonly char[] Punctuations = "!@?".ToCharArray();

        public static string Generate(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz@#$&ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
            return result;
        }
    }
}