using System;
using System.Reflection;

namespace Microsoft.Composition.ToolBox.Reflection
{
	// Token: 0x02000011 RID: 17
	public class ReflectiveObject
	{
		// Token: 0x0600004F RID: 79 RVA: 0x00002F13 File Offset: 0x00001113
		public sealed override int GetHashCode()
		{
			return ReflectiveObject.HashObject(this);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002F1B File Offset: 0x0000111B
		public override bool Equals(object obj)
		{
			return ReflectiveObject.HashObject(this) == ReflectiveObject.HashObject(obj);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002F2C File Offset: 0x0000112C
		private static int HashObject(object obj)
		{
			PropertyInfo[] properties = obj.GetType().GetProperties();
			int num = 0;
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.Name.Equals("AllFiles") && !propertyInfo.Name.Equals("AllPayloadFiles") && !propertyInfo.Name.Equals("AllManifestFiles"))
				{
					object value = propertyInfo.GetValue(obj, null);
					if (value != null)
					{
						num ^= value.GetHashCode();
					}
				}
			}
			return num;
		}
	}
}
