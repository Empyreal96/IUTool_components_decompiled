using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000007 RID: 7
	public class CmdArgsParser
	{
		// Token: 0x0600004A RID: 74 RVA: 0x000036EE File Offset: 0x000018EE
		public static T ParseArgs<T>(string[] args, params object[] configuration) where T : class, new()
		{
			return CmdArgsParser.ParseArgs<T>(args.ToList<string>(), configuration);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000036FC File Offset: 0x000018FC
		public static T ParseArgs<T>(List<string> args, params object[] configuration) where T : class, new()
		{
			List<CmdModes> list = new List<CmdModes>();
			foreach (object obj in configuration)
			{
				if (!(obj is CmdModes))
				{
					throw new ArgumentException(string.Format("A configuration argument was passed that was not of type \"CmdModes\". ARG={0}", obj));
				}
				list.Add((CmdModes)obj);
			}
			List<string> list2 = new List<string>();
			foreach (string text in args)
			{
				if (text.StartsWith("@", StringComparison.OrdinalIgnoreCase))
				{
					if (!File.Exists(text.Substring(1)))
					{
						throw new FileNotFoundException(string.Format("Response file '{0}' could not be found.", text.Substring(1)));
					}
					foreach (string item in File.ReadAllLines(text.Substring(1)))
					{
						list2.Add(item);
					}
				}
				else
				{
					list2.Add(text);
				}
			}
			args = list2;
			Type typeFromHandle = typeof(T);
			Dictionary<string, string> dictionary = CmdArgsParser.ProcessCommandLine<T>(args, list);
			if (dictionary.ContainsKey("?"))
			{
				CmdArgsParser.ParseUsage<T>(list);
				return default(T);
			}
			while (!list.Contains(CmdModes.DisableCFG) && dictionary.ContainsKey("cfg"))
			{
				string configLocation = dictionary["cfg"];
				dictionary.Remove("cfg");
				dictionary = CmdArgsParser.ParseConfig(configLocation, dictionary, list);
			}
			CmdArgsParser.MissingArguments<T>(dictionary, list);
			dictionary = CmdArgsParser.ExtraArguments<T>(dictionary, list);
			T t = Activator.CreateInstance<T>();
			using (Dictionary<string, string>.Enumerator enumerator2 = dictionary.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KeyValuePair<string, string> commandEntry = enumerator2.Current;
					PropertyInfo property = typeFromHandle.GetProperty(commandEntry.Key);
					Type type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
					if (type.IsGenericType)
					{
						if (type.GetGenericTypeDefinition() == typeof(List<>))
						{
							IList value = CmdArgsParser.ReflectionListFactory(type.GetGenericArguments()[0], commandEntry.Value);
							property.SetValue(t, value, null);
						}
						else if (type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
						{
							Type keyType = type.GetGenericArguments()[0];
							Type valueType = type.GetGenericArguments()[1];
							IDictionary value2 = CmdArgsParser.ReflectionDictionaryFactory(keyType, valueType, commandEntry.Value);
							property.SetValue(t, value2, null);
						}
						else
						{
							if (!(type.GetGenericTypeDefinition() == typeof(HashSet<>)))
							{
								throw new NotImplementedException(string.Format("CmdArgsParser does not support generic type '{0}'.", type.Name));
							}
							IEnumerable value3 = CmdArgsParser.ReflectionSetFactory(type.GetGenericArguments()[0], commandEntry.Value, commandEntry.Key);
							property.SetValue(t, value3, null);
						}
					}
					else if (type.IsEnum)
					{
						if (!Enum.GetNames(type).Any((string x) => x.Equals(commandEntry.Value, StringComparison.OrdinalIgnoreCase)))
						{
							string text2 = Enum.GetNames(type)[0];
							foreach (string arg in Enum.GetNames(type).Skip(1))
							{
								text2 += string.Format(" {0}", arg);
							}
							throw new ArgumentException(string.Format("The value for \"{0}\" is incorrect.\nValue: {1}\nSupported Values: {2}", commandEntry.Key, commandEntry.Value, text2));
						}
						property.SetValue(t, Convert.ChangeType(Enum.Parse(type, commandEntry.Value, true), type), null);
					}
					else
					{
						if (!TypeDescriptor.GetConverter(type).IsValid(commandEntry.Value))
						{
							throw new ArgumentException(string.Format("The value for \"{0}\" is incorrect.\nValue '{1}' cannot be converted to switch type '{2}'.", commandEntry.Key, commandEntry.Value, type.Name));
						}
						property.SetValue(t, Convert.ChangeType(commandEntry.Value, type), null);
					}
				}
			}
			return t;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003B90 File Offset: 0x00001D90
		public static void ParseUsage<T>(List<CmdModes> modes) where T : class, new()
		{
			if (modes.Contains(CmdModes.DisableUsage))
			{
				return;
			}
			int num = 12;
			Type typeFromHandle = typeof(T);
			PropertyInfo[] properties = typeFromHandle.GetProperties();
			T t = Activator.CreateInstance<T>();
			string text = string.Format("Usage: {0}", Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName));
			foreach (DescriptionAttribute descriptionAttribute in typeFromHandle.GetCustomAttributes(typeof(DescriptionAttribute), false))
			{
				text = text + "\n" + descriptionAttribute.Description;
			}
			foreach (PropertyInfo propertyInfo in properties)
			{
				string text2;
				if (!propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), false).Any<object>())
				{
					text2 = string.Format("[{0}]", propertyInfo.Name);
				}
				else
				{
					text2 = propertyInfo.Name;
				}
				if (text2.Length + 1 > num)
				{
					num = text2.Length + 1;
				}
				text += string.Format(" {0}", text2);
			}
			if (!modes.Contains(CmdModes.DisableCFG))
			{
				text += " [cfg]";
			}
			foreach (PropertyInfo propertyInfo2 in properties)
			{
				string text3 = "";
				object[] customAttributes = propertyInfo2.GetCustomAttributes(typeof(DescriptionAttribute), false);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					text3 = ((DescriptionAttribute)customAttributes[j]).Description;
				}
				if (propertyInfo2.GetCustomAttributes(typeof(CmdArgsParser.CaseInsensitive), false).Any<object>())
				{
					text3 = text3 + "\n" + new string(' ', num + 3) + "Case Insensitive.";
				}
				string text4 = null;
				string text5;
				if (!propertyInfo2.GetCustomAttributes(typeof(RequiredAttribute), false).Any<object>())
				{
					text5 = string.Format("[{0}]", propertyInfo2.Name);
					if (propertyInfo2.GetValue(t, null) == null)
					{
						text4 = null;
					}
					else
					{
						text4 = propertyInfo2.GetValue(t, null).ToString();
					}
				}
				else
				{
					text5 = propertyInfo2.Name;
				}
				string text6 = new string('·', num - text5.Length);
				string text7 = new string(' ', num);
				string text8 = string.Format("  {0}{1} {2}\n{3}  ", new object[]
				{
					text5,
					text6,
					text3,
					text7
				});
				Type type = Nullable.GetUnderlyingType(propertyInfo2.PropertyType) ?? propertyInfo2.PropertyType;
				if (type.IsGenericType)
				{
					if (type.GetGenericTypeDefinition() == typeof(List<>))
					{
						text8 += string.Format(" Values:<{0}>, Multiple values seperated by ';'", type.GetGenericArguments()[0].Name);
					}
					else if (type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
					{
						text8 += string.Format(" Values:<{0}={1}>, Multiple values seperated by ';'", type.GetGenericArguments()[0].Name, type.GetGenericArguments()[1].Name);
					}
					else if (type.GetGenericTypeDefinition() == typeof(HashSet<>))
					{
						text8 += string.Format(" Values:<{0}>, Multiple values seperated by ';'\n{1}   Duplicates are not supported.", type.GetGenericArguments()[0].Name, text7);
					}
				}
				else if (type.IsEnum)
				{
					text8 += string.Format(" Values:<{0}", Enum.GetNames(type)[0]);
					foreach (string arg in Enum.GetNames(type).Skip(1))
					{
						text8 += string.Format(" | {0}", arg);
					}
					text8 += ">";
				}
				else if (type == typeof(bool))
				{
					text8 += string.Format(" Values:<true | false>", new object[0]);
				}
				else if (type == typeof(int))
				{
					text8 += string.Format(" Values:<Number>", new object[0]);
				}
				else if (type == typeof(float))
				{
					text8 += string.Format(" Values:<Decimal>", new object[0]);
				}
				else
				{
					text8 += string.Format(" Values:<Free Text>", new object[0]);
				}
				if (!propertyInfo2.GetCustomAttributes(typeof(RequiredAttribute), false).Any<object>())
				{
					if (text4 == null)
					{
						text8 += string.Format(" Default=NULL", new object[0]);
					}
					else if (propertyInfo2.PropertyType == typeof(string))
					{
						text8 += string.Format(" Default=\"{0}\"", text4);
					}
					else
					{
						text8 += string.Format(" Default={0}", text4);
					}
				}
				text += string.Format("\n{0}", text8);
			}
			if (!modes.Contains(CmdModes.DisableCFG))
			{
				string text9 = new string('·', num - "[cfg]".Length);
				string text10 = new string(' ', num);
				text += string.Format("\n  {0}{1} {2}\n{3}   {4}\n{3}   Values:<Free Text>", new object[]
				{
					"[cfg]",
					text9,
					"A configuration file used to configure the enviornment.",
					text10,
					"If supplied the configuration file will override the command line. Used as named argument only."
				});
			}
			Console.WriteLine(text);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000411C File Offset: 0x0000231C
		public static Dictionary<string, string> ParseConfig(string configLocation, Dictionary<string, string> cmds, List<CmdModes> modes)
		{
			if (string.IsNullOrEmpty(configLocation) || configLocation.Equals("true"))
			{
				throw new ArgumentNullException(string.Format("No configuration file was specified on switch \"cfg\". Please specify a config location with: /cfg:<location>", new object[0]));
			}
			if (!File.Exists(configLocation))
			{
				throw new FileNotFoundException(string.Format("Can't find configuration file {0}.", configLocation));
			}
			XDocument xdocument = XDocument.Load(configLocation, LoadOptions.SetLineInfo);
			if (!xdocument.Root.Name.LocalName.Equals("Configuration"))
			{
				throw new FormatException(string.Format("The format of the provided configuration file is incorrect. Root element was \"{0}\" instead of \"Configuration\". CFG:{1}", xdocument.Root.Name.LocalName, configLocation));
			}
			HashSet<string> hashSet = new HashSet<string>();
			List<string> list = new List<string>();
			foreach (XElement xelement in xdocument.Root.Elements())
			{
				string text = xelement.Name.LocalName.ToString();
				IXmlLineInfo xmlLineInfo = xelement;
				if (xelement.HasElements)
				{
					if (xelement.Attribute("name") == null)
					{
						throw new FormatException(string.Format("Container '{0}' is missing a 'name' attribute. Containers require a name attribute. Error at ({1},{2})", xelement.Name.LocalName, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition));
					}
					text = xelement.Attribute("name").Value.ToString();
				}
				if (string.IsNullOrEmpty(xelement.Value.ToString()) && !xelement.HasElements)
				{
					throw new FormatException(string.Format("Key \"{0}\" does not have an associated value. Null values are not supported in the configuration file. Keys need to be in to format: <\"key\">value</\"key\">.", xelement.Name.LocalName));
				}
				string text2 = xelement.Value.ToString();
				if (hashSet.Contains(text))
				{
					list.Add(string.Format("Key: {0} at ({1},{2})", text, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition));
				}
				else
				{
					hashSet.Add(text);
					bool flag = false;
					if (xelement.HasElements)
					{
						flag = true;
						XElement xelement2 = xelement.Elements().First<XElement>();
						if (xelement.Name.LocalName == "List" || xelement.Name.LocalName == "HashSet")
						{
							if (xelement2.Name.LocalName != "Value")
							{
								throw new FormatException(string.Format("Element under Container '{0}' is not reconized. '{1}' is not supported. Use 'Value' for tags. Error at ({2},{3})", new object[]
								{
									xelement.Name.LocalName,
									xelement2.Name.LocalName,
									xmlLineInfo.LineNumber,
									xmlLineInfo.LinePosition
								}));
							}
							text2 = string.Format("{0}", xelement2.Value.ToString());
						}
						else
						{
							if (!(xelement.Name.LocalName == "Dictionary"))
							{
								throw new FormatException(string.Format("Container '{0}' is not reconized. Supported: 'List', 'HashSet', 'Dictionary'. Error at ({1},{2})", xelement.Name.LocalName, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition));
							}
							if (xelement2.Name.LocalName.ToString().Contains(' '))
							{
								throw new FormatException(string.Format("'{0}' is not allowed for use as a Key in a Dictionary. Spaces are not supported in Key names. Error at ({1},{2})", xelement2.Name.LocalName, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition));
							}
							text2 = string.Format("{0}={1}", xelement2.Name.LocalName.ToString(), xelement2.Value.ToString());
						}
						foreach (XElement xelement3 in xelement.Elements().Skip(1))
						{
							if (xelement.Name.LocalName == "List" || xelement.Name.LocalName == "HashSet")
							{
								if (xelement3.Name.LocalName != "Value")
								{
									throw new FormatException(string.Format("Element under Container '{0}' is not reconized. '{1}' is not supported. Use 'Value' for tags. Error at ({2},{3})", new object[]
									{
										xelement.Name.LocalName,
										xelement3.Name.LocalName,
										xmlLineInfo.LineNumber,
										xmlLineInfo.LinePosition
									}));
								}
								text2 += string.Format(";{0}", xelement3.Value.ToString());
							}
							else
							{
								if (xelement3.Name.LocalName.ToString().Contains(' '))
								{
									throw new FormatException(string.Format("'{0}' is not allowed for use as a Key in a Dictionary. Spaces are not supported in Key names. Error at ({1},{2})", xelement3.Name.LocalName, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition));
								}
								text2 += string.Format(";{0}={1}", xelement3.Name.LocalName.ToString(), xelement3.Value.ToString());
							}
						}
					}
					if (cmds.ContainsKey(text))
					{
						if (modes.Contains(CmdModes.CFGOverride))
						{
							cmds[text] = text2;
						}
						else if (flag)
						{
							string key = text;
							cmds[key] += string.Format(";{0}", text2);
						}
					}
					else
					{
						cmds.Add(text, text2);
					}
				}
			}
			if (list.Any<string>())
			{
				string text3 = list.First<string>();
				foreach (string arg in list.Skip(1))
				{
					text3 += string.Format("\n{0}", arg);
				}
				throw new FormatException(string.Format("There were {0} duplicate entries in {1}. Duplicate Keys:\n{2}", list.Count<string>(), configLocation, text3));
			}
			return cmds;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004730 File Offset: 0x00002930
		private static void MissingArguments<T>(Dictionary<string, string> commandTable, List<CmdModes> modes) where T : class, new()
		{
			Type typeFromHandle = typeof(T);
			List<string> list = new List<string>();
			foreach (PropertyInfo propertyInfo in typeFromHandle.GetProperties())
			{
				if (propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), false).Any<object>() && !commandTable.ContainsKey(propertyInfo.Name))
				{
					list.Add(propertyInfo.Name);
				}
			}
			if (list.Any<string>())
			{
				CmdArgsParser.ParseUsage<T>(modes);
				string text = string.Format("\"{0}\"", list.First<string>());
				foreach (string arg in list.Skip(1))
				{
					text += string.Format(", \"{0}\"", arg);
				}
				throw new ArgumentNullException(string.Format("Required argument {0} {1} not specified.", text, (list.Count > 1) ? "were" : "was"));
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004838 File Offset: 0x00002A38
		private static Dictionary<string, string> ExtraArguments<T>(Dictionary<string, string> commandTable, List<CmdModes> modes) where T : class, new()
		{
			Type typeFromHandle = typeof(T);
			List<string> list = new List<string>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> keyValuePair in commandTable)
			{
				if (typeFromHandle.GetProperty(keyValuePair.Key) == null)
				{
					bool flag = true;
					if (modes.Contains(CmdModes.LegacySwitchFormat))
					{
						foreach (PropertyInfo propertyInfo in typeFromHandle.GetProperties())
						{
							if (propertyInfo.Name.StartsWith(keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
							{
								dictionary[propertyInfo.Name] = keyValuePair.Value;
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						list.Add(keyValuePair.Key);
					}
				}
				else
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			if (list.Any<string>())
			{
				CmdArgsParser.ParseUsage<T>(modes);
				string text = string.Format("\"{0}\"", list.First<string>());
				foreach (string arg in list.Skip(1))
				{
					text += string.Format(", \"{0}\"", arg);
				}
				throw new ArgumentOutOfRangeException(string.Format("Unknown argument {0} {1} provided.", text, (list.Count > 1) ? "were" : "was"));
			}
			return dictionary;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000049D4 File Offset: 0x00002BD4
		private static Dictionary<string, string> ProcessCommandLine<T>(List<string> args, List<CmdModes> modes) where T : class, new()
		{
			Type typeFromHandle = typeof(T);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			bool flag = false;
			foreach (string text in args)
			{
				string value = "true";
				bool flag2 = false;
				if (modes.Contains(CmdModes.LegacySwitchFormat))
				{
					if (text.First<char>() == '-')
					{
						flag2 = true;
						value = "false";
					}
					else if (text.First<char>() == '+')
					{
						flag2 = true;
					}
				}
				else if (text.First<char>() == '-' || text.First<char>() == '+')
				{
					throw new FormatException(string.Format("Argument {0} is in the wrong format. Legacy arguments are not supported. ARG={1}", dictionary.Count + 1, text));
				}
				string text2;
				if (text.First<char>() != '/' && !flag2)
				{
					if (flag)
					{
						throw new FormatException(string.Format("Argument {0} is in the wrong format. After a named argument all arguments must be named. ARG={1}", dictionary.Count + 1, text));
					}
					if (dictionary.Count >= typeFromHandle.GetProperties().Count<PropertyInfo>())
					{
						throw new ArgumentException(string.Format("To many positional arguments supplied. Amount allowed: {0}\nOffending Argument: {1}", typeFromHandle.GetProperties().Count<PropertyInfo>(), text));
					}
					text2 = typeFromHandle.GetProperties()[dictionary.Count].Name;
					value = text;
				}
				else
				{
					flag = true;
					if (text.IndexOf(":", StringComparison.OrdinalIgnoreCase) == -1)
					{
						text2 = text.Substring(1);
					}
					else
					{
						text2 = text.Substring(1, text.IndexOf(":", StringComparison.OrdinalIgnoreCase) - 1);
					}
					if (text.IndexOf(":", StringComparison.OrdinalIgnoreCase) != -1)
					{
						string text3 = text;
						value = text3.Substring(text3.IndexOf(":", StringComparison.OrdinalIgnoreCase) + 1);
					}
					else
					{
						PropertyInfo propertyInfo = typeFromHandle.GetProperty(text2);
						if (propertyInfo == null)
						{
							propertyInfo = typeFromHandle.GetProperty(text2, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
							if (propertyInfo != null)
							{
								if (propertyInfo.GetCustomAttributes(typeof(CmdArgsParser.CaseInsensitive), false).Any<object>())
								{
									text2 = propertyInfo.Name;
								}
								else
								{
									propertyInfo = null;
								}
							}
						}
						if (propertyInfo != null)
						{
							Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
							if (!type.IsAssignableFrom(typeof(bool)))
							{
								throw new ArgumentException(string.Format("{0} was used as a 'Boolean' switch however the switch type is '{1}'.", text2, type.Name));
							}
						}
					}
				}
				if (dictionary.ContainsKey(text2))
				{
					throw new FormatException(string.Format("Argument {0} has already been declared. ARG={1}", text2, text));
				}
				dictionary.Add(text2, value);
			}
			return dictionary;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004C60 File Offset: 0x00002E60
		public static IList ReflectionListFactory(Type contentType, string dataSource)
		{
			IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
			{
				contentType
			}));
			foreach (string value in dataSource.Split(new char[]
			{
				';'
			}))
			{
				if (contentType.IsEnum)
				{
					list.Add(Convert.ChangeType(Enum.Parse(contentType, value), contentType));
				}
				else
				{
					list.Add(Convert.ChangeType(value, contentType));
				}
			}
			return list;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00004CE4 File Offset: 0x00002EE4
		public static IDictionary ReflectionDictionaryFactory(Type keyType, Type valueType, string dataSource)
		{
			IDictionary dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<, >).MakeGenericType(new Type[]
			{
				keyType,
				valueType
			}));
			HashSet<object> hashSet = new HashSet<object>();
			foreach (string text in dataSource.Split(new char[]
			{
				';'
			}))
			{
				string[] array2 = text.Split(new char[]
				{
					'='
				});
				if (array2.Count<string>() != 2)
				{
					throw new FormatException(string.Format("The format of a dictionary argument is incorrect. The format is 'key=value'. Offending value: '{0}'", text));
				}
				object obj;
				if (keyType.IsEnum)
				{
					obj = Convert.ChangeType(Enum.Parse(keyType, array2[0], true), keyType);
				}
				else
				{
					obj = Convert.ChangeType(array2[0], keyType);
				}
				if (!hashSet.Contains(obj))
				{
					hashSet.Add(obj);
					object value;
					if (valueType.IsEnum)
					{
						value = Convert.ChangeType(Enum.Parse(valueType, array2[1], true), valueType);
					}
					else
					{
						value = Convert.ChangeType(array2[1], valueType);
					}
					dictionary.Add(obj, value);
				}
			}
			return dictionary;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004DEC File Offset: 0x00002FEC
		public static IEnumerable ReflectionSetFactory(Type contentType, string dataSource, string name)
		{
			Type type = typeof(HashSet<>).MakeGenericType(new Type[]
			{
				contentType
			});
			IEnumerable enumerable = (IEnumerable)Activator.CreateInstance(type);
			MethodInfo method = type.GetMethod("Add");
			MethodInfo method2 = type.GetMethod("Contains");
			List<string> list = new List<string>();
			foreach (string value in dataSource.Split(new char[]
			{
				';'
			}))
			{
				if (contentType.IsEnum)
				{
					object obj = Convert.ChangeType(Enum.Parse(contentType, value), contentType);
					if ((bool)method2.Invoke(enumerable, new object[]
					{
						obj
					}))
					{
						list.Add(obj.ToString());
					}
					else
					{
						method.Invoke(enumerable, new object[]
						{
							obj
						});
					}
				}
				else
				{
					object obj2 = Convert.ChangeType(value, contentType);
					if ((bool)method2.Invoke(enumerable, new object[]
					{
						obj2
					}))
					{
						list.Add(obj2.ToString());
					}
					else
					{
						method.Invoke(enumerable, new object[]
						{
							obj2
						});
					}
				}
			}
			if (list.Any<string>())
			{
				string text = string.Format("\"{0}\"", list.First<string>());
				foreach (string arg in list.Skip(1))
				{
					text += string.Format(", \"{0}\"", arg);
				}
				throw new FormatException(string.Format("HashSet '{0}' had {1} duplicate value{2}. Duplicates: {3}", new object[]
				{
					name,
					list.Count<string>(),
					(list.Count<string>() > 1) ? "s" : "",
					text
				}));
			}
			return enumerable;
		}

		// Token: 0x04000021 RID: 33
		private const int HELP_INDENTATION = 12;

		// Token: 0x0200005D RID: 93
		[AttributeUsage(AttributeTargets.Property)]
		public class CaseInsensitive : Attribute
		{
		}
	}
}
