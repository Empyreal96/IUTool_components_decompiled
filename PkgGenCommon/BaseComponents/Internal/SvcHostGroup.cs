using System;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000075 RID: 117
	[XmlRoot(ElementName = "SvcHostGroup", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class SvcHostGroup : PkgObject
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000286 RID: 646 RVA: 0x00009E55 File Offset: 0x00008055
		// (set) Token: 0x06000287 RID: 647 RVA: 0x00009E5D File Offset: 0x0000805D
		[XmlAttribute("Name")]
		public string Name { get; set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000288 RID: 648 RVA: 0x00009E66 File Offset: 0x00008066
		// (set) Token: 0x06000289 RID: 649 RVA: 0x00009E6E File Offset: 0x0000806E
		[XmlAttribute("CoInitializeSecurityParam")]
		public bool CoInitializeSecurityParam { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x0600028A RID: 650 RVA: 0x00009E77 File Offset: 0x00008077
		// (set) Token: 0x0600028B RID: 651 RVA: 0x00009E7F File Offset: 0x0000807F
		[XmlAttribute("CoInitializeSecurityAllowLowBox")]
		public bool CoInitializeSecurityAllowLowBox { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x0600028C RID: 652 RVA: 0x00009E88 File Offset: 0x00008088
		// (set) Token: 0x0600028D RID: 653 RVA: 0x00009E90 File Offset: 0x00008090
		[XmlAttribute("CoInitializeSecurityAppId")]
		public string CoInitializeSecurityAppId { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600028E RID: 654 RVA: 0x00009E99 File Offset: 0x00008099
		// (set) Token: 0x0600028F RID: 655 RVA: 0x00009EA1 File Offset: 0x000080A1
		[XmlAttribute("DefaultRpcStackSize")]
		public int DefaultRpcStackSize { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000290 RID: 656 RVA: 0x00009EAA File Offset: 0x000080AA
		// (set) Token: 0x06000291 RID: 657 RVA: 0x00009EB2 File Offset: 0x000080B2
		[XmlAttribute("SystemCritical")]
		public bool SystemCritical { get; set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000292 RID: 658 RVA: 0x00009EBB File Offset: 0x000080BB
		// (set) Token: 0x06000293 RID: 659 RVA: 0x00009EC3 File Offset: 0x000080C3
		[XmlAttribute("AuthenticationLevel")]
		public AuthenticationLevel AuthenticationLevel { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000294 RID: 660 RVA: 0x00009ECC File Offset: 0x000080CC
		// (set) Token: 0x06000295 RID: 661 RVA: 0x00009ED4 File Offset: 0x000080D4
		[XmlAttribute("AuthenticationCapabilities")]
		public AuthenticationCapabitities AuthenticationCapabitities { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000296 RID: 662 RVA: 0x00009EDD File Offset: 0x000080DD
		// (set) Token: 0x06000297 RID: 663 RVA: 0x00009EE5 File Offset: 0x000080E5
		[XmlAttribute("ImpersonationLevel")]
		public ImpersonationLevel ImpersonationLevel { get; set; }

		// Token: 0x06000298 RID: 664 RVA: 0x00009EEE File Offset: 0x000080EE
		public SvcHostGroup()
		{
			this.AuthenticationCapabitities = (AuthenticationCapabitities.NoCustomMarshal | AuthenticationCapabitities.DisableAAA);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00009F01 File Offset: 0x00008101
		public bool ShouldSerializeAuthenticationCapabilities()
		{
			return this.AuthenticationCapabitities != (AuthenticationCapabitities.NoCustomMarshal | AuthenticationCapabitities.DisableAAA);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00009F14 File Offset: 0x00008114
		protected override void DoBuild(IPackageGenerator pkgGen)
		{
			if (pkgGen.BuildPass != BuildPass.BuildTOC)
			{
				if (this.CoInitializeSecurityParam)
				{
					pkgGen.AddRegValue("$(hklm.svchostgroup)", "CoInitializeSecurityParam", RegValueType.DWord, "00000001");
				}
				if (this.CoInitializeSecurityAllowLowBox)
				{
					pkgGen.AddRegValue("$(hklm.svchostgroup)", "CoInitializeSecurityAllowLowBox", RegValueType.DWord, "00000001");
				}
				if (this.CoInitializeSecurityAppId != null)
				{
					pkgGen.AddRegValue("$(hklm.svchostgroup)", "CoInitializeSecurityAppId", RegValueType.String, this.CoInitializeSecurityAppId);
				}
				if (this.DefaultRpcStackSize != 0)
				{
					pkgGen.AddRegValue("$(hklm.svchostgroup)", "DefaultRpcStackSize", RegValueType.DWord, string.Format("{0:X8}", this.DefaultRpcStackSize));
				}
				if (this.AuthenticationLevel != AuthenticationLevel.Default)
				{
					pkgGen.AddRegValue("$(hklm.svchostgroup)", "AuthenticationLevel", RegValueType.DWord, string.Format("{0:X8}", (int)this.AuthenticationLevel));
				}
				if (this.AuthenticationCapabitities != (AuthenticationCapabitities.NoCustomMarshal | AuthenticationCapabitities.DisableAAA))
				{
					pkgGen.AddRegValue("$(hklm.svchostgroup)", "AuthenticationCapabilities", RegValueType.DWord, string.Format("{0:X8}", (int)this.AuthenticationCapabitities));
				}
				if (this.ImpersonationLevel != ImpersonationLevel.Default)
				{
					pkgGen.AddRegValue("$(hklm.svchostgroup)", "ImpersonationLevel", RegValueType.DWord, string.Format("{0:X8}", (int)this.ImpersonationLevel));
				}
				if (this.SystemCritical)
				{
					pkgGen.AddRegValue("$(hklm.svchostgroup)", "SystemCritical", RegValueType.DWord, "00000001");
				}
			}
		}

		// Token: 0x040001B0 RID: 432
		private const AuthenticationCapabitities _DEFAULT_CAPABILITIES = AuthenticationCapabitities.NoCustomMarshal | AuthenticationCapabitities.DisableAAA;
	}
}
