using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fex.Matroska
{
    public class Subtitle
    {
        //    // Fields
        //    private SubtitleFormat _format;
        //    private List<HistoryItem> _history;
        //    private List<Paragraph> _paragraphs;
        //    private bool _wasLoadedWithFrameNumbers;

        //    // Methods
        //    public Subtitle()
        //    {
        //        this._paragraphs = new List<Paragraph>();
        //        this._history = new List<HistoryItem>();
        //        this.FileName = "Untitled";
        //    }

        //    public Subtitle(List<HistoryItem> historyItems) : this()
        //    {
        //        this._history = historyItems;
        //    }

        //    public Subtitle(Subtitle subtitle) : this()
        //    {
        //        foreach (Paragraph paragraph in subtitle.Paragraphs)
        //        {
        //            this._paragraphs.Add(new Paragraph(paragraph));
        //        }
        //        this._wasLoadedWithFrameNumbers = subtitle.WasLoadedWithFrameNumbers;
        //    }

        //    public void AddTimeToAllParagraphs(TimeSpan time)
        //    {
        //        foreach (Paragraph paragraph in this.Paragraphs)
        //        {
        //            paragraph.StartTime.AddTime(time);
        //            paragraph.EndTime.AddTime(time);
        //        }
        //    }

        //    internal void AdjustDisplayTimeUsingPercent(double percent, ListView.SelectedIndexCollection selectedIndexes)
        //    {
        //        for (int i = 0; i < this._paragraphs.Count; i++)
        //        {
        //            if ((selectedIndexes == null) || selectedIndexes.Contains(i))
        //            {
        //                double num2 = this._paragraphs[this._paragraphs.Count - 1].EndTime.TotalMilliseconds + 100000.0;
        //                if ((i + 1) < this._paragraphs.Count)
        //                {
        //                    num2 = this._paragraphs[i + 1].StartTime.TotalMilliseconds;
        //                }
        //                double totalMilliseconds = this._paragraphs[i].EndTime.TotalMilliseconds;
        //                totalMilliseconds = this._paragraphs[i].StartTime.TotalMilliseconds + (((totalMilliseconds - this._paragraphs[i].StartTime.TotalMilliseconds) * percent) / 100.0);
        //                if (totalMilliseconds > num2)
        //                {
        //                    totalMilliseconds = num2 - 1.0;
        //                }
        //                this._paragraphs[i].EndTime.TotalMilliseconds = totalMilliseconds;
        //            }
        //        }
        //    }

        //    internal void AdjustDisplayTimeUsingSeconds(double seconds, ListView.SelectedIndexCollection selectedIndexes)
        //    {
        //        for (int i = 0; i < this._paragraphs.Count; i++)
        //        {
        //            if ((selectedIndexes == null) || selectedIndexes.Contains(i))
        //            {
        //                double totalMilliseconds = this._paragraphs[this._paragraphs.Count - 1].EndTime.TotalMilliseconds + 100000.0;
        //                if ((i + 1) < this._paragraphs.Count)
        //                {
        //                    totalMilliseconds = this._paragraphs[i + 1].StartTime.TotalMilliseconds;
        //                }
        //                double num3 = this._paragraphs[i].EndTime.TotalMilliseconds + (seconds * 1000.0);
        //                if (num3 > totalMilliseconds)
        //                {
        //                    num3 = totalMilliseconds - 1.0;
        //                }
        //                this._paragraphs[i].EndTime.TotalMilliseconds = num3;
        //            }
        //        }
        //    }

        //    public bool CalculateFrameNumbersFromTimeCodes(double frameRate)
        //    {
        //        if (this._format == null)
        //        {
        //            return false;
        //        }
        //        if (this._format.IsFrameBased)
        //        {
        //            return false;
        //        }
        //        foreach (Paragraph paragraph in this.Paragraphs)
        //        {
        //            paragraph.CalculateFrameNumbersFromTimeCodes(frameRate);
        //        }
        //        return true;
        //    }

        //    public void CalculateFrameNumbersFromTimeCodesNoCheck(double frameRate)
        //    {
        //        foreach (Paragraph paragraph in this.Paragraphs)
        //        {
        //            paragraph.CalculateFrameNumbersFromTimeCodes(frameRate);
        //        }
        //    }

        //    public bool CalculateTimeCodesFromFrameNumbers(double frameRate)
        //    {
        //        if (this._format == null)
        //        {
        //            return false;
        //        }
        //        if (this._format.IsTimeBased)
        //        {
        //            return false;
        //        }
        //        foreach (Paragraph paragraph in this.Paragraphs)
        //        {
        //            paragraph.CalculateTimeCodesFromFrameNumbers(frameRate);
        //        }
        //        return true;
        //    }

        //    internal void ChangeFramerate(double oldFramerate, double newFramerate)
        //    {
        //        foreach (Paragraph paragraph in this.Paragraphs)
        //        {
        //            paragraph.CalculateFrameNumbersFromTimeCodes(oldFramerate);
        //            paragraph.CalculateTimeCodesFromFrameNumbers(newFramerate);
        //        }
        //    }

        //    internal Paragraph GetFirstParagraphByLineNumber(int number)
        //    {
        //        foreach (Paragraph paragraph in this._paragraphs)
        //        {
        //            if (paragraph.Number == number)
        //            {
        //                return paragraph;
        //            }
        //        }
        //        return null;
        //    }

        //    internal int GetIndex(Paragraph p)
        //    {
        //        return this._paragraphs.IndexOf(p);
        //    }

        //    public Paragraph GetParagraphOrDefault(int index)
        //    {
        //        if (((this._paragraphs != null) && (this._paragraphs.Count > index)) && (index >= 0))
        //        {
        //            return this._paragraphs[index];
        //        }
        //        return null;
        //    }

        //    public SubtitleFormat LoadSubtitle(string fileName, out Encoding encoding, Encoding useThisEncoding)
        //    {
        //        StreamReader reader;
        //        this.FileName = fileName;
        //        this._paragraphs = new List<Paragraph>();
        //        List<string> lines = new List<string>();
        //        if (useThisEncoding != null)
        //        {
        //            reader = new StreamReader(fileName, useThisEncoding);
        //        }
        //        else
        //        {
        //            reader = new StreamReader(fileName, Utilities.GetEncodingFromFile(fileName), true);
        //        }
        //        encoding = reader.CurrentEncoding;
        //        while (!reader.EndOfStream)
        //        {
        //            lines.Add(reader.ReadLine());
        //        }
        //        reader.Close();
        //        foreach (SubtitleFormat format in Utilities.SubtitleFormats)
        //        {
        //            if (format.IsMine(lines))
        //            {
        //                format.LoadSubtitle(this, lines);
        //                this._format = format;
        //                this._wasLoadedWithFrameNumbers = this._format.IsFrameBased;
        //                return format;
        //            }
        //        }
        //        return null;
        //    }

        //    public void MakeHistoryForUndo(string description, SubtitleFormat subtitleFormat, DateTime fileModified)
        //    {
        //        this._history.Add(new HistoryItem(this._history.Count, this, description, this.FileName, fileModified, subtitleFormat.FriendlyName));
        //    }

        //    public SubtitleFormat ReloadLoadSubtitle(List<string> lines)
        //    {
        //        foreach (SubtitleFormat format in Utilities.SubtitleFormats)
        //        {
        //            if (format.IsMine(lines))
        //            {
        //                format.LoadSubtitle(this, lines);
        //                this._format = format;
        //                return format;
        //            }
        //        }
        //        return null;
        //    }

        //    internal int RemoveEmptyLines()
        //    {
        //        int num = 0;
        //        if (this._paragraphs.Count > 0)
        //        {
        //            int number = this._paragraphs[0].Number;
        //            for (int i = this._paragraphs.Count - 1; i >= 0; i--)
        //            {
        //                Paragraph paragraph = this._paragraphs[i];
        //                if (paragraph.Text.Trim().Length == 0)
        //                {
        //                    this._paragraphs.RemoveAt(i);
        //                    num++;
        //                }
        //            }
        //            this.Renumber(number);
        //        }
        //        return num;
        //    }

        //    internal void Renumber(int startNumber)
        //    {
        //        int num = startNumber;
        //        foreach (Paragraph paragraph in this._paragraphs)
        //        {
        //            paragraph.Number = num;
        //            num++;
        //        }
        //    }

        //    public void Sort(SubtitleSortCriteria sortCriteria)
        //    {
        //        switch (sortCriteria)
        //        {
        //            case SubtitleSortCriteria.Number:
        //                this._paragraphs.Sort(delegate (Paragraph p1, Paragraph p2) {
        //                    return p1.Number.CompareTo(p2.Number);
        //                });
        //                return;

        //            case SubtitleSortCriteria.StartTime:
        //                this._paragraphs.Sort(delegate (Paragraph p1, Paragraph p2) {
        //                    return p1.StartTime.TotalMilliseconds.CompareTo(p2.StartTime.TotalMilliseconds);
        //                });
        //                return;

        //            case SubtitleSortCriteria.EndTime:
        //                this._paragraphs.Sort(delegate (Paragraph p1, Paragraph p2) {
        //                    return p1.EndTime.TotalMilliseconds.CompareTo(p2.EndTime.TotalMilliseconds);
        //                });
        //                return;

        //            case SubtitleSortCriteria.Duration:
        //                this._paragraphs.Sort(delegate (Paragraph p1, Paragraph p2) {
        //                    return p1.Duration.TotalMilliseconds.CompareTo(p2.Duration.TotalMilliseconds);
        //                });
        //                return;

        //            case SubtitleSortCriteria.Text:
        //                this._paragraphs.Sort(delegate (Paragraph p1, Paragraph p2) {
        //                    return p1.Text.CompareTo(p2.Text);
        //                });
        //                return;

        //            case SubtitleSortCriteria.TextMaxLineLength:
        //                this._paragraphs.Sort(delegate (Paragraph p1, Paragraph p2) {
        //                    return Utilities.GetMaxLineLength(p1.Text).CompareTo(Utilities.GetMaxLineLength(p2.Text));
        //                });
        //                return;

        //            case SubtitleSortCriteria.TextTotalLength:
        //                this._paragraphs.Sort(delegate (Paragraph p1, Paragraph p2) {
        //                    return p1.Text.Length.CompareTo(p2.Text.Length);
        //                });
        //                return;

        //            case SubtitleSortCriteria.TextNumberOfLines:
        //                this._paragraphs.Sort(delegate (Paragraph p1, Paragraph p2) {
        //                    return p1.NumberOfLines.CompareTo(p2.NumberOfLines);
        //                });
        //                return;
        //        }
        //    }

        //    internal string ToText(SubtitleFormat format)
        //    {
        //        return format.ToText(this, Path.GetFileNameWithoutExtension(this.FileName));
        //    }

        //    public string UndoHistory(int index, out string subtitleFormatFriendlyName, out DateTime fileModified)
        //    {
        //        this._paragraphs.Clear();
        //        foreach (Paragraph paragraph in this._history[index].Subtitle.Paragraphs)
        //        {
        //            this._paragraphs.Add(new Paragraph(paragraph));
        //        }
        //        subtitleFormatFriendlyName = this._history[index].SubtitleFormatFriendlyName;
        //        this.FileName = this._history[index].FileName;
        //        fileModified = this._history[index].FileModified;
        //        return this.FileName;
        //    }

        //    // Properties
        //    public bool CanUndo
        //    {
        //        get
        //        {
        //            return (this._history.Count > 0);
        //        }
        //    }

        //    public string FileName { get; set; }

        //    public string Header { get; set; }

        //    public List<HistoryItem> HistoryItems
        //    {
        //        get
        //        {
        //            return this._history;
        //        }
        //    }

        //    public SubtitleFormat OriginalFormat { get; set; }

        private List<Paragraph> _paragraphs = new List<Paragraph>();
        public List<Paragraph> Paragraphs
        {
            get
            {
                return this._paragraphs;
            }
        }

        //    public bool WasLoadedWithFrameNumbers
        //    {
        //        get
        //        {
        //            return this._wasLoadedWithFrameNumbers;
        //        }
        //        set
        //        {
        //            this._wasLoadedWithFrameNumbers = value;
        //        }
        //    }
        //}


        public string ToSrtFormat()
        {
            StringBuilder srtOutput = new StringBuilder();
            int index = 0;
            foreach (var p in this._paragraphs)
            {
                srtOutput.AppendLine((++index).ToString());
                srtOutput.AppendLine(p.ToString());
                srtOutput.AppendLine();
            }

            return srtOutput.ToString();
        }
    }
}
