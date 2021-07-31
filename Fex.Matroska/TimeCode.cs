using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fex.Matroska
{
    public class TimeCode
    {
        // Fields
        private TimeSpan _time;

        // Methods
        public TimeCode(TimeSpan timeSpan)
        {
            this.TimeSpan = timeSpan;
        }

        public TimeCode(int hour, int minute, int seconds, int milliseconds)
        {
            this._time = new TimeSpan(0, hour, minute, seconds, milliseconds);
        }

        public void AddTime(double milliseconds)
        {
            this._time = TimeSpan.FromMilliseconds(this._time.TotalMilliseconds + milliseconds);
        }

        public void AddTime(long milliseconds)
        {
            this._time = TimeSpan.FromMilliseconds(this._time.TotalMilliseconds + milliseconds);
        }

        public void AddTime(TimeSpan timeSpan)
        {
            this._time = TimeSpan.FromMilliseconds(this._time.TotalMilliseconds + timeSpan.TotalMilliseconds);
        }

        public void AddTime(int hour, int minutes, int seconds, int milliseconds)
        {
            this.Hours += hour;
            this.Minutes += minutes;
            this.Seconds += seconds;
            this.Milliseconds += milliseconds;
        }

        public override string ToString()
        {
            return string.Format("{0:00}:{1:00}:{2:00},{3:000}", new object[] { this._time.Hours, this._time.Minutes, this._time.Seconds, this._time.Milliseconds });
        }

        // Properties
        public int Hours
        {
            get
            {
                return this._time.Hours;
            }
            set
            {
                this._time = new TimeSpan(0, value, this._time.Minutes, this._time.Seconds, this._time.Milliseconds);
            }
        }

        public int Milliseconds
        {
            get
            {
                return this._time.Milliseconds;
            }
            set
            {
                this._time = new TimeSpan(0, this._time.Hours, this._time.Minutes, this._time.Seconds, value);
            }
        }

        public int Minutes
        {
            get
            {
                return this._time.Minutes;
            }
            set
            {
                this._time = new TimeSpan(0, this._time.Hours, value, this._time.Seconds, this._time.Milliseconds);
            }
        }

        public int Seconds
        {
            get
            {
                return this._time.Seconds;
            }
            set
            {
                this._time = new TimeSpan(0, this._time.Hours, this._time.Minutes, value, this._time.Milliseconds);
            }
        }

        public TimeSpan TimeSpan
        {
            get
            {
                return this._time;
            }
            set
            {
                this._time = value;
            }
        }

        public double TotalMilliseconds
        {
            get
            {
                return this._time.TotalMilliseconds;
            }
            set
            {
                this._time = TimeSpan.FromMilliseconds(value);
            }
        }
    }



}
