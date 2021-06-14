using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200000E RID: 14
	public class BcdElementObjectList : BcdElement
	{
		// Token: 0x06000049 RID: 73 RVA: 0x000045B9 File Offset: 0x000027B9
		public BcdElementObjectList(string[] multiStringData, BcdElementDataType dataType) : base(dataType)
		{
			this._multiStringData = new List<string>(multiStringData);
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000045D0 File Offset: 0x000027D0
		public List<Guid> ObjectList
		{
			get
			{
				List<Guid> list = new List<Guid>(base.MultiStringData.Count);
				for (int i = 0; i < base.MultiStringData.Count; i++)
				{
					Guid empty = Guid.Empty;
					if (!Guid.TryParse(base.MultiStringData[i], out empty))
					{
						throw new ImageStorageException(string.Format("{0}: The string data isn't a valid Guid.", MethodBase.GetCurrentMethod().Name));
					}
					list.Add(empty);
				}
				return list;
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004644 File Offset: 0x00002844
		[CLSCompliant(false)]
		public override void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			base.LogInfo(logger, indentLevel);
			foreach (Guid guid in this.ObjectList)
			{
				logger.LogInfo(str + "Object ID: {{{0}}}", new object[]
				{
					guid
				});
			}
		}
	}
}
