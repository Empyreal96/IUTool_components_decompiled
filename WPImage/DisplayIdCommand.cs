using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.WPImage
{
	// Token: 0x02000006 RID: 6
	internal class DisplayIdCommand : IWPImageCommand
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002E8B File Offset: 0x0000108B
		public string Name
		{
			get
			{
				return "DisplayId";
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002E92 File Offset: 0x00001092
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

		// Token: 0x06000019 RID: 25 RVA: 0x00002ECA File Offset: 0x000010CA
		public void PrintUsage()
		{
			Console.WriteLine("{0} {1} image_path", "WPImage.exe", this.Name);
			Console.WriteLine("  image_path should point to a Windows Phone 8 FFU");
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002EEC File Offset: 0x000010EC
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
			ImageStoreHeader imageStoreHeader = null;
			using (FileStream imageStream = fullFlashUpdateImage.GetImageStream())
			{
				imageStoreHeader = ImageStoreHeader.ReadFromStream(imageStream);
			}
			string arg = "<empty string>";
			byte[] platformIdentifier = imageStoreHeader.PlatformIdentifier;
			for (int i = 0; i < platformIdentifier.Length; i++)
			{
				if (platformIdentifier[i] != 0)
				{
					arg = Encoding.ASCII.GetString(imageStoreHeader.PlatformIdentifier);
					break;
				}
			}
			Console.WriteLine("Platform ID: {0}", arg);
		}

		// Token: 0x0400000D RID: 13
		private string _path;
	}
}
