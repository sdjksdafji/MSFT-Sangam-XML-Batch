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

            //D:\SearchGold\deploy\builds\data\Sangam_Partners\Sangam-Prod2\OfficeWAC\TestEnv\Jobs\EventTags.job - Copy.xml
            String path = getPathFromConsole();

            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            Console.WriteLine(doc.ChildNodes.Count);

            dfsPrintNameAndInnerText(doc, 0);


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

        static private void dfsPrintNameAndInnerText(XmlNode node, int depth)
        {
            if (node == null)
            {
                return;
            }
            for (int i = 0; i < depth; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine("Name: " + node.Name + " || Inner Text: " + node.InnerText);
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                dfsPrintNameAndInnerText(node.ChildNodes[i], depth + 1);
            }
        }
    }
}
