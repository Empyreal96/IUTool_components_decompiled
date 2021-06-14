using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x0200000C RID: 12
	public class CompDBPayloadInfo
	{
		// Token: 0x0600006F RID: 111 RVA: 0x00004257 File Offset: 0x00002457
		public CompDBPayloadInfo()
		{
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000052F0 File Offset: 0x000034F0
		public CompDBPayloadInfo(CompDBPayloadInfo payload)
		{
			this.Path = payload.Path;
			this.PreviousPath = payload.PreviousPath;
			this.PayloadHash = payload.PayloadHash;
			this.PayloadSize = payload.PayloadSize;
			this.PayloadType = payload.PayloadType;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000533F File Offset: 0x0000353F
		public CompDBPayloadInfo(IPkgInfo pkgInfo, string payloadPath, string msPackageRoot, CompDBPackageInfo parentPkg, bool generateHash)
		{
			this.SetValues(pkgInfo, payloadPath, msPackageRoot, parentPkg, generateHash);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00005354 File Offset: 0x00003554
		public CompDBPayloadInfo(FeatureManifest.FMPkgInfo pkgInfo, string msPackageRoot, CompDBPackageInfo parentPkg, bool generateHash)
		{
			this.SetValues(pkgInfo, msPackageRoot, parentPkg, generateHash);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00005367 File Offset: 0x00003567
		public CompDBPayloadInfo(string payloadPath, string msPackageRoot, CompDBPackageInfo parentPkg, bool generateHash)
		{
			this.SetValues(payloadPath, msPackageRoot, parentPkg, generateHash);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000537A File Offset: 0x0000357A
		public CompDBPayloadInfo SetParentPkg(CompDBPackageInfo parentPkg)
		{
			this._parentPkg = parentPkg;
			return this;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005384 File Offset: 0x00003584
		public void SetPayloadHash(string payloadFile)
		{
			this.PayloadHash = CompDBPayloadInfo.GetPayloadHash(payloadFile);
			this.PayloadSize = CompDBPayloadInfo.GetPayloadSize(payloadFile);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000539E File Offset: 0x0000359E
		public static string GetPayloadHash(string payloadFile)
		{
			return Convert.ToBase64String(PackageTools.CalculateFileHash(payloadFile));
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000053AC File Offset: 0x000035AC
		public static long GetPayloadSize(string payloadFile)
		{
			long result = 0L;
			try
			{
				result = new FileInfo(payloadFile).Length;
			}
			catch
			{
				result = (long)LongPathFile.ReadAllBytes(payloadFile).Length;
			}
			return result;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000053E8 File Offset: 0x000035E8
		public bool Equals(CompDBPayloadInfo pkg, CompDBPayloadInfo.CompDBPayloadInfoComparison compareType)
		{
			return this.PayloadType == pkg.PayloadType && string.Equals(this.Path, pkg.Path, StringComparison.OrdinalIgnoreCase) && (compareType == CompDBPayloadInfo.CompDBPayloadInfoComparison.IgnorePayloadHash || (string.IsNullOrEmpty(this.PayloadHash) == string.IsNullOrEmpty(pkg.PayloadHash) && (string.IsNullOrEmpty(pkg.PayloadHash) || string.Equals(this.PayloadHash, pkg.PayloadHash))));
		}

		// Token: 0x06000079 RID: 121 RVA: 0x0000545C File Offset: 0x0000365C
		public int GetHashCode(CompDBPayloadInfo.CompDBPayloadInfoComparison compareType)
		{
			int num = this.Path.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
			if (compareType != CompDBPayloadInfo.CompDBPayloadInfoComparison.IgnorePayloadHash && !string.IsNullOrEmpty(this.PayloadHash))
			{
				num ^= this.PayloadHash.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
			}
			return num;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600007A RID: 122 RVA: 0x000054AC File Offset: 0x000036AC
		[XmlIgnore]
		public string ChunkName
		{
			get
			{
				if (string.IsNullOrEmpty(this._chunkName))
				{
					if (BuildCompDB.ChunkMapping != null)
					{
						if (this._chunkMapping == null)
						{
							this._chunkMapping = BuildCompDB.ChunkMapping.FindChunk(this.Path);
						}
						if (this._chunkMapping != null)
						{
							this._chunkName = this._chunkMapping.ChunkName;
						}
					}
					else if (this._isPhone)
					{
						this._chunkName = CompDBPayloadInfo.c_PhoneChunkName;
					}
					else if (!this._isDesktop)
					{
						this._chunkName = CompDBPayloadInfo.c_OnecoreChunkName;
					}
				}
				return this._chunkName;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00005534 File Offset: 0x00003734
		private bool _isPhone
		{
			get
			{
				return this._parentPkg != null && this._parentPkg.ParentDB != null && this._parentPkg.ParentDB.Product.Equals(CompDBPayloadInfo.c_ProductPhone, StringComparison.OrdinalIgnoreCase);
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600007C RID: 124 RVA: 0x0000556B File Offset: 0x0000376B
		private bool _isDesktop
		{
			get
			{
				return this._parentPkg != null && this._parentPkg.ParentDB != null && this._parentPkg.ParentDB.Product.Equals(CompDBPayloadInfo.c_ProductDesktop, StringComparison.OrdinalIgnoreCase);
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600007D RID: 125 RVA: 0x000055A4 File Offset: 0x000037A4
		[XmlIgnore]
		public string ChunkPath
		{
			get
			{
				if (string.IsNullOrEmpty(this._chunkPath))
				{
					if (BuildCompDB.ChunkMapping != null)
					{
						if (this._chunkMapping == null)
						{
							this._chunkMapping = BuildCompDB.ChunkMapping.FindChunk(this.Path);
						}
						if (this._chunkMapping != null && string.IsNullOrEmpty(this._chunkPath))
						{
							this._chunkPath = this.Path.Substring(this._chunkMapping.Path.Length).TrimStart(new char[]
							{
								'\\'
							});
						}
					}
					else if (this._isPhone)
					{
						this._chunkPath = CompDBPayloadInfo.c_Prebuilt + "\\" + this.Path;
					}
					else if (!this._isDesktop)
					{
						return this.Path;
					}
				}
				return this._chunkPath;
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005668 File Offset: 0x00003868
		public CompDBPayloadInfo ClearPayloadHash()
		{
			this.PayloadHash = null;
			this.PayloadSize = 0L;
			return this;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000567A File Offset: 0x0000387A
		public CompDBPayloadInfo SetPath(string path)
		{
			this.Path = path;
			return this;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005684 File Offset: 0x00003884
		public CompDBPayloadInfo SetPreviousPath(string path)
		{
			this.PreviousPath = path;
			return this;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000568E File Offset: 0x0000388E
		public override string ToString()
		{
			return this.Path + " (" + this.PayloadType.ToString() + ")";
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000056B6 File Offset: 0x000038B6
		private void SetValues(IPkgInfo pkgInfo, string payloadPath, string msPackageRoot, CompDBPackageInfo parentPkg, bool generateHash = false)
		{
			this.SetValues(payloadPath, msPackageRoot, parentPkg, generateHash);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000056C4 File Offset: 0x000038C4
		private void SetValues(FeatureManifest.FMPkgInfo pkgInfo, string msPackageRoot, CompDBPackageInfo parentPkg, bool generateHash = false)
		{
			string text = System.IO.Path.ChangeExtension(pkgInfo.PackagePath, PkgConstants.c_strCBSPackageExtension);
			if (LongPathFile.Exists(text))
			{
				Package.LoadFromCab(text);
			}
			else
			{
				if (!LongPathFile.Exists(pkgInfo.PackagePath))
				{
					throw new ImageCommonException(string.Format("ImageCommon::CompDBPayloadInfo!SetValues: Payload file '{0}' could not be found.", pkgInfo.PackagePath));
				}
				Package.LoadFromCab(pkgInfo.PackagePath);
			}
			this.SetValues(pkgInfo.PackagePath, msPackageRoot, parentPkg, generateHash);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005734 File Offset: 0x00003934
		private void SetValues(string payloadPath, string msPackageRoot, CompDBPackageInfo parentPkg, bool generateHash = false)
		{
			char[] trimChars = new char[]
			{
				'\\'
			};
			if (generateHash)
			{
				this.SetPayloadHash(payloadPath);
			}
			if (!string.IsNullOrEmpty(payloadPath))
			{
				this.Path = payloadPath.Replace(msPackageRoot, "", StringComparison.OrdinalIgnoreCase).Trim(trimChars);
			}
			this._parentPkg = parentPkg;
		}

		// Token: 0x04000041 RID: 65
		private static string c_ProductPhone = "Phone";

		// Token: 0x04000042 RID: 66
		private static string c_ProductDesktop = "Desktop";

		// Token: 0x04000043 RID: 67
		private static string c_PhoneChunkName = "MobileDeviceCritBinaries";

		// Token: 0x04000044 RID: 68
		private static string c_OnecoreChunkName = "SPKG";

		// Token: 0x04000045 RID: 69
		private static string c_Prebuilt = "Prebuilt";

		// Token: 0x04000046 RID: 70
		private CompDBPackageInfo _parentPkg;

		// Token: 0x04000047 RID: 71
		[XmlAttribute]
		public string PayloadHash;

		// Token: 0x04000048 RID: 72
		[DefaultValue(0)]
		[XmlAttribute]
		public long PayloadSize;

		// Token: 0x04000049 RID: 73
		[XmlAttribute]
		public string Path;

		// Token: 0x0400004A RID: 74
		[XmlAttribute]
		public string PreviousPath;

		// Token: 0x0400004B RID: 75
		[XmlAttribute]
		public CompDBPayloadInfo.PayloadTypes PayloadType;

		// Token: 0x0400004C RID: 76
		private CompDBChunkMapItem _chunkMapping;

		// Token: 0x0400004D RID: 77
		private string _chunkName;

		// Token: 0x0400004E RID: 78
		private string _chunkPath;

		// Token: 0x0200004C RID: 76
		public enum CompDBPayloadInfoComparison
		{
			// Token: 0x040001D4 RID: 468
			Standard,
			// Token: 0x040001D5 RID: 469
			IgnorePayloadHash
		}

		// Token: 0x0200004D RID: 77
		public enum SatelliteTypes
		{
			// Token: 0x040001D7 RID: 471
			Base,
			// Token: 0x040001D8 RID: 472
			Language,
			// Token: 0x040001D9 RID: 473
			Resolution,
			// Token: 0x040001DA RID: 474
			LangModel
		}

		// Token: 0x0200004E RID: 78
		public enum PayloadTypes
		{
			// Token: 0x040001DC RID: 476
			Canonical,
			// Token: 0x040001DD RID: 477
			Diff,
			// Token: 0x040001DE RID: 478
			ExpressPSF,
			// Token: 0x040001DF RID: 479
			ExpressCab
		}
	}
}
