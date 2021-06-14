using System;
using System.Globalization;
using System.IO;
using System.Management;
using System.Threading;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200002C RID: 44
	public class VHDImagePartition : ImagePartition
	{
		// Token: 0x0600016A RID: 362 RVA: 0x000083A8 File Offset: 0x000065A8
		public VHDImagePartition(string deviceId, string partitionId)
		{
			base.PhysicalDeviceId = deviceId;
			base.Name = partitionId;
			string text = string.Empty;
			int num = 10;
			int num2 = 0;
			bool flag;
			do
			{
				flag = false;
				text = this.GetLogicalDriveFromWMI(deviceId, partitionId);
				if (string.IsNullOrEmpty(text))
				{
					Console.WriteLine("  ImagePartition.GetLogicalDriveFromWMI({0}, {1}) not found, sleeping...", deviceId, partitionId);
					num2++;
					flag = (num2 < num);
					Thread.Sleep(500);
				}
			}
			while (flag);
			if (string.IsNullOrEmpty(text))
			{
				throw new IUException("Failed to retrieve logical drive name of partition {0} using WMI", new object[]
				{
					partitionId
				});
			}
			if (string.Compare(text, "NONE", true, CultureInfo.InvariantCulture) != 0)
			{
				base.MountedDriveInfo = new DriveInfo(Path.GetPathRoot(text));
				base.Root = base.MountedDriveInfo.RootDirectory.FullName;
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00008464 File Offset: 0x00006664
		private string GetLogicalDriveFromWMI(string deviceId, string partitionId)
		{
			string result = string.Empty;
			bool flag = false;
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(string.Format("Select * from Win32_DiskPartition where Name='{0}'", partitionId)))
			{
				foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
				{
					ManagementObject managementObject = (ManagementObject)managementBaseObject;
					Console.WriteLine("  ImagePartition.GetLogicalDriveFromWMI: Path={0}", managementObject.Path.ToString());
					if (string.Compare(managementObject.GetPropertyValue("Type").ToString(), "unknown", true, CultureInfo.InvariantCulture) == 0)
					{
						result = "NONE";
						break;
					}
					using (ManagementObjectCollection.ManagementObjectEnumerator enumerator2 = new ManagementObjectSearcher(new RelatedObjectQuery(managementObject.Path.ToString(), "Win32_LogicalDisk")).Get().GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							result = ((ManagementObject)enumerator2.Current).GetPropertyValue("Name").ToString();
							flag = true;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x04000085 RID: 133
		private const string WMI_GETPARTITIONS_QUERY = "Select * from Win32_DiskPartition where Name='{0}'";

		// Token: 0x04000086 RID: 134
		private const string WMI_DISKPARTITION_CLASS = "Win32_DiskPartition";

		// Token: 0x04000087 RID: 135
		private const string WMI_LOGICALDISK_CLASS = "Win32_LogicalDisk";

		// Token: 0x04000088 RID: 136
		private const string STR_NAME = "Name";

		// Token: 0x04000089 RID: 137
		private const int MAX_RETRY = 10;

		// Token: 0x0400008A RID: 138
		private const int SLEEP_500 = 500;
	}
}
