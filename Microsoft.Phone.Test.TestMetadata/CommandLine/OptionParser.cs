using System;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000025 RID: 37
	public abstract class OptionParser
	{
		// Token: 0x060000DD RID: 221
		public abstract string ParseCommandName(string[] arguments);

		// Token: 0x060000DE RID: 222
		public abstract CommandOptionCollection Parse(string[] arguments, OptionSpecificationCollection optionSpecifications);

		// Token: 0x060000DF RID: 223
		public abstract void SetOptionProperty(object command, OptionSpecification optionSpecification, CommandOption commandOption);
	}
}
