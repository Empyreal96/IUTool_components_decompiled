using System;
using System.Runtime.Serialization;

namespace FFUComponents
{
	// Token: 0x02000019 RID: 25
	[Serializable]
	public class FFUFlashException : FFUException
	{
		// Token: 0x0600009C RID: 156 RVA: 0x00003C35 File Offset: 0x00001E35
		public FFUFlashException()
		{
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00003C3D File Offset: 0x00001E3D
		public FFUFlashException(string message) : base(message)
		{
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00003C46 File Offset: 0x00001E46
		public FFUFlashException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00003C50 File Offset: 0x00001E50
		public FFUFlashException(string deviceName, Guid deviceId, FFUFlashException.ErrorCode error, string message) : base(deviceName, deviceId, message)
		{
			this.Error = error;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00003C63 File Offset: 0x00001E63
		protected FFUFlashException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			info.AddValue("Error", this.Error);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00003C83 File Offset: 0x00001E83
		protected new virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Error", this.Error);
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00003CA3 File Offset: 0x00001EA3
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x00003CAB File Offset: 0x00001EAB
		public FFUFlashException.ErrorCode Error { get; private set; }

		// Token: 0x0200004F RID: 79
		public enum ErrorCode
		{
			// Token: 0x04000180 RID: 384
			None,
			// Token: 0x04000181 RID: 385
			FlashError = 2,
			// Token: 0x04000182 RID: 386
			InvalidStoreHeader = 8,
			// Token: 0x04000183 RID: 387
			DescriptorAllocationFailed,
			// Token: 0x04000184 RID: 388
			DescriptorReadFailed = 11,
			// Token: 0x04000185 RID: 389
			BlockReadFailed,
			// Token: 0x04000186 RID: 390
			BlockWriteFailed,
			// Token: 0x04000187 RID: 391
			CrcError,
			// Token: 0x04000188 RID: 392
			SecureHeaderReadFailed,
			// Token: 0x04000189 RID: 393
			InvalidSecureHeader,
			// Token: 0x0400018A RID: 394
			InsufficientSecurityPadding,
			// Token: 0x0400018B RID: 395
			InvalidImageHeader,
			// Token: 0x0400018C RID: 396
			InsufficientImagePadding,
			// Token: 0x0400018D RID: 397
			BufferingFailed,
			// Token: 0x0400018E RID: 398
			ExcessBlocks,
			// Token: 0x0400018F RID: 399
			InvalidPlatformId,
			// Token: 0x04000190 RID: 400
			HashCheckFailed,
			// Token: 0x04000191 RID: 401
			SignatureCheckFailed,
			// Token: 0x04000192 RID: 402
			DesyncFailed = 26,
			// Token: 0x04000193 RID: 403
			FailedBcdQuery,
			// Token: 0x04000194 RID: 404
			InvalidWriteDescriptors,
			// Token: 0x04000195 RID: 405
			AntiTheftCheckFailed,
			// Token: 0x04000196 RID: 406
			RemoveableMediaCheckFailed = 32,
			// Token: 0x04000197 RID: 407
			UseOptimizedSettingsFailed
		}
	}
}
