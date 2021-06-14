using System;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000036 RID: 54
	public abstract class AppResourceBuilder<T, V> : OSComponentBuilder<T, V> where T : AppResourcePkgObject, new() where V : AppResourceBuilder<T, V>
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x00005212 File Offset: 0x00003412
		public AppResourceBuilder()
		{
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000521A File Offset: 0x0000341A
		public V SetName(string name)
		{
			this.pkgObject.Name = name;
			return (V)((object)this);
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00005233 File Offset: 0x00003433
		public V SetSuite(string suite)
		{
			this.pkgObject.Suite = suite;
			return (V)((object)this);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000524C File Offset: 0x0000344C
		public override T ToPkgObject()
		{
			base.RegisterMacro("runtime.default", "$(runtime.apps)\\" + this.pkgObject.Name);
			base.RegisterMacro("env.default", "$(env.apps)\\" + this.pkgObject.Name);
			return base.ToPkgObject();
		}
	}
}
