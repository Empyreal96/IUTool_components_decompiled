using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.WPImage
{
	// Token: 0x02000004 RID: 4
	internal class DismountCommand : IWPImageCommand
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000024A8 File Offset: 0x000006A8
		public string Name
		{
			get
			{
				return "Dismount";
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000024B0 File Offset: 0x000006B0
		public bool ParseArgs(string[] args)
		{
			if (args.Length < 2)
			{
				return false;
			}
			for (int i = 0; i < args.Length; i++)
			{
				string text = args[i].Substring(1);
				if (string.Compare(text, DismountCommand.PhysicalDriveOption.Substring(1, 1), true, CultureInfo.InstalledUICulture) == 0 || string.Compare(text.Substring(0, 1), DismountCommand.PhysicalDriveOption.Substring(0, 1), true, CultureInfo.InstalledUICulture) == 0)
				{
					if (i + 1 >= args.Length)
					{
						return false;
					}
					this._mainOSMountedPath = args[i + 1];
					if (!this._mainOSMountedPath.StartsWith("\\\\.\\PhysicalDrive", true, CultureInfo.InvariantCulture))
					{
						this._mainOSMountedPath = "\\\\.\\PhysicalDrive" + this._mainOSMountedPath;
					}
					i++;
				}
				else if (string.Compare(text, DismountCommand.TargetImageOption, true, CultureInfo.InstalledUICulture) == 0 || string.Compare(text.Substring(0, 1), DismountCommand.TargetImageOption.Substring(0, 1), true, CultureInfo.InstalledUICulture) == 0)
				{
					if (i + 1 >= args.Length)
					{
						return false;
					}
					this._imagePath = args[i + 1];
					i++;
				}
				else
				{
					if (string.Compare(text, DismountCommand.NoSignOption, true, CultureInfo.InstalledUICulture) != 0 && string.Compare(text.Substring(0, 1), DismountCommand.NoSignOption.Substring(0, 1), true, CultureInfo.InstalledUICulture) != 0)
					{
						return false;
					}
					this._signImage = false;
				}
			}
			return !string.IsNullOrEmpty(this._mainOSMountedPath);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002608 File Offset: 0x00000808
		public void PrintUsage()
		{
			this._logger.LogInfo("{0} {4} -{1} path [-{2} path [-{3}]]", new object[]
			{
				"WPImage.exe",
				DismountCommand.PhysicalDriveOption,
				DismountCommand.TargetImageOption,
				DismountCommand.NoSignOption,
				this.Name
			});
			this._logger.LogInfo("  -{0}: path where the image is mounted: eg. \\\\.\\PhysicalDrive2", new object[]
			{
				DismountCommand.PhysicalDriveOption
			});
			this._logger.LogInfo("  -{0}: save to this image file", new object[]
			{
				DismountCommand.TargetImageOption
			});
			this._logger.LogInfo("  -{0}: do not sign the image file", new object[]
			{
				DismountCommand.NoSignOption
			});
			this._logger.LogInfo("", new object[0]);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000026C4 File Offset: 0x000008C4
		public void Run()
		{
			bool flag = true;
			string extension = Path.GetExtension(this._imagePath);
			if (string.IsNullOrEmpty(extension))
			{
				Console.WriteLine("Unknown file extension.  Trying to dismount the image as a full flash image.");
			}
			else if (string.Compare(extension, ".vhd", true, CultureInfo.InvariantCulture) == 0)
			{
				Console.WriteLine("Dismounting an existing virtual disk image.");
				flag = false;
			}
			else if (string.Compare(extension, ".ffu", true, CultureInfo.InvariantCulture) != 0)
			{
				Console.WriteLine("Unknown file extension.  Trying to dismount the image as a full flash image.");
			}
			if (File.Exists(this._imagePath))
			{
				try
				{
					File.Delete(this._imagePath);
				}
				catch (IOException ex)
				{
					this._logger.LogError("Unable to delete existing output image: " + ex.Message, new object[0]);
					return;
				}
			}
			ImageStorageManager imageStorageManager = new ImageStorageManager(this._logger);
			try
			{
				List<ImageStorage> list = new List<ImageStorage>();
				this._logger.LogInfo("Attaching to mounted image at {0}.", new object[]
				{
					this._mainOSMountedPath
				});
				ImageStorage imageStorage = imageStorageManager.AttachToMountedVirtualHardDisk(this._mainOSMountedPath, false, true);
				string path = null;
				try
				{
					path = imageStorage.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME);
				}
				catch (Exception ex2)
				{
					this._logger.LogWarning("Unable to find MainOS partition: " + ex2.Message, new object[0]);
					throw;
				}
				string path2 = Path.Combine(path, "Windows\\ImageUpdate\\wpimage-storage.txt");
				string path3 = Path.Combine(path, "Windows\\ImageUpdate\\wpimage-info.txt");
				string path4 = Path.Combine(path, "Windows\\ImageUpdate\\wpimage-manifest.txt");
				uint storeHeaderVersion = 1U;
				if (File.Exists(path3))
				{
					using (StreamReader streamReader = new StreamReader(path3))
					{
						storeHeaderVersion = uint.Parse(streamReader.ReadLine());
					}
				}
				try
				{
					if (flag)
					{
						if (File.Exists(path2))
						{
							using (StreamReader streamReader2 = new StreamReader(path2))
							{
								string strA = streamReader2.ReadLine();
								if (string.Compare(strA, this._mainOSMountedPath, true, CultureInfo.InvariantCulture) != 0)
								{
									this._logger.LogError("", new object[0]);
									throw new Exception();
								}
								string text;
								while ((text = streamReader2.ReadLine()) != null)
								{
									ImageStorage item;
									if (string.Compare(strA, text, true, CultureInfo.InvariantCulture) == 0)
									{
										item = imageStorage;
									}
									else
									{
										item = imageStorageManager.AttachToMountedVirtualHardDisk(text, true, false);
									}
									list.Add(item);
								}
								goto IL_23C;
							}
						}
						this._logger.LogWarning("Unable to find wpimage-storage.txt in MainOS partition", new object[0]);
					}
					else
					{
						list.Add(imageStorage);
					}
					IL_23C:;
				}
				catch (Exception ex3)
				{
					this._logger.LogWarning("Unable to find and attach to dependent VHD/X(s): " + ex3.Message, new object[0]);
					throw;
				}
				if (string.IsNullOrEmpty(this._imagePath))
				{
					list.ForEach(delegate(ImageStorage s)
					{
						s.DismountVirtualHardDisk(true);
					});
				}
				else
				{
					this._logger.LogInfo("Initializing target image object.", new object[0]);
					FullFlashUpdateImage fullFlashUpdateImage = imageStorageManager.CreateFullFlashObjectFromAttachedImage(list);
					imageStorageManager.VirtualHardDiskSectorSize = fullFlashUpdateImage.Stores[0].SectorSize;
					try
					{
						if (File.Exists(path4))
						{
							using (StreamReader streamReader3 = new StreamReader(path4))
							{
								fullFlashUpdateImage.OSVersion = streamReader3.ReadLine();
								fullFlashUpdateImage.AntiTheftVersion = streamReader3.ReadLine();
								string text2 = streamReader3.ReadLine();
								if (!string.IsNullOrEmpty(text2))
								{
									fullFlashUpdateImage.RulesVersion = text2;
								}
								text2 = streamReader3.ReadLine();
								if (!string.IsNullOrEmpty(text2))
								{
									fullFlashUpdateImage.RulesData = text2;
								}
							}
						}
					}
					catch (Exception ex4)
					{
						this._logger.LogWarning("Unable to restore previous manifest: " + ex4.Message, new object[0]);
					}
					LongPathFile.Delete(path2);
					LongPathFile.Delete(path3);
					LongPathFile.Delete(path4);
					this._logger.LogInfo("Creating the image payload from {0}", new object[]
					{
						this._mainOSMountedPath
					});
					IPayloadWrapper payloadWrapper = DismountCommand.GetPayloadWrapper(fullFlashUpdateImage, this._imagePath, this._signImage);
					imageStorageManager.DismountFullFlashImage(true, payloadWrapper, true, storeHeaderVersion);
					this._logger.LogInfo("Success.", new object[0]);
					fullFlashUpdateImage = null;
				}
			}
			catch (Exception innerException)
			{
				this._logger.LogError("Failed to dismount the image: {0}", new object[]
				{
					innerException.ToString()
				});
				while (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
					this._logger.LogError("\t{0}", new object[]
					{
						innerException.ToString()
					});
				}
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002BCC File Offset: 0x00000DCC
		public static IPayloadWrapper GetPayloadWrapper(FullFlashUpdateImage image, string imagePath, bool signImage)
		{
			OutputWrapper outputWrapper = new OutputWrapper(imagePath);
			IPayloadWrapper innerWrapper = outputWrapper;
			if (signImage)
			{
				innerWrapper = new SigningWrapper(image, outputWrapper);
			}
			SecurityWrapper innerWrapper2 = new SecurityWrapper(image, innerWrapper);
			return new ManifestWrapper(image, innerWrapper2);
		}

		// Token: 0x04000004 RID: 4
		public static readonly string PhysicalDriveOption = "physicalDrive";

		// Token: 0x04000005 RID: 5
		public static readonly string TargetImageOption = "imagePath";

		// Token: 0x04000006 RID: 6
		public static readonly string NoSignOption = "noSign";

		// Token: 0x04000007 RID: 7
		private string _mainOSMountedPath;

		// Token: 0x04000008 RID: 8
		private string _imagePath;

		// Token: 0x04000009 RID: 9
		private bool _signImage = true;

		// Token: 0x0400000A RID: 10
		private IULogger _logger = new IULogger();
	}
}
