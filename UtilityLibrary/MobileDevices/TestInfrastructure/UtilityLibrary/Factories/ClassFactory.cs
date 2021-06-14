using System;
using System.Configuration;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary.Factories
{
	// Token: 0x02000016 RID: 22
	public class ClassFactory<I, P> where P : I, new()
	{
		// Token: 0x0600006F RID: 111 RVA: 0x00004298 File Offset: 0x00002498
		static ClassFactory()
		{
			if (!ClassFactory<I, P>.publishedInterface.IsInterface)
			{
				throw new ArgumentException(string.Format("Type {0} must be an interface", typeof(I).Name));
			}
			ClassFactory<I, P>.ProviderTypeNameKey = ClassFactory<I, P>.publishedInterface.Name + "ProviderType";
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00004320 File Offset: 0x00002520
		// (set) Token: 0x06000071 RID: 113 RVA: 0x00004336 File Offset: 0x00002536
		private static string ProviderTypeNameKey { get; set; }

		// Token: 0x06000072 RID: 114 RVA: 0x00004340 File Offset: 0x00002540
		public static Type GetProviderType()
		{
			return ClassFactory<I, P>.providerType;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004358 File Offset: 0x00002558
		public static void SetProviderType(Type value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("Provider Type must not be null");
			}
			if (!ClassFactory<I, P>.publishedInterface.IsAssignableFrom(value))
			{
				throw new ArgumentException(string.Format("Type {0} does not implement {1}.", value.FullName, ClassFactory<I, P>.publishedInterface.FullName));
			}
			lock (ClassFactory<I, P>.providerLock)
			{
				ClassFactory<I, P>.providerType = value;
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000043EC File Offset: 0x000025EC
		public static void CreateFactoryConfigFile(string exeFileName, Type providerType)
		{
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(exeFileName);
			if (configuration.AppSettings.Settings[ClassFactory<I, P>.ProviderTypeNameKey] == null)
			{
				configuration.AppSettings.Settings.Add(ClassFactory<I, P>.ProviderTypeNameKey, providerType.AssemblyQualifiedName);
			}
			else
			{
				configuration.AppSettings.Settings[ClassFactory<I, P>.ProviderTypeNameKey].Value = providerType.AssemblyQualifiedName;
			}
			configuration.Save();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000446C File Offset: 0x0000266C
		public static I Create()
		{
			if (!ClassFactory<I, P>.haveCheckedConfiguration)
			{
				ClassFactory<I, P>.haveCheckedConfiguration = true;
				string text = ConfigurationManager.AppSettings[ClassFactory<I, P>.ProviderTypeNameKey];
				if (!string.IsNullOrEmpty(text))
				{
					Type type = Type.GetType(text, true);
					ClassFactory<I, P>.SetProviderType(type);
				}
			}
			return (I)((object)Activator.CreateInstance(ClassFactory<I, P>.providerType, true));
		}

		// Token: 0x0400005E RID: 94
		private static Type publishedInterface = typeof(I);

		// Token: 0x0400005F RID: 95
		private static Type providerType = typeof(P);

		// Token: 0x04000060 RID: 96
		private static object providerLock = new object();

		// Token: 0x04000061 RID: 97
		private static bool haveCheckedConfiguration = false;
	}
}
