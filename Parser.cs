using System;
using System.Collections.Generic;
using System.Xml;

namespace SimpleDocs
{
	public class Parser
	{
		public Parser()
		{
		}
		
		public static object[] Parse(string xmlComments, bool includePrivate)
		{
			List<object> elements = new List<object>();
			
			XmlDocument doc = new XmlDocument();

			try
			{
				doc.LoadXml("<?xml version=\"1.0\"?>\n<documentation>" + xmlComments + "</documentation>");
			}
			catch (Exception ex)
			{
				Console.Out.WriteLine("XML error found: " + ex.Message);

				string[] commentLines = xmlComments.Split(new char[] { '\n' });
				for (int i = 1 ; i <= commentLines.Length ; i++)
					Console.Out.WriteLine(i + ": " + commentLines[i-1]);

				throw ex;
			}
			
			XmlNodeList containers = doc.GetElementsByTagName("container");
			XmlNodeList members = doc.GetElementsByTagName("member");
			XmlNodeList functions = doc.GetElementsByTagName("function");
			
			XmlNodeList parameters;
			
			Container con;
			Member mem;
			Function func;
			Parameter parm;
			
			// Get containers
			
			for (int i = 0 ; i < containers.Count ; i++)
			{
				if (getAttributeValue(containers[i].Attributes["name"]) == "")
					continue;
				
				con = new Container(getAttributeValue(containers[i].Attributes["name"]));
				con.Description = getNodeText(containers[i]);
				con.Extends = getAttributeValue(containers[i].Attributes["extends"]);
				
				elements.Add(con);
			}
			
			// Get members
			
			for (int i = 0 ; i < members.Count ; i++)
			{
				if (getAttributeValue(members[i].Attributes["name"]) == "")
					continue;
				
				if (members[i].Attributes["access"] != null && getAttributeValue(members[i].Attributes["access"]).ToLower() != "private" && getAttributeValue(members[i].Attributes["access"]).ToLower() != "public" && getAttributeValue(members[i].Attributes["access"]).ToLower() != "protected")
					continue;
				
				if (members[i].Attributes["static"] != null && getAttributeValue(members[i].Attributes["static"]).ToLower() != "true" && getAttributeValue(members[i].Attributes["static"]).ToLower() != "false")
					continue;
				
				if (includePrivate == false && members[i].Attributes["access"] != null && getAttributeValue(members[i].Attributes["access"]).ToLower() == "private")
					continue;
				
				mem = new Member(getAttributeValue(members[i].Attributes["name"]));
				mem.Container = (getAttributeValue(members[i].Attributes["container"]) != "" ? getAttributeValue(members[i].Attributes["container"]) : "@GlobalScope");
				mem.Access = getAttributeValue(members[i].Attributes["access"]);
				mem.Type = getAttributeValue(members[i].Attributes["type"]);
				mem.Default = getAttributeValue(members[i].Attributes["default"]);
                mem.Nullable = (getAttributeValue(members[i].Attributes["nullable"]).ToLower() == "true");
				mem.Static = (getAttributeValue(members[i].Attributes["static"]).ToLower() == "true");
				mem.Description = getNodeText(members[i]);
				
				elements.Add(mem);
				
				// Ensure container
				if (containerExists(elements.ToArray(), mem.Container) == false)
				{
					con = new Container(mem.Container);
					
					if (con.Name == "@GlobalScope")
						con.Description = "@GlobalScope is a special container listing members and functions not associated with any specific container - they are most likely globally available.";
					
				    elements.Add(con);
				}
			}
			
			// Get functions
			
			for (int i = 0 ; i < functions.Count ; i++)
			{
				if (getAttributeValue(functions[i].Attributes["name"]) == "")
					continue;
				
				if (functions[i].Attributes["access"] != null && getAttributeValue(functions[i].Attributes["access"]).ToLower() != "private" && getAttributeValue(functions[i].Attributes["access"]).ToLower() != "public" && getAttributeValue(functions[i].Attributes["access"]).ToLower() != "protected")
					continue;
				
				if (functions[i].Attributes["static"] != null && getAttributeValue(functions[i].Attributes["static"]).ToLower() != "true" && getAttributeValue(functions[i].Attributes["static"]).ToLower() != "false")
					continue;
				
				if (includePrivate == false && functions[i].Attributes["access"] != null && getAttributeValue(functions[i].Attributes["access"]).ToLower() == "private")
					continue;
				
				func = new Function(functions[i].Attributes["name"].Value);
				func.Container = (getAttributeValue(functions[i].Attributes["container"]) != "" ? getAttributeValue(functions[i].Attributes["container"]) : "@GlobalScope");
				func.Access = getAttributeValue(functions[i].Attributes["access"]);
				func.Returns = getAttributeValue(functions[i].Attributes["returns"]);
				func.Static = (getAttributeValue(functions[i].Attributes["static"]).ToLower() == "true");
				func.Virtual = (getAttributeValue(functions[i].Attributes["virtual"]).ToLower() == "true");
				func.Description = getElementText(functions[i]["description"]);
				
				parameters = ((XmlElement)functions[i]).GetElementsByTagName("param");
				
				for (int j = 0 ; j < parameters.Count ; j++)
				{
					if (getAttributeValue(parameters[j].Attributes["name"]) == "")
						continue;
					
					parm = new Parameter(getAttributeValue(parameters[j].Attributes["name"]));
					parm.Type = getAttributeValue(parameters[j].Attributes["type"]);
					parm.Default = getAttributeValue(parameters[j].Attributes["default"]);
					parm.Description = getNodeText(parameters[j]);
                    parm.Nullable = (getAttributeValue(parameters[j].Attributes["nullable"]).ToLower() == "true");
					
					func.Parameters.Add(parm);
				}
				
				elements.Add(func);
				
				// Ensure container
				if (containerExists(elements.ToArray(), func.Container) == false)
				{
					con = new Container(func.Container);
					
					if (con.Name == "@GlobalScope")
						con.Description = "@GlobalScope is a special container listing members and functions not associated with any specific container - they are most likely globally available.";
					
				    elements.Add(con);
				}
			}
			
			return elements.ToArray();
		}
		
		private static bool containerExists(object[] elements, string name)
		{
			foreach (object elm in elements)
				if (elm is Container && ((Container)elm).Name.Equals(name))
					return true;
			
			return false;
		}
		
		private static string getAttributeValue(XmlAttribute att)
		{
			return (att != null && att.Value != null ? att.Value.Trim() : "");
		}
		
		private static string getNodeText(XmlNode n)
		{
			return (n != null && n.InnerText != null ? n.InnerText.Trim() : "");
		}
		
		private static string getElementText(XmlElement e)
		{
			return (e != null && e.InnerText != null ? e.InnerText.Trim() : "");
		}
	}
}