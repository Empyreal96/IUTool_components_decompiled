using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000002 RID: 2
	public class CommandLineParser
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public CommandLineParser()
		{
			this.BuildRegularExpression();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020DC File Offset: 0x000002DC
		public CommandLineParser(char yourOwnSwitch, char yourOwnDelimiter) : this()
		{
			this.m_switchChar = yourOwnSwitch;
			this.m_delimChar = yourOwnDelimiter;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020F2 File Offset: 0x000002F2
		public void SetOptionalSwitchNumeric(string id, string description, double defaultValue, double minRange, double maxRange)
		{
			this.DeclareNumericSwitch(id, description, true, defaultValue, minRange, maxRange);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002102 File Offset: 0x00000302
		public void SetOptionalSwitchNumeric(string id, string description, double defaultValue)
		{
			this.DeclareNumericSwitch(id, description, true, defaultValue, -2147483648.0, 2147483647.0);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002120 File Offset: 0x00000320
		public void SetRequiredSwitchNumeric(string id, string description, double minRange, double maxRange)
		{
			this.DeclareNumericSwitch(id, description, false, 0.0, minRange, maxRange);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002137 File Offset: 0x00000337
		public void SetRequiredSwitchNumeric(string id, string description)
		{
			this.DeclareNumericSwitch(id, description, false, 0.0, -2147483648.0, 2147483647.0);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000215D File Offset: 0x0000035D
		public void SetOptionalSwitchString(string id, string description, string defaultValue, params string[] possibleValues)
		{
			this.DeclareStringSwitch(id, description, true, defaultValue, true, possibleValues);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000216C File Offset: 0x0000036C
		public void SetOptionalSwitchString(string id, string description, string defaultValue, bool isPossibleValuesCaseSensitive, params string[] possibleValues)
		{
			this.DeclareStringSwitch(id, description, true, defaultValue, isPossibleValuesCaseSensitive, possibleValues);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000217C File Offset: 0x0000037C
		public void SetOptionalSwitchString(string id, string description)
		{
			this.DeclareStringSwitch(id, description, true, "", true, new string[0]);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002193 File Offset: 0x00000393
		public void SetRequiredSwitchString(string id, string description, params string[] possibleValues)
		{
			this.DeclareStringSwitch(id, description, false, "", true, possibleValues);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000021A5 File Offset: 0x000003A5
		public void SetRequiredSwitchString(string id, string description, bool isPossibleValuesCaseSensitive, params string[] possibleValues)
		{
			this.DeclareStringSwitch(id, description, false, "", isPossibleValuesCaseSensitive, possibleValues);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000021B8 File Offset: 0x000003B8
		public void SetRequiredSwitchString(string id, string description)
		{
			this.DeclareStringSwitch(id, description, false, "", true, new string[0]);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000021CF File Offset: 0x000003CF
		public void SetOptionalSwitchBoolean(string id, string description, bool defaultValue)
		{
			this.DeclareBooleanSwitch(id, description, true, defaultValue);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021DB File Offset: 0x000003DB
		public void SetOptionalParameterNumeric(string id, string description, double defaultValue, double minRange, double maxRange)
		{
			this.DeclareParam_Numeric(id, description, true, defaultValue, minRange, maxRange);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021EB File Offset: 0x000003EB
		public void SetOptionalParameterNumeric(string id, string description, double defaultValue)
		{
			this.DeclareParam_Numeric(id, description, true, defaultValue, -2147483648.0, 2147483647.0);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002209 File Offset: 0x00000409
		public void SetRequiredParameterNumeric(string id, string description, double minRange, double maxRange)
		{
			this.DeclareParam_Numeric(id, description, false, 0.0, minRange, maxRange);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002220 File Offset: 0x00000420
		public void SetRequiredParameterNumeric(string id, string description)
		{
			this.DeclareParam_Numeric(id, description, false, 0.0, -2147483648.0, 2147483647.0);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002246 File Offset: 0x00000446
		public void SetOptionalParameterString(string id, string description, string defaultValue, params string[] possibleValues)
		{
			this.DeclareStringParam(id, description, true, defaultValue, true, possibleValues);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002255 File Offset: 0x00000455
		public void SetOptionalParameterString(string id, string description, string defaultValue, bool isPossibleValuesCaseSensitive, params string[] possibleValues)
		{
			this.DeclareStringParam(id, description, true, defaultValue, isPossibleValuesCaseSensitive, possibleValues);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002265 File Offset: 0x00000465
		public void SetOptionalParameterString(string id, string description)
		{
			this.DeclareStringParam(id, description, true, "", true, new string[0]);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000227C File Offset: 0x0000047C
		public void SetRequiredParameterString(string id, string description, params string[] possibleValues)
		{
			this.DeclareStringParam(id, description, false, "", true, possibleValues);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000228E File Offset: 0x0000048E
		public void SetRequiredParameterString(string id, string description, bool isPossibleValuesCaseSensitive, params string[] possibleValues)
		{
			this.DeclareStringParam(id, description, false, "", isPossibleValuesCaseSensitive, possibleValues);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000022A1 File Offset: 0x000004A1
		public void SetRequiredParameterString(string id, string description)
		{
			this.DeclareStringParam(id, description, false, "", true, new string[0]);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000022B8 File Offset: 0x000004B8
		public bool ParseCommandLine()
		{
			this.SetFirstArgumentAsAppName();
			return this.ParseString(Environment.CommandLine);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000022CB File Offset: 0x000004CB
		public bool ParseString(string argumentsLine, bool isFirstArgTheAppName)
		{
			if (isFirstArgTheAppName)
			{
				this.SetFirstArgumentAsAppName();
			}
			return this.ParseString(argumentsLine);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000022E0 File Offset: 0x000004E0
		public bool ParseString(string argumentsLine)
		{
			if (argumentsLine == null)
			{
				throw new ArgumentNullException("argumentsLine");
			}
			if (this.m_parseSuccess)
			{
				throw new ParseFailedException("Cannot parse twice!");
			}
			this.SetOptionalSwitchBoolean("?", "Displays this usage string", false);
			int num = 0;
			argumentsLine = argumentsLine.TrimStart(new char[0]) + " ";
			Match match = new Regex(this.m_Syntax).Match(argumentsLine);
			while (match.Success)
			{
				string token = match.Result("${switchToken}");
				string text = match.Result("${idToken}");
				string delim = match.Result("${delimToken}");
				string text2 = match.Result("${valueToken}");
				text2 = text2.TrimEnd(new char[0]);
				if (text2.StartsWith("\"", StringComparison.CurrentCulture) && text2.EndsWith("\"", StringComparison.CurrentCulture))
				{
					text2 = text2.Substring(1, text2.Length - 2);
				}
				if (text.Length == 0)
				{
					if (!this.InputParam(text2, num++))
					{
						return false;
					}
				}
				else
				{
					if (text == "?")
					{
						this.m_lastError = "Usage Info requested";
						this.m_parseSuccess = false;
						return false;
					}
					if (!this.InputSwitch(token, text, delim, text2))
					{
						return false;
					}
				}
				match = match.NextMatch();
			}
			foreach (CommandLineParser.CArgument cargument in this.m_declaredSwitches)
			{
				if (!cargument.isOptional && !cargument.isAssigned)
				{
					this.m_lastError = "Required switch '" + cargument.Id + "' was not assigned a value";
					return false;
				}
			}
			foreach (CommandLineParser.CArgument cargument2 in this.m_declaredParams)
			{
				if (!cargument2.isOptional && !cargument2.isAssigned)
				{
					this.m_lastError = "Required parameter '" + cargument2.Id + "' was not assigned a value";
					return false;
				}
			}
			this.m_parseSuccess = this.IsGroupRulesKept();
			return this.m_parseSuccess;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000251C File Offset: 0x0000071C
		public object GetSwitch(string id)
		{
			if (!this.m_parseSuccess)
			{
				throw new ParseFailedException(this.LastError);
			}
			if (id == "RESERVED_ID_APPLICATION_NAME")
			{
				throw new ParseException("RESERVED_ID_APPLICATION_NAME is a reserved internal id and must not be used");
			}
			CommandLineParser.CArgument cargument = this.FindExactArg(id, this.m_declaredSwitches);
			if (cargument == null)
			{
				throw new NoSuchArgumentException("switch", id);
			}
			return cargument.GetValue();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002576 File Offset: 0x00000776
		public double GetSwitchAsNumeric(string id)
		{
			return (double)this.GetSwitch(id);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002584 File Offset: 0x00000784
		public string GetSwitchAsString(string id)
		{
			return (string)this.GetSwitch(id);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002592 File Offset: 0x00000792
		public bool GetSwitchAsBoolean(string id)
		{
			return (bool)this.GetSwitch(id);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000025A0 File Offset: 0x000007A0
		public bool IsAssignedSwitch(string id)
		{
			if (!this.m_parseSuccess)
			{
				throw new ParseFailedException(this.LastError);
			}
			if (id == "RESERVED_ID_APPLICATION_NAME")
			{
				throw new ParseException("RESERVED_ID_APPLICATION_NAME is a reserved internal id and must not be used");
			}
			CommandLineParser.CArgument cargument = this.FindExactArg(id, this.m_declaredSwitches);
			if (cargument == null)
			{
				throw new NoSuchArgumentException("switch", id);
			}
			return cargument.isAssigned;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000025FC File Offset: 0x000007FC
		public object GetParameter(string id)
		{
			if (!this.m_parseSuccess)
			{
				throw new ParseFailedException(this.LastError);
			}
			if (id == "RESERVED_ID_APPLICATION_NAME")
			{
				throw new ParseException("RESERVED_ID_APPLICATION_NAME is a reserved internal id and must not be used");
			}
			CommandLineParser.CArgument cargument = this.FindExactArg(id, this.m_declaredParams);
			if (cargument == null)
			{
				throw new NoSuchArgumentException("parameter", id);
			}
			return cargument.GetValue();
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002656 File Offset: 0x00000856
		public double GetParameterAsNumeric(string id)
		{
			return (double)this.GetParameter(id);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002664 File Offset: 0x00000864
		public string GetParameterAsString(string id)
		{
			return (string)this.GetParameter(id);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002674 File Offset: 0x00000874
		public bool IsAssignedParameter(string id)
		{
			if (!this.m_parseSuccess)
			{
				throw new ParseFailedException(this.LastError);
			}
			if (id == "RESERVED_ID_APPLICATION_NAME")
			{
				throw new ParseException("RESERVED_ID_APPLICATION_NAME is a reserved internal id and must not be used");
			}
			CommandLineParser.CArgument cargument = this.FindExactArg(id, this.m_declaredParams);
			if (cargument == null)
			{
				throw new NoSuchArgumentException("parameter", id);
			}
			return cargument.isAssigned;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000026D0 File Offset: 0x000008D0
		public object[] GetParameterList()
		{
			int num = this.IsFirstArgumentAppName() ? 1 : 0;
			if (this.m_declaredParams.Count == num)
			{
				return null;
			}
			object[] array = new object[this.m_declaredParams.Count - num];
			for (int i = num; i < this.m_declaredParams.Count; i++)
			{
				array[i - num] = this.m_declaredParams[i].GetValue();
			}
			return array;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000273C File Offset: 0x0000093C
		public Array SwitchesList()
		{
			Array array = Array.CreateInstance(typeof(object), this.m_declaredSwitches.Count, 2);
			for (int i = 0; i < this.m_declaredSwitches.Count; i++)
			{
				array.SetValue(this.m_declaredSwitches[i].Id, i, 1);
				array.SetValue(this.m_declaredSwitches[i].GetValue(), i, 0);
			}
			return array;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000027AE File Offset: 0x000009AE
		public void SetAlias(string alias, string treatedAs)
		{
			if (alias != treatedAs)
			{
				this.m_aliases[alias] = treatedAs;
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000027C8 File Offset: 0x000009C8
		public void DefineSwitchGroup(uint minAppear, uint maxAppear, params string[] ids)
		{
			if (ids == null)
			{
				throw new ArgumentNullException("ids");
			}
			if (ids.Length < 2 || maxAppear < minAppear || maxAppear == 0U)
			{
				throw new BadGroupException("A group must have at least two members");
			}
			if (minAppear == 0U && (ulong)maxAppear == (ulong)((long)ids.Length))
			{
				return;
			}
			if ((ulong)minAppear > (ulong)((long)ids.Length))
			{
				throw new BadGroupException(string.Format(CultureInfo.InvariantCulture, "You cannot have {0} appearance(s) in a group of {1} switch(es)!", new object[]
				{
					minAppear,
					ids.Length
				}));
			}
			foreach (string text in ids)
			{
				if (this.FindExactArg(text, this.m_declaredSwitches) == null)
				{
					throw new NoSuchArgumentException("switch", text);
				}
			}
			CommandLineParser.CArgGroups cargGroups = new CommandLineParser.CArgGroups(minAppear, maxAppear, ids);
			this.m_argGroups.Add(cargGroups);
			if (this.m_usageGroups.Length == 0)
			{
				this.m_usageGroups = "NOTES:" + Environment.NewLine;
			}
			this.m_usageGroups = this.m_usageGroups + " - " + cargGroups.RangeDescription() + Environment.NewLine;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000028C5 File Offset: 0x00000AC5
		public string UsageString()
		{
			return this.UsageString(new FileInfo(Environment.GetCommandLineArgs()[0]).Name);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000028E0 File Offset: 0x00000AE0
		public string UsageString(string appName)
		{
			string text = "";
			if (this.m_lastError.Length != 0)
			{
				text = ">> " + this.m_lastError + Environment.NewLine + Environment.NewLine;
			}
			return string.Concat(new string[]
			{
				text,
				"Usage: ",
				appName,
				this.m_usageCmdLine,
				Environment.NewLine,
				Environment.NewLine,
				this.m_usageArgs,
				Environment.NewLine,
				this.m_usageGroups,
				Environment.NewLine
			});
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002975 File Offset: 0x00000B75
		// (set) Token: 0x0600002B RID: 43 RVA: 0x0000297D File Offset: 0x00000B7D
		public bool CaseSensitive
		{
			get
			{
				return this.m_caseSensitive;
			}
			set
			{
				this.m_caseSensitive = value;
				this.CheckNotAmbiguous();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000298C File Offset: 0x00000B8C
		public string LastError
		{
			get
			{
				if (this.m_lastError.Length == 0)
				{
					return "There was no error";
				}
				return this.m_lastError;
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000029A8 File Offset: 0x00000BA8
		private void SetFirstArgumentAsAppName()
		{
			if (this.m_declaredParams.Count > 0 && this.m_declaredParams[0].Id == "RESERVED_ID_APPLICATION_NAME")
			{
				return;
			}
			this.CheckNotAmbiguous("RESERVED_ID_APPLICATION_NAME");
			CommandLineParser.CArgument item = new CommandLineParser.CStringArgument("RESERVED_ID_APPLICATION_NAME", "the application's name", false, "", true, new string[0]);
			this.m_declaredParams.Insert(0, item);
			this.m_iRequiredParams += 1U;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002A24 File Offset: 0x00000C24
		private void BuildRegularExpression()
		{
			this.m_Syntax = string.Concat(new string[]
			{
				"\\G((?<switchToken>[\\+\\-",
				this.m_switchChar.ToString(),
				"]{1})(?<idToken>[\\w|?]+)(?<delimToken>[",
				this.m_delimChar.ToString(),
				"]?))?(?<valueToken>(\"[^\"]*\"|\\S*)\\s+){1}"
			});
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002A84 File Offset: 0x00000C84
		private void DeclareNumericSwitch(string id, string description, bool fIsOptional, double defaultValue, double minRange, double maxRange)
		{
			if (id.Length == 0)
			{
				throw new EmptyArgumentDeclaredException();
			}
			this.CheckNotAmbiguous(id);
			CommandLineParser.CArgument cargument = new CommandLineParser.CNumericArgument(id, description, fIsOptional, defaultValue, minRange, maxRange);
			this.m_declaredSwitches.Add(cargument);
			this.AddUsageInfo(cargument, true, defaultValue);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002AD0 File Offset: 0x00000CD0
		private void DeclareStringSwitch(string id, string description, bool fIsOptional, string defaultValue, bool isPossibleValuesCaseSensitive, params string[] possibleValues)
		{
			if (id.Length == 0)
			{
				throw new EmptyArgumentDeclaredException();
			}
			this.CheckNotAmbiguous(id);
			CommandLineParser.CArgument cargument = new CommandLineParser.CStringArgument(id, description, fIsOptional, defaultValue, isPossibleValuesCaseSensitive, possibleValues);
			this.m_declaredSwitches.Add(cargument);
			this.AddUsageInfo(cargument, true, defaultValue);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002B18 File Offset: 0x00000D18
		private void DeclareBooleanSwitch(string id, string description, bool fIsOptional, bool defaultValue)
		{
			if (id.Length == 0)
			{
				throw new EmptyArgumentDeclaredException();
			}
			this.CheckNotAmbiguous(id);
			CommandLineParser.CArgument cargument = new CommandLineParser.CBooleanArgument(id, description, fIsOptional, defaultValue);
			this.m_declaredSwitches.Add(cargument);
			this.AddUsageInfo(cargument, true, defaultValue);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002B60 File Offset: 0x00000D60
		private void DeclareParam_Numeric(string id, string description, bool fIsOptional, double defaultValue, double minRange, double maxRange)
		{
			if (id.Length == 0)
			{
				throw new EmptyArgumentDeclaredException();
			}
			if (!fIsOptional && (long)this.m_declaredParams.Count > (long)((ulong)this.m_iRequiredParams))
			{
				throw new RequiredParameterAfterOptionalParameterException();
			}
			this.CheckNotAmbiguous(id);
			CommandLineParser.CArgument cargument = new CommandLineParser.CNumericArgument(id, description, fIsOptional, defaultValue, minRange, maxRange);
			if (!fIsOptional)
			{
				this.m_iRequiredParams += 1U;
			}
			this.m_declaredParams.Add(cargument);
			this.AddUsageInfo(cargument, false, defaultValue);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002BDC File Offset: 0x00000DDC
		private void DeclareStringParam(string id, string description, bool fIsOptional, string defaultValue, bool isPossibleValuesCaseSensitive, params string[] possibleValues)
		{
			if (id.Length == 0)
			{
				throw new EmptyArgumentDeclaredException();
			}
			if (!fIsOptional && (long)this.m_declaredParams.Count > (long)((ulong)this.m_iRequiredParams))
			{
				throw new RequiredParameterAfterOptionalParameterException();
			}
			this.CheckNotAmbiguous(id);
			CommandLineParser.CArgument cargument = new CommandLineParser.CStringArgument(id, description, fIsOptional, defaultValue, isPossibleValuesCaseSensitive, possibleValues);
			if (!fIsOptional)
			{
				this.m_iRequiredParams += 1U;
			}
			this.m_declaredParams.Add(cargument);
			this.AddUsageInfo(cargument, false, defaultValue);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002C54 File Offset: 0x00000E54
		private void AddUsageInfo(CommandLineParser.CArgument arg, bool isSwitch, object defVal)
		{
			this.m_usageCmdLine += (arg.isOptional ? " [" : " ");
			if (isSwitch)
			{
				if (arg.GetType() != typeof(CommandLineParser.CBooleanArgument))
				{
					this.m_usageCmdLine = string.Concat(new string[]
					{
						this.m_usageCmdLine,
						this.m_switchChar.ToString(),
						arg.Id,
						this.m_delimChar.ToString(),
						"x"
					});
				}
				else if (arg.Id == "?")
				{
					this.m_usageCmdLine = this.m_usageCmdLine + this.m_switchChar.ToString() + "?";
				}
				else
				{
					this.m_usageCmdLine = this.m_usageCmdLine + "[+|-]" + arg.Id;
				}
			}
			else
			{
				this.m_usageCmdLine += arg.Id;
			}
			this.m_usageCmdLine += (arg.isOptional ? "]" : "");
			string text = ((arg.Id == "?" || (isSwitch && arg.GetType() != typeof(CommandLineParser.CBooleanArgument))) ? this.m_switchChar.ToString() : "") + arg.Id;
			if (arg.isOptional)
			{
				text = "[" + text + "]";
			}
			text = "  " + text.PadRight(22, '·') + " ";
			text += arg.description;
			if (arg.Id != "?")
			{
				text = text + ". Values: " + arg.possibleValues();
				if (arg.isOptional)
				{
					text = text + "; default= " + defVal.ToString();
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (text.Length > 0)
			{
				if (text.Length <= 79)
				{
					this.m_usageArgs = this.m_usageArgs + text + Environment.NewLine;
					return;
				}
				int num = 79;
				while (num > 69 && text[num] != ' ')
				{
					num--;
				}
				if (num <= 69)
				{
					num = 79;
				}
				this.m_usageArgs = this.m_usageArgs + text.Substring(0, num) + Environment.NewLine;
				text = text.Substring(num).TrimStart(new char[0]);
				if (text.Length > 0)
				{
					stringBuilder.Append("".PadLeft(25, ' '));
					stringBuilder.Append(text);
					text = stringBuilder.ToString();
					stringBuilder.Remove(0, stringBuilder.Length);
				}
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002F0C File Offset: 0x0000110C
		private bool InputSwitch(string token, string ID, string delim, string val)
		{
			if (this.m_aliases.ContainsKey(ID))
			{
				ID = this.m_aliases[ID];
			}
			CommandLineParser.CArgument cargument = this.FindSimilarArg(ID, this.m_declaredSwitches);
			if (cargument == null)
			{
				return false;
			}
			if (cargument.GetType() == typeof(CommandLineParser.CBooleanArgument))
			{
				cargument.SetValue(token);
				if (delim.Length != 0 || val.Length != 0)
				{
					this.m_lastError = "A boolean switch cannot be followed by a delimiter. Use \"-booleanFlag\", not \"-booleanFlag" + this.m_delimChar.ToString() + "\"";
					return false;
				}
				return true;
			}
			else
			{
				if (delim.Length == 0)
				{
					this.m_lastError = string.Concat(new string[]
					{
						"you must use the delimiter '",
						this.m_delimChar.ToString(),
						"', e.g. \"",
						this.m_switchChar.ToString(),
						"arg",
						this.m_delimChar.ToString(),
						"x\""
					});
					return false;
				}
				if (cargument.SetValue(val))
				{
					return true;
				}
				this.m_lastError = string.Concat(new string[]
				{
					"Switch '",
					ID,
					"' cannot accept '",
					val,
					"' as a value"
				});
				return false;
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003058 File Offset: 0x00001258
		private bool InputParam(string val, int paramIndex)
		{
			if (2147483647 == paramIndex)
			{
				this.m_lastError = "paramIndex must be less than Int32.MaxValue";
				return false;
			}
			if (this.m_declaredParams.Count < paramIndex + 1)
			{
				this.m_lastError = "Command-line has too many parameters";
				return false;
			}
			CommandLineParser.CArgument cargument = this.m_declaredParams[paramIndex];
			if (cargument.SetValue(val))
			{
				return true;
			}
			this.m_lastError = "Parameter '" + cargument.Id + "' did not have a legal value";
			return false;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000030CC File Offset: 0x000012CC
		private CommandLineParser.CArgument FindExactArg(string argID, List<CommandLineParser.CArgument> list)
		{
			foreach (CommandLineParser.CArgument cargument in list)
			{
				if (string.Compare(cargument.Id, argID, (!this.CaseSensitive) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
				{
					return cargument;
				}
			}
			return null;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003134 File Offset: 0x00001334
		private CommandLineParser.CArgument FindSimilarArg(string argSubstringID, List<CommandLineParser.CArgument> list)
		{
			argSubstringID = (this.CaseSensitive ? argSubstringID : argSubstringID.ToUpper(CultureInfo.InvariantCulture));
			CommandLineParser.CArgument cargument = null;
			foreach (CommandLineParser.CArgument cargument2 in list)
			{
				string text = this.CaseSensitive ? cargument2.Id : cargument2.Id.ToUpper(CultureInfo.InvariantCulture);
				if (text.StartsWith(argSubstringID, StringComparison.CurrentCulture))
				{
					if (cargument != null)
					{
						string text2 = this.CaseSensitive ? cargument.Id : cargument.Id.ToUpper(CultureInfo.InvariantCulture);
						this.m_lastError = string.Concat(new string[]
						{
							"Ambiguous ID: '",
							argSubstringID,
							"' matches both '",
							text2,
							"' and '",
							text,
							"'"
						});
						return null;
					}
					cargument = cargument2;
				}
			}
			if (cargument == null)
			{
				this.m_lastError = "No such argument '" + argSubstringID + "'";
			}
			return cargument;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003250 File Offset: 0x00001450
		private void CheckNotAmbiguous()
		{
			this.CheckNotAmbiguous("");
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000325D File Offset: 0x0000145D
		private void CheckNotAmbiguous(string newId)
		{
			this.CheckNotAmbiguous(newId, this.m_declaredSwitches);
			this.CheckNotAmbiguous(newId, this.m_declaredParams);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000327C File Offset: 0x0000147C
		private void CheckNotAmbiguous(string newID, List<CommandLineParser.CArgument> argList)
		{
			foreach (CommandLineParser.CArgument cargument in argList)
			{
				if (string.Compare(cargument.Id, newID, (!this.CaseSensitive) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
				{
					throw new ArgumentAlreadyDeclaredException(cargument.Id);
				}
				if (newID.Length != 0 && (cargument.Id.StartsWith(newID, (!this.CaseSensitive) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) || newID.StartsWith(cargument.Id, (!this.CaseSensitive) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)))
				{
					throw new AmbiguousArgumentException(cargument.Id, newID);
				}
				foreach (CommandLineParser.CArgument cargument2 in argList)
				{
					if (!cargument.Equals(cargument2))
					{
						if (string.Compare(cargument.Id, cargument2.Id, (!this.CaseSensitive) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
						{
							throw new ArgumentAlreadyDeclaredException(cargument.Id);
						}
						if (cargument.Id.StartsWith(cargument2.Id, (!this.CaseSensitive) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) || cargument2.Id.StartsWith(cargument.Id, (!this.CaseSensitive) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
						{
							throw new AmbiguousArgumentException(cargument.Id, cargument2.Id);
						}
					}
				}
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003410 File Offset: 0x00001610
		private bool IsGroupRulesKept()
		{
			foreach (CommandLineParser.CArgGroups cargGroups in this.m_argGroups)
			{
				uint num = 0U;
				foreach (string argID in cargGroups.Args)
				{
					CommandLineParser.CArgument cargument = this.FindExactArg(argID, this.m_declaredSwitches);
					if (cargument != null && cargument.isAssigned)
					{
						num += 1U;
					}
				}
				if (!cargGroups.InRange(num))
				{
					this.m_lastError = cargGroups.RangeDescription();
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000034BC File Offset: 0x000016BC
		private bool IsFirstArgumentAppName()
		{
			return this.m_declaredParams.Count > 0 && this.m_declaredParams[0].Id == "RESERVED_ID_APPLICATION_NAME";
		}

		// Token: 0x04000001 RID: 1
		private const string c_applicationNameString = "RESERVED_ID_APPLICATION_NAME";

		// Token: 0x04000002 RID: 2
		private char m_switchChar = '/';

		// Token: 0x04000003 RID: 3
		private char m_delimChar = ':';

		// Token: 0x04000004 RID: 4
		private string m_Syntax = "";

		// Token: 0x04000005 RID: 5
		private List<CommandLineParser.CArgument> m_declaredSwitches = new List<CommandLineParser.CArgument>();

		// Token: 0x04000006 RID: 6
		private List<CommandLineParser.CArgument> m_declaredParams = new List<CommandLineParser.CArgument>();

		// Token: 0x04000007 RID: 7
		private uint m_iRequiredParams;

		// Token: 0x04000008 RID: 8
		private List<CommandLineParser.CArgGroups> m_argGroups = new List<CommandLineParser.CArgGroups>();

		// Token: 0x04000009 RID: 9
		private SortedList<string, string> m_aliases = new SortedList<string, string>();

		// Token: 0x0400000A RID: 10
		private bool m_caseSensitive;

		// Token: 0x0400000B RID: 11
		private string m_lastError = "";

		// Token: 0x0400000C RID: 12
		private string m_usageCmdLine = "";

		// Token: 0x0400000D RID: 13
		private string m_usageArgs = "";

		// Token: 0x0400000E RID: 14
		private string m_usageGroups = "";

		// Token: 0x0400000F RID: 15
		private bool m_parseSuccess;

		// Token: 0x04000010 RID: 16
		private const char DEFAULT_SWITCH = '/';

		// Token: 0x04000011 RID: 17
		private const char DEFAULT_DELIM = ':';

		// Token: 0x04000012 RID: 18
		private const string SWITCH_TOKEN = "switchToken";

		// Token: 0x04000013 RID: 19
		private const string ID_TOKEN = "idToken";

		// Token: 0x04000014 RID: 20
		private const string DELIM_TOKEN = "delimToken";

		// Token: 0x04000015 RID: 21
		private const string VALUE_TOKEN = "valueToken";

		// Token: 0x04000016 RID: 22
		private const int USAGE_COL1 = 25;

		// Token: 0x04000017 RID: 23
		private const int USAGE_WIDTH = 79;

		// Token: 0x02000058 RID: 88
		internal abstract class CArgument
		{
			// Token: 0x06000299 RID: 665 RVA: 0x0000BC98 File Offset: 0x00009E98
			protected CArgument(string id, string desc, bool fIsOptional)
			{
				this.m_id = id;
				this.m_description = desc;
				this.m_fIsOptional = fIsOptional;
			}

			// Token: 0x17000063 RID: 99
			// (get) Token: 0x0600029A RID: 666 RVA: 0x0000BCE8 File Offset: 0x00009EE8
			public string Id
			{
				get
				{
					return this.m_id;
				}
			}

			// Token: 0x0600029B RID: 667 RVA: 0x0000BCF0 File Offset: 0x00009EF0
			public object GetValue()
			{
				return this.m_val;
			}

			// Token: 0x0600029C RID: 668
			public abstract bool SetValue(string val);

			// Token: 0x0600029D RID: 669
			public abstract string possibleValues();

			// Token: 0x17000064 RID: 100
			// (get) Token: 0x0600029E RID: 670 RVA: 0x0000BCF8 File Offset: 0x00009EF8
			public string description
			{
				get
				{
					if (this.m_description.Length == 0)
					{
						return this.m_id;
					}
					return this.m_description;
				}
			}

			// Token: 0x17000065 RID: 101
			// (get) Token: 0x0600029F RID: 671 RVA: 0x0000BD14 File Offset: 0x00009F14
			public bool isOptional
			{
				get
				{
					return this.m_fIsOptional;
				}
			}

			// Token: 0x17000066 RID: 102
			// (get) Token: 0x060002A0 RID: 672 RVA: 0x0000BD1C File Offset: 0x00009F1C
			public bool isAssigned
			{
				get
				{
					return this.m_fIsAssigned;
				}
			}

			// Token: 0x04000130 RID: 304
			protected object m_val = "";

			// Token: 0x04000131 RID: 305
			protected bool m_fIsAssigned;

			// Token: 0x04000132 RID: 306
			private string m_id = "";

			// Token: 0x04000133 RID: 307
			private string m_description = "";

			// Token: 0x04000134 RID: 308
			private bool m_fIsOptional = true;
		}

		// Token: 0x02000059 RID: 89
		internal class CNumericArgument : CommandLineParser.CArgument
		{
			// Token: 0x060002A1 RID: 673 RVA: 0x0000BD24 File Offset: 0x00009F24
			public CNumericArgument(string id, string desc, bool fIsOptional, double defVal, double minRange, double maxRange) : base(id, desc, fIsOptional)
			{
				this.m_val = defVal;
				this.m_minRange = minRange;
				this.m_maxRange = maxRange;
			}

			// Token: 0x060002A2 RID: 674 RVA: 0x0000BD78 File Offset: 0x00009F78
			public override bool SetValue(string val)
			{
				bool isAssigned = base.isAssigned;
				this.m_fIsAssigned = true;
				try
				{
					if (val.ToLowerInvariant().StartsWith("0x", StringComparison.CurrentCulture))
					{
						this.m_val = (double)int.Parse(val.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					}
					else
					{
						this.m_val = double.Parse(val, NumberStyles.Any, CultureInfo.InvariantCulture);
					}
				}
				catch (ArgumentNullException)
				{
					return false;
				}
				catch (FormatException)
				{
					return false;
				}
				catch (OverflowException)
				{
					return false;
				}
				return (double)this.m_val >= this.m_minRange && (double)this.m_val <= this.m_maxRange;
			}

			// Token: 0x060002A3 RID: 675 RVA: 0x0000BE4C File Offset: 0x0000A04C
			public override string possibleValues()
			{
				return string.Concat(new object[]
				{
					"between ",
					this.m_minRange,
					" and ",
					this.m_maxRange
				});
			}

			// Token: 0x04000135 RID: 309
			private double m_minRange = double.MinValue;

			// Token: 0x04000136 RID: 310
			private double m_maxRange = double.MaxValue;
		}

		// Token: 0x0200005A RID: 90
		internal class CStringArgument : CommandLineParser.CArgument
		{
			// Token: 0x060002A4 RID: 676 RVA: 0x0000BE85 File Offset: 0x0000A085
			public CStringArgument(string id, string desc, bool fIsOptional, string defVal, bool isPossibleValuesCaseSensitive, params string[] possibleValues) : base(id, desc, fIsOptional)
			{
				this.m_possibleVals = possibleValues;
				this.m_val = defVal;
				this.m_fIsPossibleValsCaseSensitive = isPossibleValuesCaseSensitive;
			}

			// Token: 0x060002A5 RID: 677 RVA: 0x0000BEC4 File Offset: 0x0000A0C4
			public override bool SetValue(string val)
			{
				bool isAssigned = base.isAssigned;
				this.m_fIsAssigned = true;
				this.m_val = val;
				if (this.m_possibleVals.Length == 0)
				{
					return true;
				}
				foreach (string text in this.m_possibleVals)
				{
					if ((string)this.m_val == text || (!this.m_fIsPossibleValsCaseSensitive && string.Compare((string)this.m_val, text, StringComparison.OrdinalIgnoreCase) == 0))
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x060002A6 RID: 678 RVA: 0x0000BF3C File Offset: 0x0000A13C
			public override string possibleValues()
			{
				if (this.m_possibleVals.Length == 0)
				{
					return "free text";
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("{");
				stringBuilder.Append(this.m_fIsPossibleValsCaseSensitive ? this.m_possibleVals[0] : this.m_possibleVals[0].ToLowerInvariant());
				for (int i = 1; i < this.m_possibleVals.Length; i++)
				{
					stringBuilder.Append("|");
					stringBuilder.Append(this.m_fIsPossibleValsCaseSensitive ? this.m_possibleVals[i] : this.m_possibleVals[i].ToLowerInvariant());
				}
				stringBuilder.Append("}");
				return stringBuilder.ToString();
			}

			// Token: 0x04000137 RID: 311
			private string[] m_possibleVals = new string[]
			{
				""
			};

			// Token: 0x04000138 RID: 312
			private bool m_fIsPossibleValsCaseSensitive = true;
		}

		// Token: 0x0200005B RID: 91
		internal class CBooleanArgument : CommandLineParser.CArgument
		{
			// Token: 0x060002A7 RID: 679 RVA: 0x0000BFE7 File Offset: 0x0000A1E7
			public CBooleanArgument(string id, string desc, bool fIsOptional, bool defVal) : base(id, desc, fIsOptional)
			{
				this.m_val = defVal;
			}

			// Token: 0x060002A8 RID: 680 RVA: 0x0000BFFF File Offset: 0x0000A1FF
			public override bool SetValue(string token)
			{
				bool isAssigned = base.isAssigned;
				this.m_fIsAssigned = true;
				this.m_val = (token != "-");
				return true;
			}

			// Token: 0x060002A9 RID: 681 RVA: 0x0000C026 File Offset: 0x0000A226
			public override string possibleValues()
			{
				return "precede by [+] or [-]";
			}
		}

		// Token: 0x0200005C RID: 92
		internal class CArgGroups
		{
			// Token: 0x060002AA RID: 682 RVA: 0x0000C02D File Offset: 0x0000A22D
			public CArgGroups(uint min, uint max, params string[] args)
			{
				this.m_minAppear = min;
				this.m_maxAppear = max;
				this.m_args = args;
			}

			// Token: 0x060002AB RID: 683 RVA: 0x0000C04A File Offset: 0x0000A24A
			public bool InRange(uint num)
			{
				return num >= this.m_minAppear && num <= this.m_maxAppear;
			}

			// Token: 0x17000067 RID: 103
			// (get) Token: 0x060002AC RID: 684 RVA: 0x0000C063 File Offset: 0x0000A263
			public string[] Args
			{
				get
				{
					return this.m_args;
				}
			}

			// Token: 0x060002AD RID: 685 RVA: 0x0000C06C File Offset: 0x0000A26C
			public string ArgList()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("{");
				foreach (string value in this.Args)
				{
					stringBuilder.Append(",");
					stringBuilder.Append(value);
				}
				return stringBuilder.ToString().Replace("{,", "{") + "}";
			}

			// Token: 0x060002AE RID: 686 RVA: 0x0000C0D8 File Offset: 0x0000A2D8
			public string RangeDescription()
			{
				if (this.m_minAppear == 1U && this.m_maxAppear == 1U)
				{
					return "one of the switches " + this.ArgList() + " must be used exclusively";
				}
				if (this.m_minAppear == 1U && (ulong)this.m_maxAppear == (ulong)((long)this.Args.Length))
				{
					return "one or more of the switches " + this.ArgList() + " must be used";
				}
				if (this.m_minAppear == 1U && this.m_maxAppear > 1U)
				{
					return string.Concat(new object[]
					{
						"one (but not more than ",
						this.m_maxAppear,
						") of the switches ",
						this.ArgList(),
						" must be used"
					});
				}
				if (this.m_minAppear == 0U && this.m_maxAppear == 1U)
				{
					return "only one of the switches " + this.ArgList() + " can be used";
				}
				if (this.m_minAppear == 0U && this.m_maxAppear > 1U)
				{
					return string.Concat(new object[]
					{
						"only ",
						this.m_maxAppear,
						" of the switches ",
						this.ArgList(),
						" can be used"
					});
				}
				return string.Concat(new object[]
				{
					"between ",
					this.m_minAppear,
					" and ",
					this.m_maxAppear,
					" of the switches ",
					this.ArgList(),
					" must be used"
				});
			}

			// Token: 0x04000139 RID: 313
			public uint m_minAppear;

			// Token: 0x0400013A RID: 314
			public uint m_maxAppear;

			// Token: 0x0400013B RID: 315
			private string[] m_args;
		}
	}
}
