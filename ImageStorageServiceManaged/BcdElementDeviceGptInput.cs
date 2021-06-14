using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200006F RID: 111
	public class BcdElementDeviceGptInput
	{
		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060004B6 RID: 1206 RVA: 0x00014A91 File Offset: 0x00012C91
		// (set) Token: 0x060004B7 RID: 1207 RVA: 0x00014A99 File Offset: 0x00012C99
		public string DiskId { get; set; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060004B8 RID: 1208 RVA: 0x00014AA2 File Offset: 0x00012CA2
		// (set) Token: 0x060004B9 RID: 1209 RVA: 0x00014AAA File Offset: 0x00012CAA
		public GptPartitionInput Partition { get; set; }

		// Token: 0x060004BA RID: 1210 RVA: 0x00014AB4 File Offset: 0x00012CB4
		public static BcdElementDevice CreateGptBootDevice(BcdElementDeviceGptInput inputValue)
		{
			BcdElementDevice bcdElementDevice = BcdElementDevice.CreateBaseBootDevice();
			Guid diskId = Guid.Empty;
			Guid partitionId = Guid.Empty;
			try
			{
				diskId = new Guid(inputValue.DiskId);
				partitionId = inputValue.Partition.PartitionId;
			}
			catch (Exception innerException)
			{
				throw new ImageStorageException("Unable to parse the GPTDevice value.", innerException);
			}
			PartitionIdentifierEx identifier = PartitionIdentifierEx.CreateSimpleGpt(diskId, partitionId);
			bcdElementDevice.ReplaceBootDeviceIdentifier(identifier);
			return bcdElementDevice;
		}
	}
}
