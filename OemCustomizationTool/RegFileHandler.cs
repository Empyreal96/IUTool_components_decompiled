using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x0200000C RID: 12
	internal class RegFileHandler
	{
		// Token: 0x06000039 RID: 57 RVA: 0x00003CD0 File Offset: 0x00001ED0
		private static string FindPartitionFromKeyName(string keyName)
		{
			for (int i = 0; i < RegFileHandler.regKeyToPartionMapping.GetLength(0); i++)
			{
				if (keyName.StartsWith(RegFileHandler.regKeyToPartionMapping[i, 0], StringComparison.OrdinalIgnoreCase))
				{
					return RegFileHandler.regKeyToPartionMapping[i, 1];
				}
			}
			throw new Exception("Should never reach this");
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003D20 File Offset: 0x00001F20
		private static string GetExpandedRegKeyName(string strKeyName)
		{
			if (RegFileHandler.s_macroResolver == null)
			{
				RegFileHandler.s_macroResolver = new MacroResolver();
				using (XmlReader xmlReader = XmlReader.Create(Settings.PkgGenCfgXml))
				{
					RegFileHandler.s_macroResolver.Load(xmlReader);
				}
			}
			string text = RegFileHandler.s_macroResolver.Resolve(strKeyName, MacroResolveOptions.ErrorOnUnknownMacro);
			if (string.Empty == text)
			{
				throw new ConfigXmlException("Unexpected registry key name:" + strKeyName + ". Please check that you are using the correct macro.");
			}
			return text;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003DA4 File Offset: 0x00001FA4
		public void AddRegValue(RegValueInfo regValueInfo, string partitionName)
		{
			regValueInfo.KeyName = RegFileHandler.GetExpandedRegKeyName(regValueInfo.KeyName);
			if (string.IsNullOrEmpty(partitionName))
			{
				partitionName = RegFileHandler.FindPartitionFromKeyName(regValueInfo.KeyName);
			}
			RegFileHandler.RegKeyInfoTable regKeyInfoTable = null;
			if (!this.regKeyTables.TryGetValue(partitionName, out regKeyInfoTable))
			{
				regKeyInfoTable = new RegFileHandler.RegKeyInfoTable(partitionName);
				this.regKeyTables.Add(partitionName, regKeyInfoTable);
			}
			TraceLogger.LogMessage(TraceLevel.Info, string.Format("Added RegKey. KeyName={0}, using file={1}, partition={2}", regValueInfo.KeyName, regKeyInfoTable.regFilename, regKeyInfoTable.partition), true);
			TraceLogger.LogMessage(TraceLevel.Info, string.Format("Added RegValue. Name={0}, val={1}, type={2}", regValueInfo.ValueName, regValueInfo.Value, regValueInfo.Type), true);
			regKeyInfoTable.regValueInfoList.Add(regValueInfo);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003E54 File Offset: 0x00002054
		public List<RegFilePartitionInfo> Write()
		{
			List<RegFilePartitionInfo> result;
			try
			{
				foreach (RegFileHandler.RegKeyInfoTable regKeyInfoTable in this.regKeyTables.Values)
				{
					RegFileWriter.Write(regKeyInfoTable.regValueInfoList, regKeyInfoTable.regFilename);
				}
				result = (from x in this.regKeyTables.Values
				select new RegFilePartitionInfo(x.regFilename, x.partition)).ToList<RegFilePartitionInfo>();
			}
			catch (Exception ex)
			{
				TraceLogger.LogMessage(TraceLevel.Error, "Exception: " + Environment.NewLine + ex.ToString(), true);
				this.Delete();
				result = null;
			}
			return result;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003F24 File Offset: 0x00002124
		public void Delete()
		{
			foreach (RegFileHandler.RegKeyInfoTable regKeyInfoTable in this.regKeyTables.Values)
			{
				File.Delete(regKeyInfoTable.regFilename);
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003F94 File Offset: 0x00002194
		// Note: this type is marked as 'beforefieldinit'.
		static RegFileHandler()
		{
			string[,] array = new string[2, 2];
			array[0, 0] = "HKEY_LOCAL_MACHINE\\BCD";
			array[0, 1] = Settings.PackageAttributes.efiPartitionString;
			array[1, 0] = string.Empty;
			array[1, 1] = Settings.PackageAttributes.mainOSPartitionString;
			RegFileHandler.regKeyToPartionMapping = array;
			RegFileHandler.s_macroResolver = null;
		}

		// Token: 0x04000037 RID: 55
		private static string[,] regKeyToPartionMapping;

		// Token: 0x04000038 RID: 56
		private static MacroResolver s_macroResolver;

		// Token: 0x04000039 RID: 57
		private Dictionary<string, RegFileHandler.RegKeyInfoTable> regKeyTables = new Dictionary<string, RegFileHandler.RegKeyInfoTable>();

		// Token: 0x0200001A RID: 26
		internal class RegKeyInfoTable
		{
			// Token: 0x0600008B RID: 139 RVA: 0x000056C0 File Offset: 0x000038C0
			public RegKeyInfoTable(string partitionName)
			{
				this.partition = partitionName;
				this.regFilename = FileUtils.GetTempFile(Settings.TempDirectoryPath);
				TraceLogger.LogMessage(TraceLevel.Info, string.Format("{0} reg file path is: {1}", this.partition, this.regFilename), true);
				this.regValueInfoList = new List<RegValueInfo>();
			}

			// Token: 0x04000064 RID: 100
			public readonly string regFilename;

			// Token: 0x04000065 RID: 101
			public readonly string partition;

			// Token: 0x04000066 RID: 102
			public List<RegValueInfo> regValueInfoList;
		}
	}
}
