using System;
using System.Collections.ObjectModel;
using System.Management;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200002B RID: 43
	public class ImagePartitionCollection : Collection<ImagePartition>
	{
		// Token: 0x06000168 RID: 360 RVA: 0x00008320 File Offset: 0x00006520
		public void PopulateFromPhysicalDeviceId(string deviceId)
		{
			foreach (ManagementBaseObject managementBaseObject in new ManagementObjectSearcher(new RelatedObjectQuery(string.Format("\\\\.\\root\\cimv2:Win32_DiskDrive.DeviceID='{0}'", deviceId), "Win32_DiskPartition")).Get())
			{
				ManagementObject managementObject = (ManagementObject)managementBaseObject;
				base.Add(new VHDImagePartition(deviceId, managementObject.GetPropertyValue("Name").ToString()));
			}
		}

		// Token: 0x04000082 RID: 130
		private const string WMI_GETPARTITIONS_QUERY = "\\\\.\\root\\cimv2:Win32_DiskDrive.DeviceID='{0}'";

		// Token: 0x04000083 RID: 131
		private const string WMI_DISKPARTITION_CLASS = "Win32_DiskPartition";

		// Token: 0x04000084 RID: 132
		private const string STR_NAME = "Name";
	}
}
