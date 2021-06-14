using System;
using System.Globalization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x02000038 RID: 56
	public sealed class AppPackageFactory
	{
		// Token: 0x060000E9 RID: 233 RVA: 0x00006A91 File Offset: 0x00004C91
		private AppPackageFactory()
		{
			throw new NotSupportedException("The 'AppPackageFactory' class should never be constructed on its own. Please use only the static methods.");
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00006AA4 File Offset: 0x00004CA4
		public static IInboxAppPackage CreateAppPackage(InboxAppParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters", "Internal error: The InboxAppParameters is null.");
			}
			string packageBasePath = parameters.PackageBasePath;
			IInboxAppPackage inboxAppPackage = null;
			if (InboxAppUtils.ExtensionMatches(packageBasePath, ".xap"))
			{
				throw new ArgumentException("This tool does not support XAP packages for infusion. Try using an appx or appxbundle.");
			}
			if (InboxAppUtils.ExtensionMatches(packageBasePath, ".appxbundle"))
			{
				inboxAppPackage = new AppPackageAppxBundle(parameters);
			}
			else if (InboxAppUtils.ExtensionMatches(packageBasePath, ".appx"))
			{
				inboxAppPackage = new AppPackageAppx(parameters, true, "");
			}
			if (inboxAppPackage == null)
			{
				LogUtil.Error(string.Format(CultureInfo.InvariantCulture, "The packageBasePath \"{0}\" is of a package type that is not supported.", new object[]
				{
					packageBasePath
				}));
			}
			return inboxAppPackage;
		}
	}
}
