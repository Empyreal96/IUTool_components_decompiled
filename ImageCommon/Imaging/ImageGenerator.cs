using System;
using System.Linq;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001B RID: 27
	public class ImageGenerator
	{
		// Token: 0x06000124 RID: 292 RVA: 0x0000CF0F File Offset: 0x0000B10F
		public void Initialize(ImageGeneratorParameters parameters, IULogger logger)
		{
			this.Initialize(parameters, logger, false);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000CF1A File Offset: 0x0000B11A
		public void Initialize(ImageGeneratorParameters parameters, IULogger logger, bool isDesktopImage)
		{
			this._logger = logger;
			if (logger == null)
			{
				this._logger = new IULogger();
			}
			this._parameters = parameters;
			this._isDesktopImage = isDesktopImage;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000CF40 File Offset: 0x0000B140
		public FullFlashUpdateImage CreateFFU()
		{
			FullFlashUpdateImage fullFlashUpdateImage = new FullFlashUpdateImage();
			if (this._parameters == null)
			{
				throw new ImageCommonException("ImageCommon!ImageGenerator::CreateFFU: ImageGenerator has not been initialized.");
			}
			try
			{
				this._parameters.VerifyInputParameters();
				fullFlashUpdateImage.Initialize();
				fullFlashUpdateImage.Description = this._parameters.Description;
				fullFlashUpdateImage.DevicePlatformIDs = this._parameters.DevicePlatformIDs.ToList<string>();
				fullFlashUpdateImage.ChunkSize = this._parameters.ChunkSize;
				fullFlashUpdateImage.HashAlgorithmID = this._parameters.Algid;
				fullFlashUpdateImage.DefaultPartitionAlignmentInBytes = this._parameters.DefaultPartitionByteAlignment;
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon!ImageGenerator::CreateFFU: Failed to Initialize FFU: ", innerException);
			}
			if (this._parameters.Rules != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (InputIntegerRule inputIntegerRule in this._parameters.Rules.IntegerRules)
				{
					if ((inputIntegerRule.Min != null || inputIntegerRule.Max != null) && inputIntegerRule.Values != null && inputIntegerRule.Values.Length != 0)
					{
						throw new ImageCommonException("ImageCommon!ImageGenerator::CreateFFU: Cannot specify both min/max value and list at the same time");
					}
					if (!inputIntegerRule.Property.All(new Func<char, bool>(char.IsLetterOrDigit)))
					{
						throw new ImageCommonException("ImageCommon!ImageGenerator::CreateFFU: Only alphanumerics are allowed for the rule property");
					}
					if (inputIntegerRule.Min != null || inputIntegerRule.Max != null)
					{
						if (inputIntegerRule.Min != null && inputIntegerRule.Max != null && inputIntegerRule.Min > inputIntegerRule.Max)
						{
							throw new ImageCommonException("ImageCommon!ImageGenerator::CreateFFU: Invalid min/max integer rule");
						}
						stringBuilder.AppendFormat("{0}={1}<{2},{3}>;", new object[]
						{
							inputIntegerRule.Property,
							inputIntegerRule.ModeCharacter,
							(inputIntegerRule.Min == null) ? string.Empty : inputIntegerRule.Min.ToString(),
							(inputIntegerRule.Max == null) ? string.Empty : inputIntegerRule.Max.ToString()
						});
					}
					else
					{
						stringBuilder.AppendFormat("{0}={1}[{2}", inputIntegerRule.Property, inputIntegerRule.ModeCharacter, inputIntegerRule.Values[0]);
						foreach (ulong num in inputIntegerRule.Values.Skip(1))
						{
							stringBuilder.AppendFormat(",{0}", num);
						}
						stringBuilder.Append("];");
					}
				}
				UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
				foreach (InputStringRule inputStringRule in this._parameters.Rules.StringRules)
				{
					if (!inputStringRule.Property.All(new Func<char, bool>(char.IsLetterOrDigit)))
					{
						throw new ImageCommonException("ImageCommon!ImageGenerator::CreateFFU: Only alphanumerics are allowed for the rule property");
					}
					stringBuilder.AppendFormat("{0}={1}", inputStringRule.Property, inputStringRule.ModeCharacter);
					stringBuilder.Append("{");
					stringBuilder.Append(Convert.ToBase64String(unicodeEncoding.GetBytes(inputStringRule.Values[0])));
					foreach (string s in inputStringRule.Values.Skip(1))
					{
						stringBuilder.AppendFormat(",{0}", Convert.ToBase64String(unicodeEncoding.GetBytes(s)));
					}
					stringBuilder.Append("};");
				}
				fullFlashUpdateImage.RulesVersion = "1.0";
				fullFlashUpdateImage.RulesData = stringBuilder.ToString();
			}
			try
			{
				foreach (InputStore inputStore in this._parameters.Stores)
				{
					FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore = new FullFlashUpdateImage.FullFlashUpdateStore();
					uint minSectorCount = this._parameters.MinSectorCount;
					if (!inputStore.IsMainOSStore())
					{
						minSectorCount = inputStore.SizeInSectors;
					}
					fullFlashUpdateStore.Initialize(fullFlashUpdateImage, inputStore.Id, inputStore.IsMainOSStore(), inputStore.DevicePath, inputStore.OnlyAllocateDefinedGptEntries, minSectorCount, this._parameters.SectorSize);
					foreach (InputPartition inputPartition in inputStore.Partitions)
					{
						FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition = new FullFlashUpdateImage.FullFlashUpdatePartition();
						fullFlashUpdatePartition.Initialize(0U, inputPartition.TotalSectors, inputPartition.Type, inputPartition.Id, inputPartition.Name, fullFlashUpdateStore, inputPartition.UseAllSpace, this._isDesktopImage);
						fullFlashUpdatePartition.FileSystem = inputPartition.FileSystem;
						fullFlashUpdatePartition.Bootable = inputPartition.Bootable;
						fullFlashUpdatePartition.ReadOnly = inputPartition.ReadOnly;
						fullFlashUpdatePartition.Hidden = inputPartition.Hidden;
						fullFlashUpdatePartition.AttachDriveLetter = inputPartition.AttachDriveLetter;
						fullFlashUpdatePartition.PrimaryPartition = inputPartition.PrimaryPartition;
						fullFlashUpdatePartition.RequiredToFlash = inputPartition.RequiredToFlash;
						fullFlashUpdatePartition.ByteAlignment = inputPartition.ByteAlignment;
						fullFlashUpdatePartition.ClusterSize = inputPartition.ClusterSize;
						fullFlashUpdateStore.AddPartition(fullFlashUpdatePartition);
						if (!inputStore.IsMainOSStore() && inputPartition.ByteAlignment == 0U)
						{
							fullFlashUpdatePartition.ByteAlignment = fullFlashUpdateImage.ChunkSize * 1024U;
						}
					}
					fullFlashUpdateImage.AddStore(fullFlashUpdateStore);
				}
			}
			catch (Exception innerException2)
			{
				throw new ImageCommonException("ImageCommon!ImageGenerator::CreateFFU: Failed to add partitions to FFU: ", innerException2);
			}
			return fullFlashUpdateImage;
		}

		// Token: 0x040000C8 RID: 200
		private IULogger _logger;

		// Token: 0x040000C9 RID: 201
		private ImageGeneratorParameters _parameters;

		// Token: 0x040000CA RID: 202
		private bool _isDesktopImage;

		// Token: 0x040000CB RID: 203
		private const string c_RulesVersion = "1.0";
	}
}
