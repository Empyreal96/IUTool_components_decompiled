using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000029 RID: 41
	public class SecurityUtils
	{
		// Token: 0x06000180 RID: 384 RVA: 0x00007254 File Offset: 0x00005454
		public static string GetFileSystemMandatoryLevel(string resourcePath)
		{
			string result = string.Empty;
			string text = SecurityUtils.ConvertSDToStringSD(SecurityUtils.GetSecurityDescriptor(resourcePath, SecurityInformationFlags.MANDATORY_ACCESS_LABEL), SecurityInformationFlags.MANDATORY_ACCESS_LABEL);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.TrimEnd(new char[1]);
				Match match = SecurityUtils.regexExtractMIL.Match(text);
				if (match.Success)
				{
					result = match.Groups["MIL"].Value;
				}
			}
			return result;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x000072B8 File Offset: 0x000054B8
		public static byte[] GetSecurityDescriptor(string resourcePath, SecurityInformationFlags flags)
		{
			byte[] array = null;
			int num = 0;
			NativeSecurityMethods.GetFileSecurity(resourcePath, flags, IntPtr.Zero, 0, ref num);
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 122)
			{
				throw new Win32Exception(lastWin32Error);
			}
			int num2 = num;
			IntPtr intPtr = Marshal.AllocHGlobal(num2);
			try
			{
				if (!NativeSecurityMethods.GetFileSecurity(resourcePath, flags, intPtr, num2, ref num))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				array = new byte[num];
				Marshal.Copy(intPtr, array, 0, num);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return array;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000733C File Offset: 0x0000553C
		public static string ConvertSDToStringSD(byte[] securityDescriptor, SecurityInformationFlags flags)
		{
			string result = string.Empty;
			IntPtr zero;
			int len;
			bool flag = NativeSecurityMethods.ConvertSecurityDescriptorToStringSecurityDescriptor(securityDescriptor, 1, flags, out zero, out len);
			try
			{
				if (!flag)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				result = Marshal.PtrToStringUni(zero, len);
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(zero);
				}
				zero = IntPtr.Zero;
			}
			return result;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000073A0 File Offset: 0x000055A0
		public static AclCollection GetFileSystemACLs(string rootDir)
		{
			if (rootDir == null)
			{
				throw new ArgumentNullException("rootDir");
			}
			if (!LongPathDirectory.Exists(rootDir))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Directory {0} does not exist", new object[]
				{
					rootDir
				}));
			}
			AclCollection aclCollection = new AclCollection();
			DirectoryInfo directoryInfo = new DirectoryInfo(rootDir);
			DirectoryAcl directoryAcl = new DirectoryAcl(directoryInfo, rootDir);
			if (!directoryAcl.IsEmpty)
			{
				aclCollection.Add(directoryAcl);
			}
			SecurityUtils.GetFileSystemACLsRecursive(directoryInfo, rootDir, aclCollection);
			return aclCollection;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00007410 File Offset: 0x00005610
		public static AclCollection GetRegistryACLs(string hiveRoot)
		{
			if (hiveRoot == null)
			{
				throw new ArgumentNullException("hiveRoot");
			}
			if (!LongPathDirectory.Exists(hiveRoot))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Directory {0} does not exist", new object[]
				{
					hiveRoot
				}));
			}
			AclCollection aclCollection = new AclCollection();
			foreach (object obj in Enum.GetValues(typeof(SystemRegistryHiveFiles)))
			{
				SystemRegistryHiveFiles systemRegistryHiveFiles = (SystemRegistryHiveFiles)obj;
				string hivefile = LongPath.Combine(hiveRoot, Enum.GetName(typeof(SystemRegistryHiveFiles), systemRegistryHiveFiles));
				string prefix = RegistryUtils.MapHiveToMountPoint(systemRegistryHiveFiles);
				using (ORRegistryKey orregistryKey = ORRegistryKey.OpenHive(hivefile, prefix))
				{
					SecurityUtils.GetRegistryACLsRecursive(orregistryKey, aclCollection);
				}
			}
			return aclCollection;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x000074F8 File Offset: 0x000056F8
		private static void GetFileSystemACLsRecursive(DirectoryInfo rootdi, string rootDir, AclCollection accesslist)
		{
			foreach (DirectoryInfo directoryInfo in rootdi.GetDirectories())
			{
				SecurityUtils.GetFileSystemACLsRecursive(directoryInfo, rootDir, accesslist);
				DirectoryAcl directoryAcl = new DirectoryAcl(directoryInfo, rootDir);
				if (!directoryAcl.IsEmpty)
				{
					accesslist.Add(directoryAcl);
				}
			}
			FileInfo[] files = rootdi.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				FileAcl fileAcl = new FileAcl(files[i], rootDir);
				if (!fileAcl.IsEmpty)
				{
					accesslist.Add(fileAcl);
				}
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00007570 File Offset: 0x00005770
		public static void GetRegistryACLsRecursive(ORRegistryKey parent, AclCollection accesslist)
		{
			foreach (string subkeyname in parent.SubKeys)
			{
				using (ORRegistryKey orregistryKey = parent.OpenSubKey(subkeyname))
				{
					SecurityUtils.GetRegistryACLsRecursive(orregistryKey, accesslist);
					RegistryAcl registryAcl = new RegistryAcl(orregistryKey);
					if (!registryAcl.IsEmpty)
					{
						accesslist.Add(registryAcl);
					}
				}
			}
		}

		// Token: 0x04000079 RID: 121
		private const int ERROR_INSUFFICIENT_BUFFER = 122;

		// Token: 0x0400007A RID: 122
		private static readonly Regex regexExtractMIL = new Regex("(?<MIL>\\(ML[^\\)]*\\))", RegexOptions.Compiled);
	}
}
