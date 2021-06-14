using System;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200000B RID: 11
	internal class ConstructorInfoRef : ConstructorInfoProxy
	{
		// Token: 0x06000040 RID: 64 RVA: 0x000028D0 File Offset: 0x00000AD0
		public ConstructorInfoRef(Type declaringType, MetadataOnlyModule scope, Token token)
		{
			this.DeclaringType = declaringType;
			this._token = token;
			this._scope = scope;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000028F0 File Offset: 0x00000AF0
		protected override ConstructorInfo GetResolvedWorker()
		{
			MethodBase methodBase = this._scope.ResolveMethod(this._token);
			return (ConstructorInfo)methodBase;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000042 RID: 66 RVA: 0x0000291F File Offset: 0x00000B1F
		public override Type DeclaringType { get; }

		// Token: 0x04000006 RID: 6
		private readonly Token _token;

		// Token: 0x04000007 RID: 7
		private readonly MetadataOnlyModule _scope;
	}
}
