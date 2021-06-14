using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000018 RID: 24
	public class PkgManifest
	{
		// Token: 0x0600011D RID: 285 RVA: 0x00006B70 File Offset: 0x00004D70
		public PkgManifest()
		{
			this.Owner = string.Empty;
			this.Component = string.Empty;
			this.SubComponent = string.Empty;
			this.Version = VersionInfo.Empty;
			this.Culture = string.Empty;
			this.Resolution = string.Empty;
			this.Partition = string.Empty;
			this.Platform = string.Empty;
			this.GroupingKey = string.Empty;
			this.FeatureType = string.Empty;
			this.BuildString = string.Empty;
			this.TargetGroups = new string[0];
			this.IsBinaryPartition = false;
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600011E RID: 286 RVA: 0x00006C1F File Offset: 0x00004E1F
		// (set) Token: 0x0600011F RID: 287 RVA: 0x00006C27 File Offset: 0x00004E27
		public ReleaseType ReleaseType { get; set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000120 RID: 288 RVA: 0x00006C30 File Offset: 0x00004E30
		// (set) Token: 0x06000121 RID: 289 RVA: 0x00006C38 File Offset: 0x00004E38
		public PackageStyle PackageStyle { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00006C41 File Offset: 0x00004E41
		// (set) Token: 0x06000123 RID: 291 RVA: 0x00006C49 File Offset: 0x00004E49
		public OwnerType OwnerType { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00006C52 File Offset: 0x00004E52
		// (set) Token: 0x06000125 RID: 293 RVA: 0x00006C5A File Offset: 0x00004E5A
		public BuildType BuildType { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00006C63 File Offset: 0x00004E63
		// (set) Token: 0x06000127 RID: 295 RVA: 0x00006C6B File Offset: 0x00004E6B
		public CpuId CpuType { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00006C74 File Offset: 0x00004E74
		// (set) Token: 0x06000129 RID: 297 RVA: 0x00006C7C File Offset: 0x00004E7C
		public string FeatureType { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00006C85 File Offset: 0x00004E85
		// (set) Token: 0x0600012B RID: 299 RVA: 0x00006C8D File Offset: 0x00004E8D
		public string Culture { get; set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00006C96 File Offset: 0x00004E96
		// (set) Token: 0x0600012D RID: 301 RVA: 0x00006C9E File Offset: 0x00004E9E
		public string Resolution { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00006CA7 File Offset: 0x00004EA7
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00006CAF File Offset: 0x00004EAF
		public string PublicKey { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00006CB8 File Offset: 0x00004EB8
		public string Name
		{
			get
			{
				if (this.PackageStyle == PackageStyle.CBS)
				{
					return this.PackageName;
				}
				if (string.IsNullOrEmpty(this.Owner) || string.IsNullOrEmpty(this.Component))
				{
					return string.Empty;
				}
				return PackageTools.BuildPackageName(this.Owner, this.Component, this.SubComponent, this.Culture, this.Resolution);
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00006D18 File Offset: 0x00004F18
		// (set) Token: 0x06000132 RID: 306 RVA: 0x00006D20 File Offset: 0x00004F20
		public string PackageName { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00006D29 File Offset: 0x00004F29
		// (set) Token: 0x06000134 RID: 308 RVA: 0x00006D31 File Offset: 0x00004F31
		public string Owner { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00006D3A File Offset: 0x00004F3A
		// (set) Token: 0x06000136 RID: 310 RVA: 0x00006D42 File Offset: 0x00004F42
		public string Component { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000137 RID: 311 RVA: 0x00006D4B File Offset: 0x00004F4B
		// (set) Token: 0x06000138 RID: 312 RVA: 0x00006D53 File Offset: 0x00004F53
		public string SubComponent { get; set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00006D5C File Offset: 0x00004F5C
		// (set) Token: 0x0600013A RID: 314 RVA: 0x00006D64 File Offset: 0x00004F64
		public VersionInfo Version { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00006D6D File Offset: 0x00004F6D
		// (set) Token: 0x0600013C RID: 316 RVA: 0x00006D75 File Offset: 0x00004F75
		public string Partition { get; set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00006D7E File Offset: 0x00004F7E
		// (set) Token: 0x0600013E RID: 318 RVA: 0x00006D86 File Offset: 0x00004F86
		public string Platform { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600013F RID: 319 RVA: 0x00006D8F File Offset: 0x00004F8F
		// (set) Token: 0x06000140 RID: 320 RVA: 0x00006D97 File Offset: 0x00004F97
		public bool IsRemoval { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000141 RID: 321 RVA: 0x00006DA0 File Offset: 0x00004FA0
		// (set) Token: 0x06000142 RID: 322 RVA: 0x00006DA8 File Offset: 0x00004FA8
		public string GroupingKey { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00006DB1 File Offset: 0x00004FB1
		// (set) Token: 0x06000144 RID: 324 RVA: 0x00006DB9 File Offset: 0x00004FB9
		public string[] TargetGroups { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00006DC2 File Offset: 0x00004FC2
		// (set) Token: 0x06000146 RID: 326 RVA: 0x00006DCA File Offset: 0x00004FCA
		public string BuildString { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00006DD3 File Offset: 0x00004FD3
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00006DDB File Offset: 0x00004FDB
		public bool IsBinaryPartition { get; private set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00006DE4 File Offset: 0x00004FE4
		// (set) Token: 0x0600014A RID: 330 RVA: 0x00006DEC File Offset: 0x00004FEC
		public string Keyform { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00006DF5 File Offset: 0x00004FF5
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00006E07 File Offset: 0x00005007
		public FileEntry[] Files
		{
			get
			{
				return this.m_files.Values.ToArray<FileEntry>();
			}
			set
			{
				this.m_files.Clear();
				Array.ForEach<FileEntry>(value, delegate(FileEntry x)
				{
					this.m_files.Add(x.DevicePath, x);
				});
			}
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00006E26 File Offset: 0x00005026
		public void BuildSourcePaths(string rootDir, BuildPathOption option)
		{
			this.BuildSourcePaths(rootDir, option, false);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00006E34 File Offset: 0x00005034
		public void BuildSourcePaths(string rootDir, BuildPathOption option, bool installed)
		{
			if (this.m_files != null)
			{
				foreach (FileEntry fileEntry in this.m_files.Values)
				{
					fileEntry.BuildSourcePath(rootDir, option, installed);
				}
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00006E94 File Offset: 0x00005094
		public void Save(string dsmPath)
		{
			IntPtr intPtr = NativeMethods.DeviceSideManifest_Create();
			if (intPtr == IntPtr.Zero)
			{
				throw new PackageException("Failed to create DSM object");
			}
			try
			{
				this.ExportToNativeObject(intPtr);
				NativeMethods.CheckHResult(NativeMethods.DeviceSideManifest_Save(intPtr, dsmPath), "DeviceSideManifest_Save");
			}
			catch (Exception innerException)
			{
				throw new PackageException(innerException, "Unable to save package manifest from file '{0}'", new object[]
				{
					dsmPath
				});
			}
			finally
			{
				NativeMethods.DeviceSideManifest_Free(intPtr);
			}
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00006F14 File Offset: 0x00005114
		public string CpuString()
		{
			switch (this.CpuType)
			{
			case CpuId.AMD64_X86:
				return "wow64";
			case CpuId.ARM64_ARM:
				return "arm64.arm";
			case CpuId.ARM64_X86:
				return "arm64.x86";
			default:
				return this.CpuType.ToString().ToLower(CultureInfo.InvariantCulture);
			}
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00006F70 File Offset: 0x00005170
		public static PkgManifest Load(string dsmPath)
		{
			IntPtr intPtr = NativeMethods.DeviceSideManifest_Create();
			PkgManifest result;
			try
			{
				NativeMethods.CheckHResult(NativeMethods.DeviceSideManifest_Load(intPtr, dsmPath), "DeviceSideManifest_LoadFromXml");
				PkgManifest pkgManifest = new PkgManifest();
				pkgManifest.PackageStyle = PackageStyle.SPKG;
				pkgManifest.ImportFromNativeObject(intPtr);
				result = pkgManifest;
			}
			catch (Exception innerException)
			{
				throw new PackageException(innerException, "Unable to load package manifest from file '{0}'", new object[]
				{
					dsmPath
				});
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					NativeMethods.DeviceSideManifest_Free(intPtr);
				}
			}
			return result;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00006FF4 File Offset: 0x000051F4
		public static PkgManifest Load_CBS(string pkgpath)
		{
			IntPtr intPtr = NativeMethods.DeviceSideManifest_Create();
			string dirPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			PkgManifest result;
			try
			{
				NativeMethods.CheckHResult(NativeMethods.DeviceSideManifest_Load_CBS(intPtr, LongPath.GetFullPathUNC(pkgpath)), "DeviceSideManifest_Load_CBS");
				PkgManifest pkgManifest = new PkgManifest();
				pkgManifest.PackageStyle = PackageStyle.CBS;
				pkgManifest.ImportFromNativeObject(intPtr);
				result = pkgManifest;
			}
			finally
			{
				FileUtils.DeleteTree(dirPath);
				if (intPtr != IntPtr.Zero)
				{
					NativeMethods.DeviceSideManifest_Free(intPtr);
				}
			}
			return result;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00007070 File Offset: 0x00005270
		public static PkgManifest Load(string pkgPath, string pathInCab)
		{
			string tempDirectory = FileUtils.GetTempDirectory();
			PkgManifest result;
			try
			{
				ulong[] array;
				PkgManifest pkgManifest;
				if (CabApiWrapper.GetFileList(pkgPath, out array).Contains("update.mum"))
				{
					pkgManifest = PkgManifest.Load_CBS(pkgPath);
				}
				else
				{
					string text = Path.Combine(tempDirectory, pathInCab);
					try
					{
						CabApiWrapper.ExtractOne(pkgPath, tempDirectory, pathInCab);
					}
					catch (Exception innerException)
					{
						throw new PackageException(innerException, "Failed to extract man.dsm.xml from package '{0}'", new object[]
						{
							pkgPath
						});
					}
					if (!LongPathFile.Exists(text))
					{
						throw new PackageException("Failed to extract the package manifest file from package '{0}'", new object[]
						{
							pkgPath
						});
					}
					pkgManifest = PkgManifest.Load(text);
				}
				result = pkgManifest;
			}
			finally
			{
				FileUtils.DeleteTree(tempDirectory);
			}
			return result;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00007118 File Offset: 0x00005318
		public void ImportFromNativeObject(IntPtr dsmObjPtr)
		{
			this.OwnerType = NativeMethods.PackageDescriptor_Get_OwnerType(dsmObjPtr);
			this.ReleaseType = NativeMethods.PackageDescriptor_Get_ReleaseType(dsmObjPtr);
			this.BuildType = NativeMethods.PackageDescriptor_Get_BuildType(dsmObjPtr);
			this.CpuType = NativeMethods.PackageDescriptor_Get_CpuType(dsmObjPtr);
			this.Keyform = NativeMethods.PackageDescriptor_Get_Keyform(dsmObjPtr);
			this.PackageName = NativeMethods.PackageDescriptor_Get_Name(dsmObjPtr);
			this.Owner = NativeMethods.PackageDescriptor_Get_Owner(dsmObjPtr);
			this.Component = NativeMethods.PackageDescriptor_Get_Component(dsmObjPtr);
			this.SubComponent = NativeMethods.PackageDescriptor_Get_SubComponent(dsmObjPtr);
			VersionInfo version = default(VersionInfo);
			NativeMethods.PackageDescriptor_Get_Version(dsmObjPtr, ref version);
			this.Version = version;
			this.Culture = NativeMethods.PackageDescriptor_Get_Culture(dsmObjPtr);
			this.Resolution = NativeMethods.PackageDescriptor_Get_Resolution(dsmObjPtr);
			this.Partition = NativeMethods.PackageDescriptor_Get_Partition(dsmObjPtr);
			this.Platform = NativeMethods.PackageDescriptor_Get_Platform(dsmObjPtr);
			this.IsRemoval = NativeMethods.PackageDescriptor_Get_IsRemoval(dsmObjPtr);
			this.GroupingKey = NativeMethods.PackageDescriptor_Get_GroupingKey(dsmObjPtr);
			this.BuildString = NativeMethods.PackageDescriptor_Get_BuildString(dsmObjPtr);
			this.PublicKey = NativeMethods.PackageDescriptor_Get_PublicKey(dsmObjPtr);
			this.IsBinaryPartition = NativeMethods.PackageDescriptor_Get_IsBinaryPartition(dsmObjPtr);
			int num = 0;
			IntPtr intPtr = NativeMethods.PackageDescriptor_Get_TargetGroups(dsmObjPtr, ref num);
			List<string> list = new List<string>();
			if (num != 0)
			{
				IntPtr[] array = new IntPtr[num];
				Marshal.Copy(intPtr, array, 0, num);
				foreach (IntPtr ptr in array)
				{
					list.Add(Marshal.PtrToStringUni(ptr));
				}
				NativeMethods.Helper_Free_Array(intPtr);
			}
			this.TargetGroups = list.ToArray();
			IntPtr zero = IntPtr.Zero;
			int num2 = NativeMethods.DeviceSideManifest_Get_Files(dsmObjPtr, ref zero);
			if (num2 > 0)
			{
				IntPtr[] array3 = new IntPtr[num2];
				Marshal.Copy(zero, array3, 0, num2);
				IntPtr[] array2 = array3;
				for (int i = 0; i < array2.Length; i++)
				{
					FileEntry fileEntry = new FileEntry(array2[i]);
					if (this.m_files.ContainsKey(fileEntry.DevicePath))
					{
						throw new PackageException("File collision in package '{0}' for file '{1}'", new object[]
						{
							this.Name,
							fileEntry.DevicePath
						});
					}
					this.m_files.Add(fileEntry.DevicePath, fileEntry);
				}
			}
			this.ValidatePackage();
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000731C File Offset: 0x0000551C
		private void ValidatePackage()
		{
			if (this.PackageStyle == PackageStyle.CBS && this.Version.Equals(VersionInfo.Parse("0.0.0.0")))
			{
				throw new PackageException("Version for package '{0}' cannot be 0.0.0.0", new object[]
				{
					this.Name
				});
			}
			if (this.OwnerType == OwnerType.Invalid)
			{
				throw new PackageException("OwnerType is Invalid for package '{0}'", new object[]
				{
					this.Name
				});
			}
			if (this.ReleaseType == ReleaseType.Invalid)
			{
				throw new PackageException("ReleaseType is Invalid for package '{0}'", new object[]
				{
					this.Name
				});
			}
			if (this.BuildType == BuildType.Invalid)
			{
				throw new PackageException("BuildType is Invalid for package '{0}'", new object[]
				{
					this.Name
				});
			}
			if (this.CpuType == CpuId.Invalid)
			{
				throw new PackageException("CpuType is Invalid for package '{0}'", new object[]
				{
					this.Name
				});
			}
		}

		// Token: 0x06000156 RID: 342 RVA: 0x000073FC File Offset: 0x000055FC
		private void ExportToNativeObject(IntPtr dsmObjPtr)
		{
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_OwnerType(dsmObjPtr, this.OwnerType), "PackageDescriptor_Set_OwnerType");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_OwnerType(dsmObjPtr, this.OwnerType), "PackageDescriptor_Set_OwnerType");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_ReleaseType(dsmObjPtr, this.ReleaseType), "PackageDescriptor_Set_ReleaseType");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_BuildType(dsmObjPtr, this.BuildType), "PackageDescriptor_Set_BuildType");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_CpuType(dsmObjPtr, this.CpuType), "PackageDescriptor_Set_CpuType");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_Owner(dsmObjPtr, this.Owner), "PackageDescriptor_Set_Owner");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_Component(dsmObjPtr, this.Component), "PackageDescriptor_Set_Component");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_SubComponent(dsmObjPtr, this.SubComponent), "PackageDescriptor_Set_SubComponent");
			VersionInfo version = this.Version;
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_Version(dsmObjPtr, ref version), "PackageDescriptor_Set_Version");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_Culture(dsmObjPtr, this.Culture), "PackageDescriptor_Set_Culture");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_Resolution(dsmObjPtr, this.Resolution), "PackageDescriptor_Set_Resolution");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_Partition(dsmObjPtr, this.Partition), "PackageDescriptor_Set_Partition");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_Platform(dsmObjPtr, this.Platform), "PackageDescriptor_Set_Platform");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_IsRemoval(dsmObjPtr, this.IsRemoval), "PackageDescriptor_Set_IsRemoval");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_GroupingKey(dsmObjPtr, this.GroupingKey), "PackageDescriptor_Set_GroupingKey");
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_BuildString(dsmObjPtr, this.BuildString), "PackageDescriptor_Set_BuildString");
			if (this.PublicKey != null)
			{
				NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_PublicKey(dsmObjPtr, this.PublicKey), "PackageDescriptor_Set_PublicKey");
			}
			NativeMethods.CheckHResult(NativeMethods.PackageDescriptor_Set_TargetGroups(dsmObjPtr, this.TargetGroups, this.TargetGroups.Length), "PackageDescriptor_Set_TargetGroups");
			foreach (FileEntry fileEntry in this.m_files.Values)
			{
				NativeMethods.CheckHResult(NativeMethods.DeviceSideManifest_Add_File(dsmObjPtr, fileEntry.FileType, fileEntry.DevicePath, fileEntry.CabPath, fileEntry.Attributes, fileEntry.SourcePackage, fileEntry.EmbeddedSigningCategory, fileEntry.Size, fileEntry.CompressedSize, fileEntry.StagedSize, fileEntry.FileHash, fileEntry.SignInfoRequired), "DeviceSideManifest_Add_File");
			}
		}

		// Token: 0x04000043 RID: 67
		public SortedDictionary<string, FileEntry> m_files = new SortedDictionary<string, FileEntry>(StringComparer.InvariantCultureIgnoreCase);
	}
}
