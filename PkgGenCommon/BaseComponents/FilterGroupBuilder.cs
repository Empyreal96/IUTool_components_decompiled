using System;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000040 RID: 64
	public abstract class FilterGroupBuilder<T, V> where T : FilterGroup, new() where V : FilterGroupBuilder<T, V>
	{
		// Token: 0x060000F7 RID: 247 RVA: 0x00005754 File Offset: 0x00003954
		public FilterGroupBuilder()
		{
			this.filterGroup = Activator.CreateInstance<T>();
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00005767 File Offset: 0x00003967
		public V SetCpuId(string value)
		{
			return this.SetCpuId(CpuIdParser.Parse(value));
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00005775 File Offset: 0x00003975
		public V SetCpuId(CpuId value)
		{
			this.filterGroup.CpuFilter = value;
			return (V)((object)this);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000578E File Offset: 0x0000398E
		public V SetResolution(string value)
		{
			this.filterGroup.Resolution = value;
			return (V)((object)this);
		}

		// Token: 0x060000FB RID: 251 RVA: 0x000057A7 File Offset: 0x000039A7
		public V SetLanguage(string value)
		{
			this.filterGroup.Language = value;
			return (V)((object)this);
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000057C0 File Offset: 0x000039C0
		public virtual T ToPkgObject()
		{
			return this.filterGroup;
		}

		// Token: 0x040000EB RID: 235
		protected T filterGroup;
	}
}
