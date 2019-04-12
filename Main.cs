using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace SimpleDocs
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if (getArg(args, "Help") != null && (getArg(args, "Help") == "" || getArg(args, "Help").ToLower() == "true"))
			{
				Console.WriteLine();
				Console.WriteLine("Execute SimpleDocs in folder containing documentation.");
				Console.WriteLine("All files are searched for documentation.");
				Console.WriteLine();
				Console.WriteLine("Optional options:");
				Console.WriteLine("   FileType=php        Search only specified file type for documentation");
				Console.WriteLine("   ResultFile=x.html   Name of result file - defaults to SimpleDocs.html");
				Console.WriteLine("   Recursively         Search current folder and all sub folders recursively");
				Console.WriteLine("   IncludePrivate      Include private members and functions in documentation");
				Console.WriteLine("   Help                Display this help");
				Console.WriteLine();
				
				return;
			}
			
			string dir = System.Environment.CurrentDirectory;
			string fileType = (getArg(args, "FileType") != null ? getArg(args, "FileType") : "");
			string filename = (getArg(args, "ResultFile") != null && getArg(args, "ResultFile") != "" ? getArg(args, "ResultFile") : "SimpleDocs.html");
			bool recursively = (getArg(args, "Recursively") != null && (getArg(args, "Recursively") == "" || getArg(args, "Recursively").ToLower() == "true"));
			bool includePrivate = (getArg(args, "IncludePrivate") != null && (getArg(args, "IncludePrivate") == "" || getArg(args, "IncludePrivate").ToLower() == "true"));
			
			string[] files = FileHandler.GetFiles(dir, fileType, recursively);
			
			List<Container> containers = new List<Container>();
			List<Member> members = new List<Member>();
			List<Function> functions = new List<Function>();
			
			string comments;
			object[] elements;

			foreach (string file in files)
			{
				Console.WriteLine("Processing " + file);
				
				comments = FileHandler.GetCommentsFromFile(file);
				elements = Parser.Parse(comments, includePrivate);
				
				foreach (object element in elements)
				{
					if (element is Container)
						containers.Add((Container)element);
					else if (element is Member)
						members.Add((Member)element);
					else if (element is Function)
						functions.Add((Function)element);
				}
			}
			
			containers.Sort();
			members.Sort();
			functions.Sort();
			
			Console.WriteLine("Generating documentation");
			
			Printer.PrintDocs(containers.ToArray(), members.ToArray(), functions.ToArray(), filename);
			
			Console.WriteLine("Done - " + filename + " created");
		}
		
		private static string getArg(string[] args, string key)
		{
			string argument;
			string[] argInfo;
			
			foreach (string arg in args)
			{
				argument = arg;
				argument = (argument.StartsWith("--") == true ? argument.Substring(2) : argument);
				argument = (argument.StartsWith("-") == true ? argument.Substring(1) : argument);
				
				argInfo = argument.Split(new char[] { '=' });
				
				if (argInfo[0].ToLower().Equals(key.ToLower()) == true)
				{
					if (argInfo.Length > 1)
						return argInfo[1];
					else
						return "";
				}
			}
			
			return null;
		}
	}
}