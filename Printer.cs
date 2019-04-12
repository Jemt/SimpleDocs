using System;
using System.IO;

namespace SimpleDocs
{
	public class Printer
	{
		public Printer ()
		{
		}
		
		public static void PrintDocs(Container[] containers, Member[] members, Function[] functions, string docName)
		{
			string cdocs = "";
			string mdocs = "";
			string fdocs = "";
			
			foreach (Container c in containers)
				cdocs += (cdocs != "" ? ", " : "") + c.ToString();
			
			foreach (Member m in members)
				mdocs += (mdocs != "" ? ", " : "") + m.ToString();
			
			foreach (Function f in functions)
				fdocs += (fdocs != "" ? ", " : "") + f.ToString();
			
			string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
			path = path.Substring(0, path.LastIndexOf("/") + 1);
			
			TextReader reader = new StreamReader(path + "DocsTemplate.html");
			string content = reader.ReadToEnd();
			reader.Close();
			
			content = content.Replace("{[JSONContainers]}", "[" + cdocs + "]");
			content = content.Replace("{[JSONMembers]}", "[" + mdocs + "]");
			content = content.Replace("{[JSONFunctions]}", "[" + fdocs + "]");
			
			TextWriter writer = new StreamWriter(docName);
			writer.Write(content);
			writer.Close();
		}
	}
}