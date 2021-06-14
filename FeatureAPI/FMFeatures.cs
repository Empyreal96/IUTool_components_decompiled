using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200001E RID: 30
	public class FMFeatures
	{
		// Token: 0x060000AA RID: 170 RVA: 0x000072E4 File Offset: 0x000054E4
		public bool ShouldSerializeMicrosoft()
		{
			return this.Microsoft != null;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000072EF File Offset: 0x000054EF
		public bool ShouldSerializeMSConditionalFeatures()
		{
			return this.MSConditionalFeatures != null && this.MSConditionalFeatures.Count<FMConditionalFeature>() > 0;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00007309 File Offset: 0x00005509
		public bool ShouldSerializeMSFeatureGroups()
		{
			return this.MSFeatureGroups != null && this.MSFeatureGroups.Count<FMFeatureGrouping>() > 0;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00007323 File Offset: 0x00005523
		public bool ShouldSerializeOEM()
		{
			return this.OEM != null && this.OEM.Count != 0;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000733D File Offset: 0x0000553D
		public bool ShouldSerializeOEMConditionalFeatures()
		{
			return this.OEMConditionalFeatures != null && this.OEMConditionalFeatures.Count<FMConditionalFeature>() > 0;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00007357 File Offset: 0x00005557
		public bool ShouldSerializeOEMFeatureGroups()
		{
			return this.OEMFeatureGroups != null && this.OEMFeatureGroups.Count<FMFeatureGrouping>() > 0;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00007374 File Offset: 0x00005574
		public void ValidateConstraints(List<string> MSFeatures, List<string> OEMFeatures)
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			if (this.MSFeatureGroups != null)
			{
				using (List<FMFeatureGrouping>.Enumerator enumerator = this.MSFeatureGroups.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string value;
						if (!enumerator.Current.ValidateConstraints(MSFeatures, out value))
						{
							flag = false;
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("Errors in Microsoft Features:");
							stringBuilder.AppendLine(value);
						}
					}
				}
			}
			if (this.OEMFeatureGroups != null)
			{
				using (List<FMFeatureGrouping>.Enumerator enumerator = this.OEMFeatureGroups.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string value2;
						if (!enumerator.Current.ValidateConstraints(OEMFeatures, out value2))
						{
							flag = false;
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("Errors in OEM Features:");
							stringBuilder.AppendLine(value2);
						}
					}
				}
			}
			if (!flag)
			{
				throw new FeatureAPIException("FeatureAPI!ValidateConstraints: OEMInput file contains invalid Feature combinations:" + stringBuilder.ToString());
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x0000747C File Offset: 0x0000567C
		public void Merge(FMFeatures srcFeatures)
		{
			if (srcFeatures.MSConditionalFeatures != null)
			{
				if (this.MSConditionalFeatures == null)
				{
					this.MSConditionalFeatures = srcFeatures.MSConditionalFeatures;
				}
				else
				{
					this.MSConditionalFeatures.AddRange(srcFeatures.MSConditionalFeatures);
				}
			}
			if (srcFeatures.MSFeatureGroups != null)
			{
				if (this.MSFeatureGroups == null)
				{
					this.MSFeatureGroups = srcFeatures.MSFeatureGroups;
				}
				else
				{
					this.MSFeatureGroups.AddRange(srcFeatures.MSFeatureGroups);
				}
			}
			if (srcFeatures.Microsoft != null)
			{
				if (this.Microsoft == null)
				{
					this.Microsoft = srcFeatures.Microsoft;
				}
				else
				{
					this.Microsoft.AddRange(srcFeatures.Microsoft);
				}
			}
			if (srcFeatures.OEMConditionalFeatures != null)
			{
				if (this.OEMConditionalFeatures == null)
				{
					this.OEMConditionalFeatures = srcFeatures.OEMConditionalFeatures;
				}
				else
				{
					this.OEMConditionalFeatures.AddRange(srcFeatures.OEMConditionalFeatures);
				}
			}
			if (srcFeatures.OEMFeatureGroups != null)
			{
				if (this.OEMFeatureGroups == null)
				{
					this.OEMFeatureGroups = srcFeatures.OEMFeatureGroups;
				}
				else
				{
					this.OEMFeatureGroups.AddRange(srcFeatures.OEMFeatureGroups);
				}
			}
			if (srcFeatures.OEM != null)
			{
				if (this.OEM == null)
				{
					this.OEM = srcFeatures.OEM;
					return;
				}
				this.OEM.AddRange(srcFeatures.OEM);
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000075A4 File Offset: 0x000057A4
		public static string GetFeatureIDWithoutPrefix(string featureID)
		{
			string text = featureID;
			if (text.StartsWith("MS_", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring("MS_".Length);
			}
			if (text.StartsWith("OEM_", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring("OEM_".Length);
			}
			return text;
		}

		// Token: 0x04000092 RID: 146
		public const string MSFeaturePrefix = "MS_";

		// Token: 0x04000093 RID: 147
		public const string OEMFeaturePrefix = "OEM_";

		// Token: 0x04000094 RID: 148
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(MSOptionalPkgFile), IsNullable = false)]
		[XmlArray]
		public List<MSOptionalPkgFile> Microsoft;

		// Token: 0x04000095 RID: 149
		[XmlArrayItem(ElementName = "ConditionalFeature", Type = typeof(FMConditionalFeature), IsNullable = false)]
		[XmlArray]
		public List<FMConditionalFeature> MSConditionalFeatures;

		// Token: 0x04000096 RID: 150
		[XmlArrayItem(ElementName = "FeatureGroup", Type = typeof(FMFeatureGrouping), IsNullable = false)]
		[XmlArray]
		public List<FMFeatureGrouping> MSFeatureGroups;

		// Token: 0x04000097 RID: 151
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(OEMOptionalPkgFile), IsNullable = false)]
		[XmlArray]
		public List<OEMOptionalPkgFile> OEM;

		// Token: 0x04000098 RID: 152
		[XmlArrayItem(ElementName = "ConditionalFeature", Type = typeof(FMConditionalFeature), IsNullable = false)]
		[XmlArray]
		public List<FMConditionalFeature> OEMConditionalFeatures;

		// Token: 0x04000099 RID: 153
		[XmlArrayItem(ElementName = "FeatureGroup", Type = typeof(FMFeatureGrouping), IsNullable = false)]
		[XmlArray]
		public List<FMFeatureGrouping> OEMFeatureGroups;
	}
}
