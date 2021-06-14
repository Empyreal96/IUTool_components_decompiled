using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.MCSF.Offline
{
	// Token: 0x0200000B RID: 11
	public class PolicyStore
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00003926 File Offset: 0x00001B26
		public IEnumerable<PolicyGroup> SettingGroups
		{
			get
			{
				return this.settingGroups;
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000392E File Offset: 0x00001B2E
		public PolicyStore()
		{
			this.settingGroups = new List<PolicyGroup>();
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003941 File Offset: 0x00001B41
		public void LoadPolicyXML(string policyDocumentPath)
		{
			this.LoadPolicyXML(policyDocumentPath, null, null);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000394C File Offset: 0x00001B4C
		public void LoadPolicyXML(string policyDocumentPath, string definedInOverride)
		{
			this.LoadPolicyXML(policyDocumentPath, definedInOverride, null);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00003958 File Offset: 0x00001B58
		public void LoadPolicyXML(string policyDocumentPath, string definedInOverride, string partition)
		{
			string text = string.IsNullOrEmpty(definedInOverride) ? policyDocumentPath : definedInOverride;
			try
			{
				XDocument policyDocument = XDocument.Load(policyDocumentPath);
				this.LoadPolicyXML(policyDocument, text, partition);
			}
			catch (MCSFOfflineException ex)
			{
				throw ex;
			}
			catch (Exception ex2)
			{
				throw new ArgumentException(string.Format("Unable to load policy from file '{0}': {1}", text, ex2.Message), ex2);
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000039BC File Offset: 0x00001BBC
		public void LoadPolicyXML(XDocument policyDocument)
		{
			this.LoadPolicyXML(policyDocument, null, null);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000039C7 File Offset: 0x00001BC7
		public void LoadPolicyXML(XDocument policyDocument, string definedInFile)
		{
			this.LoadPolicyXML(policyDocument, definedInFile, null);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000039D4 File Offset: 0x00001BD4
		public void LoadPolicyXML(XDocument policyDocument, string definedInFile, string partition)
		{
			foreach (XElement policyGroupElement in policyDocument.Root.Elements())
			{
				PolicyGroup item = new PolicyGroup(policyGroupElement, definedInFile, partition);
				this.settingGroups.Add(item);
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00003A34 File Offset: 0x00001C34
		public void LoadPolicyXML(IEnumerable<XDocument> policyDocuments)
		{
			foreach (XDocument policyDocument in policyDocuments)
			{
				this.LoadPolicyXML(policyDocument);
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00003A7C File Offset: 0x00001C7C
		public void LoadPolicyXML(IEnumerable<string> policyDocumentPaths)
		{
			foreach (string policyDocumentPath in policyDocumentPaths)
			{
				this.LoadPolicyXML(policyDocumentPath);
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00003AC4 File Offset: 0x00001CC4
		public void LoadPolicyFromPackage(IPkgInfo policyPackage)
		{
			foreach (IFileEntry fileEntry in policyPackage.Files)
			{
				if (fileEntry.DevicePath.StartsWith("\\Windows\\CustomizationPolicy\\", StringComparison.InvariantCultureIgnoreCase))
				{
					string text = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
					policyPackage.ExtractFile(fileEntry.DevicePath, text, true);
					try
					{
						this.LoadPolicyXML(text, policyPackage.Name, policyPackage.Partition);
					}
					catch (MCSFOfflineException ex)
					{
						throw ex;
					}
					catch (Exception ex2)
					{
						throw new ArgumentException(string.Format("Failed to load policy from file '{0}' inside of the given package:{1}", fileEntry.DevicePath, ex2.Message), ex2);
					}
					try
					{
						File.Delete(text);
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00003BB8 File Offset: 0x00001DB8
		public void LoadPolicyFromPackage(string policyPackagePath)
		{
			try
			{
				this.LoadPolicyFromPackage(Package.LoadFromCab(policyPackagePath));
			}
			catch (MCSFOfflineException ex)
			{
				throw ex;
			}
			catch (Exception ex2)
			{
				throw new ArgumentException(string.Format("Failed to load policy from package '{0}':{1}", policyPackagePath, ex2.Message), ex2);
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00003C0C File Offset: 0x00001E0C
		public void LoadPolicyFromPackages(IEnumerable<IPkgInfo> policyPackages)
		{
			foreach (IPkgInfo policyPackage in policyPackages)
			{
				this.LoadPolicyFromPackage(policyPackage);
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003C54 File Offset: 0x00001E54
		public void LoadPolicyFromPackages(IEnumerable<string> policyPackagePaths)
		{
			foreach (string policyPackagePath in policyPackagePaths)
			{
				this.LoadPolicyFromPackage(policyPackagePath);
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003C9C File Offset: 0x00001E9C
		public PolicyGroup SettingGroupByPath(string settingPath)
		{
			IEnumerable<PolicyGroup> enumerable = from x in this.settingGroups
			where PolicyMacroTable.IsMatch(x.Path, settingPath, StringComparison.OrdinalIgnoreCase)
			select x;
			if (enumerable.Count<PolicyGroup>() > 1)
			{
				enumerable = from x in enumerable
				where x.Path.Equals(settingPath, StringComparison.OrdinalIgnoreCase)
				select x;
			}
			if (enumerable.Count<PolicyGroup>() > 1)
			{
				string text = "";
				foreach (PolicyGroup policyGroup in enumerable)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						text += ", ";
					}
					text += policyGroup.DefinedIn;
				}
				throw new MCSFOfflineException(string.Format(CultureInfo.InvariantCulture, "Multiple definitions when searching for setting group '{0}', defined in '{1}'.", new object[]
				{
					settingPath,
					text
				}));
			}
			return enumerable.SingleOrDefault<PolicyGroup>();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00003D80 File Offset: 0x00001F80
		public PolicySetting SettingByPathAndName(string settingPath, string settingName)
		{
			PolicyGroup policyGroup = this.SettingGroupByPath(settingPath);
			if (policyGroup == null)
			{
				return null;
			}
			return policyGroup.SettingByName(settingName);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00003DA4 File Offset: 0x00001FA4
		public PolicyAssetInfo AssetByPathAndName(string settingPath, string assetName)
		{
			PolicyGroup policyGroup = this.SettingGroupByPath(settingPath);
			if (policyGroup == null)
			{
				return null;
			}
			return policyGroup.AssetByName(assetName);
		}

		// Token: 0x04000053 RID: 83
		private const string PolicyDocumentDeviceRoot = "\\Windows\\CustomizationPolicy\\";

		// Token: 0x04000054 RID: 84
		private List<PolicyGroup> settingGroups;
	}
}
