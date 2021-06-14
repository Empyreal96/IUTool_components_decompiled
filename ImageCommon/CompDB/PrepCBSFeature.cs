using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000015 RID: 21
	public class PrepCBSFeature
	{
		// Token: 0x060000CB RID: 203 RVA: 0x0000A3F8 File Offset: 0x000085F8
		public static void Prep(string sourcePackage, string FMID, string groupName, string groupType, string buildArch, List<FeatureManifest.FMPkgInfo> packages, bool usePhoneSigning)
		{
			string text = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(text);
			string text2 = Path.ChangeExtension(sourcePackage, PkgConstants.c_strPackageExtension);
			string text3 = Path.ChangeExtension(sourcePackage, PkgConstants.c_strCBSPackageExtension);
			bool flag = FileUtils.IsTargetUpToDate(text2, text3);
			if (!flag && !File.Exists(text2) && !File.Exists(text3))
			{
				flag = true;
				text3 = sourcePackage;
			}
			if (flag)
			{
				text = Path.Combine(text, Path.GetFileNameWithoutExtension(text3));
				if (PrepCBSFeature.IsFeatureInfoAlreadyAdded(text3, FMID, groupName, groupType, buildArch, packages, text))
				{
					return;
				}
				CabApiWrapper.Extract(text3, text);
			}
			else
			{
				PkgConvertDSM.ConvertPackagesToCBS(PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_NONE, new List<string>
				{
					text2
				}, text);
				text = Path.Combine(text, Path.GetFileNameWithoutExtension(text2));
			}
			if (packages != null && packages.Count > 0)
			{
				string text4 = Path.Combine(text, PkgConstants.c_strMumFile);
				XDocument xdocument = XDocument.Load(text4);
				XNamespace @namespace = xdocument.Root.Name.Namespace;
				IEnumerable<XElement> source = xdocument.Descendants(@namespace + PrepCBSFeature.c_DeclareCapabilityElement);
				while (source.Any<XElement>())
				{
					source.First<XElement>().Remove();
					source = xdocument.Descendants(@namespace + PrepCBSFeature.c_DeclareCapabilityElement);
				}
				XElement content = PrepCBSFeature.GenFeatureElement(@namespace, FMID, groupName, groupType, buildArch, packages);
				XElement xelement = xdocument.Root.Elements(@namespace + PrepCBSFeature.c_PackageElement).First<XElement>();
				if (xelement.Element(@namespace + PrepCBSFeature.c_CustomInformationElement) != null)
				{
					xelement.Element(@namespace + PrepCBSFeature.c_CustomInformationElement).AddFirst(content);
				}
				else
				{
					xelement.AddFirst(content);
				}
				xdocument.Save(text4);
			}
			string catalogPackageName = PackageTools.GetCatalogPackageName(Package.LoadFromCab(flag ? text3 : text2));
			if (File.Exists(text3))
			{
				File.Delete(text3);
			}
			string text5 = Path.Combine(text, PkgConstants.c_strCBSCatalogFile);
			if (File.Exists(text5))
			{
				File.Delete(text5);
			}
			string[] files = Directory.GetFiles(text, "*.*", SearchOption.AllDirectories);
			PackageTools.CreateTestSignedCatalog(files, files, catalogPackageName, text5);
			Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			if (usePhoneSigning)
			{
				process.StartInfo.Arguments = "/c sign.cmd \"" + text5 + "\"";
			}
			else
			{
				process.StartInfo.Arguments = "/c ntsign.cmd \"" + text5 + "\"";
			}
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.Start();
			string text6 = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				throw new ImageCommonException(string.Format("Error: ImageCommon!PrepCBSFeature: {3} failed to resign {0}.\nErr: {1}\nOutput: {2}", new object[]
				{
					text5,
					process.ExitCode,
					text6,
					usePhoneSigning ? "sign.cmd" : "ntsign.cmd"
				}));
			}
			Console.WriteLine(text6);
			string text7 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(text7);
			CabApiWrapper.CreateCab(text3, text, text7, "*.*", CompressionType.FastLZX);
			if (usePhoneSigning)
			{
				process.StartInfo.Arguments = "/c sign.cmd \"" + sourcePackage + "\"";
			}
			else
			{
				process.StartInfo.Arguments = "/c ntsign.cmd \"" + sourcePackage + "\"";
			}
			process.Start();
			text6 = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				throw new ImageCommonException(string.Format("Error: ImageCommon!PrepCBSFeature: sign.cmd failed to resign {0}.\nErr: {1}\nOutput: {2}", sourcePackage, process.ExitCode, text6));
			}
			Console.WriteLine(text6);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000A7A8 File Offset: 0x000089A8
		public static string GetFeatureInfoXML(string updateMumFile)
		{
			string result = "";
			XDocument xdocument = XDocument.Load(updateMumFile);
			XNamespace @namespace = xdocument.Root.Name.Namespace;
			IEnumerable<XElement> source = xdocument.Descendants(@namespace + PrepCBSFeature.c_DeclareCapabilityElement);
			if (source.Count<XElement>() == 1)
			{
				result = source.First<XElement>().ToString();
			}
			return result;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000A7FC File Offset: 0x000089FC
		public static void ParseFeatureInfoXML(string dcXML, out string fmID, out string groupName, out string groupType, out string buildArch, out List<FeatureManifest.FMPkgInfo> packages)
		{
			XContainer xcontainer = XDocument.Parse(dcXML);
			packages = new List<FeatureManifest.FMPkgInfo>();
			List<string> list = new List<string>();
			fmID = "";
			groupName = "";
			groupType = "";
			buildArch = "";
			foreach (XNode xnode in xcontainer.DescendantNodes())
			{
				XElement xelement = (XElement)xnode;
				if (xelement.Name.LocalName.Equals(PrepCBSFeature.c_CapabilityElement))
				{
					using (IEnumerator<XAttribute> enumerator2 = xelement.Attributes().GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							XAttribute xattribute = enumerator2.Current;
							if (xattribute.Name.LocalName.Equals(PrepCBSFeature.c_FMIDAttribute))
							{
								fmID = xattribute.Value;
							}
							else if (xattribute.Name.LocalName.Equals(PrepCBSFeature.c_GroupAttribute))
							{
								groupType = xattribute.Value;
							}
						}
						continue;
					}
				}
				if (xelement.Name.LocalName.Equals(PrepCBSFeature.c_CapabilityIdentityElement))
				{
					using (IEnumerator<XAttribute> enumerator2 = xelement.Attributes().GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							XAttribute xattribute2 = enumerator2.Current;
							if (xattribute2.Name.LocalName.Equals(PrepCBSFeature.c_NameAttribute))
							{
								groupName = xattribute2.Value;
							}
						}
						continue;
					}
				}
				if (xelement.Name.LocalName.Equals(PrepCBSFeature.c_PackageElement))
				{
					FeatureManifest.FMPkgInfo fmpkgInfo = new FeatureManifest.FMPkgInfo();
					CompDBPackageInfo.SatelliteTypes satelliteTypes = CompDBPackageInfo.SatelliteTypes.Base;
					foreach (XAttribute xattribute3 in xelement.Attributes())
					{
						if (xattribute3.Name.LocalName.Equals(PrepCBSFeature.c_BinaryPartitionAttribute))
						{
							fmpkgInfo.BinaryPartition = true;
						}
						else if (xattribute3.Name.LocalName.Equals(PrepCBSFeature.c_TargetPartitionElement))
						{
							fmpkgInfo.Partition = xattribute3.Value;
						}
						else if (xattribute3.Name.LocalName.Equals(PrepCBSFeature.c_SatelliteTypeAttribute) && !Enum.TryParse<CompDBPackageInfo.SatelliteTypes>(xattribute3.Value, out satelliteTypes))
						{
							satelliteTypes = CompDBPackageInfo.SatelliteTypes.Base;
						}
					}
					foreach (XNode xnode2 in xelement.DescendantNodes())
					{
						XElement xelement2 = (XElement)xnode2;
						if (xelement2.Name.LocalName.Equals(PrepCBSFeature.c_AssemblyIdentityElement))
						{
							foreach (XAttribute xattribute4 in xelement2.Attributes())
							{
								if (xattribute4.Name.LocalName.Equals(PrepCBSFeature.c_NameAttribute))
								{
									fmpkgInfo.ID = xattribute4.Value;
								}
								else if (xattribute4.Name.LocalName.Equals(PrepCBSFeature.c_VersionAttribute))
								{
									fmpkgInfo.Version = new VersionInfo?(VersionInfo.Parse(xattribute4.Value));
								}
								else if (xattribute4.Name.LocalName.Equals(PrepCBSFeature.c_PublicKeyTokenAttribute))
								{
									fmpkgInfo.PublicKey = xattribute4.Value;
								}
								else if (xattribute4.Name.LocalName.Equals(PrepCBSFeature.c_ProcessorArchitectureAttribute))
								{
									list.Add(xattribute4.Value);
								}
							}
						}
					}
					if (satelliteTypes == CompDBPackageInfo.SatelliteTypes.Language)
					{
						int startIndex = fmpkgInfo.ID.IndexOf(PkgFile.DefaultLanguagePattern, StringComparison.OrdinalIgnoreCase) + PkgFile.DefaultLanguagePattern.Length;
						FeatureManifest.FMPkgInfo fmpkgInfo2 = fmpkgInfo;
						fmpkgInfo2.Language = fmpkgInfo2.ID.Substring(startIndex);
					}
					else if (satelliteTypes == CompDBPackageInfo.SatelliteTypes.Resolution)
					{
						int startIndex2 = fmpkgInfo.ID.IndexOf(PkgFile.DefaultResolutionPattern, StringComparison.OrdinalIgnoreCase) + PkgFile.DefaultResolutionPattern.Length;
						FeatureManifest.FMPkgInfo fmpkgInfo3 = fmpkgInfo;
						fmpkgInfo3.Resolution = fmpkgInfo3.ID.Substring(startIndex2);
					}
					packages.Add(fmpkgInfo);
				}
			}
			buildArch = (from pkg in list
			group pkg by pkg into g
			orderby g.Count<string>() descending
			select g.Key).First<string>();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000ACD4 File Offset: 0x00008ED4
		private static bool IsFeatureInfoAlreadyAdded(string sourcePackage, string fmID, string groupName, string groupType, string buildArch, List<FeatureManifest.FMPkgInfo> packages, string extractDir)
		{
			CabApiWrapper.ExtractOne(sourcePackage, extractDir, PkgConstants.c_strMumFile);
			XDocument xdocument = XDocument.Load(Path.Combine(extractDir, PkgConstants.c_strMumFile));
			XNamespace @namespace = xdocument.Root.Name.Namespace;
			IEnumerable<XElement> source = xdocument.Descendants(@namespace + PrepCBSFeature.c_DeclareCapabilityElement);
			if (source.Count<XElement>() != 1)
			{
				return false;
			}
			string value = source.First<XElement>().ToString();
			source.First<XElement>().Remove();
			return PrepCBSFeature.GenFeatureElement(@namespace, fmID, groupName, groupType, buildArch, packages).ToString().Equals(value);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000AD5C File Offset: 0x00008F5C
		private static XElement GenFeatureElement(XNamespace rootNS, string fmID, string groupName, string groupType, string buildArch, List<FeatureManifest.FMPkgInfo> packages)
		{
			XElement xelement = new XElement(rootNS + PrepCBSFeature.c_DeclareCapabilityElement);
			XElement xelement2;
			if (string.IsNullOrEmpty(fmID))
			{
				xelement2 = new XElement(rootNS + PrepCBSFeature.c_CapabilityElement, new object[]
				{
					new XAttribute(PrepCBSFeature.c_GroupAttribute, groupType),
					new XElement(rootNS + PrepCBSFeature.c_CapabilityIdentityElement, new XAttribute(PrepCBSFeature.c_NameAttribute, groupName))
				});
			}
			else
			{
				xelement2 = new XElement(rootNS + PrepCBSFeature.c_CapabilityElement, new object[]
				{
					new XAttribute(PrepCBSFeature.c_GroupAttribute, groupType),
					new XAttribute(PrepCBSFeature.c_FMIDAttribute, fmID),
					new XElement(rootNS + PrepCBSFeature.c_CapabilityIdentityElement, new XAttribute(PrepCBSFeature.c_NameAttribute, groupName))
				});
			}
			XElement xelement3 = new XElement(rootNS + PrepCBSFeature.c_FeaturePackageElement);
			foreach (FeatureManifest.FMPkgInfo fmpkgInfo in packages)
			{
				string text = Path.ChangeExtension(fmpkgInfo.PackagePath, PkgConstants.c_strPackageExtension);
				string text2 = Path.ChangeExtension(fmpkgInfo.PackagePath, PkgConstants.c_strCBSPackageExtension);
				string text3 = FileUtils.IsTargetUpToDate(text, text2) ? text2 : text;
				if (!File.Exists(text3))
				{
					text3 = fmpkgInfo.PackagePath;
				}
				IPkgInfo pkgInfo = Package.LoadFromCab(text3);
				CompDBPackageInfo.SatelliteTypes satelliteTypeFromFMPkgInfo = CompDBPackageInfo.GetSatelliteTypeFromFMPkgInfo(fmpkgInfo, pkgInfo);
				XElement xelement4 = new XElement(rootNS + PrepCBSFeature.c_PackageElement, new XAttribute(PrepCBSFeature.c_SatelliteTypeAttribute, satelliteTypeFromFMPkgInfo));
				if (!string.IsNullOrEmpty(fmpkgInfo.Partition))
				{
					xelement4.Add(new XAttribute(PrepCBSFeature.c_TargetPartitionElement, fmpkgInfo.Partition));
				}
				if (pkgInfo.IsBinaryPartition)
				{
					xelement4.Add(new XAttribute(PrepCBSFeature.c_BinaryPartitionAttribute, "true"));
				}
				string value = CompDBPackageInfo.CpuString(pkgInfo.ComplexCpuType);
				if (string.IsNullOrEmpty(pkgInfo.Culture))
				{
					xelement4.Add(new XElement(rootNS + PrepCBSFeature.c_AssemblyIdentityElement, new object[]
					{
						new XAttribute(PrepCBSFeature.c_NameAttribute, pkgInfo.Name),
						new XAttribute(PrepCBSFeature.c_ProcessorArchitectureAttribute, value),
						new XAttribute(PrepCBSFeature.c_PublicKeyTokenAttribute, pkgInfo.PublicKey),
						new XAttribute(PrepCBSFeature.c_VersionAttribute, pkgInfo.Version.ToString())
					}));
				}
				else
				{
					xelement4.Add(new XElement(rootNS + PrepCBSFeature.c_AssemblyIdentityElement, new object[]
					{
						new XAttribute(PrepCBSFeature.c_LanguageAttribute, pkgInfo.Culture),
						new XAttribute(PrepCBSFeature.c_NameAttribute, pkgInfo.Name),
						new XAttribute(PrepCBSFeature.c_ProcessorArchitectureAttribute, value),
						new XAttribute(PrepCBSFeature.c_PublicKeyTokenAttribute, pkgInfo.PublicKey),
						new XAttribute(PrepCBSFeature.c_VersionAttribute, pkgInfo.Version.ToString())
					}));
				}
				xelement3.Add(xelement4);
			}
			xelement2.Add(xelement3);
			xelement.Add(xelement2);
			return xelement;
		}

		// Token: 0x040000A4 RID: 164
		private static string c_DeclareCapabilityElement = "declareCapability";

		// Token: 0x040000A5 RID: 165
		private static string c_CapabilityIdentityElement = "capabilityIdentity";

		// Token: 0x040000A6 RID: 166
		private static string c_CapabilityElement = "capability";

		// Token: 0x040000A7 RID: 167
		private static string c_AssemblyIdentityElement = "assemblyIdentity";

		// Token: 0x040000A8 RID: 168
		private static string c_PackageElement = "package";

		// Token: 0x040000A9 RID: 169
		private static string c_CustomInformationElement = "customInformation";

		// Token: 0x040000AA RID: 170
		private static string c_FeaturePackageElement = "featurePackage";

		// Token: 0x040000AB RID: 171
		private static string c_TargetPartitionElement = "targetPartition";

		// Token: 0x040000AC RID: 172
		private static string c_BinaryPartitionAttribute = "binaryPartition";

		// Token: 0x040000AD RID: 173
		private static string c_NameAttribute = "name";

		// Token: 0x040000AE RID: 174
		private static string c_ProcessorArchitectureAttribute = "processorArchitecture";

		// Token: 0x040000AF RID: 175
		private static string c_PublicKeyTokenAttribute = "publicKeyToken";

		// Token: 0x040000B0 RID: 176
		private static string c_VersionAttribute = "version";

		// Token: 0x040000B1 RID: 177
		private static string c_LanguageAttribute = "language";

		// Token: 0x040000B2 RID: 178
		private static string c_GroupAttribute = "group";

		// Token: 0x040000B3 RID: 179
		private static string c_FMIDAttribute = "FMID";

		// Token: 0x040000B4 RID: 180
		private static string c_SatelliteTypeAttribute = "satelliteType";
	}
}
