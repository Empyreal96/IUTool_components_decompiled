using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000014 RID: 20
	public class ORRegistryKey : IDisposable
	{
		// Token: 0x0600008D RID: 141 RVA: 0x00005564 File Offset: 0x00003764
		private ORRegistryKey(string name, IntPtr handle, bool isRoot, ORRegistryKey parent)
		{
			this.m_name = name;
			this.m_handle = handle;
			this.m_isRoot = isRoot;
			this.m_parent = parent;
			if (this.m_parent != null)
			{
				this.m_parent.m_children[this] = true;
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000055E0 File Offset: 0x000037E0
		~ORRegistryKey()
		{
			this.Dispose(false);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005610 File Offset: 0x00003810
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005620 File Offset: 0x00003820
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (ORRegistryKey orregistryKey in this.m_children.Keys)
				{
					orregistryKey.Close();
				}
				this.m_children.Clear();
				if (this.m_parent != null)
				{
					this.m_parent.m_children.Remove(this);
					return;
				}
				this.Close();
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000056A4 File Offset: 0x000038A4
		public static ORRegistryKey OpenHive(string hivefile)
		{
			return ORRegistryKey.OpenHive(hivefile, null);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000056AD File Offset: 0x000038AD
		public static ORRegistryKey OpenHive(string hivefile, string prefix)
		{
			if (prefix == null)
			{
				prefix = "\\";
			}
			return new ORRegistryKey(prefix, OfflineRegUtils.OpenHive(hivefile), true, null);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000056C7 File Offset: 0x000038C7
		public static ORRegistryKey CreateEmptyHive()
		{
			return ORRegistryKey.CreateEmptyHive(null);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000056CF File Offset: 0x000038CF
		public static ORRegistryKey CreateEmptyHive(string prefix)
		{
			return new ORRegistryKey(string.IsNullOrEmpty(prefix) ? "\\" : prefix, OfflineRegUtils.CreateHive(), true, null);
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000095 RID: 149 RVA: 0x000056ED File Offset: 0x000038ED
		public ORRegistryKey Parent
		{
			get
			{
				return this.m_parent;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000096 RID: 150 RVA: 0x000056F5 File Offset: 0x000038F5
		public string[] SubKeys
		{
			get
			{
				return OfflineRegUtils.GetSubKeys(this.m_handle);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00005702 File Offset: 0x00003902
		public string FullName
		{
			get
			{
				return this.m_name;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000098 RID: 152 RVA: 0x0000570A File Offset: 0x0000390A
		public string Class
		{
			get
			{
				return OfflineRegUtils.GetClass(this.m_handle);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000099 RID: 153 RVA: 0x00005717 File Offset: 0x00003917
		public string[] ValueNames
		{
			get
			{
				return OfflineRegUtils.GetValueNames(this.m_handle);
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00005724 File Offset: 0x00003924
		public List<KeyValuePair<string, RegistryValueType>> ValueNameAndTypes
		{
			get
			{
				return OfflineRegUtils.GetValueNamesAndTypes(this.m_handle);
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00005731 File Offset: 0x00003931
		public RegistrySecurity RegistrySecurity
		{
			get
			{
				return OfflineRegUtils.GetRegistrySecurity(this.m_handle);
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005740 File Offset: 0x00003940
		public ORRegistryKey OpenSubKey(string subkeyname)
		{
			int num = subkeyname.IndexOf("\\", StringComparison.OrdinalIgnoreCase);
			ORRegistryKey result;
			if (-1 < num)
			{
				string[] array = subkeyname.Split(this.BSLASH_DELIMITER);
				ORRegistryKey orregistryKey = this;
				ORRegistryKey orregistryKey2 = null;
				foreach (string subkeyname2 in array)
				{
					orregistryKey2 = orregistryKey.OpenSubKey(subkeyname2);
					orregistryKey = orregistryKey2;
				}
				result = orregistryKey2;
			}
			else
			{
				IntPtr handle = OfflineRegUtils.OpenKey(this.m_handle, subkeyname);
				result = new ORRegistryKey(this.CombineSubKeys(this.m_name, subkeyname), handle, false, this);
			}
			return result;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000057C1 File Offset: 0x000039C1
		public RegistryValueType GetValueKind(string valueName)
		{
			return OfflineRegUtils.GetValueType(this.m_handle, valueName);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000057CF File Offset: 0x000039CF
		public byte[] GetByteValue(string valueName)
		{
			return OfflineRegUtils.GetValue(this.m_handle, valueName);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000057E0 File Offset: 0x000039E0
		[CLSCompliant(false)]
		public uint GetDwordValue(string valueName)
		{
			byte[] byteValue = this.GetByteValue(valueName);
			if (byteValue.Length != 0)
			{
				return BitConverter.ToUInt32(byteValue, 0);
			}
			return 0U;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005804 File Offset: 0x00003A04
		[CLSCompliant(false)]
		public ulong GetQwordValue(string valueName)
		{
			byte[] byteValue = this.GetByteValue(valueName);
			if (byteValue.Length != 0)
			{
				return BitConverter.ToUInt64(byteValue, 0);
			}
			return 0UL;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005828 File Offset: 0x00003A28
		public string GetStringValue(string valueName)
		{
			byte[] byteValue = this.GetByteValue(valueName);
			string empty = string.Empty;
			if (byteValue.Length > 1)
			{
				byte[] array = byteValue;
				if (array[array.Length - 1] == 0)
				{
					byte[] array2 = byteValue;
					if (array2[array2.Length - 2] == 0)
					{
						return Encoding.Unicode.GetString(byteValue, 0, byteValue.Length - 2);
					}
				}
			}
			return Encoding.Unicode.GetString(byteValue);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000587C File Offset: 0x00003A7C
		public string[] GetMultiStringValue(string valueName)
		{
			byte[] byteValue = this.GetByteValue(valueName);
			return Encoding.Unicode.GetString(byteValue).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000058A8 File Offset: 0x00003AA8
		public object GetValue(string valueName)
		{
			RegistryValueType valueKind = this.GetValueKind(valueName);
			object result = null;
			switch (valueKind)
			{
			case RegistryValueType.None:
				result = this.GetByteValue(valueName);
				break;
			case RegistryValueType.String:
				result = this.GetStringValue(valueName);
				break;
			case RegistryValueType.ExpandString:
				result = this.GetStringValue(valueName);
				break;
			case RegistryValueType.Binary:
				result = this.GetByteValue(valueName);
				break;
			case RegistryValueType.DWord:
				result = this.GetDwordValue(valueName);
				break;
			case RegistryValueType.DWordBigEndian:
				result = this.GetByteValue(valueName);
				break;
			case RegistryValueType.Link:
				result = this.GetByteValue(valueName);
				break;
			case RegistryValueType.MultiString:
				result = this.GetMultiStringValue(valueName);
				break;
			case RegistryValueType.RegResourceList:
				result = this.GetByteValue(valueName);
				break;
			case RegistryValueType.RegFullResourceDescriptor:
				result = this.GetByteValue(valueName);
				break;
			case RegistryValueType.RegResourceRequirementsList:
				result = this.GetByteValue(valueName);
				break;
			case RegistryValueType.QWord:
				result = this.GetQwordValue(valueName);
				break;
			}
			return result;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000597B File Offset: 0x00003B7B
		public void SaveHive(string path)
		{
			if (!this.m_isRoot)
			{
				throw new IUException("Invalid operation - This registry key does not represent hive root");
			}
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			OfflineRegUtils.SaveHive(this.m_handle, path, 6, 3);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000059B4 File Offset: 0x00003BB4
		public ORRegistryKey CreateSubKey(string subkeyName)
		{
			int num = subkeyName.IndexOf("\\", StringComparison.OrdinalIgnoreCase);
			ORRegistryKey result;
			if (-1 != num)
			{
				string[] array = subkeyName.Split(this.BSLASH_DELIMITER, StringSplitOptions.RemoveEmptyEntries);
				ORRegistryKey orregistryKey = this;
				ORRegistryKey orregistryKey2 = null;
				foreach (string subkeyName2 in array)
				{
					orregistryKey2 = orregistryKey.CreateSubKey(subkeyName2);
					orregistryKey = orregistryKey2;
				}
				result = orregistryKey2;
			}
			else
			{
				IntPtr handle = OfflineRegUtils.CreateKey(this.m_handle, subkeyName);
				result = new ORRegistryKey(this.CombineSubKeys(this.m_name, subkeyName), handle, false, this);
			}
			return result;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00005A36 File Offset: 0x00003C36
		public void SetValue(string valueName, byte[] value)
		{
			OfflineRegUtils.SetValue(this.m_handle, valueName, RegistryValueType.Binary, value);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00005A48 File Offset: 0x00003C48
		public void SetValue(string valueName, string value)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(value);
			OfflineRegUtils.SetValue(this.m_handle, valueName, RegistryValueType.String, bytes);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00005A70 File Offset: 0x00003C70
		public void SetValue(string valueName, string[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			StringBuilder stringBuilder = new StringBuilder(1024);
			foreach (string arg in values)
			{
				stringBuilder.AppendFormat("{0}{1}", arg, "\0");
			}
			stringBuilder.Append("\0");
			byte[] bytes = Encoding.Unicode.GetBytes(stringBuilder.ToString());
			OfflineRegUtils.SetValue(this.m_handle, valueName, RegistryValueType.MultiString, bytes);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00005AEC File Offset: 0x00003CEC
		public void SetValue(string valueName, int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			OfflineRegUtils.SetValue(this.m_handle, valueName, RegistryValueType.DWord, bytes);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005B10 File Offset: 0x00003D10
		public void SetValue(string valueName, long value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			OfflineRegUtils.SetValue(this.m_handle, valueName, RegistryValueType.QWord, bytes);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00005B33 File Offset: 0x00003D33
		public void DeleteValue(string valueName)
		{
			OfflineRegUtils.DeleteValue(this.m_handle, valueName);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00005B41 File Offset: 0x00003D41
		public void DeleteKey(string keyName)
		{
			OfflineRegUtils.DeleteKey(this.m_handle, keyName);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00005B50 File Offset: 0x00003D50
		private string CombineSubKeys(string path1, string path2)
		{
			if (path1 == null)
			{
				throw new ArgumentNullException("path1", "The first registry key path to combine cannot be null.");
			}
			if (path2 == null)
			{
				throw new ArgumentNullException("path2", "The second registry key path to combine cannot be null.");
			}
			if (-1 < path2.IndexOf("\\", StringComparison.OrdinalIgnoreCase) || path1.Length == 0)
			{
				return path2;
			}
			if (path2.Length == 0)
			{
				return path1;
			}
			if (path1.Length == path1.LastIndexOfAny(this.BSLASH_DELIMITER) + 1)
			{
				return path1 + path2;
			}
			return path1 + this.BSLASH_DELIMITER[0].ToString() + path2;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00005BE2 File Offset: 0x00003DE2
		private void Close()
		{
			if (this.m_handle != IntPtr.Zero)
			{
				if (this.m_isRoot)
				{
					OfflineRegUtils.CloseHive(this.m_handle);
				}
				else
				{
					OfflineRegUtils.CloseKey(this.m_handle);
				}
				this.m_handle = IntPtr.Zero;
			}
		}

		// Token: 0x04000039 RID: 57
		private IntPtr m_handle = IntPtr.Zero;

		// Token: 0x0400003A RID: 58
		private string m_name = string.Empty;

		// Token: 0x0400003B RID: 59
		private bool m_isRoot;

		// Token: 0x0400003C RID: 60
		private ORRegistryKey m_parent;

		// Token: 0x0400003D RID: 61
		private const string STR_ROOT = "\\";

		// Token: 0x0400003E RID: 62
		private const string STR_NULLCHAR = "\0";

		// Token: 0x0400003F RID: 63
		private readonly char[] BSLASH_DELIMITER = new char[]
		{
			'\\'
		};

		// Token: 0x04000040 RID: 64
		private Dictionary<ORRegistryKey, bool> m_children = new Dictionary<ORRegistryKey, bool>();
	}
}
