using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000010 RID: 16
	[XmlRoot(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
	[Serializable]
	public class AssemblyIdentity
	{
		// Token: 0x04000094 RID: 148
		[XmlAttribute]
		public string buildType;

		// Token: 0x04000095 RID: 149
		[XmlAttribute]
		public string language;

		// Token: 0x04000096 RID: 150
		[XmlAttribute]
		public string name;

		// Token: 0x04000097 RID: 151
		[XmlAttribute]
		public string processorArchitecture;

		// Token: 0x04000098 RID: 152
		[XmlAttribute]
		public string publicKeyToken;

		// Token: 0x04000099 RID: 153
		[XmlAttribute]
		public string version;
	}
}
