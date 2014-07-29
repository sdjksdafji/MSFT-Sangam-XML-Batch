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
                dfsXnode((XElement)doc.FirstNode, 0);
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

            if (AddjustDateTimeInParameteres(node))
            {
                return;
            }

            if (AddjustDateTimeInMacros(node))
            {
                return;
            }

            ChangeNodeToProcessDateTime(node);
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
            bool parameterFound = false;
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
                            parameterFound = true;
                            goto BreakLabel;
                        }
                        if (att.Name.ToString().Equals("name", StringComparison.Ordinal) &&
                            att.Value.ToString().Equals("ProcessDateTime", StringComparison.Ordinal))
                        {
                            parameterFound = true;
                            goto BreakLabel;
                        }
                    }

                    childNode = (XElement)childNode.NextNode;
                }
            BreakLabel:


                if (!parameterFound)
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
            jobParameterNode.SetAttributeValue("default", "1970-01-01 12:00:00 AM");
        }

        private static void AddJobParameterProcessDateTime(XElement parameterNode)
        {
            XElement processDateTime = new XElement(BING_NS + "JobParameter");
            processDateTime.SetAttributeValue("name", "ProcessDateTime");
            processDateTime.SetAttributeValue("type", "DateTime");
            processDateTime.SetAttributeValue("isOptional", "false");
            processDateTime.SetAttributeValue("default", "1970-01-01 12:00:00 AM");
            parameterNode.Add(processDateTime);
        }

        private static bool AddjustDateTimeInMacros(XElement node)
        {
            if (node.Name.ToString().Equals("Macros", StringComparison.Ordinal))
            {
                XElement childNode = null;
                childNode = (XElement)node.FirstNode;
                while (childNode != null)
                {
                    IEnumerable<XAttribute> attList =
   from at in childNode.Attributes()
   select at;
                    foreach (XAttribute att in attList)
                    {
                        if (att.Name.ToString().Equals("name", StringComparison.Ordinal) && att.Value.ToString().Equals("today", StringComparison.Ordinal))
                        {
                            ChangeJobMacroToResolvedProcessDateTime(childNode);
                        }
                    }
                    childNode = (XElement)childNode.NextNode;
                }
                return true;
            }
            return false;
        }

        private static void ChangeJobMacroToResolvedProcessDateTime(XElement macroNode)
        {
            macroNode.SetAttributeValue("name", "ResolvedProcessDateTime");
            macroNode.SetAttributeValue("expression", "(StringFormat('{0:yyyy-MM-dd}', ./ProcessDateTime) == '1970-01-01') ? DateTime.Subtract(DateTime.Now, 1, 0, 0, 0) : ./ProcessDateTime");
        }


        private static void ChangeNodeToProcessDateTime(XElement node)
        {
            IEnumerable<XAttribute> attList =
    from at in node.Attributes()
    select at;
            foreach (XAttribute att in attList)
            {
                if (att.Value.ToString().IndexOf("today") >= 0)
                {
                    String oldValue = att.Value.ToString();
                    if (node.Name.ToString().Equals("ScriptParameter"))
                    {
                        att.Value = "[StringFormat('" + oldValue.Replace("(today-1)", "{0:yyyy-MM-dd}") +
                                    "', ./ResolvedProcessDateTime)]";
                    }
                    else
                    {
                        att.Value = oldValue.Replace("today", "ProcessDateTime");
                    }
                }
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

        private static XNamespace BING_NS = "http://schemas.microsoft.com/bing/spatialdata/activity";
    }
}
