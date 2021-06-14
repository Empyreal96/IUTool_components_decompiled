using System;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.SecureBoot
{
	// Token: 0x02000004 RID: 4
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Build.Signing")]
	internal class SignInfo
	{
		// Token: 0x0600000F RID: 15 RVA: 0x0000243C File Offset: 0x0000063C
		public static SignInfo LoadFromFile(string SignInfoPath)
		{
			SignInfo result;
			using (FileStream fileStream = new FileStream(SignInfoPath, FileMode.Open, FileAccess.Read))
			{
				result = (SignInfo)new DataContractSerializer(typeof(SignInfo)).ReadObject(fileStream);
			}
			return result;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000010 RID: 16 RVA: 0x0000248C File Offset: 0x0000068C
		// (set) Token: 0x06000011 RID: 17 RVA: 0x00002494 File Offset: 0x00000694
		[DataMember]
		public string BinaryID { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000249D File Offset: 0x0000069D
		// (set) Token: 0x06000013 RID: 19 RVA: 0x000024A5 File Offset: 0x000006A5
		[DataMember]
		public string BinaryIdHash { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000024AE File Offset: 0x000006AE
		// (set) Token: 0x06000015 RID: 21 RVA: 0x000024B6 File Offset: 0x000006B6
		[DataMember]
		public string CodeAuthorization { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000016 RID: 22 RVA: 0x000024BF File Offset: 0x000006BF
		// (set) Token: 0x06000017 RID: 23 RVA: 0x000024C7 File Offset: 0x000006C7
		[DataMember]
		public byte[] FullFileHash { get; set; }
	}
}
