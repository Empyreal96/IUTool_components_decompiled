using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x0200004F RID: 79
	public sealed class ServiceBuilder : OSComponentBuilder<ServicePkgObject, ServiceBuilder>
	{
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000142 RID: 322 RVA: 0x00006044 File Offset: 0x00004244
		// (set) Token: 0x06000143 RID: 323 RVA: 0x0000604C File Offset: 0x0000424C
		public ServiceBuilder.FailureActionsBuilder FailureActions { get; private set; }

		// Token: 0x06000144 RID: 324 RVA: 0x00006055 File Offset: 0x00004255
		public ServiceBuilder(string name)
		{
			this.FailureActions = new ServiceBuilder.FailureActionsBuilder();
			this.pkgObject = new ServicePkgObject();
			this.pkgObject.Name = name;
			this.serviceEntry = null;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00006086 File Offset: 0x00004286
		public ServiceBuilder SetDisplayName(string value)
		{
			this.pkgObject.DisplayName = value;
			return this;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00006095 File Offset: 0x00004295
		public ServiceBuilder SetDescription(string value)
		{
			this.pkgObject.Description = value;
			return this;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x000060A4 File Offset: 0x000042A4
		public ServiceBuilder SetGroup(string value)
		{
			this.pkgObject.Group = value;
			return this;
		}

		// Token: 0x06000148 RID: 328 RVA: 0x000060B3 File Offset: 0x000042B3
		public ServiceBuilder SetSvcHostGroupName(string value)
		{
			this.pkgObject.SvcHostGroupName = value;
			return this;
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000060C2 File Offset: 0x000042C2
		public ServiceBuilder SetStartMode(string value)
		{
			return this.SetStartMode((ServiceStartMode)Enum.Parse(typeof(ServiceStartMode), value));
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000060DF File Offset: 0x000042DF
		public ServiceBuilder SetStartMode(ServiceStartMode value)
		{
			this.pkgObject.StartMode = value;
			return this;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x000060EE File Offset: 0x000042EE
		public ServiceBuilder SetType(string value)
		{
			return this.SetType((ServiceType)Enum.Parse(typeof(ServiceType), value));
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000610B File Offset: 0x0000430B
		public ServiceBuilder SetType(ServiceType value)
		{
			this.pkgObject.SvcType = value;
			return this;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000611A File Offset: 0x0000431A
		public ServiceBuilder SetErrorControl(string value)
		{
			return this.SetErrorControl((ErrorControlOption)Enum.Parse(typeof(ErrorControlOption), value));
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00006137 File Offset: 0x00004337
		public ServiceBuilder SetErrorControl(ErrorControlOption value)
		{
			this.pkgObject.ErrorControl = value;
			return this;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00006146 File Offset: 0x00004346
		public ServiceBuilder SetDependOnGroup(string value)
		{
			this.pkgObject.DependOnGroup = value;
			return this;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00006155 File Offset: 0x00004355
		public ServiceBuilder SetDependOnService(string value)
		{
			this.pkgObject.DependOnService = value;
			return this;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00006164 File Offset: 0x00004364
		public ServiceBuilder SetRequiredCapabilities(IEnumerable<XElement> requiredCapabilities)
		{
			this.pkgObject.RequiredCapabilities = new XElement(XName.Get("RequiredCapabilities", "urn:Microsoft.WindowsPhone/PackageSchema.v8.00"), requiredCapabilities);
			return this;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00006187 File Offset: 0x00004387
		public ServiceBuilder SetPrivateResources(IEnumerable<XElement> privateResources)
		{
			this.pkgObject.PrivateResources = new XElement(XName.Get("PrivateResources", "urn:Microsoft.WindowsPhone/PackageSchema.v8.00"), privateResources);
			return this;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x000061AA File Offset: 0x000043AA
		public ServiceBuilder.ServiceDllEntryBuilder AddServiceDll(XElement element)
		{
			if (this.serviceEntry != null)
			{
				throw new ArgumentException("Service already has an entry point.");
			}
			this.serviceEntry = new ServiceBuilder.ServiceDllEntryBuilder(element);
			return (ServiceBuilder.ServiceDllEntryBuilder)this.serviceEntry;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x000061D6 File Offset: 0x000043D6
		public ServiceBuilder.ServiceDllEntryBuilder AddServiceDll(string source, string destinationDir)
		{
			if (this.serviceEntry != null)
			{
				throw new ArgumentException("Service already has an entry point.");
			}
			this.serviceEntry = new ServiceBuilder.ServiceDllEntryBuilder(source, destinationDir);
			return (ServiceBuilder.ServiceDllEntryBuilder)this.serviceEntry;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00006203 File Offset: 0x00004403
		public ServiceBuilder.ServiceDllEntryBuilder AddServiceDll(string source)
		{
			return this.AddServiceDll(source, "$(runtime.default)");
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00006211 File Offset: 0x00004411
		public ServiceBuilder.ServiceExeEntryBuilder AddExecutable(XElement element)
		{
			if (this.serviceEntry != null)
			{
				throw new ArgumentException("Service already has an entry point.");
			}
			this.serviceEntry = new ServiceBuilder.ServiceExeEntryBuilder(element);
			return (ServiceBuilder.ServiceExeEntryBuilder)this.serviceEntry;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000623D File Offset: 0x0000443D
		public ServiceBuilder.ServiceExeEntryBuilder AddExecutable(string source, string destinationDir)
		{
			if (this.serviceEntry != null)
			{
				throw new ArgumentException("Service already has an entry point.");
			}
			this.serviceEntry = new ServiceBuilder.ServiceExeEntryBuilder(source, destinationDir);
			return (ServiceBuilder.ServiceExeEntryBuilder)this.serviceEntry;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000626A File Offset: 0x0000446A
		public ServiceBuilder.ServiceExeEntryBuilder AddExecutable(string source)
		{
			return this.AddExecutable(source, "$(runtime.default)");
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00006278 File Offset: 0x00004478
		public override ServicePkgObject ToPkgObject()
		{
			base.RegisterMacro("runtime.default", "$(runtime.system32)");
			base.RegisterMacro("env.default", "$(env.system32)");
			base.RegisterMacro("hklm.service", "$(hklm.system)\\controlset001\\services\\" + this.pkgObject.Name);
			if (this.serviceEntry == null)
			{
				throw new ArgumentException("Service needs to have an entry point.");
			}
			if (this.serviceEntry is ServiceBuilder.ServiceExeEntryBuilder)
			{
				if (!string.IsNullOrEmpty(this.pkgObject.SvcHostGroupName))
				{
					throw new ArgumentException("SvcHostGroupName should not be set when using an ExeEntry");
				}
				this.pkgObject.SvcEntry = ((ServiceBuilder.ServiceExeEntryBuilder)this.serviceEntry).ToPkgObject();
			}
			else if (this.serviceEntry is ServiceBuilder.ServiceDllEntryBuilder)
			{
				if (string.IsNullOrEmpty(this.pkgObject.SvcHostGroupName))
				{
					throw new ArgumentException("SvcHostGroupName should be set when using an DllEntry");
				}
				this.pkgObject.SvcEntry = ((ServiceBuilder.ServiceDllEntryBuilder)this.serviceEntry).ToPkgObject();
			}
			this.pkgObject.FailureActions = this.FailureActions.ToFailureActions();
			return base.ToPkgObject();
		}

		// Token: 0x04000114 RID: 276
		private object serviceEntry;

		// Token: 0x02000095 RID: 149
		public class FailureActionsBuilder
		{
			// Token: 0x0600032E RID: 814 RVA: 0x0000AD10 File Offset: 0x00008F10
			public FailureActionsBuilder()
			{
				this.pkgObject = new FailureActionsPkgObject();
			}

			// Token: 0x0600032F RID: 815 RVA: 0x0000AD24 File Offset: 0x00008F24
			public ServiceBuilder.FailureActionsBuilder SetResetPeriod(string period)
			{
				int num;
				if (!period.Equals("INFINITE", StringComparison.InvariantCultureIgnoreCase) && !int.TryParse(period, out num))
				{
					throw new ArgumentException("Period must be a number or 'INFINITE'");
				}
				this.pkgObject.ResetPeriod = period;
				return this;
			}

			// Token: 0x06000330 RID: 816 RVA: 0x0000AD61 File Offset: 0x00008F61
			public ServiceBuilder.FailureActionsBuilder SetResetPeriod(int period)
			{
				this.pkgObject.ResetPeriod = ((period < 0) ? "INFINITE" : period.ToString());
				return this;
			}

			// Token: 0x06000331 RID: 817 RVA: 0x0000AD81 File Offset: 0x00008F81
			public ServiceBuilder.FailureActionsBuilder SetRebootMessage(string value)
			{
				this.pkgObject.RebootMsg = value;
				return this;
			}

			// Token: 0x06000332 RID: 818 RVA: 0x0000AD90 File Offset: 0x00008F90
			public ServiceBuilder.FailureActionsBuilder SetCommand(string value)
			{
				this.pkgObject.Command = value;
				return this;
			}

			// Token: 0x06000333 RID: 819 RVA: 0x0000ADA0 File Offset: 0x00008FA0
			public ServiceBuilder.FailureActionsBuilder AddFailureAction(XNode element)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(FailureAction));
				using (XmlReader xmlReader = element.CreateReader())
				{
					this.pkgObject.Actions.Add((FailureAction)xmlSerializer.Deserialize(xmlReader));
				}
				return this;
			}

			// Token: 0x06000334 RID: 820 RVA: 0x0000AE00 File Offset: 0x00009000
			public ServiceBuilder.FailureActionsBuilder AddFailureAction(FailureActionType type, uint delay)
			{
				this.pkgObject.Actions.Add(new FailureAction(type, delay));
				return this;
			}

			// Token: 0x06000335 RID: 821 RVA: 0x0000AE1A File Offset: 0x0000901A
			public FailureActionsPkgObject ToFailureActions()
			{
				return this.pkgObject;
			}

			// Token: 0x040001FD RID: 509
			public const int INFINITE_RESET_PERIOD = -1;

			// Token: 0x040001FE RID: 510
			private FailureActionsPkgObject pkgObject;
		}

		// Token: 0x02000096 RID: 150
		public class ServiceExeEntryBuilder : FileBuilder<SvcExe, ServiceBuilder.ServiceExeEntryBuilder>
		{
			// Token: 0x06000336 RID: 822 RVA: 0x0000AE22 File Offset: 0x00009022
			public ServiceExeEntryBuilder(XElement element) : base(element)
			{
			}

			// Token: 0x06000337 RID: 823 RVA: 0x0000AE2B File Offset: 0x0000902B
			public ServiceExeEntryBuilder(string source, string destinationDir) : base(source, destinationDir)
			{
			}
		}

		// Token: 0x02000097 RID: 151
		public class ServiceDllEntryBuilder : FileBuilder<SvcDll, ServiceBuilder.ServiceDllEntryBuilder>
		{
			// Token: 0x06000338 RID: 824 RVA: 0x0000AE35 File Offset: 0x00009035
			public ServiceDllEntryBuilder(XElement element) : base(element)
			{
			}

			// Token: 0x06000339 RID: 825 RVA: 0x0000AE3E File Offset: 0x0000903E
			public ServiceDllEntryBuilder(string source, string destinationDir) : base(source, destinationDir)
			{
			}

			// Token: 0x0600033A RID: 826 RVA: 0x0000AE48 File Offset: 0x00009048
			public ServiceBuilder.ServiceDllEntryBuilder SetServiceManifest(string value)
			{
				this.pkgObject.ServiceManifest = value;
				return this;
			}

			// Token: 0x0600033B RID: 827 RVA: 0x0000AE57 File Offset: 0x00009057
			public ServiceBuilder.ServiceDllEntryBuilder SetServiceMain(string value)
			{
				this.pkgObject.ServiceName = value;
				return this;
			}

			// Token: 0x0600033C RID: 828 RVA: 0x0000AE66 File Offset: 0x00009066
			public ServiceBuilder.ServiceDllEntryBuilder SetUnloadOnStop(bool value)
			{
				this.pkgObject.UnloadOnStop = value;
				return this;
			}

			// Token: 0x0600033D RID: 829 RVA: 0x0000AE75 File Offset: 0x00009075
			public ServiceBuilder.ServiceDllEntryBuilder SetHostExe(string value)
			{
				this.pkgObject.HostExe = value;
				return this;
			}
		}
	}
}
