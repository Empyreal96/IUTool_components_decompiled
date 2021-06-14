using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;
using Microsoft.WindowsPhone.Imaging.WimInterop;

namespace ImgToWIM
{
	// Token: 0x02000002 RID: 2
	internal class ImgToWIM
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static void Main(string[] args)
		{
			bool flag = false;
			Environment.ExitCode = 0;
			if (args.Count<string>() < 2)
			{
				Console.WriteLine("Must have both image file and output WIM specified.");
				ImgToWIM.DisplayUsage();
				Environment.ExitCode = 1;
				return;
			}
			string text = args[0];
			if (!File.Exists(text))
			{
				Console.WriteLine("The image file does not exist.");
				Console.WriteLine();
				ImgToWIM.DisplayUsage();
				Environment.ExitCode = 1;
				return;
			}
			if (string.Compare(Path.GetExtension(text), ".FFU", StringComparison.OrdinalIgnoreCase) == 0)
			{
				ImgToWIM._bDoingFFU = true;
			}
			else if (string.Compare(Path.GetExtension(text), ".VHD", StringComparison.OrdinalIgnoreCase) != 0)
			{
				Console.WriteLine("Unrecognized file type.  Must be a .FFU or .VHD file.");
				Console.WriteLine();
				ImgToWIM.DisplayUsage();
				Environment.ExitCode = 1;
				return;
			}
			ImgToWIM._wimFile = args[1];
			ImgToWIM._wimFile = Path.GetFullPath(ImgToWIM._wimFile);
			if (!Directory.Exists(Path.GetDirectoryName(ImgToWIM._wimFile)))
			{
				Console.WriteLine("The wim file directory does not exists.  Use an existing directory.");
				Console.WriteLine();
				ImgToWIM.DisplayUsage();
				Environment.ExitCode = 1;
				return;
			}
			if (args.Count<string>() > 2)
			{
				ImgToWIM.DisplayUsage();
				Environment.ExitCode = 1;
				return;
			}
			using (Mutex mutex = new Mutex(false, "Global\\VHDMutex_{585b0806-2d3b-4226-b259-9c8d3b237d5c}"))
			{
				try
				{
					mutex.WaitOne();
					try
					{
						Console.WriteLine("Reading the image file: " + text);
						if (ImgToWIM._bDoingFFU)
						{
							FullFlashUpdateImage fullFlashUpdateImage = new FullFlashUpdateImage();
							fullFlashUpdateImage.Initialize(text);
							ImgToWIM._storageManager = new ImageStorageManager();
							ImgToWIM._storageManager.MountFullFlashImage(fullFlashUpdateImage, false);
						}
						else
						{
							ImgToWIM._storageManager = new ImageStorageManager();
							ImgToWIM._storageManager.MountExistingVirtualHardDisk(text, true);
						}
						flag = true;
						ImgToWIM.GetPartitionPaths();
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						Environment.ExitCode = 1;
					}
					if (Environment.ExitCode == 0)
					{
						try
						{
							ImgToWIM.CloneImageToWim();
						}
						catch (Exception ex2)
						{
							Console.WriteLine("Failed to capture image: " + ex2.Message);
							Environment.ExitCode = 1;
						}
					}
					if (flag)
					{
						if (ImgToWIM._bDoingFFU)
						{
							ImgToWIM._storageManager.DismountFullFlashImage(false);
						}
						else
						{
							ImgToWIM._storageManager.DismountVirtualHardDisk();
						}
						flag = false;
					}
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002268 File Offset: 0x00000468
		private static void DisplayUsage()
		{
			Console.WriteLine("ImgToWIM Usage:");
			Console.WriteLine("ImgToWIM converts an FFU/VHD to WIM file.");
			Console.WriteLine("ImgToWIM <ImageFileName> <WimFileName>");
			Console.WriteLine("\tImageFileName- FFU/VHD to file from which to capture files");
			Console.WriteLine("\tWimFileName- The output WIM file");
			Console.WriteLine();
			Console.WriteLine("\tExamples:");
			Console.WriteLine("\t\tImgToWIM Flash.ffu Flash.wim");
			Console.WriteLine("\t\tImgToWIM Flash.vhd Flash.wim");
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000022CC File Offset: 0x000004CC
		private static void CloneImageToWim()
		{
			WindowsImageContainer windowsImageContainer = null;
			string text = Path.Combine(ImgToWIM._tempFileDirectory, "mountDir");
			try
			{
				Console.WriteLine();
				Console.WriteLine("Creating WIM at '{0}' ...", ImgToWIM._wimFile);
				Console.WriteLine();
				windowsImageContainer = new WindowsImageContainer(ImgToWIM._wimFile, WindowsImageContainer.CreateFileMode.CreateAlways, WindowsImageContainer.CreateFileAccess.Write, WindowsImageContainer.CreateFileCompression.WIM_COMPRESS_LZX);
				Console.WriteLine("Capturing '{0}'...", ImageConstants.MAINOS_PARTITION_NAME);
				windowsImageContainer.CaptureImage(ImgToWIM._mainOSPath);
				WindowsImageContainer windowsImageContainer2 = windowsImageContainer;
				windowsImageContainer2.SetBootImage(windowsImageContainer2.ImageCount);
				LongPathDirectory.CreateDirectory(text);
				windowsImageContainer[0].Mount(text, false);
				foreach (string text2 in Directory.GetDirectories(text, "*", SearchOption.TopDirectoryOnly))
				{
					if ((new FileInfo(text2).Attributes & FileAttributes.ReparsePoint) != (FileAttributes)0)
					{
						LongPathDirectory.Delete(text2);
					}
				}
				bool saveChanges = true;
				windowsImageContainer[0].DismountImage(saveChanges);
				Console.WriteLine("WIM creation complete.");
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to create WIM: " + ex.Message);
			}
			finally
			{
				FileUtils.DeleteTree(text);
				if (windowsImageContainer != null)
				{
					windowsImageContainer.Dispose();
					windowsImageContainer = null;
				}
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000023F0 File Offset: 0x000005F0
		private static void GetPartitionPaths()
		{
			IULogger logger = new IULogger();
			InputPartition[] partitions = null;
			ImgToWIM._mainOSPath = ImgToWIM._storageManager.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME);
			if (string.IsNullOrEmpty(ImgToWIM._mainOSPath))
			{
				throw new Exception("Unable to find the MainOS path. Make sure you are trying to convert and image made with ImageApp.");
			}
			string text = Path.Combine(ImgToWIM._mainOSPath, DevicePaths.ImageUpdatePath, ImgToWIM._deviceLayoutFileName);
			if (!File.Exists(text))
			{
				throw new Exception("Unable to find DeviceLayout file and thus unable to extract metadata from VHD.");
			}
			XsdValidator xsdValidator = new XsdValidator();
			try
			{
				using (Stream deviceLayoutXSD = ImageGeneratorParameters.GetDeviceLayoutXSD(text))
				{
					xsdValidator.ValidateXsd(deviceLayoutXSD, text, logger);
				}
			}
			catch (XsdValidatorException innerException)
			{
				throw new Exception("Unable to validate Device Layout XSD.", innerException);
			}
			TextReader textReader = new StreamReader(text);
			try
			{
				if (ImageGeneratorParameters.IsDeviceLayoutV2(text))
				{
					partitions = ((DeviceLayoutInputv2)new XmlSerializer(typeof(DeviceLayoutInputv2)).Deserialize(textReader)).MainOSStore.Partitions;
				}
				else
				{
					partitions = ((DeviceLayoutInput)new XmlSerializer(typeof(DeviceLayoutInput)).Deserialize(textReader)).Partitions;
				}
			}
			catch (Exception innerException2)
			{
				throw new Exception("Unable to parse Device Layout XML.", innerException2);
			}
			finally
			{
				textReader.Close();
				textReader = null;
			}
			ImgToWIM.PopulatePartitionList(partitions);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002544 File Offset: 0x00000744
		private static void PopulatePartitionList(InputPartition[] partitions)
		{
			char[] trimChars = new char[]
			{
				'\\'
			};
			string text = "";
			int num = 0;
			foreach (InputPartition inputPartition in partitions)
			{
				if (string.Compare(inputPartition.Name, ImageConstants.MAINOS_PARTITION_NAME, StringComparison.OrdinalIgnoreCase) != 0)
				{
					try
					{
						text = ImgToWIM._storageManager.GetPartitionPath(inputPartition.Name);
						text = text.TrimEnd(trimChars);
					}
					catch (ImageStorageException)
					{
						text = Path.Combine(ImgToWIM._mainOSPath, inputPartition.Name);
					}
					ImgToWIM._allPartitionPaths.Add(text);
					ImgToWIM._allPartitionNames.Add(inputPartition.Name);
					num++;
					if (ImgToWIM._iEFIESPIndex == -1 && string.Compare(inputPartition.Name, ImgToWIM._EFIESP, StringComparison.OrdinalIgnoreCase) == 0)
					{
						ImgToWIM._iEFIESPIndex = num;
					}
				}
			}
			if (ImgToWIM._iEFIESPIndex == -1)
			{
				throw new Exception("The EFIESP partition could not be found in the image.  Make sure you are using a Windows Phone 8 image as the source.");
			}
		}

		// Token: 0x04000001 RID: 1
		private static string _tempFileDirectory = FileUtils.GetTempDirectory();

		// Token: 0x04000002 RID: 2
		private static string _wimFile;

		// Token: 0x04000003 RID: 3
		private static string _mainOSPath;

		// Token: 0x04000004 RID: 4
		private static string _EFIESP = "EFIESP";

		// Token: 0x04000005 RID: 5
		private static int _iEFIESPIndex = -1;

		// Token: 0x04000006 RID: 6
		private static ImageStorageManager _storageManager = null;

		// Token: 0x04000007 RID: 7
		private static List<string> _allPartitionPaths = new List<string>();

		// Token: 0x04000008 RID: 8
		private static List<string> _allPartitionNames = new List<string>();

		// Token: 0x04000009 RID: 9
		private static string _deviceLayoutFileName = "DeviceLayout.xml";

		// Token: 0x0400000A RID: 10
		private static bool _bDoingFFU = false;
	}
}
