using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsPhone.ImageUpdate.Customization.XML;
using Microsoft.WindowsPhone.MCSF.Offline;
using Microsoft.WindowsPhone.Multivariant.Offline;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization
{
	// Token: 0x0200000B RID: 11
	internal static class Extensions
	{
		// Token: 0x060000A0 RID: 160 RVA: 0x00005B3D File Offset: 0x00003D3D
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> knownKeys = new HashSet<TKey>();
			foreach (TSource tsource in source)
			{
				if (knownKeys.Add(keySelector(tsource)))
				{
					yield return tsource;
				}
			}
			IEnumerator<TSource> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005B54 File Offset: 0x00003D54
		public static TSource FirstOrNull<TSource>(this IEnumerable<TSource> source) where TSource : class
		{
			if (source.Count<TSource>() <= 0)
			{
				return default(TSource);
			}
			return source.First<TSource>();
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00005B7A File Offset: 0x00003D7A
		public static IEnumerable<TSource> ToEnumerable<TSource>(this TSource source)
		{
			yield return source;
			yield break;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005B8C File Offset: 0x00003D8C
		public static string DestinationPath(this CustomizationDataAssetType source)
		{
			switch (source)
			{
			case CustomizationDataAssetType.MapData:
				return "SharedData\\MapData";
			case CustomizationDataAssetType.RetailDemo_Microsoft:
				return "SharedData\\RetailDemo\\OfflineContent\\Microsoft";
			case CustomizationDataAssetType.RetailDemo_OEM:
				return "SharedData\\RetailDemo\\OfflineContent\\OEM";
			case CustomizationDataAssetType.RetailDemo_MO:
				return "SharedData\\RetailDemo\\OfflineContent\\MO";
			case CustomizationDataAssetType.RetailDemo_Apps:
				return "SharedData\\RetailDemo\\OfflineContent\\Apps";
			default:
				return "";
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00005BD8 File Offset: 0x00003DD8
		public static MVSetting ToVariantSetting(this Asset asset, PolicyAssetInfo assetInfo, IEnumerable<string> provisionPath, MVSettingProvisioning provisionTime, PolicyMacroTable settingMacro)
		{
			string text = asset.GetDevicePath(assetInfo.TargetDir);
			string text2 = settingMacro.ReplaceMacros((asset.Type == CustomizationAssetOwner.OEM) ? assetInfo.OemRegKey : assetInfo.MORegKey);
			if (assetInfo.HasOEMMacros)
			{
				PolicyMacroTable policyMacroTable = new PolicyMacroTable(assetInfo.Name, asset.Name);
				text = policyMacroTable.ReplaceMacros(assetInfo.TargetDir);
				text2 = policyMacroTable.ReplaceMacros(text2);
			}
			string text3 = assetInfo.FileNameOnly ? Path.GetFileName(text) : text;
			return new MVSetting(provisionPath.Concat(text3.ToEnumerable<string>()), text2, text3, "REG_SZ")
			{
				Value = (asset.DisplayName ?? string.Empty),
				ProvisioningTime = provisionTime
			};
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00005C84 File Offset: 0x00003E84
		public static MVSetting ToVariantSetting(this PolicyAssetInfo assetInfo, IEnumerable<Asset> assets, IEnumerable<string> provisionPath, MVSettingProvisioning provisionTime, PolicyMacroTable settingMacro)
		{
			List<string> list;
			if (assetInfo.HasOEMMacros)
			{
				list = (from x in assets
				select x.GetDevicePathWithMacros(assetInfo)).ToList<string>();
				list = (from x in list
				select settingMacro.ReplaceMacros(x)).ToList<string>();
			}
			else
			{
				list = (from x in assets
				select settingMacro.ReplaceMacros(x.GetDevicePath(assetInfo.TargetDir))).ToList<string>();
			}
			if (assetInfo.HasOEMMacros)
			{
				list = (from x in list
				select new PolicyMacroTable(assetInfo.Name, x).ReplaceMacros(x)).ToList<string>();
			}
			return new MVSetting(provisionPath, settingMacro.ReplaceMacros(assetInfo.OemRegKey), settingMacro.ReplaceMacros(assetInfo.OemRegValue), "REG_MULTI_SZ")
			{
				Value = string.Join(new string(';', 1), list),
				ProvisioningTime = provisionTime
			};
		}
	}
}
