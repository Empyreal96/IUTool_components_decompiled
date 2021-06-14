using System;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000045 RID: 69
	public class PkgXmlException : PkgGenException
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600011D RID: 285 RVA: 0x00005B2B File Offset: 0x00003D2B
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00005B33 File Offset: 0x00003D33
		public XElement XmlElement { get; set; }

		// Token: 0x0600011F RID: 287 RVA: 0x00005B3C File Offset: 0x00003D3C
		public PkgXmlException(XElement failedElement, string message, params object[] args) : this(null, failedElement, message, args)
		{
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00005B48 File Offset: 0x00003D48
		public PkgXmlException(Exception innerException, XElement failedElement, string message, params object[] args) : base(innerException, message, args)
		{
			this.XmlElement = failedElement;
		}
	}
}
