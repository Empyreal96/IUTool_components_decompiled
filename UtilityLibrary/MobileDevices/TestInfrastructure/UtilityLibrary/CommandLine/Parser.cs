using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary.CommandLine
{
	// Token: 0x02000007 RID: 7
	public sealed class Parser
	{
		// Token: 0x06000016 RID: 22 RVA: 0x0000220E File Offset: 0x0000040E
		private Parser()
		{
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000221C File Offset: 0x0000041C
		public static bool ParseArgumentsWithUsage(string[] arguments, object destination)
		{
			bool result;
			if (Parser.ParseHelp(arguments) || !Parser.ParseArguments(arguments, destination))
			{
				Parser.PrintUsage(destination);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002252 File Offset: 0x00000452
		public static void PrintUsage(object destination)
		{
			Parser.PrintUsage(destination, new ErrorReporter(Console.Out.WriteLine));
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002270 File Offset: 0x00000470
		public static void PrintUsage(object destination, ErrorReporter reporter)
		{
			IArgumentHolder argumentHolder = destination as IArgumentHolder;
			if (argumentHolder != null)
			{
				reporter(argumentHolder.GetUsageString());
			}
			reporter(Parser.ArgumentsUsage(destination.GetType()));
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000022B0 File Offset: 0x000004B0
		public static bool ParseArguments(string[] arguments, object destination)
		{
			return Parser.ParseArguments(arguments, destination, new ErrorReporter(Console.Error.WriteLine));
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000022DC File Offset: 0x000004DC
		public static bool ParseArguments(string[] arguments, object destination, ErrorReporter reporter)
		{
			Parser parser = new Parser(destination.GetType(), reporter);
			return parser.Parse(arguments, destination);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002303 File Offset: 0x00000503
		private static void NullErrorReporter(string message)
		{
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002308 File Offset: 0x00000508
		public static bool ParseHelp(string[] args)
		{
			Parser parser = new Parser(typeof(Parser.HelpArgument), new ErrorReporter(Parser.NullErrorReporter));
			Parser.HelpArgument helpArgument = new Parser.HelpArgument();
			parser.Parse(args, helpArgument);
			return helpArgument.Help;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000234C File Offset: 0x0000054C
		public static string ArgumentsUsage(Type argumentType)
		{
			int num = Parser.GetConsoleWindowWidth();
			if (num == 0)
			{
				num = 80;
			}
			return Parser.ArgumentsUsage(argumentType, num);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000237C File Offset: 0x0000057C
		public static string ArgumentsUsage(Type argumentType, int columns)
		{
			return new Parser(argumentType, null).GetUsageString(columns);
		}

		// Token: 0x06000020 RID: 32
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetStdHandle(int nStdHandle);

		// Token: 0x06000021 RID: 33
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetConsoleScreenBufferInfo(int hConsoleOutput, ref Parser.CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

		// Token: 0x06000022 RID: 34 RVA: 0x0000239C File Offset: 0x0000059C
		public static int GetConsoleWindowWidth()
		{
			Parser.CONSOLE_SCREEN_BUFFER_INFO console_SCREEN_BUFFER_INFO = default(Parser.CONSOLE_SCREEN_BUFFER_INFO);
			int consoleScreenBufferInfo = Parser.GetConsoleScreenBufferInfo(Parser.GetStdHandle(-11), ref console_SCREEN_BUFFER_INFO);
			return (int)console_SCREEN_BUFFER_INFO.dwSize.x;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000023D4 File Offset: 0x000005D4
		public static int IndexOf(StringBuilder text, char value, int startIndex)
		{
			for (int i = startIndex; i < text.Length; i++)
			{
				if (text[i] == value)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002414 File Offset: 0x00000614
		public static int LastIndexOf(StringBuilder text, char value, int startIndex)
		{
			for (int i = Math.Min(startIndex, text.Length - 1); i >= 0; i--)
			{
				if (text[i] == value)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002460 File Offset: 0x00000660
		public Parser(Type argumentSpecification, ErrorReporter reporter)
		{
			this.reporter = reporter;
			this.arguments = new ArrayList();
			this.defaultArguments = new ArrayList();
			this.argumentMap = new Hashtable();
			foreach (FieldInfo fieldInfo in argumentSpecification.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (!fieldInfo.IsStatic && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
				{
					ArgumentAttribute attribute = Parser.GetAttribute(fieldInfo);
					Parser.Argument value = new Parser.Argument(attribute, fieldInfo, reporter);
					if (attribute is DefaultArgumentAttribute)
					{
						if (this.defaultArguments.Count != 0)
						{
						}
						this.defaultArguments.Add(value);
					}
					else
					{
						this.arguments.Add(value);
					}
				}
			}
			foreach (object obj in this.arguments)
			{
				Parser.Argument argument = (Parser.Argument)obj;
				this.argumentMap[argument.LongName] = argument;
				if (argument.ExplicitShortName)
				{
					if (argument.ShortName != null && argument.ShortName.Length > 0)
					{
						this.argumentMap[argument.ShortName] = argument;
					}
					else
					{
						argument.ClearShortName();
					}
				}
			}
			foreach (object obj2 in this.arguments)
			{
				Parser.Argument argument = (Parser.Argument)obj2;
				if (!argument.ExplicitShortName)
				{
					if (argument.ShortName != null && argument.ShortName.Length > 0 && !this.argumentMap.ContainsKey(argument.ShortName))
					{
						this.argumentMap[argument.ShortName] = argument;
					}
					else
					{
						argument.ClearShortName();
					}
				}
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000026C4 File Offset: 0x000008C4
		private static ArgumentAttribute GetAttribute(FieldInfo field)
		{
			object[] customAttributes = field.GetCustomAttributes(typeof(ArgumentAttribute), false);
			ArgumentAttribute result;
			if (customAttributes.Length == 1)
			{
				result = (ArgumentAttribute)customAttributes[0];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002701 File Offset: 0x00000901
		private void ReportUnrecognizedArgument(string argument)
		{
			this.reporter(string.Format("Unrecognized command line argument '{0}'", argument));
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002728 File Offset: 0x00000928
		private bool ParseArgumentList(string[] args, object destination)
		{
			bool flag = false;
			if (args != null)
			{
				foreach (string text in args)
				{
					if (text.Length > 0)
					{
						char c = text[0];
						switch (c)
						{
						case '-':
						case '/':
						{
							int num = text.IndexOfAny(new char[]
							{
								':',
								'+',
								'-'
							}, 1);
							string text2 = text.Substring(1, (num == -1) ? (text.Length - 1) : (num - 1));
							string value;
							if (text2.Length + 1 == text.Length)
							{
								value = null;
							}
							else if (text.Length > 1 + text2.Length && text[1 + text2.Length] == ':')
							{
								value = text.Substring(text2.Length + 2);
							}
							else
							{
								value = text.Substring(text2.Length + 1);
							}
							Parser.Argument argument = (Parser.Argument)this.argumentMap[text2];
							if (argument == null)
							{
								this.ReportUnrecognizedArgument(text);
								flag = true;
							}
							else
							{
								flag |= !argument.SetValue(value, destination);
							}
							break;
						}
						case '.':
							goto IL_175;
						default:
						{
							if (c != '@')
							{
								goto IL_175;
							}
							string[] args2;
							flag |= this.LexFileArguments(text.Substring(1), out args2);
							flag |= this.ParseArgumentList(args2, destination);
							break;
						}
						}
						goto IL_1ED;
						IL_175:
						if (this.defaultArguments.Count != 0)
						{
							flag |= !((Parser.Argument)this.defaultArguments[this.usedDefaults]).SetValue(text, destination);
							if (this.usedDefaults + 1 < this.defaultArguments.Count)
							{
								this.usedDefaults++;
							}
						}
						else
						{
							this.ReportUnrecognizedArgument(text);
							flag = true;
						}
					}
					IL_1ED:;
				}
			}
			return flag;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002944 File Offset: 0x00000B44
		public bool Parse(string[] args, object destination)
		{
			bool flag = this.ParseArgumentList(args, destination);
			foreach (object obj in this.arguments)
			{
				Parser.Argument argument = (Parser.Argument)obj;
				flag |= argument.Finish(destination);
			}
			foreach (object obj2 in this.defaultArguments)
			{
				Parser.Argument argument = (Parser.Argument)obj2;
				flag |= argument.Finish(destination);
			}
			return !flag;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002A24 File Offset: 0x00000C24
		public string GetUsageString(int screenWidth)
		{
			Parser.ArgumentHelpStrings[] allHelpStrings = this.GetAllHelpStrings();
			int num = 0;
			foreach (Parser.ArgumentHelpStrings argumentHelpStrings in allHelpStrings)
			{
				num = Math.Max(num, argumentHelpStrings.syntax.Length);
			}
			int num2 = 4 + num + 2;
			screenWidth = Math.Max(screenWidth, 19);
			int num3;
			if (screenWidth < num2 + 10)
			{
				num3 = 9;
			}
			else
			{
				num3 = num2;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Parser.ArgumentHelpStrings argumentHelpStrings2 in allHelpStrings)
			{
				stringBuilder.Append(' ', 4);
				int num4 = 4;
				int length = argumentHelpStrings2.syntax.Length;
				stringBuilder.Append(argumentHelpStrings2.syntax);
				num4 += length;
				if (num4 >= num3)
				{
					stringBuilder.Append("\n");
					num4 = 4;
				}
				int num5 = screenWidth - num3;
				int j = 0;
				while (j < argumentHelpStrings2.help.Length)
				{
					stringBuilder.Append(' ', num3 - num4);
					num4 = num3;
					int num6 = j + num5;
					if (num6 >= argumentHelpStrings2.help.Length)
					{
						num6 = argumentHelpStrings2.help.Length;
					}
					else
					{
						num6 = argumentHelpStrings2.help.LastIndexOf(' ', num6 - 1, Math.Min(num6 - j, num5));
						if (num6 <= j)
						{
							num6 = j + num5;
						}
					}
					stringBuilder.Append(argumentHelpStrings2.help, j, num6 - j);
					j = num6;
					Parser.AddNewLine("\n", stringBuilder, ref num4);
					while (j < argumentHelpStrings2.help.Length && argumentHelpStrings2.help[j] == ' ')
					{
						j++;
					}
				}
				if (argumentHelpStrings2.help.Length == 0)
				{
					stringBuilder.Append("\n");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002C61 File Offset: 0x00000E61
		private static void AddNewLine(string newLine, StringBuilder builder, ref int currentColumn)
		{
			builder.Append(newLine);
			currentColumn = 0;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002C70 File Offset: 0x00000E70
		private Parser.ArgumentHelpStrings[] GetAllHelpStrings()
		{
			int num = this.NumberOfParametersToDisplay();
			Parser.ArgumentHelpStrings[] array = new Parser.ArgumentHelpStrings[num];
			int num2 = 0;
			foreach (object obj in this.arguments)
			{
				Parser.Argument argument = (Parser.Argument)obj;
				if (!argument.IsHidden)
				{
					array[num2] = Parser.GetHelpStrings(argument);
					num2++;
				}
			}
			foreach (object obj2 in this.defaultArguments)
			{
				Parser.Argument argument = (Parser.Argument)obj2;
				array[num2] = Parser.GetHelpStrings(argument);
				num2++;
			}
			if (num > 2)
			{
				array[num2++] = new Parser.ArgumentHelpStrings("@<file>", "Read additional options from response file");
			}
			return array;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002DB4 File Offset: 0x00000FB4
		private static Parser.ArgumentHelpStrings GetHelpStrings(Parser.Argument arg)
		{
			return new Parser.ArgumentHelpStrings(arg.SyntaxHelp, arg.FullHelpText);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002DD8 File Offset: 0x00000FD8
		private int NumberOfParametersToDisplay()
		{
			int num = this.arguments.Count + this.defaultArguments.Count;
			if (num > 2)
			{
				num++;
			}
			return num;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002E14 File Offset: 0x00001014
		public bool HasDefaultArgument
		{
			get
			{
				return this.defaultArguments.Count != 0;
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002E38 File Offset: 0x00001038
		private bool LexFileArguments(string fileName, out string[] arguments)
		{
			string text = null;
			try
			{
				using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					text = new StreamReader(fileStream).ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				this.reporter(string.Format("Error: Can't open command line argument file '{0}' : '{1}'", fileName, ex.Message));
				arguments = null;
				return false;
			}
			bool result = false;
			ArrayList arrayList = new ArrayList();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			int num = 0;
			try
			{
				for (;;)
				{
					while (char.IsWhiteSpace(text[num]))
					{
						num++;
					}
					if (text[num] == '#')
					{
						num++;
						while (text[num] != '\n')
						{
							num++;
						}
					}
					else
					{
						do
						{
							if (text[num] == '\\')
							{
								int num2 = 1;
								num++;
								while (num == text.Length && text[num] == '\\')
								{
									num2++;
								}
								if (num == text.Length || text[num] != '"')
								{
									stringBuilder.Append('\\', num2);
								}
								else
								{
									stringBuilder.Append('\\', num2 >> 1);
									if (0 != (num2 & 1))
									{
										stringBuilder.Append('"');
									}
									else
									{
										flag = !flag;
									}
								}
							}
							else if (text[num] == '"')
							{
								flag = !flag;
								num++;
							}
							else
							{
								stringBuilder.Append(text[num]);
								num++;
							}
						}
						while (!char.IsWhiteSpace(text[num]) || flag);
						arrayList.Add(stringBuilder.ToString());
						stringBuilder.Length = 0;
					}
				}
			}
			catch (IndexOutOfRangeException)
			{
				if (flag)
				{
					this.reporter(string.Format("Error: Unbalanced '\"' in command line argument file '{0}'", fileName));
					result = true;
				}
				else if (stringBuilder.Length > 0)
				{
					arrayList.Add(stringBuilder.ToString());
				}
			}
			arguments = (string[])arrayList.ToArray(typeof(string));
			return result;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003110 File Offset: 0x00001310
		private static string LongName(ArgumentAttribute attribute, FieldInfo field)
		{
			return (attribute == null || attribute.DefaultLongName) ? field.Name : attribute.LongName;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000313C File Offset: 0x0000133C
		private static string ShortName(ArgumentAttribute attribute, FieldInfo field)
		{
			string result;
			if (attribute is DefaultArgumentAttribute)
			{
				result = null;
			}
			else if (!Parser.ExplicitShortName(attribute))
			{
				result = Parser.LongName(attribute, field).Substring(0, 1);
			}
			else
			{
				result = attribute.ShortName;
			}
			return result;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003184 File Offset: 0x00001384
		private static string HelpText(ArgumentAttribute attribute, FieldInfo field)
		{
			string result;
			if (attribute == null)
			{
				result = null;
			}
			else
			{
				result = attribute.HelpText;
			}
			return result;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000031AC File Offset: 0x000013AC
		private static bool IsHidden(ArgumentAttribute attribute)
		{
			return attribute != null && (attribute.Type & ArgumentType.Hidden) == ArgumentType.Hidden;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000031D0 File Offset: 0x000013D0
		private static bool HasHelpText(ArgumentAttribute attribute)
		{
			return attribute != null && attribute.HasHelpText;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000031F0 File Offset: 0x000013F0
		private static bool ExplicitShortName(ArgumentAttribute attribute)
		{
			return attribute != null && !attribute.DefaultShortName;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003214 File Offset: 0x00001414
		private static object DefaultValue(ArgumentAttribute attribute, FieldInfo field)
		{
			return (attribute == null || !attribute.HasDefaultValue) ? null : attribute.DefaultValue;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000323C File Offset: 0x0000143C
		private static Type ElementType(FieldInfo field)
		{
			Type result;
			if (Parser.IsCollectionType(field.FieldType))
			{
				result = field.FieldType.GetElementType();
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003270 File Offset: 0x00001470
		private static ArgumentType Flags(ArgumentAttribute attribute, FieldInfo field)
		{
			ArgumentType result;
			if (attribute != null)
			{
				result = attribute.Type;
			}
			else if (Parser.IsCollectionType(field.FieldType))
			{
				result = ArgumentType.MultipleUnique;
			}
			else
			{
				result = ArgumentType.AtMostOnce;
			}
			return result;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000032AC File Offset: 0x000014AC
		private static bool IsCollectionType(Type type)
		{
			return type.IsArray;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000032C4 File Offset: 0x000014C4
		private static bool IsValidElementType(Type type)
		{
			return type != null && (type == typeof(int) || type == typeof(uint) || type == typeof(double) || type == typeof(string) || type == typeof(bool) || type.IsEnum);
		}

		// Token: 0x0400000E RID: 14
		public const string NewLine = "\r\n";

		// Token: 0x0400000F RID: 15
		private const int STD_OUTPUT_HANDLE = -11;

		// Token: 0x04000010 RID: 16
		private const int spaceBeforeParam = 2;

		// Token: 0x04000011 RID: 17
		private const int minParameterSpecsForResponseFile = 2;

		// Token: 0x04000012 RID: 18
		private ArrayList arguments;

		// Token: 0x04000013 RID: 19
		private Hashtable argumentMap;

		// Token: 0x04000014 RID: 20
		private ArrayList defaultArguments;

		// Token: 0x04000015 RID: 21
		private int usedDefaults;

		// Token: 0x04000016 RID: 22
		private ErrorReporter reporter;

		// Token: 0x02000008 RID: 8
		private class HelpArgument
		{
			// Token: 0x1700000B RID: 11
			// (get) Token: 0x0600003C RID: 60 RVA: 0x00003348 File Offset: 0x00001548
			public bool Help
			{
				get
				{
					return this.help || this.help2;
				}
			}

			// Token: 0x04000017 RID: 23
			[Argument(ArgumentType.AtMostOnce, ShortName = "?")]
			private bool help = false;

			// Token: 0x04000018 RID: 24
			[Argument(ArgumentType.AtMostOnce, ShortName = "h")]
			private bool help2 = false;
		}

		// Token: 0x02000009 RID: 9
		private struct COORD
		{
			// Token: 0x04000019 RID: 25
			internal short x;

			// Token: 0x0400001A RID: 26
			internal short y;
		}

		// Token: 0x0200000A RID: 10
		private struct SMALL_RECT
		{
			// Token: 0x0400001B RID: 27
			internal short Left;

			// Token: 0x0400001C RID: 28
			internal short Top;

			// Token: 0x0400001D RID: 29
			internal short Right;

			// Token: 0x0400001E RID: 30
			internal short Bottom;
		}

		// Token: 0x0200000B RID: 11
		private struct CONSOLE_SCREEN_BUFFER_INFO
		{
			// Token: 0x0400001F RID: 31
			internal Parser.COORD dwSize;

			// Token: 0x04000020 RID: 32
			internal Parser.COORD dwCursorPosition;

			// Token: 0x04000021 RID: 33
			internal short wAttributes;

			// Token: 0x04000022 RID: 34
			internal Parser.SMALL_RECT srWindow;

			// Token: 0x04000023 RID: 35
			internal Parser.COORD dwMaximumWindowSize;
		}

		// Token: 0x0200000C RID: 12
		private struct ArgumentHelpStrings
		{
			// Token: 0x0600003E RID: 62 RVA: 0x00003383 File Offset: 0x00001583
			public ArgumentHelpStrings(string syntax, string help)
			{
				this.syntax = syntax;
				this.help = help;
			}

			// Token: 0x04000024 RID: 36
			public string syntax;

			// Token: 0x04000025 RID: 37
			public string help;
		}

		// Token: 0x0200000D RID: 13
		private class Argument
		{
			// Token: 0x0600003F RID: 63 RVA: 0x00003394 File Offset: 0x00001594
			public Argument(ArgumentAttribute attribute, FieldInfo field, ErrorReporter reporter)
			{
				this.longName = Parser.LongName(attribute, field);
				this.explicitShortName = Parser.ExplicitShortName(attribute);
				this.shortName = Parser.ShortName(attribute, field);
				this.hasHelpText = Parser.HasHelpText(attribute);
				this.helpText = Parser.HelpText(attribute, field);
				this.defaultValue = Parser.DefaultValue(attribute, field);
				this.isHidden = Parser.IsHidden(attribute);
				this.elementType = Parser.ElementType(field);
				this.flags = Parser.Flags(attribute, field);
				this.field = field;
				this.seenValue = false;
				this.reporter = reporter;
				this.isDefault = (attribute != null && attribute is DefaultArgumentAttribute);
				if (this.IsCollection)
				{
					this.collectionValues = new ArrayList();
				}
			}

			// Token: 0x06000040 RID: 64 RVA: 0x00003460 File Offset: 0x00001660
			public bool Finish(object destination)
			{
				if (!this.SeenValue && this.HasDefaultValue)
				{
					this.field.SetValue(destination, this.DefaultValue);
				}
				if (this.IsCollection)
				{
					this.field.SetValue(destination, this.collectionValues.ToArray(this.elementType));
				}
				return this.ReportMissingRequiredArgument();
			}

			// Token: 0x06000041 RID: 65 RVA: 0x000034D4 File Offset: 0x000016D4
			private bool ReportMissingRequiredArgument()
			{
				bool result;
				if (this.IsRequired && !this.SeenValue)
				{
					if (this.IsDefault)
					{
						this.reporter(string.Format("Missing required argument '<{0}>'.", this.LongName));
					}
					else
					{
						this.reporter(string.Format("Missing required argument '/{0}'.", this.LongName));
					}
					result = true;
				}
				else
				{
					result = false;
				}
				return result;
			}

			// Token: 0x06000042 RID: 66 RVA: 0x00003549 File Offset: 0x00001749
			private void ReportDuplicateArgumentValue(string value)
			{
				this.reporter(string.Format("Duplicate '{0}' argument '{1}'", this.LongName, value));
			}

			// Token: 0x06000043 RID: 67 RVA: 0x0000356C File Offset: 0x0000176C
			public bool SetValue(string value, object destination)
			{
				bool result;
				if (this.SeenValue && !this.AllowMultiple)
				{
					this.reporter(string.Format("Duplicate '{0}' argument", this.LongName));
					result = false;
				}
				else
				{
					this.seenValue = true;
					object obj;
					if (!this.ParseValue(this.ValueType, value, out obj))
					{
						result = false;
					}
					else
					{
						if (this.IsCollection)
						{
							if (this.Unique && this.collectionValues.Contains(obj))
							{
								this.ReportDuplicateArgumentValue(value);
								return false;
							}
							this.collectionValues.Add(obj);
						}
						else
						{
							this.field.SetValue(destination, obj);
						}
						result = true;
					}
				}
				return result;
			}

			// Token: 0x1700000C RID: 12
			// (get) Token: 0x06000044 RID: 68 RVA: 0x00003630 File Offset: 0x00001830
			public Type ValueType
			{
				get
				{
					return this.IsCollection ? this.elementType : this.Type;
				}
			}

			// Token: 0x06000045 RID: 69 RVA: 0x00003659 File Offset: 0x00001859
			private void ReportBadArgumentValue(string value)
			{
				this.reporter(string.Format("'{0}' is not a valid value for the '{1}' command line option", value, this.LongName));
			}

			// Token: 0x06000046 RID: 70 RVA: 0x0000367C File Offset: 0x0000187C
			private bool ParseValue(Type type, string stringData, out object value)
			{
				if ((stringData != null || type == typeof(bool)) && (stringData == null || stringData.Length > 0))
				{
					try
					{
						if (type == typeof(string))
						{
							value = stringData;
							return true;
						}
						if (type == typeof(bool))
						{
							if (stringData == null || stringData == "+")
							{
								value = true;
								return true;
							}
							if (stringData == "-")
							{
								value = false;
								return true;
							}
						}
						else
						{
							if (type == typeof(int))
							{
								value = int.Parse(stringData);
								return true;
							}
							if (type == typeof(uint))
							{
								value = int.Parse(stringData);
								return true;
							}
							if (type == typeof(double))
							{
								value = double.Parse(stringData);
								return true;
							}
							value = Enum.Parse(type, stringData, true);
							return true;
						}
					}
					catch
					{
					}
				}
				this.ReportBadArgumentValue(stringData);
				value = null;
				return false;
			}

			// Token: 0x06000047 RID: 71 RVA: 0x0000380C File Offset: 0x00001A0C
			private void AppendValue(StringBuilder builder, object value)
			{
				if (value is string || value is int || value is uint || value.GetType().IsEnum)
				{
					builder.Append(value.ToString());
				}
				else if (value is bool)
				{
					builder.Append(((bool)value) ? "+" : "-");
				}
				else
				{
					bool flag = true;
					foreach (object value2 in ((Array)value))
					{
						if (!flag)
						{
							builder.Append(", ");
						}
						this.AppendValue(builder, value2);
						flag = false;
					}
				}
			}

			// Token: 0x1700000D RID: 13
			// (get) Token: 0x06000048 RID: 72 RVA: 0x00003900 File Offset: 0x00001B00
			public string LongName
			{
				get
				{
					return this.longName;
				}
			}

			// Token: 0x1700000E RID: 14
			// (get) Token: 0x06000049 RID: 73 RVA: 0x00003918 File Offset: 0x00001B18
			public bool ExplicitShortName
			{
				get
				{
					return this.explicitShortName;
				}
			}

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x0600004A RID: 74 RVA: 0x00003930 File Offset: 0x00001B30
			public string ShortName
			{
				get
				{
					return this.shortName;
				}
			}

			// Token: 0x17000010 RID: 16
			// (get) Token: 0x0600004B RID: 75 RVA: 0x00003948 File Offset: 0x00001B48
			public bool HasShortName
			{
				get
				{
					return this.shortName != null;
				}
			}

			// Token: 0x0600004C RID: 76 RVA: 0x00003966 File Offset: 0x00001B66
			public void ClearShortName()
			{
				this.shortName = null;
			}

			// Token: 0x17000011 RID: 17
			// (get) Token: 0x0600004D RID: 77 RVA: 0x00003970 File Offset: 0x00001B70
			public bool IsHidden
			{
				get
				{
					return this.isHidden;
				}
			}

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x0600004E RID: 78 RVA: 0x00003988 File Offset: 0x00001B88
			public bool HasHelpText
			{
				get
				{
					return this.hasHelpText;
				}
			}

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x0600004F RID: 79 RVA: 0x000039A0 File Offset: 0x00001BA0
			public string HelpText
			{
				get
				{
					return this.helpText;
				}
			}

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x06000050 RID: 80 RVA: 0x000039B8 File Offset: 0x00001BB8
			public object DefaultValue
			{
				get
				{
					return this.defaultValue;
				}
			}

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x06000051 RID: 81 RVA: 0x000039D0 File Offset: 0x00001BD0
			public bool HasDefaultValue
			{
				get
				{
					return null != this.defaultValue;
				}
			}

			// Token: 0x17000016 RID: 22
			// (get) Token: 0x06000052 RID: 82 RVA: 0x000039F0 File Offset: 0x00001BF0
			public string FullHelpText
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (this.HasHelpText)
					{
						stringBuilder.Append(this.HelpText);
					}
					if (this.HasDefaultValue)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(" ");
						}
						stringBuilder.Append("Default value:'");
						this.AppendValue(stringBuilder, this.DefaultValue);
						stringBuilder.Append('\'');
					}
					if (this.HasShortName)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(" ");
						}
						stringBuilder.Append("(short form /");
						stringBuilder.Append(this.ShortName);
						stringBuilder.Append(")");
					}
					return stringBuilder.ToString();
				}
			}

			// Token: 0x17000017 RID: 23
			// (get) Token: 0x06000053 RID: 83 RVA: 0x00003AC8 File Offset: 0x00001CC8
			public string SyntaxHelp
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (this.IsDefault)
					{
						stringBuilder.Append("<");
						stringBuilder.Append(this.LongName);
						stringBuilder.Append(">");
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(this.LongName);
						Type valueType = this.ValueType;
						if (valueType == typeof(int))
						{
							stringBuilder.Append(":<int>");
						}
						else if (valueType == typeof(uint))
						{
							stringBuilder.Append(":<uint>");
						}
						else if (valueType == typeof(bool))
						{
							stringBuilder.Append("[+|-]");
						}
						else if (valueType == typeof(string))
						{
							stringBuilder.Append(":<string>");
						}
						else if (valueType == typeof(double))
						{
							stringBuilder.Append(":<double>");
						}
						else
						{
							stringBuilder.Append(":{");
							bool flag = true;
							foreach (FieldInfo fieldInfo in valueType.GetFields())
							{
								if (fieldInfo.IsStatic)
								{
									if (flag)
									{
										flag = false;
									}
									else
									{
										stringBuilder.Append('|');
									}
									stringBuilder.Append(fieldInfo.Name);
								}
							}
							stringBuilder.Append('}');
						}
					}
					return stringBuilder.ToString();
				}
			}

			// Token: 0x17000018 RID: 24
			// (get) Token: 0x06000054 RID: 84 RVA: 0x00003C98 File Offset: 0x00001E98
			public bool IsRequired
			{
				get
				{
					return ArgumentType.AtMostOnce != (this.flags & ArgumentType.Required);
				}
			}

			// Token: 0x17000019 RID: 25
			// (get) Token: 0x06000055 RID: 85 RVA: 0x00003CB8 File Offset: 0x00001EB8
			public bool SeenValue
			{
				get
				{
					return this.seenValue;
				}
			}

			// Token: 0x1700001A RID: 26
			// (get) Token: 0x06000056 RID: 86 RVA: 0x00003CD0 File Offset: 0x00001ED0
			public bool AllowMultiple
			{
				get
				{
					return ArgumentType.AtMostOnce != (this.flags & ArgumentType.Multiple);
				}
			}

			// Token: 0x1700001B RID: 27
			// (get) Token: 0x06000057 RID: 87 RVA: 0x00003CF0 File Offset: 0x00001EF0
			public bool Unique
			{
				get
				{
					return ArgumentType.AtMostOnce != (this.flags & ArgumentType.Unique);
				}
			}

			// Token: 0x1700001C RID: 28
			// (get) Token: 0x06000058 RID: 88 RVA: 0x00003D10 File Offset: 0x00001F10
			public Type Type
			{
				get
				{
					return this.field.FieldType;
				}
			}

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x06000059 RID: 89 RVA: 0x00003D30 File Offset: 0x00001F30
			public bool IsCollection
			{
				get
				{
					return Parser.IsCollectionType(this.Type);
				}
			}

			// Token: 0x1700001E RID: 30
			// (get) Token: 0x0600005A RID: 90 RVA: 0x00003D50 File Offset: 0x00001F50
			public bool IsDefault
			{
				get
				{
					return this.isDefault;
				}
			}

			// Token: 0x04000026 RID: 38
			private string longName;

			// Token: 0x04000027 RID: 39
			private string shortName;

			// Token: 0x04000028 RID: 40
			private string helpText;

			// Token: 0x04000029 RID: 41
			private bool isHidden;

			// Token: 0x0400002A RID: 42
			private bool hasHelpText;

			// Token: 0x0400002B RID: 43
			private bool explicitShortName;

			// Token: 0x0400002C RID: 44
			private object defaultValue;

			// Token: 0x0400002D RID: 45
			private bool seenValue;

			// Token: 0x0400002E RID: 46
			private FieldInfo field;

			// Token: 0x0400002F RID: 47
			private Type elementType;

			// Token: 0x04000030 RID: 48
			private ArgumentType flags;

			// Token: 0x04000031 RID: 49
			private ArrayList collectionValues;

			// Token: 0x04000032 RID: 50
			private ErrorReporter reporter;

			// Token: 0x04000033 RID: 51
			private bool isDefault;
		}
	}
}
