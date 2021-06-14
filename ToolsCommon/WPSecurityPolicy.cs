using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200003D RID: 61
	[XmlRoot("PhoneSecurityPolicy")]
	public class WPSecurityPolicy
	{
		// Token: 0x06000174 RID: 372 RVA: 0x000085B8 File Offset: 0x000067B8
		public WPSecurityPolicy()
		{
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00008618 File Offset: 0x00006818
		public WPSecurityPolicy(string packageName)
		{
			this.m_packageId = packageName;
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000176 RID: 374 RVA: 0x0000867F File Offset: 0x0000687F
		// (set) Token: 0x06000177 RID: 375 RVA: 0x00008687 File Offset: 0x00006887
		[XmlAttribute]
		public string Description
		{
			get
			{
				return this.m_descr;
			}
			set
			{
				this.m_descr = value;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000178 RID: 376 RVA: 0x00008690 File Offset: 0x00006890
		// (set) Token: 0x06000179 RID: 377 RVA: 0x00008698 File Offset: 0x00006898
		[XmlAttribute]
		public string Vendor
		{
			get
			{
				return this.m_vendor;
			}
			set
			{
				this.m_vendor = value;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600017A RID: 378 RVA: 0x000086A1 File Offset: 0x000068A1
		// (set) Token: 0x0600017B RID: 379 RVA: 0x000086A9 File Offset: 0x000068A9
		[XmlAttribute]
		public string RequiredOSVersion
		{
			get
			{
				return this.m_OSVersion;
			}
			set
			{
				this.m_OSVersion = value;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600017C RID: 380 RVA: 0x000086B2 File Offset: 0x000068B2
		// (set) Token: 0x0600017D RID: 381 RVA: 0x000086BA File Offset: 0x000068BA
		[XmlAttribute]
		public string FileVersion
		{
			get
			{
				return this.m_fileVersion;
			}
			set
			{
				this.m_fileVersion = value;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600017E RID: 382 RVA: 0x000086C3 File Offset: 0x000068C3
		// (set) Token: 0x0600017F RID: 383 RVA: 0x000086CB File Offset: 0x000068CB
		[XmlAttribute]
		public string HashType
		{
			get
			{
				return this.m_hashType;
			}
			set
			{
				this.m_hashType = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000180 RID: 384 RVA: 0x000086D4 File Offset: 0x000068D4
		// (set) Token: 0x06000181 RID: 385 RVA: 0x000086DC File Offset: 0x000068DC
		[XmlAttribute]
		public string PackageID
		{
			get
			{
				return this.m_packageId;
			}
			set
			{
				this.m_packageId = value;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000182 RID: 386 RVA: 0x000086E5 File Offset: 0x000068E5
		// (set) Token: 0x06000183 RID: 387 RVA: 0x000086F2 File Offset: 0x000068F2
		[XmlArrayItem(typeof(DirectoryAcl), ElementName = "Directory")]
		[XmlArrayItem(typeof(FileAcl), ElementName = "File")]
		[XmlArrayItem(typeof(RegistryAcl), ElementName = "RegKey")]
		[XmlArrayItem(typeof(RegAclWithFullAcl), ElementName = "RegKeyFullACL")]
		[XmlArrayItem(typeof(RegistryStoredAcl), ElementName = "SDRegValue")]
		public ResourceAcl[] Rules
		{
			get
			{
				return this.m_aclCollection.ToArray<ResourceAcl>();
			}
			set
			{
				this.m_aclCollection.Clear();
				this.m_aclCollection.UnionWith(value);
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000870B File Offset: 0x0000690B
		public void Add(IEnumerable<ResourceAcl> acls)
		{
			this.m_aclCollection.UnionWith(acls);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000871C File Offset: 0x0000691C
		public void SaveToXml(string policyFile)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(WPSecurityPolicy), "urn:Microsoft.WindowsPhone/PhoneSecurityPolicyInternal.v8.00");
			using (TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(policyFile)))
			{
				xmlSerializer.Serialize(textWriter, this);
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00008770 File Offset: 0x00006970
		public static WPSecurityPolicy LoadFromXml(string policyFile)
		{
			if (!LongPathFile.Exists(policyFile))
			{
				throw new FileNotFoundException(string.Format("Policy file {0} does not exist, or cannot be read", policyFile), policyFile);
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(WPSecurityPolicy), "urn:Microsoft.WindowsPhone/PhoneSecurityPolicyInternal.v8.00");
			WPSecurityPolicy result = null;
			using (TextReader textReader = new StreamReader(LongPathFile.OpenRead(policyFile)))
			{
				result = (WPSecurityPolicy)xmlSerializer.Deserialize(textReader);
			}
			return result;
		}

		// Token: 0x040000C2 RID: 194
		private string m_descr = "Mobile Core Policy";

		// Token: 0x040000C3 RID: 195
		private string m_vendor = "Microsoft";

		// Token: 0x040000C4 RID: 196
		private string m_OSVersion = "8.00";

		// Token: 0x040000C5 RID: 197
		private string m_fileVersion = "8.00";

		// Token: 0x040000C6 RID: 198
		private string m_hashType = "Sha2";

		// Token: 0x040000C7 RID: 199
		private string m_packageId = "";

		// Token: 0x040000C8 RID: 200
		private AclCollection m_aclCollection = new AclCollection();

		// Token: 0x040000C9 RID: 201
		private const string WP8PolicyNamespace = "urn:Microsoft.WindowsPhone/PhoneSecurityPolicyInternal.v8.00";
	}
}
