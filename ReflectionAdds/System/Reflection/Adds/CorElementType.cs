using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Reflection.Adds
{
	// Token: 0x02000006 RID: 6
	[SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cor")]
	public enum CorElementType
	{
		// Token: 0x04000006 RID: 6
		Array = 20,
		// Token: 0x04000007 RID: 7
		Bool = 2,
		// Token: 0x04000008 RID: 8
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Byref")]
		Byref = 16,
		// Token: 0x04000009 RID: 9
		Char = 3,
		// Token: 0x0400000A RID: 10
		Class = 18,
		// Token: 0x0400000B RID: 11
		CModOpt = 32,
		// Token: 0x0400000C RID: 12
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Reqd")]
		CModReqd = 31,
		// Token: 0x0400000D RID: 13
		End = 0,
		// Token: 0x0400000E RID: 14
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Fn")]
		FnPtr = 27,
		// Token: 0x0400000F RID: 15
		IntPtr = 24,
		// Token: 0x04000010 RID: 16
		SByte = 4,
		// Token: 0x04000011 RID: 17
		Short = 6,
		// Token: 0x04000012 RID: 18
		Int = 8,
		// Token: 0x04000013 RID: 19
		Long = 10,
		// Token: 0x04000014 RID: 20
		Internal = 33,
		// Token: 0x04000015 RID: 21
		Max,
		// Token: 0x04000016 RID: 22
		Modifier = 64,
		// Token: 0x04000017 RID: 23
		Object = 28,
		// Token: 0x04000018 RID: 24
		Pinned = 69,
		// Token: 0x04000019 RID: 25
		Pointer = 15,
		// Token: 0x0400001A RID: 26
		Float = 12,
		// Token: 0x0400001B RID: 27
		Double,
		// Token: 0x0400001C RID: 28
		Sentinel = 65,
		// Token: 0x0400001D RID: 29
		String = 14,
		// Token: 0x0400001E RID: 30
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sz")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Sz")]
		SzArray = 29,
		// Token: 0x0400001F RID: 31
		TypedByRef = 22,
		// Token: 0x04000020 RID: 32
		UIntPtr = 25,
		// Token: 0x04000021 RID: 33
		Byte = 5,
		// Token: 0x04000022 RID: 34
		UShort = 7,
		// Token: 0x04000023 RID: 35
		UInt = 9,
		// Token: 0x04000024 RID: 36
		ULong = 11,
		// Token: 0x04000025 RID: 37
		ValueType = 17,
		// Token: 0x04000026 RID: 38
		Void = 1,
		// Token: 0x04000027 RID: 39
		TypeVar = 19,
		// Token: 0x04000028 RID: 40
		MethodVar = 30,
		// Token: 0x04000029 RID: 41
		GenericInstantiation = 21,
		// Token: 0x0400002A RID: 42
		Type = 80,
		// Token: 0x0400002B RID: 43
		Enum = 85
	}
}
