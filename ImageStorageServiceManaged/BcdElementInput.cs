using System;
using System.IO;
using System.Reflection;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000067 RID: 103
	public class BcdElementInput
	{
		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x00014263 File Offset: 0x00012463
		// (set) Token: 0x06000495 RID: 1173 RVA: 0x0001426B File Offset: 0x0001246B
		public BcdElementDataTypeInput DataType { get; set; }

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x00014274 File Offset: 0x00012474
		// (set) Token: 0x06000497 RID: 1175 RVA: 0x0001427C File Offset: 0x0001247C
		public BcdElementValueTypeInput ValueType { get; set; }

		// Token: 0x06000498 RID: 1176 RVA: 0x00014288 File Offset: 0x00012488
		protected void RegFilePreProcessing()
		{
			if (this.DataType.TypeIdentifier == DataTypeChoice.WellKnownType && BcdElementDataTypes.GetWellKnownDataType(this.DataType.DataType as string) == BcdElementDataTypes.CustomActionsList)
			{
				if (this.ValueType.ValueIdentifier != ValueTypeChoice.IntegerListValue)
				{
					throw new ImageStorageException(string.Format("{0}: A custom action list should have an integer list associated with it.", MethodBase.GetCurrentMethod().Name));
				}
				BcdElementIntegerListInput bcdElementIntegerListInput = this.ValueType.ValueType as BcdElementIntegerListInput;
				if (bcdElementIntegerListInput.StringValues.Length % 2 != 0)
				{
					throw new ImageStorageException(string.Format("{0}: A custom action list should have one element associated with each scan key code.", MethodBase.GetCurrentMethod().Name));
				}
				for (int i = 0; i < bcdElementIntegerListInput.StringValues.Length; i += 2)
				{
					BcdElementDataType wellKnownDataType = BcdElementDataTypes.GetWellKnownDataType(bcdElementIntegerListInput.StringValues[i + 1]);
					if (wellKnownDataType != null)
					{
						bcdElementIntegerListInput.StringValues[i + 1] = string.Format("{0:x8}", wellKnownDataType.RawValue);
					}
				}
			}
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x00014368 File Offset: 0x00012568
		public void SaveAsRegFile(StreamWriter writer, string path)
		{
			this.RegFilePreProcessing();
			this.DataType.SaveAsRegFile(writer, path);
			this.ValueType.SaveAsRegFile(writer, string.Format("{0:x8}", this.DataType.Type));
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0001439E File Offset: 0x0001259E
		public void SaveAsRegData(BcdRegData bcdRegData, string path)
		{
			this.RegFilePreProcessing();
			this.DataType.SaveAsRegData(bcdRegData, path);
			this.ValueType.SaveAsRegData(bcdRegData, string.Format("{0}\\{1:x8}", path, this.DataType.Type));
		}
	}
}
