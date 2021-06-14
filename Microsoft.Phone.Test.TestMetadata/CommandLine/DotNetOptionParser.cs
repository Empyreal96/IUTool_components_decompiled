using System;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000021 RID: 33
	public class DotNetOptionParser : OptionParser
	{
		// Token: 0x060000C4 RID: 196 RVA: 0x000045DA File Offset: 0x000027DA
		public DotNetOptionParser(string commandName)
		{
			this._commandName = commandName;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000045EC File Offset: 0x000027EC
		public override CommandOptionCollection Parse(string[] arguments, OptionSpecificationCollection optionSpecifications)
		{
			bool flag = arguments == null;
			if (flag)
			{
				throw new ArgumentNullException("arguments");
			}
			bool flag2 = optionSpecifications == null;
			if (flag2)
			{
				throw new ArgumentNullException("arguments");
			}
			CommandOptionCollection commandOptionCollection = new CommandOptionCollection();
			for (int i = 0; i < arguments.Length; i++)
			{
				bool flag3 = arguments[i][0] == '/' || arguments[i][0] == '-';
				if (flag3)
				{
					int num = arguments[i].IndexOf(':');
					bool flag4 = num >= 0;
					string name;
					string optionValue;
					if (flag4)
					{
						name = arguments[i].Substring(1, num - 1);
						optionValue = arguments[i].Substring(num + 1);
					}
					else
					{
						name = arguments[i].Substring(1);
						optionValue = string.Empty;
					}
					CommandOption commandOption = commandOptionCollection[name];
					bool flag5 = commandOption == null;
					if (flag5)
					{
						commandOption = new CommandOption(name);
						commandOptionCollection.Add(commandOption);
					}
					commandOption.Add(optionValue);
				}
			}
			return commandOptionCollection;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00004700 File Offset: 0x00002900
		public override string ParseCommandName(string[] arguments)
		{
			return this._commandName;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00003AD9 File Offset: 0x00001CD9
		public override void SetOptionProperty(object command, OptionSpecification optionSpecification, CommandOption commandOption)
		{
		}

		// Token: 0x040000A4 RID: 164
		private readonly string _commandName;
	}
}
