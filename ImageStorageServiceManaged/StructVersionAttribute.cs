using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002F RID: 47
	[AttributeUsage(AttributeTargets.Property)]
	internal class StructVersionAttribute : Attribute
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x000085D9 File Offset: 0x000067D9
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x000085E1 File Offset: 0x000067E1
		public ushort Version { get; set; }
	}
}
