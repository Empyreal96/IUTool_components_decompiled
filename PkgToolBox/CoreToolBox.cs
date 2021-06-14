using System;

namespace Microsoft.Composition.ToolBox
{
	// Token: 0x02000002 RID: 2
	public class CoreToolBox
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static T ParseEnum<T>(string value)
		{
			return (T)((object)Enum.Parse(typeof(T), value, true));
		}
	}
}
