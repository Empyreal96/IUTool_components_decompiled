using System;
using System.Collections.Generic;
using System.Globalization;

namespace DH.Common
{
	// Token: 0x0200001A RID: 26
	public class CommandLineParser
	{
		// Token: 0x060000AB RID: 171 RVA: 0x00003EB8 File Offset: 0x000020B8
		public CommandLineParser(Argument[] knownArguments)
		{
			foreach (Argument argument in knownArguments)
			{
				this.Arguments.Add(argument.Name, argument);
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00003F04 File Offset: 0x00002104
		private void ResetArgumentValues()
		{
			foreach (Argument argument in this.Arguments.Values)
			{
				argument.Value = argument.DefaultValue;
				argument.IsProvided = false;
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00003F68 File Offset: 0x00002168
		public bool IsArgumentProvided(string name)
		{
			return this.Arguments[name].IsProvided;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00003F7B File Offset: 0x0000217B
		public string GetArgumentValueAsString(string name)
		{
			return this.Arguments[name].Value;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003F90 File Offset: 0x00002190
		public int GetArgumentValueAsInt32(string name)
		{
			int result;
			try
			{
				result = int.Parse(this.Arguments[name].Value, NumberFormatInfo.CurrentInfo);
			}
			catch (Exception)
			{
				throw new ArgumentException(string.Format(NumberFormatInfo.CurrentInfo, "Param \"{0}\" is not of valid format", new object[]
				{
					name
				}), name);
			}
			return result;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00003FF0 File Offset: 0x000021F0
		public double GetArgumentValueAsDouble(string name)
		{
			double result;
			try
			{
				result = double.Parse(this.Arguments[name].Value, NumberFormatInfo.CurrentInfo);
			}
			catch (Exception)
			{
				throw new ArgumentException(string.Format(NumberFormatInfo.CurrentInfo, "Param \"{0}\" is not of valid format", new object[]
				{
					name
				}), name);
			}
			return result;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00004050 File Offset: 0x00002250
		public double GetArgumentValueAsDouble(string name, double min, double max)
		{
			double argumentValueAsDouble = this.GetArgumentValueAsDouble(name);
			if (argumentValueAsDouble < min || argumentValueAsDouble > max)
			{
				throw new ArgumentException(string.Format(NumberFormatInfo.CurrentInfo, "Param \"{0}\" has to be between {1} and {2}", new object[]
				{
					name,
					min,
					max
				}), name);
			}
			return argumentValueAsDouble;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000040A0 File Offset: 0x000022A0
		public bool GetArgumentValueAsBool(string name)
		{
			bool result;
			try
			{
				result = bool.Parse(this.Arguments[name].Value);
			}
			catch (Exception)
			{
				throw new ArgumentException(string.Format(NumberFormatInfo.CurrentInfo, "Param \"{0}\" is not of valid format (boolean)", new object[]
				{
					name
				}), name);
			}
			return result;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000040F8 File Offset: 0x000022F8
		public bool? GetArgumentValueAsNullableBool(string name)
		{
			if (!this.Arguments[name].IsProvided || this.Arguments[name].Value == null)
			{
				return null;
			}
			bool value;
			try
			{
				value = bool.Parse(this.Arguments[name].Value);
			}
			catch (Exception)
			{
				throw new ArgumentException(string.Format(NumberFormatInfo.CurrentInfo, "Param \"{0}\" is not of valid format (boolean)", new object[]
				{
					name
				}), name);
			}
			return new bool?(value);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00004188 File Offset: 0x00002388
		public bool Parse(string[] cmdLineArgs)
		{
			this.ResetArgumentValues();
			for (int i = 0; i < cmdLineArgs.Length; i++)
			{
				string[] array = cmdLineArgs[i].Split(new char[]
				{
					':'
				}, 2);
				string text = array[0];
				string text2 = null;
				if (!text.StartsWith("/", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException(string.Format(NumberFormatInfo.CurrentInfo, "Argument {0} does not start with /", new object[]
					{
						text
					}), text);
				}
				text = text.Substring(1);
				if (2 == array.Length && !string.IsNullOrEmpty(array[1]))
				{
					text2 = array[1];
				}
				if ("?" == text)
				{
					return false;
				}
				Argument argument;
				if (!this.Arguments.TryGetValue(text, out argument))
				{
					throw new ArgumentException(string.Format(NumberFormatInfo.CurrentInfo, "Unknown argument \"{0}\"", new object[]
					{
						text
					}), text);
				}
				argument.IsProvided = true;
				argument.Value = text2;
				if (argument.RequiresValue && text2 == null)
				{
					throw new ArgumentException(string.Format(NumberFormatInfo.CurrentInfo, "Argument \"{0}\" requires a value", new object[]
					{
						text
					}), text);
				}
			}
			foreach (Argument argument2 in this.Arguments.Values)
			{
				if (!argument2.IsOptional && !argument2.IsProvided)
				{
					throw new ArgumentException(string.Format(NumberFormatInfo.CurrentInfo, "Required argument \"{0}\" was not provided", new object[]
					{
						argument2.Name
					}), argument2.Name);
				}
			}
			return true;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000431C File Offset: 0x0000251C
		private static void WriteArgHelp(Argument arg)
		{
			Console.WriteLine("/" + arg.Name + "       " + ((arg.DefaultValue != null) ? ("Default Value=" + arg.DefaultValue) : string.Empty));
			Console.WriteLine("     " + arg.Description);
			Console.WriteLine();
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000437C File Offset: 0x0000257C
		public void ShowHelp(string argName)
		{
			if (this.Arguments.ContainsKey(argName))
			{
				Console.WriteLine("Usage:");
				CommandLineParser.WriteArgHelp(this.Arguments[argName]);
				Console.WriteLine("Use /? to see help for all arguments");
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000043B4 File Offset: 0x000025B4
		public void ShowHelp()
		{
			Console.WriteLine("All arguments specified must be of format \"/name:value\". All available arguments:");
			foreach (Argument arg in this.Arguments.Values)
			{
				CommandLineParser.WriteArgHelp(arg);
			}
		}

		// Token: 0x04000047 RID: 71
		private Dictionary<string, Argument> Arguments = new Dictionary<string, Argument>(StringComparer.OrdinalIgnoreCase);
	}
}
