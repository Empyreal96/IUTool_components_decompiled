using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x0200003A RID: 58
	public sealed class ComServerBuilder : OSComponentBuilder<ComPkgObject, ComServerBuilder>
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x000052F9 File Offset: 0x000034F9
		public ComServerBuilder()
		{
			this.dll = null;
			this.classes = new List<ComServerBuilder.ComClassBuilder>();
			this.interfaces = new List<ComServerBuilder.ComInterfaceBuilder>();
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000531E File Offset: 0x0000351E
		public ComServerBuilder.ComDllBuilder SetComDll(XElement file)
		{
			this.dll = new ComServerBuilder.ComDllBuilder(file);
			return this.dll;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00005332 File Offset: 0x00003532
		public ComServerBuilder.ComDllBuilder SetComDll(string source)
		{
			return this.SetComDll(source, "$(runtime.default)");
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00005340 File Offset: 0x00003540
		public ComServerBuilder.ComDllBuilder SetComDll(string source, string destinationDir)
		{
			this.dll = new ComServerBuilder.ComDllBuilder(source, destinationDir);
			return this.dll;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00005358 File Offset: 0x00003558
		public ComServerBuilder.ComClassBuilder AddClass(XElement element)
		{
			ComServerBuilder.ComClassBuilder comClassBuilder = new ComServerBuilder.ComClassBuilder(element);
			this.classes.Add(comClassBuilder);
			return comClassBuilder;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000537C File Offset: 0x0000357C
		public ComServerBuilder.ComClassBuilder AddClass(Guid classId)
		{
			ComServerBuilder.ComClassBuilder comClassBuilder = new ComServerBuilder.ComClassBuilder(classId);
			this.classes.Add(comClassBuilder);
			return comClassBuilder;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000053A0 File Offset: 0x000035A0
		public ComServerBuilder.ComInterfaceBuilder AddInterface(XElement element)
		{
			ComServerBuilder.ComInterfaceBuilder comInterfaceBuilder = new ComServerBuilder.ComInterfaceBuilder(element);
			this.interfaces.Add(comInterfaceBuilder);
			return comInterfaceBuilder;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000053C4 File Offset: 0x000035C4
		public ComServerBuilder.ComInterfaceBuilder AddInterface(Guid interfaceId)
		{
			ComServerBuilder.ComInterfaceBuilder comInterfaceBuilder = new ComServerBuilder.ComInterfaceBuilder(interfaceId);
			this.interfaces.Add(comInterfaceBuilder);
			return comInterfaceBuilder;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000053E8 File Offset: 0x000035E8
		public override ComPkgObject ToPkgObject()
		{
			this.pkgObject.ComDll = this.dll.ToPkgObject();
			this.pkgObject.Classes.Clear();
			this.pkgObject.Interfaces.Clear();
			this.classes.ForEach(delegate(ComServerBuilder.ComClassBuilder x)
			{
				this.pkgObject.Classes.Add(x.ToPkgObject());
			});
			this.interfaces.ForEach(delegate(ComServerBuilder.ComInterfaceBuilder x)
			{
				this.pkgObject.Interfaces.Add(x.ToPkgObject());
			});
			return base.ToPkgObject();
		}

		// Token: 0x040000E4 RID: 228
		private ComServerBuilder.ComDllBuilder dll;

		// Token: 0x040000E5 RID: 229
		private List<ComServerBuilder.ComClassBuilder> classes;

		// Token: 0x040000E6 RID: 230
		private List<ComServerBuilder.ComInterfaceBuilder> interfaces;

		// Token: 0x02000091 RID: 145
		public class ComDllBuilder : FileBuilder<ComDll, ComServerBuilder.ComDllBuilder>
		{
			// Token: 0x06000315 RID: 789 RVA: 0x0000AAA8 File Offset: 0x00008CA8
			public ComDllBuilder(XElement element) : base(element)
			{
			}

			// Token: 0x06000316 RID: 790 RVA: 0x0000AAB1 File Offset: 0x00008CB1
			public ComDllBuilder(string source, string destinationDir) : base(source, destinationDir)
			{
			}
		}

		// Token: 0x02000092 RID: 146
		public abstract class ComBaseBuilder<T, V> : PkgObjectBuilder<T, V> where T : ComBase, new() where V : ComServerBuilder.ComBaseBuilder<T, V>
		{
			// Token: 0x06000317 RID: 791 RVA: 0x0000AABB File Offset: 0x00008CBB
			internal ComBaseBuilder()
			{
				this.regKeys = new List<RegistryKeyBuilder>();
			}

			// Token: 0x06000318 RID: 792 RVA: 0x0000AACE File Offset: 0x00008CCE
			public V SetTypeLib(string value)
			{
				this.pkgObject.TypeLib = value;
				return this as V;
			}

			// Token: 0x06000319 RID: 793 RVA: 0x0000AAEC File Offset: 0x00008CEC
			public V SetVersion(string value)
			{
				this.pkgObject.Version = value;
				return this as V;
			}

			// Token: 0x0600031A RID: 794 RVA: 0x0000AB0C File Offset: 0x00008D0C
			public RegistryKeyBuilder AddRegistryKey(string keyName)
			{
				RegistryKeyBuilder registryKeyBuilder = new RegistryKeyBuilder(keyName);
				this.regKeys.Add(registryKeyBuilder);
				return registryKeyBuilder;
			}

			// Token: 0x0600031B RID: 795 RVA: 0x0000AB2D File Offset: 0x00008D2D
			public override T ToPkgObject()
			{
				this.regKeys.ForEach(delegate(RegistryKeyBuilder x)
				{
					this.pkgObject.RegKeys.Add(x.ToPkgObject());
				});
				return base.ToPkgObject();
			}

			// Token: 0x040001FC RID: 508
			private List<RegistryKeyBuilder> regKeys;
		}

		// Token: 0x02000093 RID: 147
		public sealed class ComClassBuilder : ComServerBuilder.ComBaseBuilder<ComClass, ComServerBuilder.ComClassBuilder>
		{
			// Token: 0x0600031D RID: 797 RVA: 0x0000AB69 File Offset: 0x00008D69
			internal ComClassBuilder(XElement element)
			{
				this.pkgObject = element.FromXElement<ComClass>();
			}

			// Token: 0x0600031E RID: 798 RVA: 0x0000AB7D File Offset: 0x00008D7D
			internal ComClassBuilder(Guid classId)
			{
				this.pkgObject = new ComClass();
				this.pkgObject.Id = classId.ToString();
			}

			// Token: 0x0600031F RID: 799 RVA: 0x0000ABA8 File Offset: 0x00008DA8
			public ComServerBuilder.ComClassBuilder SetThreadingModel(ThreadingModel model)
			{
				this.pkgObject.ThreadingModel = Enum.GetName(typeof(ThreadingModel), model);
				return this;
			}

			// Token: 0x06000320 RID: 800 RVA: 0x0000ABCB File Offset: 0x00008DCB
			public ComServerBuilder.ComClassBuilder SetProgId(string value)
			{
				this.pkgObject.ProgId = value;
				return this;
			}

			// Token: 0x06000321 RID: 801 RVA: 0x0000ABDA File Offset: 0x00008DDA
			public ComServerBuilder.ComClassBuilder SetVersionIndependentProgId(string value)
			{
				this.pkgObject.VersionIndependentProgId = value;
				return this;
			}

			// Token: 0x06000322 RID: 802 RVA: 0x0000ABE9 File Offset: 0x00008DE9
			public ComServerBuilder.ComClassBuilder SetDescription(string value)
			{
				this.pkgObject.Description = value;
				return this;
			}

			// Token: 0x06000323 RID: 803 RVA: 0x0000ABF8 File Offset: 0x00008DF8
			public ComServerBuilder.ComClassBuilder SetDefaultIcon(string value)
			{
				this.pkgObject.DefaultIcon = value;
				return this;
			}

			// Token: 0x06000324 RID: 804 RVA: 0x0000AC07 File Offset: 0x00008E07
			public ComServerBuilder.ComClassBuilder SetAppId(string value)
			{
				this.pkgObject.AppId = value;
				return this;
			}

			// Token: 0x06000325 RID: 805 RVA: 0x0000AC16 File Offset: 0x00008E16
			public ComServerBuilder.ComClassBuilder SetSkipInProcServer32(bool flag)
			{
				this.pkgObject.SkipInProcServer32 = flag;
				return this;
			}

			// Token: 0x06000326 RID: 806 RVA: 0x0000AC25 File Offset: 0x00008E25
			public override ComClass ToPkgObject()
			{
				base.RegisterMacro("hkcr.clsid", "$(hkcr.root)\\CLSID\\" + this.pkgObject.Id);
				return base.ToPkgObject();
			}
		}

		// Token: 0x02000094 RID: 148
		public sealed class ComInterfaceBuilder : ComServerBuilder.ComBaseBuilder<ComInterface, ComServerBuilder.ComInterfaceBuilder>
		{
			// Token: 0x06000327 RID: 807 RVA: 0x0000AC4E File Offset: 0x00008E4E
			internal ComInterfaceBuilder(XElement element)
			{
				this.pkgObject = element.FromXElement<ComInterface>();
			}

			// Token: 0x06000328 RID: 808 RVA: 0x0000AC62 File Offset: 0x00008E62
			internal ComInterfaceBuilder(Guid classId)
			{
				this.pkgObject = new ComInterface();
				this.pkgObject.Id = classId.ToString();
			}

			// Token: 0x06000329 RID: 809 RVA: 0x0000AC8D File Offset: 0x00008E8D
			public ComServerBuilder.ComInterfaceBuilder SetName(string name)
			{
				this.pkgObject.Name = name;
				return this;
			}

			// Token: 0x0600032A RID: 810 RVA: 0x0000AC9C File Offset: 0x00008E9C
			public ComServerBuilder.ComInterfaceBuilder SetProxyStubClsId(Guid clsId)
			{
				this.pkgObject.ProxyStubClsId = clsId.ToString();
				return this;
			}

			// Token: 0x0600032B RID: 811 RVA: 0x0000ACB7 File Offset: 0x00008EB7
			public ComServerBuilder.ComInterfaceBuilder SetProxyStubClsId32(Guid clsId)
			{
				this.pkgObject.ProxyStubClsId32 = clsId.ToString();
				return this;
			}

			// Token: 0x0600032C RID: 812 RVA: 0x0000ACD2 File Offset: 0x00008ED2
			public ComServerBuilder.ComInterfaceBuilder SetNumMethods(int numMethods)
			{
				this.pkgObject.NumMethods = numMethods.ToString();
				return this;
			}

			// Token: 0x0600032D RID: 813 RVA: 0x0000ACE7 File Offset: 0x00008EE7
			public override ComInterface ToPkgObject()
			{
				base.RegisterMacro("hkcr.iid", "$(hkcr.root)\\Interface\\" + this.pkgObject.Id);
				return base.ToPkgObject();
			}
		}
	}
}
