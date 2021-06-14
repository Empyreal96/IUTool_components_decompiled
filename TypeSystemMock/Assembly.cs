using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace System.Reflection.Mock
{
	// Token: 0x02000002 RID: 2
	internal abstract class Assembly
	{
		// Token: 0x06000001 RID: 1
		public abstract AssemblyName GetName();

		// Token: 0x06000002 RID: 2
		public abstract AssemblyName GetName(bool copiedName);

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3
		public abstract string FullName { get; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4
		public abstract string Location { get; }

		// Token: 0x06000005 RID: 5
		public abstract Type[] GetExportedTypes();

		// Token: 0x06000006 RID: 6 RVA: 0x00002050 File Offset: 0x00000250
		public virtual Type GetType(string name)
		{
			return this.GetType(name, false, false);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000206C File Offset: 0x0000026C
		public virtual Type GetType(string name, bool throwOnError)
		{
			return this.GetType(name, throwOnError, false);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000009 RID: 9
		public abstract Type[] GetTypes();

		// Token: 0x0600000A RID: 10 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Module GetModule(string name)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002090 File Offset: 0x00000290
		public Module[] GetLoadedModules()
		{
			return this.GetLoadedModules(false);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Module[] GetLoadedModules(bool getResourceModules)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000020AC File Offset: 0x000002AC
		public Module[] GetModules()
		{
			return this.GetModules(false);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Module[] GetModules(bool getResourceModules)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Module ManifestModule
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000010 RID: 16
		public abstract MethodInfo EntryPoint { get; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002087 File Offset: 0x00000287
		public virtual string CodeBase
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000020C8 File Offset: 0x000002C8
		public virtual string EscapedCodeBase
		{
			get
			{
				return Uri.EscapeUriString(this.CodeBase);
			}
		}

		// Token: 0x06000013 RID: 19
		public abstract IList<CustomAttributeData> GetCustomAttributesData();

		// Token: 0x06000014 RID: 20 RVA: 0x000020E5 File Offset: 0x000002E5
		public virtual object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000020E5 File Offset: 0x000002E5
		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002087 File Offset: 0x00000287
		public virtual AssemblyName[] GetReferencedAssemblies()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000020F0 File Offset: 0x000002F0
		public virtual Assembly GetSatelliteAssembly(CultureInfo culture)
		{
			return this.GetSatelliteAssembly(culture, null);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002087 File Offset: 0x00000287
		public virtual bool GlobalAssemblyCache
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002087 File Offset: 0x00000287
		public virtual long HostContext
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000210A File Offset: 0x0000030A
		public virtual Stream GetManifestResourceStream(Type type, string name)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000210A File Offset: 0x0000030A
		public virtual Stream GetManifestResourceStream(string name)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000210A File Offset: 0x0000030A
		public virtual string[] GetManifestResourceNames()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000210A File Offset: 0x0000030A
		public virtual ManifestResourceInfo GetManifestResourceInfo(string resourceName)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002114 File Offset: 0x00000314
		public virtual FileStream[] GetFiles(bool getResourceModules)
		{
			Module[] modules = this.GetModules(getResourceModules);
			int num = modules.Length;
			FileStream[] array = new FileStream[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new FileStream(modules[i].FullyQualifiedName, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			return array;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002164 File Offset: 0x00000364
		public virtual FileStream[] GetFiles()
		{
			return this.GetFiles(false);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000210A File Offset: 0x0000030A
		public virtual FileStream GetFile(string name)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002180 File Offset: 0x00000380
		public override string ToString()
		{
			string fullName = this.FullName;
			bool flag = fullName == null;
			string result;
			if (flag)
			{
				result = base.ToString();
			}
			else
			{
				result = fullName;
			}
			return result;
		}
	}
}
