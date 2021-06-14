using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x0200000E RID: 14
	public class CompDBPublishingPackageInfo
	{
		// Token: 0x0600008D RID: 141 RVA: 0x00004257 File Offset: 0x00002457
		public CompDBPublishingPackageInfo()
		{
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00005B46 File Offset: 0x00003D46
		public CompDBPublishingPackageInfo(CompDBPublishingPackageInfo srcPkg)
		{
			this.Path = srcPkg.Path;
			this.PackageHash = srcPkg.PackageHash;
			this.ChunkName = srcPkg.ChunkName;
			this.ChunkRelativePath = srcPkg.ChunkRelativePath;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005B7E File Offset: 0x00003D7E
		public CompDBPublishingPackageInfo(CompDBPayloadInfo srcPayload)
		{
			this.Path = srcPayload.Path;
			this.PackageHash = srcPayload.PayloadHash;
			this.ChunkName = srcPayload.ChunkName;
			this.ChunkRelativePath = srcPayload.ChunkPath;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005BB8 File Offset: 0x00003DB8
		public static bool ValidateSignature(string file)
		{
			List<string> validRootThumbprints = new string[]
			{
				"3B1EFD3A66EA28B16697394703A72CA340A05BD5",
				"9E594333273339A97051B0F82E86F266B917EDB3",
				"5f444a6740b7ca2434c7a5925222c2339ee0f1b7"
			}.ToList<string>();
			return ImageSigner.HasValidSignature(file, validRootThumbprints);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00005BF0 File Offset: 0x00003DF0
		public static string GetSignatureString(string file)
		{
			return ImageSigner.GetSignatureIssuer(file);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005BF8 File Offset: 0x00003DF8
		public override string ToString()
		{
			return this.Path + " (" + this.ChunkName + ")";
		}

		// Token: 0x0400005C RID: 92
		[XmlAttribute]
		public string Path;

		// Token: 0x0400005D RID: 93
		[XmlAttribute]
		public string PackageHash;

		// Token: 0x0400005E RID: 94
		[XmlAttribute]
		public string ChunkName;

		// Token: 0x0400005F RID: 95
		[XmlAttribute]
		public string ChunkRelativePath;
	}
}
