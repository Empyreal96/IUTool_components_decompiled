using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Images
{
	// Token: 0x02000002 RID: 2
	public class ImageExtractor : IDisposable
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public ImageExtractor(string imageFile, IULogger logger, string extractionDirectory)
		{
			this._logger = logger;
			this._extractionDirectory = extractionDirectory;
			this._imageFile = imageFile;
			try
			{
				this._wpImage = new WPImage(this._logger);
				this._wpImage.LoadImage(this._imageFile);
			}
			catch (Exception ex)
			{
				throw new ImagesException("Tools.ImgCommon!ImageExtractor: Failed to open the Image to extract : " + ex.Message);
			}
			this._imgDump = new ImageInfo(this._wpImage, this._extractionDirectory);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020DC File Offset: 0x000002DC
		~ImageExtractor()
		{
			this.Dispose(false);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000210C File Offset: 0x0000030C
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000211C File Offset: 0x0000031C
		protected virtual void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (this._imgDump != null)
			{
				this._imgDump.Dispose();
				this._imgDump = null;
			}
			if (this._wpImage != null)
			{
				this._wpImage.Dispose();
				this._wpImage = null;
			}
			this._alreadyDisposed = true;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000216F File Offset: 0x0000036F
		// (set) Token: 0x06000006 RID: 6 RVA: 0x00002177 File Offset: 0x00000377
		public bool MetaDataOnly
		{
			get
			{
				return this._fMetadataOnly;
			}
			set
			{
				this._fMetadataOnly = value;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002180 File Offset: 0x00000380
		public List<WPPartition> Partitions
		{
			get
			{
				if (this._wpImage == null)
				{
					return null;
				}
				return this._wpImage.Partitions;
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002198 File Offset: 0x00000398
		public bool ExtractImage()
		{
			bool result = false;
			if (this._wpImage == null)
			{
				throw new ImagesException("Tools.ImgCommon!ExtractImage: Image is not loaded.");
			}
			if (!this._userCancelled)
			{
				string a = Path.GetExtension(this._imageFile).ToUpper(CultureInfo.InvariantCulture);
				if (!(a == ".FFU") && !(a == ".VHD"))
				{
					if (!(a == ".WIM"))
					{
						this._logger.LogError("Tools.ImgCommon!ExtractImage: Unrecognized file format. Unable to compare.", new object[0]);
					}
					else
					{
						this._logger.LogError("Tools.ImgCommon!ExtractImage: WIM not yet supported. Unable to compare.", new object[0]);
					}
				}
				else
				{
					this._logger.LogInfo("Extracting " + this._imageFile, new object[0]);
					result = this.ExtractFiles();
				}
			}
			return result;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002260 File Offset: 0x00000460
		private bool ExtractFiles()
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			StringBuilder stringBuilder5 = new StringBuilder();
			StringBuilder stringBuilder6 = new StringBuilder();
			try
			{
				if (!this._userCancelled)
				{
					foreach (WPPartition wppartition in this._wpImage.Partitions)
					{
						if (!this._fMetadataOnly)
						{
							flag = this.ExtractPartition(wppartition);
						}
						if (wppartition.IsWim)
						{
							stringBuilder4.Append(this._imgDump.DisplayWIM(wppartition));
						}
						else
						{
							stringBuilder3.Append(this._imgDump.DisplayPartition(wppartition));
						}
						stringBuilder6.Append(this._imgDump.DisplayPackageList(wppartition));
						if (!wppartition.IsBinaryPartition)
						{
							stringBuilder5.Append(this._imgDump.DisplayPackages(wppartition, true));
							stringBuilder2.Append(this._imgDump.DisplayRegistry(wppartition));
						}
						if (this._userCancelled || !flag)
						{
							break;
						}
					}
				}
				if (flag && !this._userCancelled)
				{
					try
					{
						this._logger.LogInfo("Extracting Metadata...", new object[0]);
						stringBuilder.Append(this._imgDump.DisplayMetadata());
						stringBuilder.Append(this._imgDump.DisplayStore());
						stringBuilder.Append(this._imgDump.DisplayPartitions(stringBuilder3.ToString()));
						stringBuilder.Append(this._imgDump.DisplayWIMs(stringBuilder4.ToString()));
						stringBuilder.Append(this._imgDump.DisplayPackages(stringBuilder5.ToString()));
						stringBuilder.Append(stringBuilder2.ToString());
					}
					catch (Exception ex)
					{
						this._logger.LogError("Tools.ImgCommon!ExtractFiles: Failure occurred getting metadata: " + ex.Message, new object[]
						{
							ex.InnerException
						});
					}
					if (stringBuilder.Length > 0)
					{
						this._logger.LogInfo("Writing Metadata file....", new object[0]);
						File.WriteAllText(Path.Combine(this._extractionDirectory, "_Metadata.txt"), stringBuilder.ToString());
					}
					if (stringBuilder6.Length > 0)
					{
						this._logger.LogInfo("Writing Metadata file....", new object[0]);
						File.WriteAllText(Path.Combine(this._extractionDirectory, "_PackageList.txt"), stringBuilder6.ToString());
					}
				}
			}
			catch (Exception ex2)
			{
				this._logger.LogError("Tools.ImgCommon!ExtractFiles: Failure occurred getting metadata: " + ex2.Message, new object[]
				{
					ex2.InnerException
				});
			}
			return flag;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002540 File Offset: 0x00000740
		public bool ExtractPartition(WPPartition partition)
		{
			bool result = false;
			this._logger.LogInfo("Extracting {0}: {1}", new object[]
			{
				partition.PartitionTypeLabel,
				partition.Name
			});
			if (partition.IsBinaryPartition)
			{
				string text = Path.Combine(this._extractionDirectory, partition.Name);
				LongPathDirectory.CreateDirectory(text);
				FileUtils.CleanDirectory(text);
				string destinationFile = Path.Combine(text, partition.Name + ".bin");
				try
				{
					partition.CopyAsBinary(destinationFile);
					return true;
				}
				catch (Exception ex)
				{
					if (partition.InvalidPartition)
					{
						this._logger.LogError("Tools.ImgCommon!ExtractPartition: Failed to extract partition '" + partition.Name + "' as binary data because the partition is inaccessible.", new object[0]);
					}
					else
					{
						this._logger.LogError(string.Concat(new string[]
						{
							"Tools.ImgCommon!ExtractPartition: Failed to extract partition '",
							partition.Name,
							"' as binary data: '",
							ex.Message,
							"'"
						}), new object[0]);
					}
					return result;
				}
			}
			this._logger.LogInfo("Extracting files... ", new object[0]);
			string text2 = Path.Combine(this._extractionDirectory, partition.Name);
			LongPathDirectory.CreateDirectory(text2);
			FileUtils.CleanDirectory(text2);
			if (partition.Name.ToUpper(CultureInfo.InvariantCulture) == ImageConstants.MAINOS_PARTITION_NAME.ToUpper(CultureInfo.InvariantCulture))
			{
				foreach (string text3 in Directory.EnumerateFiles(partition.PartitionPath))
				{
					File.Copy(text3, Path.Combine(text2, Path.GetFileName(text3)));
				}
				using (IEnumerator<string> enumerator = Directory.EnumerateDirectories(partition.PartitionPath).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text4 = enumerator.Current;
						bool flag = false;
						string text5 = Path.GetFileName(text4).ToUpper(CultureInfo.InvariantCulture);
						using (List<WPPartition>.Enumerator enumerator2 = this._wpImage.Partitions.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.Name.ToUpper(CultureInfo.InvariantCulture) == text5 || text5 == "DATA")
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag && text5 != "SYSTEM VOLUME INFORMATION")
						{
							string destination = Path.Combine(text2, text5);
							FileUtils.CopyDirectory(text4, destination);
						}
					}
					goto IL_277;
				}
			}
			FileUtils.CopyDirectory(partition.PartitionPath, text2);
			IL_277:
			result = true;
			return result;
		}

		// Token: 0x04000001 RID: 1
		private IULogger _logger;

		// Token: 0x04000002 RID: 2
		private string _extractionDirectory;

		// Token: 0x04000003 RID: 3
		private bool _userCancelled;

		// Token: 0x04000004 RID: 4
		private bool _fMetadataOnly;

		// Token: 0x04000005 RID: 5
		private string _imageFile;

		// Token: 0x04000006 RID: 6
		private WPImage _wpImage;

		// Token: 0x04000007 RID: 7
		private ImageInfo _imgDump;

		// Token: 0x04000008 RID: 8
		private bool _alreadyDisposed;
	}
}
