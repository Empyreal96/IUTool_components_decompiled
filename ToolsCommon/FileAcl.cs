using System;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000043 RID: 67
	public class FileAcl : ResourceAcl
	{
		// Token: 0x060001B3 RID: 435 RVA: 0x00008F49 File Offset: 0x00007149
		public FileAcl()
		{
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x000091E0 File Offset: 0x000073E0
		public FileAcl(string file, string rootPath)
		{
			if (!LongPathFile.Exists(file))
			{
				throw new FileNotFoundException("Specified file cannot be found", file);
			}
			FileInfo fi = new FileInfo(file);
			this.Initialize(fi, rootPath);
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00009216 File Offset: 0x00007416
		public FileAcl(FileInfo fi, string rootPath)
		{
			if (fi == null)
			{
				throw new ArgumentNullException("fi");
			}
			this.Initialize(fi, rootPath);
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x00009234 File Offset: 0x00007434
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x00009012 File Offset: 0x00007212
		[XmlAttribute("SACL")]
		public override string MandatoryIntegrityLabel
		{
			get
			{
				if (!this.m_macLablelProcessed)
				{
					this.m_macLablelProcessed = true;
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
				}
				return this.m_macLabel;
			}
			set
			{
				this.m_macLabel = value;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x00009298 File Offset: 0x00007498
		public override NativeObjectSecurity ObjectSecurity
		{
			get
			{
				if (this.m_objectSecurity == null && this.m_nos != null)
				{
					FileSecurity fileSecurity = new FileSecurity();
					fileSecurity.SetSecurityDescriptorBinaryForm(this.m_nos.GetSecurityDescriptorBinaryForm());
					this.m_objectSecurity = fileSecurity;
				}
				return this.m_objectSecurity;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x000092DB File Offset: 0x000074DB
		protected override string TypeString
		{
			get
			{
				return "File";
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x000092E4 File Offset: 0x000074E4
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

		// Token: 0x060001BB RID: 443 RVA: 0x000093C4 File Offset: 0x000075C4
		private void Initialize(FileInfo fi, string rootPath)
		{
			if (fi == null)
			{
				throw new ArgumentNullException("fi");
			}
			this.m_fi = fi;
			this.m_nos = fi.GetAccessControl(AccessControlSections.All);
			this.m_fullPath = fi.FullName;
			this.m_path = System.IO.Path.Combine("\\", this.m_fullPath.Remove(0, rootPath.Length)).ToUpper(CultureInfo.InvariantCulture);
		}

		// Token: 0x040000E1 RID: 225
		private FileInfo m_fi;

		// Token: 0x040000E2 RID: 226
		private NativeObjectSecurity m_objectSecurity;
	}
}
