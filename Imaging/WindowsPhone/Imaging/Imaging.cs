using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using Microsoft.Composition.Packaging;
using Microsoft.Composition.ToolBox;
using Microsoft.WindowsPhone.CompDB;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.Customization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging.WimInterop;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000009 RID: 9
	public class Imaging
	{
		// Token: 0x06000035 RID: 53 RVA: 0x00002988 File Offset: 0x00000B88
		public Imaging(IULogger logger)
		{
			this._logger = logger;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002B68 File Offset: 0x00000D68
		~Imaging()
		{
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002B90 File Offset: 0x00000D90
		private void SetPaths()
		{
			try
			{
				this._outputFile = Path.GetFullPath(this._outputFile);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("Imaging!SetPaths: OutputFile path and name are too long.", innerException);
			}
			string text = Path.GetDirectoryName(this._outputFile);
			LongPathDirectory.CreateDirectory(text);
			text = FileUtils.GetShortPathName(text);
			this._processedFilesDir = Path.Combine(text, "ProcessedFiles");
			if (Directory.Exists(this._processedFilesDir))
			{
				FileUtils.CleanDirectory(this._processedFilesDir);
			}
			else
			{
				LongPathDirectory.CreateDirectory(this._processedFilesDir);
			}
			this._tempDirectoryPath = BuildPaths.GetImagingTempPath(text) + Process.GetCurrentProcess().Id;
			this._updateStagingRoot = Path.Combine(this._tempDirectoryPath, "USERS\\System\\AppData\\Local\\UpdateStagingRoot");
			LongPathDirectory.CreateDirectory(this._tempDirectoryPath);
			this._tempDirectoryPath = FileUtils.GetShortPathName(this._tempDirectoryPath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this._outputFile);
			if (fileNameWithoutExtension.Length == 0)
			{
				throw new ImageCommonException("Imaging!SetPaths: The Output File cannot be empty when extension is removed.");
			}
			if (this._outputFile.EndsWith(".FFU", StringComparison.OrdinalIgnoreCase))
			{
				this._bDoingFFU = true;
				this._outputType = "FFU";
				if (this.CPUId == CpuId.Invalid)
				{
					this._logger.LogInfo("Imaging: Generating FFU thus setting CPU Type to 'arm'.", new object[0]);
					this.CPUId = CpuId.ARM;
				}
			}
			else
			{
				if (!this._outputFile.EndsWith(".VHD", StringComparison.OrdinalIgnoreCase))
				{
					throw new ImageCommonException("Imaging!SetPaths: The OutputFile must end with either '.FFU' or '.VHD'.");
				}
				this._bDoingFFU = false;
				this._outputType = "VHD";
				if (this.CPUId == CpuId.Invalid)
				{
					this._logger.LogInfo("Imaging: Generating VHD thus setting CPU Type to 'x86'.", new object[0]);
					this.CPUId = CpuId.X86;
				}
			}
			if (this._bDoingFFU)
			{
				this._catalogFile = Path.Combine(text, fileNameWithoutExtension + ".CAT");
			}
			if (!string.IsNullOrEmpty(this._oemInputFile))
			{
				if (this._oemInputFile.Length > 260)
				{
					this._oemInputFile = FileUtils.GetShortPathName(this._oemInputFile);
				}
				if (string.IsNullOrEmpty(this._msPackagesRoot))
				{
					throw new ImageCommonException("Imaging!SetPaths: The OEMInputXML requires a path to MSPackagesRoot.");
				}
			}
			this._bspDBFile = Path.Combine(text, fileNameWithoutExtension + ".BSPDB.xml");
			this._deviceDBFile = Path.Combine(text, fileNameWithoutExtension + ".DeviceDB.xml");
			this._bspCompDB.BuildArch = this.CPUId.ToString();
			this._deviceCompDB.BuildArch = this.CPUId.ToString();
			this._updateInputFileGenerated = Path.Combine(text, fileNameWithoutExtension + ".UpdateInput.xml");
			if (this._updateInputFileGenerated.Length > 260)
			{
				this._updateInputFileGenerated = FileUtils.GetShortPathName(this._updateInputFileGenerated);
			}
			this._UOSOutputDestination = Path.Combine(text, fileNameWithoutExtension + "." + DevicePaths.UpdateOutputFile);
			if (this._UOSOutputDestination.Length > 260)
			{
				this._UOSOutputDestination = FileUtils.GetShortPathName(this._UOSOutputDestination);
			}
			this._UpdateHistoryDestination = Path.Combine(text, fileNameWithoutExtension + "." + DevicePaths.UpdateHistoryFile);
			if (this._UpdateHistoryDestination.Length > 260)
			{
				this._UpdateHistoryDestination = FileUtils.GetShortPathName(this._UpdateHistoryDestination);
			}
			this._PackageListFile = Path.Combine(text, fileNameWithoutExtension + ".PackageList.xml");
			if (this._PackageListFile.Length > 260)
			{
				this._PackageListFile = FileUtils.GetShortPathName(this._PackageListFile);
			}
			int num = Math.Max(this._UOSOutputDestination.Length, this._UpdateHistoryDestination.Length);
			num = Math.Max(this._catalogFile.Length, num);
			num = Math.Max(this._outputFile.Length, num);
			if (num >= 260)
			{
				throw new ImageCommonException(string.Concat(new object[]
				{
					"Imaging!SetPaths: The Output File and path '",
					this._outputFile,
					"' and cannot be longer than ",
					260 - (num - 260),
					" characters."
				}));
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002F80 File Offset: 0x00001180
		public void UpdateExistingImage(string imageFile, string updateInputXML, bool randomizeGptIds)
		{
			this._outputFile = imageFile;
			this._updateInputFile = updateInputXML;
			this._bDoingUpdate = true;
			this.ProcessImage(randomizeGptIds);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002FA0 File Offset: 0x000011A0
		public void BuildNewImage(string outputFile, string oemInputXML, string msPackageRoot, string oemCustomizationXML, string oemCustomizationPPKG, string oemVersion, bool randomizeGptIds)
		{
			this._outputFile = outputFile;
			this._oemInputFile = oemInputXML;
			this._msPackagesRoot = Path.GetFullPath(msPackageRoot);
			this._oemCustomizationXML = oemCustomizationXML;
			this._oemCustomizationPPKG = oemCustomizationPPKG;
			if (!string.IsNullOrEmpty(this._oemCustomizationXML) || !string.IsNullOrEmpty(this._oemCustomizationPPKG))
			{
				if (string.IsNullOrEmpty(oemVersion))
				{
					this._logger.LogError("Imaging!BuildNewImage: The OEMVersion must be set if OEMCustomizationXML\\OEMCustomizationPPKG is provided.", new object[0]);
					throw new ImageCommonException("Imaging!BuildNewImage: The OEMVersion must be set if OEMCustomizationXML\\OEMCustomizationPPKG is provided.");
				}
				if (!VersionInfo.TryParse(oemVersion, out this._oemCustomizationVersion))
				{
					this._logger.LogError("Imaging!BuildNewImage: Provided OEMVersion that was not a valid version string.", new object[0]);
					throw new ImageCommonException("Imaging!BuildNewImage: Provided OEMVersion that was not a valid version string.");
				}
			}
			this.ProcessImage(randomizeGptIds);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003054 File Offset: 0x00001254
		private void ProcessImage(bool randomizeGptIds)
		{
			try
			{
				ProcessPrivilege.Adjust(PrivilegeNames.RestorePrivilege, true);
				ProcessPrivilege.Adjust(PrivilegeNames.SecurityPrivilege, true);
				this._dtStartTime = DateTime.Now;
				this.SetPaths();
				Guid guid = Guid.NewGuid();
				this._telemetryLogger.LogString("ProcessImageStarted", guid, new string[]
				{
					this._bDoingUpdate.ToString(CultureInfo.InvariantCulture)
				});
				try
				{
					string location = base.GetType().Assembly.Location;
					FileInfo fileInfo = new FileInfo(location);
					this._telemetryLogger.LogString("BinaryInfo", guid, new string[]
					{
						FileVersionInfo.GetVersionInfo(location).ProductVersion,
						fileInfo.CreationTime.ToString("yyMMdd-HHmm", CultureInfo.InvariantCulture),
						fileInfo.LastWriteTime.ToString("yyMMdd-HHmm", CultureInfo.InvariantCulture)
					});
				}
				catch
				{
				}
				try
				{
					if (!this.SkipUpdateMain)
					{
						if (this._bDoingUpdate)
						{
							this._logger.LogInfo("Imaging: Reading the update input file...", new object[0]);
							this._updateInput = this.ValidateInput();
							this._updateInput.WriteToFile(this._updateInputFileGenerated);
						}
						else
						{
							this._logger.LogInfo("Imaging: Reading the OEM Input XML file...", new object[0]);
							this.ValidateInput(ref this._oemInput);
							if (this._oemInput.Edition == null)
							{
								throw new ImageCommonException("Imaging!ProcessImage: The Product entry in the OEMInput '" + this._oemInput.Product + "' is not recognized.");
							}
							this._isOneCore = (!this._oemInput.Edition.IsProduct("Windows Phone") && !this._oemInput.Edition.IsProduct("Phone Manufacturing OS") && !this._oemInput.Edition.IsProduct("Phone Factory OS"));
							try
							{
								this._releaseType = (ReleaseType)Enum.Parse(typeof(ReleaseType), this._oemInput.ReleaseType);
							}
							catch (PackageException)
							{
								throw new ImageCommonException("Imaging!ProcessImage: The ReleaseType '" + this._oemInput.ReleaseType + "' in the OEM Input file is not valid.  Please use 'Production' or 'Test'.");
							}
							if (this._oemInput.Edition.ReleaseType == ReleaseType.Test && this._releaseType == ReleaseType.Production)
							{
								throw new ImageCommonException("Imaging!ProcessImage: The Product entry in the OEMInput '" + this._oemInput.Product + "' is a ReleaseType=Test Product and cannot be used to create ReleaseType=Production images as specified in the OEMInput.");
							}
							this.GenerateInputFile(null);
							this.GenerateCustomizationContent();
							if (this._releaseType != ReleaseType.Production && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IMAGING_PACKAGES_OVERRIDE")))
							{
								this.GenerateInputFile(Directory.GetFiles(Environment.GetEnvironmentVariable("IMAGING_PACKAGES_OVERRIDE")).ToList<string>());
							}
							this.ValidateProductionImage();
						}
					}
					IList<string> partitionsTargeted = (from p in this._packageInfoList
					select p.Value.Partition).Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
					this._swStorageStackTime.Start();
					this._storageManagerCommit = new ImageStorageManager(this._logger, partitionsTargeted);
					this._storageManagerCommit.RandomizeDiskIds = randomizeGptIds;
					this._storageManagerCommit.RandomizePartitionIDs = randomizeGptIds;
					this._swStorageStackTime.Stop();
					this._storageManager = this._storageManagerCommit;
					if (this.SkipImaging)
					{
						this._logger.LogInfo("Imaging: OEM Customizations processing complete. Skipping Imaging...", new object[0]);
					}
					else
					{
						if (this._bDoingUpdate)
						{
							this.LoadImage(randomizeGptIds);
						}
						else if (!string.IsNullOrEmpty(this._oemInputFile))
						{
							this._deviceLayoutValidator.Initialize(this._msCoreFMPackages.FirstOrDefault<IPkgInfo>(), this._oemInput, this._logger, this._tempDirectoryPath);
							this.ReadDeviceLayout(new Guid?(guid));
							this.InitializeMinFreeSectors();
							this._swStorageStackTime.Start();
							FullFlashUpdateImage.IsGPTPartitionType(this._parameters.MainOSStore.Partitions[0].Type);
							this._storageManagerStaging = new ImageStorageManager(this._logger, partitionsTargeted);
							this._storageManagerStaging.IsDesktopImage = false;
							this._storageManagerStaging.RandomizeDiskIds = true;
							this._storageManagerStaging.RandomizePartitionIDs = true;
							this._swStorageStackTime.Stop();
							this._storageManagerStaging.VirtualHardDiskSectorSize = this._parameters.VirtualHardDiskSectorSize;
							this._storageManager = this._storageManagerStaging;
							this.CreateImage(null);
							this._tempDirectoryPath = this._storageManager.GetPartitionPath(ImageConstants.DATA_PARTITION_NAME) + Process.GetCurrentProcess().Id;
							LongPathDirectory.CreateDirectory(this._tempDirectoryPath);
							this._tempDirectoryPath = FileUtils.GetShortPathName(this._tempDirectoryPath);
							this._updateStagingRoot = Path.Combine(this._tempDirectoryPath, "USERS\\System\\AppData\\Local\\UpdateStagingRoot");
						}
						else if (!this.FormatDPP)
						{
							throw new ImageCommonException("Imaging!Run: No Input XML file specified.");
						}
						try
						{
							this._hasDPPPartition = !string.IsNullOrEmpty(this._storageManager.GetPartitionPath(ImageConstants.DPP_PARTITION_NAME));
						}
						catch
						{
							this._hasDPPPartition = false;
						}
						if (this.FormatDPP && !this._hasDPPPartition)
						{
							throw new ImageCommonException("Imaging!ProcessImage: The OEM Input XML specifies FormatDPP but the DeviceLayout does not contain a DPP partition.");
						}
						if (!this.SkipUpdateMain)
						{
							if (!this._bDoingUpdate)
							{
								Environment.SetEnvironmentVariable("WINDOWS_WCP_INSKUASSEMBLY", "1");
							}
							this.StageImage();
							if (!this._bDoingUpdate)
							{
								this._logger.LogInfo("Imaging: Preparing to create new image '{0}'...", new object[]
								{
									this._outputFile
								});
								this.ReadDeviceLayout(null);
								this.ProcessMinFreeSectors();
								this._storageManagerCommit.VirtualHardDiskSectorSize = this._parameters.VirtualHardDiskSectorSize;
								this._storageManager = this._storageManagerCommit;
								this.CreateImage(this._outputFile);
								this.EnforcePartitionRestrictions();
								this.PrePopulateCommitVolumes();
							}
							this.CommitImage();
							this._telemetryLogger.LogString("ImageStoreCount", guid, new string[]
							{
								this._ffuImage.StoreCount.ToString(CultureInfo.InvariantCulture)
							});
							if (!this._bDoingUpdate && !this._oemInput.Edition.IsProduct("Windows 10 IoT Core"))
							{
								string environmentVariable = Environment.GetEnvironmentVariable("WINDOWS_TRACING_LOGFILE");
								string text = environmentVariable + ".tmp";
								LongPathFile.Delete(text);
								LongPathFile.Copy(environmentVariable, text);
								if (new List<string>(File.ReadAllLines(text)).Contains("    Installer name: 'Mof'"))
								{
									throw new ImagingException("MOF AI is limited to IoT only, please contact aipanel with questions.");
								}
								LongPathFile.Delete(text);
							}
							try
							{
								StringBuilder stringBuilder = new StringBuilder();
								foreach (string value in this._ffuImage.DevicePlatformIDs)
								{
									stringBuilder.Append(value);
									stringBuilder.Append(";");
								}
								this._telemetryLogger.LogString("PlatformIds", guid, new string[]
								{
									stringBuilder.ToString()
								});
							}
							catch
							{
							}
							Environment.SetEnvironmentVariable("WINDOWS_WCP_INSKUASSEMBLY", null);
						}
						if (!this._bDoingUpdate)
						{
							this.ProcessBSPProductNameAndVersion();
							this.LoadDataAssets();
							this.ValidateMinFreeSectors();
							this.CopyPristineHivesForFactoryReset();
						}
						this.FinalizeImage();
						this._logger.LogInfo("Imaging: Image processing complete.", new object[0]);
						Environment.ExitCode = 0;
					}
				}
				catch (ImageCommonException ex)
				{
					this._logger.LogError("{0}", new object[]
					{
						ex.Message
					});
					this._telemetryLogger.LogString("ImagingFailed", guid, new string[]
					{
						ex.Message
					});
					if (ex.InnerException != null)
					{
						string text2 = ex.InnerException.ToString();
						this._logger.LogError("\t{0}", new object[]
						{
							text2
						});
						this._telemetryLogger.LogString("ImagingException", guid, new string[]
						{
							text2
						});
					}
					Environment.ExitCode = 1;
				}
				catch (Exception ex2)
				{
					this._logger.LogError("{0}", new object[]
					{
						ex2.ToString()
					});
					this._telemetryLogger.LogString("ImagingFailed", guid, new string[]
					{
						ex2.ToString()
					});
					if (ex2.InnerException != null)
					{
						string text3 = ex2.InnerException.ToString();
						this._logger.LogError("\t{0}", new object[]
						{
							text3
						});
						this._telemetryLogger.LogString("ImagingUnhandledException", guid, new string[]
						{
							text3
						});
					}
					this._logger.LogError("An unhandled exception was thrown: {0}", new object[]
					{
						ex2.ToString()
					});
					Environment.ExitCode = 3;
				}
				finally
				{
					if (Environment.ExitCode != 0)
					{
						try
						{
							this.CleanupHandler(null, null);
						}
						catch (Exception ex3)
						{
							LogUtil.Diagnostic("Ignoring exception during cleanup: " + ex3.Message);
						}
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this._outputFile);
						this._logger.LogInfo("Imaging: See {0}.cbs.log and {0}.csi.log for details.", new object[]
						{
							fileNameWithoutExtension
						});
					}
					bool deleteFile = true;
					this.CleanupStorageManager(this._storageManagerStaging, deleteFile);
					if (!string.IsNullOrEmpty(this._tempDirectoryPath))
					{
						FileUtils.DeleteTree(this._tempDirectoryPath);
					}
					this._dtEndTime = DateTime.Now;
					TimeSpan timeSpan = this._dtEndTime - this._dtStartTime;
					this._logger.LogInfo("Imaging: Performance Results:", new object[0]);
					this._logger.LogInfo("\tTotal Run Time:\t" + timeSpan, new object[0]);
					this._telemetryLogger.LogString("PerformanceTotalTime", guid, new string[]
					{
						timeSpan.ToString()
					});
					if (this._swCreateFFUTime.Elapsed != TimeSpan.Zero)
					{
						TimeSpan timeSpan2 = this._swReqXMLProcessingTime.Elapsed + this._swCreateFFUTime.Elapsed;
						this._logger.LogInfo("\tImage Creation Time:\t{0}", new object[]
						{
							timeSpan2
						});
						this._telemetryLogger.LogString("PerformanceImageCreationTime", guid, new string[]
						{
							timeSpan2.ToString()
						});
						this._logger.LogDebug("\t\tReading\\Parsing XML Time:\t" + this._swReqXMLProcessingTime, new object[0]);
						this._logger.LogDebug("\t\tCreate metadata Time:\t" + this._swCreateFFUTime.Elapsed, new object[0]);
					}
					TimeSpan timeSpan3 = this._swStorageStackTime.Elapsed + this._swDismountImageTime.Elapsed + this._swMountImageTime.Elapsed;
					this._logger.LogInfo("\tStorage Stack Time:\t{0}", new object[]
					{
						timeSpan3
					});
					this._telemetryLogger.LogString("PerformanceStorageStackTime", guid, new string[]
					{
						timeSpan3.ToString()
					});
					this._logger.LogDebug("\t\tImage Mount Time:\t" + this._swMountImageTime.Elapsed, new object[0]);
					this._telemetryLogger.LogString("PerformanceImageMountTime", guid, new string[]
					{
						this._swMountImageTime.Elapsed.ToString()
					});
					this._logger.LogDebug("\t\tImage Dismount Time:\t" + this._swDismountImageTime.Elapsed, new object[0]);
					this._telemetryLogger.LogString("PerformanceImageDismountTime", guid, new string[]
					{
						this._swDismountImageTime.Elapsed.ToString()
					});
					if (this._tsCompDBAnswersTime != TimeSpan.Zero)
					{
						this._logger.LogInfo("\tCompDB Answer gathering Time:\t{0}", new object[]
						{
							this._tsCompDBAnswersTime
						});
					}
					if (this._tsCompDBTime != TimeSpan.Zero)
					{
						this._logger.LogInfo("\tCompDB Total Time:\t{0}", new object[]
						{
							this._tsCompDBTime
						});
					}
					if (Imaging._swMutexTime.Elapsed != TimeSpan.Zero)
					{
						this._logger.LogInfo("\tWaiting on other Imaging instance Time:\t{0}", new object[]
						{
							Imaging._swMutexTime.Elapsed
						});
						this._telemetryLogger.LogString("PerformanceMutexTime", guid, new string[]
						{
							Imaging._swMutexTime.Elapsed.ToString()
						});
					}
				}
			}
			finally
			{
				ProcessPrivilege.Adjust(PrivilegeNames.RestorePrivilege, false);
				ProcessPrivilege.Adjust(PrivilegeNames.SecurityPrivilege, false);
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003D84 File Offset: 0x00001F84
		private void PrePopulateCommitVolumes()
		{
			Stopwatch stopwatch = new Stopwatch();
			this._logger.LogInfo("Imaging: Pre-populating commit volumes", new object[0]);
			stopwatch.Start();
			foreach (InputStore inputStore in this._parameters.Stores)
			{
				foreach (InputPartition partition in from x in inputStore.Partitions
				where string.Equals(x.FileSystem, "FAT", StringComparison.InvariantCultureIgnoreCase) || string.Equals(x.FileSystem, "NTFS", StringComparison.InvariantCultureIgnoreCase)
				select x)
				{
					this.PrePopulateCommitVolume(partition);
				}
			}
			stopwatch.Stop();
			this._logger.LogInfo(string.Format("Imaging: Pre-populating commit volumes completed in {0}...", stopwatch.Elapsed.ToString()), new object[0]);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003E8C File Offset: 0x0000208C
		private void PrePopulateCommitVolume(InputPartition partition)
		{
			this._logger.LogInfo("Imaging: Pre-populating component store for '{0}'...", new object[]
			{
				partition.Name
			});
			string partitionPath = this._storageManagerStaging.GetPartitionPath(partition.Name);
			string partitionPath2 = this._storageManagerCommit.GetPartitionPath(partition.Name);
			this.CopyDirectoryTree(Path.Combine(partitionPath, "Windows\\winsxs"), Path.Combine(partitionPath2, "Windows\\winsxs"));
			this.CopyDirectoryTree(Path.Combine(partitionPath, "Windows\\servicing"), Path.Combine(partitionPath2, "Windows\\servicing"));
			bool overwrite = true;
			LongPathFile.Copy(Path.Combine(partitionPath, "Windows\\system32\\config\\COMPONENTS"), Path.Combine(partitionPath2, "Windows\\system32\\config\\COMPONENTS"), overwrite);
			LongPathFile.Copy(Path.Combine(partitionPath, "Windows\\system32\\config\\SOFTWARE"), Path.Combine(partitionPath2, "Windows\\system32\\config\\SOFTWARE"), overwrite);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003F50 File Offset: 0x00002150
		private void CopyDirectoryTree(string sourcePath, string targetPath)
		{
			int num = Imaging.CopyAllFiles(sourcePath, targetPath, true, false);
			if (num != 0)
			{
				throw new ImageCommonException(string.Format("CopyAllFiles failed with non-zero exit code {0}", num));
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003F80 File Offset: 0x00002180
		private string GetMainOSPath()
		{
			return this._storageManager.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003F92 File Offset: 0x00002192
		private string GetDataPath()
		{
			return this._storageManager.GetPartitionPath(ImageConstants.DATA_PARTITION_NAME);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003FA4 File Offset: 0x000021A4
		private string GetUpdateOsWimPath()
		{
			return Path.Combine(this.GetMainOSPath(), DevicePaths.UpdateOSWIMFilePath);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003FB6 File Offset: 0x000021B6
		private string GetPendingUpdateOSWimPath()
		{
			return Path.Combine(this._updateStagingRoot, "Pending_UpdateOS.wim");
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003FC8 File Offset: 0x000021C8
		private void ValidateInput(ref OEMInput xmlInput)
		{
			try
			{
				OEMInput.ValidateInput(ref xmlInput, this._oemInputFile, this._logger, this._msPackagesRoot, this.CPUId.ToString());
			}
			catch (Exception ex)
			{
				throw new ImageCommonException("Imaging!ValidateInput: Failed to load OEM Input XML file " + this._oemInputFile + ":" + ex.Message, ex);
			}
			xmlInput.WriteToFile(Path.Combine(this._processedFilesDir, "OEMInput.xml"));
			this.EnsureAllFilesExist(xmlInput.PackageFiles, "OEM input");
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000405C File Offset: 0x0000225C
		private UpdateOSInput ValidateInput()
		{
			UpdateOSInput updateOSInput = null;
			try
			{
				updateOSInput = UpdateOSInput.ValidateInput(this._updateInputFile, this._logger);
			}
			catch (Exception ex)
			{
				throw new ImageCommonException("Imaging!ValidateInput: Failed to load update input file file " + this._updateInputFile + ":" + ex.Message, ex);
			}
			if (updateOSInput != null)
			{
				this.EnsureAllFilesExist(updateOSInput.PackageFiles, "update input");
			}
			return updateOSInput;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000040C8 File Offset: 0x000022C8
		private void ValidateInput(ref FeatureManifest xmlInput, string fmFile)
		{
			try
			{
				FeatureManifest.ValidateAndLoad(ref xmlInput, fmFile, this._logger);
			}
			catch (Exception ex)
			{
				throw new ImageCommonException("Imaging!ValidateInput: Failed to load Feature Manifest XML file " + fmFile + ":" + ex.Message, ex);
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004114 File Offset: 0x00002314
		private void EnsureAllFilesExist(List<string> filesToCheck, string inputType)
		{
			if (filesToCheck == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string str in from x in filesToCheck
			where !File.Exists(x)
			select x)
			{
				stringBuilder.AppendLine("\t'" + str + "'");
			}
			if (stringBuilder.Length != 0)
			{
				throw new ImageCommonException(string.Concat(new string[]
				{
					"Imaging!EnsureAllFilesExist: The ",
					inputType,
					" contains the package file(s) that do not exist:",
					Environment.NewLine,
					stringBuilder.ToString()
				}));
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000041D8 File Offset: 0x000023D8
		private void GenerateInputFile(List<string> packages)
		{
			this._updateInput = new UpdateOSInput();
			this._updateInput.Description = ((this._oemInput != null) ? this._oemInput.Description : "Imaging generated update input file");
			this._updateInput.DateTime = DateTime.Now.ToString();
			this._updateInput.PackageFiles = new List<string>();
			if (packages != null)
			{
				this._updateInput.PackageFiles.AddRange(packages);
			}
			else if (this._oemInput != null)
			{
				this.ProcessFMs();
				if (this._oemInput.PackageFiles != null)
				{
					this._updateInput.PackageFiles.AddRange(this._oemInput.PackageFiles);
				}
				if (!string.IsNullOrEmpty(this._oemInput.FormatDPP) && !bool.TryParse(this._oemInput.FormatDPP, out this.FormatDPP))
				{
					throw new ImageCommonException("Imaging!GenerateInputFile: The OEM Input XML specifies FormatDPP with an invalid value.  Value must be 'true' or 'false'.");
				}
			}
			this._updateInput.WriteToFile(this._updateInputFileGenerated);
			this.LoadPackages();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000042DC File Offset: 0x000024DC
		private void GenerateCustomizationContent()
		{
			if (string.IsNullOrWhiteSpace(this._oemCustomizationXML) && string.IsNullOrWhiteSpace(this._oemCustomizationPPKG))
			{
				return;
			}
			bool flag = false;
			Customizations customizations = new Customizations();
			customizations.CustomizationXMLFilePath = this._oemCustomizationXML;
			customizations.CustomizationPPKGFilePath = this._oemCustomizationPPKG;
			customizations.ImageCpuType = this.CPUId;
			customizations.ImageBuildType = (this._oemInput.BuildType.Equals("chk") ? Microsoft.WindowsPhone.ImageUpdate.PkgCommon.BuildType.Checked : Microsoft.WindowsPhone.ImageUpdate.PkgCommon.BuildType.Retail);
			customizations.ImageDeviceName = this._oemInput.Device;
			customizations.ImageVersion = this._oemCustomizationVersion;
			customizations.ImagePackages = this._packageInfoList.Values.ToList<IPkgInfo>();
			customizations.OutputDirectory = Path.GetDirectoryName(this._outputFile);
			Customizations.StrictSettingPolicies = this.StrictSettingPolicies;
			CustomContent customContent = CustomContentGenerator.GenerateCustomContent(customizations);
			foreach (CustomizationError customizationError in from x in customContent.CustomizationErrors
			where x.Severity == CustomizationErrorSeverity.Error
			select x)
			{
				flag = true;
				if (customizationError.FilesInvolved != null)
				{
					IULogger logger = this._logger;
					string format = "{0} ({1})";
					object[] array = new object[2];
					array[0] = customizationError.Message;
					array[1] = string.Join(", ", customizationError.FilesInvolved.Select((IDefinedIn x) => x.DefinedInFile));
					logger.LogError(format, array);
				}
				else
				{
					this._logger.LogError(customizationError.Message, new object[0]);
				}
			}
			foreach (CustomizationError customizationError2 in from x in customContent.CustomizationErrors
			where x.Severity == CustomizationErrorSeverity.Warning
			select x)
			{
				if (customizationError2.FilesInvolved != null)
				{
					IULogger logger2 = this._logger;
					string format2 = "{0} ({1})";
					object[] array2 = new object[2];
					array2[0] = customizationError2.Message;
					array2[1] = string.Join(", ", customizationError2.FilesInvolved.Select((IDefinedIn x) => x.DefinedInFile));
					logger2.LogWarning(format2, array2);
				}
				else
				{
					this._logger.LogWarning(customizationError2.Message, new object[0]);
				}
			}
			if (flag)
			{
				throw new ImageCommonException("Imaging: Customization package generation failed.");
			}
			this._dataAssetFileList.AddRange(customContent.DataContent);
			List<string> list = new List<string>();
			foreach (string text in customContent.PackagePaths)
			{
				this._logger.LogInfo("Imaging: Including Image Customization Package: {0}", new object[]
				{
					Path.GetFileName(text)
				});
				IPkgInfo value = Package.LoadFromCab(text);
				this._updateInput.PackageFiles.Add(text);
				this._packageInfoList.Add(text, value);
				list.Add(text);
			}
			this.ProcessOEMCustomizationForCompDB(list, customizations);
			this._updateInput.WriteToFile(this._updateInputFileGenerated);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004638 File Offset: 0x00002838
		private void ProcessOEMCustomizationForCompDB(List<string> customizationPkgs, Customizations customizations)
		{
			if (this.SkipImaging)
			{
				return;
			}
			if (!customizationPkgs.Any<string>())
			{
				return;
			}
			CompDBFeature compDBFeature = new CompDBFeature();
			compDBFeature.FMID = "Generated";
			compDBFeature.FeatureID = "OEMCustomization";
			compDBFeature.Group = OwnerType.OEM.ToString();
			compDBFeature.Type = CompDBFeature.CompDBFeatureTypes.MobileFeature;
			List<CompDBPackageInfo> list = new List<CompDBPackageInfo>();
			foreach (string text in customizationPkgs)
			{
				CompDBPackageInfo compDBPackageInfo = new CompDBPackageInfo(this._packageInfoList[text], text, this._msPackagesRoot, "", this._bspCompDB, true, false);
				list.Add(compDBPackageInfo);
				CompDBFeaturePackage item = new CompDBFeaturePackage(compDBPackageInfo.ID, false);
				compDBFeature.Packages.Add(item);
			}
			CbsPackage cbsPackage = this.CreatePackage(customizations.ImageDeviceName, "Customization.FIP.MainOS", "OEM", OwnerType.OEM.ToString(), customizations.ImageCpuType.ToString(), ReleaseType.Production.ToString(), customizations.ImageBuildType.ToString(), customizations.ImageVersion.ToString());
			cbsPackage.SetCBSFeatureInfo(compDBFeature.FMID, compDBFeature.FeatureID, compDBFeature.Group, customizationPkgs);
			string text2 = Path.Combine(Path.GetDirectoryName(this._outputFile), cbsPackage.PackageName + Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension);
			cbsPackage.SaveCab(text2);
			IPkgInfo pkgInfo = Package.LoadFromCab(text2);
			this._packageInfoList.Add(text2, pkgInfo);
			CompDBPackageInfo compDBPackageInfo2 = new CompDBPackageInfo(pkgInfo, text2, this._msPackagesRoot, "", this._bspCompDB, true, false);
			list.Add(compDBPackageInfo2);
			CompDBFeaturePackage item2 = new CompDBFeaturePackage(compDBPackageInfo2.ID, true);
			compDBFeature.Packages.Add(item2);
			this._updateInput.PackageFiles.Add(text2);
			this._bspCompDB.Packages.AddRange(list);
			this._bspCompDB.Features.Add(compDBFeature);
			this._deviceCompDB.Packages.AddRange(list);
			this._deviceCompDB.Features.Add(compDBFeature);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004884 File Offset: 0x00002A84
		private CbsPackage CreatePackage(string component, string subComponent, string owner, string ownerType, string cpuType, string releaseType, string buildType, string version)
		{
			CbsPackage cbsPackage = new CbsPackage();
			cbsPackage.Owner = owner;
			cbsPackage.OwnerType = ManifestToolBox.ConvertOwnerType(ownerType);
			cbsPackage.Component = component;
			if (!string.IsNullOrEmpty(subComponent))
			{
				cbsPackage.SubComponent = subComponent;
			}
			cbsPackage.Partition = Microsoft.Composition.ToolBox.PkgConstants.MainOsPartition;
			cbsPackage.PhoneReleaseType = ManifestToolBox.ConvertReleaseType(releaseType);
			cbsPackage.HostArch = ManifestToolBox.ConvertCpuIdToCpuArch(cpuType);
			cbsPackage.Version = new Version(version);
			cbsPackage.BuildType = ManifestToolBox.ConvertBuildType(buildType);
			return cbsPackage;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004904 File Offset: 0x00002B04
		private void ValidateProductionImage()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			bool flag = false;
			if (this._bDoingUpdate)
			{
				return;
			}
			if (this._releaseType != ReleaseType.Production)
			{
				return;
			}
			string text = Path.Combine(this._tempDirectoryPath, "temp.cat");
			stringBuilder.Clear();
			stringBuilder2.Clear();
			if (!string.Equals(this._oemInput.BuildType, OEMInput.BuildType_FRE, StringComparison.OrdinalIgnoreCase))
			{
				throw new ImageCommonException("Imaging!ValidateProductionImage: The BuildType '" + this._oemInput.BuildType + "' in the OEM Input file is not valid.  Please use 'fre' for Retail images.");
			}
			if (this._oemInput.PackageFiles != null && this._oemInput.PackageFiles.Count<string>() > 0)
			{
				throw new ImageCommonException("Imaging!ValidateProductionImage: The Retail images cannot use the PackageFiles section of the OEMInput.");
			}
			foreach (IPkgInfo pkgInfo in this._packageInfoList.Values)
			{
				if (pkgInfo.ReleaseType != this._releaseType || pkgInfo.BuildType != Microsoft.WindowsPhone.ImageUpdate.PkgCommon.BuildType.Retail)
				{
					this._logger.LogInfo("Imaging!ValidateProductionImage: Non-production package '{0}': ReleaseType='{1}'; BuildType-'{2}'", new object[]
					{
						pkgInfo.Name,
						pkgInfo.ReleaseType,
						pkgInfo.BuildType
					});
					stringBuilder.AppendLine("\t'" + pkgInfo.Name + "'");
				}
				IEnumerable<IFileEntry> source = from file in pkgInfo.Files
				where file.FileType == Microsoft.WindowsPhone.ImageUpdate.PkgCommon.FileType.Catalog
				select file;
				if (source.Count<IFileEntry>() == 0)
				{
					this._logger.LogWarning("This package has no queryable catalog: " + pkgInfo.Name, new object[0]);
				}
				else
				{
					IFileEntry fileEntry = source.First<IFileEntry>();
					bool overwriteExistingFiles = true;
					pkgInfo.ExtractFile(fileEntry.DevicePath, text, overwriteExistingFiles);
					if (!ImageSigner.HasSignature(text, pkgInfo.OwnerType == OwnerType.Microsoft))
					{
						stringBuilder2.AppendLine(string.Concat(new object[]
						{
							"\t'",
							pkgInfo.Name,
							"': (",
							pkgInfo.OwnerType,
							")"
						}));
					}
					LongPathFile.Delete(text);
				}
			}
			if (stringBuilder.Length != 0)
			{
				if (this._bDoingFFU)
				{
					flag = true;
					this._logger.LogError("Imaging: The OEM Input XML combined with the Feature Manifest contains the following non-production package(s) while the OEM Input specifies this is a 'Production' image:" + Environment.NewLine + stringBuilder.ToString(), new object[0]);
				}
				else
				{
					this._logger.LogInfo("Imaging: The OEM Input XML combined with the Feature Manifest contains the following non-production package(s) while the OEM Input specifies this is a 'Production' image:" + Environment.NewLine + stringBuilder.ToString(), new object[0]);
				}
			}
			if (stringBuilder2.Length != 0)
			{
				if (this._bDoingFFU)
				{
					flag = true;
					this._logger.LogError("Imaging: The OEM Input XML combined with the Feature Manifest contains the following improperly signed package(s) while the OEM Input specifies this is a 'Production' image:" + Environment.NewLine + stringBuilder2.ToString(), new object[0]);
				}
				else
				{
					this._logger.LogInfo("Imaging: The OEM Input XML combined with the Feature Manifest contains the following improperly signed package(s) while the OEM Input specifies this is a 'Production' image:" + Environment.NewLine + stringBuilder2.ToString(), new object[0]);
				}
			}
			if (flag)
			{
				throw new ImageCommonException("Imaging: Production image validation failed.");
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004C20 File Offset: 0x00002E20
		private void ProcessFMs()
		{
			if (this._oemInput.AdditionalFMs != null)
			{
				foreach (string text in this._oemInput.AdditionalFMs)
				{
					this._logger.LogInfo("Imaging: Reading the FM XML file '" + text + "'...", new object[0]);
					this.ProcessFMEntries(text, false);
				}
			}
			this._logger.LogInfo("Imaging: Reading the Feature Manifest XML file...", new object[0]);
			string text2 = this._oemInput.BuildType;
			if (string.IsNullOrWhiteSpace(text2))
			{
				text2 = Environment.GetEnvironmentVariable("BuildType");
			}
			foreach (EditionPackage editionPackage in this._oemInput.Edition.CoreFeatureManifestPackages)
			{
				string text3 = editionPackage.GetPackagePath(this._msPackagesRoot, this.CPUId.ToString(), text2);
				text3 = this._oemInput.ProcessOEMInputVariables(text3);
				if (!File.Exists(text3))
				{
					text3 = Path.ChangeExtension(text3, Microsoft.WindowsPhone.ImageUpdate.PkgCommon.PkgConstants.c_strCBSPackageExtension);
					if (!File.Exists(text3))
					{
						throw new ImageCommonException("Imaging!ProcessFMs: Failed to find the Feature Manifest package '" + Path.ChangeExtension(text3, "") + "*'.  Unable to create image.");
					}
				}
				IPkgInfo pkgInfo;
				try
				{
					pkgInfo = Package.LoadFromCab(text3);
					this._msCoreFMPackages.Add(pkgInfo);
					this._osVersion = pkgInfo.Version.ToString();
				}
				catch (IUException ex)
				{
					throw new ImageCommonException("Imaging!ProcessFMs: Failed to load the Feature Manifest package '" + text3 + "' due to the following error: " + ex.Message, ex);
				}
				if (pkgInfo.OwnerType != OwnerType.Microsoft)
				{
					throw new ImageCommonException("Imaging!ProcessFMs: The Feature Manifest package '" + text3 + "' must be a Microsoft owned and signed package.");
				}
				string fmdevicePath = editionPackage.FMDevicePath;
				IFileEntry fileEntry = pkgInfo.FindFile(fmdevicePath);
				if (fileEntry == null)
				{
					throw new ImageCommonException("Imaging!ProcessFMs: Failed to find the Feature Manifest xml '" + fmdevicePath + "'.");
				}
				string text4 = this._tempDirectoryPath + fileEntry.DevicePath;
				pkgInfo.ExtractFile(fileEntry.DevicePath, text4, true);
				this.ProcessFMEntries(text4, true);
				LongPathFile.Delete(text4);
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004EA4 File Offset: 0x000030A4
		private FeatureManifest ProcessFMEntries(string fmFileXML, bool coreFM = false)
		{
			FeatureManifest featureManifest = null;
			fmFileXML = Environment.ExpandEnvironmentVariables(fmFileXML);
			this.ValidateInput(ref featureManifest, fmFileXML);
			featureManifest.OemInput = this._oemInput;
			if (coreFM)
			{
				if (string.IsNullOrEmpty(this._buildID))
				{
					this._buildID = featureManifest.BuildID;
				}
				if (string.IsNullOrEmpty(this._buildInfo))
				{
					this._buildInfo = featureManifest.BuildInfo;
				}
			}
			if (featureManifest.Features != null && featureManifest.Features.MSConditionalFeatures != null)
			{
				foreach (FMConditionalFeature fmconditionalFeature in featureManifest.Features.MSConditionalFeatures)
				{
					if (fmconditionalFeature.GetAllConditions().FirstOrDefault((Condition cond) => cond.Type == Condition.ConditionType.Registry) != null)
					{
						this._condFeatures.Add(fmconditionalFeature);
					}
				}
			}
			if (featureManifest.Features != null && featureManifest.Features.OEMConditionalFeatures != null && featureManifest.Features.OEMConditionalFeatures.Any<FMConditionalFeature>())
			{
				this._oemConditionalFeatures.AddRange(featureManifest.Features.OEMConditionalFeatures);
			}
			FeatureManifest featureManifest2 = new FeatureManifest(featureManifest);
			featureManifest2.ProcessVariables();
			string fileName = Path.GetFileName(fmFileXML);
			featureManifest2.WriteToFile(Path.Combine(this._processedFilesDir, fileName));
			List<FeatureManifest.FMPkgInfo> list = new List<FeatureManifest.FMPkgInfo>();
			list = featureManifest.GetFilteredPackagesByGroups();
			List<string> list2 = (from pkg in list
			select pkg.PackagePath).Distinct<string>().ToList<string>();
			this.EnsureAllFilesExist(list2, "OEM input + feature manifest");
			this.ProcessCompDBPackages(list, featureManifest, fileName);
			this._updateInput.PackageFiles.AddRange(list2);
			if (string.IsNullOrEmpty(this._oemDevicePlatformPackagePath))
			{
				string oemdevicePlatformPackage = featureManifest.GetOEMDevicePlatformPackage(this._oemInput.Device);
				if (!string.IsNullOrEmpty(oemdevicePlatformPackage) && this.GetPackageFile(oemdevicePlatformPackage, this._oemDevicePlatformDevicePath) != null)
				{
					this._oemDevicePlatformPackagePath = oemdevicePlatformPackage;
				}
				if (string.IsNullOrEmpty(this._oemDevicePlatformPackagePath))
				{
					if (list.Any((FeatureManifest.FMPkgInfo pkg) => pkg.FMGroup == FeatureManifest.PackageGroups.DEVICE))
					{
						foreach (FeatureManifest.FMPkgInfo fmpkgInfo in list.FindAll((FeatureManifest.FMPkgInfo pkg) => pkg.FMGroup == FeatureManifest.PackageGroups.DEVICE && (string.IsNullOrEmpty(pkg.Partition) || pkg.Partition.Equals(ImageConstants.MAINOS_PARTITION_NAME, StringComparison.OrdinalIgnoreCase))))
						{
							if (this.GetPackageFile(fmpkgInfo.PackagePath, this._oemDevicePlatformDevicePath) != null)
							{
								if (string.IsNullOrEmpty(fmpkgInfo.Partition))
								{
									IPkgInfo pkgInfo = this._packageInfoList[fmpkgInfo.ID];
									if (pkgInfo == null)
									{
										pkgInfo = Package.LoadFromCab(fmpkgInfo.PackagePath);
									}
									if (!pkgInfo.Partition.Equals(ImageConstants.MAINOS_PARTITION_NAME, StringComparison.OrdinalIgnoreCase))
									{
										continue;
									}
								}
								this._oemDevicePlatformPackagePath = fmpkgInfo.PackagePath;
								break;
							}
						}
					}
				}
			}
			string deviceLayoutPackage = featureManifest.GetDeviceLayoutPackage(this._oemInput.SOC);
			if (!string.IsNullOrEmpty(deviceLayoutPackage))
			{
				if (this.GetPackageFile(deviceLayoutPackage, this._deviceLayoutDevicePath) != null)
				{
					if (!string.IsNullOrEmpty(this._deviceLayoutPackagePath))
					{
						throw new ImageCommonException("Imaging!ProcessFMEntries: The OEM Input XML combined with the Feature Manifest files contains more than one definition for the Device Layout package.");
					}
					this._deviceLayoutPackagePath = deviceLayoutPackage;
				}
				if (string.IsNullOrEmpty(this._deviceLayoutPackagePath))
				{
					if (list.Any((FeatureManifest.FMPkgInfo pkg) => pkg.FMGroup == FeatureManifest.PackageGroups.SOC))
					{
						foreach (FeatureManifest.FMPkgInfo fmpkgInfo2 in list.FindAll((FeatureManifest.FMPkgInfo pkg) => pkg.FMGroup == FeatureManifest.PackageGroups.SOC && (string.IsNullOrEmpty(pkg.Partition) || pkg.Partition.Equals(ImageConstants.MAINOS_PARTITION_NAME, StringComparison.OrdinalIgnoreCase))))
						{
							if (this.GetPackageFile(fmpkgInfo2.PackagePath, this._deviceLayoutDevicePath) != null)
							{
								if (string.IsNullOrEmpty(fmpkgInfo2.Partition))
								{
									IPkgInfo pkgInfo2 = this._packageInfoList[fmpkgInfo2.ID];
									if (pkgInfo2 == null)
									{
										pkgInfo2 = Package.LoadFromCab(fmpkgInfo2.PackagePath);
									}
									if (!pkgInfo2.Partition.Equals(ImageConstants.MAINOS_PARTITION_NAME, StringComparison.OrdinalIgnoreCase))
									{
										continue;
									}
								}
								this._deviceLayoutPackagePath = fmpkgInfo2.PackagePath;
								break;
							}
						}
					}
				}
			}
			return featureManifest;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000052F8 File Offset: 0x000034F8
		private void ProcessCompDBPackages(List<FeatureManifest.FMPkgInfo> packages, FeatureManifest fm, string fmFilename)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			OwnerType ownerType = (fm.OwnerType == OwnerType.Microsoft) ? fm.OwnerType : OwnerType.OEM;
			List<CompDBPackageInfo> list = new List<CompDBPackageInfo>();
			List<CompDBFeature> list2 = new List<CompDBFeature>();
			foreach (FeatureManifest.FMPkgInfo fmpkgInfo in from pkg in packages
			where pkg.FMGroup == FeatureManifest.PackageGroups.OEMDEVICEPLATFORM
			select pkg)
			{
				fmpkgInfo.FMGroup = FeatureManifest.PackageGroups.DEVICE;
			}
			foreach (FeatureManifest.FMPkgInfo fmpkgInfo2 in from pkg in packages
			where pkg.FMGroup == FeatureManifest.PackageGroups.DEVICELAYOUT
			select pkg)
			{
				fmpkgInfo2.FMGroup = FeatureManifest.PackageGroups.SOC;
			}
			using (IEnumerator<string> enumerator2 = (from fminfo in packages
			select fminfo.FeatureID).Distinct<string>().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Imaging.<>c__DisplayClass101_0 CS$<>8__locals1 = new Imaging.<>c__DisplayClass101_0();
					CS$<>8__locals1.feature = enumerator2.Current;
					CompDBFeature compDBFeature = new CompDBFeature(CS$<>8__locals1.feature, fm.ID, CompDBFeature.CompDBFeatureTypes.MobileFeature, fm.OwnerType.ToString());
					Func<FeatureManifest.FMPkgInfo, bool> predicate;
					if ((predicate = CS$<>8__locals1.<>9__3) == null)
					{
						Imaging.<>c__DisplayClass101_0 CS$<>8__locals2 = CS$<>8__locals1;
						predicate = (CS$<>8__locals2.<>9__3 = ((FeatureManifest.FMPkgInfo pkg) => pkg.FeatureID.Equals(CS$<>8__locals2.feature, StringComparison.OrdinalIgnoreCase)));
					}
					foreach (FeatureManifest.FMPkgInfo fmpkgInfo3 in packages.Where(predicate))
					{
						CompDBFeaturePackage item = new CompDBFeaturePackage(fmpkgInfo3.ID, fmpkgInfo3.FeatureIdentifierPackage);
						compDBFeature.Packages.Add(item);
						CompDBPackageInfo item2 = new CompDBPackageInfo(fmpkgInfo3, fm, fmFilename, this._msPackagesRoot, this._deviceCompDB, true, false);
						list.Add(item2);
					}
					list2.Add(compDBFeature);
				}
			}
			list = list.Distinct<CompDBPackageInfo>().ToList<CompDBPackageInfo>();
			this._deviceCompDB.Features.AddRange(list2);
			this._deviceCompDB.Packages.AddRange(from pkg in list
			select new CompDBPackageInfo(pkg).ClearPackageHashes());
			this._deviceCompDB.Languages = (from lang in this._oemInput.SupportedLanguages.UserInterface
			select new CompDBLanguage(lang)).ToList<CompDBLanguage>();
			this._deviceCompDB.Resolutions = (from res in this._oemInput.Resolutions
			select new CompDBResolution(res)).ToList<CompDBResolution>();
			if (ownerType == OwnerType.OEM)
			{
				this._bspCompDB.Features.AddRange(list2);
				List<CompDBPackageInfo> list3 = new List<CompDBPackageInfo>(list);
				list3 = (from pkg in list3
				select pkg.SetParentDB(this._bspCompDB)).ToList<CompDBPackageInfo>();
				this._bspCompDB.Packages.AddRange(list3);
			}
			stopwatch.Stop();
			this._tsCompDBTime += stopwatch.Elapsed;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000566C File Offset: 0x0000386C
		private List<Hashtable> GetRegistryTable(List<string> packages)
		{
			List<Hashtable> list = new List<Hashtable>();
			foreach (string cabPath in packages)
			{
				try
				{
					Hashtable hashtable = Package.LoadRegistry(cabPath);
					if (hashtable.Count != 0)
					{
						list.Add(hashtable);
					}
				}
				catch
				{
				}
			}
			return list;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000056E4 File Offset: 0x000038E4
		private void WriteCompDBs()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Stopwatch stopwatch2 = new Stopwatch();
			this._deviceCompDB.ReleaseType = (this._bspCompDB.ReleaseType = this._releaseType);
			this._deviceCompDB.Product = (this._bspCompDB.Product = this._oemInput.Edition.InternalProductDir);
			this._deviceCompDB.BuildInfo = (this._bspCompDB.BuildInfo = this._buildInfo);
			Guid buildID = string.IsNullOrEmpty(this._buildID) ? Guid.NewGuid() : new Guid(this._buildID);
			this._deviceCompDB.BuildID = (this._bspCompDB.BuildID = buildID);
			stopwatch2.Start();
			List<string> packages = (from pkg in this._packageInfoList
			where pkg.Value.OwnerType != OwnerType.Microsoft
			select pkg into pkg2
			select pkg2.Key).ToList<string>();
			List<Hashtable> list = new List<Hashtable>(this.GetRegistryTable(packages));
			List<Hashtable> collection = new List<Hashtable>();
			if (this._condFeatures.Count<FMConditionalFeature>() > 0)
			{
				List<string> packages2 = (from pkg in this._packageInfoList
				where pkg.Value.OwnerType == OwnerType.Microsoft
				select pkg into pkg2
				select pkg2.Key).ToList<string>();
				collection = this.GetRegistryTable(packages2);
				list.AddRange(collection);
			}
			if (this._condFeatures.Count<FMConditionalFeature>() > 0)
			{
				DeviceConditionAnswers deviceConditionAnswers = new DeviceConditionAnswers(this._logger);
				deviceConditionAnswers.PopulateConditionAnswers(this._condFeatures, list);
				if (deviceConditionAnswers.Conditions != null)
				{
					this._deviceCompDB.ConditionAnswers = deviceConditionAnswers;
				}
			}
			stopwatch2.Stop();
			if (this._oemConditionalFeatures.Any<FMConditionalFeature>())
			{
				this._bspCompDB.OEMConditionalFeatures = this._oemConditionalFeatures;
			}
			this._bspCompDB.OSVersion = (this._deviceCompDB.OSVersion = this._osVersion);
			this._bspCompDB.WriteToFile(this._bspDBFile, true);
			this._deviceCompDB.WriteToFile(this._deviceDBFile);
			this._tsCompDBTime += stopwatch.Elapsed;
			this._tsCompDBAnswersTime += stopwatch2.Elapsed;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00005968 File Offset: 0x00003B68
		private void LoadImage(bool randomizeGptIds)
		{
			try
			{
				this.AcquireMutex(this._logger);
				if (this._bDoingFFU)
				{
					try
					{
						this._logger.LogInfo("Imaging: Loading exsiting FFU file '{0}'...", new object[]
						{
							this._outputFile
						});
						this._ffuImage = new FullFlashUpdateImage();
						this._ffuImage.Initialize(this._outputFile);
						this._imgSigner = new ImageSigner();
						this._imgSigner.Initialize(this._ffuImage, this._catalogFile, this._logger);
					}
					catch (Exception innerException)
					{
						throw new ImageCommonException("Imaging!LoadImage: Failed while loading the FFU image " + this._outputFile + " with :", innerException);
					}
					try
					{
						this._logger.LogInfo("Imaging: Verifying the existing FFU image...", new object[0]);
						this._imgSigner.VerifyCatalog();
					}
					catch (Exception innerException2)
					{
						throw new ImageCommonException("Imaging!LoadImage: The FFU " + this._outputFile + " has been tampered with outside ImageApp and is no longer usable.", innerException2);
					}
					try
					{
						this._logger.LogInfo("Imaging: Mounting FFU file '{0}'...", new object[]
						{
							this._outputFile
						});
						this._swMountImageTime.Start();
						this._storageManager.MountFullFlashImage(this._ffuImage, randomizeGptIds);
						this._swMountImageTime.Stop();
						goto IL_1B0;
					}
					catch (Exception innerException3)
					{
						throw new ImageCommonException("Imaging!LoadImage: Failed to mount FFU '" + this._outputFile + "' :", innerException3);
					}
				}
				try
				{
					this._logger.LogInfo("Imaging: Loading existing VHD file '{0}'...", new object[]
					{
						this._outputFile
					});
					this._swMountImageTime.Start();
					bool readOnly = false;
					this._storageManager.MountExistingVirtualHardDisk(this._outputFile, readOnly);
					this._swMountImageTime.Stop();
				}
				catch (Exception innerException4)
				{
					throw new ImageCommonException("Imaging!LoadImage: Failed to mount VHD '" + this._outputFile + "' :", innerException4);
				}
				IL_1B0:
				this._logger.LogInfo("Imaging: {0} file '{1}' loaded.", new object[]
				{
					this._outputType,
					this._outputFile
				});
			}
			finally
			{
				this.ReleaseMutex();
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00005BD4 File Offset: 0x00003DD4
		private void ReadDeviceLayout(Guid? telemetrySessionId = null)
		{
			string deviceLayoutXMLFile = string.Empty;
			string oemDevicePlatformXMLFile = string.Empty;
			this._parameters = new ImageGeneratorParameters();
			string text = this.SaveToTempXMLFiles();
			this._logger.LogInfo("Imaging: Found required files '{0}' and '{1}'...", new object[]
			{
				"DeviceLayout.xml",
				"OEMDevicePlatform.xml"
			});
			deviceLayoutXMLFile = Path.Combine(text, "DeviceLayout.xml");
			if (telemetrySessionId != null)
			{
				this._telemetryLogger.LogString("IsDeviceLayoutV2", telemetrySessionId.Value, new string[]
				{
					ImageGeneratorParameters.IsDeviceLayoutV2(deviceLayoutXMLFile).ToString(CultureInfo.InvariantCulture)
				});
			}
			oemDevicePlatformXMLFile = Path.Combine(text, "OEMDevicePlatform.xml");
			this._swReqXMLProcessingTime.Start();
			this._parameters.Initialize(this._logger);
			this._logger.LogInfo("Imaging: Processing Device Layout XML files...", new object[0]);
			this._parameters.ProcessInputXML(deviceLayoutXMLFile, oemDevicePlatformXMLFile);
			this._swReqXMLProcessingTime.Stop();
			FileUtils.DeleteTree(text);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00005CCC File Offset: 0x00003ECC
		private void CreateImage(string imagePath)
		{
			this.AcquireMutex(this._logger);
			try
			{
				ImageGenerator imageGenerator = new ImageGenerator();
				this._swCreateFFUTime.Start();
				imageGenerator.Initialize(this._parameters, this._logger);
				this._logger.LogInfo("Imaging: Creating initial image...", new object[0]);
				this._ffuImage = imageGenerator.CreateFFU();
				this._swCreateFFUTime.Stop();
				this._ffuImage.OSVersion = this._osVersion;
				this._ffuImage.AntiTheftVersion = "1.1";
				this._logger.LogInfo("Imaging: Mounting {0}, {1}, {2} ...", new object[]
				{
					this._outputType,
					this._ffuImage,
					imagePath
				});
				this._swMountImageTime.Start();
				if (this._bDoingFFU)
				{
					this._storageManager.CreateFullFlashImage(this._ffuImage);
				}
				else
				{
					bool preparePartitions = true;
					this._storageManager.CreateVirtualHardDisk(this._ffuImage.Stores[0], imagePath, ImageConstants.PartitionTypeMbr, preparePartitions);
				}
				string mainOSPath = this.GetMainOSPath();
				foreach (InputStore inputStore in this._parameters.Stores)
				{
					foreach (InputPartition inputPartition in from x in inputStore.Partitions
					where string.Equals(x.FileSystem, "FAT", StringComparison.InvariantCultureIgnoreCase) || string.Equals(x.FileSystem, "NTFS", StringComparison.InvariantCultureIgnoreCase)
					select x)
					{
						this._logger.LogInfo("Imaging: Creating Windows layout for partition '{0}'...", new object[]
						{
							inputPartition.Name
						});
						this.CreateWindowsInPartition(this._storageManager.GetPartitionPath(inputPartition.Name), string.Compare(inputPartition.Name, ImageConstants.SYSTEM_PARTITION_NAME, StringComparison.OrdinalIgnoreCase) == 0);
						if (inputPartition.Compressed)
						{
							if (!string.Equals(inputPartition.FileSystem, "NTFS", StringComparison.InvariantCultureIgnoreCase))
							{
								throw new ImageCommonException("Partition " + inputPartition.Name + " is marked compressed, but its filesystem isn't NTFS. Compressed is only supported on NTFS partitions. Please fix the OEMDeviceLayout (FileSystem) and/or OEMDevicePlatform (UncompressedPartitions) to only specify compression on NTFS partitions.");
							}
							this._logger.LogInfo("Imaging: Attaching WOF to partition '{0}'...", new object[]
							{
								inputPartition.Name
							});
							try
							{
								this._storageManager.AttachWOFToVolume(inputPartition.Name);
							}
							catch
							{
								if (this._releaseType == ReleaseType.Production)
								{
									this._logger.LogError("Moblie image compression is on by default. This image is configured for compression, but is likely running < Windows 10 without the WOFADK compression driver installed. Please see the documentation regarding image compression.", new object[0]);
									throw;
								}
								this._logger.LogWarning("Imaging: Unable to attach WOF to partition '{0}', continuing without compression due to non-production image...", new object[]
								{
									inputPartition.Name
								});
								continue;
							}
							this._logger.LogInfo("Imaging: Marking partition '{0}' as Compact...", new object[]
							{
								inputPartition.Name
							});
							string text = Path.Combine(this._storageManager.GetPartitionPath(inputPartition.Name), "Windows\\system32\\config\\SYSTEM");
							using (ORRegistryKey orregistryKey = ORRegistryKey.OpenHive(text))
							{
								using (ORRegistryKey orregistryKey2 = orregistryKey.CreateSubKey("Setup"))
								{
									orregistryKey2.SetValue("Compact", 1);
									orregistryKey.SaveHive(text);
								}
							}
						}
					}
				}
				string text2 = Path.Combine(mainOSPath, "Windows\\ImageUpdate", "OEMInput.xml");
				LongPathDirectory.CreateDirectory(Path.GetDirectoryName(text2));
				LongPathFile.Copy(this._oemInputFile, text2, true);
				this._swMountImageTime.Stop();
			}
			catch (ImageCommonException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ImageCommonException("Imaging!CreateImage: Failed to create " + this._outputType + ": " + ex.Message, ex);
			}
			finally
			{
				this.ReleaseMutex();
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000611C File Offset: 0x0000431C
		private void CreateWindowsInPartition(string root, bool fEFIESP)
		{
			UIntPtr zero = UIntPtr.Zero;
			UIntPtr zero2 = UIntPtr.Zero;
			UIntPtr zero3 = UIntPtr.Zero;
			UIntPtr zero4 = UIntPtr.Zero;
			uint num = 42U | (this._isOneCore ? 64U : 0U);
			uint num2 = 0U;
			if (fEFIESP)
			{
				num |= 16U;
				if (new List<string>
				{
					"VM",
					"VM64",
					"VMGen1"
				}.Contains(this._oemInput.Device, StringComparer.OrdinalIgnoreCase))
				{
					num |= 128U;
				}
			}
			uint ulProcessorArchitecture;
			switch (this.CPUId)
			{
			case CpuId.X86:
				ulProcessorArchitecture = 0U;
				break;
			case CpuId.ARM:
				ulProcessorArchitecture = 5U;
				break;
			case CpuId.ARM64:
				ulProcessorArchitecture = 12U;
				break;
			case CpuId.AMD64:
				ulProcessorArchitecture = 9U;
				break;
			default:
				throw new ImageCommonException(string.Format("Unsupported CPUId {0} encountered.", (uint)this.CPUId));
			}
			UpdateMain.OFFLINE_STORE_CREATION_PARAMETERS offline_STORE_CREATION_PARAMETERS = default(UpdateMain.OFFLINE_STORE_CREATION_PARAMETERS);
			offline_STORE_CREATION_PARAMETERS.cbSize = (UIntPtr)((ulong)((long)Marshal.SizeOf(offline_STORE_CREATION_PARAMETERS)));
			offline_STORE_CREATION_PARAMETERS.dwFlags = 0U;
			offline_STORE_CREATION_PARAMETERS.ulProcessorArchitecture = ulProcessorArchitecture;
			offline_STORE_CREATION_PARAMETERS.pszHostSystemDrivePath = root;
			string szSystemDrive = "c:";
			int num3;
			if (UpdateMain.FAILED(num3 = UpdateMain.WcpInitialize(out zero)))
			{
				throw new ImageCommonException("Imaging!UpdateImage: Failed call WcpInitialize with error code: " + string.Format("{0} (0x{0:X})", num3));
			}
			if (UpdateMain.FAILED(num3 = UpdateMain.CoGetMalloc(1U, out zero2)))
			{
				throw new ImageCommonException("Imaging!UpdateImage: Failed call CoGetMalloc with error code: " + string.Format("{0} (0x{0:X})", num3));
			}
			if (UpdateMain.FAILED(num3 = UpdateMain.SetIsolationIMalloc(zero2)))
			{
				throw new ImageCommonException("Imaging!UpdateImage: Failed call SetIsolationIMalloc with error code: " + string.Format("{0} (0x{0:X})", num3));
			}
			if (UpdateMain.FAILED(num3 = UpdateMain.CreateNewWindows(num, szSystemDrive, ref offline_STORE_CREATION_PARAMETERS, zero3, out num2)))
			{
				throw new ImageCommonException("Imaging!UpdateImage: Failed call CreateNewWindows with error code: " + string.Format("{0} (0x{0:X})", num3));
			}
			UpdateMain.WcpShutdown(zero);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00006310 File Offset: 0x00004510
		private void StageImage()
		{
			using (UpdateMain updateMain = new UpdateMain())
			{
				try
				{
					if (!this._bDoingUpdate)
					{
						LongPathDirectory.CreateDirectory(Path.GetDirectoryName(this.GetUpdateOsWimPath()));
						using (WindowsImageContainer windowsImageContainer = new WindowsImageContainer(this.GetUpdateOsWimPath(), WindowsImageContainer.CreateFileMode.CreateAlways, WindowsImageContainer.CreateFileAccess.Write, WindowsImageContainer.CreateFileCompression.WIM_COMPRESS_LZX))
						{
							string text = Path.Combine(this._tempDirectoryPath, "UpdateOSWim");
							LongPathDirectory.CreateDirectory(text);
							FileUtils.CleanDirectory(text);
							this.CreateWindowsInPartition(text, false);
							windowsImageContainer.CaptureImage(text);
							WindowsImageContainer windowsImageContainer2 = windowsImageContainer;
							windowsImageContainer2.SetBootImage(windowsImageContainer2.ImageCount);
							FileUtils.DeleteTree(text);
						}
					}
					ImageStructures.STORE_ID[] array = new ImageStructures.STORE_ID[this._storageManager.Storages.Count];
					for (int i = 0; i < this._storageManager.Storages.Count; i++)
					{
						array[i] = this._storageManager.Storages[i].StoreId;
					}
					int num;
					if (UpdateMain.FAILED(num = updateMain.Initialize(array.Length, array, this._updateInputFileGenerated, this._tempDirectoryPath, new LogUtil.InteropLogString(this.ErrorLogger), new LogUtil.InteropLogString(this.WarningLogger), new LogUtil.InteropLogString(this.InformationLogger), new LogUtil.InteropLogString(this.DebugLogger))))
					{
						throw new ImageCommonException("Imaging!UpdateImage: Failed to Initialize UpdateDLL::UpdateMain with error code: " + string.Format("{0} (0x{0:X})", num));
					}
					this._logger.LogInfo("Imaging: Staging the image...", new object[0]);
					if (UpdateMain.FAILED(num = updateMain.PrepareUpdate()))
					{
						throw new ImageCommonException("Imaging!UpdateImage: Failed call to UpdateDLL::PrepareUpdate with error code: " + string.Format("{0} (0x{0:X})", num));
					}
				}
				catch (ImageCommonException)
				{
					throw;
				}
				catch (Exception ex)
				{
					throw new ImageCommonException("Imaging!UpdateImage: Failed to stage " + this._outputType + ": " + ex.Message, ex);
				}
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00006540 File Offset: 0x00004740
		private void CommitImage()
		{
			using (UpdateMain updateMain = new UpdateMain())
			{
				DateTime now = DateTime.Now;
				bool overwrite = true;
				string pendingUpdateOSWimPath = this.GetPendingUpdateOSWimPath();
				if (LongPathFile.Exists(this.GetPendingUpdateOSWimPath()))
				{
					string updateOsWimPath = this.GetUpdateOsWimPath();
					LongPathDirectory.CreateDirectory(Path.GetDirectoryName(updateOsWimPath));
					LongPathFile.Copy(pendingUpdateOSWimPath, updateOsWimPath, overwrite);
				}
				ImageStructures.STORE_ID[] array = new ImageStructures.STORE_ID[this._storageManager.Storages.Count];
				for (int i = 0; i < this._storageManager.Storages.Count; i++)
				{
					array[i] = this._storageManager.Storages[i].StoreId;
				}
				int num;
				if (UpdateMain.FAILED(num = updateMain.Initialize(array.Length, array, this._updateInputFileGenerated, this._tempDirectoryPath, new LogUtil.InteropLogString(this.ErrorLogger), new LogUtil.InteropLogString(this.WarningLogger), new LogUtil.InteropLogString(this.InformationLogger), new LogUtil.InteropLogString(this.DebugLogger))))
				{
					throw new ImageCommonException("Imaging!UpdateImage: Failed to Initialize UpdateDLL::UpdateMain with error code: " + string.Format("{0} (0x{0:X})", num));
				}
				this._logger.LogInfo("Imaging: Committing the image...", new object[0]);
				if (UpdateMain.FAILED(num = updateMain.ExecuteUpdate()))
				{
					throw new ImageCommonException("Imaging!UpdateImage: Failed call to UpdateDLL::ExecuteUpdate with error code: " + string.Format("{0} (0x{0:X})", num));
				}
				this._logger.LogInfo("Imaging: Update completed...", new object[0]);
				string path = Path.Combine(this.GetMainOSPath(), "Users\\default\\ntuser.dat");
				DirectorySecurity accessControl = Directory.GetAccessControl(path);
				accessControl.SetAccessRuleProtection(false, true);
				Directory.SetAccessControl(path, accessControl);
				this._logger.LogInfo("Imaging: Saving Update XML files...", new object[0]);
				LongPathFile.Copy(Path.Combine(this.GetDataPath(), DevicePaths.UpdateOutputFilePath), this._UOSOutputDestination, overwrite);
				this._updateHistoryFile = Path.Combine(this.GetMainOSPath(), DevicePaths.UpdateHistoryFilePath);
				LongPathFile.Copy(this._updateHistoryFile, this._UpdateHistoryDestination, overwrite);
				this.GetPackageListFromUpdateHistory();
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00006760 File Offset: 0x00004960
		private void ProcessBSPProductNameAndVersion()
		{
			if (this._bDoingUpdate)
			{
				return;
			}
			string mainOSPath = this.GetMainOSPath();
			string text = "";
			string text2 = "";
			if (!string.IsNullOrEmpty(this.BSPProductName))
			{
				text2 = BuildCompDB.GetProductNamePrefix(this._oemInput.Product);
				text2 += this.BSPProductName;
				this._logger.LogInfo("Imaging: Writing BSP Product Name to the registry Overrides via ProvXML: '{0}'", new object[]
				{
					text2
				});
				string text3 = Path.Combine(mainOSPath, "Programs\\PhoneProvisioner_OEM\\OEM");
				if (!LongPathDirectory.Exists(text3))
				{
					LongPathDirectory.CreateDirectory(text3);
				}
				string text4 = Path.Combine(text3, "mxipcold_BSPProductName_001.provxml");
				string destinationPath = Path.Combine(this._processedFilesDir, Path.GetFileName(text4));
				string contents = Imaging.c_BSPProductName_provxml.Replace("$(BSPProductName)", text2);
				string tempFile = FileUtils.GetTempFile(this._tempDirectoryPath);
				File.WriteAllText(tempFile, contents);
				LongPathFile.Copy(tempFile, text4, true);
				LongPathFile.Copy(tempFile, destinationPath, true);
				File.Delete(tempFile);
				this._logger.LogInfo("Imaging: Writing BSP Product Name ProvXML file complete: '{0}'", new object[]
				{
					text4
				});
			}
			string sourcePath = Path.Combine(mainOSPath, "Windows\\system32\\config\\SOFTWARE");
			string tempFile2 = FileUtils.GetTempFile(this._tempDirectoryPath);
			LongPathFile.Copy(sourcePath, tempFile2);
			using (ORRegistryKey orregistryKey = ORRegistryKey.OpenHive(tempFile2))
			{
				using (ORRegistryKey orregistryKey2 = orregistryKey.CreateSubKey("Microsoft\\Windows NT\\CurrentVersion\\Update\\TargetingInfo\\Overrides\\BSP"))
				{
					if (string.IsNullOrEmpty(this.BSPProductName))
					{
						try
						{
							text2 = orregistryKey2.GetStringValue("Name");
						}
						catch
						{
							text2 = "";
						}
					}
					try
					{
						text = orregistryKey2.GetStringValue("Version");
					}
					catch
					{
						text = "";
					}
				}
			}
			LongPathFile.Delete(tempFile2);
			if (string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text))
			{
				using (ORRegistryKey orregistryKey3 = ORRegistryKey.OpenHive(Path.Combine(mainOSPath, "Windows\\system32\\config\\SYSTEM")))
				{
					using (ORRegistryKey orregistryKey4 = orregistryKey3.CreateSubKey("Platform\\DeviceTargetingInfo"))
					{
						if (string.IsNullOrEmpty(text2))
						{
							try
							{
								text2 = BuildCompDB.GetProductNamePrefix(this._oemInput.Product);
								string stringValue = orregistryKey4.GetStringValue("PhoneManufacturer");
								string stringValue2 = orregistryKey4.GetStringValue("PhoneHardwareVariant");
								text2 = text2 + stringValue + "." + stringValue2;
							}
							catch
							{
							}
						}
						if (string.IsNullOrEmpty(text))
						{
							try
							{
								text = orregistryKey4.GetStringValue("PhoneFirmwareRevision");
							}
							catch
							{
							}
						}
					}
				}
			}
			this._bspCompDB.BSPVersion = text;
			this._bspCompDB.BSPProductName = text2;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00006A24 File Offset: 0x00004C24
		private void GetPackageListFromUpdateHistory()
		{
			if (!File.Exists(this._UpdateHistoryDestination))
			{
				throw new ImageCommonException("Imaging!GetPackageListFromUpdateHistory: Unable to find History file: " + this._UpdateHistoryDestination);
			}
			UpdateHistory.ValidateUpdateHistory(this._UpdateHistoryDestination, this._logger).GetPackageList().WriteToFile(this._PackageListFile);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00006A78 File Offset: 0x00004C78
		private void LoadDataAssets()
		{
			this._logger.LogInfo("Imaging: Copying Data Partition Assets...", new object[0]);
			foreach (KeyValuePair<string, string> keyValuePair in this._dataAssetFileList)
			{
				string text = Path.Combine(this.GetDataPath(), keyValuePair.Value);
				LongPathDirectory.CreateDirectory(Path.GetDirectoryName(text));
				LongPathFile.Copy(keyValuePair.Key, text);
			}
			this._logger.LogInfo("Imaging:   {0} Data Asset Files Copied.", new object[]
			{
				this._dataAssetFileList.Count
			});
			if (this._oemInput == null || this._oemInput.UserStoreMapData == null)
			{
				return;
			}
			if (!Directory.Exists(this._oemInput.UserStoreMapData.SourceDir))
			{
				throw new ImageCommonException("Imaging!LoadMapData: The source directory for the User Store map data does not exist: " + this._oemInput.UserStoreMapData.SourceDir);
			}
			char[] trimChars = new char[]
			{
				'\\'
			};
			string destination = Path.Combine(this.GetDataPath(), this._oemInput.UserStoreMapData.UserStoreDir.TrimStart(trimChars));
			FileUtils.CopyDirectory(this._oemInput.UserStoreMapData.SourceDir, destination);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00006BC4 File Offset: 0x00004DC4
		private void FinalizeImage()
		{
			this._logger.LogInfo("Imaging: Finalizing the {0} image...", new object[]
			{
				this._outputType
			});
			try
			{
				this.AcquireMutex(this._logger);
				if (this._hasDPPPartition)
				{
					this._storageManager.WaitForVolume(ImageConstants.DPP_PARTITION_NAME);
				}
				this._storageManager.WaitForVolume(ImageConstants.MAINOS_PARTITION_NAME);
				if (this.FormatDPP)
				{
					this._logger.LogInfo("Imaging: Formatting the DPP partition.", new object[0]);
					this._swStorageStackTime.Start();
					InputPartition inputPartition = null;
					foreach (InputStore inputStore in this._parameters.Stores)
					{
						inputPartition = inputStore.Partitions.FirstOrDefault((InputPartition x) => string.Equals(x.Name, ImageConstants.DPP_PARTITION_NAME, StringComparison.InvariantCultureIgnoreCase));
						if (inputPartition != null)
						{
							break;
						}
					}
					if (inputPartition == null)
					{
						throw new ImageCommonException("Imaging!FinalizeImage: DPP partition is not present.");
					}
					if (string.IsNullOrEmpty(inputPartition.FileSystem))
					{
						inputPartition.FileSystem = "FAT";
					}
					this._storageManager.FormatPartition(inputPartition.Name, inputPartition.FileSystem, inputPartition.ClusterSize);
					if (this._storageManager.MainOSStorage.StoreId.StoreType == ImageConstants.PartitionTypeGpt)
					{
						this._storageManager.SetPartitionType(inputPartition.Name, ImageConstants.PARTITION_BASIC_DATA_GUID);
					}
					else
					{
						this._storageManager.SetPartitionType(inputPartition.Name, 11);
					}
					this._swStorageStackTime.Stop();
				}
				this.WriteCompDBs();
				if (this._bDoingFFU)
				{
					this.UpdateUsedSectors();
					this._ffuImage.Description = this.GetUpdateDescription(this._updateHistoryFile);
					OutputWrapper innerWrapper = new OutputWrapper(this._outputFile);
					SecurityWrapper securityWrapper = new SecurityWrapper(this._ffuImage, innerWrapper);
					ManifestWrapper payloadWrapper = new ManifestWrapper(this._ffuImage, securityWrapper);
					this._swDismountImageTime.Start();
					uint storeHeaderVersion;
					if (this._parameters != null)
					{
						storeHeaderVersion = this._parameters.DeviceLayoutVersion;
					}
					else
					{
						storeHeaderVersion = (ImageGeneratorParameters.IsDeviceLayoutV2(Path.Combine(this.GetMainOSPath(), DevicePaths.ImageUpdatePath, "DeviceLayout.xml")) ? 2U : 1U);
					}
					bool saveChanges = true;
					bool deleteFile = true;
					this._storageManager.DismountFullFlashImage(saveChanges, payloadWrapper, deleteFile, storeHeaderVersion);
					this._swDismountImageTime.Stop();
					LongPathFile.WriteAllBytes(Path.ChangeExtension(this._outputFile, ".cat"), securityWrapper.CatalogData);
				}
				else
				{
					this._swDismountImageTime.Start();
					this._storageManager.DismountVirtualHardDisk(false, false, true);
					this._swDismountImageTime.Stop();
				}
			}
			catch (ImageCommonException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ImageCommonException("Imaging!FinalizeImage: Failed to finalize the " + this._outputType + ": " + ex.Message, ex);
			}
			finally
			{
				this.ReleaseMutex();
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00006EE4 File Offset: 0x000050E4
		private void UpdateUsedSectors()
		{
			if (this._ffuImage == null)
			{
				return;
			}
			this._storageManager.FlushVolumesForDismount();
			foreach (FullFlashUpdateImage.FullFlashUpdateStore fullFlashUpdateStore in this._ffuImage.Stores)
			{
				foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in fullFlashUpdateStore.Partitions)
				{
					if (!this._storageManager.PartitionIsMountedRaw(fullFlashUpdatePartition.Name))
					{
						try
						{
							this._storageManager.WaitForVolume(fullFlashUpdatePartition.Name);
							ulong freeBytesOnVolume = this._storageManager.GetFreeBytesOnVolume(fullFlashUpdatePartition.Name);
							fullFlashUpdatePartition.TotalSectors -= (uint)Math.Ceiling(freeBytesOnVolume / fullFlashUpdateStore.SectorSize);
						}
						catch (Exception ex)
						{
							throw new ImageCommonException("Imaging!UpdateUsedSectors: Failed to calculate free space on partition '" + fullFlashUpdatePartition.Name + "' : " + ex.Message, ex);
						}
					}
				}
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000701C File Offset: 0x0000521C
		private void ErrorLogger(string errorStr)
		{
			this._logger.LogError("{0}", new object[]
			{
				errorStr
			});
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00007038 File Offset: 0x00005238
		private void WarningLogger(string warnStr)
		{
			this._logger.LogWarning("{0}", new object[]
			{
				warnStr
			});
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00007054 File Offset: 0x00005254
		private void InformationLogger(string infoStr)
		{
			this._logger.LogInfo("{0}", new object[]
			{
				infoStr
			});
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00007070 File Offset: 0x00005270
		private void DebugLogger(string debugStr)
		{
			this._logger.LogDebug("{0}", new object[]
			{
				debugStr
			});
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000708C File Offset: 0x0000528C
		private void LoadPackages()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this._packageInfoList.Clear();
			foreach (string text in this._updateInput.PackageFiles)
			{
				try
				{
					if (this._packageInfoList.ContainsKey(text))
					{
						stringBuilder.AppendLine("Imaging!LoadPackages: The package '" + text + "' is contained in more than one feature specified by the OEMInput entries.");
					}
					else
					{
						IPkgInfo value = Package.LoadFromCab(text);
						this._packageInfoList.Add(text, value);
					}
				}
				catch (IUException ex)
				{
					stringBuilder.AppendLine("Imaging!LoadPackages: Unable to load package '" + text + "' due to the following error: " + ex.ToString());
				}
			}
			if (stringBuilder.Length != 0)
			{
				throw new ImageCommonException(stringBuilder.ToString());
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00007170 File Offset: 0x00005370
		private IFileEntry GetPackageFile(string packagePath, string deviceFile)
		{
			IPkgInfo pkgInfo = null;
			try
			{
				pkgInfo = Package.LoadFromCab(packagePath);
			}
			catch (PackageException ex)
			{
				throw new PackageException(ex, "Imaging!GetPackageFile: Failed to load '" + packagePath + "' package: " + ex.Message);
			}
			return this.GetPackageFile(pkgInfo, deviceFile);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000071C0 File Offset: 0x000053C0
		private IFileEntry GetPackageFile(IPkgInfo pkgInfo, string deviceFile)
		{
			IFileEntry result = null;
			if (pkgInfo != null)
			{
				result = pkgInfo.FindFile(deviceFile);
			}
			return result;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000071DC File Offset: 0x000053DC
		private string SaveToTempXMLFiles()
		{
			string text = Path.Combine(this._tempDirectoryPath, "XMLConfig");
			bool flag = false;
			bool flag2 = false;
			LongPathDirectory.CreateDirectory(text);
			if (this._releaseType != ReleaseType.Production && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IMAGING_DEVICELAYOUT_PACKAGE")))
			{
				this._deviceLayoutPackagePath = Environment.GetEnvironmentVariable("IMAGING_DEVICELAYOUT_PACKAGE");
			}
			if (this._releaseType != ReleaseType.Production && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IMAGING_DEVICEPLATFORM_PACKAGE")))
			{
				this._oemDevicePlatformPackagePath = Environment.GetEnvironmentVariable("IMAGING_DEVICEPLATFORM_PACKAGE");
			}
			if (string.IsNullOrEmpty(this._oemDevicePlatformPackagePath))
			{
				throw new PackageException("Imaging!SaveToTempXMLFiles: Unable to create image without OEMDevicePlatform.xml.  No OEMDevicePlatform package could be found in the Additional FMs.");
			}
			if (!File.Exists(this._oemDevicePlatformPackagePath))
			{
				throw new PackageException("Imaging!SaveToTempXMLFiles: Unable to create image without OEMDevicePlatform.xml.  The specified package cound not be found: '" + this._oemDevicePlatformPackagePath + "'");
			}
			IPkgInfo pkgInfo;
			try
			{
				pkgInfo = Package.LoadFromCab(this._oemDevicePlatformPackagePath);
			}
			catch (PackageException ex)
			{
				throw new PackageException(ex, "Imaging!SaveToTempXMLFiles: Failed to load '" + this._oemDevicePlatformPackagePath + "' package: " + ex.Message);
			}
			if (!this._isOneCore && pkgInfo.OwnerType != OwnerType.OEM)
			{
				throw new PackageException("Imaging!SaveToTempXMLFiles: The OEMDevicePlatform.xml must be contained in and OEM owned package.  '" + this._oemDevicePlatformPackagePath + "' package specified is not.");
			}
			IFileEntry packageFile = this.GetPackageFile(pkgInfo, this._oemDevicePlatformDevicePath);
			if (packageFile != null)
			{
				pkgInfo.ExtractFile(packageFile.DevicePath, Path.Combine(text, "OEMDevicePlatform.xml"), true);
				flag2 = true;
			}
			if (string.IsNullOrEmpty(this._deviceLayoutPackagePath) || !File.Exists(this._deviceLayoutPackagePath))
			{
				throw new PackageException("Imaging!SaveToTempXMLFiles: Unable to create image without DeviceLayout.xml.  The specified device '" + this._oemInput.Device + "' does not have an associated DeviceLayout in the Feature Manifest.");
			}
			IPkgInfo pkgInfo2;
			try
			{
				pkgInfo2 = Package.LoadFromCab(this._deviceLayoutPackagePath);
			}
			catch (PackageException ex2)
			{
				throw new PackageException(ex2, "Imaging!SaveToTempXMLFiles: Failed to load '" + this._deviceLayoutPackagePath + "' package: " + ex2.Message);
			}
			packageFile = this.GetPackageFile(pkgInfo2, this._deviceLayoutDevicePath);
			if (packageFile != null)
			{
				string text2 = Path.Combine(text, "DeviceLayout.xml");
				pkgInfo2.ExtractFile(packageFile.DevicePath, text2, true);
				this._deviceLayoutValidator.ValidateDeviceLayout(this._msCoreFMPackages.FirstOrDefault<IPkgInfo>(), pkgInfo2, this._deviceLayoutPackagePath, text2);
				flag = true;
			}
			if (!flag || !flag2)
			{
				string text3 = "ImageApp: Unable to create image without file(s):";
				FileUtils.DeleteTree(text);
				text = null;
				if (!flag)
				{
					text3 = text3 + Environment.NewLine + "DeviceLayout.xml";
				}
				if (!flag2)
				{
					text3 = text3 + Environment.NewLine + "OEMDevicePlatform.xml";
				}
				throw new ImageCommonException(text3);
			}
			return text;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00007450 File Offset: 0x00005650
		private void CleanupStorageManager(ImageStorageManager storageManager, bool deleteFile)
		{
			if (storageManager == null)
			{
				return;
			}
			if (this._bDoingFFU)
			{
				storageManager.DismountFullFlashImage(false);
				return;
			}
			storageManager.DismountVirtualHardDisk(deleteFile, deleteFile);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00007470 File Offset: 0x00005670
		public void CleanupHandler(object sender, ConsoleCancelEventArgs args)
		{
			try
			{
				this._swStorageStackTime.Start();
				bool deleteFile = true;
				this.CleanupStorageManager(this._storageManagerStaging, deleteFile);
				deleteFile = !this._bDoingUpdate;
				this.CleanupStorageManager(this._storageManagerCommit, deleteFile);
				this._swStorageStackTime.Stop();
				if (!string.IsNullOrEmpty(this._tempDirectoryPath))
				{
					FileUtils.DeleteTree(this._tempDirectoryPath);
				}
				Environment.SetEnvironmentVariable("WINDOWS_WCP_INSKUASSEMBLY", null);
				Environment.SetEnvironmentVariable("COMPONENT_BASED_SERVICING_LOGFILE", null);
				Environment.SetEnvironmentVariable("WINDOWS_TRACING_FLAGS", null);
				Environment.SetEnvironmentVariable("WINDOWS_TRACING_LOGFILE", null);
			}
			catch (Exception ex)
			{
				LogUtil.Diagnostic("Ignoring exception during cleanup: " + ex.ToString());
			}
			if (args != null)
			{
				Environment.Exit(1);
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00007530 File Offset: 0x00005730
		private string GetUpdateDescription(string updateHistoryFile)
		{
			string text = string.Empty;
			UpdateHistory updateHistory = null;
			try
			{
				updateHistory = UpdateHistory.ValidateUpdateHistory(updateHistoryFile, this._logger);
			}
			catch
			{
			}
			if (updateHistory != null && updateHistory.UpdateEvents != null)
			{
				foreach (UpdateEvent updateEvent in updateHistory.UpdateEvents)
				{
					if (!string.IsNullOrWhiteSpace(updateEvent.Summary))
					{
						text += updateEvent.Summary.Replace("\n", "\r\n", StringComparison.OrdinalIgnoreCase);
						text += Environment.NewLine;
					}
					else if (!string.IsNullOrWhiteSpace(updateEvent.UpdateResults.Description))
					{
						text += updateEvent.UpdateResults.Description.Replace("\n", "\r\n", StringComparison.OrdinalIgnoreCase);
						text += Environment.NewLine;
					}
					else
					{
						text = text + "Update on: " + updateEvent.DateTime;
						text += Environment.NewLine;
					}
				}
			}
			return text;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00007654 File Offset: 0x00005854
		public void AcquireMutex(IULogger logger)
		{
			this._imageAppMutex = new Mutex(false, "Global\\VHDMutex_{585b0806-2d3b-4226-b259-9c8d3b237d5c}");
			if (!this._imageAppMutex.WaitOne(0))
			{
				logger.LogInfo("Imaging - Another imaging tool is currently running.  Waiting for it to complete before continuing....", new object[0]);
				Imaging._swMutexTime.Start();
				bool flag = false;
				try
				{
					for (int i = 0; i < 800; i++)
					{
						if (this._imageAppMutex.WaitOne(this.MutexTimeout))
						{
							flag = true;
							logger.LogInfo("Imaging - Mutex acquired.", new object[0]);
							break;
						}
						if ((i + 1) % 6 == 0)
						{
							logger.LogInfo("Imaging - Still waiting for other imaging tools to complete. Current wait time: {0}:{1} minute(s)", new object[]
							{
								Imaging._swMutexTime.Elapsed.Minutes,
								Imaging._swMutexTime.Elapsed.Seconds
							});
						}
					}
				}
				catch (AbandonedMutexException)
				{
					flag = true;
				}
				Imaging._swMutexTime.Stop();
				if (!flag)
				{
					this._imageAppMutex = null;
					throw new ImageCommonException("Imaging: Failed to acquire VHD Mutex (timeout)");
				}
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00007760 File Offset: 0x00005960
		public void ReleaseMutex()
		{
			if (this._imageAppMutex != null)
			{
				this._imageAppMutex.ReleaseMutex();
				this._imageAppMutex = null;
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000777C File Offset: 0x0000597C
		private void ValidateMinFreeSectors()
		{
			if (this._parameters == null)
			{
				return;
			}
			foreach (InputPartition inputPartition in from x in this._parameters.MainOSStore.Partitions
			where x.MinFreeSectors > 0U
			select x)
			{
				this._logger.LogInfo("Imaging: Validating MinFreeSectors for partition '{0}'...", new object[]
				{
					inputPartition.Name
				});
				this._storageManager.WaitForVolume(inputPartition.Name);
				uint num = (uint)(this._storageManager.GetFreeBytesOnVolume(inputPartition.Name) / (ulong)this._parameters.SectorSize);
				uint num2 = (uint)Math.Abs((int)(inputPartition.MinFreeSectors - num));
				string format = string.Format("Imaging!ValidateMinFreeSectors: Partition '{0}' requested {1} minimum free sectors, {2} actual free sectors were found (difference of {3} sectors ({4} MB)).", new object[]
				{
					inputPartition.Name,
					inputPartition.MinFreeSectors,
					num,
					num2,
					(num2 * this._parameters.SectorSize / 1024U / 1024U).ToString("F")
				});
				if (num < inputPartition.MinFreeSectors)
				{
					if (ReleaseType.Production == (ReleaseType)Enum.Parse(typeof(ReleaseType), this._oemInput.ReleaseType))
					{
						this._logger.LogError(format, new object[0]);
					}
					else
					{
						this._logger.LogWarning(format, new object[0]);
					}
				}
				else
				{
					this._logger.LogInfo(format, new object[0]);
				}
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00007938 File Offset: 0x00005B38
		private ulong GetFileSystemOverhead(InputPartition partition, string stagedRoot)
		{
			ulong num = 0UL;
			this._storageManager.WaitForVolume(partition.Name);
			if (Directory.Exists(stagedRoot))
			{
				uint num2 = (uint)LongPathDirectory.GetDirectories(stagedRoot, "*", SearchOption.AllDirectories).Length;
				num += (ulong)(num2 * partition.ClusterSize);
			}
			if (string.Equals(partition.FileSystem, "NTFS", StringComparison.InvariantCultureIgnoreCase))
			{
				this._storageManager.CreateUsnJournal(partition.Name);
			}
			ulong num3 = this._storageManager.GetPartitionSize(partition.Name) * (ulong)this._parameters.SectorSize;
			num3 -= this._storageManager.GetFreeBytesOnVolume(partition.Name);
			return num + num3;
		}

		// Token: 0x0600006A RID: 106
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int IU_GetDirectorySize(string folder, bool recursive, uint clusterSize, out ulong folderSize);

		// Token: 0x0600006B RID: 107
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern uint IU_GetClusterSize(string folder);

		// Token: 0x0600006C RID: 108
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		private static extern int CopyAllFiles(string source, string dest, bool recursive, bool mirror);

		// Token: 0x0600006D RID: 109
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		public static extern int IU_CaptureUVMState(string pathOnDisk, string outputRoot);

		// Token: 0x0600006E RID: 110 RVA: 0x000079D8 File Offset: 0x00005BD8
		public static ulong AlignUp(ulong value, ulong boundary)
		{
			ulong num = value + boundary - 1UL;
			return num - num % boundary;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000079E4 File Offset: 0x00005BE4
		private void ProcessMinFreeSectors()
		{
			if (this._parameters == null)
			{
				throw new ImageCommonException("Imaging!ProcessMinFreeSectors: Incorrectly called before reading the DeviceLayout package.");
			}
			this._logger.LogInfo("Imaging: Processing MinFreeSectors...", new object[0]);
			foreach (InputPartition inputPartition in from x in this._parameters.MainOSStore.Partitions
			where x.MinFreeSectors > 0U
			select x)
			{
				this._storageManager.WaitForVolume(inputPartition.Name);
				string text = Path.Combine(this._updateStagingRoot, inputPartition.Name);
				ulong num = 0UL;
				uint num2 = inputPartition.ClusterSize;
				uint sectorSize = this._parameters.SectorSize;
				bool flag = inputPartition.Name.Equals(ImageConstants.MAINOS_PARTITION_NAME, StringComparison.OrdinalIgnoreCase);
				if (num2 == 0U)
				{
					num2 = Imaging.IU_GetClusterSize(this._storageManager.GetPartitionPath(inputPartition.Name));
					inputPartition.ClusterSize = num2;
				}
				if (Directory.Exists(text))
				{
					LongPathFile.Delete(Path.Combine(text, "ReservedSpace"));
					if (UpdateMain.FAILED(Imaging.IU_GetDirectorySize(text, true, num2, out num)))
					{
						throw new ImageCommonException(string.Format("Failed to get directory size for staged folder '{0}'", text));
					}
				}
				string pendingUpdateOSWimPath = this.GetPendingUpdateOSWimPath();
				if (flag && LongPathFile.Exists(pendingUpdateOSWimPath))
				{
					uint num3 = (uint)new FileInfo(pendingUpdateOSWimPath).Length;
					num += Imaging.AlignUp((ulong)num3, (ulong)num2);
				}
				ulong fileSystemOverhead = this.GetFileSystemOverhead(inputPartition, text);
				uint num4 = (uint)Math.Ceiling((Imaging.AlignUp(num, (ulong)num2) + Imaging.AlignUp(fileSystemOverhead, (ulong)num2)) / this._parameters.SectorSize);
				uint num5 = num2 / sectorSize;
				num4 = (uint)Imaging.AlignUp((ulong)num4, (ulong)num5);
				inputPartition.MinFreeSectors = (uint)Imaging.AlignUp((ulong)inputPartition.MinFreeSectors, (ulong)num5);
				inputPartition.GeneratedFileOverheadSectors = (uint)Imaging.AlignUp((ulong)inputPartition.GeneratedFileOverheadSectors, (ulong)num5);
				inputPartition.TotalSectors = num4 + inputPartition.MinFreeSectors + inputPartition.GeneratedFileOverheadSectors;
				if (flag)
				{
					inputPartition.TotalSectors += (uint)(inputPartition.TotalSectors * 0.04);
				}
				inputPartition.TotalSectors = (uint)Imaging.AlignUp((ulong)inputPartition.TotalSectors, (ulong)this.MBToSectors(1UL));
				this._logger.LogInfo(string.Format("\tResized partition '{0}' to {1} sectors ({2} MB, {3} clusters)", new object[]
				{
					inputPartition.Name,
					inputPartition.TotalSectors,
					(ulong)inputPartition.TotalSectors * (ulong)sectorSize / 1024UL / 1024UL,
					(ulong)(inputPartition.TotalSectors * sectorSize / num2)
				}), new object[0]);
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00007CB4 File Offset: 0x00005EB4
		private void EnforcePartitionRestrictions()
		{
			InputPartition inputPartition = this._parameters.MainOSStore.Partitions.FirstOrDefault((InputPartition x) => x.Name.Equals(ImageConstants.MAINOS_PARTITION_NAME, StringComparison.OrdinalIgnoreCase));
			if ((ulong)this._parameters.MinSectorCount <= 5368709120UL / (ulong)this._parameters.SectorSize && !inputPartition.Compressed)
			{
				throw new ImagingException("The MainOS partition is not marked as compressed, but the platform has less than 5 GB of space.  Please enable compression.");
			}
			uint num = 1717986918U;
			if (this._oemInput.Edition.MinimumUserStoreSize > 0U)
			{
				num = this._oemInput.Edition.MinimumUserStoreSize;
			}
			ulong partitionSize = this._storageManager.GetPartitionSize(ImageConstants.DATA_PARTITION_NAME);
			if (partitionSize < (ulong)(num / this._parameters.SectorSize) && ReleaseType.Production == (ReleaseType)Enum.Parse(typeof(ReleaseType), this._oemInput.ReleaseType))
			{
				throw new ImageCommonException(string.Format("The user store should be at least {0} bytes in size, but is only {1}.  Please reduce the size of other partitions in the image or increase the MinSectorCount to ensure users will have a sufficient amount of space.", num, partitionSize * (ulong)this._parameters.SectorSize));
			}
			IEnumerable<InputPartition> source = from x in this._parameters.MainOSStore.Partitions
			where x.RequiresCompression && x.ClusterSize > 4096U
			select x;
			if (source.Count<InputPartition>() > 0)
			{
				string arg = "{ " + string.Join(", ", (from x in source
				select x.Name).ToArray<string>()) + " }";
				throw new ImageCommonException(string.Format("Partitions {0} require compression, but have invalid (non-4k) cluster sizes.  Please change the layout to use 4k sectors or remove the compression requirement.", arg));
			}
			IEnumerable<InputPartition> source2 = from x in this._parameters.MainOSStore.Partitions
			where x.RequiresCompression && !x.Compressed
			select x;
			if (source2.Count<InputPartition>() > 0)
			{
				string arg2 = "{ " + string.Join(", ", (from x in source2
				select x.Name).ToArray<string>()) + " }";
				throw new ImagingException(string.Format("Partitions {0} require compression, but are not marked as compressed.  Please compress those partitions that require it, or switch layouts to remove compression requirements.", arg2));
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00007EF1 File Offset: 0x000060F1
		private uint MBToSectors(ulong RequestedMB)
		{
			return (uint)(RequestedMB * 1048576UL / (ulong)this._parameters.SectorSize);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00007F0C File Offset: 0x0000610C
		private void InitializeMinFreeSectors()
		{
			if (this._parameters == null)
			{
				throw new ImageCommonException("Imaging!ProcessMinFreeSectors: Incorrectly called before reading the DeviceLayout package.");
			}
			this._parameters.MinSectorCount = this.MBToSectors(102400UL);
			foreach (InputPartition inputPartition in from x in this._parameters.MainOSStore.Partitions
			where x.MinFreeSectors > 0U
			select x)
			{
				inputPartition.TotalSectors = (uint)Math.Ceiling((inputPartition.MinFreeSectors + inputPartition.GeneratedFileOverheadSectors) * 3.5);
				if (!string.Equals(inputPartition.Name, ImageConstants.MAINOS_PARTITION_NAME, StringComparison.OrdinalIgnoreCase))
				{
					inputPartition.TotalSectors += this.MBToSectors(1500UL);
				}
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00007FFC File Offset: 0x000061FC
		private void CopyPristineHivesForFactoryReset()
		{
			this._logger.LogInfo("ImageApp: Copying pristine hives for factory reset...", new object[0]);
			List<string> list = new List<string>
			{
				"SOFTWARE",
				"SYSTEM",
				"DRIVERS",
				"SAM",
				"SECURITY",
				"..\\..\\..\\USERS\\DEFAULT\\NTUSER.DAT",
				"DEFAULT",
				"COMPONENTS"
			};
			string path = "windows\\system32\\config";
			string sourceDir = Path.Combine(this.GetMainOSPath(), path);
			string cabPath = Path.Combine(this.GetDataPath(), "Windows\\ImageUpdate\\ImagingHives\\ImagingHives.cab");
			foreach (string path2 in list)
			{
				string text = Path.Combine(sourceDir, path2);
				int num = RegValidator.ValidateRegistryHive(text);
				if (num != 0)
				{
					throw new IUException("Registry hive validation failed for path '{0}', err '0x{1:X8}'", new object[]
					{
						text,
						num
					});
				}
			}
			CabArchiver cab = new CabArchiver();
			list.ForEach(delegate(string x)
			{
				cab.AddFile(Path.GetFileName(x), Path.Combine(sourceDir, x));
			});
			cab.Save(cabPath, CompressionType.FastLZX);
		}

		// Token: 0x04000028 RID: 40
		private IULogger _logger;

		// Token: 0x04000029 RID: 41
		private string _oemInputFile = string.Empty;

		// Token: 0x0400002A RID: 42
		private string _oemCustomizationXML = string.Empty;

		// Token: 0x0400002B RID: 43
		private string _oemCustomizationPPKG = string.Empty;

		// Token: 0x0400002C RID: 44
		private VersionInfo _oemCustomizationVersion;

		// Token: 0x0400002D RID: 45
		private string _msPackagesRoot = string.Empty;

		// Token: 0x0400002E RID: 46
		private string _updateInputFile = string.Empty;

		// Token: 0x0400002F RID: 47
		private string _updateInputFileGenerated = string.Empty;

		// Token: 0x04000030 RID: 48
		private string _outputFile = string.Empty;

		// Token: 0x04000031 RID: 49
		private bool _bDoingFFU = true;

		// Token: 0x04000032 RID: 50
		private bool _bDoingUpdate;

		// Token: 0x04000033 RID: 51
		private string _outputType = string.Empty;

		// Token: 0x04000034 RID: 52
		private string _catalogFile = string.Empty;

		// Token: 0x04000035 RID: 53
		private string _UOSOutputDestination = string.Empty;

		// Token: 0x04000036 RID: 54
		private string _PackageListFile = string.Empty;

		// Token: 0x04000037 RID: 55
		private string _UpdateHistoryDestination = string.Empty;

		// Token: 0x04000038 RID: 56
		private string _tempDirectoryPath = string.Empty;

		// Token: 0x04000039 RID: 57
		private string _updateStagingRoot = string.Empty;

		// Token: 0x0400003A RID: 58
		private DeviceLayoutValidator _deviceLayoutValidator = new DeviceLayoutValidator();

		// Token: 0x0400003B RID: 59
		private ReleaseType _releaseType;

		// Token: 0x0400003C RID: 60
		private List<IPkgInfo> _msCoreFMPackages = new List<IPkgInfo>();

		// Token: 0x0400003D RID: 61
		private Dictionary<string, IPkgInfo> _packageInfoList = new Dictionary<string, IPkgInfo>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x0400003E RID: 62
		private string _osVersion;

		// Token: 0x0400003F RID: 63
		private const string c_AntiTheftMinVersion = "1.1";

		// Token: 0x04000040 RID: 64
		private List<KeyValuePair<string, string>> _dataAssetFileList = new List<KeyValuePair<string, string>>();

		// Token: 0x04000041 RID: 65
		private readonly ImagingTelemetryLogger _telemetryLogger = ImagingTelemetryLogger.Instance;

		// Token: 0x04000042 RID: 66
		private ImageStorageManager _storageManager;

		// Token: 0x04000043 RID: 67
		private ImageStorageManager _storageManagerStaging;

		// Token: 0x04000044 RID: 68
		private ImageStorageManager _storageManagerCommit;

		// Token: 0x04000045 RID: 69
		private FullFlashUpdateImage _ffuImage;

		// Token: 0x04000046 RID: 70
		private ImageSigner _imgSigner;

		// Token: 0x04000047 RID: 71
		private UpdateOSInput _updateInput;

		// Token: 0x04000048 RID: 72
		private OEMInput _oemInput;

		// Token: 0x04000049 RID: 73
		private string _updateHistoryFile = string.Empty;

		// Token: 0x0400004A RID: 74
		private string _processedFilesDir = string.Empty;

		// Token: 0x0400004B RID: 75
		private string _oemDevicePlatformPackagePath = string.Empty;

		// Token: 0x0400004C RID: 76
		private string _oemDevicePlatformDevicePath = Path.Combine("\\" + DevicePaths.ImageUpdatePath, "OEMDevicePlatform.xml");

		// Token: 0x0400004D RID: 77
		private string _deviceLayoutPackagePath = string.Empty;

		// Token: 0x0400004E RID: 78
		private string _deviceLayoutDevicePath = Path.Combine("\\" + DevicePaths.ImageUpdatePath, "DeviceLayout.xml");

		// Token: 0x0400004F RID: 79
		private ImageGeneratorParameters _parameters;

		// Token: 0x04000050 RID: 80
		private bool _hasDPPPartition;

		// Token: 0x04000051 RID: 81
		private bool _isOneCore;

		// Token: 0x04000052 RID: 82
		public bool FormatDPP;

		// Token: 0x04000053 RID: 83
		public bool StrictSettingPolicies;

		// Token: 0x04000054 RID: 84
		public bool SkipImaging;

		// Token: 0x04000055 RID: 85
		public bool SkipUpdateMain;

		// Token: 0x04000056 RID: 86
		public CpuId CPUId;

		// Token: 0x04000057 RID: 87
		public string BSPProductName;

		// Token: 0x04000058 RID: 88
		private const string _processedFilesSubdir = "ProcessedFiles";

		// Token: 0x04000059 RID: 89
		private const string _deviceLayout = "DeviceLayout.xml";

		// Token: 0x0400005A RID: 90
		private const string _oemDevicePlatform = "OEMDevicePlatform.xml";

		// Token: 0x0400005B RID: 91
		private const string _imgupdFilesSubdir = "Windows\\ImageUpdate";

		// Token: 0x0400005C RID: 92
		private const int _ImageAppRetryCount = 800;

		// Token: 0x0400005D RID: 93
		private const int _DisplayMessageOnCount = 6;

		// Token: 0x0400005E RID: 94
		private TimeSpan MutexTimeout = new TimeSpan(0, 0, 0, 15);

		// Token: 0x0400005F RID: 95
		private Mutex _imageAppMutex;

		// Token: 0x04000060 RID: 96
		private DateTime _dtStartTime;

		// Token: 0x04000061 RID: 97
		private DateTime _dtEndTime;

		// Token: 0x04000062 RID: 98
		private Stopwatch _swReqXMLProcessingTime = new Stopwatch();

		// Token: 0x04000063 RID: 99
		private Stopwatch _swCreateFFUTime = new Stopwatch();

		// Token: 0x04000064 RID: 100
		private Stopwatch _swWritingFFUTime = new Stopwatch();

		// Token: 0x04000065 RID: 101
		private Stopwatch _swStorageStackTime = new Stopwatch();

		// Token: 0x04000066 RID: 102
		private Stopwatch _swMountImageTime = new Stopwatch();

		// Token: 0x04000067 RID: 103
		private Stopwatch _swDismountImageTime = new Stopwatch();

		// Token: 0x04000068 RID: 104
		private static Stopwatch _swMutexTime = new Stopwatch();

		// Token: 0x04000069 RID: 105
		private TimeSpan _tsCompDBTime;

		// Token: 0x0400006A RID: 106
		private TimeSpan _tsCompDBAnswersTime;

		// Token: 0x0400006B RID: 107
		private static readonly object _lock = new object();

		// Token: 0x0400006C RID: 108
		private const uint c_MinimumUserStoreSize = 1717986918U;

		// Token: 0x0400006D RID: 109
		private BSPCompDB _bspCompDB = new BSPCompDB();

		// Token: 0x0400006E RID: 110
		private DeviceCompDB _deviceCompDB = new DeviceCompDB();

		// Token: 0x0400006F RID: 111
		private string _bspDBFile;

		// Token: 0x04000070 RID: 112
		private string _deviceDBFile;

		// Token: 0x04000071 RID: 113
		private List<FMConditionalFeature> _condFeatures = new List<FMConditionalFeature>();

		// Token: 0x04000072 RID: 114
		private List<FMConditionalFeature> _oemConditionalFeatures = new List<FMConditionalFeature>();

		// Token: 0x04000073 RID: 115
		private string _buildInfo;

		// Token: 0x04000074 RID: 116
		private string _buildID;

		// Token: 0x04000075 RID: 117
		private static string c_BSPProductName_provxml = "<!--\r\n            Copyright (c) Microsoft Corporation.  All rights reserved.\r\n            -->\r\n            <wap-provisioningdoc>\r\n\r\n              <characteristic type=\"Registry\">\r\n                <!-- MS as OEM BSP Product name.  For internal use only-->\r\n                <characteristic type=\"HKLM\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Update\\TargetingInfo\\Overrides\\BSP\">\r\n                <parm name=\"Name\" value=\"$(BSPProductName)\"  />\r\n                </characteristic>\r\n              </characteristic>\r\n\r\n            </wap-provisioningdoc>";
	}
}
