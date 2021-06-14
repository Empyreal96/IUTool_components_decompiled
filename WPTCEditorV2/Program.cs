using System;
using System.Reflection;

namespace WPTCEditorTool
{
	// Token: 0x02000003 RID: 3
	internal class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static void Main(string[] args)
		{
			string name = Assembly.GetExecutingAssembly().GetName().Name;
			string value = string.Concat(new string[]
			{
				name,
				" adds the \"FoAllJobs\" tag to all of the <RemoteFile> nodes for Test Central dep.xml files. Logs of all edited files at C:\\WPTCEditorLog.txt\n\nUsage: ",
				name,
				" -tags [FileName]\n       ",
				name,
				" -tagsfullinstall [PackagePath]\n\n[FileName] Path of the dep.xml file to be edited.\n   Values: free text\n   Default=\n\n[PackagePath] Path to the package root folder that contains dep.xml files.\n   Values: free text\n   Default=\"C:\\Program Files (x86)\\Windows Phone Blue Test Central Test Content\\Packages\\PreBuilt\n\nExamples: ",
				name,
				" -tags \"C:\\TCDep\\Microsoft.Phone.Test.DU.dep.xml\"\n          ",
				name,
				" -tagsfullinstall\n          ",
				name,
				" -tagsfullinstall \"C:\\Program Files (x86)\\Windows Phone Blue Test Central Test Content\\Packages\\PreBuilt\"\nn"
			});
			try
			{
				Environment.ExitCode = 0;
				TCDEPS tcdeps = new TCDEPS();
				if (args.Length <= 2)
				{
					if (args[0].ToLower() == "-tags" && args[1].ToLower().Contains("microsoft"))
					{
						tcdeps.AddTags(args[1]);
						tcdeps.PrintGeneralLog();
					}
					else if (args[0].ToLower() == "-tagsfullinstall")
					{
						Console.WriteLine("Adding \"FoAllJobs\" tags in <RemoteFile> nodes for all dep.xml files...");
						if (args.Length == 2)
						{
							tcdeps.AddTagsFullInstall(args[1]);
						}
						else
						{
							tcdeps.AddTagsFullInstall(string.Empty);
						}
						tcdeps.PrintGeneralLog();
					}
					else
					{
						Console.WriteLine(string.Concat(new string[]
						{
							"'",
							args[0].ToString(),
							"' and '",
							args[1].ToString(),
							"' are not valid parameters!"
						}));
						Console.WriteLine(value);
						Environment.ExitCode = -1;
					}
				}
				else
				{
					Console.WriteLine(value);
					Environment.ExitCode = -1;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.WriteLine(value);
				Environment.ExitCode = -1;
			}
		}
	}
}
