using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000076 RID: 118
	[XmlRoot("Action", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class FailureAction
	{
		// Token: 0x0600029B RID: 667 RVA: 0x00003E08 File Offset: 0x00002008
		public FailureAction()
		{
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000A062 File Offset: 0x00008262
		public FailureAction(FailureActionType type, uint delay)
		{
			this.Type = type;
			this.Delay = delay;
		}

		// Token: 0x040001BA RID: 442
		[XmlAttribute("Type")]
		public FailureActionType Type;

		// Token: 0x040001BB RID: 443
		[XmlAttribute("Delay")]
		public uint Delay;
	}
}
