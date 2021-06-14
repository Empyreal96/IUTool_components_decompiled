using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000034 RID: 52
	public class FeatureIdentifierPackage
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000225 RID: 549 RVA: 0x00013553 File Offset: 0x00011753
		[XmlIgnore]
		public string FeatureIDWithFMID
		{
			get
			{
				return FeatureManifest.GetFeatureIDWithFMID(this.FeatureID, this.FMID);
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00013566 File Offset: 0x00011766
		public FeatureIdentifierPackage()
		{
		}

		// Token: 0x06000227 RID: 551 RVA: 0x00013580 File Offset: 0x00011780
		public FeatureIdentifierPackage(PublishingPackageInfo pkg)
		{
			this.ID = pkg.ID;
			this.Partition = pkg.Partition;
			this.ownerType = pkg.OwnerType;
			this.FeatureID = pkg.FeatureID;
			this.FMID = pkg.FMID;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x000135E4 File Offset: 0x000117E4
		public override string ToString()
		{
			string text = string.Concat(new string[]
			{
				this.FeatureIDWithFMID,
				" : ",
				this.ID,
				":",
				this.Partition
			});
			switch (this.FixUpAction)
			{
			case FeatureIdentifierPackage.FixUpActions.Ignore:
				text = text + " (" + this.FixUpAction.ToString() + ")";
				break;
			case FeatureIdentifierPackage.FixUpActions.MoveToAnotherFeature:
			case FeatureIdentifierPackage.FixUpActions.AndFeature:
				text = string.Concat(new string[]
				{
					text,
					" (",
					this.FixUpAction.ToString(),
					" = ",
					this.FixUpActionValue,
					")"
				});
				break;
			}
			return text;
		}

		// Token: 0x0400016C RID: 364
		[XmlAttribute]
		public string FeatureID;

		// Token: 0x0400016D RID: 365
		[XmlAttribute]
		public string FMID;

		// Token: 0x0400016E RID: 366
		[XmlAttribute]
		public string ID;

		// Token: 0x0400016F RID: 367
		[XmlAttribute]
		[DefaultValue("MainOS")]
		public string Partition = "MainOS";

		// Token: 0x04000170 RID: 368
		[XmlAttribute("OwnerType")]
		[DefaultValue(OwnerType.Microsoft)]
		public OwnerType ownerType = OwnerType.Microsoft;

		// Token: 0x04000171 RID: 369
		[XmlAttribute]
		[DefaultValue(FeatureIdentifierPackage.FixUpActions.None)]
		public FeatureIdentifierPackage.FixUpActions FixUpAction;

		// Token: 0x04000172 RID: 370
		[XmlAttribute]
		public string FixUpActionValue;

		// Token: 0x0200009C RID: 156
		public enum FixUpActions
		{
			// Token: 0x0400034E RID: 846
			None,
			// Token: 0x0400034F RID: 847
			Ignore,
			// Token: 0x04000350 RID: 848
			MoveToAnotherFeature,
			// Token: 0x04000351 RID: 849
			AndFeature
		}
	}
}
