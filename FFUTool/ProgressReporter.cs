using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Microsoft.Windows.ImageTools
{
	// Token: 0x0200000A RID: 10
	public class ProgressReporter
	{
		// Token: 0x06000015 RID: 21 RVA: 0x00002873 File Offset: 0x00000A73
		public ProgressReporter()
		{
			this.width = Console.WindowWidth;
			this.stopwatch = Stopwatch.StartNew();
			this.summaryCount = 0;
			this.progressPoints = new Queue<Tuple<double, long>>();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000028A4 File Offset: 0x00000AA4
		public string CreateProgressDisplay(long position, long totalLength)
		{
			StringBuilder stringBuilder = new StringBuilder(2 * this.width);
			if (position == totalLength)
			{
				this.stopwatch.Stop();
				if (Interlocked.Add(ref this.summaryCount, 1) == 1)
				{
					double num = (double)totalLength / 1048576.0;
					stringBuilder.AppendFormat(Resources.TRANSFER_STATISTICS, num, this.stopwatch.Elapsed.TotalSeconds, num / this.stopwatch.Elapsed.TotalSeconds);
				}
				else
				{
					stringBuilder.Clear();
				}
			}
			else
			{
				double num2 = (double)position / (double)totalLength;
				if (num2 > 1.0)
				{
					num2 = 1.0;
				}
				int num3 = (int)Math.Floor(50.0 * num2);
				for (int i = 0; i < this.width; i++)
				{
					stringBuilder.Append('\b');
				}
				stringBuilder.Append('[');
				for (int j = 0; j < num3; j++)
				{
					stringBuilder.Append('=');
				}
				if (num3 < 50)
				{
					stringBuilder.Append('>');
					num3++;
				}
				for (int k = num3; k < 50; k++)
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append("]  ");
				stringBuilder.AppendFormat("{0:0.00%}", num2);
				stringBuilder.AppendFormat(" {0}", this.GetSpeedString(position));
				for (int l = stringBuilder.Length; l < 2 * this.width - 1; l++)
				{
					stringBuilder.Append(' ');
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002A40 File Offset: 0x00000C40
		private string GetSpeedString(long position)
		{
			string result = string.Empty;
			Tuple<double, long> item = new Tuple<double, long>(this.stopwatch.Elapsed.TotalSeconds, position);
			this.progressPoints.Enqueue(item);
			if (this.progressPoints.Count >= 8)
			{
				result = this.GetSpeedFromPoints(this.progressPoints.ToArray());
				this.progressPoints.Dequeue();
			}
			return result;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002AA8 File Offset: 0x00000CA8
		private string GetSpeedFromPoints(Tuple<double, long>[] points)
		{
			double num = 0.0;
			for (int i = 1; i < points.Length; i++)
			{
				double num2 = (double)(points[i].Item2 - points[i - 1].Item2) / 1048576.0;
				double num3 = points[i].Item1 - points[i - 1].Item1;
				num += num2 / num3 / (double)(points.Length - 1);
			}
			return string.Format(CultureInfo.CurrentCulture, Resources.FORMAT_SPEED, new object[]
			{
				num
			});
		}

		// Token: 0x0400002B RID: 43
		private int width;

		// Token: 0x0400002C RID: 44
		private Stopwatch stopwatch;

		// Token: 0x0400002D RID: 45
		private int summaryCount;

		// Token: 0x0400002E RID: 46
		private Queue<Tuple<double, long>> progressPoints;

		// Token: 0x0400002F RID: 47
		private const double OneMegabyte = 1048576.0;
	}
}
