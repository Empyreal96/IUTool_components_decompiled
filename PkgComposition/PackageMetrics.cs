using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Composition.ToolBox;
using Microsoft.Composition.ToolBox.IO;
using Microsoft.Composition.ToolBox.Logging;

namespace Microsoft.Composition.Packaging
{
	// Token: 0x02000006 RID: 6
	public class PackageMetrics
	{
		// Token: 0x06000078 RID: 120 RVA: 0x00003B4C File Offset: 0x00001D4C
		public PackageMetrics(Logger logger, IntPtr parsemanifestSession, LoadType loadType)
		{
			this.PackageSize = 0UL;
			this.CompressedSize = 0UL;
			this.StagedSize = 0UL;
			this.ParsemanifestSession = parsemanifestSession;
			this.PackageLoadType = loadType;
			if (this.Logger == null)
			{
				this.Logger = new Logger();
			}
			else
			{
				this.Logger = logger;
			}
			foreach (object obj in Enum.GetValues(typeof(ManifestType)))
			{
				ManifestType key = (ManifestType)obj;
				this.processedManifests.Add(key, new HashSet<PkgManifest>());
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00003C50 File Offset: 0x00001E50
		public PackageMetrics(Logger logger, LoadType loadType)
		{
			this.PackageSize = 0UL;
			this.CompressedSize = 0UL;
			this.StagedSize = 0UL;
			this.PackageLoadType = loadType;
			if (this.Logger == null)
			{
				this.Logger = new Logger();
			}
			else
			{
				this.Logger = logger;
			}
			foreach (object obj in Enum.GetValues(typeof(ManifestType)))
			{
				ManifestType key = (ManifestType)obj;
				this.processedManifests.Add(key, new HashSet<PkgManifest>());
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00003D4C File Offset: 0x00001F4C
		~PackageMetrics()
		{
			if (!IntPtr.Zero.Equals(this.ParsemanifestSession))
			{
				NativeMethods.ManagedFreeSession(this.ParsemanifestSession);
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00003DA0 File Offset: 0x00001FA0
		// (set) Token: 0x0600007C RID: 124 RVA: 0x00003DA8 File Offset: 0x00001FA8
		public IntPtr ParsemanifestSession { get; private set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00003DB1 File Offset: 0x00001FB1
		// (set) Token: 0x0600007E RID: 126 RVA: 0x00003DB9 File Offset: 0x00001FB9
		public LoadType PackageLoadType { get; private set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00003DC2 File Offset: 0x00001FC2
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00003DCA File Offset: 0x00001FCA
		public Logger Logger { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00003DD3 File Offset: 0x00001FD3
		// (set) Token: 0x06000082 RID: 130 RVA: 0x00003DDB File Offset: 0x00001FDB
		public PackageType PackageType { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00003DE4 File Offset: 0x00001FE4
		public HashSet<string> SignInfoFiles
		{
			get
			{
				return this._signInfoFiles;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00003DEC File Offset: 0x00001FEC
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00003DF4 File Offset: 0x00001FF4
		public int ProcessedManifestCount { get; private set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00003DFD File Offset: 0x00001FFD
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00003E05 File Offset: 0x00002005
		public int ProcessedFileCount { get; private set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00003E0E File Offset: 0x0000200E
		// (set) Token: 0x06000089 RID: 137 RVA: 0x00003E16 File Offset: 0x00002016
		public ulong PackageSize { get; private set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00003E1F File Offset: 0x0000201F
		// (set) Token: 0x0600008B RID: 139 RVA: 0x00003E27 File Offset: 0x00002027
		public ulong CompressedSize { get; private set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00003E30 File Offset: 0x00002030
		// (set) Token: 0x0600008D RID: 141 RVA: 0x00003E38 File Offset: 0x00002038
		public ulong StagedSize { get; private set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00003E41 File Offset: 0x00002041
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00003E49 File Offset: 0x00002049
		public TimeSpan ProcessTime { get; private set; }

		// Token: 0x06000090 RID: 144 RVA: 0x00003E54 File Offset: 0x00002054
		public void ProcessPayload(string sourcePath, PkgFile file)
		{
			if (!this.payloadSet.ContainsKey(sourcePath))
			{
				this.payloadSet.Add(sourcePath, file);
				int processedFileCount = this.ProcessedFileCount;
				this.ProcessedFileCount = processedFileCount + 1;
				this.PackageSize += (ulong)file.Size;
				this.CompressedSize += (ulong)file.CompressedSize;
				this.StagedSize += (ulong)file.StagedSize;
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00003EC8 File Offset: 0x000020C8
		public void ProcessManifest(string sourcePath, PkgManifest manifest)
		{
			if (!this.manifestSet.ContainsKey(sourcePath))
			{
				this.manifestSet.Add(sourcePath, manifest);
				if (!this.processedManifests.ContainsKey(manifest.ManifestType))
				{
					this.processedManifests[manifest.ManifestType] = new HashSet<PkgManifest>();
				}
				this.processedManifests[manifest.ManifestType].Add(manifest);
				int num = this.ProcessedManifestCount;
				this.ProcessedManifestCount = num + 1;
				num = this.ProcessedFileCount;
				this.ProcessedFileCount = num + 1;
				ulong num2 = (ulong)FileToolBox.Size(sourcePath);
				this.PackageSize += num2;
				this.CompressedSize += num2;
				this.StagedSize += num2;
				return;
			}
			this.duplicates.Add(manifest);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00003F93 File Offset: 0x00002193
		public PkgManifest GetProcessedManifest(string sourcePath)
		{
			if (this.manifestSet.ContainsKey(sourcePath))
			{
				this.duplicates.Add(this.manifestSet[sourcePath]);
				return this.manifestSet[sourcePath];
			}
			return null;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00003FC9 File Offset: 0x000021C9
		public PkgFile GetProcessedPayload(string sourcePath)
		{
			if (this.payloadSet.ContainsKey(sourcePath))
			{
				return this.payloadSet[sourcePath];
			}
			return null;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00003FE7 File Offset: 0x000021E7
		public Dictionary<ManifestType, HashSet<PkgManifest>> GetAllProcessedManifest()
		{
			return this.processedManifests;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00003FEF File Offset: 0x000021EF
		public void AddNoMergePackage(PkgManifest manifest)
		{
			this.noMergeManifests.Add(manifest);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003FFE File Offset: 0x000021FE
		public void StartOperation()
		{
			this.operationTime.Restart();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000400B File Offset: 0x0000220B
		public void StopOperation()
		{
			this.ProcessTime = this.operationTime.Elapsed;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000401E File Offset: 0x0000221E
		public string FormattedSize()
		{
			return this.FormattedBytes(this.PackageSize);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000402C File Offset: 0x0000222C
		public string FormattedCompressedSize()
		{
			return this.FormattedBytes(this.CompressedSize);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000403A File Offset: 0x0000223A
		public string FormattedStagedSize()
		{
			return this.FormattedBytes(this.StagedSize);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00004048 File Offset: 0x00002248
		public void LogMetrics()
		{
			this.Logger.LogInfo("\nLoad Time       : {0}", new object[]
			{
				this.ProcessTime
			});
			this.Logger.LogInfo("Processed Manifest: {0}", new object[]
			{
				this.ProcessedManifestCount
			});
			this.Logger.LogInfo("Overlap           : {0}", new object[]
			{
				this.duplicates.Count
			});
			this.Logger.LogInfo("Total File Count  : {0}", new object[]
			{
				this.ProcessedFileCount
			});
			this.Logger.LogInfo("Package Size      : {0}", new object[]
			{
				this.FormattedSize()
			});
			this.Logger.LogInfo("Compressed Size   : {0}", new object[]
			{
				this.FormattedCompressedSize()
			});
			this.Logger.LogInfo("Staged Size       : {0}", new object[]
			{
				this.FormattedStagedSize()
			});
			foreach (KeyValuePair<ManifestType, HashSet<PkgManifest>> keyValuePair in this.processedManifests)
			{
				this.Logger.LogInfo("{0} Count{2}: {1}", new object[]
				{
					char.ToUpper(keyValuePair.Key.ToString()[0], CultureInfo.InvariantCulture).ToString() + keyValuePair.Key.ToString().Substring(1),
					keyValuePair.Value.Count,
					new string(' ', 12 - keyValuePair.Key.ToString().Length)
				});
			}
			if (this.processedManifests.ContainsKey(ManifestType.Package))
			{
				this.Logger.LogInfo("Catalog Count     : {0}", new object[]
				{
					this.processedManifests[ManifestType.Package].Count
				});
			}
			this.Logger.LogInfo("Validation Time   : {0}", new object[]
			{
				this.ProcessTime
			});
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004294 File Offset: 0x00002494
		public void LogCondenseMetrics()
		{
			this.Logger.LogInfo("\nCondense Time     : {0}", new object[]
			{
				this.ProcessTime
			});
			this.Logger.LogInfo("Non merged manifests     : {0}", new object[]
			{
				this.noMergeManifests.Count
			});
			foreach (KeyValuePair<ManifestType, HashSet<PkgManifest>> keyValuePair in this.processedManifests)
			{
				this.Logger.LogInfo("{0} Removed{2}: {1}", new object[]
				{
					char.ToUpper(keyValuePair.Key.ToString()[0], CultureInfo.InvariantCulture).ToString() + keyValuePair.Key.ToString().Substring(1),
					keyValuePair.Value.Count,
					new string(' ', 10 - keyValuePair.Key.ToString().Length)
				});
			}
			if (this.processedManifests.ContainsKey(ManifestType.Package))
			{
				this.Logger.LogInfo("Catalogs Removed  : {0}", new object[]
				{
					this.processedManifests[ManifestType.Package].Count
				});
			}
			this.Logger.LogInfo("Reduced Size by   : {0}", new object[]
			{
				this.FormattedSize()
			});
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004438 File Offset: 0x00002638
		public void ClearMetrics()
		{
			this.manifestSet.Clear();
			this.ProcessedManifestCount = 0;
			this.ProcessedFileCount = 0;
			this.duplicates.Clear();
			this.PackageSize = 0UL;
			this.CompressedSize = 0UL;
			this.StagedSize = 0UL;
			this.noMergeManifests.Clear();
			foreach (object obj in Enum.GetValues(typeof(ManifestType)))
			{
				ManifestType key = (ManifestType)obj;
				this.processedManifests[key] = new HashSet<PkgManifest>();
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000044EC File Offset: 0x000026EC
		private string FormattedBytes(ulong packageSize)
		{
			string[] array = new string[]
			{
				"B",
				"KB",
				"MB",
				"GB",
				"TB",
				"PB"
			};
			double num = packageSize;
			int num2 = 0;
			while (num >= 1024.0)
			{
				num /= 1024.0;
				num2++;
			}
			return string.Format("{0:0.##} {1}", num, array[num2]);
		}

		// Token: 0x04000010 RID: 16
		private Stopwatch operationTime = new Stopwatch();

		// Token: 0x04000011 RID: 17
		private Dictionary<string, PkgManifest> manifestSet = new Dictionary<string, PkgManifest>();

		// Token: 0x04000012 RID: 18
		private Dictionary<string, PkgFile> payloadSet = new Dictionary<string, PkgFile>();

		// Token: 0x04000013 RID: 19
		private Dictionary<ManifestType, HashSet<PkgManifest>> processedManifests = new Dictionary<ManifestType, HashSet<PkgManifest>>();

		// Token: 0x04000014 RID: 20
		private HashSet<PkgManifest> duplicates = new HashSet<PkgManifest>();

		// Token: 0x04000015 RID: 21
		private HashSet<PkgManifest> noMergeManifests = new HashSet<PkgManifest>();

		// Token: 0x0400001A RID: 26
		private HashSet<string> _signInfoFiles = new HashSet<string>();
	}
}
