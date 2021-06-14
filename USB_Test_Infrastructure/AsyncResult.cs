using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000002 RID: 2
	internal class AsyncResult<TResult> : AsyncResultNoResult
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public AsyncResult(AsyncCallback asyncCallback, object state) : base(asyncCallback, state)
		{
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000205A File Offset: 0x0000025A
		public void SetAsCompleted(TResult result, bool completedSynchronously)
		{
			this.result = result;
			base.SetAsCompleted(null, completedSynchronously);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000206B File Offset: 0x0000026B
		public new TResult EndInvoke()
		{
			base.EndInvoke();
			return this.result;
		}

		// Token: 0x04000001 RID: 1
		private TResult result;
	}
}
