using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200004F RID: 79
	public sealed class OfflineRegistryHandle : SafeHandle
	{
		// Token: 0x060003A3 RID: 931 RVA: 0x00011290 File Offset: 0x0000F490
		public OfflineRegistryHandle(string hivePath) : base(IntPtr.Zero, true)
		{
			this._registryHandle = Win32Exports.OfflineRegistryOpenHive(hivePath);
			this._hive = true;
			this._name = hivePath;
			this._path = "";
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x000112C3 File Offset: 0x0000F4C3
		private OfflineRegistryHandle(IntPtr subKeyHandle, string name, string path) : base(IntPtr.Zero, true)
		{
			this._registryHandle = subKeyHandle;
			this._hive = false;
			this._name = name;
			this._path = path;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x000112ED File Offset: 0x0000F4ED
		public void SaveHive(string path)
		{
			if (this._hive)
			{
				Win32Exports.OfflineRegistrySaveHive(this._registryHandle, path);
				return;
			}
			throw new ImageStorageException(string.Format("{0}: This function can only be called on a hive handle.", MethodBase.GetCurrentMethod().Name));
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x0001131D File Offset: 0x0000F51D
		public IntPtr UnsafeHandle
		{
			get
			{
				return this._registryHandle;
			}
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x0001131D File Offset: 0x0000F51D
		public static implicit operator IntPtr(OfflineRegistryHandle offlineRegistryHandle)
		{
			return offlineRegistryHandle._registryHandle;
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00011325 File Offset: 0x0000F525
		public override bool IsInvalid
		{
			get
			{
				return this._disposed;
			}
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x00011330 File Offset: 0x0000F530
		protected override bool ReleaseHandle()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				GC.SuppressFinalize(this);
				if (this._registryHandle != IntPtr.Zero)
				{
					if (this._hive)
					{
						Win32Exports.OfflineRegistryCloseHive(this._registryHandle);
					}
					else
					{
						Win32Exports.OfflineRegistryCloseSubKey(this._registryHandle);
					}
				}
			}
			return true;
		}

		// Token: 0x060003AA RID: 938 RVA: 0x00011388 File Offset: 0x0000F588
		public string[] GetSubKeyNames()
		{
			List<string> list = new List<string>();
			uint num = 0U;
			string text;
			do
			{
				text = Win32Exports.OfflineRegistryEnumKey(this._registryHandle, num++);
				if (text != null)
				{
					list.Add(text);
				}
			}
			while (text != null);
			return list.ToArray();
		}

		// Token: 0x060003AB RID: 939 RVA: 0x000113C4 File Offset: 0x0000F5C4
		public string[] GetValueNames()
		{
			List<string> list = new List<string>();
			uint num = 0U;
			string text;
			do
			{
				text = Win32Exports.OfflineRegistryEnumValue(this._registryHandle, num++);
				if (text != null)
				{
					list.Add(text);
				}
			}
			while (text != null);
			return list.ToArray();
		}

		// Token: 0x060003AC RID: 940 RVA: 0x00011400 File Offset: 0x0000F600
		public OfflineRegistryHandle OpenSubKey(string keyName)
		{
			IntPtr intPtr = Win32Exports.OfflineRegistryOpenSubKey(this._registryHandle, keyName);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return new OfflineRegistryHandle(intPtr, keyName, this.Path);
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00011436 File Offset: 0x0000F636
		public RegistryValueKind GetValueKind(string valueName)
		{
			return OfflineRegistryHandle.GetValueKind(Win32Exports.OfflineRegistryGetValueKind(this._registryHandle, valueName));
		}

		// Token: 0x060003AE RID: 942 RVA: 0x00011449 File Offset: 0x0000F649
		[CLSCompliant(false)]
		public uint GetValueSize(string valueName)
		{
			return Win32Exports.OfflineRegistryGetValueSize(this._registryHandle, valueName);
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00011457 File Offset: 0x0000F657
		public object GetValue(string valueName)
		{
			return Win32Exports.OfflineRegistryGetValue(this._registryHandle, valueName);
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00011468 File Offset: 0x0000F668
		public object GetValue(string valueName, object defaultValue)
		{
			object result = defaultValue;
			try
			{
				result = Win32Exports.OfflineRegistryGetValue(this._registryHandle, valueName);
			}
			catch (ImageStorageException)
			{
			}
			return result;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0001149C File Offset: 0x0000F69C
		public void SetValue(string valueName, byte[] binaryData)
		{
			Win32Exports.OfflineRegistrySetValue(this._registryHandle, valueName, Win32Exports.OfflineRegistryGetValueKind(this._registryHandle, valueName), binaryData);
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x000114B7 File Offset: 0x0000F6B7
		[CLSCompliant(false)]
		public void SetValue(string valueName, byte[] binaryData, uint valueType)
		{
			Win32Exports.OfflineRegistrySetValue(this._registryHandle, valueName, valueType, binaryData);
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x000114C8 File Offset: 0x0000F6C8
		public void SetValue(string valueName, string stringData)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(stringData);
			this.SetValue(valueName, bytes, 1U);
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x000114EC File Offset: 0x0000F6EC
		public void SetValue(string valueName, List<string> values)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in values)
			{
				stringBuilder.Append(value);
				stringBuilder.Append('\0');
			}
			byte[] bytes = Encoding.Unicode.GetBytes(stringBuilder.ToString());
			this.SetValue(valueName, bytes, 7U);
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00011564 File Offset: 0x0000F764
		[CLSCompliant(false)]
		public void SetValue(string valueName, uint value)
		{
			byte[] array = new byte[]
			{
				0,
				0,
				0,
				(byte)(value >> 24 & 255U)
			};
			array[2] = (byte)(value >> 16 & 255U);
			array[1] = (byte)(value >> 8 & 255U);
			array[0] = (byte)(value & 255U);
			this.SetValue(valueName, array, 4U);
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x000115B5 File Offset: 0x0000F7B5
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060003B7 RID: 951 RVA: 0x000115BD File Offset: 0x0000F7BD
		public string Path
		{
			get
			{
				if (this._hive)
				{
					return "[" + this.Name + "]";
				}
				return this._path + "\\" + this.Name;
			}
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x000115F3 File Offset: 0x0000F7F3
		public OfflineRegistryHandle CreateSubKey(string subKey)
		{
			return new OfflineRegistryHandle(Win32Exports.OfflineRegistryCreateKey(this, subKey), subKey, this.Path);
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0001160D File Offset: 0x0000F80D
		public override string ToString()
		{
			return this.Path;
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00011618 File Offset: 0x0000F818
		private static RegistryValueKind GetValueKind(uint valueType)
		{
			RegistryValueKind result = RegistryValueKind.Unknown;
			switch (valueType)
			{
			case 0U:
				result = RegistryValueKind.None;
				break;
			case 1U:
				result = RegistryValueKind.String;
				break;
			case 2U:
				result = RegistryValueKind.ExpandString;
				break;
			case 3U:
				result = RegistryValueKind.Binary;
				break;
			case 4U:
				result = RegistryValueKind.DWord;
				break;
			case 5U:
				result = RegistryValueKind.DWord;
				break;
			case 6U:
				result = RegistryValueKind.String;
				break;
			case 7U:
				result = RegistryValueKind.MultiString;
				break;
			case 8U:
				result = RegistryValueKind.MultiString;
				break;
			case 9U:
				result = RegistryValueKind.String;
				break;
			case 10U:
				result = RegistryValueKind.Binary;
				break;
			case 11U:
				result = RegistryValueKind.QWord;
				break;
			}
			return result;
		}

		// Token: 0x040001F3 RID: 499
		private readonly IntPtr _registryHandle;

		// Token: 0x040001F4 RID: 500
		private string _name;

		// Token: 0x040001F5 RID: 501
		private string _path;

		// Token: 0x040001F6 RID: 502
		private bool _disposed;

		// Token: 0x040001F7 RID: 503
		private bool _hive;
	}
}
