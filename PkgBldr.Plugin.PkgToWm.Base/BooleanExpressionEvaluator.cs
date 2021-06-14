using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000020 RID: 32
	public class BooleanExpressionEvaluator
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00006464 File Offset: 0x00004664
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00006442 File Offset: 0x00004642
		public string and
		{
			get
			{
				return this.m_and;
			}
			set
			{
				this.m_and = value;
				if (this.m_and != null)
				{
					this.m_and = this.m_and.ToLowerInvariant();
				}
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000068 RID: 104 RVA: 0x0000648E File Offset: 0x0000468E
		// (set) Token: 0x06000067 RID: 103 RVA: 0x0000646C File Offset: 0x0000466C
		public string or
		{
			get
			{
				return this.m_or;
			}
			set
			{
				this.m_or = value;
				if (this.m_or != null)
				{
					this.m_or = this.m_or.ToLowerInvariant();
				}
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600006A RID: 106 RVA: 0x000064B8 File Offset: 0x000046B8
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00006496 File Offset: 0x00004696
		public string not
		{
			get
			{
				return this.m_not;
			}
			set
			{
				this.m_not = value;
				if (this.m_not != null)
				{
					this.m_not = this.m_not.ToLowerInvariant();
				}
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000064C0 File Offset: 0x000046C0
		public BooleanExpressionEvaluator()
		{
			this.and = "and";
			this.or = "or";
			this.not = "not";
			this.variablePattern = "[a-zA-Z][a-zA-Z0-9_\\-]+";
			this.expressionPattern = "^(.+)$";
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00006515 File Offset: 0x00004715
		// (set) Token: 0x0600006D RID: 109 RVA: 0x0000651D File Offset: 0x0000471D
		public string variablePattern { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00006526 File Offset: 0x00004726
		// (set) Token: 0x0600006F RID: 111 RVA: 0x0000652E File Offset: 0x0000472E
		public string expressionPattern { get; set; }

		// Token: 0x06000070 RID: 112 RVA: 0x00006538 File Offset: 0x00004738
		private void setVariables(string expression)
		{
			foreach (object obj in new Regex(this.variablePattern).Matches(expression))
			{
				string text = ((Match)obj).Value.ToLowerInvariant();
				if (!(text == this.and) && !(text == this.or) && !(text == this.not) && !(text == "(") && !(text == ")") && !(text == "=") && !this.m_variables.ContainsKey(text))
				{
					this.m_variables.Add(text, false);
				}
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00006614 File Offset: 0x00004814
		public string Evaluate(string expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (!new Regex(this.expressionPattern).Match(expression).Success)
			{
				return null;
			}
			string text = expression.ToLowerInvariant();
			this.setVariables(text);
			if (this.and != null)
			{
				text = text.Replace(this.and, " and ");
			}
			if (this.or != null)
			{
				text = text.Replace(this.or, " or ");
			}
			if (this.not != null)
			{
				text = text.Replace(this.not, " not ");
			}
			foreach (KeyValuePair<string, bool> keyValuePair in this.m_variables.Reverse<KeyValuePair<string, bool>>())
			{
				text = Regex.Replace(text, keyValuePair.Key ?? "", keyValuePair.Value.ToString());
			}
			DataTable dataTable = new DataTable();
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("", typeof(bool));
			dataTable.Columns[0].Expression = text;
			DataRow dataRow = dataTable.NewRow();
			dataTable.Rows.Add(dataRow);
			if ((bool)dataRow[0])
			{
				return "true";
			}
			return "false";
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00006774 File Offset: 0x00004974
		public void Set(string var, bool state)
		{
			if (var == null)
			{
				throw new ArgumentNullException("var");
			}
			var = var.ToLowerInvariant();
			if (!this.m_variables.ContainsKey(var))
			{
				this.m_variables.Add(var, state);
				return;
			}
			this.m_variables[var] = state;
		}

		// Token: 0x0400000E RID: 14
		private string m_and;

		// Token: 0x0400000F RID: 15
		private string m_or;

		// Token: 0x04000010 RID: 16
		private string m_not;

		// Token: 0x04000013 RID: 19
		private SortedDictionary<string, bool> m_variables = new SortedDictionary<string, bool>();
	}
}
