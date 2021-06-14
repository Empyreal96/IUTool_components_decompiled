using System;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000014 RID: 20
	[CLSCompliant(false)]
	public interface IDeviceIdentifier
	{
		// Token: 0x06000079 RID: 121
		void ReadFromStream(BinaryReader reader);

		// Token: 0x0600007A RID: 122
		void WriteToStream(BinaryWriter writer);

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600007B RID: 123
		[CLSCompliant(false)]
		uint Size { get; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600007C RID: 124
		// (set) Token: 0x0600007D RID: 125
		BcdElementBootDevice Parent { get; set; }

		// Token: 0x0600007E RID: 126
		void LogInfo(IULogger logger, int indentLevel);
	}
}
