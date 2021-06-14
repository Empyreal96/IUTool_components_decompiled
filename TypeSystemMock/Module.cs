using System;
using System.Collections.Generic;

namespace System.Reflection.Mock
{
	// Token: 0x02000012 RID: 18
	internal abstract class Module
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00002087 File Offset: 0x00000287
		public virtual string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00002087 File Offset: 0x00000287
		public virtual void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00002087 File Offset: 0x00000287
		public virtual int MDStreamVersion
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000129 RID: 297 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Assembly Assembly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00003900 File Offset: 0x00001B00
		public Type GetType(string className)
		{
			return this.GetType(className, false, false);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000391C File Offset: 0x00001B1C
		public Type GetType(string className, bool ignoreCase)
		{
			return this.GetType(className, false, ignoreCase);
		}

		// Token: 0x0600012C RID: 300
		public abstract Type GetType(string className, bool throwOnError, bool ignoreCase);

		// Token: 0x0600012D RID: 301
		public abstract Type[] GetTypes();

		// Token: 0x0600012E RID: 302
		public abstract Type[] FindTypes(TypeFilter filter, object filterCriteria);

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600012F RID: 303 RVA: 0x00002087 File Offset: 0x00000287
		public virtual string ScopeName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Guid ModuleVersionId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00002087 File Offset: 0x00000287
		public virtual int MetadataToken
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00002087 File Offset: 0x00000287
		public virtual bool IsResource()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00003938 File Offset: 0x00001B38
		public FieldInfo GetField(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00002087 File Offset: 0x00000287
		public virtual FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00003968 File Offset: 0x00001B68
		public FieldInfo[] GetFields()
		{
			return this.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00002087 File Offset: 0x00000287
		public virtual FieldInfo[] GetFields(BindingFlags bindingFlags)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00003984 File Offset: 0x00001B84
		public MethodInfo GetMethod(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, null, null);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x000039B8 File Offset: 0x00001BB8
		public MethodInfo GetMethod(string name, Type[] types)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = types == null;
			if (flag2)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				bool flag3 = types[i] == null;
				if (flag3)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00003A30 File Offset: 0x00001C30
		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = types == null;
			if (flag2)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				bool flag3 = types[i] == null;
				if (flag3)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00002087 File Offset: 0x00000287
		protected virtual MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00003AAC File Offset: 0x00001CAC
		public MethodInfo[] GetMethods()
		{
			return this.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00002087 File Offset: 0x00000287
		public virtual MethodInfo[] GetMethods(BindingFlags bindingFlags)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00003AC8 File Offset: 0x00001CC8
		public MemberInfo ResolveMember(int metadataToken)
		{
			return this.ResolveMember(metadataToken, null, null);
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00002087 File Offset: 0x00000287
		public virtual MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00002087 File Offset: 0x00000287
		public virtual byte[] ResolveSignature(int metadataToken)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00002087 File Offset: 0x00000287
		public virtual string ResolveString(int metadataToken)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00003AE4 File Offset: 0x00001CE4
		public Type ResolveType(int metadataToken)
		{
			return this.ResolveType(metadataToken, null, null);
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00003B00 File Offset: 0x00001D00
		public FieldInfo ResolveField(int metadataToken)
		{
			return this.ResolveField(metadataToken, null, null);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00002087 File Offset: 0x00000287
		public virtual FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00003B1C File Offset: 0x00001D1C
		public MethodBase ResolveMethod(int metadataToken)
		{
			return this.ResolveMethod(metadataToken, null, null);
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00002087 File Offset: 0x00000287
		public virtual MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000147 RID: 327
		public abstract IList<CustomAttributeData> GetCustomAttributesData();

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000148 RID: 328 RVA: 0x0000210A File Offset: 0x0000030A
		public virtual string FullyQualifiedName
		{
			get
			{
				throw new InvalidOperationException();
			}
		}
	}
}
