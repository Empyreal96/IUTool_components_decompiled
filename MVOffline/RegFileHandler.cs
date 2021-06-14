using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Multivariant.Offline
{
	// Token: 0x02000002 RID: 2
	internal class RegFileHandler
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static string GetExpandedRegKeyName(string strKeyName, Stream cfgXml)
		{
			if (RegFileHandler.s_macroResolver == null)
			{
				RegFileHandler.s_macroResolver = new MacroResolver();
				using (XmlReader xmlReader = XmlReader.Create(cfgXml))
				{
					RegFileHandler.s_macroResolver.Load(xmlReader);
				}
			}
			string text = RegFileHandler.s_macroResolver.Resolve(strKeyName, MacroResolveOptions.ErrorOnUnknownMacro);
			if (string.Empty == text)
			{
				throw new XmlException("Unexpected registry key name:" + strKeyName + ". Please check that you are using the correct macro.");
			}
			return text;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020D0 File Offset: 0x000002D0
		public RegFileHandler(string tempDir, Stream cfgXml)
		{
			this.tempDirectory = tempDir;
			this.cfgXml = cfgXml;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020F8 File Offset: 0x000002F8
		public void AddRegValue(RegValueInfo regValueInfo)
		{
			regValueInfo.KeyName = RegFileHandler.GetExpandedRegKeyName(regValueInfo.KeyName, this.cfgXml);
			string text = regValueInfo.Partition;
			if (string.IsNullOrEmpty(text))
			{
				text = PkgConstants.c_strMainOsPartition;
			}
			RegFileHandler.RegKeyInfoTable regKeyInfoTable = null;
			if (!this.regKeyTables.TryGetValue(text, out regKeyInfoTable))
			{
				regKeyInfoTable = new RegFileHandler.RegKeyInfoTable(text, this.tempDirectory);
				this.regKeyTables.Add(text, regKeyInfoTable);
			}
			regKeyInfoTable.regValueInfoList.Add(regValueInfo);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000216C File Offset: 0x0000036C
		public List<RegFilePartitionInfo> Write()
		{
			List<RegFilePartitionInfo> result;
			try
			{
				foreach (RegFileHandler.RegKeyInfoTable regKeyInfoTable in this.regKeyTables.Values)
				{
					List<RegValueInfo> regValueInfoList = regKeyInfoTable.regValueInfoList;
					foreach (RegValueInfo regValueInfo in regValueInfoList)
					{
						if (regValueInfo.Type == RegValueType.Binary)
						{
							byte[] data = Convert.FromBase64String(regValueInfo.Value);
							StringBuilder stringBuilder = new StringBuilder();
							RegUtil.ByteArrayToRegString(stringBuilder, data);
							regValueInfo.Value = stringBuilder.ToString();
						}
					}
					RegFileWriter.Write(regValueInfoList, regKeyInfoTable.regFilename);
				}
				result = (from x in this.regKeyTables.Values
				select new RegFilePartitionInfo(x.regFilename, x.partition)).ToList<RegFilePartitionInfo>();
			}
			catch
			{
				this.Delete();
				throw;
			}
			return result;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002290 File Offset: 0x00000490
		public void Delete()
		{
			foreach (RegFileHandler.RegKeyInfoTable regKeyInfoTable in this.regKeyTables.Values)
			{
				File.Delete(regKeyInfoTable.regFilename);
			}
		}

		// Token: 0x04000001 RID: 1
		private Stream cfgXml;

		// Token: 0x04000002 RID: 2
		private string tempDirectory;

		// Token: 0x04000003 RID: 3
		private static MacroResolver s_macroResolver;

		// Token: 0x04000004 RID: 4
		private Dictionary<string, RegFileHandler.RegKeyInfoTable> regKeyTables = new Dictionary<string, RegFileHandler.RegKeyInfoTable>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x0200000D RID: 13
		internal class RegKeyInfoTable
		{
			// Token: 0x0600005D RID: 93 RVA: 0x000038D5 File Offset: 0x00001AD5
			public RegKeyInfoTable(string partitionName, string tempDirectory)
			{
				this.partition = partitionName;
				this.regFilename = FileUtils.GetTempFile(tempDirectory);
				this.regValueInfoList = new List<RegValueInfo>();
			}

			// Token: 0x0400003E RID: 62
			public readonly string regFilename;

			// Token: 0x0400003F RID: 63
			public readonly string partition;

			// Token: 0x04000040 RID: 64
			public List<RegValueInfo> regValueInfoList;
		}
	}
}
