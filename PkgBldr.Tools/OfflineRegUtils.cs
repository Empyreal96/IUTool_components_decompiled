using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000016 RID: 22
	public class OfflineRegUtils
	{
		// Token: 0x060000DA RID: 218 RVA: 0x00004724 File Offset: 0x00002924
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

		// Token: 0x060000DB RID: 219 RVA: 0x0000474C File Offset: 0x0000294C
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
			foreach (string subKeyName in keyName.Split(OfflineRegUtils.BSLASH_DELIMITER))
			{
				int num2 = OffRegNativeMethods.ORCreateKey(handle, subKeyName, null, 0U, null, ref zero, ref num);
				if (num2 != 0)
				{
					throw new Win32Exception(num2);
				}
				handle = zero;
			}
			return zero;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000047D0 File Offset: 0x000029D0
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

		// Token: 0x060000DD RID: 221 RVA: 0x00004818 File Offset: 0x00002A18
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

		// Token: 0x060000DE RID: 222 RVA: 0x0000485C File Offset: 0x00002A5C
		public static void DeleteKey(IntPtr handle, string keyName)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			if (keyName == null)
			{
				throw new ArgumentNullException("keyName");
			}
			int num = OffRegNativeMethods.ORDeleteKey(handle, keyName);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000048A4 File Offset: 0x00002AA4
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

		// Token: 0x060000E0 RID: 224 RVA: 0x000048E0 File Offset: 0x00002AE0
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
			if (LongPathFile.Exists(path))
			{
				FileUtils.DeleteFile(path);
			}
			int num = OffRegNativeMethods.ORSaveHive(handle, path, osMajor, osMinor);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000493C File Offset: 0x00002B3C
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

		// Token: 0x060000E2 RID: 226 RVA: 0x00004974 File Offset: 0x00002B74
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

		// Token: 0x060000E3 RID: 227 RVA: 0x000049CC File Offset: 0x00002BCC
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

		// Token: 0x060000E4 RID: 228 RVA: 0x00004A02 File Offset: 0x00002C02
		public static void ConvertHiveToReg(string inputHiveFile, string outputRegFile)
		{
			new HiveToRegConverter(inputHiveFile, null).ConvertToReg(outputRegFile, null, false);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00004A13 File Offset: 0x00002C13
		public static void ConvertHiveToReg(string inputHiveFile, string outputRegFile, string keyPrefix)
		{
			new HiveToRegConverter(inputHiveFile, keyPrefix).ConvertToReg(outputRegFile, null, false);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00004A24 File Offset: 0x00002C24
		public static void ConvertHiveToReg(string inputHiveFile, string outputRegFile, string keyPrefix, bool appendExisting)
		{
			new HiveToRegConverter(inputHiveFile, keyPrefix).ConvertToReg(outputRegFile, null, appendExisting);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00004A35 File Offset: 0x00002C35
		public static string ConvertByteArrayToRegStrings(byte[] data)
		{
			return OfflineRegUtils.ConvertByteArrayToRegStrings(data, 40);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00004A40 File Offset: 0x00002C40
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

		// Token: 0x060000E9 RID: 233 RVA: 0x00004AD0 File Offset: 0x00002CD0
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

		// Token: 0x060000EA RID: 234 RVA: 0x00004AFC File Offset: 0x00002CFC
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

		// Token: 0x060000EB RID: 235 RVA: 0x00004B98 File Offset: 0x00002D98
		public static string[] GetValueNames(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			return (from a in OfflineRegUtils.GetValueNamesAndTypes(handle)
			select a.Key).ToArray<string>();
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00004BEC File Offset: 0x00002DEC
		public static string GetClass(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
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

		// Token: 0x060000ED RID: 237 RVA: 0x00004CD0 File Offset: 0x00002ED0
		public static byte[] GetValue(IntPtr handle, string valueName)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
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

		// Token: 0x060000EE RID: 238 RVA: 0x00004D34 File Offset: 0x00002F34
		public static string[] GetSubKeys(IntPtr registryKey)
		{
			if (registryKey == IntPtr.Zero)
			{
				throw new ArgumentNullException("registryKey");
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

		// Token: 0x060000EF RID: 239 RVA: 0x00004DC4 File Offset: 0x00002FC4
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

		// Token: 0x060000F0 RID: 240 RVA: 0x00004E24 File Offset: 0x00003024
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

		// Token: 0x060000F1 RID: 241 RVA: 0x00004E60 File Offset: 0x00003060
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

		// Token: 0x060000F2 RID: 242 RVA: 0x00004EA4 File Offset: 0x000030A4
		[SuppressMessage("Microsoft.Usage", "CA1806")]
		public static int GetVirtualFlags(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			int result = 0;
			OffRegNativeMethods.ORGetVirtualFlags(handle, ref result);
			return result;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00004ED8 File Offset: 0x000030D8
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
			if (!LongPathFile.Exists(hivePath))
			{
				throw new FileNotFoundException("Hive file {0} does not exist", hivePath);
			}
			int result = 0;
			bool flag = false;
			using (ORRegistryKey orregistryKey = ORRegistryKey.OpenHive(hivePath, null))
			{
				using (ORRegistryKey orregistryKey2 = ORRegistryKey.CreateEmptyHive(null))
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

		// Token: 0x060000F4 RID: 244 RVA: 0x00004F94 File Offset: 0x00003194
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

		// Token: 0x04000042 RID: 66
		private static readonly char[] BSLASH_DELIMITER = new char[]
		{
			'\\'
		};
	}
}
