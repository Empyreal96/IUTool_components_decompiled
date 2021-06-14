using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x0200001E RID: 30
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class Variant : IDefinedIn
	{
		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00008108 File Offset: 0x00006308
		// (set) Token: 0x06000186 RID: 390 RVA: 0x00008110 File Offset: 0x00006310
		[XmlIgnore]
		public string DefinedInFile { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000187 RID: 391 RVA: 0x00008119 File Offset: 0x00006319
		// (set) Token: 0x06000188 RID: 392 RVA: 0x00008121 File Offset: 0x00006321
		[XmlAttribute]
		public virtual string Name { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000189 RID: 393 RVA: 0x0000812A File Offset: 0x0000632A
		// (set) Token: 0x0600018A RID: 394 RVA: 0x00008132 File Offset: 0x00006332
		public virtual List<TargetRef> TargetRefs
		{
			get
			{
				return this._targetRefs;
			}
			set
			{
				this._targetRefs = value;
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x0600018B RID: 395 RVA: 0x0000813B File Offset: 0x0000633B
		// (set) Token: 0x0600018C RID: 396 RVA: 0x00008143 File Offset: 0x00006343
		[XmlElement(ElementName = "Settings")]
		public List<Settings> SettingGroups { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x0600018D RID: 397 RVA: 0x0000814C File Offset: 0x0000634C
		// (set) Token: 0x0600018E RID: 398 RVA: 0x00008154 File Offset: 0x00006354
		[XmlElement(ElementName = "Applications")]
		public List<Applications> ApplicationGroups { get; set; }

		// Token: 0x0600018F RID: 399 RVA: 0x0000815D File Offset: 0x0000635D
		public Variant()
		{
			this.ApplicationGroups = new List<Applications>();
			this.SettingGroups = new List<Settings>();
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00008186 File Offset: 0x00006386
		public bool ShouldSerializeTargetRefs()
		{
			return this.TargetRefs.Count > 0;
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00008196 File Offset: 0x00006396
		public virtual bool ShouldSerializeName()
		{
			return true;
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000819C File Offset: 0x0000639C
		public virtual void LinkToFile(IDefinedIn file)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			this.DefinedInFile = file.DefinedInFile;
			foreach (TargetRef targetRef in this.TargetRefs)
			{
				targetRef.DefinedInFile = file.DefinedInFile;
			}
			foreach (Application application in this.ApplicationGroups.SelectMany((Applications x) => x.Items))
			{
				((IDefinedIn)application).DefinedInFile = file.DefinedInFile;
			}
			foreach (Settings settings in this.SettingGroups)
			{
				settings.DefinedInFile = file.DefinedInFile;
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x000082B4 File Offset: 0x000064B4
		public IEnumerable<CustomizationError> Merge(Variant otherVariant, bool allowOverride)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (otherVariant == null)
			{
				throw new ArgumentNullException("otherVariant");
			}
			IEnumerable<CustomizationError> collection = this.MergeApplicationGroups(otherVariant.ApplicationGroups, allowOverride);
			list.AddRange(collection);
			collection = this.MergeSettingGroups(otherVariant.SettingGroups, allowOverride);
			list.AddRange(collection);
			if (otherVariant.TargetRefs.Count != this.TargetRefs.Count)
			{
				List<IDefinedIn> filesInvolved = new List<IDefinedIn>
				{
					this,
					otherVariant
				};
				CustomizationError item = new CustomizationError(CustomizationErrorSeverity.Error, filesInvolved, Strings.MismatchedTargets, new object[]
				{
					this.Name,
					otherVariant.Name
				});
				list.Add(item);
			}
			IEnumerable<TargetRef> enumerable = from x in this.TargetRefs.Concat(otherVariant.TargetRefs)
			group x by x.Id into x
			where x.Count<TargetRef>() == 1
			select x.First<TargetRef>();
			if (enumerable.Count<TargetRef>() > 0)
			{
				CustomizationError item2 = new CustomizationError(CustomizationErrorSeverity.Error, enumerable, Strings.MismatchedTargets, new object[]
				{
					this.Name,
					otherVariant.Name
				});
				list.Add(item2);
			}
			if (allowOverride)
			{
				this.Name = otherVariant.Name;
			}
			return list;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00008420 File Offset: 0x00006620
		private IEnumerable<CustomizationError> MergeApplicationGroups(IEnumerable<Applications> otherApps, bool allowOverride)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			IEnumerable<Application> source = otherApps.Concat(this.ApplicationGroups).SelectMany((Applications x) => x.Items);
			foreach (IGrouping<string, Application> grouping in from x in source
			group x by x.ExpandedSourcePath into grp
			where grp.Count<Application>() > 1
			select grp)
			{
				CustomizationError item = new CustomizationError(allowOverride ? CustomizationErrorSeverity.Warning : CustomizationErrorSeverity.Error, grouping, Strings.DuplicateApplications, new object[]
				{
					grouping.Key
				});
				list.Add(item);
			}
			Applications applications = new Applications();
			applications.Items = source.DistinctBy((Application x) => x.Source).ToList<Application>();
			this.ApplicationGroups = new List<Applications>
			{
				applications
			};
			return list;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00008558 File Offset: 0x00006758
		private IEnumerable<CustomizationError> MergeSettingGroups(List<Settings> otherSettings, bool allowOverride)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if ((from x in otherSettings
			group x by x.Path).Any((IGrouping<string, Settings> grp) => grp.Count<Settings>() > 1))
			{
				otherSettings = this.CollapseSettingGroup(otherSettings);
			}
			if ((from x in this.SettingGroups
			group x by x.Path).Any((IGrouping<string, Settings> grp) => grp.Count<Settings>() > 1))
			{
				this.SettingGroups = this.CollapseSettingGroup(this.SettingGroups);
			}
			foreach (Settings settings in otherSettings)
			{
				string settingPath = settings.Path;
				Settings settings2 = this.SettingGroups.SingleOrDefault((Settings x) => x.Path.Equals(settingPath, StringComparison.OrdinalIgnoreCase));
				if (settings2 == null)
				{
					this.SettingGroups.Add(settings);
				}
				else
				{
					IEnumerable<CustomizationError> collection = settings2.MergeSettingGroup(settings, allowOverride);
					list.AddRange(collection);
				}
			}
			return list;
		}

		// Token: 0x06000196 RID: 406 RVA: 0x000086AC File Offset: 0x000068AC
		private List<Settings> CollapseSettingGroup(List<Settings> settingGroup)
		{
			List<Settings> list = new List<Settings>();
			foreach (IGrouping<string, Settings> source in from x in settingGroup
			group x by x.Path into grp
			where grp.Count<Settings>() > 1
			select grp)
			{
				IEnumerable<Setting> collection = source.SelectMany((Settings x) => x.Items);
				IEnumerable<Asset> collection2 = source.SelectMany((Settings x) => x.Assets);
				Settings settings = new Settings();
				settings.DefinedInFile = source.First<Settings>().DefinedInFile;
				settings.Path = source.First<Settings>().Path;
				settings.Items.AddRange(collection);
				settings.Assets.AddRange(collection2);
				list.Add(settings);
			}
			list.AddRange(from x in settingGroup
			group x by x.Path into grp
			where grp.Count<Settings>() == 1
			select grp into x
			select x.First<Settings>());
			return list;
		}

		// Token: 0x0400008E RID: 142
		[XmlArray(ElementName = "TargetRefs")]
		[XmlArrayItem(ElementName = "TargetRef", Type = typeof(TargetRef), IsNullable = false)]
		private List<TargetRef> _targetRefs = new List<TargetRef>();
	}
}
