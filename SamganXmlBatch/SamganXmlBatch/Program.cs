using System;
using System.Xml;
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

            //D:\SearchGold\deploy\builds\data\Sangam_Partners\Sangam-Prod2\OfficeWAC\TestEnv\Jobs\EventTags.job.xml
            String path = getPathFromConsole();

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            Console.WriteLine(doc.InnerText);

            Console.ReadKey();
        }

        static private String getPathFromConsole()
        {
            Console.Write("Please enter the path of XML file:");
            String path = Console.ReadLine();
            if (path != null)
            {
                Console.WriteLine("The path is: " + path);
            }
            return path;
        }
    }
}
