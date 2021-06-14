using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000074 RID: 116
	public abstract class PkgObjectBuilder<T, V> where T : PkgObject, new() where V : PkgObjectBuilder<T, V>
	{
		// Token: 0x06000282 RID: 642 RVA: 0x00009D8C File Offset: 0x00007F8C
		public PkgObjectBuilder()
		{
			this.localMacros = new Dictionary<string, Macro>();
			this.pkgObject = Activator.CreateInstance<T>();
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00009DAA File Offset: 0x00007FAA
		public V RegisterMacro(string name, string value)
		{
			if (!this.localMacros.Keys.Contains(name))
			{
				this.localMacros.Add(name, new Macro(name, value));
			}
			return (V)((object)this);
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00009DD8 File Offset: 0x00007FD8
		public V RegisterMacro(string name, object value, MacroDelegate del)
		{
			if (!this.localMacros.Keys.Contains(name))
			{
				this.localMacros.Add(name, new Macro(name, value, del));
			}
			return (V)((object)this);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00009E08 File Offset: 0x00008008
		public virtual T ToPkgObject()
		{
			this.pkgObject.LocalMacros = new MacroTable();
			this.pkgObject.LocalMacros.Macros.AddRange(this.localMacros.Values);
			return this.pkgObject;
		}

		// Token: 0x040001AE RID: 430
		private Dictionary<string, Macro> localMacros;

		// Token: 0x040001AF RID: 431
		protected T pkgObject;
	}
}
