using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000044 RID: 68
	public class MacroResolver : MacroStack, IMacroResolver
	{
		// Token: 0x06000141 RID: 321 RVA: 0x00008787 File Offset: 0x00006987
		public void BeginLocal()
		{
			this._dictionaries.Push(new Dictionary<string, Macro>(StringComparer.OrdinalIgnoreCase));
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000879E File Offset: 0x0000699E
		public void Register(string name, string value)
		{
			this.Register(name, value, false);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x000087A9 File Offset: 0x000069A9
		public void Register(string name, object value, MacroDelegate del)
		{
			this.Register(name, value, del, false);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x000087B8 File Offset: 0x000069B8
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public void Register(IEnumerable<KeyValuePair<string, Macro>> values, bool allowOverride = false)
		{
			if (values != null)
			{
				foreach (KeyValuePair<string, Macro> keyValuePair in values)
				{
					this.Register(keyValuePair.Key, keyValuePair.Value.Value, keyValuePair.Value.Delegate, allowOverride);
				}
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00008824 File Offset: 0x00006A24
		public Dictionary<string, Macro> GetMacroTable()
		{
			Dictionary<string, Macro> dictionary = new Dictionary<string, Macro>();
			foreach (KeyValuePair<string, Macro> keyValuePair in this._dictionaries.Peek())
			{
				dictionary[keyValuePair.Key] = keyValuePair.Value;
			}
			return dictionary;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00008890 File Offset: 0x00006A90
		public void EndLocal()
		{
			if (this._dictionaries.Count > 0)
			{
				this._dictionaries.Pop();
			}
		}

		// Token: 0x06000147 RID: 327 RVA: 0x000088AC File Offset: 0x00006AAC
		public string Resolve(string input)
		{
			return this.Resolve(input, MacroResolveOptions.ValidWhenEqual);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x000088B6 File Offset: 0x00006AB6
		public bool PassThrough(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return input.StartsWith("$", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000088D4 File Offset: 0x00006AD4
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
				string value = text;
				text = this._varReferencePattern.Replace(text, new System.Text.RegularExpressions.MatchEvaluator(matchEvaluator.Evaluate));
				if (option == MacroResolveOptions.ValidWhenEqual && text.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return text;
				}
				if (matchEvaluator.MatchCount == 0)
				{
					return text;
				}
				i++;
			}
			throw new PkgGenException("Too many recurrence maros");
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00008941 File Offset: 0x00006B41
		public MacroResolver()
		{
			this.BeginLocal();
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000897C File Offset: 0x00006B7C
		public MacroResolver(MacroResolver parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			foreach (Dictionary<string, Macro> item in parent._dictionaries)
			{
				this._dictionaries.Push(item);
			}
			this.BeginLocal();
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00008A1C File Offset: 0x00006C1C
		public void Register(string name, string value, bool allowOverride)
		{
			this.Register(name, value, (object x) => x.ToString(), allowOverride);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00008A48 File Offset: 0x00006C48
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

		// Token: 0x0600014E RID: 334 RVA: 0x00008AC6 File Offset: 0x00006CC6
		public bool Unregister(string name)
		{
			return !string.IsNullOrEmpty(name) && base.RemoveName(name);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00008ADC File Offset: 0x00006CDC
		public void Import(string File)
		{
			if (!LongPathFile.Exists(File))
			{
				return;
			}
			XmlReader macroDefinitionReader = XmlReader.Create(new FileStream(File, FileMode.Open, FileAccess.Read));
			this.Load(macroDefinitionReader);
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00008B08 File Offset: 0x00006D08
		public void Load(XmlReader macroDefinitionReader)
		{
			try
			{
				MacroTable macroTable = (MacroTable)new XmlSerializer(typeof(MacroTable)).Deserialize(macroDefinitionReader);
				if (macroTable != null)
				{
					this.Register(macroTable.Values, false);
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

		// Token: 0x04000083 RID: 131
		private const int _maxResolveCount = 99;

		// Token: 0x04000084 RID: 132
		private Regex _varReferencePattern = new Regex("\\$\\((?<name>.*?)\\)");

		// Token: 0x04000085 RID: 133
		private Regex _varNamePattern = new Regex("^[A-Za-z.0-9_{-][A-Za-z.0-9_+{}-]*$");

		// Token: 0x04000086 RID: 134
		private MacroStack _macroStack = new MacroStack();

		// Token: 0x02000068 RID: 104
		private class MatchEvaluator
		{
			// Token: 0x1700009D RID: 157
			// (get) Token: 0x06000230 RID: 560 RVA: 0x0000B448 File Offset: 0x00009648
			// (set) Token: 0x06000231 RID: 561 RVA: 0x0000B450 File Offset: 0x00009650
			public int MatchCount { get; private set; }

			// Token: 0x06000232 RID: 562 RVA: 0x0000B45C File Offset: 0x0000965C
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

			// Token: 0x06000233 RID: 563 RVA: 0x0000B4E3 File Offset: 0x000096E3
			public MatchEvaluator(MacroStack macroStack, MacroResolveOptions option)
			{
				this._macroStack = macroStack;
				this._option = option;
			}

			// Token: 0x04000168 RID: 360
			private MacroResolveOptions _option = MacroResolveOptions.ValidWhenEqual;

			// Token: 0x04000169 RID: 361
			private MacroStack _macroStack;
		}
	}
}
