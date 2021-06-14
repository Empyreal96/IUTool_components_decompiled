using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000005 RID: 5
	public sealed class DiffPkgManifest
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00002BCC File Offset: 0x00000DCC
		public DiffPkgManifest()
		{
			this.Name = string.Empty;
			this.SourceHash = new byte[0];
			this.SourceVersion = VersionInfo.Empty;
			this.TargetVersion = VersionInfo.Empty;
			this.m_files = new SortedDictionary<string, DiffFileEntry>(StringComparer.InvariantCultureIgnoreCase);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002C1C File Offset: 0x00000E1C
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002C24 File Offset: 0x00000E24
		public string Name { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002C2D File Offset: 0x00000E2D
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002C35 File Offset: 0x00000E35
		public VersionInfo SourceVersion { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002C3E File Offset: 0x00000E3E
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002C46 File Offset: 0x00000E46
		public VersionInfo TargetVersion { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002C4F File Offset: 0x00000E4F
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002C57 File Offset: 0x00000E57
		public byte[] SourceHash { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002C60 File Offset: 0x00000E60
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00002C74 File Offset: 0x00000E74
		public DiffFileEntry[] Files
		{
			get
			{
				return this.m_files.Values.ToArray<DiffFileEntry>();
			}
			set
			{
				this.m_files.Clear();
				for (int i = 0; i < value.Length; i++)
				{
					DiffFileEntry fe = value[i];
					this.AddFileEntry(fe);
				}
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002CA8 File Offset: 0x00000EA8
		public DiffFileEntry TargetDsmFile
		{
			get
			{
				DiffFileEntry result = null;
				try
				{
					result = this.m_files.Values.First((DiffFileEntry dfe) => dfe.DiffType == DiffType.TargetDSM);
				}
				catch (InvalidOperationException)
				{
				}
				return result;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002D00 File Offset: 0x00000F00
		public void AddFileEntry(DiffFileEntry fe)
		{
			if (fe.DiffType == DiffType.Invalid)
			{
				throw new PackageException("Trying to add uninitialized diff file entry");
			}
			if (this.m_files.ContainsKey(fe.DevicePath))
			{
				throw new PackageException("Multiple file entries with same device path are not allowed in DDSM");
			}
			this.m_files.Add(fe.DevicePath, fe);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002D50 File Offset: 0x00000F50
		public void Save(string diffDsmPath)
		{
			if (string.IsNullOrEmpty(diffDsmPath))
			{
				throw new ArgumentNullException("diffDsmPath", "the path to diff DSM is null or empty");
			}
			this.MakeShortPaths();
			this.Validate();
			IntPtr intPtr = NativeMethods.DiffManifest_Create();
			try
			{
				this.ExportToNativeObject(intPtr);
				NativeMethods.CheckHResult(NativeMethods.DiffManifest_Save(intPtr, diffDsmPath), "DiffManifest_Save");
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					NativeMethods.DiffManifest_Free(intPtr);
				}
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002DC8 File Offset: 0x00000FC8
		public static DiffPkgManifest Load(string diffDsmPath)
		{
			if (string.IsNullOrEmpty(diffDsmPath))
			{
				throw new PackageException("The path to diff DSM is null or empty");
			}
			IntPtr intPtr = NativeMethods.DiffManifest_Create();
			DiffPkgManifest result;
			try
			{
				NativeMethods.CheckHResult(NativeMethods.DiffManifest_Load(intPtr, diffDsmPath), "DiffManifest_Load");
				DiffPkgManifest diffPkgManifest = new DiffPkgManifest();
				diffPkgManifest.ImportFromNativeObject(intPtr);
				diffPkgManifest.Validate();
				result = diffPkgManifest;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					NativeMethods.DiffManifest_Free(intPtr);
				}
			}
			return result;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002E3C File Offset: 0x0000103C
		public static void Load_Diff_CBS(string diffDsmPath, out DiffPkgManifest diffMan, out PkgManifest pkgMan)
		{
			if (string.IsNullOrEmpty(diffDsmPath))
			{
				throw new PackageException("The path to diff DSM is null or empty");
			}
			IntPtr intPtr = NativeMethods.DiffManifest_Create();
			IntPtr intPtr2 = NativeMethods.DeviceSideManifest_Create();
			try
			{
				NativeMethods.CheckHResult(NativeMethods.DiffManifest_Load_XPD(intPtr2, intPtr, diffDsmPath), "DiffManifest_Load_XPD");
				diffMan = new DiffPkgManifest();
				diffMan.ImportFromNativeObject(intPtr);
				pkgMan = new PkgManifest();
				pkgMan.ImportFromNativeObject(intPtr2);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					NativeMethods.DiffManifest_Free(intPtr);
				}
				if (intPtr2 != IntPtr.Zero)
				{
					NativeMethods.DeviceSideManifest_Free(intPtr2);
				}
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002ED4 File Offset: 0x000010D4
		public void BuildSourcePath(string rootDir, BuildPathOption option)
		{
			foreach (DiffFileEntry diffFileEntry in this.m_files.Values)
			{
				if (diffFileEntry.DiffType != DiffType.Remove)
				{
					diffFileEntry.BuildSourcePath(rootDir, option, false);
				}
			}
			this.Validate();
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002F40 File Offset: 0x00001140
		private void Validate()
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				throw new PackageException("Empty Name is not allowed in a diff package");
			}
			if (this.SourceHash == null || this.SourceHash.Length != PkgConstants.c_iHashSize)
			{
				throw new PackageException("Invalid SourceHash size ({0}) for a diff package", new object[]
				{
					(this.SourceHash == null) ? 0 : this.SourceHash.Length
				});
			}
			if (this.SourceVersion == VersionInfo.Empty)
			{
				throw new PackageException("Empty SourceVerison is not allowed in a diff package");
			}
			if (!(this.SourceVersion < this.TargetVersion))
			{
				throw new PackageException("SourceVersion ({0}) must be less than TargetVersion ({1}) in a diff package", new object[]
				{
					this.SourceVersion,
					this.TargetVersion
				});
			}
			DiffFileEntry diffFileEntry = null;
			string text = Path.Combine(PkgConstants.c_strDsmDeviceFolder, this.Name + PkgConstants.c_strDsmExtension);
			if (!this.m_files.TryGetValue(text, out diffFileEntry) || diffFileEntry.DiffType != DiffType.TargetDSM)
			{
				throw new PackageException("DSM file ({0}) not found in the diff file list", new object[]
				{
					text
				});
			}
			foreach (DiffFileEntry diffFileEntry2 in this.m_files.Values)
			{
				diffFileEntry2.Validate();
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003098 File Offset: 0x00001298
		private void MakeShortPaths()
		{
			int num = 0;
			foreach (DiffFileEntry diffFileEntry in this.m_files.Values)
			{
				if (diffFileEntry.DiffType == DiffType.TargetDSM)
				{
					diffFileEntry.CabPath = PkgConstants.c_strDsmFile;
				}
				else
				{
					diffFileEntry.CabPath = PackageTools.MakeShortPath(diffFileEntry.DevicePath, num.ToString());
				}
				num++;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003120 File Offset: 0x00001320
		private void ImportFromNativeObject(IntPtr diffDsmPtr)
		{
			this.Name = NativeMethods.DiffManifest_Get_Name(diffDsmPtr);
			VersionInfo versionInfo = default(VersionInfo);
			NativeMethods.DiffManifest_Get_SourceVersion(diffDsmPtr, ref versionInfo);
			this.SourceVersion = versionInfo;
			versionInfo = default(VersionInfo);
			NativeMethods.DiffManifest_Get_TargetVersion(diffDsmPtr, ref versionInfo);
			this.TargetVersion = versionInfo;
			int num = 0;
			IntPtr source = NativeMethods.DiffManifest_Get_SourceHash(diffDsmPtr, out num);
			if (num != 0)
			{
				byte[] array = new byte[num];
				Marshal.Copy(source, array, 0, num);
				this.SourceHash = array;
			}
			IntPtr zero = IntPtr.Zero;
			int num2 = NativeMethods.DiffManifest_Get_Files(diffDsmPtr, ref zero);
			if (num2 > 0)
			{
				IntPtr[] array2 = new IntPtr[num2];
				Marshal.Copy(zero, array2, 0, num2);
				IntPtr[] array3 = array2;
				for (int i = 0; i < array3.Length; i++)
				{
					DiffFileEntry diffFileEntry = new DiffFileEntry(array3[i]);
					this.m_files.Add(diffFileEntry.DevicePath, diffFileEntry);
				}
				NativeMethods.Helper_Free_Array(zero);
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000031F8 File Offset: 0x000013F8
		private void ExportToNativeObject(IntPtr diffDsmPtr)
		{
			NativeMethods.CheckHResult(NativeMethods.DiffManifest_Set_Name(diffDsmPtr, this.Name), "DiffManifest_Set_Name");
			VersionInfo versionInfo = this.SourceVersion;
			NativeMethods.CheckHResult(NativeMethods.DiffManifest_Set_SourceVersion(diffDsmPtr, ref versionInfo), "DiffManifest_Set_SourceVersion");
			versionInfo = this.TargetVersion;
			NativeMethods.CheckHResult(NativeMethods.DiffManifest_Set_TargetVersion(diffDsmPtr, ref versionInfo), "DiffManifest_Set_TargetVersion");
			NativeMethods.CheckHResult(NativeMethods.DiffManifest_Set_SourceHash(diffDsmPtr, this.SourceHash, this.SourceHash.Length), "DiffManifest_Set_SourceHash");
			foreach (DiffFileEntry diffFileEntry in this.m_files.Values)
			{
				NativeMethods.CheckHResult(NativeMethods.DiffManifest_Add_File(diffDsmPtr, diffFileEntry.FileType, diffFileEntry.DiffType, diffFileEntry.DevicePath, diffFileEntry.CabPath), "DiffManifest_Add_File");
			}
		}

		// Token: 0x04000008 RID: 8
		[XmlIgnore]
		public SortedDictionary<string, DiffFileEntry> m_files;
	}
}
