using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000061 RID: 97
	[XmlRoot(ElementName = "Driver", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class DriverPkgObject : OSComponentPkgObject
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060001BA RID: 442 RVA: 0x00006DC7 File Offset: 0x00004FC7
		// (set) Token: 0x060001BB RID: 443 RVA: 0x00006DCF File Offset: 0x00004FCF
		[XmlAttribute("InfSource")]
		public string InfSource { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00006DD8 File Offset: 0x00004FD8
		[XmlElement("Reference")]
		public List<Reference> References { get; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001BD RID: 445 RVA: 0x00006DE0 File Offset: 0x00004FE0
		[XmlElement("Security")]
		public List<Security> InfSecurity { get; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00006DE8 File Offset: 0x00004FE8
		// (set) Token: 0x060001BF RID: 447 RVA: 0x00006DF0 File Offset: 0x00004FF0
		[XmlIgnore]
		internal string Partition { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x00006DF9 File Offset: 0x00004FF9
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x00006E01 File Offset: 0x00005001
		[XmlIgnore]
		private string ProjectFilePath { get; set; }

		// Token: 0x060001C2 RID: 450 RVA: 0x00006E0A File Offset: 0x0000500A
		public DriverPkgObject()
		{
			this.References = new List<Reference>();
			this.InfSecurity = new List<Security>();
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00006E28 File Offset: 0x00005028
		protected override void DoPreprocess(PackageProject proj, IMacroResolver macroResolver)
		{
			this.Partition = proj.Partition;
			this.ProjectFilePath = proj.ProjectFilePath;
			base.DoPreprocess(proj, macroResolver);
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00006E4C File Offset: 0x0000504C
		protected override void DoBuild(IPackageGenerator pkgGen)
		{
			base.DoBuild(pkgGen);
			if (pkgGen.BuildPass != BuildPass.BuildTOC)
			{
				string[] references = null;
				string[] stagingSubdirs = null;
				if (this.References != null)
				{
					references = (from x in this.References
					select pkgGen.MacroResolver.Resolve(x.Source)).ToArray<string>();
					stagingSubdirs = (from x in this.References
					select x.StagingSubDir).ToArray<string>();
				}
				string text = pkgGen.MacroResolver.Resolve(this.InfSource);
				string hiveRoot;
				if (pkgGen.MacroResolver.Resolve("$(__nohives)", MacroResolveOptions.SkipOnUnknownMacro).Equals("true"))
				{
					hiveRoot = "no:hives";
				}
				else
				{
					hiveRoot = pkgGen.MacroResolver.Resolve("$(HIVE_ROOT)");
				}
				string wimRoot = pkgGen.MacroResolver.Resolve("$(WIM_ROOT)", MacroResolveOptions.SkipOnUnknownMacro);
				string productName = pkgGen.MacroResolver.Resolve("$(PRODUCT_NAME)", MacroResolveOptions.SkipOnUnknownMacro);
				string tempDirectory = FileUtils.GetTempDirectory();
				string tempDirectory2 = FileUtils.GetTempDirectory();
				try
				{
					string text2 = text;
					ISecurityPolicyCompiler policyCompiler = pkgGen.PolicyCompiler;
					policyCompiler.DriverSecurityInitialize(this.ProjectFilePath, pkgGen.MacroResolver);
					if (this.InfSecurity != null && this.InfSecurity.Count > 0)
					{
						InfFile infFile = new InfFile(text2);
						infFile.SecurityCompiler = policyCompiler;
						foreach (Security security in this.InfSecurity)
						{
							infFile.UpdateSecurityPolicy(security.InfSectionName);
						}
						text2 = Path.Combine(tempDirectory2, Path.GetFileName(text2));
						infFile.SaveInf(text2);
					}
					InfFileConverter.DoConvert(text2, references, stagingSubdirs, hiveRoot, wimRoot, this.Partition, pkgGen.CPU, tempDirectory, productName, pkgGen.ToolPaths);
					foreach (SystemRegistryHiveFiles hive in new SystemRegistryHiveFiles[]
					{
						SystemRegistryHiveFiles.SYSTEM,
						SystemRegistryHiveFiles.SOFTWARE
					})
					{
						foreach (KeyValuePair<string, InfFileConverter.RegKeyData> keyValuePair in InfFileConverter.ExtractKeys(tempDirectory, hive))
						{
							string key = keyValuePair.Key;
							List<InfFileConverter.RegKeyValue> valueList = keyValuePair.Value.ValueList;
							pkgGen.AddRegKey(key);
							foreach (InfFileConverter.RegKeyValue regKeyValue in valueList)
							{
								if (regKeyValue.IsMultiSz)
								{
									pkgGen.AddRegMultiSzSegment(key, regKeyValue.MultiSzName, regKeyValue.MultiSzValue);
								}
								else
								{
									pkgGen.AddRegValue(key, regKeyValue.Value.Name, regKeyValue.Value.RegValType, regKeyValue.Value.Value);
								}
							}
						}
					}
				}
				catch (IUException ex)
				{
					throw new PkgGenException(ex, ex.Message, new object[0]);
				}
				finally
				{
					FileUtils.DeleteTree(tempDirectory);
					FileUtils.DeleteTree(tempDirectory2);
				}
			}
		}

		// Token: 0x04000156 RID: 342
		private static readonly string STR_SYSTEM_HIVE = Enum.GetName(typeof(SystemRegistryHiveFiles), SystemRegistryHiveFiles.SYSTEM);

		// Token: 0x04000157 RID: 343
		private static readonly string STR_DRIVERS_HIVE = Enum.GetName(typeof(SystemRegistryHiveFiles), SystemRegistryHiveFiles.DRIVERS);

		// Token: 0x04000158 RID: 344
		private static readonly string STR_SOFTWARE_HIVE = Enum.GetName(typeof(SystemRegistryHiveFiles), SystemRegistryHiveFiles.SOFTWARE);
	}
}
