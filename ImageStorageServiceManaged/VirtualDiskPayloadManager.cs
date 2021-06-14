using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000042 RID: 66
	internal class VirtualDiskPayloadManager : IDisposable
	{
		// Token: 0x0600028D RID: 653 RVA: 0x0000BAB0 File Offset: 0x00009CB0
		public VirtualDiskPayloadManager(IULogger logger, ushort storeHeaderVersion, ushort numOfStores)
		{
			this._logger = logger;
			this._storeHeaderVersion = storeHeaderVersion;
			this._numOfStores = numOfStores;
			this._generators = new List<Tuple<VirtualDiskPayloadGenerator, ImageStorage>>();
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0000BAD8 File Offset: 0x00009CD8
		public void AddStore(ImageStorage storage)
		{
			if (this._storeHeaderVersion < 2 && this._generators.Count > 1)
			{
				throw new ImageStorageException(string.Format("{0}: Cannot add more than one store to a FFU using v1 store header.", MethodBase.GetCurrentMethod().Name));
			}
			VirtualDiskPayloadGenerator item = new VirtualDiskPayloadGenerator(this._logger, ImageConstants.PAYLOAD_BLOCK_SIZE, storage, this._storeHeaderVersion, this._numOfStores, (ushort)(this._generators.Count + 1));
			this._generators.Add(new Tuple<VirtualDiskPayloadGenerator, ImageStorage>(item, storage));
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000BB54 File Offset: 0x00009D54
		public void Write(IPayloadWrapper payloadWrapper)
		{
			long num = 0L;
			foreach (Tuple<VirtualDiskPayloadGenerator, ImageStorage> tuple in this._generators)
			{
				VirtualDiskPayloadGenerator item = tuple.Item1;
				ImageStorage item2 = tuple.Item2;
				item.GenerateStorePayload(item2);
				num += item.TotalSize;
			}
			payloadWrapper.InitializeWrapper(num);
			this._generators.ForEach(delegate(Tuple<VirtualDiskPayloadGenerator, ImageStorage> t)
			{
				t.Item1.WriteMetadata(payloadWrapper);
			});
			this._generators.ForEach(delegate(Tuple<VirtualDiskPayloadGenerator, ImageStorage> t)
			{
				t.Item1.WriteStorePayload(payloadWrapper);
			});
			payloadWrapper.FinalizeWrapper();
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000BC14 File Offset: 0x00009E14
		~VirtualDiskPayloadManager()
		{
			this.Dispose(false);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000BC44 File Offset: 0x00009E44
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000BC54 File Offset: 0x00009E54
		protected virtual void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				this._generators.ForEach(delegate(Tuple<VirtualDiskPayloadGenerator, ImageStorage> t)
				{
					t.Item1.Dispose();
				});
				this._generators.Clear();
				this._generators = null;
			}
			this._alreadyDisposed = true;
		}

		// Token: 0x0400019B RID: 411
		private IULogger _logger;

		// Token: 0x0400019C RID: 412
		private List<Tuple<VirtualDiskPayloadGenerator, ImageStorage>> _generators;

		// Token: 0x0400019D RID: 413
		private ushort _storeHeaderVersion;

		// Token: 0x0400019E RID: 414
		private ushort _numOfStores;

		// Token: 0x0400019F RID: 415
		private bool _alreadyDisposed;
	}
}
