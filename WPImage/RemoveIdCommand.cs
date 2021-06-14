using System;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.WPImage
{
	// Token: 0x02000005 RID: 5
	internal class RemoveIdCommand : IWPImageCommand
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002C36 File Offset: 0x00000E36
		public string Name
		{
			get
			{
				return "RemoveId";
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002C3D File Offset: 0x00000E3D
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
			return true;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002C75 File Offset: 0x00000E75
		public void PrintUsage()
		{
			Console.WriteLine("{0} {1} image_path", "WPImage.exe", this.Name);
			Console.WriteLine("  image_path should point to a Windows Phone 8 FFU");
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002C98 File Offset: 0x00000E98
		public void Run()
		{
			FullFlashUpdateImage fullFlashUpdateImage = new FullFlashUpdateImage();
			try
			{
				fullFlashUpdateImage.Initialize(this._path);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Unable to initialize full flash update object.");
				Console.Error.WriteLine("{0}", ex.Message);
				return;
			}
			long position;
			using (FileStream imageStream = fullFlashUpdateImage.GetImageStream())
			{
				position = imageStream.Position;
			}
			fullFlashUpdateImage = null;
			using (FileStream fileStream = new FileStream(this._path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				fileStream.Position = position;
				ImageStoreHeader imageStoreHeader = ImageStoreHeader.ReadFromStream(fileStream);
				for (int i = 0; i < imageStoreHeader.PlatformIdentifier.Length; i++)
				{
					imageStoreHeader.PlatformIdentifier[i] = 0;
				}
				fileStream.Position = position;
				imageStoreHeader.WriteToStream(fileStream);
			}
			fullFlashUpdateImage = new FullFlashUpdateImage();
			try
			{
				fullFlashUpdateImage.Initialize(this._path);
			}
			catch (Exception ex2)
			{
				Console.Error.WriteLine("Unable to initialize full flash update object.");
				Console.Error.WriteLine("{0}", ex2.Message);
				return;
			}
			string text = this._path + ".tmp";
			IPayloadWrapper payloadWrapper = DismountCommand.GetPayloadWrapper(fullFlashUpdateImage, text, true);
			using (FileStream imageStream2 = fullFlashUpdateImage.GetImageStream())
			{
				payloadWrapper.InitializeWrapper(imageStream2.Length - imageStream2.Position);
				while (imageStream2.Position < imageStream2.Length)
				{
					byte[] array = new byte[fullFlashUpdateImage.ChunkSizeInBytes];
					imageStream2.Read(array, 0, array.Length);
					payloadWrapper.Write(array);
				}
			}
			payloadWrapper.FinalizeWrapper();
			payloadWrapper = null;
			File.Replace(text, this._path, null);
		}

		// Token: 0x0400000B RID: 11
		private string _path;

		// Token: 0x0400000C RID: 12
		private IULogger _logger = new IULogger();
	}
}
