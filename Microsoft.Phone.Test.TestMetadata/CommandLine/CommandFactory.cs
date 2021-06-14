using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x0200001C RID: 28
	public class CommandFactory
	{
		// Token: 0x0600009B RID: 155 RVA: 0x00003BEA File Offset: 0x00001DEA
		public CommandFactory(Assembly commandAssembly, OptionParser parser) : this(new Assembly[]
		{
			commandAssembly
		}, parser)
		{
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00003BFF File Offset: 0x00001DFF
		public CommandFactory(Assembly[] commandAssemblies, OptionParser parser)
		{
			this._argumentParser = parser;
			this.LoadCommandSpecification(commandAssemblies);
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600009D RID: 157 RVA: 0x00003C18 File Offset: 0x00001E18
		// (set) Token: 0x0600009E RID: 158 RVA: 0x00003C20 File Offset: 0x00001E20
		public CommandSpecificationCollection Commands { get; private set; }

		// Token: 0x0600009F RID: 159 RVA: 0x00003C2C File Offset: 0x00001E2C
		public Command Create(string[] arguments)
		{
			Command command = null;
			string text = this._argumentParser.ParseCommandName(arguments);
			CommandSpecification commandSpecification = this.Commands[text];
			bool flag = commandSpecification == null;
			if (flag)
			{
				throw new UsageException(string.Format(CultureInfo.CurrentCulture, "Unknown command '{0}'", new object[]
				{
					text
				}));
			}
			try
			{
				command = (Activator.CreateInstance(commandSpecification.CommandType) as Command);
				command.Specification = commandSpecification;
				CommandOptionCollection commandOptionCollection = this._argumentParser.Parse(arguments, commandSpecification.OptionSpecifications);
				foreach (object obj in commandSpecification.OptionSpecifications)
				{
					OptionSpecification optionSpecification = (OptionSpecification)obj;
					bool flag2 = optionSpecification.DefaultValue != null && !commandOptionCollection.Contains(optionSpecification.OptionName);
					if (flag2)
					{
						CommandOption commandOption = new CommandOption(optionSpecification.OptionName);
						commandOption.Add(optionSpecification.DefaultValue);
						commandOptionCollection.Add(commandOption);
					}
				}
				bool flag3 = !command.Specification.AllowNoNameOptions && commandOptionCollection[""] != null;
				if (flag3)
				{
					throw new UsageException(string.Format(CultureInfo.CurrentCulture, "Invalid option '{0}'", new object[]
					{
						commandOptionCollection[""].Value
					}));
				}
				foreach (object obj2 in commandSpecification.OptionSpecifications)
				{
					OptionSpecification optionSpecification2 = (OptionSpecification)obj2;
					bool flag4 = optionSpecification2.RelatedProperty != null && commandOptionCollection.Contains(optionSpecification2.OptionName);
					if (flag4)
					{
						this._argumentParser.SetOptionProperty(command, optionSpecification2, commandOptionCollection[optionSpecification2.OptionName]);
						commandOptionCollection.Remove(optionSpecification2.OptionName);
					}
				}
				command.Load(commandOptionCollection);
				bool flag5 = command.Output == null;
				if (flag5)
				{
					command.Output = Console.Out;
				}
				bool flag6 = command.Error == null;
				if (flag6)
				{
					command.Error = Console.Error;
				}
			}
			catch (CommandException ex)
			{
				bool flag7 = ex.Command == null;
				if (flag7)
				{
					ex.Command = command;
				}
				throw;
			}
			return command;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00003EE0 File Offset: 0x000020E0
		private void LoadCommandSpecification(Assembly[] commandAssemblies)
		{
			this.Commands = new CommandSpecificationCollection();
			foreach (Assembly assembly in commandAssemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					object[] customAttributes = type.GetCustomAttributes(typeof(CommandAttribute), false);
					bool flag = customAttributes.Length != 0;
					if (flag)
					{
						CommandSpecification specification = new CommandSpecification(type);
						this.Commands.Add(specification);
					}
				}
			}
		}

		// Token: 0x0400009B RID: 155
		private readonly OptionParser _argumentParser;
	}
}
