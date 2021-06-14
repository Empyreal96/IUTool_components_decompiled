using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200001B RID: 27
	public abstract class MobileCoreImage
	{
		// Token: 0x060000FD RID: 253 RVA: 0x00006D7E File Offset: 0x00004F7E
		protected MobileCoreImage(string path)
		{
			this.m_mobileCoreImagePath = path;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00006D98 File Offset: 0x00004F98
		public static MobileCoreImage Create(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException(path);
			}
			if (!LongPathFile.Exists(path))
			{
				throw new FileNotFoundException(string.Format("The specified file ({0}) is not a valid VHD image.", path));
			}
			FileInfo fileInfo = new FileInfo(path);
			MobileCoreImage result;
			if (fileInfo.Extension.Equals(".VHD", StringComparison.OrdinalIgnoreCase))
			{
				result = new MobileCoreVHD(path);
			}
			else
			{
				if (!fileInfo.Extension.Equals(".WIM", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException(string.Format("The specified file ({0}) is not a valid VHD image.", path));
				}
				result = new MobileCoreWIM(path);
			}
			return result;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000FF RID: 255 RVA: 0x00006E20 File Offset: 0x00005020
		public string ImagePath
		{
			get
			{
				return this.m_mobileCoreImagePath;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00006E28 File Offset: 0x00005028
		// (set) Token: 0x06000101 RID: 257 RVA: 0x00006E30 File Offset: 0x00005030
		public bool IsMounted { get; protected set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00006E3C File Offset: 0x0000503C
		public ReadOnlyCollection<ImagePartition> Partitions
		{
			get
			{
				ReadOnlyCollection<ImagePartition> result = null;
				if (this.IsMounted)
				{
					result = new ReadOnlyCollection<ImagePartition>(this.m_partitions);
				}
				return result;
			}
		}

		// Token: 0x06000103 RID: 259
		public abstract void Mount();

		// Token: 0x06000104 RID: 260
		public abstract void MountReadOnly();

		// Token: 0x06000105 RID: 261
		public abstract void Unmount();

		// Token: 0x06000106 RID: 262 RVA: 0x00006E60 File Offset: 0x00005060
		public ImagePartition GetPartition(MobileCorePartitionType type)
		{
			ImagePartition imagePartition = null;
			if (!this.IsMounted)
			{
				return null;
			}
			foreach (ImagePartition imagePartition2 in this.Partitions)
			{
				if (imagePartition2.Root != null && type == MobileCorePartitionType.System && LongPathDirectory.Exists(Path.Combine(imagePartition2.Root, "Windows\\System32")))
				{
					imagePartition = imagePartition2;
				}
			}
			if (imagePartition == null)
			{
				throw new IUException("Request partition {0} cannot be found in the image", new object[]
				{
					Enum.GetName(typeof(MobileCorePartitionType), type)
				});
			}
			return imagePartition;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00006F04 File Offset: 0x00005104
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.IsMounted)
			{
				using (IEnumerator<ImagePartition> enumerator = this.Partitions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ImagePartition imagePartition = enumerator.Current;
						stringBuilder.AppendFormat("{0}, Root = {1}", imagePartition.Name, imagePartition.Root);
					}
					goto IL_5B;
				}
			}
			stringBuilder.AppendLine("This image is not mounted");
			IL_5B:
			return stringBuilder.ToString();
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00006F84 File Offset: 0x00005184
		public AclCollection GetFileSystemACLs()
		{
			ImagePartition partition = this.GetPartition(MobileCorePartitionType.System);
			bool flag = false;
			if (!this.IsMounted)
			{
				this.Mount();
				flag = true;
			}
			AclCollection result = null;
			try
			{
				result = SecurityUtils.GetFileSystemACLs(partition.Root);
			}
			finally
			{
				if (flag)
				{
					this.Unmount();
				}
			}
			return result;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00006FD8 File Offset: 0x000051D8
		public AclCollection GetRegistryACLs()
		{
			ImagePartition partition = this.GetPartition(MobileCorePartitionType.System);
			bool flag = false;
			if (!this.IsMounted)
			{
				this.Mount();
				flag = true;
			}
			AclCollection aclCollection = null;
			try
			{
				aclCollection = new AclCollection();
				string hiveRoot = Path.Combine(partition.Root, "Windows\\System32\\Config");
				aclCollection.UnionWith(SecurityUtils.GetRegistryACLs(hiveRoot));
			}
			finally
			{
				if (flag)
				{
					this.Unmount();
				}
			}
			return aclCollection;
		}

		// Token: 0x0400004E RID: 78
		protected string m_mobileCoreImagePath;

		// Token: 0x0400004F RID: 79
		protected ImagePartitionCollection m_partitions = new ImagePartitionCollection();

		// Token: 0x04000050 RID: 80
		private const string EXTENSION_VHD = ".VHD";

		// Token: 0x04000051 RID: 81
		private const string EXTENSION_WIM = ".WIM";

		// Token: 0x04000052 RID: 82
		private const string ERROR_IMAGENOTFOUND = "The specified file ({0}) either does not exist or cannot be read.";

		// Token: 0x04000053 RID: 83
		private const string ERROR_INVALIDIMAGE = "The specified file ({0}) is not a valid VHD image.";

		// Token: 0x04000054 RID: 84
		private const string STR_HIVE_PATH = "Windows\\System32\\Config";

		// Token: 0x04000055 RID: 85
		private const string ERROR_NO_SUCH_PARTITION = "Request partition {0} cannot be found in the image";

		// Token: 0x04000056 RID: 86
		private const string STR_SYSTEM32_DIR = "Windows\\System32";
	}
}
