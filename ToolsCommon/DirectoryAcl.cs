using System;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000042 RID: 66
	public class DirectoryAcl : ResourceAcl
	{
		// Token: 0x060001AA RID: 426 RVA: 0x00008F49 File Offset: 0x00007149
		public DirectoryAcl()
		{
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00008F54 File Offset: 0x00007154
		public DirectoryAcl(string directory, string rootPath)
		{
			if (!LongPathDirectory.Exists(directory))
			{
				throw new DirectoryNotFoundException(string.Format("Folder {0} cannot be found", directory));
			}
			DirectoryInfo di = new DirectoryInfo(directory);
			this.Initialize(di, rootPath);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00008F8F File Offset: 0x0000718F
		public DirectoryAcl(DirectoryInfo di, string rootPath)
		{
			if (di == null)
			{
				throw new ArgumentNullException("di");
			}
			this.Initialize(di, rootPath);
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060001AD RID: 429 RVA: 0x00008FB0 File Offset: 0x000071B0
		// (set) Token: 0x060001AE RID: 430 RVA: 0x00009012 File Offset: 0x00007212
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

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000901C File Offset: 0x0000721C
		public override NativeObjectSecurity ObjectSecurity
		{
			get
			{
				if (this.m_objectSecurity == null)
				{
					DirectorySecurity directorySecurity = null;
					if (this.m_nos != null)
					{
						directorySecurity = new DirectorySecurity();
						directorySecurity.SetSecurityDescriptorBinaryForm(this.m_nos.GetSecurityDescriptorBinaryForm());
					}
					this.m_objectSecurity = directorySecurity;
				}
				return this.m_objectSecurity;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x0000905F File Offset: 0x0000725F
		protected override string TypeString
		{
			get
			{
				return "Directory";
			}
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00009068 File Offset: 0x00007268
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

		// Token: 0x060001B2 RID: 434 RVA: 0x00009164 File Offset: 0x00007364
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
			this.m_path = System.IO.Path.Combine("\\", di.FullName.Remove(0, rootPath.Length)).ToUpper(CultureInfo.InvariantCulture);
		}

		// Token: 0x040000DE RID: 222
		private bool m_isRoot;

		// Token: 0x040000DF RID: 223
		private DirectoryInfo m_di;

		// Token: 0x040000E0 RID: 224
		private NativeObjectSecurity m_objectSecurity;
	}
}
