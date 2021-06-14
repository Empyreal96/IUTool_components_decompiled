using System;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000023 RID: 35
	public class DirectoryAcl : ResourceAcl
	{
		// Token: 0x06000158 RID: 344 RVA: 0x000068FC File Offset: 0x00004AFC
		public DirectoryAcl()
		{
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00006904 File Offset: 0x00004B04
		public DirectoryAcl(string directory, string rootPath)
		{
			if (!LongPathDirectory.Exists(directory))
			{
				throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, "Folder {0} cannot be found", new object[]
				{
					directory
				}));
			}
			DirectoryInfo di = new DirectoryInfo(directory);
			this.Initialize(di, rootPath);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000694D File Offset: 0x00004B4D
		public DirectoryAcl(DirectoryInfo di, string rootPath)
		{
			if (di == null)
			{
				throw new ArgumentNullException("di");
			}
			this.Initialize(di, rootPath);
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600015B RID: 347 RVA: 0x0000696C File Offset: 0x00004B6C
		// (set) Token: 0x0600015C RID: 348 RVA: 0x000069BF File Offset: 0x00004BBF
		[XmlAttribute("SACL")]
		public override string MandatoryIntegrityLabel
		{
			get
			{
				if (this.m_nos != null)
				{
					this.m_macLabel = SecurityUtils.GetFileSystemMandatoryLevel(this.m_fullPath);
					if (string.IsNullOrEmpty(this.m_macLabel))
					{
						this.m_macLabel = null;
					}
					else
					{
						this.m_macLabel = SddlNormalizer.FixAceSddl(this.m_macLabel);
					}
				}
				return this.m_macLabel;
			}
			set
			{
				this.m_macLabel = value;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600015D RID: 349 RVA: 0x000069C8 File Offset: 0x00004BC8
		public override NativeObjectSecurity ObjectSecurity
		{
			get
			{
				DirectorySecurity directorySecurity = null;
				if (this.m_nos != null)
				{
					directorySecurity = new DirectorySecurity();
					directorySecurity.SetSecurityDescriptorBinaryForm(this.m_nos.GetSecurityDescriptorBinaryForm());
				}
				return directorySecurity;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600015E RID: 350 RVA: 0x000069F7 File Offset: 0x00004BF7
		protected override string TypeString
		{
			get
			{
				return "Directory";
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00006A00 File Offset: 0x00004C00
		protected override string ComputeExplicitDACL()
		{
			string text = null;
			if (this.m_isRoot)
			{
				text = this.m_nos.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
			}
			else
			{
				DirectorySecurity accessControl = this.m_di.GetAccessControl(AccessControlSections.All);
				AuthorizationRuleCollection accessRules = accessControl.GetAccessRules(true, false, typeof(NTAccount));
				int num = accessRules.Count;
				foreach (object obj in accessRules)
				{
					FileSystemAccessRule fileSystemAccessRule = (FileSystemAccessRule)obj;
					if (fileSystemAccessRule.IsInherited)
					{
						accessControl.RemoveAccessRule(fileSystemAccessRule);
						num--;
					}
				}
				if (base.DACLProtected && accessControl.AreAccessRulesCanonical)
				{
					accessControl.SetAccessRuleProtection(true, base.PreserveInheritance);
				}
				if (base.DACLProtected || num > 0)
				{
					text = accessControl.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = ResourceAcl.regexStripDacl.Replace(text, string.Empty);
			}
			return SddlNormalizer.FixAceSddl(text);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00006AFC File Offset: 0x00004CFC
		private void Initialize(DirectoryInfo di, string rootPath)
		{
			if (di == null)
			{
				throw new ArgumentNullException("di");
			}
			this.m_di = di;
			this.m_nos = di.GetAccessControl(AccessControlSections.All);
			this.m_fullPath = di.FullName;
			this.m_isRoot = string.Equals(di.FullName, rootPath, StringComparison.OrdinalIgnoreCase);
			this.m_path = LongPath.Combine("\\", di.FullName.Remove(0, rootPath.Length)).ToUpperInvariant();
		}

		// Token: 0x04000069 RID: 105
		private bool m_isRoot;

		// Token: 0x0400006A RID: 106
		private DirectoryInfo m_di;
	}
}
