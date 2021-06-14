using System;

namespace Microsoft.Phone.Test.TestMetadata.Helper
{
	// Token: 0x02000003 RID: 3
	public static class Contract
	{
		// Token: 0x0600000D RID: 13 RVA: 0x000022C8 File Offset: 0x000004C8
		public static void Requires(bool precondition)
		{
			bool flag = !precondition;
			if (flag)
			{
				throw new ArgumentException("Method precondition violated");
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000022EC File Offset: 0x000004EC
		public static void Requires(bool precondition, string parameter)
		{
			bool flag = !precondition;
			if (flag)
			{
				throw new ArgumentException("Invalid argument value", parameter);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002310 File Offset: 0x00000510
		public static void Requires(bool precondition, string parameter, string message)
		{
			bool flag = !precondition;
			if (flag)
			{
				throw new ArgumentException(message, parameter);
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002330 File Offset: 0x00000530
		public static void RequiresNotNull(object value, string parameter)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException(parameter);
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002350 File Offset: 0x00000550
		public static void RequiresNotEmpty(string variable, string parameter)
		{
			bool flag = variable != null && variable.Length == 0;
			if (flag)
			{
				throw new ArgumentException("Non-empty string required", parameter);
			}
		}
	}
}
