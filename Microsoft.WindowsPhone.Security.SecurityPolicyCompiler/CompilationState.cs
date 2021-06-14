using System;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000006 RID: 6
	public enum CompilationState
	{
		// Token: 0x04000099 RID: 153
		CompletedSuccessfully,
		// Token: 0x0400009A RID: 154
		UsageError,
		// Token: 0x0400009B RID: 155
		MacroFileLoadAndValidation,
		// Token: 0x0400009C RID: 156
		GlobalMacroDereferencing,
		// Token: 0x0400009D RID: 157
		PolicyFileLoadAndValidation,
		// Token: 0x0400009E RID: 158
		PolicyMacroDereferencing,
		// Token: 0x0400009F RID: 159
		PolicyElementsDataExtraction,
		// Token: 0x040000A0 RID: 160
		PolicyElementsCompilation,
		// Token: 0x040000A1 RID: 161
		CompilingCapability,
		// Token: 0x040000A2 RID: 162
		CompilingCapabilityRule,
		// Token: 0x040000A3 RID: 163
		SaveXmlFile,
		// Token: 0x040000A4 RID: 164
		Unknown,
		// Token: 0x040000A5 RID: 165
		PolicyFileAddHeaderAttributes,
		// Token: 0x040000A6 RID: 166
		PolicyFileAddElements
	}
}
