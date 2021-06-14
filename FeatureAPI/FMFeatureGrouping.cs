using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200001F RID: 31
	public class FMFeatureGrouping
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x000075F2 File Offset: 0x000057F2
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x000075FC File Offset: 0x000057FC
		[XmlAttribute]
		[DefaultValue(null)]
		public string FMID
		{
			get
			{
				return this._fmID;
			}
			set
			{
				this._fmID = value;
				if (this.SubGroups != null)
				{
					foreach (FMFeatureGrouping fmfeatureGrouping in this.SubGroups)
					{
						fmfeatureGrouping.FMID = value;
					}
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x0000765C File Offset: 0x0000585C
		[XmlIgnore]
		public List<string> FeatureIDWithFMIDs
		{
			get
			{
				List<string> list = new List<string>();
				foreach (string featureID in this.FeatureIDs)
				{
					list.Add(FeatureManifest.GetFeatureIDWithFMID(featureID, this.FMID));
				}
				return list;
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000076C4 File Offset: 0x000058C4
		public bool ShouldSerializeSubGroups()
		{
			return this.SubGroups != null && this.SubGroups.Count != 0;
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x000076E0 File Offset: 0x000058E0
		[XmlIgnore]
		public List<string> AllFeatureIDWithFMIDs
		{
			get
			{
				List<string> list = new List<string>();
				if (this.FeatureIDs != null)
				{
					list.AddRange(this.FeatureIDWithFMIDs);
				}
				if (this.SubGroups != null)
				{
					foreach (FMFeatureGrouping fmfeatureGrouping in this.SubGroups)
					{
						list.AddRange(fmfeatureGrouping.AllFeatureIDWithFMIDs);
					}
				}
				return list;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x0000775C File Offset: 0x0000595C
		[XmlIgnore]
		public List<string> AllFeatureIDs
		{
			get
			{
				List<string> list = new List<string>();
				if (this.FeatureIDs != null)
				{
					list.AddRange(this.FeatureIDs);
				}
				if (this.SubGroups != null)
				{
					foreach (FMFeatureGrouping fmfeatureGrouping in this.SubGroups)
					{
						list.AddRange(fmfeatureGrouping.AllFeatureIDs);
					}
				}
				return list;
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000077D8 File Offset: 0x000059D8
		public bool ValidateConstraints(IEnumerable<string> FeatureIDs, out string errorMessage)
		{
			bool result = true;
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = FeatureIDs.Intersect(this.AllFeatureIDs, this.IgnoreCase).ToList<string>();
			int num = list.Count<string>();
			switch (this.Constraint)
			{
			case FMFeatureGrouping.FeatureConstraints.None:
				goto IL_1A5;
			case FMFeatureGrouping.FeatureConstraints.OneOrMore:
				break;
			case FMFeatureGrouping.FeatureConstraints.ZeroOrOne:
				goto IL_153;
			case FMFeatureGrouping.FeatureConstraints.OneAndOnlyOne:
				if (num == 1)
				{
					goto IL_1A5;
				}
				result = false;
				if (num == 0)
				{
					stringBuilder.AppendLine("One of the following features must be selected:");
					using (List<string>.Enumerator enumerator = this.AllFeatureIDs.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string feature = enumerator.Current;
							stringBuilder.AppendFormat("\t{0}\n", this.GetFixedFeatureConstraintErrorStr(feature));
						}
						goto IL_1A5;
					}
				}
				stringBuilder.AppendLine("Only one of the following features may be selected:");
				using (List<string>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string feature2 = enumerator.Current;
						stringBuilder.AppendFormat("\t{0}\n", this.GetFixedFeatureConstraintErrorStr(feature2));
					}
					goto IL_1A5;
				}
				break;
			default:
				goto IL_1A5;
			}
			if (num != 0)
			{
				goto IL_1A5;
			}
			result = false;
			stringBuilder.AppendLine("One or more of the following features must be selected:");
			using (List<string>.Enumerator enumerator = this.AllFeatureIDs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string feature3 = enumerator.Current;
					stringBuilder.AppendFormat("\t{0}\n", this.GetFixedFeatureConstraintErrorStr(feature3));
				}
				goto IL_1A5;
			}
			IL_153:
			if (num > 1)
			{
				result = false;
				stringBuilder.AppendLine("Only one (or none) of the following features may be selected:");
				foreach (string feature4 in list)
				{
					stringBuilder.AppendFormat("\t{0}\n", this.GetFixedFeatureConstraintErrorStr(feature4));
				}
			}
			IL_1A5:
			errorMessage = stringBuilder.ToString();
			return result;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000079C8 File Offset: 0x00005BC8
		private string GetFixedFeatureConstraintErrorStr(string feature)
		{
			string str = " (under Features\\Microsoft)";
			string str2 = " (under Features\\OEM)";
			string result;
			if (feature.StartsWith("MS_", StringComparison.OrdinalIgnoreCase))
			{
				result = feature.Replace("MS_", "", StringComparison.OrdinalIgnoreCase) + str;
			}
			else if (feature.StartsWith("OEM_", StringComparison.OrdinalIgnoreCase))
			{
				result = feature.Replace("OEM_", "", StringComparison.OrdinalIgnoreCase) + str2;
			}
			else
			{
				result = feature;
			}
			return result;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00007A38 File Offset: 0x00005C38
		public override string ToString()
		{
			string text = "";
			if (!string.IsNullOrEmpty(this.Name))
			{
				text = this.Name + " ";
			}
			return string.Concat(new object[]
			{
				text,
				"(Constraint= ",
				this.Constraint,
				")"
			});
		}

		// Token: 0x0400009A RID: 154
		private StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;

		// Token: 0x0400009B RID: 155
		[XmlAttribute("Name")]
		public string Name;

		// Token: 0x0400009C RID: 156
		[XmlAttribute("PublishingFeatureGroup")]
		[DefaultValue(false)]
		public bool PublishingFeatureGroup;

		// Token: 0x0400009D RID: 157
		private string _fmID;

		// Token: 0x0400009E RID: 158
		[XmlAttribute("Constraint")]
		[DefaultValue(FMFeatureGrouping.FeatureConstraints.None)]
		public FMFeatureGrouping.FeatureConstraints Constraint;

		// Token: 0x0400009F RID: 159
		[XmlAttribute("GroupingType")]
		public string GroupingType;

		// Token: 0x040000A0 RID: 160
		[XmlArrayItem(ElementName = "FeatureID", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public List<string> FeatureIDs;

		// Token: 0x040000A1 RID: 161
		[XmlArrayItem(ElementName = "FeatureGroup", Type = typeof(FMFeatureGrouping), IsNullable = false)]
		[XmlArray]
		public List<FMFeatureGrouping> SubGroups;

		// Token: 0x0200003A RID: 58
		public enum FeatureConstraints
		{
			// Token: 0x0400012C RID: 300
			None,
			// Token: 0x0400012D RID: 301
			OneOrMore,
			// Token: 0x0400012E RID: 302
			ZeroOrOne,
			// Token: 0x0400012F RID: 303
			OneAndOnlyOne
		}
	}
}
