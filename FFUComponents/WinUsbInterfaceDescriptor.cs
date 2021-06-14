using System;

namespace FFUComponents
{
	// Token: 0x02000036 RID: 54
	internal struct WinUsbInterfaceDescriptor
	{
		// Token: 0x040000AF RID: 175
		public byte Length;

		// Token: 0x040000B0 RID: 176
		public byte DescriptorType;

		// Token: 0x040000B1 RID: 177
		public byte InterfaceNumber;

		// Token: 0x040000B2 RID: 178
		public byte AlternateSetting;

		// Token: 0x040000B3 RID: 179
		public byte NumEndpoints;

		// Token: 0x040000B4 RID: 180
		public byte InterfaceClass;

		// Token: 0x040000B5 RID: 181
		public byte InterfaceSubClass;

		// Token: 0x040000B6 RID: 182
		public byte InterfaceProtocol;

		// Token: 0x040000B7 RID: 183
		public byte Interface;
	}
}
