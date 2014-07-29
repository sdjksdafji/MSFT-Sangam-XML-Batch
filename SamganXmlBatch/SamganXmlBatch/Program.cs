using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SamganXmlBatch
{
    class Program
    {
        static void Main(string[] args)
        {



            //D:\SearchGold\deploy\builds\data\Sangam_Partners\Sangam-Prod2\OfficeWAC\TestEnv\Jobs\EventTags.job - Copy.xml
            String dir = getDirFromConsole();
            FileInfo[] jobFiles = getJobFiles(dir, true);

            
            for (int i = 0; i < jobFiles.Length; i++)
            {
                XDocument doc = XDocument.Load(jobFiles[i].FullName);
                Console.WriteLine(doc.ToString());
                dfsXnode((XElement) doc.FirstNode, 0);
                doc.Save(jobFiles[i].FullName);
            }


  


            Console.ReadKey();
        }

        static private String getDirFromConsole()
        {
            Console.Write("Please enter the directory of XML file:");
            String path = Console.ReadLine();
            if (path != null)
            {
                Console.WriteLine("The directory is: " + path);
            }
            return path;
        }

        static private FileInfo[] getJobFiles(String dir, Boolean print)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            Console.WriteLine(di.Attributes.ToString());
            FileInfo[] fileNames = di.GetFiles("*.job.xml", SearchOption.AllDirectories);
            if (print)
            {
                foreach (FileInfo fi in fileNames)
                {
                    Console.WriteLine("{0}: {1}: {2}", fi.Name, fi.LastAccessTime, fi.Length);
                }
            }
            return fileNames;
        }

        private static void dfsXnode(XElement node, int depth)
        {
            if (node == null)
            {
                return;
            }
            for (int i = 0; i < depth; i++)
            {
                Console.Write("\t");
            }

            //Console.WriteLine("Name: " + node.Name);
            if (AddjustDateTimeInParameteres(node))
            {
                return;
            }

            IEnumerable<XAttribute> attList =
                from at in node.Attributes()
                select at;
            foreach (XAttribute att in attList)
            {
                for (int i = 0; i < depth; i++)
                {
                    //Console.Write("\t");
                }
                //Console.WriteLine("ATT: " + att);
            }

            XElement childNode = null;
            childNode = (XElement)node.FirstNode;
            while (childNode != null)
            {
                dfsXnode(childNode, depth + 1);
                childNode = (XElement)childNode.NextNode;
            }
        }

        private static bool AddjustDateTimeInParameteres(XElement node)
        {
            bool todayParameterFound = false;
            if (node.Name.ToString().Equals("Parameters", StringComparison.Ordinal))
            {
                XElement childNode = null;
                childNode = (XElement)node.FirstNode;
             while (childNode != null)
                {
                    // search today in attributes of JobParameters
                    IEnumerable<XAttribute> attList =
    from at in childNode.Attributes()
    select at;
                    foreach (XAttribute att in attList)
                    {
                        if (att.Name.ToString().Equals("name", StringComparison.Ordinal) && att.Value.ToString().Equals("today", StringComparison.Ordinal))
                        {
                            ChangeJobParameterToProcessDateTime(childNode);
                            todayParameterFound = true;
                            goto BreakLabel;
                        }
                    }

                    childNode = (XElement)childNode.NextNode;
                }
                BreakLabel:


                if (!todayParameterFound)
                {
                    AddJobParameterProcessDateTime(node);
                }
                return true;
            }
            return false;
        }

        private static void ChangeJobParameterToProcessDateTime(XElement jobParameterNode)
        {
            jobParameterNode.SetAttributeValue("name", "ProcessDateTime");
            jobParameterNode.SetAttributeValue("default", "1970-01-01 12:00:00 AM"); Console.WriteLine("---------------------------- 1");
        }

        private static void AddJobParameterProcessDateTime(XElement parameterNode)
        {
            XNamespace bing = "http://schemas.microsoft.com/bing/spatialdata/activity";
            XElement processDateTime = new XElement(bing + "JobParameter");
            processDateTime.SetAttributeValue("name", "ProcessDateTime");
            processDateTime.SetAttributeValue("type", "DateTime");
            processDateTime.SetAttributeValue("isOptional", "false");
            processDateTime.SetAttributeValue("default", "1970-01-01 12:00:00 AM");
            parameterNode.Add(processDateTime); Console.WriteLine("---------------------------- 2");
        }

        private static void ReplaceDate(XAttribute attribute)
        {

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
