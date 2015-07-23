using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Class;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
             Console.Write("Txt do zaszyfrowania: ");

            string toEncode = Console.ReadLine();

            Encrypt Encrypt = new Encrypt();
            string encryptValue = Encrypt.EncryptText(toEncode, true);
            Console.Write(String.Format("Txt zaszyfrowany: {0}", encryptValue));

            Console.Read();
        }
    }
}
