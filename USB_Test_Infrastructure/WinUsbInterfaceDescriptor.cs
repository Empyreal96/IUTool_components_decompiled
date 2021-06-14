using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000011 RID: 17
	internal struct WinUsbInterfaceDescriptor
	{
		// Token: 0x0400005E RID: 94
		public byte Length;

		// Token: 0x0400005F RID: 95
		public byte DescriptorType;

		// Token: 0x04000060 RID: 96
		public byte InterfaceNumber;

		// Token: 0x04000061 RID: 97
		public byte AlternateSetting;

		// Token: 0x04000062 RID: 98
		public byte NumEndpoints;

		// Token: 0x04000063 RID: 99
		public byte InterfaceClass;

		// Token: 0x04000064 RID: 100
		public byte InterfaceSubClass;

		// Token: 0x04000065 RID: 101
		public byte InterfaceProtocol;

		// Token: 0x04000066 RID: 102
		public byte Interface;
	}
}
