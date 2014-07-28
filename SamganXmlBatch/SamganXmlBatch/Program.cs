using System;
using System.Xml.Linq;
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

            XDocument doc = XDocument.Load(path);
            Console.WriteLine(doc.ToString());
            dfsXnode((XElement)doc.FirstNode, 0);



            //dfsPrintNameAndInnerText(doc, 0);

            doc.Save(path);


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

        static private void dfsXnode(XElement node, int depth)
        {
            if (node == null)
            {
                return;
            }
            for (int i = 0; i < depth; i++)
            {
                Console.Write("\t");
            }

            Console.WriteLine("Name: " + node.Name);

            XElement childNode = null;
            childNode = (XElement)node.FirstNode;
            while (childNode != null)
            {
                dfsXnode(childNode, depth + 1);
                childNode = (XElement)childNode.NextNode;
            }
        }


        static private void dfsPrintNameAndInnerText(System.Xml.XmlNode node, int depth)
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
