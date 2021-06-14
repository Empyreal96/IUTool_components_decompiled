using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Multivariant.Offline
{
	// Token: 0x02000008 RID: 8
	public class MVDatastore
	{
		// Token: 0x06000021 RID: 33 RVA: 0x00002934 File Offset: 0x00000B34
		public static IEnumerable<RegValueInfo> GetDefaultDatastoreRegistration(bool provisionCab, bool criticalCab)
		{
			yield return new RegValueInfo
			{
				Type = RegValueType.String,
				KeyName = "$(hklm.software)\\Microsoft\\Multivariant",
				ValueName = "DatastoreRoot",
				Value = Path.Combine("\\Programs\\CommonFiles\\ADC\\Microsoft\\", "MasterDatastore.xml")
			};
			yield return new RegValueInfo
			{
				Type = RegValueType.DWord,
				KeyName = "$(hklm.software)\\Microsoft\\Multivariant",
				ValueName = "Enable",
				Value = "1"
			};
			yield return new RegValueInfo
			{
				Type = RegValueType.DWord,
				KeyName = "$(hklm.software)\\Microsoft\\ADC",
				ValueName = "RunADC",
				Value = "0"
			};
			if (provisionCab)
			{
				yield return new RegValueInfo
				{
					Type = RegValueType.String,
					KeyName = "$(hklm.software)\\Microsoft\\Multivariant",
					ValueName = "ProvisionDataCABPath",
					Value = Path.Combine("\\Programs\\CommonFiles\\ADC\\Microsoft\\", "ProvisionData.cab")
				};
			}
			if (criticalCab)
			{
				yield return new RegValueInfo
				{
					Type = RegValueType.String,
					KeyName = "$(hklm.software)\\Microsoft\\Multivariant",
					ValueName = "ProvisionDataCriticalSettingsCABPath",
					Value = Path.Combine("\\Programs\\CommonFiles\\ADC\\Microsoft\\", "ProvisionDataCriticalSettings.cab")
				};
			}
			yield break;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000022 RID: 34 RVA: 0x0000294B File Offset: 0x00000B4B
		// (set) Token: 0x06000023 RID: 35 RVA: 0x00002953 File Offset: 0x00000B53
		public List<MVVariant> Variants { get; private set; }

		// Token: 0x06000024 RID: 36 RVA: 0x0000295C File Offset: 0x00000B5C
		public MVDatastore()
		{
			this.Variants = new List<MVVariant>();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000296F File Offset: 0x00000B6F
		private static bool Is_DeviceStatic(MVSetting setting)
		{
			return setting.ProvisioningTime == MVSettingProvisioning.Static && string.IsNullOrWhiteSpace(setting.RegistryKey);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002986 File Offset: 0x00000B86
		private static bool Is_UiccConnectivity(MVCondition condition)
		{
			return condition.IsValidCondition();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000298E File Offset: 0x00000B8E
		private static bool Is_UiccConnectivity(MVSetting setting)
		{
			return setting.ProvisioningTime == MVSettingProvisioning.Connectivity;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002986 File Offset: 0x00000B86
		private static bool Is_UiccGeneral(MVCondition condition)
		{
			return condition.IsValidCondition();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002999 File Offset: 0x00000B99
		private static bool Is_UiccGeneral(MVSetting setting)
		{
			return setting.ProvisioningTime == MVSettingProvisioning.General;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002986 File Offset: 0x00000B86
		private static bool Is_UiccRunOnce(MVCondition condition)
		{
			return condition.IsValidCondition();
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000029A4 File Offset: 0x00000BA4
		private static bool Is_UiccRunOnce(MVSetting setting)
		{
			return setting.ProvisioningTime == MVSettingProvisioning.RunOnce;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000029AF File Offset: 0x00000BAF
		public void SaveStaticDatastore(string tempStaticDatastoreRoot)
		{
			Directory.CreateDirectory(tempStaticDatastoreRoot);
			this.WriteStaticSettings(tempStaticDatastoreRoot, (MVSetting setting) => MVDatastore.Is_DeviceStatic(setting));
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000029E0 File Offset: 0x00000BE0
		public void SaveDatastore(string tempDatastoreRoot, string tempProvisioningOutputRoot, string tempCriticalDatastoreRoot)
		{
			Directory.CreateDirectory(tempDatastoreRoot);
			Directory.CreateDirectory(tempProvisioningOutputRoot);
			Directory.CreateDirectory(tempCriticalDatastoreRoot);
			IEnumerable<KeyValuePair<VariantEvent, string>> sources = this.WriteDataStoreByEvent(tempProvisioningOutputRoot, tempCriticalDatastoreRoot);
			string outputPath = Path.Combine(tempDatastoreRoot, "MasterDatastore.xml");
			this.WriteSourceListXml(sources, outputPath, "$(_adc)\\Microsoft\\");
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002A24 File Offset: 0x00000C24
		public IEnumerable<RegFilePartitionInfo> SaveShadowRegistry(string shadowRegRoot, IEnumerable<RegValueInfo> datastoreRegEntries)
		{
			if (!Directory.Exists(shadowRegRoot))
			{
				Directory.CreateDirectory(shadowRegRoot);
			}
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PkgGen.cfg.xml");
			RegFileHandler regFileHandler = new RegFileHandler(shadowRegRoot, manifestResourceStream);
			IEnumerable<RegValueInfo> staticRegistryValues = this.GetStaticRegistryValues();
			foreach (RegValueInfo regValueInfo in datastoreRegEntries.Concat(staticRegistryValues))
			{
				regFileHandler.AddRegValue(regValueInfo);
			}
			return regFileHandler.Write();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002AA8 File Offset: 0x00000CA8
		private IEnumerable<KeyValuePair<VariantEvent, string>> WriteDataStoreByEvent(string tempProvisioningOutputRoot, string criticalTempDatastoreRoot)
		{
			yield return this.WriteApplicationProvisioning(tempProvisioningOutputRoot);
			yield return this.WriteVariantSettings(VariantEvent.Uicc_Connectivity, criticalTempDatastoreRoot, (MVCondition cond) => MVDatastore.Is_UiccConnectivity(cond), (MVSetting setting) => MVDatastore.Is_UiccConnectivity(setting), "$(_provisionCriticalSettingsCab)\\");
			yield return this.WriteVariantSettings(VariantEvent.Uicc_RunOnce, tempProvisioningOutputRoot, (MVCondition cond) => MVDatastore.Is_UiccRunOnce(cond), (MVSetting setting) => MVDatastore.Is_UiccRunOnce(setting), "$(_provisionCab)\\");
			yield return this.WriteVariantSettings(VariantEvent.Uicc_General, tempProvisioningOutputRoot, (MVCondition cond) => MVDatastore.Is_UiccGeneral(cond), (MVSetting setting) => MVDatastore.Is_UiccGeneral(setting), "$(_provisionCab)\\");
			yield break;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002AC8 File Offset: 0x00000CC8
		private KeyValuePair<VariantEvent, string> WriteStaticSettings(string tempDatastoreRoot, Func<MVSetting, bool> settingPredicate)
		{
			MVDatastore.<>c__DisplayClass40_0 CS$<>8__locals1 = new MVDatastore.<>c__DisplayClass40_0();
			CS$<>8__locals1.settingPredicate = settingPredicate;
			if (!this.Variants.Any<MVVariant>())
			{
				return new KeyValuePair<VariantEvent, string>(VariantEvent.Device_Static, null);
			}
			int num = 0;
			foreach (MVSettingGroup mvsettingGroup in this.Variants.SingleOrDefault<MVVariant>().SettingsGroups)
			{
				IEnumerable<MVSetting> settings = mvsettingGroup.Settings;
				Func<MVSetting, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__0) == null)
				{
					MVDatastore.<>c__DisplayClass40_0 CS$<>8__locals2 = CS$<>8__locals1;
					predicate = (CS$<>8__locals2.<>9__0 = ((MVSetting x) => CS$<>8__locals2.settingPredicate(x)));
				}
				IEnumerable<MVSetting> enumerable = settings.Where(predicate);
				if (enumerable.Count<MVSetting>() != 0)
				{
					string arg = this.formatPathForFileName(mvsettingGroup.Path);
					string path = string.Format("{0}_{1}_{2}.provxml", "static_settings_group", arg, num);
					num++;
					string outputPath = Path.Combine(tempDatastoreRoot, path);
					this.WriteProvXml(enumerable, outputPath);
				}
			}
			return new KeyValuePair<VariantEvent, string>(VariantEvent.Device_Static, null);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002BC4 File Offset: 0x00000DC4
		private KeyValuePair<VariantEvent, string> WriteVariantSettings(VariantEvent variantEvent, string tempDatastoreRoot, Func<MVCondition, bool> conditionPredicate, Func<MVSetting, bool> settingPredicate, string cabstore)
		{
			MVDatastore.<>c__DisplayClass41_0 CS$<>8__locals1 = new MVDatastore.<>c__DisplayClass41_0();
			CS$<>8__locals1.conditionPredicate = conditionPredicate;
			CS$<>8__locals1.settingPredicate = settingPredicate;
			int num = 0;
			List<MVDatastore.ProvXmlInfo> list = new List<MVDatastore.ProvXmlInfo>();
			foreach (MVVariant mvvariant in this.Variants)
			{
				IEnumerable<MVCondition> conditions = mvvariant.Conditions;
				Func<MVCondition, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__0) == null)
				{
					MVDatastore.<>c__DisplayClass41_0 CS$<>8__locals2 = CS$<>8__locals1;
					predicate = (CS$<>8__locals2.<>9__0 = ((MVCondition x) => CS$<>8__locals2.conditionPredicate(x)));
				}
				IEnumerable<MVCondition> source = conditions.Where(predicate);
				if (source.Count<MVCondition>() != 0)
				{
					using (IEnumerator<IGrouping<string, MVSettingGroup>> enumerator2 = (from x in mvvariant.SettingsGroups
					group x by x.PolicyPath).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							MVDatastore.<>c__DisplayClass41_1 CS$<>8__locals3 = new MVDatastore.<>c__DisplayClass41_1();
							CS$<>8__locals3.group = enumerator2.Current;
							IEnumerable<MVSetting> source2 = CS$<>8__locals3.group.SelectMany((MVSettingGroup x) => x.Settings);
							Func<MVSetting, bool> predicate2;
							if ((predicate2 = CS$<>8__locals1.<>9__3) == null)
							{
								MVDatastore.<>c__DisplayClass41_0 CS$<>8__locals5 = CS$<>8__locals1;
								predicate2 = (CS$<>8__locals5.<>9__3 = ((MVSetting x) => CS$<>8__locals5.settingPredicate(x)));
							}
							IEnumerable<MVSetting> enumerable = source2.Where(predicate2);
							if (enumerable.Count<MVSetting>() != 0)
							{
								string arg = this.formatPathForFileName(CS$<>8__locals3.group.Key);
								string path = string.Format("{0}_{1}_{2}.provxml", variantEvent.ToString(), arg, num);
								num++;
								string provxmlPath = Path.Combine(tempDatastoreRoot, path);
								this.WriteProvXml(enumerable, provxmlPath);
								list.AddRange(from x in source
								select new MVDatastore.ProvXmlInfo(x, CS$<>8__locals3.@group.Key, null, provxmlPath));
							}
						}
					}
				}
			}
			if (list.Count == 0)
			{
				return new KeyValuePair<VariantEvent, string>(variantEvent, null);
			}
			string text = Path.Combine(tempDatastoreRoot, variantEvent.ToString() + ".xml");
			this.WriteSourceXml(list, variantEvent.ToString(), text, cabstore);
			return new KeyValuePair<VariantEvent, string>(variantEvent, text);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002E30 File Offset: 0x00001030
		private string formatPathForFileName(string original)
		{
			string text = original;
			foreach (char c in text)
			{
				if (c > '~' || !char.IsLetterOrDigit(c))
				{
					text = text.Replace(c.ToString(), "");
				}
			}
			if (text.Length > 40)
			{
				text = text.Substring(0, 40);
			}
			return text;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002E8F File Offset: 0x0000108F
		private IEnumerable<MVDatastore.AppInfo> ApplicationsCartesian()
		{
			using (List<MVVariant>.Enumerator enumerator = this.Variants.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MVVariant variant = enumerator.Current;
					IEnumerable<MVDatastore.AppInfo> enumerable = variant.Conditions.SelectMany((MVCondition x) => from y in variant.Applications
					select new MVDatastore.AppInfo(x, y.Key, y.Value));
					foreach (MVDatastore.AppInfo appInfo in enumerable)
					{
						yield return appInfo;
					}
					IEnumerator<MVDatastore.AppInfo> enumerator2 = null;
				}
			}
			List<MVVariant>.Enumerator enumerator = default(List<MVVariant>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002EA0 File Offset: 0x000010A0
		private KeyValuePair<VariantEvent, string> WriteApplicationProvisioning(string tempDatastoreRoot)
		{
			if (!this.Variants.SelectMany((MVVariant x) => x.Conditions).Any<MVCondition>())
			{
				return new KeyValuePair<VariantEvent, string>(VariantEvent.Uicc_Apps, null);
			}
			IEnumerable<IGrouping<XElement, MVDatastore.AppInfo>> enumerable = from x in this.ApplicationsCartesian()
			group x by x.provXML;
			if (enumerable.Count<IGrouping<XElement, MVDatastore.AppInfo>>() == 0)
			{
				return new KeyValuePair<VariantEvent, string>(VariantEvent.Uicc_Apps, null);
			}
			List<MVDatastore.ProvXmlInfo> list = new List<MVDatastore.ProvXmlInfo>();
			foreach (IGrouping<XElement, MVDatastore.AppInfo> grouping in enumerable)
			{
				XElement key = grouping.Key;
				string text = Guid.NewGuid().ToString() + ".provxml";
				string text2 = Path.Combine(tempDatastoreRoot, text);
				if (!File.Exists(text2))
				{
					key.Save(text2);
				}
				foreach (MVCondition condition in from x in grouping
				select x.condition)
				{
					list.Add(new MVDatastore.ProvXmlInfo(condition, null, grouping.First<MVDatastore.AppInfo>().productID.ToString(), text));
				}
			}
			string text3 = Path.Combine(tempDatastoreRoot, "Applications.xml");
			this.WriteSourceXml(list, "Applications", text3, "$(_provisionCab)\\");
			return new KeyValuePair<VariantEvent, string>(VariantEvent.Uicc_Apps, text3);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003054 File Offset: 0x00001254
		private void WriteSourceListXml(IEnumerable<KeyValuePair<VariantEvent, string>> sources, string outputPath, string devicePath)
		{
			XElement xelement = new XElement("ConfigurationSourceList");
			xelement.Add(new XAttribute("Version", "1.0"));
			foreach (KeyValuePair<VariantEvent, string> keyValuePair in sources)
			{
				if (keyValuePair.Value != null)
				{
					XElement xelement2 = new XElement("ConfigurationSource");
					xelement2.Add(new XAttribute("Name", Path.GetFileNameWithoutExtension(keyValuePair.Value)));
					xelement2.Add(new XAttribute("Path", Path.Combine(devicePath, Path.GetFileName(keyValuePair.Value))));
					XElement xelement3 = new XElement("EventList");
					XElement xelement4 = new XElement("Event");
					xelement4.Add(new XAttribute("Name", keyValuePair.Key.ToString()));
					xelement3.Add(xelement4);
					xelement2.Add(xelement3);
					xelement.Add(xelement2);
				}
			}
			xelement.Save(outputPath);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003198 File Offset: 0x00001398
		private void WriteSourceXml(IEnumerable<MVDatastore.ProvXmlInfo> provXmls, string eventName, string outputPath, string devicePath)
		{
			XElement xelement = new XElement("ConfigurationSource");
			xelement.Add(new XAttribute("Version", "1.0"));
			foreach (MVDatastore.ProvXmlInfo provXmlInfo in provXmls)
			{
				XElement xelement2 = new XElement("ConfigurationSet");
				xelement2.Add(new XAttribute("Type", "provxml"));
				if (provXmlInfo.settingsGroupPath != null)
				{
					xelement2.Add(new XAttribute("SettingsGroup", provXmlInfo.settingsGroupPath));
				}
				if (provXmlInfo.productID != null)
				{
					xelement2.Add(new XAttribute("ProductID", provXmlInfo.productID));
				}
				xelement2.Add(new XAttribute("Data", Path.Combine(devicePath, Path.GetFileName(provXmlInfo.provXmlPath))));
				if (provXmlInfo.condition != null)
				{
					MVCondition condition = provXmlInfo.condition;
					foreach (string text in condition.KeyValues.Keys)
					{
						if (!text.Equals("UIOrder", StringComparison.OrdinalIgnoreCase) || eventName.Equals(VariantEvent.Uicc_Connectivity.ToString()))
						{
							if (condition.KeyValues[text].IsWildCard)
							{
								xelement2.Add(new XAttribute(text.ToLower(CultureInfo.InvariantCulture), "*"));
							}
							else
							{
								xelement2.Add(new XAttribute(text.ToLower(CultureInfo.InvariantCulture), condition.KeyValues[text].KeyValue));
							}
						}
					}
				}
				xelement.Add(xelement2);
			}
			xelement.Save(outputPath);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000033B0 File Offset: 0x000015B0
		private void WriteProvXml(IEnumerable<MVSetting> settings, string outputPath)
		{
			XElement xelement = new XElement("wap-provisioningdoc");
			foreach (MVSetting mvsetting in settings)
			{
				if (mvsetting.DataType.Equals("multiplestring"))
				{
					MVSetting mvsetting2 = mvsetting;
					mvsetting2.Value = mvsetting2.Value.Replace(";", "&#xF000;");
					string value = "&#xF000;&#xF000;";
					while (!mvsetting.Value.EndsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						mvsetting.Value += "&#xF000;";
					}
				}
				XElement xelement2 = new XElement("parm");
				xelement2.Add(new XAttribute("name", mvsetting.ProvisioningPath.Last<string>()));
				xelement2.Add(new XAttribute("value", mvsetting.Value));
				xelement2.Add(new XAttribute("datatype", mvsetting.DataType));
				List<string> list = mvsetting.ProvisioningPath.ToList<string>();
				list.RemoveAt(list.Count<string>() - 1);
				XElement xelement3 = xelement;
				foreach (string text in list)
				{
					XElement xelement4 = xelement3.XPathSelectElement(string.Format("characteristic[@type=\"{0}\"]", text));
					if (xelement4 == null)
					{
						xelement4 = new XElement("characteristic");
						xelement4.Add(new XAttribute("type", text));
						xelement3.Add(xelement4);
					}
					xelement3 = xelement4;
				}
				xelement3.Add(xelement2);
			}
			new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[]
			{
				xelement
			}).Save(outputPath);
			string text2 = File.ReadAllText(outputPath);
			text2 = text2.Replace("&amp;#xF000;", "&#xF000;");
			File.WriteAllText(outputPath, text2);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000035E0 File Offset: 0x000017E0
		private IEnumerable<RegValueInfo> GetStaticRegistryValues()
		{
			if (!this.Variants.Any<MVVariant>())
			{
				yield break;
			}
			IEnumerable<MVSetting> enumerable = from x in this.Variants.Single<MVVariant>().SettingsGroups.SelectMany((MVSettingGroup x) => x.Settings)
			where !string.IsNullOrWhiteSpace(x.RegistryKey)
			select x;
			foreach (MVSetting mvsetting in enumerable)
			{
				RegValueInfo regValueInfo = new RegValueInfo();
				regValueInfo.SetRegValueType(mvsetting.RegType);
				regValueInfo.KeyName = mvsetting.RegistryKey;
				regValueInfo.ValueName = mvsetting.RegistryValue;
				regValueInfo.Partition = mvsetting.Partition;
				if (regValueInfo.Type == RegValueType.MultiString)
				{
					regValueInfo.Value = mvsetting.Value.TrimEnd(new char[]
					{
						''
					}).Replace('', ';');
				}
				else if (regValueInfo.Type == RegValueType.DWord)
				{
					regValueInfo.Value = int.Parse(mvsetting.Value).ToString("X8");
				}
				else
				{
					regValueInfo.Value = mvsetting.Value;
				}
				yield return regValueInfo;
			}
			IEnumerator<MVSetting> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x04000013 RID: 19
		private const string c_strPkgGenCfgXmlName = "PkgGen.cfg.xml";

		// Token: 0x04000014 RID: 20
		public const string MasterFileName = "MasterDatastore.xml";

		// Token: 0x04000015 RID: 21
		public const string DefaultDatastoreRoot = "\\Programs\\CommonFiles\\ADC\\Microsoft\\";

		// Token: 0x04000016 RID: 22
		public const string DatastoreRegKey = "$(hklm.software)\\Microsoft\\Multivariant";

		// Token: 0x04000017 RID: 23
		public const string ADCRegKey = "$(hklm.software)\\Microsoft\\ADC";

		// Token: 0x04000018 RID: 24
		public const string DatastoreCabFileName = "ProvisionData.cab";

		// Token: 0x04000019 RID: 25
		public const string CriticalSettingsCabFileName = "ProvisionDataCriticalSettings.cab";

		// Token: 0x0400001A RID: 26
		public const string StaticProvXMLPrefix = "static_settings_group";

		// Token: 0x0400001B RID: 27
		public const string MxipUpdatePrefix = "mxipupdate";

		// Token: 0x0400001C RID: 28
		public const string DefaultDatastoreRootWithMacro = "$(_adc)\\Microsoft\\";

		// Token: 0x0400001D RID: 29
		public const string DefaultCabstoreRootWithMacro = "$(_provisionCab)\\";

		// Token: 0x0400001E RID: 30
		public const string DefaultCriticalCabstoreRootWithMacro = "$(_provisionCriticalSettingsCab)\\";

		// Token: 0x0400001F RID: 31
		public const string StaticDatastoreRoot = "\\Programs\\PhoneProvisioner_OEM\\OEM\\";

		// Token: 0x04000020 RID: 32
		public const string MxipUpdateRoot = "\\Windows\\System32\\Migrators\\DuMigrationProvisionerOEM\\provxml";

		// Token: 0x04000021 RID: 33
		public const char MultiStringSeparator = '';

		// Token: 0x04000022 RID: 34
		public const string MVMultiStringSeparator = "&#xF000;";

		// Token: 0x04000023 RID: 35
		private const string ProvXmlExtension = ".provxml";

		// Token: 0x04000024 RID: 36
		private const string ApplicationSourceName = "Applications";

		// Token: 0x04000025 RID: 37
		private const string ApplicationSourceFile = "Applications.xml";

		// Token: 0x04000026 RID: 38
		private const string DatastoreVersion = "1.0";

		// Token: 0x04000027 RID: 39
		private const string ProvisioningDocumentRootName = "wap-provisioningdoc";

		// Token: 0x02000011 RID: 17
		private class ProvXmlInfo
		{
			// Token: 0x1700001B RID: 27
			// (get) Token: 0x0600006B RID: 107 RVA: 0x00003A15 File Offset: 0x00001C15
			public MVCondition condition
			{
				get
				{
					return this.t.Item1;
				}
			}

			// Token: 0x1700001C RID: 28
			// (get) Token: 0x0600006C RID: 108 RVA: 0x00003A22 File Offset: 0x00001C22
			public string settingsGroupPath
			{
				get
				{
					return this.t.Item2;
				}
			}

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x0600006D RID: 109 RVA: 0x00003A2F File Offset: 0x00001C2F
			public string productID
			{
				get
				{
					return this.t.Item3;
				}
			}

			// Token: 0x1700001E RID: 30
			// (get) Token: 0x0600006E RID: 110 RVA: 0x00003A3C File Offset: 0x00001C3C
			public string provXmlPath
			{
				get
				{
					return this.t.Item4;
				}
			}

			// Token: 0x0600006F RID: 111 RVA: 0x00003A49 File Offset: 0x00001C49
			public ProvXmlInfo(MVCondition condition, string settingsGroupPath, string productID, string provXmlPath)
			{
				this.t = new Tuple<MVCondition, string, string, string>(condition, settingsGroupPath, productID, provXmlPath);
			}

			// Token: 0x0400004A RID: 74
			private Tuple<MVCondition, string, string, string> t;
		}

		// Token: 0x02000012 RID: 18
		private class AppInfo
		{
			// Token: 0x1700001F RID: 31
			// (get) Token: 0x06000070 RID: 112 RVA: 0x00003A61 File Offset: 0x00001C61
			public MVCondition condition
			{
				get
				{
					return this.t.Item1;
				}
			}

			// Token: 0x17000020 RID: 32
			// (get) Token: 0x06000071 RID: 113 RVA: 0x00003A6E File Offset: 0x00001C6E
			public Guid productID
			{
				get
				{
					return this.t.Item2;
				}
			}

			// Token: 0x17000021 RID: 33
			// (get) Token: 0x06000072 RID: 114 RVA: 0x00003A7B File Offset: 0x00001C7B
			public XElement provXML
			{
				get
				{
					return this.t.Item3;
				}
			}

			// Token: 0x06000073 RID: 115 RVA: 0x00003A88 File Offset: 0x00001C88
			public AppInfo(MVCondition condition, Guid productID, XElement provXML)
			{
				this.t = new Tuple<MVCondition, Guid, XElement>(condition, productID, provXML);
			}

			// Token: 0x0400004B RID: 75
			private Tuple<MVCondition, Guid, XElement> t;
		}
	}
}
