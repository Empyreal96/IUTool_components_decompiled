using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x0200001F RID: 31
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class StaticVariant : Variant
	{
		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000197 RID: 407 RVA: 0x00008850 File Offset: 0x00006A50
		[XmlIgnore]
		public override string Name
		{
			get
			{
				return "Static";
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000198 RID: 408 RVA: 0x00008857 File Offset: 0x00006A57
		[XmlIgnore]
		public override List<TargetRef> TargetRefs
		{
			get
			{
				return new List<TargetRef>();
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000199 RID: 409 RVA: 0x0000885E File Offset: 0x00006A5E
		// (set) Token: 0x0600019A RID: 410 RVA: 0x00008866 File Offset: 0x00006A66
		[XmlElement(ElementName = "DataAssets")]
		public List<DataAssets> DataAssetGroups { get; set; }

		// Token: 0x0600019B RID: 411 RVA: 0x0000886F File Offset: 0x00006A6F
		public StaticVariant()
		{
			this.DataAssetGroups = new List<DataAssets>();
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00008882 File Offset: 0x00006A82
		public override bool ShouldSerializeName()
		{
			return false;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00008888 File Offset: 0x00006A88
		public override void LinkToFile(IDefinedIn file)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			base.LinkToFile(file);
			foreach (DataAsset dataAsset in this.DataAssetGroups.SelectMany((DataAssets x) => x.Items))
			{
				((IDefinedIn)dataAsset).DefinedInFile = file.DefinedInFile;
			}
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00008914 File Offset: 0x00006B14
		public IEnumerable<CustomizationError> Merge(StaticVariant otherVariant, bool allowOverride)
		{
			if (otherVariant == null)
			{
				throw new ArgumentNullException("otherVariant");
			}
			List<CustomizationError> list = new List<CustomizationError>();
			IEnumerable<CustomizationError> collection = base.Merge(otherVariant, allowOverride);
			list.AddRange(collection);
			IEnumerable<CustomizationError> collection2 = this.MergeDataAssets(otherVariant.DataAssetGroups, allowOverride);
			list.AddRange(collection2);
			return list;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000895C File Offset: 0x00006B5C
		private IEnumerable<CustomizationError> MergeDataAssets(IEnumerable<DataAssets> otherAssets, bool allowOverride)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			IEnumerable<IGrouping<CustomizationDataAssetType, DataAsset>> enumerable = from x in otherAssets.Concat(this.DataAssetGroups)
			from item in x.Items
			select new
			{
				Key = x.Type,
				Value = item
			} into x
			group x.Value by x.Key;
			List<DataAssets> list2 = new List<DataAssets>();
			foreach (IGrouping<CustomizationDataAssetType, DataAsset> grouping in enumerable)
			{
				foreach (IGrouping<string, DataAsset> grouping2 in from x in grouping
				group x by x.ExpandedSourcePath into grp
				where grp.Count<DataAsset>() > 1
				select grp)
				{
					CustomizationError item2 = new CustomizationError(allowOverride ? CustomizationErrorSeverity.Warning : CustomizationErrorSeverity.Error, grouping2, Strings.DuplicateApplications, new object[]
					{
						grouping2.Key
					});
					list.Add(item2);
				}
				DataAssets dataAssets = new DataAssets(grouping.Key);
				dataAssets.Items = grouping.DistinctBy((DataAsset x) => x.ExpandedSourcePath).ToList<DataAsset>();
				list2.Add(dataAssets);
			}
			this.DataAssetGroups = list2;
			return list;
		}
	}
}
