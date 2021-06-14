using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection.Mock
{
	// Token: 0x02000006 RID: 6
	[ComVisible(true)]
	internal abstract class ConstructorInfo : MethodBase
	{
		// Token: 0x060000B3 RID: 179
		public abstract object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);
	}
}
