using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000024 RID: 36
	public class FileAcl : ResourceAcl
	{
		// Token: 0x06000161 RID: 353 RVA: 0x000068FC File Offset: 0x00004AFC
		public FileAcl()
		{
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00006B74 File Offset: 0x00004D74
		public FileAcl(string file, string rootPath)
		{
			if (!LongPathFile.Exists(file))
			{
				throw new FileNotFoundException("Specified file cannot be found", file);
			}
			FileInfo fi = new FileInfo(file);
			this.Initialize(fi, rootPath);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00006BAA File Offset: 0x00004DAA
		public FileAcl(FileInfo fi, string rootPath)
		{
			if (fi == null)
			{
				throw new ArgumentNullException("fi");
			}
			this.Initialize(fi, rootPath);
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00006BC8 File Offset: 0x00004DC8
		// (set) Token: 0x06000165 RID: 357 RVA: 0x000069BF File Offset: 0x00004BBF
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

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000166 RID: 358 RVA: 0x00006C1C File Offset: 0x00004E1C
		public override NativeObjectSecurity ObjectSecurity
		{
			get
			{
				FileSecurity fileSecurity = null;
				if (this.m_nos != null)
				{
					fileSecurity = new FileSecurity();
					fileSecurity.SetSecurityDescriptorBinaryForm(this.m_nos.GetSecurityDescriptorBinaryForm());
				}
				return fileSecurity;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000167 RID: 359 RVA: 0x00006C4B File Offset: 0x00004E4B
		protected override string TypeString
		{
			get
			{
				return "File";
			}
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00006C54 File Offset: 0x00004E54
		protected override string ComputeExplicitDACL()
		{
			FileSecurity accessControl = this.m_fi.GetAccessControl(AccessControlSections.All);
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
			string text = null;
			if (base.DACLProtected || num > 0)
			{
				text = accessControl.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
				if (!string.IsNullOrEmpty(text))
				{
					text = ResourceAcl.regexStripDacl.Replace(text, string.Empty);
				}
			}
			return SddlNormalizer.FixAceSddl(text);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00006D34 File Offset: 0x00004F34
		private void Initialize(FileInfo fi, string rootPath)
		{
			if (fi == null)
			{
				throw new ArgumentNullException("fi");
			}
			this.m_fi = fi;
			this.m_nos = fi.GetAccessControl(AccessControlSections.All);
			this.m_fullPath = fi.FullName;
			this.m_path = LongPath.Combine("\\", this.m_fullPath.Remove(0, rootPath.Length)).ToUpperInvariant();
		}

		// Token: 0x0400006B RID: 107
		private FileInfo m_fi;
	}
}
