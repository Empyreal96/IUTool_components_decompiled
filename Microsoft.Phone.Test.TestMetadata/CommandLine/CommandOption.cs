using System;
using System.Collections.Specialized;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x0200001D RID: 29
	public class CommandOption
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x00003F6C File Offset: 0x0000216C
		public CommandOption(string name)
		{
			this.Name = name;
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00003F88 File Offset: 0x00002188
		public string Name { get; }

		// Token: 0x060000A3 RID: 163 RVA: 0x00003F90 File Offset: 0x00002190
		public void Add(string optionValue)
		{
			this.Values.Add(optionValue);
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00003FA0 File Offset: 0x000021A0
		public string Value
		{
			get
			{
				bool flag = this.Values.Count > 0;
				string result;
				if (flag)
				{
					result = this.Values[0];
				}
				else
				{
					result = string.Empty;
				}
				return result;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00003FD9 File Offset: 0x000021D9
		public StringCollection Values { get; } = new StringCollection();

		// Token: 0x1700002A RID: 42
		public string this[int index]
		{
			get
			{
				return this.Values[index];
			}
		}

		// Token: 0x0400009D RID: 157
		public const string NoName = "";
	}
}
