using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000020 RID: 32
	public class Components : IPolicyElement
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00006204 File Offset: 0x00004404
		[XmlElement(ElementName = "Application")]
		public List<Application> ApplicationCollection
		{
			get
			{
				return this.applicationCollection;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000110 RID: 272 RVA: 0x0000620C File Offset: 0x0000440C
		[XmlElement(ElementName = "AppBinaries")]
		public List<AppBinaries> AppBinariesCollection
		{
			get
			{
				return this.appBinariesCollection;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000111 RID: 273 RVA: 0x00006214 File Offset: 0x00004414
		[XmlElement(ElementName = "Service")]
		public List<Service> ServiceCollection
		{
			get
			{
				return this.serviceCollection;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000112 RID: 274 RVA: 0x0000621C File Offset: 0x0000441C
		[XmlElement(ElementName = "FullTrust")]
		public List<FullTrust> FullTrustCollection
		{
			get
			{
				return this.fullTrustCollection;
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00006224 File Offset: 0x00004424
		public void Add(IXPathNavigable componentXmlElement)
		{
			this.AddElements((XmlElement)componentXmlElement);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00006232 File Offset: 0x00004432
		private void AddElements(XmlElement componentXmlElement)
		{
			this.AddApplications(componentXmlElement);
			this.AddAppBinaries(componentXmlElement);
			this.AddServices(componentXmlElement);
			this.AddFullTrust(componentXmlElement);
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00006250 File Offset: 0x00004450
		private void AddApplications(XmlElement componentsXmlElement)
		{
			XmlNodeList xmlNodeList = componentsXmlElement.SelectNodes("./WP_Policy:Application", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.applicationCollection == null)
				{
					this.applicationCollection = new List<Application>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement applicationXmlElement = (XmlElement)obj;
					Application application = new Application();
					application.Add(applicationXmlElement);
					this.applicationCollection.Add(application);
					if (application.HasPrivateResources())
					{
						if (this.capabilityCollection == null)
						{
							this.capabilityCollection = new List<Capability>();
						}
						this.capabilityCollection.Add(application.PrivateResources);
					}
				}
			}
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00006318 File Offset: 0x00004518
		private void AddAppBinaries(XmlElement componentsXmlElement)
		{
			XmlNodeList xmlNodeList = componentsXmlElement.SelectNodes("./WP_Policy:AppResource", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.appBinariesCollection == null)
				{
					this.appBinariesCollection = new List<AppBinaries>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement applicationXmlElement = (XmlElement)obj;
					AppBinaries appBinaries = new AppBinaries();
					appBinaries.Add(applicationXmlElement);
					if (appBinaries.ApplicationFileCollection.Count > 0)
					{
						this.appBinariesCollection.Add(appBinaries);
					}
				}
			}
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000063BC File Offset: 0x000045BC
		private void AddServices(XmlElement componentsXmlElement)
		{
			XmlNodeList xmlNodeList = componentsXmlElement.SelectNodes("./WP_Policy:Service[@Type='Win32OwnProcess' or @Type='Win32ShareProcess']", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.serviceCollection == null)
				{
					this.serviceCollection = new List<Service>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement appServiceXmlElement = (XmlElement)obj;
					Service service = new Service();
					service.Add(appServiceXmlElement);
					this.serviceCollection.Add(service);
					if (service.HasPrivateResources())
					{
						if (this.capabilityCollection == null)
						{
							this.capabilityCollection = new List<Capability>();
						}
						this.capabilityCollection.Add(service.PrivateResources);
					}
				}
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00006484 File Offset: 0x00004684
		private void AddFullTrust(XmlElement componentsXmlElement)
		{
			XmlNodeList xmlNodeList = componentsXmlElement.SelectNodes("./WP_Policy:FullTrust", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.fullTrustCollection == null)
				{
					this.fullTrustCollection = new List<FullTrust>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement fullTrustXmlElement = (XmlElement)obj;
					FullTrust fullTrust = new FullTrust();
					fullTrust.Add(fullTrustXmlElement);
					this.fullTrustCollection.Add(fullTrust);
				}
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000651C File Offset: 0x0000471C
		public bool HasPrivateCapabilities()
		{
			return this.capabilityCollection != null;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00006528 File Offset: 0x00004728
		public bool HasChild()
		{
			return (this.serviceCollection != null && this.serviceCollection.Count > 0) || (this.applicationCollection != null && this.applicationCollection.Count > 0) || (this.appBinariesCollection != null && this.appBinariesCollection.Count > 0);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000657B File Offset: 0x0000477B
		public List<Capability> GetPrivateCapabilities()
		{
			return this.capabilityCollection;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00006584 File Offset: 0x00004784
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel1, "Components");
			if (this.applicationCollection != null)
			{
				foreach (Application application in this.applicationCollection)
				{
					instance.DebugLine(string.Empty);
					application.Print();
				}
			}
			if (this.appBinariesCollection != null)
			{
				foreach (ApplicationFiles applicationFiles in this.appBinariesCollection)
				{
					instance.DebugLine(string.Empty);
					applicationFiles.Print();
				}
			}
			if (this.serviceCollection != null)
			{
				foreach (Service service in this.serviceCollection)
				{
					instance.DebugLine(string.Empty);
					service.Print();
				}
			}
			if (this.fullTrustCollection != null)
			{
				foreach (FullTrust fullTrust in this.fullTrustCollection)
				{
					instance.DebugLine(string.Empty);
					fullTrust.Print();
				}
			}
		}

		// Token: 0x040000F7 RID: 247
		private List<Application> applicationCollection;

		// Token: 0x040000F8 RID: 248
		private List<AppBinaries> appBinariesCollection;

		// Token: 0x040000F9 RID: 249
		private List<Service> serviceCollection;

		// Token: 0x040000FA RID: 250
		private List<FullTrust> fullTrustCollection;

		// Token: 0x040000FB RID: 251
		private List<Capability> capabilityCollection;
	}
}
