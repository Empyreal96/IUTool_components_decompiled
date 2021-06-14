using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000063 RID: 99
	public class FilterGroup : PkgElement
	{
		// Token: 0x060001C6 RID: 454 RVA: 0x00007254 File Offset: 0x00005454
		private string GetExpansionString(SatelliteType type)
		{
			if (type == SatelliteType.Neutral)
			{
				throw new ArgumentException("SatelliteType.Neutral is not valid for parameter 'type' of FilterGroup.GetExpansionString");
			}
			if (this._satteliteType != type)
			{
				return null;
			}
			if (this._expansionValues.Count == 0)
			{
				return "*";
			}
			return string.Format("{0}({1})", (this._restrictionType == RestrictionType.Exclude) ? "!" : string.Empty, string.Join(";", from x in this._expansionValues
			select x.Id));
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x000072E0 File Offset: 0x000054E0
		private void SetExpansionString(SatelliteType type, string value)
		{
			if (type == SatelliteType.Neutral)
			{
				throw new ArgumentException("SatelliteType.Neutral is not a valid value for parameter 'type' of FilterGroup.SetExpansionString");
			}
			if (this._satteliteType != SatelliteType.Neutral)
			{
				throw new PkgGenException("Expansion list can't be set twice", new object[0]);
			}
			if (value == null)
			{
				throw new PkgGenException("Empty expansion string", new object[0]);
			}
			this._satteliteType = type;
			this._expansionValues = new HashSet<SatelliteId>();
			if (value == "*")
			{
				this._restrictionType = RestrictionType.Exclude;
				return;
			}
			Match match = Regex.Match(value, "^(?<name>!?)\\((?<values>.*)\\)$");
			if (!match.Success)
			{
				throw new PkgGenException("Invalid expansion string", new object[0]);
			}
			this._satteliteType = type;
			if (match.Groups["name"].Value.Length != 0)
			{
				this._restrictionType = RestrictionType.Exclude;
			}
			foreach (string text in match.Groups["values"].Value.Split(new char[]
			{
				';'
			}))
			{
				SatelliteId satelliteId = SatelliteId.Create(this._satteliteType, text.Trim());
				if (this._expansionValues.Contains(satelliteId))
				{
					throw new PkgGenException("Duplicate langauge/resolution identifier in expansion list: {0}", new object[]
					{
						satelliteId.Id
					});
				}
				this._expansionValues.Add(satelliteId);
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x00007421 File Offset: 0x00005621
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x0000742A File Offset: 0x0000562A
		[XmlAttribute("Language")]
		public string Language
		{
			get
			{
				return this.GetExpansionString(SatelliteType.Language);
			}
			set
			{
				this.SetExpansionString(SatelliteType.Language, value);
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060001CA RID: 458 RVA: 0x00007434 File Offset: 0x00005634
		// (set) Token: 0x060001CB RID: 459 RVA: 0x0000743D File Offset: 0x0000563D
		[XmlAttribute("Resolution")]
		public string Resolution
		{
			get
			{
				return this.GetExpansionString(SatelliteType.Resolution);
			}
			set
			{
				this.SetExpansionString(SatelliteType.Resolution, value);
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00007447 File Offset: 0x00005647
		public bool ShouldSerializeCpuFilter()
		{
			return this.CpuFilter > CpuId.Invalid;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00007454 File Offset: 0x00005654
		public override void Build(IPackageGenerator pkgGen)
		{
			if (this.CpuFilter != CpuId.Invalid && this.CpuFilter != pkgGen.CPU)
			{
				return;
			}
			if (this._satteliteType != SatelliteType.Neutral)
			{
				IEnumerable<SatelliteId> enumerable = pkgGen.GetSatelliteValues(this._satteliteType);
				if (this._restrictionType == RestrictionType.Exclude)
				{
					enumerable = enumerable.Except(this._expansionValues);
				}
				else
				{
					enumerable = enumerable.Intersect(this._expansionValues);
				}
				using (IEnumerator<SatelliteId> enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SatelliteId satelliteId = enumerator.Current;
						pkgGen.MacroResolver.BeginLocal();
						pkgGen.MacroResolver.Register(satelliteId.MacroName, satelliteId.MacroValue);
						this.Build(pkgGen, satelliteId);
						pkgGen.MacroResolver.EndLocal();
					}
					return;
				}
			}
			this.Build(pkgGen, SatelliteId.Neutral);
		}

		// Token: 0x0400015C RID: 348
		private SatelliteType _satteliteType;

		// Token: 0x0400015D RID: 349
		private RestrictionType _restrictionType;

		// Token: 0x0400015E RID: 350
		private HashSet<SatelliteId> _expansionValues;

		// Token: 0x0400015F RID: 351
		[XmlAttribute("CpuFilter")]
		public CpuId CpuFilter;
	}
}
