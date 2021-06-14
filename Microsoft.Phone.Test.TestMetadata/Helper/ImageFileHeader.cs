using System;

namespace Microsoft.Phone.Test.TestMetadata.Helper
{
	// Token: 0x0200000A RID: 10
	internal struct ImageFileHeader
	{
		// Token: 0x0400002A RID: 42
		public ushort Machine;

		// Token: 0x0400002B RID: 43
		public ushort NumberOfSections;

		// Token: 0x0400002C RID: 44
		public uint TimeDateStamp;

		// Token: 0x0400002D RID: 45
		public uint PointerToSymbolTable;

		// Token: 0x0400002E RID: 46
		public uint NumberOfSymbols;

		// Token: 0x0400002F RID: 47
		public ushort SizeOfOptionalHeader;

		// Token: 0x04000030 RID: 48
		public ushort Characteristics;
	}
}
