using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000065 RID: 101
	[XmlRoot(ElementName = "RegKeys", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public class RegGroup : FilterGroup
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x000075B4 File Offset: 0x000057B4
		public RegGroup()
		{
			this.Keys = new List<RegistryKey>();
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x000075C8 File Offset: 0x000057C8
		public override void Build(IPackageGenerator pkgGen, SatelliteId satelliteId)
		{
			this.Keys.ForEach(delegate(RegistryKey x)
			{
				x.Build(pkgGen, satelliteId);
			});
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00007600 File Offset: 0x00005800
		public static RegGroup Load(XmlReader regFileReader)
		{
			RegGroup result;
			try
			{
				result = (RegGroup)new XmlSerializer(typeof(RegGroup), "urn:Microsoft.WindowsPhone/PackageSchema.v8.00").Deserialize(regFileReader);
			}
			catch (InvalidOperationException ex)
			{
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				throw;
			}
			return result;
		}

		// Token: 0x04000161 RID: 353
		[XmlElement("RegKey")]
		public List<RegistryKey> Keys;
	}
}
