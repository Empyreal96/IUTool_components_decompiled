using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000036 RID: 54
	public abstract class ProvXMLBase : IInboxProvXML
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x00005A78 File Offset: 0x00003C78
		protected ProvXMLBase(InboxAppParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters", "INTERNAL ERROR: The parameters passed into the ProvXMLBase constructor is null!");
			}
			this._parameters = parameters;
			InboxAppUtils.ValidateFileOrDir(this._parameters.ProvXMLBasePath, false);
			this.ValidateFileNameDetails();
		}

		// Token: 0x060000D1 RID: 209
		public abstract void ReadProvXML();

		// Token: 0x060000D2 RID: 210 RVA: 0x00005AE9 File Offset: 0x00003CE9
		public void Save(string outputBasePath)
		{
			this._document.Save(outputBasePath);
		}

		// Token: 0x060000D3 RID: 211
		public abstract void Update(string installDestinationPath, string licenseFileDestinationPath);

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00005AF7 File Offset: 0x00003CF7
		public string ProvXMLDestinationPath
		{
			get
			{
				return this._provXMLDestinationPath;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00005AFF File Offset: 0x00003CFF
		public string UpdateProvXMLDestinationPath
		{
			get
			{
				return this._updateProvXMLDestinationPath;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00005B07 File Offset: 0x00003D07
		public string LicenseDestinationPath
		{
			get
			{
				return this._licenseDestinationPath;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00005B0F File Offset: 0x00003D0F
		public ProvXMLCategory Category
		{
			get
			{
				return this._parameters.Category;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00005B1C File Offset: 0x00003D1C
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00005B24 File Offset: 0x00003D24
		public string DependencyHash
		{
			get
			{
				return this._packageHash;
			}
			set
			{
				this._packageHash = value;
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00005B30 File Offset: 0x00003D30
		protected void ValidateFileNameDetails()
		{
			if (!this._parameters.ProvXMLBasePath.Contains(".provxml"))
			{
				string message = string.Format(CultureInfo.InvariantCulture, "The provxml filename \"{0}\" does not match the expected format '{1}*{2}'.", new object[]
				{
					this._parameters.ProvXMLBasePath,
					"MPAP_",
					".provxml"
				});
				LogUtil.Error(message);
				throw new NotSupportedException(message);
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00005B94 File Offset: 0x00003D94
		protected string GetMxipFileDestinationPath(string provxmlPBath, ProvXMLCategory category, UpdateType updateValue, IInboxAppManifest manifest)
		{
			string result = string.Empty;
			string fileName = Path.GetFileName(provxmlPBath);
			if (category == ProvXMLCategory.Test || category == ProvXMLCategory.Microsoft)
			{
				if (manifest != null && manifest is AppManifestAppxBase && ((AppManifestAppxBase)manifest).IsFramework)
				{
					result = string.Format(CultureInfo.InvariantCulture, "$(runtime.updateProvxmlMS)\\appframework{0}", new object[]
					{
						fileName.CleanFileNameForUpdate(false)
					});
				}
				else
				{
					result = string.Format(CultureInfo.InvariantCulture, "$(runtime.updateProvxmlMS)\\mxipupdate{0}", new object[]
					{
						fileName.CleanFileNameForUpdate(updateValue == UpdateType.UpdateEarly)
					});
				}
			}
			else if (category == ProvXMLCategory.OEM)
			{
				result = string.Format(CultureInfo.InvariantCulture, "$(runtime.updateProvxmlOEM)\\mxipupdate{0}", new object[]
				{
					fileName.CleanFileNameForUpdate(updateValue == UpdateType.UpdateEarly)
				});
			}
			return result;
		}

		// Token: 0x060000DC RID: 220
		protected abstract string DetermineProvXMLDestinationPath();

		// Token: 0x060000DD RID: 221
		protected abstract string DetermineLicenseDestinationPath();

		// Token: 0x04000045 RID: 69
		protected InboxAppParameters _parameters;

		// Token: 0x04000046 RID: 70
		protected string _provXMLDestinationPath = string.Empty;

		// Token: 0x04000047 RID: 71
		protected string _updateProvXMLDestinationPath = string.Empty;

		// Token: 0x04000048 RID: 72
		protected string _licenseDestinationPath = string.Empty;

		// Token: 0x04000049 RID: 73
		protected string _packageHash = string.Empty;

		// Token: 0x0400004A RID: 74
		protected XDocument _document;
	}
}
