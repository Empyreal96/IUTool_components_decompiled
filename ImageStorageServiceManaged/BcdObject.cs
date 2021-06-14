using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200000B RID: 11
	public class BcdObject
	{
		// Token: 0x06000033 RID: 51 RVA: 0x00004044 File Offset: 0x00002244
		public BcdObject(string objectName)
		{
			this.Name = objectName;
			try
			{
				this.Id = new Guid(objectName);
			}
			catch (Exception innerException)
			{
				throw new ImageStorageException(string.Format("{0}: Object '{1}' isn't a valid ID.", MethodBase.GetCurrentMethod().Name, objectName), innerException);
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000040A4 File Offset: 0x000022A4
		[CLSCompliant(false)]
		public BcdObject(Guid objectId, uint dataType)
		{
			this.Id = objectId;
			this.Name = string.Format("{{{0}}}", objectId);
			this.Type = dataType;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000035 RID: 53 RVA: 0x000040DB File Offset: 0x000022DB
		// (set) Token: 0x06000036 RID: 54 RVA: 0x000040E3 File Offset: 0x000022E3
		public string Name { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000040EC File Offset: 0x000022EC
		// (set) Token: 0x06000038 RID: 56 RVA: 0x000040F4 File Offset: 0x000022F4
		public Guid Id { get; set; }

		// Token: 0x06000039 RID: 57 RVA: 0x00004100 File Offset: 0x00002300
		public void ReadFromRegistry(OfflineRegistryHandle objectKey)
		{
			OfflineRegistryHandle offlineRegistryHandle = null;
			OfflineRegistryHandle offlineRegistryHandle2 = null;
			try
			{
				offlineRegistryHandle = objectKey.OpenSubKey("Description");
				uint type = (uint)offlineRegistryHandle.GetValue("Type", 0);
				this.Type = type;
			}
			catch (ImageStorageException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ImageStorageException(string.Format("{0}: There was a problem accessing the Description key for object {1}", MethodBase.GetCurrentMethod().Name, this.Name), innerException);
			}
			finally
			{
				if (offlineRegistryHandle != null)
				{
					offlineRegistryHandle.Close();
					offlineRegistryHandle = null;
				}
			}
			try
			{
				offlineRegistryHandle2 = objectKey.OpenSubKey("Elements");
				foreach (string text in offlineRegistryHandle2.GetSubKeyNames())
				{
					OfflineRegistryHandle offlineRegistryHandle3 = null;
					try
					{
						offlineRegistryHandle3 = offlineRegistryHandle2.OpenSubKey(text);
						this.Elements.Add(BcdElement.CreateElement(offlineRegistryHandle3));
					}
					catch (Exception innerException2)
					{
						throw new ImageStorageException(string.Format("{0}: There was a problem accessing element {1} for object {{{2}}}", MethodBase.GetCurrentMethod().Name, text, this.Name), innerException2);
					}
					finally
					{
						if (offlineRegistryHandle3 != null)
						{
							offlineRegistryHandle3.Close();
							offlineRegistryHandle3 = null;
						}
					}
				}
			}
			catch (ImageStorageException)
			{
				throw;
			}
			catch (Exception innerException3)
			{
				throw new ImageStorageException(string.Format("{0}: There was a problem accessing the Elements key for object {1}", MethodBase.GetCurrentMethod().Name, this.Name), innerException3);
			}
			finally
			{
				offlineRegistryHandle2.Close();
				offlineRegistryHandle2 = null;
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000428C File Offset: 0x0000248C
		public void AddElement(BcdElement element)
		{
			using (List<BcdElement>.Enumerator enumerator = this.Elements.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.DataType == element.DataType)
					{
						throw new ImageStorageException(string.Format("{0}: A bcd element with the given datatype already exists.", MethodBase.GetCurrentMethod().Name));
					}
				}
			}
			this.Elements.Add(element);
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003B RID: 59 RVA: 0x0000430C File Offset: 0x0000250C
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00004314 File Offset: 0x00002514
		[CLSCompliant(false)]
		public uint Type { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003D RID: 61 RVA: 0x0000431D File Offset: 0x0000251D
		public List<BcdElement> Elements
		{
			get
			{
				return this._elements;
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004328 File Offset: 0x00002528
		public Guid GetDefaultObjectId()
		{
			Guid empty = Guid.Empty;
			foreach (BcdElement bcdElement in this.Elements)
			{
				if (bcdElement.DataType.Equals(BcdElementDataTypes.DefaultObject))
				{
					BcdElementObject bcdElementObject = bcdElement as BcdElementObject;
					if (bcdElementObject != null)
					{
						return bcdElementObject.ElementObject;
					}
				}
			}
			return empty;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000043A4 File Offset: 0x000025A4
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "BCD Object: {{{0}}}", new object[]
			{
				this.Id
			});
			if (BcdObjects.BootObjectList.ContainsKey(this.Id))
			{
				logger.LogInfo(str + "Friendly Name: {0}", new object[]
				{
					BcdObjects.BootObjectList[this.Id].Name
				});
			}
			logger.LogInfo("", new object[0]);
			foreach (BcdElement bcdElement in this.Elements)
			{
				bcdElement.LogInfo(logger, checked(indentLevel + 2));
				logger.LogInfo("", new object[0]);
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00004494 File Offset: 0x00002694
		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			return "Unnamed BcdObject";
		}

		// Token: 0x040000B7 RID: 183
		private List<BcdElement> _elements = new List<BcdElement>();
	}
}
