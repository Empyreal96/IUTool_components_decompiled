using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000015 RID: 21
	public class ORRegistryKey : IDisposable
	{
		// Token: 0x060000BA RID: 186 RVA: 0x00004068 File Offset: 0x00002268
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

		// Token: 0x060000BB RID: 187 RVA: 0x000040E4 File Offset: 0x000022E4
		~ORRegistryKey()
		{
			this.Dispose(false);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00004114 File Offset: 0x00002314
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00004124 File Offset: 0x00002324
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
				}
			}
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000041A4 File Offset: 0x000023A4
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static ORRegistryKey OpenHive(string hivefile, string prefix = null)
		{
			if (prefix == null)
			{
				prefix = "\\";
			}
			return new ORRegistryKey(prefix, OfflineRegUtils.OpenHive(hivefile), true, null);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000041BE File Offset: 0x000023BE
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static ORRegistryKey CreateEmptyHive(string prefix = null)
		{
			return new ORRegistryKey(string.IsNullOrEmpty(prefix) ? "\\" : prefix, OfflineRegUtils.CreateHive(), true, null);
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x000041DC File Offset: 0x000023DC
		public ORRegistryKey Parent
		{
			get
			{
				return this.m_parent;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x000041E4 File Offset: 0x000023E4
		[SuppressMessage("Microsoft.Performance", "CA1819")]
		public string[] SubKeys
		{
			get
			{
				return OfflineRegUtils.GetSubKeys(this.m_handle);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x000041F1 File Offset: 0x000023F1
		public string FullName
		{
			get
			{
				return this.m_name;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x000041F9 File Offset: 0x000023F9
		public string Class
		{
			get
			{
				return OfflineRegUtils.GetClass(this.m_handle);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00004206 File Offset: 0x00002406
		public ReadOnlyCollection<string> ValueNames
		{
			get
			{
				return new ReadOnlyCollection<string>(OfflineRegUtils.GetValueNames(this.m_handle));
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00004218 File Offset: 0x00002418
		public List<KeyValuePair<string, RegistryValueType>> ValueNameAndTypes
		{
			get
			{
				return OfflineRegUtils.GetValueNamesAndTypes(this.m_handle);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00004225 File Offset: 0x00002425
		public RegistrySecurity RegistrySecurity
		{
			get
			{
				return OfflineRegUtils.GetRegistrySecurity(this.m_handle);
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00004234 File Offset: 0x00002434
		public ORRegistryKey OpenSubKey(string subkeyname)
		{
			if (subkeyname == null)
			{
				throw new ArgumentNullException("subkeyname");
			}
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

		// Token: 0x060000C8 RID: 200 RVA: 0x000042C3 File Offset: 0x000024C3
		public RegistryValueType GetValueKind(string valueName)
		{
			return OfflineRegUtils.GetValueType(this.m_handle, valueName);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000042D1 File Offset: 0x000024D1
		public byte[] GetByteValue(string valueName)
		{
			return OfflineRegUtils.GetValue(this.m_handle, valueName);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000042E0 File Offset: 0x000024E0
		public uint GetDwordValue(string valueName)
		{
			byte[] byteValue = this.GetByteValue(valueName);
			if (byteValue.Length != 0)
			{
				return BitConverter.ToUInt32(byteValue, 0);
			}
			return 0U;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00004304 File Offset: 0x00002504
		public ulong GetQwordValue(string valueName)
		{
			byte[] byteValue = this.GetByteValue(valueName);
			if (byteValue.Length != 0)
			{
				return BitConverter.ToUInt64(byteValue, 0);
			}
			return 0UL;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00004328 File Offset: 0x00002528
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

		// Token: 0x060000CD RID: 205 RVA: 0x0000437C File Offset: 0x0000257C
		public string[] GetMultiStringValue(string valueName)
		{
			byte[] byteValue = this.GetByteValue(valueName);
			return Encoding.Unicode.GetString(byteValue).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000043A8 File Offset: 0x000025A8
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

		// Token: 0x060000CF RID: 207 RVA: 0x0000447B File Offset: 0x0000267B
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

		// Token: 0x060000D0 RID: 208 RVA: 0x000044B4 File Offset: 0x000026B4
		public ORRegistryKey CreateSubKey(string subkeyName)
		{
			if (subkeyName == null)
			{
				throw new ArgumentNullException("subkeyName");
			}
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

		// Token: 0x060000D1 RID: 209 RVA: 0x00004544 File Offset: 0x00002744
		public void SetValue(string valueName, byte[] value)
		{
			OfflineRegUtils.SetValue(this.m_handle, valueName, RegistryValueType.Binary, value);
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00004554 File Offset: 0x00002754
		public void SetValue(string valueName, string value)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(value);
			OfflineRegUtils.SetValue(this.m_handle, valueName, RegistryValueType.String, bytes);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000457C File Offset: 0x0000277C
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

		// Token: 0x060000D4 RID: 212 RVA: 0x000045F8 File Offset: 0x000027F8
		public void SetValue(string valueName, int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			OfflineRegUtils.SetValue(this.m_handle, valueName, RegistryValueType.DWord, bytes);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000461C File Offset: 0x0000281C
		public void SetValue(string valueName, long value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			OfflineRegUtils.SetValue(this.m_handle, valueName, RegistryValueType.QWord, bytes);
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000463F File Offset: 0x0000283F
		public void DeleteValue(string valueName)
		{
			OfflineRegUtils.DeleteValue(this.m_handle, valueName);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000464D File Offset: 0x0000284D
		public void DeleteKey(string keyName)
		{
			OfflineRegUtils.DeleteKey(this.m_handle, keyName);
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000465C File Offset: 0x0000285C
		private string CombineSubKeys(string path1, string path2)
		{
			if (path1 == null)
			{
				throw new ArgumentNullException("path1");
			}
			if (path2 == null)
			{
				throw new ArgumentNullException("path2");
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

		// Token: 0x060000D9 RID: 217 RVA: 0x000046E4 File Offset: 0x000028E4
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

		// Token: 0x0400003A RID: 58
		private IntPtr m_handle = IntPtr.Zero;

		// Token: 0x0400003B RID: 59
		private string m_name = string.Empty;

		// Token: 0x0400003C RID: 60
		private bool m_isRoot;

		// Token: 0x0400003D RID: 61
		private ORRegistryKey m_parent;

		// Token: 0x0400003E RID: 62
		private const string STR_ROOT = "\\";

		// Token: 0x0400003F RID: 63
		private const string STR_NULLCHAR = "\0";

		// Token: 0x04000040 RID: 64
		private readonly char[] BSLASH_DELIMITER = new char[]
		{
			'\\'
		};

		// Token: 0x04000041 RID: 65
		private Dictionary<ORRegistryKey, bool> m_children = new Dictionary<ORRegistryKey, bool>();
	}
}
