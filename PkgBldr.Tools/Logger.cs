using System;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200000E RID: 14
	public class Logger : IULogger, IDeploymentLogger, IDeploymentLogger
	{
		// Token: 0x0600006A RID: 106 RVA: 0x000036E2 File Offset: 0x000018E2
		public void LogSpkgGenOutput(string spkggenOutput)
		{
			base.LogDebug(spkggenOutput, new object[0]);
		}
	}
}
