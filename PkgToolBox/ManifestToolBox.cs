using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Composition.ToolBox.IO;

namespace Microsoft.Composition.ToolBox
{
	// Token: 0x0200000B RID: 11
	public class ManifestToolBox
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002070 File Offset: 0x00000270
		public static CpuArch GetHostType(string cpuStr)
		{
			if (cpuStr.Contains('.'))
			{
				return (CpuArch)Enum.Parse(typeof(CpuArch), cpuStr.Substring(0, cpuStr.IndexOf('.')), true);
			}
			return (CpuArch)Enum.Parse(typeof(CpuArch), cpuStr, true);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020C4 File Offset: 0x000002C4
		public static CpuArch GetGuestType(string cpuStr)
		{
			if (cpuStr.Contains('.'))
			{
				return (CpuArch)Enum.Parse(typeof(CpuArch), cpuStr.Substring(cpuStr.IndexOf('.') + 1), true);
			}
			if (cpuStr.ToLower(CultureInfo.InvariantCulture).Equals(CpuArch.WOW64.ToString(), StringComparison.InvariantCultureIgnoreCase))
			{
				return CpuArch.X86;
			}
			return CpuArch.Invalid;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002128 File Offset: 0x00000328
		public static string GetCpuString(Keyform keyform)
		{
			if (keyform.HostArch == CpuArch.WOW64)
			{
				return keyform.HostArch.ToString().ToLower(CultureInfo.InvariantCulture);
			}
			if (keyform.GuestArch != CpuArch.Invalid)
			{
				return string.Format("{0}.{1}", keyform.HostArch.ToString().ToLower(CultureInfo.InvariantCulture), keyform.GuestArch.ToString().ToLower(CultureInfo.InvariantCulture));
			}
			return keyform.HostArch.ToString().ToLower(CultureInfo.InvariantCulture);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021CC File Offset: 0x000003CC
		public static string GetCpuString(CpuArch hostArch, CpuArch guestArch)
		{
			if (hostArch == CpuArch.WOW64)
			{
				return hostArch.ToString().ToLower(CultureInfo.InvariantCulture);
			}
			if (guestArch != CpuArch.Invalid)
			{
				return string.Format("{0}.{1}", hostArch.ToString().ToLower(CultureInfo.InvariantCulture), guestArch.ToString().ToLower(CultureInfo.InvariantCulture));
			}
			return hostArch.ToString().ToLower(CultureInfo.InvariantCulture);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002248 File Offset: 0x00000448
		public static ManifestType GetManifestType(string path, XElement root)
		{
			if (string.Equals(Path.GetExtension(path), ".mum", StringComparison.InvariantCultureIgnoreCase))
			{
				return ManifestType.Package;
			}
			try
			{
				if (root.Element(root.Name.Namespace + "deployment") != null)
				{
					if (root.Element(root.Name.Namespace + "file") == null)
					{
						return ManifestType.Deployment;
					}
					return ManifestType.Driver;
				}
				else if (root.Element(root.Name.Namespace + "assemblyIdentity").Attribute("name").Value.Contains("Client-Features-Package-AutoMerged"))
				{
					return ManifestType.Deployment;
				}
			}
			catch
			{
				return ManifestType.Invalid;
			}
			return ManifestType.Component;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002308 File Offset: 0x00000508
		public static XDocument Load(string sourceXML)
		{
			if (!File.Exists(sourceXML))
			{
				throw new Exception("ManifestToolBox::Load: SafeLoadXDocument couldn't find file " + sourceXML);
			}
			XDocument result = null;
			int i = ManifestToolBox.IntRetryCount;
			while (i > 0)
			{
				try
				{
					using (FileStream fileStream = FileToolBox.Stream(sourceXML, FileAccess.Read))
					{
						result = XDocument.Load(fileStream);
					}
					break;
				}
				catch (Exception innerException)
				{
					i--;
					if (i <= 0)
					{
						throw new Exception("ManifestToolBox::Load: SafeLoadXDocument couldn't load the file " + sourceXML, innerException);
					}
					Thread.Sleep(ManifestToolBox.IntSleepTimeMs);
				}
			}
			return result;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000023A0 File Offset: 0x000005A0
		public static void Save(XDocument doc, string path)
		{
			string tempFileName = Path.GetTempFileName();
			doc.Save(tempFileName);
			if (!FileToolBox.Exists(tempFileName))
			{
				throw new FileNotFoundException(string.Format("ManifestToolBox::Save: Unable to save XDocument to a temp location: '{0}'", tempFileName));
			}
			if (FileToolBox.Exists(path))
			{
				FileToolBox.Delete(path);
			}
			ManifestToolBox.RetryCopy(tempFileName, path);
			ManifestToolBox.SafeRetryCleanup(tempFileName);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000023F0 File Offset: 0x000005F0
		public static void SafeRetryCleanup(string fileToRemove)
		{
			for (int i = 0; i < ManifestToolBox.IntRetryCount; i++)
			{
				try
				{
					FileToolBox.Delete(fileToRemove);
					break;
				}
				catch
				{
				}
				Thread.Sleep(ManifestToolBox.IntSleepTimeMs);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002434 File Offset: 0x00000634
		public static void RetryCopy(string source, string target)
		{
			FileToolBox.Copy(source, target, true);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002440 File Offset: 0x00000640
		public static BuildType ConvertBuildType(string pkgCommonBuildTypeStr)
		{
			BuildType result = BuildType.Invalid;
			if (pkgCommonBuildTypeStr.Equals("Retail", StringComparison.OrdinalIgnoreCase))
			{
				result = BuildType.Release;
			}
			else if (pkgCommonBuildTypeStr.Equals("Checked", StringComparison.OrdinalIgnoreCase) || pkgCommonBuildTypeStr.Equals("Debug", StringComparison.OrdinalIgnoreCase))
			{
				result = BuildType.Debug;
			}
			return result;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002480 File Offset: 0x00000680
		public static CpuArch ConvertCpuIdToCpuArch(string pkgCommonCpuIdStr)
		{
			CpuArch result = CpuArch.Invalid;
			if (!Enum.TryParse<CpuArch>(pkgCommonCpuIdStr, out result))
			{
				result = CpuArch.Invalid;
			}
			return result;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000249C File Offset: 0x0000069C
		public static PhoneOwnerType ConvertOwnerType(string pkgCommonOwnerTypeStr)
		{
			PhoneOwnerType result = PhoneOwnerType.Invalid;
			if (!Enum.TryParse<PhoneOwnerType>(pkgCommonOwnerTypeStr, out result))
			{
				result = PhoneOwnerType.Invalid;
			}
			return result;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000024B8 File Offset: 0x000006B8
		public static FileType ConvertFileType(string pkgCommonFileTypeStr)
		{
			FileType result = FileType.Invalid;
			if (!Enum.TryParse<FileType>(pkgCommonFileTypeStr, out result))
			{
				result = FileType.Invalid;
			}
			return result;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000024D4 File Offset: 0x000006D4
		public static PhoneReleaseType ConvertReleaseType(string pkgCommonReleaseTypeStr)
		{
			PhoneReleaseType result = PhoneReleaseType.Invalid;
			if (!Enum.TryParse<PhoneReleaseType>(pkgCommonReleaseTypeStr, out result))
			{
				result = PhoneReleaseType.Invalid;
			}
			return result;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000024F0 File Offset: 0x000006F0
		public static string GetSatelliteType(string culture, string ownerType, string packageName, string featureId, string pkgGroup)
		{
			if (!string.IsNullOrEmpty(pkgGroup) && (pkgGroup.Equals("KEYBOARD", StringComparison.OrdinalIgnoreCase) || pkgGroup.Equals("SPEECH", StringComparison.OrdinalIgnoreCase)))
			{
				return "LangModel";
			}
			if (!culture.Equals(PkgConstants.NeutralCulture, StringComparison.InvariantCultureIgnoreCase) && ownerType.Equals(PhoneOwnerType.Microsoft.ToString(), StringComparison.InvariantCultureIgnoreCase) && (featureId.StartsWith("MS_SPEECHSYSTEM_", StringComparison.OrdinalIgnoreCase) || featureId.StartsWith("MS_SPEECHDATA_", StringComparison.OrdinalIgnoreCase)))
			{
				return "LangModel";
			}
			if (!culture.Equals(PkgConstants.NeutralCulture, StringComparison.InvariantCultureIgnoreCase))
			{
				return "Language";
			}
			if (!string.IsNullOrEmpty(ManifestToolBox.GetResolution(packageName)))
			{
				return "Resolution";
			}
			return "Base";
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000025A0 File Offset: 0x000007A0
		public static string GetResolution(string pkgName)
		{
			int num = pkgName.IndexOf("_RES_", StringComparison.InvariantCultureIgnoreCase);
			if (num < 0)
			{
				return string.Empty;
			}
			return pkgName.Substring(num + "_RES_".Count<char>());
		}

		// Token: 0x04000029 RID: 41
		public static readonly int IntRetryCount = 20;

		// Token: 0x0400002A RID: 42
		public static readonly int IntSleepTimeMs = 100;
	}
}
