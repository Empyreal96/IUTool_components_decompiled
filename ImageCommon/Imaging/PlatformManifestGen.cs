using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SecureBoot;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000036 RID: 54
	public class PlatformManifestGen
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000233 RID: 563 RVA: 0x00013978 File Offset: 0x00011B78
		public bool ErrorsFound
		{
			get
			{
				return this.ErrorMessages.Length > 0;
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00013988 File Offset: 0x00011B88
		public PlatformManifestGen(Guid featureManifestID, string buildBranchInfo, string signInfoPath, ReleaseType releaseType, IULogger logger)
		{
			this._logger = logger;
			this._signinfoPath = signInfoPath;
			this._pmAPI = new PlatformManifest(featureManifestID, buildBranchInfo);
			this._pmAPI.ImageType = ((releaseType == ReleaseType.Production) ? PlatformManifest.ImageReleaseType.Retail : PlatformManifest.ImageReleaseType.Test);
			if (!string.IsNullOrWhiteSpace(this._signinfoPath) && Directory.Exists(this._signinfoPath))
			{
				this._doSignInfo = true;
			}
			if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(PlatformManifestGen.c_strSignInfoEnabledEnvVar)))
			{
				if (!this._doSignInfo)
				{
					throw new ImageCommonException("ImageCommon!PlatformManifestGen::PlatformManifestGen: The SignInfo Path does not exist '" + this._signinfoPath + "' but is required.");
				}
			}
			else
			{
				this._doSignInfo = false;
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00013A34 File Offset: 0x00011C34
		public void AddPackages(List<IPkgInfo> packages)
		{
			foreach (IPkgInfo package in packages)
			{
				this.AddPackage(package);
			}
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00013A84 File Offset: 0x00011C84
		public void AddPackage(IPkgInfo package)
		{
			this._pmAPI.AddStringEntry(package.Name);
			if (this._doSignInfo)
			{
				foreach (IFileEntry fileEntry in package.Files)
				{
					if (fileEntry.SignInfoRequired)
					{
						byte[] array = PlatformManifestGen.HexStringToByteArray(fileEntry.FileHash);
						if (array.Count<byte>() == 0)
						{
							this._logger.LogWarning("Warning: ImageCommon!PlatformManifestGen::AddPackage: File '{0}' in package '{1}' requires signInfo but has empty fileHash!", new object[]
							{
								fileEntry.DevicePath,
								package.Name
							});
						}
						else
						{
							try
							{
								this._pmAPI.AddBinaryFromFullFileHash(array);
							}
							catch (Exception ex)
							{
								this._logger.LogWarning("Warning: ImageCommon!PlatformManifestGen::AddPackage: File '{0}' in package '{1}' requires signInfo but failed to load with error: {2}", new object[]
								{
									fileEntry.DevicePath,
									package.Name,
									ex.Message
								});
								this.ErrorsWithSignInfos = true;
								string text = Convert.ToBase64String(array);
								this.ErrorMessages.AppendLine(string.Concat(new string[]
								{
									"Error: File '",
									fileEntry.DevicePath,
									"' in package '",
									package.Name,
									"' failed to find any SignInfo files using the following hash: ",
									text
								}));
							}
						}
					}
				}
			}
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00013BF8 File Offset: 0x00011DF8
		public static byte[] HexStringToByteArray(string hex)
		{
			return (from x in Enumerable.Range(0, hex.Length)
			where x % 2 == 0
			select Convert.ToByte(hex.Substring(x, 2), 16)).ToArray<byte>();
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00013C60 File Offset: 0x00011E60
		public void WriteToFile(string outputFile)
		{
			try
			{
				this._pmAPI.WriteToFile(outputFile);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon!PlatformManifestGen::WriteToFile: Failed to write file with error:", innerException);
			}
		}

		// Token: 0x0400017B RID: 379
		private IULogger _logger;

		// Token: 0x0400017C RID: 380
		private string _signinfoPath;

		// Token: 0x0400017D RID: 381
		private bool _doSignInfo;

		// Token: 0x0400017E RID: 382
		private PlatformManifest _pmAPI;

		// Token: 0x0400017F RID: 383
		public static string c_strSignInfoExtension = ".signinfo";

		// Token: 0x04000180 RID: 384
		public static string c_strSignInfoDir = "SignInfo";

		// Token: 0x04000181 RID: 385
		public static string c_strPlatformManifestMainOSDevicePath = "\\Windows\\System32\\PlatformManifest\\";

		// Token: 0x04000182 RID: 386
		public static string c_strPlatformManifestEFIESPDevicePath = "\\EFI\\Microsoft\\Boot\\PlatformManifest\\";

		// Token: 0x04000183 RID: 387
		public static string c_strPlatformManifestSubcomponent = "PlatformManifest";

		// Token: 0x04000184 RID: 388
		public static string c_strPlatformManifestExtension = ".pm";

		// Token: 0x04000185 RID: 389
		public static string c_strSignInfoEnabledEnvVar = "GENERATE_SIGNINFO_FILES";

		// Token: 0x04000186 RID: 390
		public StringBuilder ErrorMessages = new StringBuilder();

		// Token: 0x04000187 RID: 391
		public bool ErrorsWithSignInfos;

		// Token: 0x04000188 RID: 392
		public static string SignInfoFailureInstructions = "ImageCommon:PlatformManifestGen: Missing\\Duplicate signinfo errors can be troubleshooted by following instructions at: https://osgwiki.com/wiki/Signinfo_for_Resign_Removal ";
	}
}
