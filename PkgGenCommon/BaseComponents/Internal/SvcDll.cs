using System;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200007A RID: 122
	[XmlRoot("ServiceDll", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public class SvcDll : SvcEntry
	{
		// Token: 0x060002A4 RID: 676 RVA: 0x0000A300 File Offset: 0x00008500
		public bool ShouldSerializeUnloadOnStop()
		{
			return !this.UnloadOnStop;
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0000A30B File Offset: 0x0000850B
		public bool ShouldSerializeHostExe()
		{
			return !this.HostExe.Equals("$(env.system32)\\svchost.exe", StringComparison.InvariantCulture);
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000A324 File Offset: 0x00008524
		public override void Build(IPackageGenerator pkgGen)
		{
			base.Build(pkgGen);
			if (pkgGen.BuildPass != BuildPass.BuildTOC)
			{
				string keyName = "$(hklm.service)\\Parameters";
				pkgGen.AddRegExpandValue(keyName, "ServiceDll", base.DevicePath);
				if (this.ServiceManifest != null)
				{
					pkgGen.AddRegValue(keyName, "ServiceManifest", RegValueType.ExpandString, this.ServiceManifest);
				}
				if (this.ServiceMain != null)
				{
					pkgGen.AddRegValue(keyName, "ServiceMain", RegValueType.String, this.ServiceMain);
				}
				if (this.UnloadOnStop)
				{
					pkgGen.AddRegValue(keyName, "ServiceDllUnloadOnStop", RegValueType.DWord, "00000001");
				}
				pkgGen.AddRegValue("$(hklm.service)", "ImagePath", RegValueType.ExpandString, string.Format("{0} -k {1}", this.HostExe, this.HostGroupName));
				pkgGen.AddRegMultiSzSegment("$(hklm.svchost)", this.HostGroupName, new string[]
				{
					this.ServiceName
				});
			}
		}

		// Token: 0x040001C3 RID: 451
		[XmlAttribute("ServiceManifest")]
		public string ServiceManifest;

		// Token: 0x040001C4 RID: 452
		[XmlAttribute("ServiceMain")]
		public string ServiceMain;

		// Token: 0x040001C5 RID: 453
		[XmlAttribute("UnloadOnStop")]
		public bool UnloadOnStop = true;

		// Token: 0x040001C6 RID: 454
		[XmlAttribute("BinaryInOneCorePkg")]
		public bool BinaryInOneCorePkg;

		// Token: 0x040001C7 RID: 455
		[XmlAttribute("HostExe")]
		public string HostExe = "$(env.system32)\\svchost.exe";

		// Token: 0x040001C8 RID: 456
		public string HostGroupName;

		// Token: 0x040001C9 RID: 457
		public string ServiceName;
	}
}
