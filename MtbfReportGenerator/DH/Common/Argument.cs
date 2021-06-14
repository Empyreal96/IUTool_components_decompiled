using System;

namespace DH.Common
{
	// Token: 0x02000019 RID: 25
	public class Argument
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00003E13 File Offset: 0x00002013
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00003E1B File Offset: 0x0000201B
		public string Name { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00003E24 File Offset: 0x00002024
		// (set) Token: 0x0600009F RID: 159 RVA: 0x00003E2C File Offset: 0x0000202C
		public string DefaultValue { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00003E35 File Offset: 0x00002035
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x00003E3D File Offset: 0x0000203D
		public bool IsOptional { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00003E46 File Offset: 0x00002046
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x00003E4E File Offset: 0x0000204E
		public bool RequiresValue { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00003E57 File Offset: 0x00002057
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x00003E5F File Offset: 0x0000205F
		public string Description { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x00003E68 File Offset: 0x00002068
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x00003E70 File Offset: 0x00002070
		public string Value { get; set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00003E79 File Offset: 0x00002079
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00003E81 File Offset: 0x00002081
		public bool IsProvided { get; set; }

		// Token: 0x060000AA RID: 170 RVA: 0x00003E8A File Offset: 0x0000208A
		public Argument(string name, string defaultValue, bool isOptional, bool requiresValue, string description)
		{
			this.Name = name;
			this.DefaultValue = defaultValue;
			this.IsOptional = isOptional;
			this.RequiresValue = requiresValue;
			this.Description = description;
		}
	}
}
