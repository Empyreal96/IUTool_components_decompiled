using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000077 RID: 119
	public sealed class FailureActionsPkgObject
	{
		// Token: 0x0600029D RID: 669 RVA: 0x0000A078 File Offset: 0x00008278
		public FailureActionsPkgObject()
		{
			this.Actions = new List<FailureAction>();
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000A098 File Offset: 0x00008298
		public void Build(IPackageGenerator pkgGen)
		{
			int value = -1;
			if (this.ResetPeriod != null && !this.ResetPeriod.Equals("INFINITE", StringComparison.InvariantCulture))
			{
				if (!int.TryParse(this.ResetPeriod, out value))
				{
					throw new PkgGenException("Invalid ResetPeriod value '{0}' in Service object", new object[]
					{
						this.ResetPeriod
					});
				}
			}
			else
			{
				value = -1;
			}
			if (this.Command != null)
			{
				pkgGen.AddRegValue("$(hklm.service)", "FailureCommand", RegValueType.String, this.Command);
			}
			if (this.RebootMsg != null)
			{
				pkgGen.AddRegValue("$(hklm.service)", "RebootMessage", RegValueType.String, this.RebootMsg);
			}
			List<byte> list = new List<byte>();
			if (this.Actions != null)
			{
				list.AddRange(BitConverter.GetBytes(value));
				int value2 = (this.RebootMsg != null) ? 1 : 0;
				list.AddRange(BitConverter.GetBytes(value2));
				int value3 = (this.Command != null) ? 1 : 0;
				list.AddRange(BitConverter.GetBytes(value3));
				list.AddRange(BitConverter.GetBytes(this.Actions.Count));
				list.AddRange(BitConverter.GetBytes(20));
				foreach (FailureAction failureAction in this.Actions)
				{
					list.AddRange(BitConverter.GetBytes((int)failureAction.Type));
					list.AddRange(BitConverter.GetBytes((int)failureAction.Delay));
				}
				pkgGen.AddRegValue("$(hklm.service)", "FailureActions", RegValueType.Binary, BitConverter.ToString(list.ToArray()).Replace('-', ','));
			}
		}

		// Token: 0x040001BC RID: 444
		private const int SIZE_OF_SERVICE_FAILURE_ACTIONS_WOW64 = 20;

		// Token: 0x040001BD RID: 445
		[XmlAttribute("ResetPeriod")]
		public string ResetPeriod = "INFINITE";

		// Token: 0x040001BE RID: 446
		[XmlAttribute("RebootMessage")]
		public string RebootMsg;

		// Token: 0x040001BF RID: 447
		[XmlAttribute("Command")]
		public string Command;

		// Token: 0x040001C0 RID: 448
		[XmlElement("Action")]
		public List<FailureAction> Actions;
	}
}
