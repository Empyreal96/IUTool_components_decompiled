using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200001D RID: 29
	public class SecurityUtils
	{
		// Token: 0x0600010A RID: 266 RVA: 0x00007044 File Offset: 0x00005244
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

		// Token: 0x0600010B RID: 267 RVA: 0x000070A8 File Offset: 0x000052A8
		[CLSCompliant(false)]
		public static byte[] GetSecurityDescriptor(string resourcePath, SecurityInformationFlags flags)
		{
			byte[] array = null;
			int num = 0;
			NativeSecurityMethods.GetFileSecurity(resourcePath, flags, IntPtr.Zero, 0, ref num);
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 122)
			{
				Console.WriteLine("Error {0} Calling GetFileSecurity() on {1}", lastWin32Error, resourcePath);
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

		// Token: 0x0600010C RID: 268 RVA: 0x0000713C File Offset: 0x0000533C
		[CLSCompliant(false)]
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

		// Token: 0x0600010D RID: 269 RVA: 0x000071A0 File Offset: 0x000053A0
		public static AclCollection GetFileSystemACLs(string rootDir)
		{
			if (rootDir == null)
			{
				throw new ArgumentNullException("rootDir");
			}
			if (!LongPathDirectory.Exists(rootDir))
			{
				throw new ArgumentException(string.Format("Directory {0} does not exist", rootDir));
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

		// Token: 0x0600010E RID: 270 RVA: 0x00007200 File Offset: 0x00005400
		public static AclCollection GetRegistryACLs(string hiveRoot)
		{
			if (hiveRoot == null)
			{
				throw new ArgumentNullException("hiveRoot");
			}
			if (!LongPathDirectory.Exists(hiveRoot))
			{
				throw new ArgumentException(string.Format("Directory {0} does not exist", hiveRoot));
			}
			AclCollection aclCollection = new AclCollection();
			foreach (object obj in Enum.GetValues(typeof(SystemRegistryHiveFiles)))
			{
				SystemRegistryHiveFiles systemRegistryHiveFiles = (SystemRegistryHiveFiles)obj;
				string hivefile = Path.Combine(hiveRoot, Enum.GetName(typeof(SystemRegistryHiveFiles), systemRegistryHiveFiles));
				string prefix = RegistryUtils.MapHiveToMountPoint(systemRegistryHiveFiles);
				using (ORRegistryKey orregistryKey = ORRegistryKey.OpenHive(hivefile, prefix))
				{
					SecurityUtils.GetRegistryACLsRecursive(orregistryKey, aclCollection);
				}
			}
			return aclCollection;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000072D8 File Offset: 0x000054D8
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

		// Token: 0x06000110 RID: 272 RVA: 0x00007350 File Offset: 0x00005550
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

		// Token: 0x04000061 RID: 97
		private const int ERROR_INSUFFICIENT_BUFFER = 122;

		// Token: 0x04000062 RID: 98
		private static readonly Regex regexExtractMIL = new Regex("(?<MIL>\\(ML[^\\)]*\\))", RegexOptions.Compiled);
	}
}
