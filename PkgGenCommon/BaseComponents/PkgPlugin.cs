using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000044 RID: 68
	public abstract class PkgPlugin : IPkgPlugin
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000112 RID: 274 RVA: 0x00005A54 File Offset: 0x00003C54
		public virtual string Name
		{
			get
			{
				return this.XmlElementName;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00005A5C File Offset: 0x00003C5C
		public virtual string XmlElementName
		{
			get
			{
				return base.GetType().Name;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00005A69 File Offset: 0x00003C69
		public virtual string XmlElementUniqueXPath
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000115 RID: 277
		public abstract string XmlSchemaPath { get; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00005A6C File Offset: 0x00003C6C
		public virtual bool UseSecurityCompilerPassthrough
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00005A70 File Offset: 0x00003C70
		public virtual void ValidateEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
			foreach (XElement componentEntry in componentEntries)
			{
				this.ValidateEntry(packageGenerator, componentEntry);
			}
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00005ABC File Offset: 0x00003CBC
		public virtual IEnumerable<PkgObject> ProcessEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
			List<PkgObject> list = new List<PkgObject>();
			foreach (XElement componentEntry in componentEntries)
			{
				IEnumerable<PkgObject> enumerable = this.ProcessEntry(packageGenerator, componentEntry);
				if (enumerable != null)
				{
					list.AddRange(enumerable);
				}
			}
			return list;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00005B18 File Offset: 0x00003D18
		protected virtual void ValidateEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00005B18 File Offset: 0x00003D18
		protected virtual IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040000F2 RID: 242
		internal static string BaseComponentSchemaPath = "Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BasePlugins.xsd";
	}
}
