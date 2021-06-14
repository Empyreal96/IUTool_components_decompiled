using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces
{
	// Token: 0x02000022 RID: 34
	public interface IPackageGenerator
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600006D RID: 109
		ISecurityPolicyCompiler PolicyCompiler { get; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600006E RID: 110
		XmlValidator XmlValidator { get; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600006F RID: 111
		BuildPass BuildPass { get; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000070 RID: 112
		CpuId CPU { get; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000071 RID: 113
		IMacroResolver MacroResolver { get; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000072 RID: 114
		string TempDirectory { get; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000073 RID: 115
		string ToolPaths { get; }

		// Token: 0x06000074 RID: 116
		IEnumerable<SatelliteId> GetSatelliteValues(SatelliteType type);

		// Token: 0x06000075 RID: 117
		void AddRegMultiSzSegment(string keyName, string valueName, params string[] valueSegments);

		// Token: 0x06000076 RID: 118
		void AddRegValue(string keyName, string valueName, RegValueType valueType, string value, SatelliteId satelliteId);

		// Token: 0x06000077 RID: 119
		void AddRegValue(string keyName, string valueName, RegValueType valueType, string value);

		// Token: 0x06000078 RID: 120
		void AddRegExpandValue(string keyName, string valueName, string value);

		// Token: 0x06000079 RID: 121
		void AddRegKey(string keyName, SatelliteId satelliteId);

		// Token: 0x0600007A RID: 122
		void AddRegKey(string keyName);

		// Token: 0x0600007B RID: 123
		void AddFile(string sourcePath, string devicePath, FileAttributes attributes, SatelliteId satelliteId, string embedSignCategory = "None");

		// Token: 0x0600007C RID: 124
		void AddFile(string sourcePath, string devicePath, FileAttributes attributes);

		// Token: 0x0600007D RID: 125
		void AddCertificate(string sourcePath);

		// Token: 0x0600007E RID: 126
		void AddBinaryPartition(string sourcePath);

		// Token: 0x0600007F RID: 127
		void AddBCDStore(string sourcePath);

		// Token: 0x06000080 RID: 128
		RegGroup ImportRegistry(string sourcePath);

		// Token: 0x06000081 RID: 129
		void Build(string projPath, string outputDir, bool compress);
	}
}
