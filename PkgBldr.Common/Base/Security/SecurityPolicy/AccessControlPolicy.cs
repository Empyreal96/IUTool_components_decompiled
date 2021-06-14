using System;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x0200004F RID: 79
	public abstract class AccessControlPolicy
	{
		// Token: 0x0600017C RID: 380 RVA: 0x00009CD4 File Offset: 0x00007ED4
		public AccessControlPolicy(ResourceType Type)
		{
			switch (Type)
			{
			case ResourceType.File:
				this.m_Owner = "O:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";
				this.m_Group = "G:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";
				this.m_DefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";
				return;
			case ResourceType.Directory:
				this.m_InheritanceFlags = "CIOI";
				this.m_Owner = "O:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";
				this.m_Group = "G:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";
				this.m_DefaultDacl = "(A;CIOI;0x111FFFFF;;;CO)(A;CIOI;0x111FFFFF;;;SY)(A;CIOI;0x111FFFFF;;;BA)";
				return;
			case ResourceType.Registry:
				this.m_InheritanceFlags = "CI";
				this.m_Owner = "O:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";
				this.m_Group = "G:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";
				this.m_DefaultDacl = "(A;CI;0x111FFFFF;;;CO)(A;CI;0x111FFFFF;;;SY)(A;CI;0x111FFFFF;;;BA)";
				return;
			case ResourceType.TransientObject:
				this.m_DefaultDacl = "(A;;0x111FFFFF;;;CO)(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";
				return;
			case ResourceType.ServiceAccess:
				this.m_Owner = "O:SY";
				this.m_Group = "G:SY";
				this.m_DefaultDacl = "(A;;GRCR;;;IU)(A;;GRCR;;;SU)(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";
				return;
			case ResourceType.ComLaunch:
				this.m_Owner = "O:SY";
				this.m_Group = "G:SY";
				this.m_DefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";
				this.m_DefaultSacl = "(ML;;NX;;;LW)";
				return;
			case ResourceType.ComAccess:
				this.m_Owner = "O:SY";
				this.m_Group = "G:SY";
				this.m_DefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";
				this.m_DefaultSacl = "(ML;;NX;;;LW)";
				return;
			case ResourceType.WinRt:
				this.m_Owner = "O:SY";
				this.m_Group = "G:SY";
				this.m_DefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";
				this.m_DefaultSacl = "(ML;;NX;;;LW)";
				return;
			case ResourceType.EtwProvider:
				this.m_Owner = "O:SY";
				this.m_Group = "G:SY";
				this.m_DefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";
				return;
			case ResourceType.Wnf:
				this.m_DefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";
				return;
			case ResourceType.SdReg:
				this.m_Owner = "O:SY";
				this.m_Group = "G:SY";
				this.m_DefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";
				return;
			case ResourceType.Driver:
				this.m_DefaultDacl = "P(A;;GA;;;SY)";
				return;
			default:
				throw new PkgGenException("Invalid resource type. Failed to intialize policy object");
			}
		}

		// Token: 0x0600017D RID: 381
		public abstract string GetUniqueAccessControlEntries();

		// Token: 0x0600017E RID: 382 RVA: 0x00009EB9 File Offset: 0x000080B9
		public virtual string GetDefaultDacl()
		{
			return "D:" + this.m_DefaultDacl;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00009ECB File Offset: 0x000080CB
		public virtual string GetDefaultSacl()
		{
			if (this.m_DefaultSacl != null)
			{
				return "S:" + this.m_DefaultSacl;
			}
			return null;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00009EE7 File Offset: 0x000080E7
		public string GetSecurityDescriptor()
		{
			return string.Concat(new string[]
			{
				this.m_Owner,
				this.m_Group,
				this.GetDefaultDacl(),
				this.GetUniqueAccessControlEntries(),
				this.GetDefaultSacl()
			});
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00009F24 File Offset: 0x00008124
		public static string MergeUniqueAccessControlEntries(string SecurityDescriptor, string UniqueAccessControlEntries)
		{
			string[] separator = new string[]
			{
				"S:"
			};
			string[] array = SecurityDescriptor.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = UniqueAccessControlEntries.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			string text = array[0] + array2[0];
			if (array.Length > 1)
			{
				text = text + "S:" + array[1];
			}
			return text;
		}

		// Token: 0x040000F8 RID: 248
		protected string m_InheritanceFlags;

		// Token: 0x040000F9 RID: 249
		protected string m_Owner;

		// Token: 0x040000FA RID: 250
		protected string m_Group;

		// Token: 0x040000FB RID: 251
		protected string m_DefaultDacl;

		// Token: 0x040000FC RID: 252
		protected string m_DefaultSacl;

		// Token: 0x040000FD RID: 253
		protected string m_Access;
	}
}
