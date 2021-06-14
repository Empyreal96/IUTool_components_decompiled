using System;
using System.Collections.Generic;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000024 RID: 36
	internal class MetadataOnlyCustomAttributeData : CustomAttributeData
	{
		// Token: 0x060001D6 RID: 470 RVA: 0x00005145 File Offset: 0x00003345
		public MetadataOnlyCustomAttributeData(MetadataOnlyModule module, Token token, ConstructorInfo ctor)
		{
			this._ctor = ctor;
			this._token = token;
			this._module = module;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x00005164 File Offset: 0x00003364
		public MetadataOnlyCustomAttributeData(ConstructorInfo ctor, IList<CustomAttributeTypedArgument> typedArguments, IList<CustomAttributeNamedArgument> namedArguments)
		{
			this._ctor = ctor;
			this._typedArguments = typedArguments;
			this._namedArguments = namedArguments;
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x00005184 File Offset: 0x00003384
		public override ConstructorInfo Constructor
		{
			get
			{
				return this._ctor;
			}
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000519C File Offset: 0x0000339C
		private void InitArgumentData()
		{
			IList<CustomAttributeTypedArgument> typedArguments;
			IList<CustomAttributeNamedArgument> namedArguments;
			this._module.LazyAttributeParse(this._token, this._ctor, out typedArguments, out namedArguments);
			this._typedArguments = typedArguments;
			this._namedArguments = namedArguments;
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001DA RID: 474 RVA: 0x000051D4 File Offset: 0x000033D4
		public override IList<CustomAttributeTypedArgument> ConstructorArguments
		{
			get
			{
				bool flag = this._typedArguments == null;
				if (flag)
				{
					this.InitArgumentData();
				}
				return this._typedArguments;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060001DB RID: 475 RVA: 0x00005204 File Offset: 0x00003404
		public override IList<CustomAttributeNamedArgument> NamedArguments
		{
			get
			{
				bool flag = this._namedArguments == null;
				if (flag)
				{
					this.InitArgumentData();
				}
				return this._namedArguments;
			}
		}

		// Token: 0x0400005A RID: 90
		private readonly ConstructorInfo _ctor;

		// Token: 0x0400005B RID: 91
		private readonly MetadataOnlyModule _module;

		// Token: 0x0400005C RID: 92
		private readonly Token _token;

		// Token: 0x0400005D RID: 93
		private IList<CustomAttributeTypedArgument> _typedArguments;

		// Token: 0x0400005E RID: 94
		private IList<CustomAttributeNamedArgument> _namedArguments;
	}
}
