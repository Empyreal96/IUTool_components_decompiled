using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x02000012 RID: 18
	[XmlRoot(ElementName = "ImageCustomizations", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class ImageCustomizations : IDefinedIn
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000115 RID: 277 RVA: 0x00006DDC File Offset: 0x00004FDC
		// (set) Token: 0x06000116 RID: 278 RVA: 0x00006DE4 File Offset: 0x00004FE4
		[XmlIgnore]
		public string DefinedInFile { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00006DED File Offset: 0x00004FED
		// (set) Token: 0x06000118 RID: 280 RVA: 0x00006DF5 File Offset: 0x00004FF5
		[XmlAttribute]
		public string Name { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00006DFE File Offset: 0x00004FFE
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00006E06 File Offset: 0x00005006
		[XmlAttribute]
		public string Description { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00006E0F File Offset: 0x0000500F
		// (set) Token: 0x0600011C RID: 284 RVA: 0x00006E17 File Offset: 0x00005017
		[XmlAttribute]
		public string Owner { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600011D RID: 285 RVA: 0x00006E20 File Offset: 0x00005020
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00006E28 File Offset: 0x00005028
		[XmlAttribute]
		public OwnerType OwnerType { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00006E31 File Offset: 0x00005031
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00006E39 File Offset: 0x00005039
		[XmlAttribute]
		public uint Priority { get; set; }

		// Token: 0x06000121 RID: 289 RVA: 0x00006E42 File Offset: 0x00005042
		public bool ShouldSerializePriority()
		{
			return this.Priority > 0U;
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00006E4D File Offset: 0x0000504D
		// (set) Token: 0x06000123 RID: 291 RVA: 0x00006E55 File Offset: 0x00005055
		[XmlArray(ElementName = "Imports")]
		[XmlArrayItem(ElementName = "Import", Type = typeof(Import), IsNullable = false)]
		public List<Import> Imports { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00006E5E File Offset: 0x0000505E
		// (set) Token: 0x06000125 RID: 293 RVA: 0x00006E66 File Offset: 0x00005066
		[XmlArray(ElementName = "Targets")]
		[XmlArrayItem(ElementName = "Target", Type = typeof(Target), IsNullable = false)]
		public List<Target> Targets { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00006E6F File Offset: 0x0000506F
		// (set) Token: 0x06000127 RID: 295 RVA: 0x00006E77 File Offset: 0x00005077
		[XmlElement(ElementName = "Static")]
		public StaticVariant StaticVariant { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00006E80 File Offset: 0x00005080
		// (set) Token: 0x06000129 RID: 297 RVA: 0x00006E88 File Offset: 0x00005088
		[XmlElement(ElementName = "Variant")]
		public List<Variant> Variants { get; set; }

		// Token: 0x0600012A RID: 298 RVA: 0x00006E91 File Offset: 0x00005091
		public ImageCustomizations()
		{
			if (ImageCustomizations.environmentMacros == null)
			{
				ImageCustomizations.ImportEnvironmentToMacros();
			}
			this.Imports = new List<Import>();
			this.Targets = new List<Target>();
			this.StaticVariant = null;
			this.Variants = new List<Variant>();
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00006ED0 File Offset: 0x000050D0
		public static IEnumerable<CustomizationError> ImportEnvironmentToMacros()
		{
			List<CustomizationError> list = new List<CustomizationError>();
			if (ImageCustomizations.environmentMacros != null)
			{
				return list;
			}
			ImageCustomizations.environmentMacros = new MacroResolver();
			foreach (object obj in Environment.GetEnvironmentVariables())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				try
				{
					ImageCustomizations.environmentMacros.Register(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
				}
				catch (PkgGenException ex)
				{
					list.Add(new CustomizationError(CustomizationErrorSeverity.Warning, null, "Macro will be ignored : {0}", new object[]
					{
						ex.Message
					}));
				}
			}
			if (ImageCustomizations.environmentMacros.Unregister(Strings.CurrentFileMacro))
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Warning, null, Strings.CurrentFileDirOverride, new object[]
				{
					Strings.CurrentFileMacro
				}));
			}
			return list;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00006FC4 File Offset: 0x000051C4
		private void RefreshEnvironmentMacros()
		{
			ImageCustomizations.environmentMacros = null;
			ImageCustomizations.ImportEnvironmentToMacros();
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00006FD2 File Offset: 0x000051D2
		public static string ExpandPath(string path)
		{
			return ImageCustomizations.ExpandPath(path, false);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00006FDB File Offset: 0x000051DB
		public static string ExpandPath(string path, bool generateError)
		{
			if (generateError)
			{
				return ImageCustomizations.environmentMacros.Resolve(path, MacroResolveOptions.ErrorOnUnknownMacro);
			}
			return ImageCustomizations.environmentMacros.Resolve(path, MacroResolveOptions.SkipOnUnknownMacro);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00006FFC File Offset: 0x000051FC
		public static ImageCustomizations LoadFromPath(string filePath)
		{
			filePath = Path.GetFullPath(filePath);
			ImageCustomizations result;
			try
			{
				using (TextReader textReader = File.OpenText(filePath))
				{
					ImageCustomizations imageCustomizations = ImageCustomizations.LoadFromReader(textReader);
					imageCustomizations.DefinedInFile = filePath;
					imageCustomizations.LinkChildrenToFile();
					imageCustomizations.SetApplicationVariantType();
					result = imageCustomizations;
				}
			}
			catch (Exception innerException)
			{
				throw new CustomizationException(string.Format("Error with customization file '{0}'", filePath), innerException);
			}
			return result;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00007070 File Offset: 0x00005270
		public Target GetTargetWithId(string targetId)
		{
			return (from x in this.Targets
			where x.Id.Equals(targetId, StringComparison.Ordinal)
			select x).FirstOrNull<Target>();
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000070A6 File Offset: 0x000052A6
		public IEnumerable<Variant> GetVariantsWithTargetId(string targetId)
		{
			Func<TargetRef, bool> <>9__0;
			foreach (Variant variant in this.Variants)
			{
				IEnumerable<TargetRef> targetRefs = variant.TargetRefs;
				Func<TargetRef, bool> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((TargetRef x) => x.Id.Equals(targetId, StringComparison.Ordinal)));
				}
				if (targetRefs.Where(predicate).Count<TargetRef>() > 0)
				{
					yield return variant;
				}
			}
			List<Variant>.Enumerator enumerator = default(List<Variant>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000070C0 File Offset: 0x000052C0
		public void Save(string path)
		{
			path = Path.GetFullPath(path);
			File.Delete(path);
			using (Stream stream = File.OpenWrite(path))
			{
				new XmlSerializer(typeof(ImageCustomizations)).Serialize(stream, this);
				this.DefinedInFile = path;
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000711C File Offset: 0x0000531C
		public ImageCustomizations GetMergedCustomizations(out IEnumerable<CustomizationError> errors)
		{
			List<CustomizationError> list = new List<CustomizationError>();
			List<ImageCustomizations> list2 = new List<ImageCustomizations>();
			list2.Add(this);
			list2.AddRange(this.GetLinkedCustomizations());
			ImageCustomizations imageCustomizations = new ImageCustomizations();
			IEnumerable<IGrouping<uint, ImageCustomizations>> source = from x in list2
			group x by x.Priority;
			foreach (IGrouping<uint, ImageCustomizations> grouping in (from x in source
			orderby x.Key
			select x).Reverse<IGrouping<uint, ImageCustomizations>>())
			{
				if (grouping.Key != 0U)
				{
					ImageCustomizations imageCustomizations2 = new ImageCustomizations();
					foreach (ImageCustomizations customizations in grouping)
					{
						list.AddRange(imageCustomizations2.Merge(customizations));
					}
					list.AddRange(imageCustomizations.Merge(imageCustomizations2, true));
				}
			}
			IGrouping<uint, ImageCustomizations> grouping2 = source.SingleOrDefault((IGrouping<uint, ImageCustomizations> x) => x.Key == 0U);
			if (grouping2 != null)
			{
				foreach (ImageCustomizations customizations2 in grouping2)
				{
					list.AddRange(imageCustomizations.Merge(customizations2));
				}
			}
			imageCustomizations.Name = this.Name;
			imageCustomizations.Description = this.Description;
			imageCustomizations.Owner = this.Owner;
			imageCustomizations.OwnerType = this.OwnerType;
			imageCustomizations.DefinedInFile = this.DefinedInFile;
			errors = list;
			return imageCustomizations;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000072E8 File Offset: 0x000054E8
		private IEnumerable<ImageCustomizations> GetLinkedCustomizations()
		{
			List<ImageCustomizations> list = new List<ImageCustomizations>();
			foreach (Import import in this.Imports)
			{
				ImageCustomizations.environmentMacros.BeginLocal();
				ImageCustomizations imageCustomizations;
				try
				{
					ImageCustomizations.environmentMacros.Register(Strings.CurrentFileMacro, Path.GetDirectoryName(this.DefinedInFile));
					IEnumerable<CustomizationError> source = CustomContentGenerator.VerifyImportSource(import);
					if (source.Any((CustomizationError x) => x.Severity.Equals(CustomizationErrorSeverity.Error)))
					{
						throw new CustomizationException(source.First<CustomizationError>().Message);
					}
					imageCustomizations = ImageCustomizations.LoadFromPath(import.ExpandedSourcePath);
				}
				finally
				{
					ImageCustomizations.environmentMacros.EndLocal();
				}
				list.Add(imageCustomizations);
				list.AddRange(imageCustomizations.GetLinkedCustomizations());
			}
			return list;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000073E0 File Offset: 0x000055E0
		public IEnumerable<CustomizationError> Merge(ImageCustomizations customizations)
		{
			return this.Merge(customizations, false);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x000073EC File Offset: 0x000055EC
		public IEnumerable<CustomizationError> Merge(ImageCustomizations customizations, bool allowOverride)
		{
			if (customizations == null)
			{
				throw new ArgumentNullException("customizations");
			}
			List<CustomizationError> list = new List<CustomizationError>();
			IEnumerable<Target> source = customizations.Targets.Concat(this.Targets);
			foreach (IGrouping<string, Target> grouping in from x in source
			group x by x.Id into grp
			where grp.Count<Target>() > 1
			select grp)
			{
				CustomizationError item = new CustomizationError(allowOverride ? CustomizationErrorSeverity.Warning : CustomizationErrorSeverity.Error, grouping, Strings.DuplicateTargets, new object[]
				{
					grouping.Key
				});
				list.Add(item);
			}
			this.Targets = source.DistinctBy((Target x) => x.Id).ToList<Target>();
			if (this.StaticVariant == null)
			{
				this.StaticVariant = customizations.StaticVariant;
			}
			else if (customizations.StaticVariant != null)
			{
				IEnumerable<CustomizationError> collection = this.StaticVariant.Merge(customizations.StaticVariant, allowOverride);
				list.AddRange(collection);
			}
			foreach (Variant variant in customizations.Variants)
			{
				string id = variant.TargetRefs.First<TargetRef>().Id;
				Variant variant2 = this.GetVariantsWithTargetId(id).FirstOrDefault<Variant>();
				if (variant2 != null)
				{
					IEnumerable<CustomizationError> collection2 = variant2.Merge(variant, allowOverride);
					list.AddRange(collection2);
				}
				else
				{
					this.Variants.Add(variant);
				}
			}
			return list;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x000075B8 File Offset: 0x000057B8
		public bool ShouldSerializeImports()
		{
			return this.Imports.Count > 0;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x000075C8 File Offset: 0x000057C8
		public bool ShouldSerializeTargets()
		{
			return this.Targets.Count > 0;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x000075D8 File Offset: 0x000057D8
		private void LinkChildrenToFile()
		{
			foreach (Target target in this.Targets)
			{
				target.DefinedInFile = this.DefinedInFile;
			}
			if (this.StaticVariant != null)
			{
				this.StaticVariant.LinkToFile(this);
			}
			foreach (Variant variant in this.Variants)
			{
				variant.LinkToFile(this);
			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00007684 File Offset: 0x00005884
		private void SetApplicationVariantType()
		{
			foreach (Application application in this.Variants.SelectMany((Variant x) => x.ApplicationGroups).SelectMany((Applications x) => x.Items))
			{
				application.StaticApp = false;
			}
			if (this.StaticVariant == null)
			{
				return;
			}
			foreach (Application application2 in this.StaticVariant.ApplicationGroups.SelectMany((Applications x) => x.Items))
			{
				application2.StaticApp = true;
			}
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00007784 File Offset: 0x00005984
		private static ImageCustomizations LoadFromReader(TextReader reader)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			executingAssembly.GetManifestResourceNames();
			ImageCustomizations result;
			using (XmlReader xmlReader = XmlReader.Create(executingAssembly.GetManifestResourceStream("Customization.xsd")))
			{
				XmlSchema schema = XmlSchema.Read(xmlReader, null);
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.Schemas.Add(schema);
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				XmlReader xmlReader2 = XmlReader.Create(reader, xmlReaderSettings);
				result = (ImageCustomizations)new XmlSerializer(typeof(ImageCustomizations)).Deserialize(xmlReader2);
			}
			return result;
		}

		// Token: 0x04000060 RID: 96
		public static MacroResolver environmentMacros;
	}
}
