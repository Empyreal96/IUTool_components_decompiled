using System;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000032 RID: 50
	internal class ProxyGenericType : TypeProxy
	{
		// Token: 0x06000385 RID: 901 RVA: 0x0000C036 File Offset: 0x0000A236
		public ProxyGenericType(TypeProxy rawType, Type[] args) : base(rawType.Resolver)
		{
			this._rawType = rawType;
			this._args = args;
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0000C054 File Offset: 0x0000A254
		protected override Type GetResolvedTypeWorker()
		{
			return this._rawType.GetResolvedType().MakeGenericType(this._args);
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0000C07C File Offset: 0x0000A27C
		public override Type[] GetGenericArguments()
		{
			return (Type[])this._args.Clone();
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0000C0A0 File Offset: 0x0000A2A0
		public override Type GetGenericTypeDefinition()
		{
			return this._rawType;
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000389 RID: 905 RVA: 0x0000C0B8 File Offset: 0x0000A2B8
		public override string Name
		{
			get
			{
				return this._rawType.Name;
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x0600038A RID: 906 RVA: 0x0000C0D8 File Offset: 0x0000A2D8
		public override string Namespace
		{
			get
			{
				return this._rawType.Namespace;
			}
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0000C0F8 File Offset: 0x0000A2F8
		protected override bool IsPointerImpl()
		{
			return false;
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0000C10C File Offset: 0x0000A30C
		protected override bool IsArrayImpl()
		{
			return false;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0000C120 File Offset: 0x0000A320
		protected override bool IsByRefImpl()
		{
			return false;
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600038E RID: 910 RVA: 0x0000C134 File Offset: 0x0000A334
		public override Type DeclaringType
		{
			get
			{
				return this._rawType.DeclaringType;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x0600038F RID: 911 RVA: 0x0000C154 File Offset: 0x0000A354
		public override bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000390 RID: 912 RVA: 0x0000C168 File Offset: 0x0000A368
		public override bool IsGenericType
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000391 RID: 913 RVA: 0x0000C17C File Offset: 0x0000A37C
		public override bool IsEnum
		{
			get
			{
				return this._rawType.IsEnum;
			}
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0000C19C File Offset: 0x0000A39C
		protected override bool IsValueTypeImpl()
		{
			return this._rawType.IsValueType;
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000393 RID: 915 RVA: 0x0000C1BC File Offset: 0x0000A3BC
		public override Module Module
		{
			get
			{
				return this._rawType.Module;
			}
		}

		// Token: 0x040000BA RID: 186
		private readonly TypeProxy _rawType;

		// Token: 0x040000BB RID: 187
		private readonly Type[] _args;
	}
}
