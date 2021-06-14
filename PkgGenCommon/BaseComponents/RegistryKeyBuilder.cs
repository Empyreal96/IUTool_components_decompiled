using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000048 RID: 72
	public sealed class RegistryKeyBuilder
	{
		// Token: 0x0600012D RID: 301 RVA: 0x00005C40 File Offset: 0x00003E40
		internal RegistryKeyBuilder(string keyName)
		{
			this.key = new RegistryKey(keyName);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00005C54 File Offset: 0x00003E54
		public RegistryKeyBuilder AddValue(string name, string type, string value)
		{
			RegValueType regValType = RegUtil.RegValueTypeForString(type);
			RegValue regValue = new RegValue();
			regValue.Name = name;
			regValue.RegValType = regValType;
			regValue.Value = value;
			this.key.Values.Add(regValue);
			return this;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00005C98 File Offset: 0x00003E98
		public RegistryKeyBuilder AddValue(XNode valueElement)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(RegValue));
			using (XmlReader xmlReader = valueElement.CreateReader())
			{
				RegValue item = (RegValue)xmlSerializer.Deserialize(xmlReader);
				this.key.Values.Add(item);
			}
			return this;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00005CF8 File Offset: 0x00003EF8
		internal RegistryKey ToPkgObject()
		{
			return this.key;
		}

		// Token: 0x040000F6 RID: 246
		private RegistryKey key;
	}
}
