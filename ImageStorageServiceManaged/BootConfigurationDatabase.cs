using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000004 RID: 4
	public class BootConfigurationDatabase : IDisposable
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00003358 File Offset: 0x00001558
		~BootConfigurationDatabase()
		{
			this.Dispose(false);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00003388 File Offset: 0x00001588
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00003397 File Offset: 0x00001597
		protected virtual void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				this._filePath = null;
				this._logger = null;
			}
			if (this._bcdKey != null)
			{
				this._bcdKey.Close();
				this._bcdKey = null;
			}
			this._alreadyDisposed = true;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000033D4 File Offset: 0x000015D4
		public BootConfigurationDatabase(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ImageStorageException(string.Format("{0}: The filePath is empty or null.", MethodBase.GetCurrentMethod().Name));
			}
			if (!File.Exists(filePath))
			{
				throw new ImageStorageException(string.Format("{0}: The file ({1}) does not exist or is inaccessible.", MethodBase.GetCurrentMethod().Name, filePath));
			}
			this._filePath = filePath;
			this.Objects = new List<BcdObject>();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00003449 File Offset: 0x00001649
		[CLSCompliant(false)]
		public BootConfigurationDatabase(string filePath, IULogger logger) : this(filePath)
		{
			this._logger = logger;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000345C File Offset: 0x0000165C
		public void Mount()
		{
			this._bcdKey = new OfflineRegistryHandle(this._filePath);
			try
			{
				OfflineRegistryHandle offlineRegistryHandle = null;
				try
				{
					offlineRegistryHandle = this._bcdKey.OpenSubKey("Objects");
				}
				catch (Exception innerException)
				{
					throw new ImageStorageException(string.Format("{0}: The BCD hive is invalid.  Unable to open the 'Objects' key.", MethodBase.GetCurrentMethod().Name), innerException);
				}
				try
				{
					string[] subKeyNames = offlineRegistryHandle.GetSubKeyNames();
					if (subKeyNames == null || subKeyNames.Length == 0)
					{
						throw new ImageStorageException(string.Format("{0}: The BCD hive is invalid. There are no keys under 'Objects'.", MethodBase.GetCurrentMethod().Name));
					}
					foreach (string text in subKeyNames)
					{
						BcdObject bcdObject = new BcdObject(text);
						OfflineRegistryHandle offlineRegistryHandle2 = offlineRegistryHandle.OpenSubKey(text);
						try
						{
							bcdObject.ReadFromRegistry(offlineRegistryHandle2);
						}
						finally
						{
							if (offlineRegistryHandle2 != null)
							{
								offlineRegistryHandle2.Close();
								offlineRegistryHandle2 = null;
							}
						}
						this.Objects.Add(bcdObject);
					}
				}
				finally
				{
					if (offlineRegistryHandle != null)
					{
						offlineRegistryHandle.Close();
						offlineRegistryHandle = null;
					}
				}
			}
			catch (ImageStorageException)
			{
				throw;
			}
			catch (Exception innerException2)
			{
				throw new ImageStorageException(string.Format("{0}: Unable to mount and parse the BCD key.", MethodBase.GetCurrentMethod().Name), innerException2);
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000359C File Offset: 0x0000179C
		public void DismountHive(bool save)
		{
			try
			{
				string tempFileName = Path.GetTempFileName();
				File.Delete(tempFileName);
				this._bcdKey.SaveHive(tempFileName);
				this._bcdKey.Close();
				this._bcdKey = null;
				FileAttributes attributes = File.GetAttributes(this._filePath);
				if ((attributes & FileAttributes.ReadOnly) != (FileAttributes)0)
				{
					File.SetAttributes(this._filePath, attributes & ~FileAttributes.ReadOnly);
				}
				File.Delete(this._filePath);
				File.Move(tempFileName, this._filePath);
				File.SetAttributes(this._filePath, attributes);
			}
			catch (Exception innerException)
			{
				throw new ImageStorageException(string.Format("{0}: Unable to save the hive.", MethodBase.GetCurrentMethod().Name), innerException);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00003644 File Offset: 0x00001844
		public BcdObject GetObject(Guid objectId)
		{
			foreach (BcdObject bcdObject in this.Objects)
			{
				if (bcdObject.Id == objectId)
				{
					return bcdObject;
				}
			}
			return null;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000036A8 File Offset: 0x000018A8
		public void AddObject(BcdObject bcdObject)
		{
			using (List<BcdObject>.Enumerator enumerator = this.Objects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Id == bcdObject.Id)
					{
						throw new ImageStorageException(string.Format("{0}: The object already exists in the BCD.", MethodBase.GetCurrentMethod().Name));
					}
				}
			}
			this.Objects.Add(bcdObject);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000372C File Offset: 0x0000192C
		public void LogInfo(int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			this._logger.LogInfo(str + "Boot Configuration Database", new object[0]);
			foreach (BcdObject bcdObject in this.Objects)
			{
				bcdObject.LogInfo(this._logger, checked(indentLevel + 2));
				this._logger.LogInfo("", new object[0]);
			}
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000037CC File Offset: 0x000019CC
		public List<BcdObject> Objects { get; }

		// Token: 0x06000011 RID: 17 RVA: 0x000037D4 File Offset: 0x000019D4
		public void SaveObject(BcdObject bcdObject)
		{
			OfflineRegistryHandle offlineRegistryHandle = null;
			OfflineRegistryHandle offlineRegistryHandle2 = null;
			OfflineRegistryHandle offlineRegistryHandle3 = null;
			OfflineRegistryHandle offlineRegistryHandle4 = null;
			try
			{
				offlineRegistryHandle = this._bcdKey.OpenSubKey("objects");
				offlineRegistryHandle2 = offlineRegistryHandle.CreateSubKey(bcdObject.Name);
				offlineRegistryHandle3 = offlineRegistryHandle2.CreateSubKey("Elements");
				offlineRegistryHandle4 = offlineRegistryHandle2.CreateSubKey("Description");
				offlineRegistryHandle4.SetValue("Type", bcdObject.Type);
			}
			finally
			{
				if (offlineRegistryHandle2 != null)
				{
					offlineRegistryHandle2.Close();
					offlineRegistryHandle2 = null;
				}
				if (offlineRegistryHandle != null)
				{
					offlineRegistryHandle.Close();
					offlineRegistryHandle = null;
				}
				if (offlineRegistryHandle3 != null)
				{
					offlineRegistryHandle3.Close();
					offlineRegistryHandle3 = null;
				}
				if (offlineRegistryHandle4 != null)
				{
					offlineRegistryHandle4.Close();
					offlineRegistryHandle4 = null;
				}
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00003870 File Offset: 0x00001A70
		private void SaveBinaryDeviceElement(OfflineRegistryHandle elementKey, byte[] binaryData)
		{
			elementKey.SetValue("Element", binaryData, 3U);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00003880 File Offset: 0x00001A80
		[Conditional("DEBUG")]
		private void ValidateDeviceElement(BcdObject bcdObject, BcdElement bcdElement, OfflineRegistryHandle elementKey)
		{
			BcdElementDevice bcdElementDevice = bcdElement as BcdElementDevice;
			byte[] array = new byte[bcdElementDevice.BinarySize];
			MemoryStream memoryStream = null;
			try
			{
				memoryStream = new MemoryStream(array);
				bcdElementDevice.WriteToStream(memoryStream);
				byte[] binaryData = bcdElement.GetBinaryData();
				if (binaryData.Length != array.Length)
				{
					throw new ImageStorageException("The binary data length is wrong.");
				}
				for (int i = 0; i < binaryData.Length; i++)
				{
					if (array[i] != binaryData[i])
					{
						throw new ImageStorageException("The binary data is wrong.");
					}
				}
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Close();
					memoryStream = null;
				}
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00003914 File Offset: 0x00001B14
		private void SaveStringDeviceElement(OfflineRegistryHandle elementKey, string value)
		{
			elementKey.SetValue("Element", value);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00003922 File Offset: 0x00001B22
		private void SaveMultiStringDeviceElement(OfflineRegistryHandle elementKey, List<string> values)
		{
			elementKey.SetValue("Element", values);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00003930 File Offset: 0x00001B30
		public void SaveElementValue(BcdObject bcdObject, BcdElement bcdElement)
		{
			OfflineRegistryHandle offlineRegistryHandle = null;
			OfflineRegistryHandle offlineRegistryHandle2 = null;
			OfflineRegistryHandle offlineRegistryHandle3 = null;
			OfflineRegistryHandle offlineRegistryHandle4 = null;
			string text = bcdElement.DataType.ToString();
			try
			{
				offlineRegistryHandle = this._bcdKey.OpenSubKey("Objects");
				offlineRegistryHandle2 = offlineRegistryHandle.OpenSubKey(bcdObject.Name);
				offlineRegistryHandle3 = offlineRegistryHandle2.OpenSubKey("Elements");
				offlineRegistryHandle4 = offlineRegistryHandle3.OpenSubKey(text);
				if (offlineRegistryHandle4 == null)
				{
					offlineRegistryHandle4 = offlineRegistryHandle3.CreateSubKey(text);
				}
				switch (bcdElement.DataType.Format)
				{
				case ElementFormat.Device:
					this.SaveBinaryDeviceElement(offlineRegistryHandle4, bcdElement.GetBinaryData());
					break;
				case ElementFormat.String:
					this.SaveStringDeviceElement(offlineRegistryHandle4, bcdElement.StringData);
					break;
				case ElementFormat.Object:
					this.SaveStringDeviceElement(offlineRegistryHandle4, bcdElement.StringData);
					break;
				case ElementFormat.ObjectList:
					this.SaveMultiStringDeviceElement(offlineRegistryHandle4, bcdElement.MultiStringData);
					break;
				case ElementFormat.Integer:
					this.SaveBinaryDeviceElement(offlineRegistryHandle4, bcdElement.GetBinaryData());
					break;
				case ElementFormat.Boolean:
					this.SaveBinaryDeviceElement(offlineRegistryHandle4, bcdElement.GetBinaryData());
					break;
				case ElementFormat.IntegerList:
					this.SaveBinaryDeviceElement(offlineRegistryHandle4, bcdElement.GetBinaryData());
					break;
				default:
					throw new ImageStorageException(string.Format("{0}: Unknown element format: {1}.", MethodBase.GetCurrentMethod().Name, bcdElement.DataType.RawValue));
				}
			}
			finally
			{
				if (offlineRegistryHandle4 != null)
				{
					offlineRegistryHandle4.Close();
					offlineRegistryHandle4 = null;
				}
				if (offlineRegistryHandle3 != null)
				{
					offlineRegistryHandle3.Close();
					offlineRegistryHandle3 = null;
				}
				if (offlineRegistryHandle2 != null)
				{
					offlineRegistryHandle2.Close();
					offlineRegistryHandle2 = null;
				}
				if (offlineRegistryHandle != null)
				{
					offlineRegistryHandle.Close();
					offlineRegistryHandle = null;
				}
			}
		}

		// Token: 0x04000089 RID: 137
		private string _filePath;

		// Token: 0x0400008A RID: 138
		private OfflineRegistryHandle _bcdKey;

		// Token: 0x0400008B RID: 139
		private IULogger _logger = new IULogger();

		// Token: 0x0400008C RID: 140
		public static readonly string SubKeyName = "BootConfigurationKey";

		// Token: 0x0400008D RID: 141
		private bool _alreadyDisposed;
	}
}
