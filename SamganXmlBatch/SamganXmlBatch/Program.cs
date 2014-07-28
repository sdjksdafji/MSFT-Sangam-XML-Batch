using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamganXmlBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the path of XML file:");
            Console.WriteLine();
            Console.Write("   ");
            String line = Console.ReadLine();
            if (line != null)
            {
                Console.WriteLine("The path is: " + line);
            }
            Console.ReadKey();
        }
    }
}
