using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000006 RID: 6
	public class MacroResolver : MacroStack, IMacroResolver
	{
		// Token: 0x06000017 RID: 23 RVA: 0x0000223F File Offset: 0x0000043F
		public void BeginLocal()
		{
			this._dictionaries.Push(new Dictionary<string, Macro>(StringComparer.OrdinalIgnoreCase));
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002256 File Offset: 0x00000456
		public void Register(string name, string value)
		{
			this.Register(name, value, false);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002261 File Offset: 0x00000461
		public void Register(string name, object value, MacroDelegate del)
		{
			this.Register(name, value, del, false);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002270 File Offset: 0x00000470
		public void Register(IEnumerable<KeyValuePair<string, Macro>> values)
		{
			if (values != null)
			{
				foreach (KeyValuePair<string, Macro> keyValuePair in values)
				{
					this.Register(keyValuePair.Key, keyValuePair.Value.Value, keyValuePair.Value.Delegate, false);
				}
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000022DC File Offset: 0x000004DC
		public void EndLocal()
		{
			if (this._dictionaries.Count > 0)
			{
				this._dictionaries.Pop();
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000022F8 File Offset: 0x000004F8
		public string Resolve(string input)
		{
			return this.Resolve(input, MacroResolveOptions.ErrorOnUnknownMacro);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002304 File Offset: 0x00000504
		public string Resolve(string input, MacroResolveOptions option)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			int i = 0;
			string text = input;
			while (i < 99)
			{
				MacroResolver.MatchEvaluator matchEvaluator = new MacroResolver.MatchEvaluator(this, option);
				text = this._varReferencePattern.Replace(text, new System.Text.RegularExpressions.MatchEvaluator(matchEvaluator.Evaluate));
				if (matchEvaluator.MatchCount == 0)
				{
					return text;
				}
				i++;
			}
			throw new PkgGenException("Too many recurrence maros", new object[0]);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002365 File Offset: 0x00000565
		public MacroResolver()
		{
			this.BeginLocal();
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000023A0 File Offset: 0x000005A0
		public MacroResolver(MacroResolver parent)
		{
			foreach (Dictionary<string, Macro> item in parent._dictionaries)
			{
				this._dictionaries.Push(item);
			}
			this.BeginLocal();
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002430 File Offset: 0x00000630
		public void Register(string name, string value, bool allowOverride)
		{
			this.Register(name, value, (object x) => x.ToString(), allowOverride);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000245C File Offset: 0x0000065C
		public void Register(string name, object value, MacroDelegate del, bool allowOverride)
		{
			if (!this._varNamePattern.IsMatch(name))
			{
				throw new PkgGenException("Incorrect macro id: '{0}', expecting a string matching regular expression pattern '{1}'", new object[]
				{
					name,
					this._varNamePattern
				});
			}
			if (!allowOverride)
			{
				string value2 = base.GetValue(name);
				if (value2 != null)
				{
					throw new PkgGenException("Redefining macro is not allowed, id:'{0}', current value:'{1}', new value:'{2}'", new object[]
					{
						name,
						value2,
						value
					});
				}
			}
			this._dictionaries.Peek()[name] = new Macro(name, value, del);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000024DA File Offset: 0x000006DA
		public bool Unregister(string name)
		{
			return !string.IsNullOrEmpty(name) && base.RemoveName(name);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000024F0 File Offset: 0x000006F0
		public void Load(XmlReader macroDefinitionReader)
		{
			try
			{
				MacroTable macroTable = (MacroTable)new XmlSerializer(typeof(MacroTable)).Deserialize(macroDefinitionReader);
				if (macroTable != null)
				{
					this.Register(macroTable.Values);
				}
			}
			catch (InvalidOperationException ex)
			{
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				throw;
			}
		}

		// Token: 0x04000006 RID: 6
		private const int _maxResolveCount = 99;

		// Token: 0x04000007 RID: 7
		private Regex _varReferencePattern = new Regex("\\$\\((?<name>.*?)\\)");

		// Token: 0x04000008 RID: 8
		private Regex _varNamePattern = new Regex("^[A-Za-z.0-9_{-][A-Za-z.0-9_+{}-]*$");

		// Token: 0x04000009 RID: 9
		private MacroStack _macroStack = new MacroStack();

		// Token: 0x0200007E RID: 126
		private class MatchEvaluator
		{
			// Token: 0x170000C0 RID: 192
			// (get) Token: 0x060002CE RID: 718 RVA: 0x0000A75C File Offset: 0x0000895C
			// (set) Token: 0x060002CF RID: 719 RVA: 0x0000A764 File Offset: 0x00008964
			public int MatchCount { get; private set; }

			// Token: 0x060002D0 RID: 720 RVA: 0x0000A770 File Offset: 0x00008970
			public string Evaluate(Match match)
			{
				string value = this._macroStack.GetValue(match.Groups["name"].Value);
				if (value == null)
				{
					if (this._option != MacroResolveOptions.SkipOnUnknownMacro)
					{
						throw new PkgGenException("Undefined variable {0}", new object[]
						{
							match.Groups["name"].Value
						});
					}
					value = match.Groups[0].Value;
				}
				else
				{
					int matchCount = this.MatchCount;
					this.MatchCount = matchCount + 1;
				}
				return value;
			}

			// Token: 0x060002D1 RID: 721 RVA: 0x0000A7F7 File Offset: 0x000089F7
			public MatchEvaluator(MacroStack macroStack, MacroResolveOptions option)
			{
				this._macroStack = macroStack;
				this._option = option;
			}

			// Token: 0x040001DC RID: 476
			private MacroResolveOptions _option = MacroResolveOptions.ErrorOnUnknownMacro;

			// Token: 0x040001DD RID: 477
			private MacroStack _macroStack;
		}
	}
}
