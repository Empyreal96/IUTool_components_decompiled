using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200007B RID: 123
	[XmlRoot(ElementName = "Service", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class ServicePkgObject : OSComponentPkgObject
	{
		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x0000A40C File Offset: 0x0000860C
		// (set) Token: 0x060002A9 RID: 681 RVA: 0x0000A414 File Offset: 0x00008614
		[XmlAttribute("Name")]
		public string Name { get; set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060002AA RID: 682 RVA: 0x0000A41D File Offset: 0x0000861D
		// (set) Token: 0x060002AB RID: 683 RVA: 0x0000A425 File Offset: 0x00008625
		[XmlAttribute("DisplayName")]
		public string DisplayName { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060002AC RID: 684 RVA: 0x0000A42E File Offset: 0x0000862E
		// (set) Token: 0x060002AD RID: 685 RVA: 0x0000A436 File Offset: 0x00008636
		[XmlAttribute("Description")]
		public string Description { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060002AE RID: 686 RVA: 0x0000A43F File Offset: 0x0000863F
		// (set) Token: 0x060002AF RID: 687 RVA: 0x0000A447 File Offset: 0x00008647
		[XmlAttribute("Group")]
		public string Group { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x0000A450 File Offset: 0x00008650
		// (set) Token: 0x060002B1 RID: 689 RVA: 0x0000A458 File Offset: 0x00008658
		[XmlAttribute("SvcHostGroupName")]
		public string SvcHostGroupName { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x0000A461 File Offset: 0x00008661
		// (set) Token: 0x060002B3 RID: 691 RVA: 0x0000A469 File Offset: 0x00008669
		[XmlAttribute("Start")]
		public ServiceStartMode StartMode { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x0000A472 File Offset: 0x00008672
		// (set) Token: 0x060002B5 RID: 693 RVA: 0x0000A47A File Offset: 0x0000867A
		[XmlAttribute("Type")]
		public ServiceType SvcType { get; set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x0000A483 File Offset: 0x00008683
		// (set) Token: 0x060002B7 RID: 695 RVA: 0x0000A48B File Offset: 0x0000868B
		[XmlAttribute("ErrorControl")]
		public ErrorControlOption ErrorControl { get; set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x0000A494 File Offset: 0x00008694
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x0000A49C File Offset: 0x0000869C
		[XmlAttribute("DependOnGroup")]
		public string DependOnGroup { get; set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x060002BA RID: 698 RVA: 0x0000A4A5 File Offset: 0x000086A5
		// (set) Token: 0x060002BB RID: 699 RVA: 0x0000A4AD File Offset: 0x000086AD
		[XmlAttribute("DependOnService")]
		public string DependOnService { get; set; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060002BC RID: 700 RVA: 0x0000A4B6 File Offset: 0x000086B6
		// (set) Token: 0x060002BD RID: 701 RVA: 0x0000A4BE File Offset: 0x000086BE
		[XmlAnyElement("RequiredCapabilities")]
		public XElement RequiredCapabilities { get; set; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060002BE RID: 702 RVA: 0x0000A4C7 File Offset: 0x000086C7
		// (set) Token: 0x060002BF RID: 703 RVA: 0x0000A4CF File Offset: 0x000086CF
		[XmlAnyElement("PrivateResources")]
		public XElement PrivateResources { get; set; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x0000A4D8 File Offset: 0x000086D8
		// (set) Token: 0x060002C1 RID: 705 RVA: 0x0000A4E0 File Offset: 0x000086E0
		[XmlElement("ServiceDll", typeof(SvcDll))]
		[XmlElement("Executable", typeof(SvcExe))]
		public SvcEntry SvcEntry { get; set; }

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x0000A4E9 File Offset: 0x000086E9
		// (set) Token: 0x060002C3 RID: 707 RVA: 0x0000A4F1 File Offset: 0x000086F1
		[XmlElement("FailureActions")]
		public FailureActionsPkgObject FailureActions { get; set; }

		// Token: 0x060002C4 RID: 708 RVA: 0x0000A4FA File Offset: 0x000086FA
		public ServicePkgObject()
		{
			this.SvcEntry = null;
			this.FailureActions = null;
			this.ErrorControl = ErrorControlOption.Normal;
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000A517 File Offset: 0x00008717
		public bool ShouldSerializeErrorControl()
		{
			return this.ErrorControl != ErrorControlOption.Normal;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000A528 File Offset: 0x00008728
		protected override void DoPreprocess(PackageProject proj, IMacroResolver macroResolver)
		{
			SvcDll svcDll = this.SvcEntry as SvcDll;
			if (svcDll != null)
			{
				svcDll.HostGroupName = this.SvcHostGroupName;
				svcDll.ServiceName = this.Name;
			}
			this.SvcEntry.Preprocess(macroResolver);
			base.DoPreprocess(proj, macroResolver);
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000A570 File Offset: 0x00008770
		protected override void DoBuild(IPackageGenerator pkgGen)
		{
			base.DoBuild(pkgGen);
			if (pkgGen.BuildPass != BuildPass.BuildTOC)
			{
				pkgGen.AddRegValue("$(hklm.service)", "PreshutdownTimeout", RegValueType.DWord, string.Format("{0:X8}", 20000U));
				if (this.StartMode == ServiceStartMode.DelayedAuto)
				{
					pkgGen.AddRegValue("$(hklm.service)", "Start", RegValueType.DWord, string.Format("{0:X8}", 2U));
					pkgGen.AddRegValue("$(hklm.service)", "DelayedAutoStart", RegValueType.DWord, string.Format("{0:X8}", 1U));
				}
				else
				{
					pkgGen.AddRegValue("$(hklm.service)", "Start", RegValueType.DWord, string.Format("{0:X8}", (uint)this.StartMode));
				}
				pkgGen.AddRegValue("$(hklm.service)", "Type", RegValueType.DWord, string.Format("{0:X8}", (uint)this.SvcType));
				pkgGen.AddRegValue("$(hklm.service)", "ErrorControl", RegValueType.DWord, string.Format("{0:X8}", (uint)this.ErrorControl));
				if (this.DisplayName != null)
				{
					pkgGen.AddRegValue("$(hklm.service)", "DisplayName", RegValueType.String, this.DisplayName);
				}
				if (this.Description != null)
				{
					pkgGen.AddRegValue("$(hklm.service)", "Description", RegValueType.String, this.Description);
				}
				if (this.Group != null)
				{
					pkgGen.AddRegValue("$(hklm.service)", "Group", RegValueType.String, this.Group);
				}
				if (this.DependOnGroup != null)
				{
					pkgGen.AddRegValue("$(hklm.service)", "DependOnGroup", RegValueType.MultiString, this.DependOnGroup);
				}
				if (this.DependOnService != null)
				{
					pkgGen.AddRegValue("$(hklm.service)", "DependOnService", RegValueType.MultiString, this.DependOnService);
				}
				if (this.FailureActions != null)
				{
					this.FailureActions.Build(pkgGen);
				}
			}
			this.SvcEntry.Build(pkgGen);
		}
	}
}
