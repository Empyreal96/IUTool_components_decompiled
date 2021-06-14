using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000032 RID: 50
	internal class MergeErrors
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000210 RID: 528 RVA: 0x00008EC2 File Offset: 0x000070C2
		public bool HasError
		{
			get
			{
				return this._errors.Count != 0;
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00008ED2 File Offset: 0x000070D2
		public void Add(string msg)
		{
			this._errors.Add(msg);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00008EE0 File Offset: 0x000070E0
		public void Add(string format, params object[] args)
		{
			this._errors.Add(string.Format(format, args));
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00008EF4 File Offset: 0x000070F4
		public void CheckResult()
		{
			if (this.HasError)
			{
				foreach (string message in this._errors)
				{
					LogUtil.Error(message);
				}
				throw new PackageException("Error occured during merging");
			}
		}

		// Token: 0x06000214 RID: 532 RVA: 0x00008F58 File Offset: 0x00007158
		public static void Clear()
		{
			Dictionary<int, MergeErrors> instances = MergeErrors._instances;
			lock (instances)
			{
				MergeErrors._instances.Remove(Thread.CurrentThread.ManagedThreadId);
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000215 RID: 533 RVA: 0x00008FA8 File Offset: 0x000071A8
		public static MergeErrors Instance
		{
			get
			{
				Dictionary<int, MergeErrors> instances = MergeErrors._instances;
				MergeErrors result;
				lock (instances)
				{
					MergeErrors mergeErrors = null;
					if (!MergeErrors._instances.TryGetValue(Thread.CurrentThread.ManagedThreadId, out mergeErrors))
					{
						mergeErrors = new MergeErrors();
						MergeErrors._instances.Add(Thread.CurrentThread.ManagedThreadId, mergeErrors);
					}
					result = mergeErrors;
				}
				return result;
			}
		}

		// Token: 0x040000DD RID: 221
		private static Dictionary<int, MergeErrors> _instances = new Dictionary<int, MergeErrors>();

		// Token: 0x040000DE RID: 222
		private List<string> _errors = new List<string>();
	}
}
