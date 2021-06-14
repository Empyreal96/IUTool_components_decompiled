using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.Reflection.Adds
{
	// Token: 0x02000009 RID: 9
	[SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
	[Serializable]
	internal class UnresolvedAssemblyException : Exception
	{
		// Token: 0x0600005E RID: 94 RVA: 0x00002F9F File Offset: 0x0000119F
		public UnresolvedAssemblyException(string message) : base(message)
		{
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00002FAA File Offset: 0x000011AA
		public UnresolvedAssemblyException()
		{
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00002FB4 File Offset: 0x000011B4
		public UnresolvedAssemblyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002FC0 File Offset: 0x000011C0
		protected UnresolvedAssemblyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
