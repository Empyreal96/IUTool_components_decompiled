using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary.Facades
{
	// Token: 0x02000014 RID: 20
	public class FileNames
	{
		// Token: 0x06000064 RID: 100 RVA: 0x0000401C File Offset: 0x0000221C
		public static string QualifyCommandLine(string commandLine, string rootPath)
		{
			string text;
			string arguments;
			FileNames.SplitCommandFromCommandLine(commandLine, out text, out arguments);
			text = Path.Combine(rootPath, text);
			return FileNames.JoinCommandWithArguments(text, arguments);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000404C File Offset: 0x0000224C
		public static void SplitCommandFromCommandLine(string commandLine, out string command, out string arguments)
		{
			commandLine = commandLine.TrimStart(new char[]
			{
				' '
			});
			if (commandLine[0] != '"')
			{
				int num = commandLine.IndexOf(' ');
				if (num != -1)
				{
					command = commandLine.Substring(0, num);
					arguments = commandLine.Substring(num + 1);
				}
				else
				{
					command = commandLine;
					arguments = null;
				}
			}
			else
			{
				Regex regex = new Regex("^(?<command>(?:[^\"\\s]*\"[^\"]*\"[^\"\\s]*)+)(?:\\s(?<arguments>.*)|$)");
				Match match = regex.Match(commandLine);
				if (!match.Success)
				{
					throw new ArgumentException("Command doesn't use a recognizable grammar.  Expected: \"quoted command\" arg1 ...");
				}
				command = match.Groups["command"].Value;
				arguments = match.Groups["arguments"].Value;
				command = command.Replace("\"", "");
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004130 File Offset: 0x00002330
		public static string JoinCommandWithArguments(string command, string arguments)
		{
			string text = FileNames.QuoteFileNameIfNeeded(command);
			if (arguments != null)
			{
				text = text + ' ' + arguments;
			}
			return text;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004164 File Offset: 0x00002364
		public static string QuoteFileNameIfNeeded(string fileName)
		{
			string result = fileName;
			if (Regex.Match(fileName, "^[^\"].*\\s.*[^\"]$").Success)
			{
				result = '"' + fileName + '"';
			}
			return result;
		}
	}
}
