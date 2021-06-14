using System;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200000D RID: 13
	public class BcdElementObject : BcdElement
	{
		// Token: 0x06000045 RID: 69 RVA: 0x000044AF File Offset: 0x000026AF
		public BcdElementObject(string value, BcdElementDataType dataType) : base(dataType)
		{
			base.StringData = value;
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00004518 File Offset: 0x00002718
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00004555 File Offset: 0x00002755
		public Guid ElementObject
		{
			get
			{
				Guid empty = Guid.Empty;
				if (!Guid.TryParse(base.StringData, out empty))
				{
					throw new ImageStorageException(string.Format("{0}: The string data isn't a valid Guid.", MethodBase.GetCurrentMethod().Name));
				}
				return empty;
			}
			set
			{
				base.StringData = value.ToString();
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000456C File Offset: 0x0000276C
		[CLSCompliant(false)]
		public override void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			base.LogInfo(logger, indentLevel);
			logger.LogInfo(str + "Object ID: {{{0}}}", new object[]
			{
				this.ElementObject
			});
		}
	}
}
