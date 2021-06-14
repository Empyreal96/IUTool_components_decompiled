using System;
using System.Globalization;
using System.IO;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x0200004A RID: 74
	public sealed class InboxAppParameters
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00006BAD File Offset: 0x00004DAD
		public string PackageBasePath
		{
			get
			{
				return this._packageBasePath;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000104 RID: 260 RVA: 0x00006BB5 File Offset: 0x00004DB5
		public string LicenseBasePath
		{
			get
			{
				return this._licenseBasePath;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000105 RID: 261 RVA: 0x00006BBD File Offset: 0x00004DBD
		public string ProvXMLBasePath
		{
			get
			{
				return this._provXMLBasePath;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000106 RID: 262 RVA: 0x00006BC5 File Offset: 0x00004DC5
		public bool InfuseIntoDataPartition
		{
			get
			{
				return this._infuseIntoDataPartition;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00006BCD File Offset: 0x00004DCD
		public UpdateType UpdateValue
		{
			get
			{
				return this._updateValue;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000108 RID: 264 RVA: 0x00006BD5 File Offset: 0x00004DD5
		public ProvXMLCategory Category
		{
			get
			{
				return this._category;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000109 RID: 265 RVA: 0x00006BDD File Offset: 0x00004DDD
		public string WorkingBaseDir
		{
			get
			{
				return this._workingBaseDir;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600010A RID: 266 RVA: 0x00006BE5 File Offset: 0x00004DE5
		// (set) Token: 0x0600010B RID: 267 RVA: 0x00006BED File Offset: 0x00004DED
		public bool SkipSignatureValidation { get; set; }

		// Token: 0x0600010C RID: 268 RVA: 0x00006BF8 File Offset: 0x00004DF8
		public InboxAppParameters(string packageBasePath, string licenseBasePath, string provXMLBasePath, bool infuseIntoDataPartition, UpdateType updateValue, ProvXMLCategory category, string workingBaseDir)
		{
			this._packageBasePath = InboxAppUtils.ValidateFileOrDir(packageBasePath, false);
			this._licenseBasePath = licenseBasePath;
			this._provXMLBasePath = provXMLBasePath;
			this._infuseIntoDataPartition = infuseIntoDataPartition;
			this._updateValue = updateValue;
			this._category = category;
			this._workingBaseDir = workingBaseDir;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00006C74 File Offset: 0x00004E74
		public InboxAppParameters(string packageBasePath, string licenseBasePath, string provXMLBasePath, bool infuseIntoDataPartition, UpdateType updateValue, ProvXMLCategory category) : this(packageBasePath, licenseBasePath, provXMLBasePath, infuseIntoDataPartition, updateValue, category, Path.Combine(Path.GetDirectoryName(Path.GetFullPath(packageBasePath)), Path.GetRandomFileName()))
		{
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00006CA8 File Offset: 0x00004EA8
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "InboxApp Parameters: (PackageBasePath)=\"{0}\" (LicenseBasePath)=\"{1}\" (ProvXMLBasePath)=\"{2}\" InfuseIntoDataPartition=\"{3}\" UpdateType=\"{4}\" Category=\"{5}\"", new object[]
			{
				this._packageBasePath,
				this._licenseBasePath,
				this._provXMLBasePath,
				this._infuseIntoDataPartition,
				this._updateValue,
				this._category
			});
		}

		// Token: 0x040000CF RID: 207
		private string _packageBasePath = string.Empty;

		// Token: 0x040000D0 RID: 208
		private string _licenseBasePath = string.Empty;

		// Token: 0x040000D1 RID: 209
		private string _provXMLBasePath = string.Empty;

		// Token: 0x040000D2 RID: 210
		private bool _infuseIntoDataPartition;

		// Token: 0x040000D3 RID: 211
		private UpdateType _updateValue;

		// Token: 0x040000D4 RID: 212
		private ProvXMLCategory _category;

		// Token: 0x040000D5 RID: 213
		private string _workingBaseDir = string.Empty;
	}
}
