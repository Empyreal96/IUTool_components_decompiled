using System;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200000C RID: 12
	public class UpdateOSOutputIdentity
	{
		// Token: 0x0600002E RID: 46 RVA: 0x00003890 File Offset: 0x00001A90
		public override string ToString()
		{
			string text = this.Owner + "." + this.Component;
			if (!string.IsNullOrEmpty(this.SubComponent))
			{
				text = text + "." + this.SubComponent;
			}
			return text;
		}

		// Token: 0x04000045 RID: 69
		public string Owner;

		// Token: 0x04000046 RID: 70
		public string Component;

		// Token: 0x04000047 RID: 71
		public string SubComponent;

		// Token: 0x04000048 RID: 72
		[CLSCompliant(false)]
		public PkgVersion Version;
	}
}
