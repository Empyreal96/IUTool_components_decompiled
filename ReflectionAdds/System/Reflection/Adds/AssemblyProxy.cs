using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection.Mock;

namespace System.Reflection.Adds
{
	// Token: 0x02000004 RID: 4
	[DebuggerDisplay("AssemblyProxy")]
	internal abstract class AssemblyProxy : Assembly, IAssembly2, IDisposable
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002200 File Offset: 0x00000400
		protected AssemblyProxy(ITypeUniverse universe)
		{
			this.TypeUniverse = universe;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002214 File Offset: 0x00000414
		public Assembly GetResolvedAssembly()
		{
			bool flag = this._assembly == null;
			if (flag)
			{
				this._assembly = this.GetResolvedAssemblyWorker();
				bool flag2 = this._assembly == null;
				if (flag2)
				{
					throw new UnresolvedAssemblyException(string.Format(CultureInfo.InvariantCulture, Resources.UniverseCannotResolveAssembly, new object[]
					{
						this.GetNameWithNoResolution()
					}));
				}
			}
			return this._assembly;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002279 File Offset: 0x00000479
		public ITypeUniverse TypeUniverse { get; }

		// Token: 0x06000010 RID: 16 RVA: 0x00002281 File Offset: 0x00000481
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002294 File Offset: 0x00000494
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				bool flag = this._assembly != null;
				if (flag)
				{
					IDisposable disposable = this._assembly as IDisposable;
					bool flag2 = disposable != null;
					if (flag2)
					{
						disposable.Dispose();
					}
				}
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000022D8 File Offset: 0x000004D8
		public override int GetHashCode()
		{
			return this.GetResolvedAssembly().GetHashCode();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000022F8 File Offset: 0x000004F8
		public override string ToString()
		{
			return this.GetResolvedAssembly().ToString();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002318 File Offset: 0x00000518
		public override bool Equals(object obj)
		{
			return this.GetResolvedAssembly().Equals(obj);
		}

		// Token: 0x06000015 RID: 21
		protected abstract Assembly GetResolvedAssemblyWorker();

		// Token: 0x06000016 RID: 22
		protected abstract AssemblyName GetNameWithNoResolution();

		// Token: 0x06000017 RID: 23 RVA: 0x00002338 File Offset: 0x00000538
		public override AssemblyName GetName()
		{
			return this.GetResolvedAssembly().GetName();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002358 File Offset: 0x00000558
		public override AssemblyName GetName(bool copiedName)
		{
			return this.GetResolvedAssembly().GetName(copiedName);
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002378 File Offset: 0x00000578
		public override string FullName
		{
			get
			{
				return this.GetResolvedAssembly().FullName;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002398 File Offset: 0x00000598
		public override string Location
		{
			get
			{
				return this.GetResolvedAssembly().Location;
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000023B8 File Offset: 0x000005B8
		public override Type[] GetExportedTypes()
		{
			return this.GetResolvedAssembly().GetExportedTypes();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000023D8 File Offset: 0x000005D8
		public override Type[] GetTypes()
		{
			return this.GetResolvedAssembly().GetTypes();
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001D RID: 29 RVA: 0x000023F8 File Offset: 0x000005F8
		public override string CodeBase
		{
			get
			{
				return this.GetResolvedAssembly().CodeBase;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002418 File Offset: 0x00000618
		public override string EscapedCodeBase
		{
			get
			{
				return this.GetResolvedAssembly().EscapedCodeBase;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002438 File Offset: 0x00000638
		public override MethodInfo EntryPoint
		{
			get
			{
				return this.GetResolvedAssembly().EntryPoint;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002458 File Offset: 0x00000658
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.GetResolvedAssembly().GetCustomAttributesData();
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002478 File Offset: 0x00000678
		public override object[] GetCustomAttributes(bool inherit)
		{
			return this.GetResolvedAssembly().GetCustomAttributes(inherit);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002498 File Offset: 0x00000698
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.GetResolvedAssembly().GetCustomAttributes(attributeType, inherit);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000024B8 File Offset: 0x000006B8
		public override Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			return this.GetResolvedAssembly().GetType(name, throwOnError, ignoreCase);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000024D8 File Offset: 0x000006D8
		public override Module GetModule(string name)
		{
			return this.GetResolvedAssembly().GetModule(name);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000024F8 File Offset: 0x000006F8
		public override Module[] GetLoadedModules(bool getResourceModules)
		{
			return this.GetResolvedAssembly().GetLoadedModules(getResourceModules);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002518 File Offset: 0x00000718
		public override Module[] GetModules(bool getResourceModules)
		{
			return this.GetResolvedAssembly().GetModules(getResourceModules);
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002538 File Offset: 0x00000738
		public override Module ManifestModule
		{
			get
			{
				return this.GetResolvedAssembly().ManifestModule;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002558 File Offset: 0x00000758
		public override AssemblyName[] GetReferencedAssemblies()
		{
			return this.GetResolvedAssembly().GetReferencedAssemblies();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002578 File Offset: 0x00000778
		public override Assembly GetSatelliteAssembly(CultureInfo culture)
		{
			return this.GetResolvedAssembly().GetSatelliteAssembly(culture);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002598 File Offset: 0x00000798
		public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
		{
			return this.GetResolvedAssembly().GetSatelliteAssembly(culture, version);
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000025B8 File Offset: 0x000007B8
		public override bool GlobalAssemblyCache
		{
			get
			{
				return this.GetResolvedAssembly().GlobalAssemblyCache;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002C RID: 44 RVA: 0x000025D8 File Offset: 0x000007D8
		public override long HostContext
		{
			get
			{
				return this.GetResolvedAssembly().HostContext;
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000025F8 File Offset: 0x000007F8
		public override Stream GetManifestResourceStream(Type type, string name)
		{
			return this.GetResolvedAssembly().GetManifestResourceStream(type, name);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002618 File Offset: 0x00000818
		public override Stream GetManifestResourceStream(string name)
		{
			return this.GetResolvedAssembly().GetManifestResourceStream(name);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002638 File Offset: 0x00000838
		public override string[] GetManifestResourceNames()
		{
			return this.GetResolvedAssembly().GetManifestResourceNames();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002658 File Offset: 0x00000858
		public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
		{
			return this.GetResolvedAssembly().GetManifestResourceInfo(resourceName);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002678 File Offset: 0x00000878
		public override FileStream[] GetFiles(bool getResourceModules)
		{
			return this.GetResolvedAssembly().GetFiles(getResourceModules);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002698 File Offset: 0x00000898
		public override FileStream[] GetFiles()
		{
			return this.GetResolvedAssembly().GetFiles();
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000026B8 File Offset: 0x000008B8
		public override FileStream GetFile(string name)
		{
			return this.GetResolvedAssembly().GetFile(name);
		}

		// Token: 0x04000002 RID: 2
		private Assembly _assembly;
	}
}
