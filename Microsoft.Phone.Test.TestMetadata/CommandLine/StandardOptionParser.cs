using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x0200002A RID: 42
	public class StandardOptionParser : OptionParser
	{
		// Token: 0x060000FB RID: 251 RVA: 0x0000516C File Offset: 0x0000336C
		public StandardOptionParser() : this(null)
		{
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00005177 File Offset: 0x00003377
		public StandardOptionParser(string commandName)
		{
			this._commandName = commandName;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00005188 File Offset: 0x00003388
		public override string ParseCommandName(string[] arguments)
		{
			bool flag = this._commandName == null;
			string result;
			if (flag)
			{
				bool flag2 = arguments == null || arguments.Length == 0;
				if (flag2)
				{
					throw new UsageException("No command is specified");
				}
				result = arguments[0];
			}
			else
			{
				result = this._commandName;
			}
			return result;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000051D0 File Offset: 0x000033D0
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
			int num = (this._commandName == null) ? 1 : 0;
			int i = num;
			while (i < arguments.Length)
			{
				OptionSpecification optionSpecification = null;
				Match match = StandardOptionParser.s_optionRegex.Match(arguments[i]);
				bool success = match.Success;
				string text;
				string text2;
				if (!success)
				{
					text = "";
					text2 = arguments[i];
					goto IL_2A3;
				}
				string value = match.Groups["name"].Value;
				string text3 = match.Groups["plusminus"].Success ? match.Groups["plusminus"].Value : null;
				text2 = null;
				IList<OptionSpecification> partial = optionSpecifications.GetPartial(value);
				bool flag3 = partial.Count == 0;
				if (flag3)
				{
					throw new UsageException(string.Format(CultureInfo.CurrentCulture, "{0} is not a valid option.", new object[]
					{
						arguments[i]
					}));
				}
				bool flag4 = partial.Count > 1;
				if (flag4)
				{
					throw new UsageException(string.Format(CultureInfo.CurrentCulture, "{0} is an ambiguous option.", new object[]
					{
						arguments[i]
					}));
				}
				optionSpecification = partial[0];
				text = optionSpecification.OptionName;
				bool isFinalOption = optionSpecification.IsFinalOption;
				if (!isFinalOption)
				{
					bool flag5 = optionSpecification.ValueType == OptionValueType.NoValue;
					if (flag5)
					{
						bool flag6 = text3 != null;
						if (flag6)
						{
							text2 = text3;
						}
					}
					else
					{
						bool flag7 = i + 1 < arguments.Length;
						if (flag7)
						{
							bool flag8 = arguments[i + 1].StartsWith("-", StringComparison.OrdinalIgnoreCase) || arguments[i + 1].StartsWith("/", StringComparison.OrdinalIgnoreCase) || arguments[i + 1].StartsWith("–", StringComparison.OrdinalIgnoreCase);
							if (flag8)
							{
								bool flag9 = optionSpecification.ValueType == OptionValueType.ValueOptional;
								if (!flag9)
								{
									throw new UsageException(string.Format(CultureInfo.CurrentCulture, "No value specified for option {0}", new object[]
									{
										arguments[i]
									}));
								}
							}
							else
							{
								text2 = arguments[i + 1];
								i++;
							}
						}
						else
						{
							bool flag10 = optionSpecification.ValueType == OptionValueType.ValueRequired;
							if (flag10)
							{
								throw new UsageException(string.Format(CultureInfo.CurrentCulture, "No value specified for option {0}", new object[]
								{
									arguments[i]
								}));
							}
						}
					}
					goto IL_2A3;
				}
				CommandOption commandOption = new CommandOption(optionSpecification.OptionName);
				for (i++; i < arguments.Length; i++)
				{
					commandOption.Add(arguments[i]);
				}
				commandOptionCollection.Add(commandOption);
				IL_321:
				i++;
				continue;
				IL_2A3:
				CommandOption commandOption2 = commandOptionCollection[text];
				bool flag11 = commandOption2 == null;
				if (flag11)
				{
					commandOption2 = new CommandOption(text);
					commandOptionCollection.Add(commandOption2);
				}
				else
				{
					bool flag12 = optionSpecification != null && !optionSpecification.IsMultipleValue;
					if (flag12)
					{
						throw new UsageException(string.Format(CultureInfo.CurrentCulture, "-{0} is specified more than once.", new object[]
						{
							text
						}));
					}
				}
				bool flag13 = text2 != null;
				if (flag13)
				{
					commandOption2.Add(text2);
				}
				goto IL_321;
			}
			return commandOptionCollection;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000551C File Offset: 0x0000371C
		public override void SetOptionProperty(object command, OptionSpecification optionSpecification, CommandOption commandOption)
		{
			bool flag = command == null;
			if (flag)
			{
				throw new ArgumentNullException("command");
			}
			bool flag2 = optionSpecification == null;
			if (flag2)
			{
				throw new ArgumentNullException("optionSpecification");
			}
			bool flag3 = commandOption == null;
			if (flag3)
			{
				throw new ArgumentNullException("commandOption");
			}
			PropertyInfo relatedProperty = optionSpecification.RelatedProperty;
			Type propertyType = optionSpecification.RelatedProperty.PropertyType;
			bool flag4 = typeof(bool).IsAssignableFrom(propertyType);
			if (flag4)
			{
				bool flag5 = string.IsNullOrEmpty(commandOption.Value) || commandOption.Value == "+";
				if (flag5)
				{
					relatedProperty.SetValue(command, true, null);
				}
				else
				{
					bool flag6 = commandOption.Value == "-";
					if (flag6)
					{
						relatedProperty.SetValue(command, false, null);
					}
				}
			}
			else
			{
				bool flag7 = typeof(IList).IsAssignableFrom(propertyType);
				if (flag7)
				{
					IList list = optionSpecification.RelatedProperty.GetValue(command, null) as IList;
					bool flag8 = list == null;
					if (flag8)
					{
						throw new CommandException(string.Format(CultureInfo.CurrentCulture, "The collection property {0} needs to be initialized in the class constructor.", new object[]
						{
							optionSpecification.RelatedProperty.Name
						}));
					}
					Type collectionType = optionSpecification.CollectionType;
					foreach (string value in commandOption.Values)
					{
						object value2 = Convert.ChangeType(value, collectionType, CultureInfo.CurrentCulture);
						list.Add(value2);
					}
				}
				else
				{
					object value3 = Convert.ChangeType(commandOption.Value, propertyType, CultureInfo.CurrentCulture);
					relatedProperty.SetValue(command, value3, null);
				}
			}
		}

		// Token: 0x040000B4 RID: 180
		private const string NameGroup = "name";

		// Token: 0x040000B5 RID: 181
		private const string PlusMinusGroup = "plusminus";

		// Token: 0x040000B6 RID: 182
		private static readonly Regex s_optionRegex = new Regex("^(\\/|\\-|–)(?<name>[^\\+\\-]+)(?<plusminus>\\+|\\-)?$");

		// Token: 0x040000B7 RID: 183
		private readonly string _commandName;
	}
}
