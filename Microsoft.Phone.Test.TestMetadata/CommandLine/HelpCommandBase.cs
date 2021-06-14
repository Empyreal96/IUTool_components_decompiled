using System;
using System.Reflection;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000023 RID: 35
	public class HelpCommandBase : Command
	{
		// Token: 0x060000CB RID: 203 RVA: 0x00004990 File Offset: 0x00002B90
		public HelpCommandBase() : this(null)
		{
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000499C File Offset: 0x00002B9C
		public HelpCommandBase(Assembly commandAssembly)
		{
			bool flag = commandAssembly != null;
			if (flag)
			{
				this._factory = new CommandFactory(commandAssembly, new StandardOptionParser());
			}
			else
			{
				this._factory = new CommandFactory(base.GetType().Assembly, new StandardOptionParser());
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000CD RID: 205 RVA: 0x000049EE File Offset: 0x00002BEE
		// (set) Token: 0x060000CE RID: 206 RVA: 0x000049F6 File Offset: 0x00002BF6
		[Option("", OptionValueType.NoValue)]
		public string CommandName { get; set; }

		// Token: 0x060000CF RID: 207 RVA: 0x00004A00 File Offset: 0x00002C00
		protected override void RunImplementation()
		{
			base.Output.WriteLine();
			bool flag = this.CommandName != null && this._factory.Commands[this.CommandName] != null;
			if (flag)
			{
				this._factory.Commands[this.CommandName].PrintFullUsage(base.Output);
			}
			else
			{
				bool flag2 = this.CommandName != null && this._factory.Commands[this.CommandName] == null;
				if (flag2)
				{
					base.Output.WriteLine("Unknown command '{0}'", this.CommandName);
					base.Output.WriteLine();
				}
				base.Output.WriteLine("The following commands are available");
				base.Output.WriteLine();
				string[] array = new string[this._factory.Commands.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._factory.Commands[i].Name;
				}
				Array.Sort<string>(array);
				foreach (string name in array)
				{
					CommandSpecification commandSpecification = this._factory.Commands[name];
					base.Output.WriteLine("  {0} - {1}", commandSpecification.Name, commandSpecification.BriefDescription);
				}
			}
		}

		// Token: 0x040000A5 RID: 165
		private readonly CommandFactory _factory;
	}
}
