using System;

namespace FFUComponents
{
	// Token: 0x02000026 RID: 38
	public interface IFFUDevice : IDisposable
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000ED RID: 237
		string DeviceFriendlyName { get; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000EE RID: 238
		Guid DeviceUniqueID { get; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000EF RID: 239
		Guid SerialNumber { get; }

		// Token: 0x060000F0 RID: 240
		void FlashFFUFile(string ffuFilePath);

		// Token: 0x060000F1 RID: 241
		void FlashFFUFile(string ffuFilePath, bool optimize);

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x060000F2 RID: 242
		// (remove) Token: 0x060000F3 RID: 243
		event EventHandler<ProgressEventArgs> ProgressEvent;

		// Token: 0x060000F4 RID: 244
		bool WriteWim(string wimPath);

		// Token: 0x060000F5 RID: 245
		bool EndTransfer();

		// Token: 0x060000F6 RID: 246
		bool SkipTransfer();

		// Token: 0x060000F7 RID: 247
		bool Reboot();

		// Token: 0x060000F8 RID: 248
		bool EnterMassStorage();

		// Token: 0x060000F9 RID: 249
		bool ClearIdOverride();

		// Token: 0x060000FA RID: 250
		bool GetDiskInfo(out uint blockSize, out ulong lastBlock);

		// Token: 0x060000FB RID: 251
		void ReadDisk(ulong diskOffset, byte[] buffer, int offset, int count);

		// Token: 0x060000FC RID: 252
		void WriteDisk(ulong diskOffset, byte[] buffer, int offset, int count);

		// Token: 0x060000FD RID: 253
		uint SetBootMode(uint bootMode, string profileName);

		// Token: 0x060000FE RID: 254
		string GetServicingLogs(string logFolderPath);

		// Token: 0x060000FF RID: 255
		string GetFlashingLogs(string logFolderPath);

		// Token: 0x06000100 RID: 256
		void QueryDeviceUnlockId(out byte[] unlockId, out byte[] oemId, out byte[] platformId);

		// Token: 0x06000101 RID: 257
		void RelockDeviceUnlockId();

		// Token: 0x06000102 RID: 258
		uint[] QueryUnlockTokenFiles();

		// Token: 0x06000103 RID: 259
		void WriteUnlockTokenFile(uint unlockTokenId, byte[] fileData);

		// Token: 0x06000104 RID: 260
		bool QueryBitlockerState();

		// Token: 0x06000105 RID: 261
		void Unlock(uint tokenId, string tokenFilePath, string pin);

		// Token: 0x06000106 RID: 262
		byte[] GetDeviceProperties();

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000107 RID: 263
		string DeviceType { get; }
	}
}
