using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection.Mock;

namespace System.Reflection.Adds
{
	// Token: 0x02000008 RID: 8
	internal class SimpleUniverse : IMutableTypeUniverse, ITypeUniverse, IDisposable
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00002AF8 File Offset: 0x00000CF8
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private bool IsAssemblyInList(Assembly assembly)
		{
			foreach (Assembly assembly2 in this._loadedAssemblies)
			{
				bool flag = assembly2.Equals(assembly);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00002B60 File Offset: 0x00000D60
		public IEnumerable<Assembly> Assemblies
		{
			get
			{
				return this._loadedAssemblies;
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002B78 File Offset: 0x00000D78
		public void AddAssembly(Assembly assembly)
		{
			IAssembly2 assembly2 = (IAssembly2)assembly;
			this._loadedAssemblies.Add(assembly);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002B9C File Offset: 0x00000D9C
		public void SetSystemAssembly(Assembly systemAssembly)
		{
			bool flag = systemAssembly == null;
			if (flag)
			{
				throw new ArgumentNullException("systemAssembly");
			}
			this._systemAssembly = systemAssembly;
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000051 RID: 81 RVA: 0x00002BC8 File Offset: 0x00000DC8
		// (remove) Token: 0x06000052 RID: 82 RVA: 0x00002C00 File Offset: 0x00000E00
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<ResolveAssemblyNameEventArgs> OnResolveEvent;

		// Token: 0x06000053 RID: 83 RVA: 0x00002C38 File Offset: 0x00000E38
		public virtual Type GetBuiltInType(CorElementType elementType)
		{
			string nameForPrimitive = ElementTypeUtility.GetNameForPrimitive(elementType);
			return this.GetTypeXFromName(nameForPrimitive);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002C58 File Offset: 0x00000E58
		public virtual Type GetTypeXFromName(string fullName)
		{
			Type type;
			bool flag = !this._hash.TryGetValue(fullName, out type);
			if (flag)
			{
				type = this.GetSystemAssembly().GetType(fullName, true, false);
				this._hash[fullName] = type;
			}
			return type;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002CA0 File Offset: 0x00000EA0
		public virtual Assembly GetSystemAssembly()
		{
			bool flag = this._systemAssembly == null;
			if (flag)
			{
				Assembly assembly = this.FindSystemAssembly();
				bool flag2 = assembly != null;
				if (flag2)
				{
					this.SetSystemAssembly(assembly);
				}
			}
			bool flag3 = this._systemAssembly == null;
			if (flag3)
			{
				throw new UnresolvedAssemblyException(string.Format(CultureInfo.InvariantCulture, Resources.CannotDetermineSystemAssembly, new object[0]));
			}
			return this._systemAssembly;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002D0C File Offset: 0x00000F0C
		protected Assembly FindSystemAssembly()
		{
			foreach (Assembly assembly in this._loadedAssemblies)
			{
				int num = assembly.GetReferencedAssemblies().Length;
				bool flag = num == 0;
				if (flag)
				{
					return assembly;
				}
			}
			return null;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002D7C File Offset: 0x00000F7C
		public virtual Assembly ResolveAssembly(AssemblyName name)
		{
			Assembly assembly = this.TryResolveAssembly(name);
			bool flag = assembly != null;
			Assembly result;
			if (flag)
			{
				result = assembly;
			}
			else
			{
				bool flag2 = this.OnResolveEvent != null;
				if (flag2)
				{
					ResolveAssemblyNameEventArgs resolveAssemblyNameEventArgs = new ResolveAssemblyNameEventArgs(name);
					this.OnResolveEvent(this, resolveAssemblyNameEventArgs);
					assembly = resolveAssemblyNameEventArgs.Target;
					bool flag3 = assembly != null;
					if (flag3)
					{
					}
				}
				bool flag4 = assembly == null;
				if (flag4)
				{
					throw new UnresolvedAssemblyException(string.Format(CultureInfo.InvariantCulture, Resources.UniverseCannotResolveAssembly, new object[]
					{
						name
					}));
				}
				IAssembly2 assembly2 = assembly as IAssembly2;
				bool flag5 = assembly2 == null;
				if (flag5)
				{
					throw new InvalidOperationException(Resources.ResolverMustResolveToValidAssembly);
				}
				bool flag6 = assembly2.TypeUniverse != this;
				if (flag6)
				{
					throw new InvalidOperationException(Resources.ResolvedAssemblyMustBeWithinSameUniverse);
				}
				result = assembly;
			}
			return result;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002E48 File Offset: 0x00001048
		public virtual Assembly ResolveAssembly(Module scope, Token tokenAssemblyRef)
		{
			IModule2 module = (IModule2)scope;
			AssemblyName assemblyNameFromAssemblyRef = module.GetAssemblyNameFromAssemblyRef(tokenAssemblyRef);
			return this.ResolveAssembly(assemblyNameFromAssemblyRef);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002854 File Offset: 0x00000A54
		public virtual Module ResolveModule(Assembly containingAssembly, string moduleName)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002E70 File Offset: 0x00001070
		protected Assembly TryResolveAssembly(AssemblyName name)
		{
			foreach (Assembly assembly in this._loadedAssemblies)
			{
				AssemblyName name2 = assembly.GetName();
				bool flag = AssemblyName.ReferenceMatchesDefinition(name, name2);
				if (flag)
				{
					return assembly;
				}
			}
			return null;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002EE0 File Offset: 0x000010E0
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002EF4 File Offset: 0x000010F4
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				bool flag = this._loadedAssemblies != null;
				if (flag)
				{
					foreach (Assembly assembly in this._loadedAssemblies)
					{
						IDisposable disposable = assembly as IDisposable;
						bool flag2 = disposable != null;
						if (flag2)
						{
							disposable.Dispose();
						}
					}
					this._loadedAssemblies = null;
				}
			}
		}

		// Token: 0x0400002C RID: 44
		private readonly Dictionary<string, Type> _hash = new Dictionary<string, Type>();

		// Token: 0x0400002D RID: 45
		private List<Assembly> _loadedAssemblies = new List<Assembly>();

		// Token: 0x0400002E RID: 46
		private Assembly _systemAssembly;
	}
}
