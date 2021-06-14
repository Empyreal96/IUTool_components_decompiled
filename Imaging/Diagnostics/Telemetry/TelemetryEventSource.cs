using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Diagnostics.Tracing;

namespace Microsoft.Diagnostics.Telemetry
{
	// Token: 0x02000008 RID: 8
	internal class TelemetryEventSource : EventSource
	{
		// Token: 0x0600002F RID: 47 RVA: 0x000028D7 File Offset: 0x00000AD7
		public TelemetryEventSource(string eventSourceName) : base(eventSourceName, EventSourceSettings.EtwSelfDescribingEventFormat, TelemetryEventSource.telemetryTraits)
		{
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000028E6 File Offset: 0x00000AE6
		protected TelemetryEventSource() : base(EventSourceSettings.EtwSelfDescribingEventFormat, TelemetryEventSource.telemetryTraits)
		{
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000028F4 File Offset: 0x00000AF4
		public static EventSourceOptions TelemetryOptions()
		{
			return new EventSourceOptions
			{
				Keywords = (EventKeywords)35184372088832L
			};
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000291C File Offset: 0x00000B1C
		public static EventSourceOptions MeasuresOptions()
		{
			return new EventSourceOptions
			{
				Keywords = (EventKeywords)70368744177664L
			};
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002944 File Offset: 0x00000B44
		public static EventSourceOptions CriticalDataOptions()
		{
			return new EventSourceOptions
			{
				Keywords = (EventKeywords)140737488355328L
			};
		}

		// Token: 0x04000017 RID: 23
		public const EventKeywords Reserved44Keyword = (EventKeywords)17592186044416L;

		// Token: 0x04000018 RID: 24
		public const EventKeywords TelemetryKeyword = (EventKeywords)35184372088832L;

		// Token: 0x04000019 RID: 25
		public const EventKeywords MeasuresKeyword = (EventKeywords)70368744177664L;

		// Token: 0x0400001A RID: 26
		public const EventKeywords CriticalDataKeyword = (EventKeywords)140737488355328L;

		// Token: 0x0400001B RID: 27
		public const EventTags CostDeferredLatency = (EventTags)262144;

		// Token: 0x0400001C RID: 28
		public const EventTags CoreData = (EventTags)524288;

		// Token: 0x0400001D RID: 29
		public const EventTags InjectXToken = (EventTags)1048576;

		// Token: 0x0400001E RID: 30
		public const EventTags RealtimeLatency = (EventTags)2097152;

		// Token: 0x0400001F RID: 31
		public const EventTags NormalLatency = (EventTags)4194304;

		// Token: 0x04000020 RID: 32
		public const EventTags CriticalPersistence = (EventTags)8388608;

		// Token: 0x04000021 RID: 33
		public const EventTags NormalPersistence = (EventTags)16777216;

		// Token: 0x04000022 RID: 34
		public const EventTags DropPii = (EventTags)33554432;

		// Token: 0x04000023 RID: 35
		public const EventTags HashPii = (EventTags)67108864;

		// Token: 0x04000024 RID: 36
		public const EventTags MarkPii = (EventTags)134217728;

		// Token: 0x04000025 RID: 37
		public const EventFieldTags DropPiiField = (EventFieldTags)67108864;

		// Token: 0x04000026 RID: 38
		public const EventFieldTags HashPiiField = (EventFieldTags)134217728;

		// Token: 0x04000027 RID: 39
		private static readonly string[] telemetryTraits = new string[]
		{
			"ETW_GROUP",
			"{4f50731a-89cf-4782-b3e0-dce8c90476ba}"
		};

		// Token: 0x0200000D RID: 13
		private class EventDescriptionInfo<T>
		{
			// Token: 0x06000096 RID: 150 RVA: 0x00008434 File Offset: 0x00006634
			private EventDescriptionInfo(Type type)
			{
				MemberInfo typeFromHandle = typeof(T);
				EventDescriptionAttribute eventDescriptionAttribute = null;
				object[] customAttributes = typeFromHandle.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
				int num = 0;
				if (num < customAttributes.Length)
				{
					eventDescriptionAttribute = (customAttributes[num] as EventDescriptionAttribute);
				}
				if (eventDescriptionAttribute == null)
				{
					throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "WriteTelemetry requires the data type {0} to have an {1} attribute", new object[]
					{
						typeof(T).Name,
						typeof(EventDescriptionAttribute).Name
					}));
				}
				this.Options = new EventSourceOptions
				{
					Keywords = eventDescriptionAttribute.Keywords,
					Level = eventDescriptionAttribute.Level,
					Opcode = eventDescriptionAttribute.Opcode,
					Tags = eventDescriptionAttribute.Tags,
					ActivityOptions = eventDescriptionAttribute.ActivityOptions
				};
			}

			// Token: 0x06000097 RID: 151 RVA: 0x0000850C File Offset: 0x0000670C
			public static TelemetryEventSource.EventDescriptionInfo<T> GetInstance()
			{
				if (TelemetryEventSource.EventDescriptionInfo<T>.instance == null)
				{
					TelemetryEventSource.EventDescriptionInfo<T> value = new TelemetryEventSource.EventDescriptionInfo<T>(typeof(T));
					Interlocked.CompareExchange<TelemetryEventSource.EventDescriptionInfo<T>>(ref TelemetryEventSource.EventDescriptionInfo<T>.instance, value, null);
				}
				return TelemetryEventSource.EventDescriptionInfo<T>.instance;
			}

			// Token: 0x0400007D RID: 125
			private static TelemetryEventSource.EventDescriptionInfo<T> instance;

			// Token: 0x0400007E RID: 126
			public EventSourceOptions Options;
		}
	}
}
