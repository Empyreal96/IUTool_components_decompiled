using System;
using System.Globalization;

namespace System.Reflection.Mock
{
	// Token: 0x02000005 RID: 5
	internal abstract class Binder
	{
		// Token: 0x060000AB RID: 171
		public abstract MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state);

		// Token: 0x060000AC RID: 172
		public abstract FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture);

		// Token: 0x060000AD RID: 173
		public abstract MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers);

		// Token: 0x060000AE RID: 174
		public abstract PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers);

		// Token: 0x060000AF RID: 175
		public abstract object ChangeType(object value, Type type, CultureInfo culture);

		// Token: 0x060000B0 RID: 176
		public abstract void ReorderArgumentArray(ref object[] args, object state);

		// Token: 0x060000B1 RID: 177 RVA: 0x00003298 File Offset: 0x00001498
		public virtual bool CanChangeType(object value, Type type, CultureInfo culture)
		{
			return false;
		}
	}
}
