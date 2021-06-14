using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000008 RID: 8
	public class DeviceLayoutValidator
	{
		// Token: 0x06000047 RID: 71 RVA: 0x000026B7 File Offset: 0x000008B7
		public void Initialize(IPkgInfo ReferencePkg, OEMInput OemInput, IULogger Logger, string TempDirectoryPath)
		{
			this._logger = Logger;
			this._oemInput = OemInput;
			this._tempDirectoryPath = TempDirectoryPath;
			DeviceLayoutValidatorExpressionEvaluator.Initialize(OemInput, Logger);
			this.ReadDeviceLayoutValidationManifest();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000026DC File Offset: 0x000008DC
		private void ReadDeviceLayoutValidationManifest()
		{
			foreach (string text in Assembly.GetExecutingAssembly().GetManifestResourceNames())
			{
				if (Regex.IsMatch(text, "^Microsoft\\..+\\.DeviceLayoutValidation.xml$", RegexOptions.IgnoreCase))
				{
					this._deviceLayoutValidationXmlFiles.Add(text);
				}
			}
			this._logger.LogInfo("DeviceLayoutValidation: Successfully read the Device Layout Validation Manifest - FileCount -> {0}", new object[]
			{
				this._deviceLayoutValidationXmlFiles.Count
			});
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000274C File Offset: 0x0000094C
		private static int GetScopeExpressionSpecificationLevel(string scope)
		{
			int num = 0;
			foreach (string text in scope.Split(new char[]
			{
				':'
			}))
			{
				num = 10 * num;
				if (!text.Equals(".*") && !text.Equals(".+"))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000027A4 File Offset: 0x000009A4
		private string GetDeviceLayoutValidationFileInScope(string versionTag)
		{
			string text = null;
			string text2 = null;
			string text3 = string.Concat(new string[]
			{
				this._oemInput.CPUType,
				":",
				this._oemInput.SV,
				":",
				this._oemInput.SOC,
				":",
				this._oemInput.Device,
				":",
				this._oemInput.ReleaseType
			});
			foreach (string str in versionTag.Split(new char[]
			{
				'.',
				':'
			}))
			{
				text3 = text3 + ":" + str;
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			if (this._deviceLayoutValidationXmlFiles != null)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(DeviceLayoutValidationScope));
				int num = 0;
				foreach (string text4 in this._deviceLayoutValidationXmlFiles)
				{
					DeviceLayoutValidationScope deviceLayoutValidationScope;
					using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text4))
					{
						if (manifestResourceStream == null)
						{
							throw new DeviceLayoutValidationException(DeviceLayoutValidationError.UnknownInternalError, "DeviceLayoutValidation:GetDeviceLayoutValidationFileInScope - unable to open stream to resource : " + text4);
						}
						try
						{
							deviceLayoutValidationScope = (DeviceLayoutValidationScope)xmlSerializer.Deserialize(manifestResourceStream);
						}
						catch (Exception inner)
						{
							throw new PackageException(inner, "DeviceLayoutValidation: ValidateDeviceLayout Unable to parse Device Layout XML.");
						}
					}
					bool flag = Regex.IsMatch(text3, deviceLayoutValidationScope.Scope, RegexOptions.IgnoreCase);
					if (flag)
					{
						if (deviceLayoutValidationScope.ExcludedScopes != null)
						{
							foreach (string text5 in deviceLayoutValidationScope.ExcludedScopes)
							{
								if (Regex.IsMatch(text3, text5, RegexOptions.IgnoreCase))
								{
									this._logger.LogInfo("DeviceLayoutValidation: Validation file [{0}] matched for scope -> [{1}->{2} but was excluded by [{3}]", new object[]
									{
										text4,
										text3,
										deviceLayoutValidationScope.Scope,
										text5
									});
									flag = false;
									break;
								}
							}
							if (flag)
							{
								this._logger.LogInfo("DeviceLayoutValidation: Validation file [{0}] matched for scope -> [{1}->{2}] and did not match any exclusions", new object[]
								{
									text4,
									text3,
									deviceLayoutValidationScope.Scope
								});
							}
						}
						else
						{
							this._logger.LogInfo("DeviceLayoutValidation: Validation file [{0}] matched for scope -> [{1}->{2}] - no exclusions", new object[]
							{
								text4,
								text3,
								deviceLayoutValidationScope.Scope
							});
						}
					}
					if (flag)
					{
						int scopeExpressionSpecificationLevel = DeviceLayoutValidator.GetScopeExpressionSpecificationLevel(deviceLayoutValidationScope.Scope);
						this._logger.LogInfo("DeviceLayoutValidation: Validation file [{0}] was in for scope -> [{1}->{2}:{3}]", new object[]
						{
							text4,
							text3,
							deviceLayoutValidationScope.Scope,
							scopeExpressionSpecificationLevel
						});
						if (scopeExpressionSpecificationLevel > num)
						{
							this._logger.LogInfo("DeviceLayoutValidation: Validation file [{0}] was picked for scope -> [{1}->{2}]", new object[]
							{
								text4,
								text3,
								deviceLayoutValidationScope.Scope
							});
							text2 = text4;
							num = scopeExpressionSpecificationLevel;
						}
					}
					else
					{
						this._logger.LogDebug("DeviceLayoutValidation: Validation file [{0}] was NOT in for scope -> [{1}->{2}]", new object[]
						{
							text4,
							text3,
							deviceLayoutValidationScope.Scope
						});
					}
				}
			}
			if (text2 == null)
			{
				this._logger.LogInfo("DeviceLayoutValidation: could not find validation file scope -> [{0}]", new object[]
				{
					text3
				});
			}
			else
			{
				this._logger.LogInfo("DeviceLayoutValidation: Validation file [{0}] was picked for scope -> [{1}]", new object[]
				{
					text2,
					text3
				});
				text = Path.Combine(this._tempDirectoryPath, text2);
				using (Stream manifestResourceStream2 = executingAssembly.GetManifestResourceStream(text2))
				{
					try
					{
						using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write))
						{
							manifestResourceStream2.CopyTo(fileStream);
						}
					}
					catch (Exception inner2)
					{
						throw new PackageException(inner2, "DeviceLayoutValidation: ValidateDeviceLayout Unable to write Device Layout XML.");
					}
				}
			}
			return text;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002BDC File Offset: 0x00000DDC
		private bool ValidateDeviceLayoutValue(bool BoolValue, string Comparator, string ComparatorDefault = "false")
		{
			if (string.IsNullOrEmpty(Comparator))
			{
				Comparator = ComparatorDefault;
			}
			if (Comparator.Equals("*ANY*", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			bool flag;
			if (bool.TryParse(Comparator, out flag))
			{
				return flag == BoolValue;
			}
			if (Comparator.StartsWith("allow_default_plus_", StringComparison.OrdinalIgnoreCase))
			{
				if (!BoolValue)
				{
					this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutValue succeeded since [{0}] - allows the default value !!!", new object[]
					{
						Comparator
					});
					return true;
				}
				Comparator = Comparator.Substring("allow_default_plus_".Length);
			}
			return DeviceLayoutValidatorExpressionEvaluator.EvaluateBooleanExpression(BoolValue.ToString(), Comparator);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002C64 File Offset: 0x00000E64
		private bool ValidateDeviceLayoutValue(string StringValue, string Comparator, string ComparatorDefault = "")
		{
			if (Comparator == null)
			{
				Comparator = ComparatorDefault;
			}
			if (Comparator.Equals("*ANY*", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (StringValue == null)
			{
				if (string.IsNullOrEmpty(Comparator))
				{
					return true;
				}
				if (Comparator.StartsWith("allow_default_plus_", StringComparison.OrdinalIgnoreCase))
				{
					this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutValue succeeded since [{0}] - allows the default value !!!", new object[]
					{
						Comparator
					});
					return true;
				}
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutValue Failed in validation of field value [{0}] but field was empty or not specified as required !!!", new object[]
				{
					Comparator
				});
				return false;
			}
			else
			{
				if (StringValue.Equals(Comparator, StringComparison.Ordinal))
				{
					return true;
				}
				if (string.IsNullOrEmpty(Comparator))
				{
					this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutValue Failed in validation of field value [{0}] - field was NOT empty as required !!!", new object[]
					{
						StringValue
					});
					return false;
				}
				if (Comparator.StartsWith("allow_default_plus_", StringComparison.OrdinalIgnoreCase))
				{
					if (StringValue == "")
					{
						this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutValue succeeded since [{0}] - allows the default value !!!", new object[]
						{
							Comparator
						});
						return true;
					}
					Comparator = Comparator.Substring("allow_default_plus_".Length);
				}
				DeviceLayoutValidatorExpressionEvaluator.EvaluateBooleanExpression(StringValue, Comparator);
				return true;
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002D5C File Offset: 0x00000F5C
		private bool ValidateDeviceLayoutValue(uint UintValue, string Comparator, string ComparatorDefault = "0")
		{
			if (string.IsNullOrEmpty(Comparator))
			{
				Comparator = ComparatorDefault;
			}
			if (Comparator.Equals("*ANY*", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			uint num;
			if (DeviceLayoutValidator.StringToUint(Comparator, out num))
			{
				return num == UintValue;
			}
			if (Comparator.StartsWith("allow_default_plus_", StringComparison.OrdinalIgnoreCase))
			{
				if (UintValue == 0U)
				{
					this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutValue succeeded since [{0}] - allows the default value !!!", new object[]
					{
						Comparator
					});
					return true;
				}
				Comparator = Comparator.Substring("allow_default_plus_".Length);
			}
			return DeviceLayoutValidatorExpressionEvaluator.EvaluateBooleanExpression(UintValue.ToString(), Comparator);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002DE4 File Offset: 0x00000FE4
		private bool GetParentDeviceLayoutValue(string ValidationRuleValue, bool ParentValue, ref bool OriginalValue)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(ValidationRuleValue))
			{
				if (ValidationRuleValue.Equals("*PARENT*", StringComparison.OrdinalIgnoreCase))
				{
					OriginalValue = ParentValue;
					result = true;
				}
				else
				{
					result = bool.TryParse(ValidationRuleValue, out OriginalValue);
				}
			}
			return result;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002E1C File Offset: 0x0000101C
		private bool GetParentDeviceLayoutValue(string ValidationRuleValue, string ParentValue, ref string OriginalValue)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(ValidationRuleValue))
			{
				result = true;
				if (ValidationRuleValue.Equals("*PARENT*", StringComparison.OrdinalIgnoreCase))
				{
					OriginalValue = ParentValue;
				}
				else
				{
					OriginalValue = ValidationRuleValue;
				}
			}
			return result;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002E4C File Offset: 0x0000104C
		private bool GetParentDeviceLayoutValue(string ValidationRuleValue, uint ParentValue, ref uint OriginalValue)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(ValidationRuleValue))
			{
				if (ValidationRuleValue.Equals("*PARENT*", StringComparison.OrdinalIgnoreCase))
				{
					OriginalValue = ParentValue;
					result = true;
				}
				else
				{
					result = DeviceLayoutValidator.StringToUint(ValidationRuleValue, out OriginalValue);
				}
			}
			return result;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002E84 File Offset: 0x00001084
		public static bool StringToUint(string ValueAsString, out uint Value)
		{
			bool result = true;
			if (ValueAsString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			{
				if (!uint.TryParse(ValueAsString.Substring(2, ValueAsString.Length - 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out Value))
				{
					result = false;
				}
			}
			else if (!uint.TryParse(ValueAsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out Value))
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002ED8 File Offset: 0x000010D8
		private bool ValidateDeviceLayoutFields(InputPartition Partition, InputValidationPartition ValidationPartition, uint LayoutSectorSize, uint RulesSectorSize)
		{
			bool result = true;
			if (LayoutSectorSize != RulesSectorSize)
			{
				this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutFields LayoutSectorSize [{0}] does not match RulesSectorSize [{1}] - ALL sector sizes will be scaled by [{0}/{1}] before checking of rules !!!", new object[]
				{
					LayoutSectorSize,
					RulesSectorSize
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.Type, ValidationPartition.PartitionType, ""))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [PartitionType] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.Type,
					ValidationPartition.PartitionType
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.Bootable, ValidationPartition.Bootable, "false"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [Bootable] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.Bootable,
					ValidationPartition.Bootable
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.ReadOnly, ValidationPartition.ReadOnly, "false"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [ReadOnly] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.ReadOnly,
					ValidationPartition.ReadOnly
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.Hidden, ValidationPartition.Hidden, "false"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [Hidden] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.Hidden,
					ValidationPartition.Hidden
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.AttachDriveLetter, ValidationPartition.AttachDriveLetter, "false"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [AttachDriveLetter] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.AttachDriveLetter,
					ValidationPartition.AttachDriveLetter
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.UseAllSpace, ValidationPartition.UseAllSpace, "false"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [UseAllSpace] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.UseAllSpace,
					ValidationPartition.UseAllSpace
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.TotalSectors * LayoutSectorSize / RulesSectorSize, ValidationPartition.TotalSectors, "0"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [TotalSectors] mismatched : found [{1}], scaled_to[{2}] -> expected [{3}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.TotalSectors,
					Partition.TotalSectors * LayoutSectorSize / RulesSectorSize,
					ValidationPartition.TotalSectors
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.MinFreeSectors * LayoutSectorSize / RulesSectorSize, ValidationPartition.MinFreeSectors, "0"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [MinFreeSectors] mismatched : found [{1}], scaled_to[{2}] -> expected [{3}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.MinFreeSectors,
					Partition.MinFreeSectors * LayoutSectorSize / RulesSectorSize,
					ValidationPartition.MinFreeSectors
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.RequiresCompression, ValidationPartition.RequiresCompression, "false"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [RequiresCompression] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.RequiresCompression,
					ValidationPartition.RequiresCompression
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.FileSystem, ValidationPartition.FileSystem, ""))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [FileSystem] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.FileSystem,
					ValidationPartition.FileSystem
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.RequiredToFlash, ValidationPartition.RequiredToFlash, "false"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [RequiredToFlash] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.RequiredToFlash,
					ValidationPartition.RequiredToFlash
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.PrimaryPartition, ValidationPartition.PrimaryPartition, Partition.Name))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [PrimaryPartition] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.PrimaryPartition,
					ValidationPartition.PrimaryPartition
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.SingleSectorAlignment, ValidationPartition.SingleSectorAlignment, "false"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [SingleSectorAlignment] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.SingleSectorAlignment,
					ValidationPartition.SingleSectorAlignment
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.ByteAlignment, ValidationPartition.ByteAlignment, "0"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [ByteAlignment] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.ByteAlignment,
					ValidationPartition.ByteAlignment
				});
			}
			if (!this.ValidateDeviceLayoutValue(Partition.ClusterSize, ValidationPartition.ClusterSize, "0"))
			{
				result = false;
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayoutFields Failed in validation of partition [{0}] value for field [ClusterSize] mismatched : found [{1}] -> expected [{2}] !!!", new object[]
				{
					ValidationPartition.Name,
					Partition.ClusterSize,
					ValidationPartition.ClusterSize
				});
			}
			return result;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000033F8 File Offset: 0x000015F8
		private void ValidateDeviceLayout(DeviceLayoutInput XmlDeviceLayout, DeviceLayoutValidationInput XmlDeviceLayoutValidation, out bool deviceLayoutHasChanged)
		{
			bool flag = true;
			int num = 1;
			InputValidationPartition inputValidationPartition = null;
			List<InputPartition> list = new List<InputPartition>(XmlDeviceLayout.Partitions);
			deviceLayoutHasChanged = false;
			DeviceLayoutValidationError deviceLayoutValidationError = DeviceLayoutValidationError.Pass;
			this._logger.LogInfo("DeviceLayoutValidation: Validating device layout attributes", new object[0]);
			if (!this.ValidateDeviceLayoutValue(XmlDeviceLayout.SectorSize, XmlDeviceLayoutValidation.SectorSize, "0"))
			{
				flag = false;
				if (deviceLayoutValidationError == DeviceLayoutValidationError.Pass)
				{
					deviceLayoutValidationError = DeviceLayoutValidationError.DeviceLayoutAttributeMismatch;
				}
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayout Failed in validation of SectorSize : found [{0}] -> expected [{1}]", new object[]
				{
					XmlDeviceLayout.SectorSize,
					XmlDeviceLayoutValidation.SectorSize
				});
			}
			if (!this.ValidateDeviceLayoutValue(XmlDeviceLayout.ChunkSize, XmlDeviceLayoutValidation.ChunkSize, "0"))
			{
				flag = false;
				if (deviceLayoutValidationError == DeviceLayoutValidationError.Pass)
				{
					deviceLayoutValidationError = DeviceLayoutValidationError.DeviceLayoutAttributeMismatch;
				}
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayout Failed in validation of ChunkSize : found [{0}] -> expected [{1}]", new object[]
				{
					XmlDeviceLayout.ChunkSize,
					XmlDeviceLayoutValidation.ChunkSize
				});
			}
			if (!this.ValidateDeviceLayoutValue(XmlDeviceLayout.DefaultPartitionByteAlignment, XmlDeviceLayoutValidation.DefaultPartitionByteAlignment, "0"))
			{
				flag = false;
				if (deviceLayoutValidationError == DeviceLayoutValidationError.Pass)
				{
					deviceLayoutValidationError = DeviceLayoutValidationError.DeviceLayoutAttributeMismatch;
				}
				this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayout Failed in validation of DefaultPartitionByteAlignment : found [{0}] -> expected [{1}]", new object[]
				{
					XmlDeviceLayout.DefaultPartitionByteAlignment,
					XmlDeviceLayoutValidation.DefaultPartitionByteAlignment
				});
			}
			if (XmlDeviceLayoutValidation.Partitions != null)
			{
				this._logger.LogInfo("DeviceLayoutValidation: Validating partition existence and positioning", new object[0]);
				foreach (InputValidationPartition inputValidationPartition2 in XmlDeviceLayoutValidation.Partitions)
				{
					InputPartition inputPartition = null;
					int num2 = 0;
					foreach (InputPartition inputPartition2 in XmlDeviceLayout.Partitions)
					{
						num2++;
						if (inputPartition2.Name.Equals(inputValidationPartition2.Name, StringComparison.OrdinalIgnoreCase))
						{
							inputPartition = inputPartition2;
							break;
						}
					}
					if (inputPartition == null)
					{
						if (inputValidationPartition2.Name.Equals("BACKUP_*", StringComparison.OrdinalIgnoreCase))
						{
							inputValidationPartition = inputValidationPartition2;
						}
						else
						{
							bool flag2 = false;
							if (!bool.TryParse(inputValidationPartition2.Optional, out flag2))
							{
								this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayout found unsupported value in [Optional] field of partition [{0}] - Defaulting to false.", new object[]
								{
									inputValidationPartition2.Name
								});
							}
							if (!flag2)
							{
								flag = false;
								if (deviceLayoutValidationError == DeviceLayoutValidationError.Pass)
								{
									deviceLayoutValidationError = DeviceLayoutValidationError.PartitionNotFound;
								}
								this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayout Failed in validation of partition [{0}]  - NOT FOUND !!!", new object[]
								{
									inputValidationPartition2.Name
								});
							}
							else
							{
								this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayout optional partition [{0}]  - NOT FOUND", new object[]
								{
									inputValidationPartition2.Name
								});
							}
						}
					}
					else if (inputPartition.Name.Equals("BACKUP_*", StringComparison.OrdinalIgnoreCase))
					{
						flag = false;
						if (deviceLayoutValidationError == DeviceLayoutValidationError.Pass)
						{
							deviceLayoutValidationError = DeviceLayoutValidationError.PartitionInvalidName;
						}
						this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayout Failed in validation of partition [{0}]  - INVALID NAME !!!", new object[]
						{
							inputValidationPartition2.Name
						});
					}
					else
					{
						if (inputValidationPartition2.Position != null)
						{
							int num3 = 0;
							if (!int.TryParse(inputValidationPartition2.Position, out num3))
							{
								this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayout found unsupported value in [Position] field of partition [{0}] - Defaulting to 0.", new object[]
								{
									inputValidationPartition2.Name
								});
							}
							if (num3 == 0)
							{
								if (inputValidationPartition2.Position.Equals("BACKUP_BOOKEND", StringComparison.OrdinalIgnoreCase))
								{
									num = num2;
								}
							}
							else if (num3 > 0)
							{
								if (num3 != num2)
								{
									flag = false;
									if (deviceLayoutValidationError == DeviceLayoutValidationError.Pass)
									{
										deviceLayoutValidationError = DeviceLayoutValidationError.PartitionPositionMismatch;
									}
									this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayout Failed in validation of partition [{0}]  - POSITION MISMATCH - position is [{1}] but was expecting [{2}] !!!", new object[]
									{
										inputValidationPartition2.Name,
										num2,
										inputValidationPartition2.Position
									});
								}
								else
								{
									this._logger.LogInfo("DeviceLayoutValidation: Validated correct position for partition -> " + inputValidationPartition2.Name, new object[0]);
								}
							}
							else
							{
								int num4 = num2 - XmlDeviceLayout.Partitions.Length - 1;
								if (num3 != num4)
								{
									flag = false;
									if (deviceLayoutValidationError == DeviceLayoutValidationError.Pass)
									{
										deviceLayoutValidationError = DeviceLayoutValidationError.PartitionPositionMismatch;
									}
									this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayout Failed in validation of partition [{0}]  - POSITION MISMATCH - position is [{1}] but was expecting [{2}] !!!", new object[]
									{
										inputValidationPartition2.Name,
										num4,
										inputValidationPartition2.Position
									});
								}
								else
								{
									this._logger.LogInfo("DeviceLayoutValidation: Correct position for partition -> " + inputValidationPartition2.Name, new object[0]);
								}
							}
						}
						this._logger.LogInfo("DeviceLayoutValidation: Validating attributes for partition -> " + inputValidationPartition2.Name, new object[0]);
						if (!this.ValidateDeviceLayoutFields(inputPartition, inputValidationPartition2, XmlDeviceLayout.SectorSize, XmlDeviceLayoutValidation.RulesSectorSize))
						{
							flag = false;
							if (deviceLayoutValidationError == DeviceLayoutValidationError.Pass)
							{
								deviceLayoutValidationError = DeviceLayoutValidationError.PartitionAttributeValueMismatch;
							}
						}
					}
				}
				if (inputValidationPartition != null)
				{
					this._logger.LogInfo("DeviceLayoutValidation: creating BACKUP partitions for partitions marked with UpdateType = 'Critical'", new object[0]);
					foreach (InputPartition inputPartition3 in XmlDeviceLayout.Partitions)
					{
						if (!inputPartition3.Name.StartsWith("BACKUP_", StringComparison.OrdinalIgnoreCase))
						{
							string backupPartitionName = "BACKUP_" + inputPartition3.Name;
							InputPartition inputPartition4 = Array.Find<InputPartition>(XmlDeviceLayout.Partitions, (InputPartition p) => p.Name.Equals(backupPartitionName, StringComparison.OrdinalIgnoreCase));
							string text = inputPartition3.UpdateType ?? "Normal";
							if (inputPartition4 == null)
							{
								this._logger.LogInfo("DeviceLayoutValidation: Partition [{0}] has no backup - UpdateType is [{1}]", new object[]
								{
									inputPartition3.Name,
									text
								});
								if (text.Equals("Critical", StringComparison.OrdinalIgnoreCase))
								{
									string text2 = "";
									uint num5 = 0U;
									bool flag3 = false;
									inputPartition4 = new InputPartition();
									inputPartition4.Name = "BACKUP_" + inputPartition3.Name;
									this._logger.LogInfo("DeviceLayoutValidation: Creating new backup partition : [{0}] for partition : [{1}]", new object[]
									{
										inputPartition4.Name,
										inputPartition3.Name
									});
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.PartitionType, inputPartition3.Type, ref text2))
									{
										inputPartition4.Type = text2;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.ReadOnly, inputPartition3.ReadOnly, ref flag3))
									{
										inputPartition4.ReadOnly = flag3;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.AttachDriveLetter, inputPartition3.AttachDriveLetter, ref flag3))
									{
										inputPartition4.AttachDriveLetter = flag3;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.Hidden, inputPartition3.Hidden, ref flag3))
									{
										inputPartition4.Hidden = flag3;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.Bootable, inputPartition3.Bootable, ref flag3))
									{
										inputPartition4.Bootable = flag3;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.TotalSectors, inputPartition3.TotalSectors, ref num5))
									{
										inputPartition4.TotalSectors = num5;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.MinFreeSectors, inputPartition3.MinFreeSectors, ref num5))
									{
										inputPartition4.MinFreeSectors = num5;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.UseAllSpace, inputPartition3.UseAllSpace, ref flag3))
									{
										inputPartition4.UseAllSpace = flag3;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.FileSystem, inputPartition3.FileSystem, ref text2))
									{
										inputPartition4.FileSystem = text2;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.UpdateType, inputPartition3.UpdateType, ref text2))
									{
										inputPartition4.UpdateType = text2;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.Compressed, inputPartition3.Compressed, ref flag3))
									{
										inputPartition4.Compressed = flag3;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.RequiredToFlash, inputPartition3.RequiredToFlash, ref flag3))
									{
										inputPartition4.RequiredToFlash = flag3;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.SingleSectorAlignment, inputPartition3.SingleSectorAlignment, ref flag3))
									{
										inputPartition4.SingleSectorAlignment = flag3;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.ByteAlignment, inputPartition3.ByteAlignment, ref num5))
									{
										inputPartition4.ByteAlignment = num5;
									}
									if (this.GetParentDeviceLayoutValue(inputValidationPartition.ClusterSize, inputPartition3.ClusterSize, ref num5))
									{
										inputPartition4.ClusterSize = num5;
									}
									list.Insert(num - 1, inputPartition4);
									num++;
								}
							}
						}
					}
					if (list.Count > XmlDeviceLayout.Partitions.Count<InputPartition>())
					{
						this._logger.LogInfo("DeviceLayoutValidation: [{0}] new backup partitions created", new object[]
						{
							list.Count - XmlDeviceLayout.Partitions.Count<InputPartition>()
						});
						XmlDeviceLayout.Partitions = list.ToArray();
						deviceLayoutHasChanged = true;
					}
				}
				this._logger.LogInfo("DeviceLayoutValidation: Validating partition BACKUP rules", new object[0]);
				foreach (InputPartition inputPartition5 in XmlDeviceLayout.Partitions)
				{
					if (!inputPartition5.Name.StartsWith("BACKUP_", StringComparison.OrdinalIgnoreCase))
					{
						string backupPartitionName = "BACKUP_" + inputPartition5.Name;
						InputPartition inputPartition6 = Array.Find<InputPartition>(XmlDeviceLayout.Partitions, (InputPartition p) => p.Name.Equals(backupPartitionName, StringComparison.OrdinalIgnoreCase));
						string text3 = inputPartition5.UpdateType ?? "Normal";
						if (inputPartition6 == null)
						{
							this._logger.LogInfo("DeviceLayoutValidation: Partition [{0}] has no backup - UpdateType is [{1}]", new object[]
							{
								inputPartition5.Name,
								text3
							});
							if (text3.Equals("Critical", StringComparison.OrdinalIgnoreCase))
							{
								flag = false;
								if (deviceLayoutValidationError == DeviceLayoutValidationError.Pass)
								{
									deviceLayoutValidationError = DeviceLayoutValidationError.BackupPartitionNotFound;
								}
								this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayout Failed in validation of partition [{0}]  - partition with UpdateType critical does not have a backup partition !!!", new object[]
								{
									inputPartition5.Name
								});
							}
						}
						else
						{
							this._logger.LogInfo("DeviceLayoutValidation: Partition [{0}] has backup - UpdateType is [{1}]", new object[]
							{
								inputPartition5.Name,
								text3
							});
							if (inputPartition5.TotalSectors != inputPartition6.TotalSectors)
							{
								flag = false;
								if (deviceLayoutValidationError == DeviceLayoutValidationError.Pass)
								{
									deviceLayoutValidationError = DeviceLayoutValidationError.BackupPartitionSizeMismatch;
								}
								this._logger.LogError("DeviceLayoutValidation: ValidateDeviceLayout Failed in validation of partition [{0}] and [{1}]  - partition has a different size (TotalSectors) [{2}] from its backup [{3}] !!!", new object[]
								{
									inputPartition5.Name,
									inputPartition6.Name,
									inputPartition5.TotalSectors,
									inputPartition6.TotalSectors
								});
							}
						}
					}
				}
			}
			this._logger.LogInfo("DeviceLayoutValidation: Validating completed - results => [{0}]", new object[]
			{
				flag
			});
			if (!flag)
			{
				throw new DeviceLayoutValidationException(deviceLayoutValidationError, "DeviceLayoutValidation: ValidateDeviceLayout DeviceLayout does not comply with Microsoft rules for the SOC");
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003D98 File Offset: 0x00001F98
		private void ReadDeviceLayoutXmlFile(string DeviceLayoutXMLFile, ref DeviceLayoutInput xmlDeviceLayout, ref DeviceLayoutInputv2 xmlDeviceLayoutv2)
		{
			XsdValidator xsdValidator = new XsdValidator();
			try
			{
				using (Stream deviceLayoutXSD = ImageGeneratorParameters.GetDeviceLayoutXSD(DeviceLayoutXMLFile))
				{
					xsdValidator.ValidateXsd(deviceLayoutXSD, DeviceLayoutXMLFile, this._logger);
				}
			}
			catch (XsdValidatorException inner)
			{
				throw new PackageException(inner, "DeviceLayoutValidation: ValidateDeviceLayout Unable to validate Device Layout XSD.");
			}
			this._logger.LogInfo("DeviceLayoutValidation: Successfully validated the Device Layout XML", new object[0]);
			if (ImageGeneratorParameters.IsDeviceLayoutV2(DeviceLayoutXMLFile))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(DeviceLayoutInputv2));
				using (StreamReader streamReader = new StreamReader(DeviceLayoutXMLFile))
				{
					try
					{
						xmlDeviceLayoutv2 = (DeviceLayoutInputv2)xmlSerializer.Deserialize(streamReader);
					}
					catch (Exception inner2)
					{
						throw new PackageException(inner2, "DeviceLayoutValidation: ValidateDeviceLayout Unable to parse Device Layout XML.");
					}
				}
				if (xmlDeviceLayoutv2.VersionTag == null || xmlDeviceLayoutv2.VersionTag.Equals(""))
				{
					xmlDeviceLayoutv2.VersionTag = "WindowsMobile.10.0000";
					return;
				}
			}
			else
			{
				XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(DeviceLayoutInput));
				using (StreamReader streamReader2 = new StreamReader(DeviceLayoutXMLFile))
				{
					try
					{
						xmlDeviceLayout = (DeviceLayoutInput)xmlSerializer2.Deserialize(streamReader2);
					}
					catch (Exception inner3)
					{
						throw new PackageException(inner3, "DeviceLayoutValidation: ValidateDeviceLayout Unable to parse Device Layout XML.");
					}
				}
				if (xmlDeviceLayout.VersionTag == null || xmlDeviceLayout.VersionTag.Equals(""))
				{
					xmlDeviceLayout.VersionTag = "WindowsMobile.10.0000";
				}
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003F14 File Offset: 0x00002114
		private void ValidateDeviceLayout(DeviceLayoutInput XmlDeviceLayout, string DeviceLayoutXMLFile, string DeviceLayoutValidationXMLFile)
		{
			DeviceLayoutValidationInput xmlDeviceLayoutValidation = null;
			bool flag = false;
			this._logger.LogInfo("DeviceLayoutValidation: Successfully validated the Device Layout Validation XML", new object[0]);
			XmlValidator xmlValidator = new XmlValidator();
			try
			{
				xmlValidator.ValidateXmlAndAddDefaults("DeviceLayoutValidation.xsd", DeviceLayoutValidationXMLFile, this._logger);
			}
			catch (XmlValidatorException inner)
			{
				throw new PackageException(inner, "DeviceLayoutValidation: ValidateDeviceLayout Unable to validate Device Layout Validation - XSD.");
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DeviceLayoutValidationInput));
			using (StreamReader streamReader = new StreamReader(DeviceLayoutValidationXMLFile))
			{
				try
				{
					xmlDeviceLayoutValidation = (DeviceLayoutValidationInput)xmlSerializer.Deserialize(streamReader);
				}
				catch (Exception inner2)
				{
					throw new PackageException(inner2, "DeviceLayoutValidation: ValidateDeviceLayout Unable to parse Device Layout XML.");
				}
			}
			this.ValidateDeviceLayout(XmlDeviceLayout, xmlDeviceLayoutValidation, out flag);
			if (flag)
			{
				this._logger.LogInfo("DeviceLayoutValidation: Device layout has changed - regenerating file [" + DeviceLayoutXMLFile + "]", new object[0]);
				XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(DeviceLayoutInput));
				using (StreamWriter streamWriter = new StreamWriter(DeviceLayoutXMLFile))
				{
					try
					{
						xmlSerializer2.Serialize(streamWriter, XmlDeviceLayout);
					}
					catch (Exception inner3)
					{
						throw new PackageException(inner3, "DeviceLayoutValidation: ValidateDeviceLayout Unable to write Device Layout XML.");
					}
				}
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000404C File Offset: 0x0000224C
		private DeviceLayoutValidator.FileSignatureCertificateType GetFileSignatureCertificateType(string FilePath)
		{
			DeviceLayoutValidator.FileSignatureCertificateType fileSignatureCertificateType = DeviceLayoutValidator.FileSignatureCertificateType.None;
			try
			{
				X509Certificate2 x509Certificate = new X509Certificate2(FilePath);
				if (x509Certificate != null && !string.IsNullOrEmpty(x509Certificate.Subject) && !DeviceLayoutValidator._certPublicKeys.TryGetValue(x509Certificate.Thumbprint, out fileSignatureCertificateType))
				{
					fileSignatureCertificateType = DeviceLayoutValidator.FileSignatureCertificateType.NonMicrosoftCertificate;
					X509Chain x509Chain = new X509Chain(true);
					x509Chain.Build(x509Certificate);
					foreach (X509ChainElement x509ChainElement in x509Chain.ChainElements)
					{
						if (string.Compare("3B1EFD3A66EA28B16697394703A72CA340A05BD5", x509ChainElement.Certificate.Thumbprint, true, CultureInfo.InvariantCulture) == 0)
						{
							fileSignatureCertificateType = DeviceLayoutValidator.FileSignatureCertificateType.MicrosoftProductionCertificate;
							break;
						}
						if (string.Compare("9E594333273339A97051B0F82E86F266B917EDB3", x509ChainElement.Certificate.Thumbprint, true, CultureInfo.InvariantCulture) == 0)
						{
							fileSignatureCertificateType = DeviceLayoutValidator.FileSignatureCertificateType.MicrosoftFlightCertificate;
							break;
						}
						if (string.Compare("5f444a6740b7ca2434c7a5925222c2339ee0f1b7", x509ChainElement.Certificate.Thumbprint, true, CultureInfo.InvariantCulture) == 0)
						{
							fileSignatureCertificateType = DeviceLayoutValidator.FileSignatureCertificateType.MicrosoftFlightCertificate;
							break;
						}
						if (string.Compare("8A334AA8052DD244A647306A76B8178FA215F344", x509ChainElement.Certificate.Thumbprint, true, CultureInfo.InvariantCulture) == 0)
						{
							fileSignatureCertificateType = DeviceLayoutValidator.FileSignatureCertificateType.MicrosoftTestCertificate;
							break;
						}
					}
					foreach (X509ChainElement x509ChainElement2 in x509Chain.ChainElements)
					{
						DeviceLayoutValidator._certPublicKeys[x509ChainElement2.Certificate.Thumbprint] = fileSignatureCertificateType;
					}
				}
			}
			catch
			{
				fileSignatureCertificateType = DeviceLayoutValidator.FileSignatureCertificateType.None;
			}
			return fileSignatureCertificateType;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000041A8 File Offset: 0x000023A8
		private void ValidateDeviceLayoutPackageIsMicrosoftOwned(IPkgInfo ReferencePkg, IPkgInfo Pkg, string PkgName, string PkgPath)
		{
			if (this._oemInput.CPUType.Equals(FeatureManifest.CPUType_ARM, StringComparison.OrdinalIgnoreCase) || this._oemInput.CPUType.Equals(FeatureManifest.CPUType_ARM64, StringComparison.OrdinalIgnoreCase))
			{
				if (Pkg.OwnerType != OwnerType.Microsoft)
				{
					throw new DeviceLayoutValidationException(DeviceLayoutValidationError.DeviceLayoutNotMsOwned, string.Concat(new object[]
					{
						"DeviceLayoutValidation: ValidateDeviceLayoutPackageIsMicrosoftOwned Package Owner '",
						Pkg.OwnerType,
						"' must be a Microsoft owned package.  '",
						PkgPath,
						"' device package is not."
					}));
				}
				if (this._oemInput.ReleaseType.Equals("Production", StringComparison.OrdinalIgnoreCase))
				{
					if (!string.Equals(this._oemInput.BuildType, OEMInput.BuildType_FRE, StringComparison.OrdinalIgnoreCase))
					{
						throw new DeviceLayoutValidationException(DeviceLayoutValidationError.DeviceLayoutNotProductionSigned, "DeviceLayoutValidation: ValidateDeviceLayoutPackageIsMicrosoftOwned: The BuildType '" + this._oemInput.BuildType + "' in the OEM Input file is not valid.  Please use 'fre' for Retail images.");
					}
					if (Pkg.ReleaseType != ReleaseType.Production || Pkg.BuildType != BuildType.Retail)
					{
						throw new DeviceLayoutValidationException(DeviceLayoutValidationError.DeviceLayoutNotProductionSigned, "DeviceLayoutValidation: ValidateDeviceLayoutPackageIsMicrosoftOwned: The Package BuildType must be 'Production' and the package release type must be 'Retail'");
					}
					IFileEntry fileEntry = (from file in Pkg.Files
					where file.FileType == FileType.Catalog
					select file).First<IFileEntry>();
					string text = Path.Combine(this._tempDirectoryPath, "tempref.cat");
					string text2 = Path.Combine(this._tempDirectoryPath, "temp.cat");
					try
					{
						Pkg.ExtractFile(fileEntry.DevicePath, text2, true);
						DeviceLayoutValidator.FileSignatureCertificateType fileSignatureCertificateType = this.GetFileSignatureCertificateType(text2);
						if (fileSignatureCertificateType != DeviceLayoutValidator.FileSignatureCertificateType.MicrosoftProductionCertificate && fileSignatureCertificateType != DeviceLayoutValidator.FileSignatureCertificateType.MicrosoftFlightCertificate)
						{
							if (fileSignatureCertificateType == DeviceLayoutValidator.FileSignatureCertificateType.None || fileSignatureCertificateType == DeviceLayoutValidator.FileSignatureCertificateType.NonMicrosoftCertificate)
							{
								throw new DeviceLayoutValidationException(DeviceLayoutValidationError.DeviceLayoutNotProductionSigned, "DeviceLayoutValidation: ValidateDeviceLayoutPackageIsMicrosoftOwned: The Package '" + PkgName + "' is NOT Microsoft signed");
							}
							this._logger.LogInfo(string.Concat(new string[]
							{
								"DeviceLayoutValidation: ValidateDeviceLayoutPackageIsMicrosoftOwned - '",
								PkgName,
								" 'is signed by ",
								fileSignatureCertificateType.ToString(),
								" - checking that the reference package also is.'"
							}), new object[0]);
							IFileEntry fileEntry2 = (from file in ReferencePkg.Files
							where file.FileType == FileType.Catalog
							select file).First<IFileEntry>();
							ReferencePkg.ExtractFile(fileEntry2.DevicePath, text, true);
							DeviceLayoutValidator.FileSignatureCertificateType fileSignatureCertificateType2 = this.GetFileSignatureCertificateType(text);
							if (fileSignatureCertificateType2 != fileSignatureCertificateType)
							{
								if (fileSignatureCertificateType2 == DeviceLayoutValidator.FileSignatureCertificateType.None || fileSignatureCertificateType2 == DeviceLayoutValidator.FileSignatureCertificateType.NonMicrosoftCertificate)
								{
									throw new DeviceLayoutValidationException(DeviceLayoutValidationError.UnknownInternalError, string.Concat(new string[]
									{
										"DeviceLayoutValidation: ValidateDeviceLayoutPackageIsMicrosoftOwned: Unable to validate if the Package '",
										PkgName,
										"' is Microsoft signed - since the reference package provided has an invalid certificate '",
										fileSignatureCertificateType2.ToString(),
										"'"
									}));
								}
								throw new DeviceLayoutValidationException(DeviceLayoutValidationError.DeviceLayoutValidationManifestNotProductionSigned, string.Concat(new string[]
								{
									"DeviceLayoutValidation: ValidateDeviceLayoutPackageIsMicrosoftOwned: The DeviceLayoutValidationManifest package '",
									PkgName,
									"' signature (",
									fileSignatureCertificateType.ToString(),
									") does not match the signature type of the reference package (",
									fileSignatureCertificateType2.ToString(),
									"."
								}));
							}
						}
						this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutPackageIsMicrosoftOwned - Success Package '" + PkgName + " 'is signed correctly.", new object[0]);
						return;
					}
					finally
					{
						File.Delete(text2);
						File.Delete(text);
					}
				}
				this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutPackageIsMicrosoftOwned - ReleaseType '" + this._oemInput.ReleaseType.ToString() + "' is not production - ignoring certificate check.", new object[0]);
				return;
			}
			else
			{
				this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutPackageIsMicrosoftOwned - CPUType '" + this._oemInput.CPUType.ToString() + "' is not ARM based - ignoring certificate check.", new object[0]);
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004520 File Offset: 0x00002720
		private void ValidateDeviceLayoutPackageIsOEMOwned(IPkgInfo Pkg, string PkgName, string PkgPath)
		{
			if (this._oemInput.CPUType.Equals(FeatureManifest.CPUType_ARM, StringComparison.OrdinalIgnoreCase) || this._oemInput.CPUType.Equals(FeatureManifest.CPUType_ARM64, StringComparison.OrdinalIgnoreCase))
			{
				if (Pkg.OwnerType != OwnerType.OEM)
				{
					this._logger.LogInfo(string.Concat(new string[]
					{
						"DeviceLayoutValidation: ValidateDeviceLayoutPackageIsOEMOwned - Package '",
						PkgName,
						" 'should be contained in a OEM owned package.  '",
						PkgPath,
						"' device package is not."
					}), new object[0]);
				}
				if (this._oemInput.ReleaseType.Equals("Production", StringComparison.OrdinalIgnoreCase))
				{
					if (!string.Equals(this._oemInput.BuildType, OEMInput.BuildType_FRE, StringComparison.OrdinalIgnoreCase))
					{
						throw new DeviceLayoutValidationException(DeviceLayoutValidationError.DeviceLayoutNotProductionSigned, "DeviceLayoutValidation: ValidateDeviceLayoutPackageIsOEMOwned: The BuildType '" + this._oemInput.BuildType + "' in the OEM Input file is not valid.  Please use 'fre' for Retail images.");
					}
					if (Pkg.ReleaseType != ReleaseType.Production || Pkg.BuildType != BuildType.Retail)
					{
						throw new DeviceLayoutValidationException(DeviceLayoutValidationError.DeviceLayoutNotProductionSigned, "DeviceLayoutValidation: ValidateDeviceLayoutPackageIsOEMOwned: The Package BuildType must be 'Production' and the package release type must be 'Retail'");
					}
					IFileEntry fileEntry = (from file in Pkg.Files
					where file.FileType == FileType.Catalog
					select file).First<IFileEntry>();
					string text = Path.Combine(this._tempDirectoryPath, "temp.cat");
					try
					{
						Pkg.ExtractFile(fileEntry.DevicePath, text, true);
						DeviceLayoutValidator.FileSignatureCertificateType fileSignatureCertificateType = this.GetFileSignatureCertificateType(text);
						if (fileSignatureCertificateType == DeviceLayoutValidator.FileSignatureCertificateType.None)
						{
							throw new DeviceLayoutValidationException(DeviceLayoutValidationError.DeviceLayoutNotProductionSigned, "DeviceLayoutValidation: ValidateDeviceLayoutPackageIsOEMOwned: The Package '" + PkgName + "' is not signed");
						}
						if (fileSignatureCertificateType != DeviceLayoutValidator.FileSignatureCertificateType.NonMicrosoftCertificate)
						{
							this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutPackageIsOEMOwned - Package '" + PkgName + " 'is signed by Microsoft but needs to be OEM signed.'", new object[0]);
						}
						return;
					}
					finally
					{
						File.Delete(text);
					}
				}
				this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutPackageIsOEMOwned - ReleaseType '" + this._oemInput.ReleaseType.ToString() + "' is not production - ignoring certificate check.", new object[0]);
				return;
			}
			this._logger.LogInfo("DeviceLayoutValidation: ValidateDeviceLayoutPackageIsOEMOwned - CPUType '" + this._oemInput.CPUType.ToString() + "' is not ARM based - ignoring certificate check.", new object[0]);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004728 File Offset: 0x00002928
		public void ValidateDeviceLayout(IPkgInfo ReferencePkg, IPkgInfo PkgDeviceLayout, string DeviceLayoutPackagePath, string DeviceLayoutXmlFilePath)
		{
			DeviceLayoutInput deviceLayoutInput = null;
			DeviceLayoutInputv2 deviceLayoutInputv = null;
			if (this._oemInput == null)
			{
				throw new ImageCommonException("DeviceLayoutValidation: DeviceLayoutValidator not initialized");
			}
			this.ReadDeviceLayoutXmlFile(DeviceLayoutXmlFilePath, ref deviceLayoutInput, ref deviceLayoutInputv);
			string versionTag;
			if (deviceLayoutInputv != null)
			{
				versionTag = deviceLayoutInputv.VersionTag;
			}
			else
			{
				versionTag = deviceLayoutInput.VersionTag;
			}
			string deviceLayoutValidationFileInScope = this.GetDeviceLayoutValidationFileInScope(versionTag);
			if (string.IsNullOrEmpty(deviceLayoutValidationFileInScope))
			{
				this._logger.LogInfo("DeviceLayoutValidation: SOC '" + this._oemInput.SOC + "' is not opened - reverting to validating Microsoft ownership of DeviceLayout Package", new object[0]);
				this.ValidateDeviceLayoutPackageIsMicrosoftOwned(ReferencePkg, PkgDeviceLayout, "DeviceLayout", DeviceLayoutPackagePath);
				return;
			}
			this._logger.LogInfo(string.Concat(new string[]
			{
				"DeviceLayoutValidation: SOC '",
				this._oemInput.SOC,
				"' is open. Using '",
				deviceLayoutValidationFileInScope,
				"' to validate  DeviceLayout Package"
			}), new object[0]);
			this.ValidateDeviceLayoutPackageIsOEMOwned(PkgDeviceLayout, "DeviceLayout", DeviceLayoutPackagePath);
			if (deviceLayoutInput != null)
			{
				this.ValidateDeviceLayout(deviceLayoutInput, DeviceLayoutXmlFilePath, deviceLayoutValidationFileInScope);
			}
		}

		// Token: 0x04000034 RID: 52
		private const string ValidationAnyValueAllowed = "*ANY*";

		// Token: 0x04000035 RID: 53
		private const string BackupPartitionPlaceHolderName = "BACKUP_*";

		// Token: 0x04000036 RID: 54
		private const string ValidateRuleGetValueFromParent = "*PARENT*";

		// Token: 0x04000037 RID: 55
		private const string ValidateRuleAcceptDefaultPrefix = "allow_default_plus_";

		// Token: 0x04000038 RID: 56
		private const string UpdateTypeValueCritical = "Critical";

		// Token: 0x04000039 RID: 57
		private const string BackupPartitionsBookEndPosition = "BACKUP_BOOKEND";

		// Token: 0x0400003A RID: 58
		private const string BackupPartitionNamePrefix = "BACKUP_";

		// Token: 0x0400003B RID: 59
		private OEMInput _oemInput;

		// Token: 0x0400003C RID: 60
		private IULogger _logger;

		// Token: 0x0400003D RID: 61
		private string _tempDirectoryPath;

		// Token: 0x0400003E RID: 62
		private List<string> _deviceLayoutValidationXmlFiles = new List<string>();

		// Token: 0x0400003F RID: 63
		private const string DeviceLayoutValidationSchema = "DeviceLayoutValidation.xsd";

		// Token: 0x04000040 RID: 64
		private const string DeviceLayout = "DeviceLayout.xml";

		// Token: 0x04000041 RID: 65
		private const string DeviceLayoutValidation = "DeviceLayoutValidation.xml";

		// Token: 0x04000042 RID: 66
		private const string ReleaseTypeProduction = "Production";

		// Token: 0x04000043 RID: 67
		private const string ReleaseTypeTest = "Test";

		// Token: 0x04000044 RID: 68
		private static Dictionary<string, DeviceLayoutValidator.FileSignatureCertificateType> _certPublicKeys = new Dictionary<string, DeviceLayoutValidator.FileSignatureCertificateType>();

		// Token: 0x0200000D RID: 13
		private enum FileSignatureCertificateType
		{
			// Token: 0x04000054 RID: 84
			None,
			// Token: 0x04000055 RID: 85
			NonMicrosoftCertificate,
			// Token: 0x04000056 RID: 86
			MicrosoftTestCertificate,
			// Token: 0x04000057 RID: 87
			MicrosoftFlightCertificate,
			// Token: 0x04000058 RID: 88
			MicrosoftProductionCertificate
		}
	}
}
