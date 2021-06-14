using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy;

namespace Microsoft.CompPlat.PkgBldr.Base.Security
{
	// Token: 0x02000047 RID: 71
	public class GlobalSecurity
	{
		// Token: 0x06000167 RID: 359 RVA: 0x00009600 File Offset: 0x00007800
		public GlobalSecurity()
		{
			this.m_FileSecurityDescriptorDefinitionList = new Dictionary<string, string>();
			this.m_DirectorySecurityDescriptorDefinitionList = new Dictionary<string, string>();
			this.m_RegKeySecurityDescriptorDefinitionList = new Dictionary<string, string>();
			this.m_ServiceAccessSecurityDescriptorDefinitionList = new Dictionary<string, string>();
			this.m_WnfSecurityDescriptorDefinitionList = new Dictionary<string, WnfValue>();
			this.m_SecurityDescriptorRegKeyList = new Dictionary<string, SdRegValue>();
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00009658 File Offset: 0x00007858
		public bool SddlListsAreEmpty()
		{
			return this.m_DirectorySecurityDescriptorDefinitionList.Count<KeyValuePair<string, string>>() <= 0 && this.m_FileSecurityDescriptorDefinitionList.Count<KeyValuePair<string, string>>() <= 0 && this.m_RegKeySecurityDescriptorDefinitionList.Count<KeyValuePair<string, string>>() <= 0 && this.m_SecurityDescriptorRegKeyList.Count<KeyValuePair<string, SdRegValue>>() <= 0 && this.m_ServiceAccessSecurityDescriptorDefinitionList.Count<KeyValuePair<string, string>>() <= 0 && this.m_WnfSecurityDescriptorDefinitionList.Count<KeyValuePair<string, WnfValue>>() <= 0;
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000169 RID: 361 RVA: 0x000096C6 File Offset: 0x000078C6
		public Dictionary<string, string> FileSddlList
		{
			get
			{
				return this.m_FileSecurityDescriptorDefinitionList;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600016A RID: 362 RVA: 0x000096CE File Offset: 0x000078CE
		public Dictionary<string, string> DirSddlList
		{
			get
			{
				return this.m_DirectorySecurityDescriptorDefinitionList;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600016B RID: 363 RVA: 0x000096D6 File Offset: 0x000078D6
		public Dictionary<string, string> RegKeySddlList
		{
			get
			{
				return this.m_RegKeySecurityDescriptorDefinitionList;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600016C RID: 364 RVA: 0x000096DE File Offset: 0x000078DE
		public Dictionary<string, string> ServiceSddlList
		{
			get
			{
				return this.m_ServiceAccessSecurityDescriptorDefinitionList;
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600016D RID: 365 RVA: 0x000096E6 File Offset: 0x000078E6
		public Dictionary<string, WnfValue> WnfValueList
		{
			get
			{
				return this.m_WnfSecurityDescriptorDefinitionList;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600016E RID: 366 RVA: 0x000096EE File Offset: 0x000078EE
		public Dictionary<string, SdRegValue> SdRegValuelList
		{
			get
			{
				return this.m_SecurityDescriptorRegKeyList;
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x000096F8 File Offset: 0x000078F8
		public void AddCapability(string capId, WnfValue wnfValue, string rights, bool adminOnMultiSession)
		{
			if (wnfValue == null)
			{
				throw new PkgGenException("Invalid WnfValue parameter");
			}
			ResourceType type = ResourceType.Wnf;
			AccessControlPolicy accessControlPolicy = new Capability(capId, type, rights, adminOnMultiSession);
			string id = wnfValue.GetId();
			WnfValue wnfValue2;
			if (this.m_WnfSecurityDescriptorDefinitionList.TryGetValue(id, out wnfValue2))
			{
				this.m_WnfSecurityDescriptorDefinitionList[id].SecurityDescriptor = AccessControlPolicy.MergeUniqueAccessControlEntries(this.m_WnfSecurityDescriptorDefinitionList[id].SecurityDescriptor, accessControlPolicy.GetUniqueAccessControlEntries());
				return;
			}
			wnfValue.SecurityDescriptor = accessControlPolicy.GetSecurityDescriptor();
			this.m_WnfSecurityDescriptorDefinitionList.Add(id, wnfValue);
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00009780 File Offset: 0x00007980
		public void AddPrivateResource(WnfValue wnfValue, string resourceClaimer, PrivateResourceClaimerType resourceClaimerType, bool readOnly)
		{
			if (wnfValue == null)
			{
				throw new PkgGenException("Invalid WnfValue parameter");
			}
			AccessControlPolicy accessControlPolicy = new PrivateResource(ResourceType.Wnf, resourceClaimer, resourceClaimerType, readOnly);
			string id = wnfValue.GetId();
			WnfValue wnfValue2;
			if (this.m_WnfSecurityDescriptorDefinitionList.TryGetValue(id, out wnfValue2))
			{
				this.m_WnfSecurityDescriptorDefinitionList[id].SecurityDescriptor = AccessControlPolicy.MergeUniqueAccessControlEntries(this.m_WnfSecurityDescriptorDefinitionList[id].SecurityDescriptor, accessControlPolicy.GetUniqueAccessControlEntries());
				return;
			}
			wnfValue.SecurityDescriptor = accessControlPolicy.GetSecurityDescriptor();
			this.m_WnfSecurityDescriptorDefinitionList.Add(id, wnfValue);
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00009805 File Offset: 0x00007A05
		public void AddCapability(string capId, SdRegValue sdRegValue, ResourceType resourceType, string rights, bool adminOnMultiSession)
		{
			this.AddCapability(capId, sdRegValue, resourceType, rights, adminOnMultiSession, false);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00009818 File Offset: 0x00007A18
		public void AddCapability(string capId, SdRegValue sdRegValue, ResourceType resourceType, string rights, bool adminOnMultiSession, bool protectToUser)
		{
			if (sdRegValue == null)
			{
				throw new PkgGenException("Invalid SdRegValue parameter");
			}
			AccessControlPolicy accessControlPolicy = new Capability(capId, resourceType, rights, adminOnMultiSession, protectToUser);
			bool flag = false;
			SdRegValue sdRegValue2 = null;
			string uniqueIdentifier = sdRegValue.GetUniqueIdentifier();
			if (resourceType == ResourceType.ComLaunch)
			{
				flag = true;
			}
			if (!this.m_SecurityDescriptorRegKeyList.TryGetValue(uniqueIdentifier, out sdRegValue2))
			{
				if (flag)
				{
					sdRegValue.AdditionalValue = accessControlPolicy.GetSecurityDescriptor();
				}
				else
				{
					sdRegValue.Value = accessControlPolicy.GetSecurityDescriptor();
				}
				this.m_SecurityDescriptorRegKeyList.Add(uniqueIdentifier, sdRegValue);
				return;
			}
			if (flag)
			{
				if (sdRegValue2.AdditionalValue == null)
				{
					sdRegValue2.AdditionalValue = accessControlPolicy.GetSecurityDescriptor();
					return;
				}
				sdRegValue2.AdditionalValue = AccessControlPolicy.MergeUniqueAccessControlEntries(sdRegValue2.AdditionalValue, accessControlPolicy.GetUniqueAccessControlEntries());
				return;
			}
			else
			{
				if (sdRegValue2.Value == null)
				{
					sdRegValue2.Value = accessControlPolicy.GetSecurityDescriptor();
					return;
				}
				sdRegValue2.Value = AccessControlPolicy.MergeUniqueAccessControlEntries(sdRegValue2.Value, accessControlPolicy.GetUniqueAccessControlEntries());
				return;
			}
		}

		// Token: 0x06000173 RID: 371 RVA: 0x000098E9 File Offset: 0x00007AE9
		public void AddPrivateResource(SdRegValue sdRegValue, ResourceType resourceType, string resourceClaimer, PrivateResourceClaimerType resourceClaimerType, bool readOnly)
		{
			this.AddPrivateResource(sdRegValue, resourceType, resourceClaimer, resourceClaimerType, readOnly, false);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x000098FC File Offset: 0x00007AFC
		public void AddPrivateResource(SdRegValue sdRegValue, ResourceType resourceType, string resourceClaimer, PrivateResourceClaimerType resourceClaimerType, bool readOnly, bool protectToUser)
		{
			if (sdRegValue == null)
			{
				throw new PkgGenException("Invalid SdRegValue parameter");
			}
			AccessControlPolicy accessControlPolicy = new PrivateResource(resourceType, resourceClaimer, resourceClaimerType, readOnly, protectToUser);
			bool flag = false;
			SdRegValue sdRegValue2 = null;
			string uniqueIdentifier = sdRegValue.GetUniqueIdentifier();
			if (resourceType == ResourceType.ComLaunch)
			{
				flag = true;
			}
			if (!this.m_SecurityDescriptorRegKeyList.TryGetValue(uniqueIdentifier, out sdRegValue2))
			{
				if (flag)
				{
					sdRegValue.AdditionalValue = accessControlPolicy.GetSecurityDescriptor();
				}
				else
				{
					sdRegValue.Value = accessControlPolicy.GetSecurityDescriptor();
				}
				this.m_SecurityDescriptorRegKeyList.Add(uniqueIdentifier, sdRegValue);
				return;
			}
			if (flag)
			{
				if (sdRegValue2.AdditionalValue == null)
				{
					sdRegValue2.AdditionalValue = accessControlPolicy.GetSecurityDescriptor();
					return;
				}
				sdRegValue2.AdditionalValue = AccessControlPolicy.MergeUniqueAccessControlEntries(sdRegValue2.AdditionalValue, accessControlPolicy.GetUniqueAccessControlEntries());
				return;
			}
			else
			{
				if (sdRegValue2.Value == null)
				{
					sdRegValue2.Value = accessControlPolicy.GetSecurityDescriptor();
					return;
				}
				sdRegValue2.Value = AccessControlPolicy.MergeUniqueAccessControlEntries(sdRegValue2.Value, accessControlPolicy.GetUniqueAccessControlEntries());
				return;
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x000099CD File Offset: 0x00007BCD
		public void AddCapability(string capId, string path, ResourceType resourceType, string rights, bool adminOnMultiSession)
		{
			this.AddCapability(capId, path, resourceType, rights, adminOnMultiSession, false, null);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x000099E0 File Offset: 0x00007BE0
		public void AddCapability(string capId, string path, ResourceType resourceType, string rights, bool adminOnMultiSession, bool protectToUser, string phoneSddl)
		{
			if (path == null)
			{
				throw new PkgGenException("Invalid path parameter");
			}
			Dictionary<string, string> dictionary;
			switch (resourceType)
			{
			case ResourceType.File:
				dictionary = this.m_FileSecurityDescriptorDefinitionList;
				goto IL_A5;
			case ResourceType.Directory:
				dictionary = this.m_DirectorySecurityDescriptorDefinitionList;
				path = path.TrimEnd("(*)".ToCharArray());
				path = path.TrimEnd(new char[]
				{
					'\\'
				});
				goto IL_A5;
			case ResourceType.Registry:
				dictionary = this.m_RegKeySecurityDescriptorDefinitionList;
				path = path.TrimEnd("(*)".ToCharArray());
				path = path.TrimEnd(new char[]
				{
					'\\'
				});
				goto IL_A5;
			case ResourceType.ServiceAccess:
				dictionary = this.m_ServiceAccessSecurityDescriptorDefinitionList;
				goto IL_A5;
			}
			throw new PkgGenException("Invalid resource type. Failed to add capability");
			IL_A5:
			if (phoneSddl != null)
			{
				dictionary.Add(path, phoneSddl);
				return;
			}
			AccessControlPolicy accessControlPolicy = new Capability(capId, resourceType, rights, adminOnMultiSession, protectToUser);
			string text;
			if (dictionary.TryGetValue(path, out text))
			{
				dictionary[path] = AccessControlPolicy.MergeUniqueAccessControlEntries(dictionary[path], accessControlPolicy.GetUniqueAccessControlEntries());
				return;
			}
			dictionary.Add(path, accessControlPolicy.GetSecurityDescriptor());
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00009AE0 File Offset: 0x00007CE0
		public void AddPrivateResource(string path, ResourceType resourceType, string resourceClaimer, PrivateResourceClaimerType resourceClaimerType, bool readOnly)
		{
			this.AddPrivateResource(path, resourceType, resourceClaimer, resourceClaimerType, readOnly, false);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00009AF0 File Offset: 0x00007CF0
		public void AddPrivateResource(string path, ResourceType resourceType, string resourceClaimer, PrivateResourceClaimerType resourceClaimerType, bool readOnly, bool protectToUser)
		{
			if (path == null)
			{
				throw new PkgGenException("Invalid path parameter");
			}
			Dictionary<string, string> dictionary;
			switch (resourceType)
			{
			case ResourceType.File:
				dictionary = this.m_FileSecurityDescriptorDefinitionList;
				goto IL_A5;
			case ResourceType.Directory:
				dictionary = this.m_DirectorySecurityDescriptorDefinitionList;
				path = path.TrimEnd("(*)".ToCharArray());
				path = path.TrimEnd(new char[]
				{
					'\\'
				});
				goto IL_A5;
			case ResourceType.Registry:
				dictionary = this.m_RegKeySecurityDescriptorDefinitionList;
				path = path.TrimEnd("(*)".ToCharArray());
				path = path.TrimEnd(new char[]
				{
					'\\'
				});
				goto IL_A5;
			case ResourceType.ServiceAccess:
				dictionary = this.m_ServiceAccessSecurityDescriptorDefinitionList;
				goto IL_A5;
			}
			throw new PkgGenException("Invalid resource type. Failed to add private resource");
			IL_A5:
			AccessControlPolicy accessControlPolicy = new PrivateResource(resourceType, resourceClaimer, resourceClaimerType, readOnly, protectToUser);
			string text;
			if (dictionary.TryGetValue(path, out text))
			{
				dictionary[path] = AccessControlPolicy.MergeUniqueAccessControlEntries(dictionary[path], accessControlPolicy.GetUniqueAccessControlEntries());
				return;
			}
			dictionary.Add(path, accessControlPolicy.GetSecurityDescriptor());
		}

		// Token: 0x0400008D RID: 141
		private Dictionary<string, string> m_FileSecurityDescriptorDefinitionList;

		// Token: 0x0400008E RID: 142
		private Dictionary<string, string> m_DirectorySecurityDescriptorDefinitionList;

		// Token: 0x0400008F RID: 143
		private Dictionary<string, string> m_RegKeySecurityDescriptorDefinitionList;

		// Token: 0x04000090 RID: 144
		private Dictionary<string, string> m_ServiceAccessSecurityDescriptorDefinitionList;

		// Token: 0x04000091 RID: 145
		private Dictionary<string, WnfValue> m_WnfSecurityDescriptorDefinitionList;

		// Token: 0x04000092 RID: 146
		private Dictionary<string, SdRegValue> m_SecurityDescriptorRegKeyList;
	}
}
