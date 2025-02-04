using System;

using System.Security.Cryptography;
using System.Text;


namespace Sfc.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string arg1 = args[0];

                if (string.IsNullOrEmpty(arg1) || arg1.Equals("-h"))
                {
                    Usage();
                    
                    return;
                }

                Console.WriteLine(GetHash(arg1));
            }else
            {
                Usage();
                return;
            }

        }
        private static void Usage()
        {
            var sb = new StringBuilder();
            sb.Append("Usage: ");
            sb.Append("ClientIdGenerate ClientId");

            Console.WriteLine(sb.ToString());
        }

        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
    }
}
