using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000015 RID: 21
	public class OfflineRegUtils
	{
		// Token: 0x060000AF RID: 175 RVA: 0x00005C24 File Offset: 0x00003E24
		public static IntPtr CreateHive()
		{
			IntPtr zero = IntPtr.Zero;
			int num = OffRegNativeMethods.ORCreateHive(ref zero);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
			return zero;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005C4C File Offset: 0x00003E4C
		public static IntPtr CreateKey(IntPtr handle, string keyName)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			if (string.IsNullOrEmpty("keyName"))
			{
				throw new ArgumentNullException("keyName");
			}
			IntPtr zero = IntPtr.Zero;
			uint num = 0U;
			foreach (string text in keyName.Split(OfflineRegUtils.BSLASH_DELIMITER))
			{
				int num2 = OffRegNativeMethods.ORCreateKey(handle, keyName, null, 0U, null, ref zero, ref num);
				if (num2 != 0)
				{
					throw new Win32Exception(num2);
				}
			}
			return zero;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00005CCC File Offset: 0x00003ECC
		public static void SetValue(IntPtr handle, string valueName, RegistryValueType type, byte[] value)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			if (valueName == null)
			{
				valueName = string.Empty;
			}
			int num = OffRegNativeMethods.ORSetValue(handle, valueName, (uint)type, value, (uint)value.Length);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00005D14 File Offset: 0x00003F14
		public static void DeleteValue(IntPtr handle, string valueName)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			if (valueName == null)
			{
				valueName = string.Empty;
			}
			int num = OffRegNativeMethods.ORDeleteValue(handle, valueName);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00005D58 File Offset: 0x00003F58
		public static void DeleteKey(IntPtr handle, string keyName)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			int num = OffRegNativeMethods.ORDeleteKey(handle, keyName);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00005D90 File Offset: 0x00003F90
		public static IntPtr OpenHive(string hivefile)
		{
			if (string.IsNullOrEmpty(hivefile))
			{
				throw new ArgumentNullException("hivefile");
			}
			IntPtr zero = IntPtr.Zero;
			int num = OffRegNativeMethods.OROpenHive(hivefile, ref zero);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
			return zero;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00005DCC File Offset: 0x00003FCC
		public static void SaveHive(IntPtr handle, string path, int osMajor, int osMinor)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			if (File.Exists(path))
			{
				FileUtils.DeleteFile(path);
			}
			int num = OffRegNativeMethods.ORSaveHive(handle, path, osMajor, osMinor);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00005E28 File Offset: 0x00004028
		public static void CloseHive(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			int num = OffRegNativeMethods.ORCloseHive(handle);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00005E60 File Offset: 0x00004060
		public static IntPtr OpenKey(IntPtr handle, string subKeyName)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			if (string.IsNullOrEmpty("subKeyName"))
			{
				throw new ArgumentNullException("subKeyName");
			}
			IntPtr zero = IntPtr.Zero;
			int num = OffRegNativeMethods.OROpenKey(handle, subKeyName, ref zero);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
			return zero;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00005EB8 File Offset: 0x000040B8
		public static void CloseKey(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			int num = OffRegNativeMethods.ORCloseKey(handle);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00005EEE File Offset: 0x000040EE
		public static void ConvertHiveToReg(string inputHiveFile, string outputRegFile)
		{
			new HiveToRegConverter(inputHiveFile).ConvertToReg(outputRegFile);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00005EFC File Offset: 0x000040FC
		public static void ConvertHiveToReg(string inputHiveFile, string outputRegFile, string keyPrefix)
		{
			new HiveToRegConverter(inputHiveFile, keyPrefix).ConvertToReg(outputRegFile);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005F0B File Offset: 0x0000410B
		public static void ConvertHiveToReg(string inputHiveFile, string outputRegFile, string keyPrefix, bool appendExisting)
		{
			new HiveToRegConverter(inputHiveFile, keyPrefix).ConvertToReg(outputRegFile, null, appendExisting);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00005F1C File Offset: 0x0000411C
		public static string ConvertByteArrayToRegStrings(byte[] data)
		{
			return OfflineRegUtils.ConvertByteArrayToRegStrings(data, 40);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00005F28 File Offset: 0x00004128
		public static string ConvertByteArrayToRegStrings(byte[] data, int maxOnALine)
		{
			string result = string.Empty;
			if (-1 == maxOnALine)
			{
				result = BitConverter.ToString(data).Replace('-', ',');
			}
			else
			{
				int num = 0;
				int i = data.Length;
				StringBuilder stringBuilder = new StringBuilder();
				while (i > 0)
				{
					int num2 = (i > maxOnALine) ? maxOnALine : i;
					string text = BitConverter.ToString(data, num, num2);
					num += num2;
					i -= num2;
					text = text.Replace('-', ',');
					stringBuilder.Append(text);
					if (i > 0)
					{
						stringBuilder.Append(",\\");
						stringBuilder.AppendLine();
					}
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00005FB8 File Offset: 0x000041B8
		public static RegistryValueType GetValueType(IntPtr handle, string valueName)
		{
			uint result = 0U;
			uint num = 0U;
			int num2 = OffRegNativeMethods.ORGetValue(handle, null, valueName, out result, null, ref num);
			if (num2 != 0)
			{
				throw new Win32Exception(num2);
			}
			return (RegistryValueType)result;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00005FE4 File Offset: 0x000041E4
		public static List<KeyValuePair<string, RegistryValueType>> GetValueNamesAndTypes(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			uint num = 0U;
			StringBuilder stringBuilder = new StringBuilder(1024);
			List<KeyValuePair<string, RegistryValueType>> list = new List<KeyValuePair<string, RegistryValueType>>();
			int num3;
			for (;;)
			{
				uint capacity = (uint)stringBuilder.Capacity;
				uint num2 = 0U;
				num3 = OffRegNativeMethods.OREnumValue(handle, num, stringBuilder, ref capacity, out num2, IntPtr.Zero, IntPtr.Zero);
				if (num3 != 0)
				{
					if (num3 != 259)
					{
						break;
					}
				}
				else
				{
					string key = stringBuilder.ToString();
					RegistryValueType value = (RegistryValueType)num2;
					list.Add(new KeyValuePair<string, RegistryValueType>(key, value));
					num += 1U;
				}
				if (num3 == 259)
				{
					return list;
				}
			}
			throw new Win32Exception(num3);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00006080 File Offset: 0x00004280
		public static string[] GetValueNames(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle", "Handle cannot be empty.");
			}
			return (from a in OfflineRegUtils.GetValueNamesAndTypes(handle)
			select a.Key).ToArray<string>();
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000060DC File Offset: 0x000042DC
		public static string GetClass(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle", "Handle cannot be empty.");
			}
			StringBuilder stringBuilder = new StringBuilder(128);
			uint num = (uint)stringBuilder.Capacity;
			uint[] array = new uint[8];
			IntPtr zero = IntPtr.Zero;
			int num2 = OffRegNativeMethods.ORQueryInfoKey(handle, stringBuilder, ref num, out array[0], out array[1], out array[3], out array[4], out array[5], out array[6], out array[7], zero);
			if (num2 == 234)
			{
				num += 1U;
				stringBuilder.Capacity = (int)num;
				num2 = OffRegNativeMethods.ORQueryInfoKey(handle, stringBuilder, ref num, out array[0], out array[1], out array[3], out array[4], out array[5], out array[6], out array[7], zero);
			}
			if (num2 != 0)
			{
				throw new Win32Exception(num2);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000061C8 File Offset: 0x000043C8
		public static byte[] GetValue(IntPtr handle, string valueName)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle", "Handle cannot be empty.");
			}
			uint num = 0U;
			uint num2 = 0U;
			int num3 = OffRegNativeMethods.ORGetValue(handle, null, valueName, out num, null, ref num2);
			if (num3 != 0)
			{
				throw new Win32Exception(num3);
			}
			byte[] array = new byte[num2];
			num3 = OffRegNativeMethods.ORGetValue(handle, null, valueName, out num, array, ref num2);
			if (num3 != 0)
			{
				throw new Win32Exception(num3);
			}
			return array;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00006230 File Offset: 0x00004430
		public static string[] GetSubKeys(IntPtr registryKey)
		{
			if (registryKey == IntPtr.Zero)
			{
				throw new ArgumentNullException("registryKey", "registryKey pointer cannot be empty.");
			}
			uint num = 0U;
			StringBuilder stringBuilder = new StringBuilder(1024);
			List<string> list = new List<string>();
			int num3;
			for (;;)
			{
				uint num2 = 0U;
				IntPtr zero = IntPtr.Zero;
				uint capacity = (uint)stringBuilder.Capacity;
				num3 = OffRegNativeMethods.OREnumKey(registryKey, num, stringBuilder, ref capacity, null, ref num2, ref zero);
				if (num3 != 0)
				{
					if (num3 != 259)
					{
						break;
					}
				}
				else
				{
					list.Add(stringBuilder.ToString());
					num += 1U;
				}
				if (num3 == 259)
				{
					goto Block_4;
				}
			}
			throw new Win32Exception(num3);
			Block_4:
			return list.ToArray();
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000062C8 File Offset: 0x000044C8
		public static byte[] GetRawRegistrySecurity(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			uint num = 0U;
			int num2 = 234;
			int num3 = OffRegNativeMethods.ORGetKeySecurity(handle, (SecurityInformationFlags)28U, null, ref num);
			if (num2 != num3)
			{
				throw new Win32Exception(num3);
			}
			byte[] array = new byte[num];
			num3 = OffRegNativeMethods.ORGetKeySecurity(handle, (SecurityInformationFlags)28U, array, ref num);
			if (num3 != 0)
			{
				throw new Win32Exception(num3);
			}
			return array;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006328 File Offset: 0x00004528
		public static void SetRawRegistrySecurity(IntPtr handle, byte[] buf)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			int num = OffRegNativeMethods.ORSetKeySecurity(handle, (SecurityInformationFlags)28U, buf);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006364 File Offset: 0x00004564
		public static RegistrySecurity GetRegistrySecurity(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			byte[] rawRegistrySecurity = OfflineRegUtils.GetRawRegistrySecurity(handle);
			SecurityUtils.ConvertSDToStringSD(rawRegistrySecurity, (SecurityInformationFlags)24U);
			RegistrySecurity registrySecurity = new RegistrySecurity();
			registrySecurity.SetSecurityDescriptorBinaryForm(rawRegistrySecurity);
			return registrySecurity;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000063A8 File Offset: 0x000045A8
		public static int GetVirtualFlags(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			int result = 0;
			int num = OffRegNativeMethods.ORGetVirtualFlags(handle, ref result);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
			return result;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000063E4 File Offset: 0x000045E4
		public static int ExtractFromHive(string hivePath, RegistryValueType type, string targetPath)
		{
			if (string.IsNullOrEmpty("hivePath"))
			{
				throw new ArgumentNullException("hivePath");
			}
			if (string.IsNullOrEmpty("targetPath"))
			{
				throw new ArgumentNullException("targetPath");
			}
			if (!File.Exists(hivePath))
			{
				throw new FileNotFoundException("Hive file {0} does not exist", hivePath);
			}
			int result = 0;
			bool flag = false;
			using (ORRegistryKey orregistryKey = ORRegistryKey.OpenHive(hivePath))
			{
				using (ORRegistryKey orregistryKey2 = ORRegistryKey.CreateEmptyHive())
				{
					flag = (0 < (result = OfflineRegUtils.ExtractFromHiveRecursive(orregistryKey, type, orregistryKey2)));
					if (flag)
					{
						orregistryKey2.SaveHive(targetPath);
					}
				}
				if (flag)
				{
					orregistryKey.SaveHive(hivePath);
				}
			}
			return result;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000649C File Offset: 0x0000469C
		private static int ExtractFromHiveRecursive(ORRegistryKey srcHiveRoot, RegistryValueType type, ORRegistryKey dstHiveRoot)
		{
			int num = 0;
			string fullName = srcHiveRoot.FullName;
			foreach (string text in from p in srcHiveRoot.ValueNameAndTypes
			where p.Value == RegistryValueType.MultiString
			select p into q
			select q.Key)
			{
				string valueName = string.IsNullOrEmpty(text) ? null : text;
				string[] multiStringValue = srcHiveRoot.GetMultiStringValue(valueName);
				using (ORRegistryKey orregistryKey = dstHiveRoot.CreateSubKey(fullName))
				{
					orregistryKey.SetValue(valueName, multiStringValue);
					num++;
				}
				srcHiveRoot.DeleteValue(valueName);
			}
			foreach (string subkeyname in srcHiveRoot.SubKeys)
			{
				using (ORRegistryKey orregistryKey2 = srcHiveRoot.OpenSubKey(subkeyname))
				{
					num += OfflineRegUtils.ExtractFromHiveRecursive(orregistryKey2, type, dstHiveRoot);
				}
			}
			return num;
		}

		// Token: 0x04000041 RID: 65
		private static readonly char[] BSLASH_DELIMITER = new char[]
		{
			'\\'
		};
	}
}
