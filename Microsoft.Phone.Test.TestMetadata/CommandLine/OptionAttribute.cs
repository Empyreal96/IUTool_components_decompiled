using System;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000024 RID: 36
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
	public sealed class OptionAttribute : Attribute
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x00004B76 File Offset: 0x00002D76
		public OptionAttribute(string name, OptionValueType valueType)
		{
			this.Name = name;
			this.ValueType = valueType;
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00004B8E File Offset: 0x00002D8E
		public string Name { get; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00004B96 File Offset: 0x00002D96
		public OptionValueType ValueType { get; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00004B9E File Offset: 0x00002D9E
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x00004BA6 File Offset: 0x00002DA6
		public bool IsMultipleValue { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00004BAF File Offset: 0x00002DAF
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x00004BB7 File Offset: 0x00002DB7
		public string DefaultValue { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00004BC0 File Offset: 0x00002DC0
		// (set) Token: 0x060000D8 RID: 216 RVA: 0x00004BC8 File Offset: 0x00002DC8
		public bool IsFinalOption { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00004BD1 File Offset: 0x00002DD1
		// (set) Token: 0x060000DA RID: 218 RVA: 0x00004BD9 File Offset: 0x00002DD9
		public string Description { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00004BE2 File Offset: 0x00002DE2
		// (set) Token: 0x060000DC RID: 220 RVA: 0x00004BEA File Offset: 0x00002DEA
		public Type CollectionType { get; set; }
	}
}
