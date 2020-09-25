using System;
using System.IO;

namespace SimpleDocs
{
	public class Printer
	{
		public Printer ()
		{
		}
		
		public static void PrintDocs(Container[] containers, Member[] members, Function[] functions, string docName, string format)
		{
			string cdocs = "";
			string mdocs = "";
			string fdocs = "";

			if (format == "JSON")
			{
				foreach (Container c in containers)
					cdocs += (cdocs != "" ? ",\n" : "") + "\t\t" + c.ToString();

				foreach (Member m in members)
					mdocs += (mdocs != "" ? ",\n" : "") + "\t\t" + m.ToString();

				foreach (Function f in functions)
					fdocs += (fdocs != "" ? ",\n" : "") + "\t\t" + f.ToString();

				TextWriter tw = new StreamWriter(docName);
				tw.WriteLine("{");
				tw.WriteLine("\t\"Containers\":\n\t[\n" + cdocs + "\n\t],");
				tw.WriteLine("\t\"Members\":\n\t[\n" + mdocs + "\n\t],");
				tw.WriteLine("\t\"Functions\":\n\t[\n" + fdocs + "\n\t]");
				tw.WriteLine("}");
				tw.Close();
			}
			else // HTML
			{
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

				// OLD INSERTION MODE (requires eval(..)) - DEPRECATED, does not support the use of quotes
				content = content.Replace("{[JSONContainers]}", "[" + cdocs + "]");
				content = content.Replace("{[JSONMembers]}", "[" + mdocs + "]");
				content = content.Replace("{[JSONFunctions]}", "[" + fdocs + "]");

				// NEW INSERTION MODE (does not require eval(..))
				content = content.Replace("/*** Containers ***/", cdocs);
				content = content.Replace("/*** Members ***/", mdocs);
				content = content.Replace("/*** Functions ***/", fdocs);

				TextWriter writer = new StreamWriter(docName);
				writer.Write(content);
				writer.Close();
			}
		}
	}
}