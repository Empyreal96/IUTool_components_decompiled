using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.WPImage
{
	// Token: 0x02000003 RID: 3
	internal class MountCommand : IWPImageCommand
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002050 File Offset: 0x00000250
		public string Name
		{
			get
			{
				return "Mount";
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002058 File Offset: 0x00000258
		public bool ParseArgs(string[] args)
		{
			if (args.Length < 1)
			{
				return false;
			}
			this._path = args[0];
			if (!File.Exists(this._path))
			{
				Console.Error.WriteLine("The file {0} does not exist.", this._path);
				return false;
			}
			for (int i = 1; i < args.Length; i++)
			{
				if (string.Compare(args[i], "waitForMountedSystemVolume", true, CultureInfo.InvariantCulture) == 0)
				{
					this._waitForSystemVolume = true;
				}
				else if (string.Compare(args[i], "randomGptIds", true, CultureInfo.InvariantCulture) == 0)
				{
					this._randomizeGptIds = true;
				}
			}
			return true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020E3 File Offset: 0x000002E3
		public void PrintUsage()
		{
			Console.WriteLine("{0} {1} image_path", "WPImage.exe", this.Name);
			Console.WriteLine("  image_path should point to a Windows Phone 8 VHD or FFU");
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002104 File Offset: 0x00000304
		public void Run()
		{
			bool flag = true;
			string extension = Path.GetExtension(this._path);
			if (string.IsNullOrEmpty(extension))
			{
				Console.WriteLine("Unknown file extension.  Trying to mount the image as a full flash image.");
			}
			else if (string.Compare(extension, ".vhd", true, CultureInfo.InvariantCulture) == 0)
			{
				Console.WriteLine("Mounting an existing virtual disk image.");
				flag = false;
			}
			else if (string.Compare(extension, ".ffu", true, CultureInfo.InvariantCulture) != 0)
			{
				Console.WriteLine("Unknown file extension.  Trying to mount the image as a full flash image.");
			}
			IULogger iulogger = new IULogger();
			iulogger.DebugLogger = new LogString(WPImage.NullLog);
			FullFlashUpdateImage fullFlashUpdateImage = null;
			ImageStorageManager imageStorageManager = new ImageStorageManager(iulogger);
			string text = null;
			try
			{
				if (flag)
				{
					fullFlashUpdateImage = new FullFlashUpdateImage();
					Console.WriteLine("Initializing full flash update image {0}.", this._path);
					fullFlashUpdateImage.Initialize(this._path);
					Console.WriteLine("Mounting the full flash update image.");
					imageStorageManager.VirtualHardDiskSectorSize = fullFlashUpdateImage.Stores[0].SectorSize;
					uint storeHeaderVersion = imageStorageManager.MountFullFlashImage(fullFlashUpdateImage, this._randomizeGptIds);
					if (this._waitForSystemVolume)
					{
						foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in fullFlashUpdateImage.Stores.Single((FullFlashUpdateImage.FullFlashUpdateStore s) => s.IsMainOSStore).Partitions)
						{
							if (string.Compare(ImageConstants.SYSTEM_PARTITION_NAME, fullFlashUpdatePartition.Name, true, CultureInfo.InvariantCulture) == 0)
							{
								imageStorageManager.WaitForVolume(ImageConstants.SYSTEM_PARTITION_NAME);
								text = imageStorageManager.GetPartitionPath(ImageConstants.SYSTEM_PARTITION_NAME);
								break;
							}
						}
					}
					this.StoreMountStateToFiles(imageStorageManager, fullFlashUpdateImage, storeHeaderVersion);
				}
				else
				{
					imageStorageManager.MountExistingVirtualHardDisk(this._path, false);
				}
				imageStorageManager.WaitForVolume(ImageConstants.MAINOS_PARTITION_NAME);
				string partitionPath = imageStorageManager.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME);
				Console.WriteLine("\tMain Mount Path: {0}", partitionPath);
				if (!string.IsNullOrEmpty(text))
				{
					Console.WriteLine("\tSystem Mount Path: {0}", text);
				}
				Console.WriteLine("\tPhysical Disk Name: {0}", imageStorageManager.MainOSStorage.GetDiskName());
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to mount the image: {0}", ex.Message);
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002338 File Offset: 0x00000538
		private void StoreMountStateToFiles(ImageStorageManager storageManager, FullFlashUpdateImage fullFlashImage, uint storeHeaderVersion)
		{
			string path = null;
			try
			{
				path = storageManager.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unable to find MainOS partition: " + ex.Message);
				throw;
			}
			using (StreamWriter streamWriter = new StreamWriter(Path.Combine(path, "Windows\\ImageUpdate\\wpimage-storage.txt")))
			{
				streamWriter.WriteLine(storageManager.MainOSStorage.GetDiskName());
				foreach (FullFlashUpdateImage.FullFlashUpdateStore store in fullFlashImage.Stores)
				{
					ImageStorage imageStorage = storageManager.GetImageStorage(store);
					streamWriter.WriteLine(imageStorage.GetDiskName());
				}
			}
			using (StreamWriter streamWriter2 = new StreamWriter(Path.Combine(path, "Windows\\ImageUpdate\\wpimage-info.txt")))
			{
				streamWriter2.WriteLine(storeHeaderVersion);
			}
			try
			{
				string[] contents = new string[]
				{
					fullFlashImage.OSVersion,
					fullFlashImage.AntiTheftVersion,
					fullFlashImage.RulesVersion,
					fullFlashImage.RulesData
				};
				File.WriteAllLines(Path.Combine(path, "Windows\\ImageUpdate\\wpimage-manifest.txt"), contents);
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Non-fatal: Failed to preserve manifest data: {0}", ex2.Message);
			}
		}

		// Token: 0x04000001 RID: 1
		private string _path;

		// Token: 0x04000002 RID: 2
		private bool _waitForSystemVolume;

		// Token: 0x04000003 RID: 3
		private bool _randomizeGptIds;
	}
}
