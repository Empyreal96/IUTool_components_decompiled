using System;

namespace FFUComponents
{
	// Token: 0x0200000B RID: 11
	internal class AsyncResult<TResult> : AsyncResultNoResult
	{
		// Token: 0x0600004D RID: 77 RVA: 0x000030D2 File Offset: 0x000012D2
		public AsyncResult(AsyncCallback asyncCallback, object state) : base(asyncCallback, state)
		{
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000030DC File Offset: 0x000012DC
		public void SetAsCompleted(TResult result, bool completedSynchronously)
		{
			this.result = result;
			base.SetAsCompleted(null, completedSynchronously);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000030ED File Offset: 0x000012ED
		public new TResult EndInvoke()
		{
			base.EndInvoke();
			return this.result;
		}

		// Token: 0x04000035 RID: 53
		private TResult result;
	}
}
