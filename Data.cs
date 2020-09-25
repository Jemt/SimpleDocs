using System;
using System.Collections.Generic;
using System.Web;

namespace SimpleDocs
{
	public class Container : IComparable
	{
		public string Name = "";
		public string Description = "";
		public string Extends = "";
		
		public Container(string name)
		{
			this.Name = name;
		}
		
		public override string ToString()
		{
			return "{ Name : '" + this.Name + "', Description : '" + Helper.Encode(this.Description) + "', Extends : '" + this.Extends + "' }";
		}
		
		public int CompareTo(object obj)
		{
			if ((obj is Container) == false)
				throw new Exception("Unable to compare object that is not of type Container");

			//return this.Name.CompareTo(((Container)obj).Name);
			int res = this.Name.CompareTo(((Container)obj).Name);
			return (res == 0 ? -1 : res); // Make 'this' preceed 'obj' when they are identical to maintain order of declaration
		}
	}
	
	public class Member : IComparable
	{
		public string Container = "";
		public string Access = "";
		public string Name = "";
		public string Type = "";
		public string Default = "";
        public bool Nullable = false;
		public bool Static = false;
		public string Description = "";
		
		public Member(string name)
		{
			this.Name = name;
		}
		
		public override string ToString()
		{
            return "{ Container : '" + this.Container + "', Access : '" + this.Access + "', Name : '" + this.Name + "', Type : '" + this.Type + "', Default : '" + Helper.Encode(this.Default) + "', Nullable : " + (this.Nullable ? "true" : "false") + ", Static : " + (this.Static ? "true" : "false") + ", Description : '" + Helper.Encode(this.Description) + "' }";
		}
		
		public int CompareTo(object obj)
		{
			if ((obj is Member) == false)
				throw new Exception("Unable to compare object that is not of type Member");
			
			//if (this.Access != ((Member)obj).Access || this.Static != ((Member)obj).Static)
			if (this.Container == ((Function)obj).Container && this.Access != ((Member)obj).Access || this.Static != ((Member)obj).Static)
			{
				int thisPoints = Helper.GetSortPoints(this.Access, this.Static);
				int objPoints = Helper.GetSortPoints(((Member)obj).Access, ((Member)obj).Static);

				if (thisPoints > objPoints)
					return -1;
				else if (thisPoints < objPoints)
					return 1;
				else
					return -1; // Make 'this' preceed 'obj' when they are identical to maintain order of declaration //return 0;
			}
			else
			{
				//return this.Name.CompareTo(((Member)obj).Name);
				int res = (this.Container + this.Name).CompareTo(((Member)obj).Container + ((Member)obj).Name);
				return (res == 0 ? -1 : res); // Make 'this' preceed 'obj' when they are identical to maintain order of declaration
			}
		}
	}
	
	public class Function : IComparable
	{
		public string Container = "";
		public string Name = "";
		public string Returns = "";
		public string Access = "";
		public string Description = "";
		public bool Static = false;
		public bool Virtual = false;
		public List<Parameter> Parameters = new List<Parameter>();
		
		public Function(string name)
		{
			this.Name = name;
		}
		
		public override string ToString()
		{
			string parms = "";
			
			foreach (Parameter p in this.Parameters)
				parms += (parms != "" ? ", " : "") + p.ToString();
			
			return "{ Container : '" + this.Container + "', Name : '" + this.Name + "', Returns : '" + this.Returns + "', Access : '" + this.Access + "', Description : '" + Helper.Encode(this.Description) + "', Static : " + (this.Static ? "true" : "false") + ", Virtual : " + (this.Virtual ? "true" : "false") + ", Parameters : [" + parms + "] }";
		}
		
		public int CompareTo(object obj)
		{
			if ((obj is Function) == false)
				throw new Exception("Unable to compare object that is not of type Function");

			//if (this.Access != ((Function)obj).Access || this.Static != ((Function)obj).Static)
			if (this.Container == ((Function)obj).Container && this.Access != ((Function)obj).Access || this.Static != ((Function)obj).Static)
			{
				int thisPoints = Helper.GetSortPoints(this.Access, this.Static);
				int objPoints = Helper.GetSortPoints(((Function)obj).Access, ((Function)obj).Static);

				if (thisPoints > objPoints)
					return -1;
				else if (thisPoints < objPoints)
					return 1;
				else
					return -1; // Make 'this' preceed 'obj' when they are identical to maintain order of declaration //return 0;
			}
			else
			{
				//return this.Name.CompareTo(((Function)obj).Name);
				int res = (this.Container + this.Name).CompareTo(((Function)obj).Container + ((Function)obj).Name);
				return (res == 0 ? -1 : res); // Make 'this' preceed 'obj' when they are identical to maintain order of declaration
			}
		}
	}
	
	public class Parameter
	{
		public string Name = "";
		public string Type = "";
		public string Default = "";
		public string Description = "";
        public bool Nullable = false;
		
		public Parameter(string name)
		{
			this.Name = name;
		}
		
		public override string ToString()
		{
            return "{ Name : '" + this.Name + "', Type : '" + this.Type + "', Default : '" + Helper.Encode(this.Default) + "', Nullable: " + (this.Nullable ? "true" : "false") + ", Description : '" + Helper.Encode(this.Description) + "' }";
		}
	}
	
	public class Helper
	{
		public static int GetSortPoints(string access, bool isStatic)
		{
			// public			= 4
			// protected		= 3
			// private			= 2
			// static			= 1 (extra point for public/protected/private)
			// [empty]			= 10
			
			// Items are sorted like this (highest score = top, lowest score = bottom):
			//   No access info
			//   public static
			//   public
			//   protected static
			//   protected
			//   private static
			//   private
			//   static
			
			if (access == "" && isStatic == false)
				return 10;
			else if (access == "")
				return 1;
			else if (access == "private")
				return 2 + (isStatic == false ? 0 : 1);
			else if (access == "protected")
				return 3 + (isStatic == false ? 0 : 1);
			else if (access == "public")
				return 4 + (isStatic == false ? 0 : 1);
			
			return -1;
		}
		
		public static string Encode(string s)
		{
			s = HttpUtility.HtmlEncode(s);
			s = s.Replace("'", "\\\\'");
			s = s.Replace("\r", "").Replace("\n", "<br>");
			
			return s;
		}
	}
}