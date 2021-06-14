using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Reflection.Adds
{
	// Token: 0x02000007 RID: 7
	internal static class ElementTypeUtility
	{
		// Token: 0x0600004C RID: 76 RVA: 0x00002998 File Offset: 0x00000B98
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public static string GetNameForPrimitive(CorElementType value)
		{
			switch (value)
			{
			case CorElementType.Void:
				return "System.Void";
			case CorElementType.Bool:
				return "System.Boolean";
			case CorElementType.Char:
				return "System.Char";
			case CorElementType.SByte:
				return "System.SByte";
			case CorElementType.Byte:
				return "System.Byte";
			case CorElementType.Short:
				return "System.Int16";
			case CorElementType.UShort:
				return "System.UInt16";
			case CorElementType.Int:
				return "System.Int32";
			case CorElementType.UInt:
				return "System.UInt32";
			case CorElementType.Long:
				return "System.Int64";
			case CorElementType.ULong:
				return "System.UInt64";
			case CorElementType.Float:
				return "System.Single";
			case CorElementType.Double:
				return "System.Double";
			case CorElementType.String:
				return "System.String";
			case CorElementType.Pointer:
			case CorElementType.Byref:
			case CorElementType.ValueType:
			case CorElementType.Class:
			case CorElementType.TypeVar:
			case CorElementType.Array:
			case CorElementType.GenericInstantiation:
			case CorElementType.TypedByRef:
			case (CorElementType)23:
			case (CorElementType)26:
			case CorElementType.FnPtr:
				break;
			case CorElementType.IntPtr:
				return "System.IntPtr";
			case CorElementType.UIntPtr:
				return "System.UIntPtr";
			case CorElementType.Object:
				return "System.Object";
			default:
				if (value == CorElementType.Type)
				{
					return "System.Type";
				}
				break;
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.IllegalElementType, new object[]
			{
				value
			}));
		}
	}
}
