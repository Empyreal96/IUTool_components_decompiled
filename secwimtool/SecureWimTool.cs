using System;
using System.Text.RegularExpressions;

namespace SecureWim
{
	// Token: 0x02000009 RID: 9
	public class SecureWimTool
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00002BFC File Offset: 0x00000DFC
		public static int Main(string[] arguments)
		{
			return SecureWimTool.Parse(arguments).Run();
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002C0C File Offset: 0x00000E0C
		private static IToolCommand Parse(string[] arguments)
		{
			string[] commands = new string[]
			{
				"?",
				"build",
				"extractcat",
				"replacecat"
			};
			UsagePrinter.UsageDelegate[] array = new UsagePrinter.UsageDelegate[]
			{
				new UsagePrinter.UsageDelegate(new UsageCommand(commands).PrintUsage),
				new UsagePrinter.UsageDelegate(BuildCommand.PrintUsage),
				new UsagePrinter.UsageDelegate(ExtractCommand.PrintUsage),
				new UsagePrinter.UsageDelegate(ReplaceCommand.PrintUsage)
			};
			Func<string[], IToolCommand>[] array2 = new Func<string[], IToolCommand>[4];
			array2[0] = ((string[] a) => new UsageCommand(commands));
			array2[1] = ((string[] a) => new BuildCommand(a));
			array2[2] = ((string[] a) => new ExtractCommand(a));
			array2[3] = ((string[] a) => new ReplaceCommand(a));
			Func<string[], IToolCommand>[] array3 = array2;
			string text = string.Empty;
			if (arguments.Length != 0)
			{
				text = arguments[0];
			}
			IToolCommand result;
			try
			{
				Regex regex = new Regex("^[/-]\\?$", RegexOptions.Compiled);
				int i = 0;
				while (i < commands.Length)
				{
					string pattern = "^[/-]" + commands[i] + "$";
					if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase) || regex.IsMatch(text))
					{
						if (arguments.Length == 2 && regex.IsMatch(arguments[1]))
						{
							return new UsagePrinter(array[i]);
						}
						return array3[i](arguments);
					}
					else
					{
						i++;
					}
				}
				throw new ArgParseException(string.Format("Unrecognized command: \"{0}\"", text), array[0]);
			}
			catch (ArgParseException ex)
			{
				result = new UsagePrinter(ex.Message, ex.PrintUsage);
			}
			return result;
		}
	}
}
