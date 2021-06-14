using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000017 RID: 23
	public class SummaryLogEntry
	{
		// Token: 0x06000087 RID: 135 RVA: 0x00003B29 File Offset: 0x00001D29
		public SummaryLogEntry(string line)
		{
			this.attributes = new Dictionary<string, string>();
			this.ParseLine(line);
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00003B43 File Offset: 0x00001D43
		// (set) Token: 0x06000089 RID: 137 RVA: 0x00003B4B File Offset: 0x00001D4B
		public DateTime TimeStamp { get; private set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00003B54 File Offset: 0x00001D54
		// (set) Token: 0x0600008B RID: 139 RVA: 0x00003B5C File Offset: 0x00001D5C
		public int TickCount { get; private set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00003B65 File Offset: 0x00001D65
		// (set) Token: 0x0600008D RID: 141 RVA: 0x00003B6D File Offset: 0x00001D6D
		public string Type { get; private set; }

		// Token: 0x0600008E RID: 142 RVA: 0x00003B76 File Offset: 0x00001D76
		public string GetAttribute(string attributeName)
		{
			if (!this.attributes.ContainsKey(attributeName))
			{
				throw new MtbfLogParserException(attributeName + " is not found in the summary log line.");
			}
			return this.attributes[attributeName];
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00003BA4 File Offset: 0x00001DA4
		private void ParseLine(string line)
		{
			string[] array = Regex.Split(line, ";(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)|,(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
			if (array.Length < 4)
			{
				throw new MtbfLogParserException("Too few tokens in the summary log line: " + line);
			}
			DateTime timeStamp;
			if (!DateTime.TryParse(array[0].Trim(), out timeStamp))
			{
				throw new MtbfLogParserException(string.Format(CultureInfo.InvariantCulture, "Invalid time stamp '{0}' in the summary log line: {1}", new object[]
				{
					array[0],
					line
				}));
			}
			this.TimeStamp = timeStamp;
			int tickCount;
			if (!int.TryParse(array[1].Trim(), out tickCount))
			{
				throw new MtbfLogParserException(string.Format(CultureInfo.InvariantCulture, "Invalid tick count '{0}' in the summary log line: {1}", new object[]
				{
					array[1],
					line
				}));
			}
			this.TickCount = tickCount;
			this.Type = array[2].Trim();
			for (int i = 3; i < array.Length; i++)
			{
				int num = array[i].IndexOf('=');
				if (num < 0)
				{
					throw new MtbfLogParserException(string.Format(CultureInfo.InvariantCulture, "Invalid token '{0}' in the summary log line: {1}. Required delimiter '=' not found.", new object[]
					{
						array[i],
						line
					}));
				}
				if (num >= array[i].Length - 1)
				{
					throw new MtbfLogParserException(string.Format(CultureInfo.InvariantCulture, "Invalid token '{0}' in the summary log line: {1}. Token is not in the format '<key>=<value>'", new object[]
					{
						array[i],
						line
					}));
				}
				string text = array[i].Remove(num).Trim();
				string value = array[i].Substring(num + 1).Trim(new char[]
				{
					' ',
					'"'
				});
				if (string.IsNullOrEmpty(text))
				{
					throw new MtbfLogParserException(string.Format(CultureInfo.InvariantCulture, "Invalid token '{0}' in the summary log line: {1}. Token must contain a non-empty key followed by '='", new object[]
					{
						array[i],
						line
					}));
				}
				this.attributes.Add(text, value);
			}
		}

		// Token: 0x04000037 RID: 55
		private const int TokenIndexTimeStamp = 0;

		// Token: 0x04000038 RID: 56
		private const int TokenTickCount = 1;

		// Token: 0x04000039 RID: 57
		private const int TokenIndexEntryType = 2;

		// Token: 0x0400003A RID: 58
		private const int MinTokensCount = 4;

		// Token: 0x0400003B RID: 59
		private Dictionary<string, string> attributes;
	}
}
