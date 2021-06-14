using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.MCSF.Offline;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x02000014 RID: 20
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class Settings : IDefinedIn
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000144 RID: 324 RVA: 0x00007854 File Offset: 0x00005A54
		// (set) Token: 0x06000145 RID: 325 RVA: 0x0000785C File Offset: 0x00005A5C
		[XmlIgnore]
		public string DefinedInFile
		{
			get
			{
				return this._definedInFile;
			}
			set
			{
				foreach (Setting setting in this.Items)
				{
					setting.DefinedInFile = value;
				}
				foreach (Asset asset in this.Assets)
				{
					asset.DefinedInFile = value;
				}
				this._definedInFile = value;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000146 RID: 326 RVA: 0x000078F4 File Offset: 0x00005AF4
		// (set) Token: 0x06000147 RID: 327 RVA: 0x000078FC File Offset: 0x00005AFC
		[XmlAttribute]
		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				this._path = PolicyMacroTable.MacroDollarToTilde(value);
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000148 RID: 328 RVA: 0x0000790A File Offset: 0x00005B0A
		// (set) Token: 0x06000149 RID: 329 RVA: 0x00007912 File Offset: 0x00005B12
		[XmlElement(ElementName = "Setting")]
		public List<Setting> Items { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600014A RID: 330 RVA: 0x0000791B File Offset: 0x00005B1B
		// (set) Token: 0x0600014B RID: 331 RVA: 0x00007923 File Offset: 0x00005B23
		[XmlElement(ElementName = "Asset")]
		public List<Asset> Assets { get; set; }

		// Token: 0x0600014C RID: 332 RVA: 0x0000792C File Offset: 0x00005B2C
		public Settings()
		{
			this.Items = new List<Setting>();
			this.Assets = new List<Asset>();
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000794C File Offset: 0x00005B4C
		public IEnumerable<CustomizationError> MergeSettingGroup(Settings otherSettingGroup, bool allowOverride)
		{
			Settings.<>c__DisplayClass17_1 CS$<>8__locals1 = new Settings.<>c__DisplayClass17_1();
			CS$<>8__locals1.otherSettingGroup = otherSettingGroup;
			List<CustomizationError> list = new List<CustomizationError>();
			if (!this.Path.Equals(CS$<>8__locals1.otherSettingGroup.Path, StringComparison.OrdinalIgnoreCase))
			{
				throw new Exception("Cannot merge two setting groups with different paths!");
			}
			IEnumerable<IGrouping<string, Setting>> source = CS$<>8__locals1.otherSettingGroup.Items.Concat(this.Items).GroupBy((Setting x) => x.Name, StringComparer.OrdinalIgnoreCase);
			using (IEnumerator<IGrouping<string, Setting>> enumerator = (from grp in source
			where grp.Count<Setting>() > 1
			select grp).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IGrouping<string, Setting> dupe = enumerator.Current;
					CustomizationErrorSeverity severity = allowOverride ? CustomizationErrorSeverity.Warning : CustomizationErrorSeverity.Error;
					if (dupe.All((Setting dupeSetting) => dupeSetting.Value.Equals(dupe.First<Setting>().Value)))
					{
						severity = CustomizationErrorSeverity.Warning;
					}
					CustomizationError item = new CustomizationError(severity, dupe, Strings.DuplicateSettings, new object[]
					{
						dupe.Key,
						this.Path
					});
					list.Add(item);
					this.Items.RemoveAll((Setting x) => dupe.Contains(x));
					List<Setting> items = this.Items;
					IEnumerable<Setting> dupe2 = dupe;
					Func<Setting, bool> predicate;
					if ((predicate = CS$<>8__locals1.<>9__4) == null)
					{
						Settings.<>c__DisplayClass17_1 CS$<>8__locals3 = CS$<>8__locals1;
						predicate = (CS$<>8__locals3.<>9__4 = ((Setting x) => CS$<>8__locals3.otherSettingGroup.Items.Contains(x)));
					}
					items.AddRange(dupe2.Where(predicate));
				}
			}
			foreach (IGrouping<string, Setting> grouping in from grp in source
			where grp.Count<Setting>() == 1
			select grp)
			{
				List<Setting> items2 = this.Items;
				IEnumerable<Setting> source2 = grouping;
				Func<Setting, bool> predicate2;
				if ((predicate2 = CS$<>8__locals1.<>9__6) == null)
				{
					Settings.<>c__DisplayClass17_1 CS$<>8__locals4 = CS$<>8__locals1;
					predicate2 = (CS$<>8__locals4.<>9__6 = ((Setting x) => CS$<>8__locals4.otherSettingGroup.Items.Contains(x)));
				}
				items2.AddRange(source2.Where(predicate2));
			}
			IEnumerable<IGrouping<string, Asset>> source3 = CS$<>8__locals1.otherSettingGroup.Assets.Concat(this.Assets).GroupBy((Asset x) => x.Name, StringComparer.OrdinalIgnoreCase);
			foreach (IGrouping<string, Asset> grouping2 in from grp in source3
			where grp.Count<Asset>() > 1
			select grp)
			{
				IEnumerable<IGrouping<string, Asset>> source4 = grouping2.GroupBy((Asset x) => x.Id, StringComparer.OrdinalIgnoreCase);
				using (IEnumerator<IGrouping<string, Asset>> enumerator3 = source4.Where((IGrouping<string, Asset> grp) => grp.Count<Asset>() > 1).GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						IGrouping<string, Asset> dupe = enumerator3.Current;
						CustomizationError item2 = new CustomizationError(allowOverride ? CustomizationErrorSeverity.Warning : CustomizationErrorSeverity.Error, dupe, Strings.DuplicateAssets, new object[]
						{
							grouping2.Key,
							this.Path
						});
						list.Add(item2);
						this.Assets.RemoveAll((Asset x) => dupe.Contains(x));
						this.Assets.AddRange(dupe.Where((Asset x) => x.DefinedInFile.Equals(dupe.First<Asset>().DefinedInFile)));
					}
				}
				foreach (IGrouping<string, Asset> grouping3 in source4.Where((IGrouping<string, Asset> grp) => grp.Count<Asset>() == 1))
				{
					List<Asset> assets = this.Assets;
					IEnumerable<Asset> source5 = grouping3;
					Func<Asset, bool> predicate3;
					if ((predicate3 = CS$<>8__locals1.<>9__14) == null)
					{
						Settings.<>c__DisplayClass17_1 CS$<>8__locals6 = CS$<>8__locals1;
						predicate3 = (CS$<>8__locals6.<>9__14 = ((Asset x) => CS$<>8__locals6.otherSettingGroup.Assets.Contains(x)));
					}
					assets.AddRange(source5.Where(predicate3));
				}
			}
			foreach (IGrouping<string, Asset> grouping4 in from grp in source3
			where grp.Count<Asset>() == 1
			select grp)
			{
				List<Asset> assets2 = this.Assets;
				IEnumerable<Asset> source6 = grouping4;
				Func<Asset, bool> predicate4;
				if ((predicate4 = CS$<>8__locals1.<>9__16) == null)
				{
					Settings.<>c__DisplayClass17_1 CS$<>8__locals7 = CS$<>8__locals1;
					predicate4 = (CS$<>8__locals7.<>9__16 = ((Asset x) => CS$<>8__locals7.otherSettingGroup.Assets.Contains(x)));
				}
				assets2.AddRange(source6.Where(predicate4));
			}
			return list;
		}

		// Token: 0x04000068 RID: 104
		private string _definedInFile;

		// Token: 0x04000069 RID: 105
		private string _path;
	}
}
