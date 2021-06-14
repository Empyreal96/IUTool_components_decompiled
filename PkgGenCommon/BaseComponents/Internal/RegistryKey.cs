using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000069 RID: 105
	[XmlRoot(ElementName = "RegistryKey", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public class RegistryKey : PkgElement
	{
		// Token: 0x060001E5 RID: 485 RVA: 0x00007744 File Offset: 0x00005944
		public RegistryKey()
		{
			this.Values = new List<RegValue>();
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00007757 File Offset: 0x00005957
		public RegistryKey(string keyName) : this()
		{
			this.KeyName = keyName;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00007768 File Offset: 0x00005968
		public override void Build(IPackageGenerator pkgGen, SatelliteId satelliteId)
		{
			pkgGen.AddRegKey(this.KeyName, satelliteId);
			this.Values.ForEach(delegate(RegValue v)
			{
				pkgGen.AddRegValue(this.KeyName, v.Name, v.RegValType, v.Value, satelliteId);
			});
		}

		// Token: 0x0400016A RID: 362
		[XmlAttribute("KeyName")]
		public string KeyName;

		// Token: 0x0400016B RID: 363
		[XmlElement("RegValue")]
		public List<RegValue> Values;
	}
}
