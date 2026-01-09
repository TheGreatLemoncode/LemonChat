using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace test_project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string message = "hello world";
            byte[] encode = Encoding.UTF8.GetBytes(message);
            Console.WriteLine(encode);

            ArraySegment<byte> segment = new ArraySegment<byte>(encode, 0, encode.Length);
        }
    }
}
