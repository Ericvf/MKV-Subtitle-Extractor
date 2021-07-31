using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fex.Matroska
{
    public class Paragraph
    {
        public int Number { get; set; }
        public int StartFrame { get; set; }
        public int EndFrame { get; set; }
        public TimeCode StartTime { get; set; }
        public TimeCode EndTime { get; set; }
        public string Text { get; set; }

        // Methods
        public Paragraph()
        {
            this.StartTime = new TimeCode(TimeSpan.FromSeconds(0.0));
            this.EndTime = new TimeCode(TimeSpan.FromSeconds(0.0));
            this.Text = string.Empty;
        }

        public Paragraph(Paragraph paragraph)
        {
            this.Number = paragraph.Number;
            this.Text = paragraph.Text;
            this.StartTime = new TimeCode(paragraph.StartTime.TimeSpan);
            this.EndTime = new TimeCode(paragraph.EndTime.TimeSpan);
            this.StartFrame = paragraph.StartFrame;
            this.EndFrame = paragraph.EndFrame;
        }

        public Paragraph(TimeCode startTime, TimeCode endTime, string text)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Text = text;
        }

        public Paragraph(int startFrame, int endFrame, string text)
        {
            this.StartTime = new TimeCode(0, 0, 0, 0);
            this.EndTime = new TimeCode(0, 0, 0, 0);
            this.StartFrame = startFrame;
            this.EndFrame = endFrame;
            this.Text = text;
        }

        public Paragraph(string text, double startTotalMilliseconds, double endTotalMilliseconds)
        {
            this.StartTime = new TimeCode(TimeSpan.FromMilliseconds(startTotalMilliseconds));
            this.EndTime = new TimeCode(TimeSpan.FromMilliseconds(endTotalMilliseconds));
            this.Text = text;
        }

        internal void Adjust(double factor, double adjust)
        {
            double num = (this.StartTime.TimeSpan.TotalSeconds * factor) + adjust;
            this.StartTime.TimeSpan = TimeSpan.FromSeconds(num);
            num = (this.EndTime.TimeSpan.TotalSeconds * factor) + adjust;
            this.EndTime.TimeSpan = TimeSpan.FromSeconds(num);
        }

        public void CalculateFrameNumbersFromTimeCodes(double frameRate)
        {
            this.StartFrame = (int)((this.StartTime.TotalMilliseconds / 1000.0) * frameRate);
            this.EndFrame = (int)((this.EndTime.TotalMilliseconds / 1000.0) * frameRate);
        }

        public void CalculateTimeCodesFromFrameNumbers(double frameRate)
        {
            this.StartTime.TotalMilliseconds = this.StartFrame * (1000.0 / frameRate);
            this.EndTime.TotalMilliseconds = this.EndFrame * (1000.0 / frameRate);
        }

        public override string ToString()
        {
            return string.Concat(new object[] { this.StartTime, " --> ", this.EndTime, Environment.NewLine, this.Text });
        }

        // Properties
        public TimeCode Duration
        {
            get
            {
                TimeCode code = new TimeCode(this.EndTime.TimeSpan);
                code.AddTime(-this.StartTime.TotalMilliseconds);
                return code;
            }
        }
    }
}
