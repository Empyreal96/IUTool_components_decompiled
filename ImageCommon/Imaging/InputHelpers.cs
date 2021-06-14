using System;
using System.Globalization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001C RID: 28
	internal class InputHelpers
	{
		// Token: 0x06000128 RID: 296 RVA: 0x0000D584 File Offset: 0x0000B784
		public static bool StringToUint(string valueAsString, out uint value)
		{
			bool result = true;
			if (valueAsString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			{
				if (!uint.TryParse(valueAsString.Substring(2, valueAsString.Length - 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value))
				{
					result = false;
				}
			}
			else if (!uint.TryParse(valueAsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000D5D8 File Offset: 0x0000B7D8
		public static bool IsPowerOfTwo(uint value)
		{
			return (value & value - 1U) == 0U;
		}
	}
}
