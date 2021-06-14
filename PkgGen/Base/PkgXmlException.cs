using System;
using System.Xml.Linq;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000005 RID: 5
	public class PkgXmlException : PkgGenException
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00003BA1 File Offset: 0x00001DA1
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00003BA9 File Offset: 0x00001DA9
		public XElement XmlElement { get; set; }

		// Token: 0x06000026 RID: 38 RVA: 0x00003BB2 File Offset: 0x00001DB2
		public PkgXmlException(XElement failedElement, string message, params object[] args) : this(null, failedElement, message, args)
		{
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003BBE File Offset: 0x00001DBE
		public PkgXmlException(Exception innerException, XElement failedElement, string message, params object[] args) : base(innerException, message, args)
		{
			this.XmlElement = failedElement;
		}
	}
}
