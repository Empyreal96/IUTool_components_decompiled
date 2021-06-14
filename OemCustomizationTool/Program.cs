using System;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x0200000B RID: 11
	internal class Program
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00003C4C File Offset: 0x00001E4C
		private static void Main(string[] args)
		{
			try
			{
				TraceLogger.TraceLevel = TraceLevel.Warn;
				if (!new InputParameters(args).IsInputParamValid)
				{
					TraceLogger.LogMessage(TraceLevel.Error, "Exiting. Try again with the expected parameters.", true);
				}
				else
				{
					Customization cust = new Customization(Settings.CustomizationFiles);
					Configuration conf = new Configuration(Settings.ConfigFiles);
					new CustomizationPkgBuilder(cust, conf).GenerateCustomizationPackage();
				}
			}
			catch (Exception ex)
			{
				TraceLogger.LogMessage(TraceLevel.Error, "Customization package creation failed!", true);
				TraceLogger.LogMessage(TraceLevel.Info, ex.ToString(), true);
			}
		}
	}
}
