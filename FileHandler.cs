using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleDocs
{
	public class FileHandler
	{
		public static string[] GetFiles(string dir, string fileType, bool recursively)
		{
			List<string> files = new List<string>();
			getFilesRecursively(dir, fileType, files, recursively);
			
			return files.ToArray();
		}
		
		private static void getFilesRecursively(string dir, string fileType, List<string> files, bool recursively)
		{
			if (fileType == null || fileType == "")
			{
				files.AddRange(Directory.GetFiles(dir));
			}
			else
			{
				string[] containedFiles = Directory.GetFiles(dir);
				
				foreach (string file in containedFiles)
				{
					if (file.ToLower().EndsWith(fileType) == false)
						continue;
					
					files.Add(file);
				}
			}
			
			if (recursively == false)
				return;
			
			string[] subdirs = Directory.GetDirectories(dir);
			
			foreach (string subdir in subdirs)
				getFilesRecursively(subdir, fileType, files, recursively);
		}
		
		public static string GetCommentsFromFile(string file)
		{
			TextReader reader = new StreamReader(file);
			string content = reader.ReadToEnd();
			reader.Close();
			
			string[] lines = content.Replace("\r", "").Split(new char[] { '\n' });
			string comment;
			string comments = "";
			
			foreach (string line in lines)
			{
				comment = line.Trim();
				
				if (comment.StartsWith("///") == false)
					continue;
				
				comments += (comments != "" ? "\n" : "") + comment.Substring(3).Trim();
			}
			
			return comments;
		}
	}
}