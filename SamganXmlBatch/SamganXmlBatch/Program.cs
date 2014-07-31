using System;
using System.Xml;
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
            if (args.Length < 1)
            {
                Console.WriteLine(
                    "usage: [filename]\n Parameters:\nfilename: the text file should contain all the file pathes which need to be done");
            }
            else
            {
                Console.WriteLine("Doing file operation");
                string filename;

                // Read the file and display it line by line.
                System.IO.StreamReader file =
                    new System.IO.StreamReader(args[0]);
                while ((filename = file.ReadLine()) != null)
                {
                    Console.WriteLine(filename);
                    FileInfo jobFile = new FileInfo(filename);
                    XElement doc = XElement.Load(jobFile.FullName, LoadOptions.PreserveWhitespace);
                    dfsXnode(doc, 0);
                    //doc.Save(jobFile.FullName, SaveOptions.DisableFormatting);

                    StringBuilder sb = new StringBuilder();
                    XmlWriterSettings xws = new XmlWriterSettings();
                    xws.NewLineHandling = NewLineHandling.Entitize;

                    xws.OmitXmlDeclaration = true;
                    xws.OmitXmlDeclaration = true;
                    using (XmlWriter xw = XmlWriter.Create(sb, xws))
                    {
                        doc.Save(xw);
                    }
                    Console.WriteLine(sb.ToString());

                    using (StreamWriter outfile = new StreamWriter(jobFile.FullName))
                    {
                        outfile.Write(sb.ToString());
                    }

                    //FormatCarriageReturn(jobFile);
                }

                file.Close();

            }
            ////D:\SearchGold\deploy\builds\data\Sangam_Partners\Sangam-Prod2\OfficeWAC\TestEnv\Jobs\EventTags.job - Copy.xml
            //String dir = getDirFromConsole();
            //FileInfo[] jobFiles = getJobFiles(dir, true);


            //for (int i = 0; i < jobFiles.Length; i++)
            //{
            //    XDocument doc = XDocument.Load(jobFiles[i].FullName);
            //    dfsXnode((XElement)doc.FirstNode, 0);
            //    doc.Save(jobFiles[i].FullName);
            //}





            Console.ReadKey();
        }
        private static void FormatCarriageReturn(FileInfo jobFile)
        {
            String line;
            List<String> newFile = new List<string>();
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(jobFile.FullName);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains(@"<?xml version="))
                {
                    String[] results = line.Split(new String[] { "<b", "activity\"" }, StringSplitOptions.None);
                    if (results.Length > 0)
                    {
                        for (int i = 0; i < results.Length; i++)
                        {
                            if (i == 0)
                            {
                                newFile.Add(results[i]);
                            }
                            else if (i == 1)
                            {
                                newFile.Add("<b" + results[i] + "activity\"");
                            }
                            else
                            {
                                newFile.Add("\t" + results[i]);

                            }
                        }
                    }
                }
                else if (line.Contains("<b:ScopeJobActivity"))
                {
                    String[] results = line.Split(new String[] { "<b:ScopeJobActivity" }, StringSplitOptions.RemoveEmptyEntries);
                    if (results.Length > 0)
                    {
                        String tabs = results[0];
                        results = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (results.Length > 0)
                        {
                            newFile.Add(results[0]);
                            int indexTime = -1;
                            int indexMaxAvaliabilty = -1;
                            int indexFullJobName = -1;
                            for (int i = 1; i < results.Length; i++)
                            {
                                if (results[i].Contains("FullJobName"))
                                {
                                    indexFullJobName = i;
                                }
                                if (results[i].Contains("%Y_%m_%d"))
                                {
                                    indexTime = i;
                                }
                                if (results[i].Contains("maxUnavailability"))
                                {
                                    indexMaxAvaliabilty = i;
                                }

                            }

                            for (int i = 1; i < indexFullJobName; i++)
                            {
                                newFile.Add(tabs + "\t" + results[i]);
                            }
                            string fullJobNameline = null;
                            for (int i = indexFullJobName; i <= indexTime; i++)
                            {
                                fullJobNameline = fullJobNameline + results[i] + " ";

                            }
                            newFile.Add(tabs + "\t" + fullJobNameline);
                            if (indexMaxAvaliabilty != -1)
                            {
                                for (int i = indexTime + 1; i < indexMaxAvaliabilty; i++)
                                {
                                    newFile.Add(tabs + "\t" + results[i]);
                                }
                                newFile.Add(tabs + "\t" + results[indexMaxAvaliabilty] + results[indexMaxAvaliabilty + 1]);

                                for (int i = indexMaxAvaliabilty + 2; i < results.Length; i++)
                                {
                                    newFile.Add(tabs + "\t" + results[i]);
                                }
                            }
                            else
                            {
                                for (int i = indexTime + 1; i < results.Length; i++)
                                {
                                    newFile.Add(tabs + "\t" + results[i]);
                                }
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("Serious Error");
                    }
                }
                else
                {
                    newFile.Add(line);
                }

            }
            file.Close();

            using (System.IO.StreamWriter wFile = new System.IO.StreamWriter(jobFile.FullName))
            {
                foreach (string wLine in newFile)
                {

                    wFile.WriteLine(wLine);

                }
            }
        }


        private static void dfsXnode(XElement node, int depth)
        {
            if (node == null)
            {
                return;
            }

            if (depth == 0)
            {
                parameterTodayFound = false;
                yesterdayMacroFound = false;
            }

            if (AddjustDateTimeInParameteres(node))
            {
                return;
            }

            if (AddjustDateTimeInMacros(node))
            {
                return;
            }

            ChangeNodeToYesterday(node);
            foreach (XElement childNode in node.Descendants())
            {
                dfsXnode(childNode, depth + 1);
            }
            if (depth == 0)
            {

                if (!parameterTodayFound)
                {
                    AddJobParameterProcessDateTime(node);
                }
                if (!yesterdayMacroFound)
                {
                    AddMacroForYesterday(node);
                }
            }
        }

        private static bool AddjustDateTimeInParameteres(XElement node)
        {

            if (node.Name.ToString().Equals("Parameters", StringComparison.Ordinal))
            {
                foreach (XElement childNode in node.Descendants())
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
                            parameterTodayFound = true;
                            goto BreakLabel;
                        }
                        if (att.Name.ToString().Equals("name", StringComparison.Ordinal) &&
                            att.Value.ToString().Equals("ProcessDateTime", StringComparison.Ordinal))
                        {
                            parameterTodayFound = true;
                            goto BreakLabel;
                        }
                    }

                }
            BreakLabel:

                return true;
            }
            return false;
        }

        private static void ChangeJobParameterToProcessDateTime(XElement jobParameterNode)
        {
            jobParameterNode.SetAttributeValue("name", "ProcessDateTime");
            jobParameterNode.SetAttributeValue("default", "1970-01-01 12:00:00 AM");
        }

        private static void AddJobParameterProcessDateTime(XElement node)
        {
            XElement parameterNode = findChild("Parameters", node);
            if (parameterNode == null)
            {
                parameterNode = new XElement("Parameters");
                node.AddFirst(parameterNode);
            }
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
                foreach (XElement childNode in node.Descendants())
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
                        if (att.Name.ToString().Equals("name", StringComparison.Ordinal) && att.Value.ToString().Equals("Yesterday", StringComparison.Ordinal))
                        {
                            yesterdayMacroFound = true;
                        }
                    }
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

        private static void AddMacroForYesterday(XElement node)
        {
            XElement macroForYesterday = new XElement(BING_NS + "JobMacro");
            XElement macroNode = findChild("Macros", node);
            if (macroNode == null)
            {
                XElement parameterNode = findChild("Parameters", node);
                if (parameterNode == null)
                {
                    parameterNode = new XElement("Parameters");
                    node.AddFirst(parameterNode);
                }
                macroNode = new XElement("Macros");
                parameterNode.AddAfterSelf(macroNode);
            }
            macroForYesterday.SetAttributeValue("name", "Yesterday");
            macroForYesterday.SetAttributeValue("type", "DateTime");
            macroForYesterday.SetAttributeValue("expression", "DateTime.Subtract(./ProcessDateTime, 1)");
            macroNode.Add(macroForYesterday);
        }


        private static void ChangeNodeToYesterday(XElement node)
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
                                    "', ./Yesterday)]";
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

        static private XElement findChild(String name, XElement node)
        {
            IEnumerable<XElement> elements =
    from el in node.Elements(name)
    select el;
            foreach (XElement el in elements)
            {
                return el;
            }
            return null;
        }

        private static XNamespace BING_NS = "http://schemas.microsoft.com/bing/spatialdata/activity";
        private static bool parameterTodayFound = false;
        private static bool yesterdayMacroFound = false;
    }
}
