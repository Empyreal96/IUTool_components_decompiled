using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200006B RID: 107
	[XmlType(IncludeInSchema = false)]
	public enum DeviceTypeChoice
	{
		// Token: 0x0400028B RID: 651
		GPTDevice,
		// Token: 0x0400028C RID: 652
		MBRDevice,
		// Token: 0x0400028D RID: 653
		RamdiskDevice
	}
}
