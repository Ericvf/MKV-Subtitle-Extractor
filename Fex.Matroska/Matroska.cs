using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Fex.Matroska
{
    public class Matroska
    {
        // Fields
        private double _durationInMilliseconds;
        private double _frameRate;
        private int _pixelHeight;
        private int _pixelWidth;
        private List<MatroskaSubtitleInfo> _subtitleList;
        private Subtitle _subtitleRip = new Subtitle();
        private int _subtitleRipTrackNumber;
        private string _videoCodecId;
        private FileStream f;

        // Methods
        private void AnalyzeMatroskaBlock(long clusterTimeCode)
        {
            long matroskaVariableSizeUnsignedInt = 0L;
            byte b = (byte)this.f.ReadByte();
            if (b == 0xa1)
            {
                b = (byte)this.f.ReadByte();
                int matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                long matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                long offset = this.f.Position + matroskaDataSize;
                b = (byte)this.f.ReadByte();
                int num6 = GetMatroskaVariableIntLength(b);
                long num7 = this.GetMatroskaDataSize((long)num6, b);
                short num8 = this.GetInt16();
                if (num7 == this._subtitleRipTrackNumber)
                {
                    b = (byte)((b + 1) - 1);
                }
                byte num9 = (byte)this.f.ReadByte();
                byte num10 = 0;
                switch ((num9 & 6))
                {
                    case 2:
                        num10 = (byte)this.f.ReadByte();
                        num10 = (byte)(num10 + 1);
                        break;

                    case 4:
                        num10 = (byte)this.f.ReadByte();
                        num10 = (byte)(num10 + 1);
                        for (int i = 1; i <= num10; i++)
                        {
                            b = (byte)this.f.ReadByte();
                        }
                        break;

                    case 6:
                        num10 = (byte)this.f.ReadByte();
                        num10 = (byte)(num10 + 1);
                        break;
                }
                string text = string.Empty;
                if (this.f.Position < offset)
                {
                    text = this.GetMatroskaString(offset - this.f.Position);
                }
                this.f.Seek(offset, SeekOrigin.Begin);
                b = (byte)this.f.ReadByte();
                if (b == 0x9b)
                {
                    b = (byte)this.f.ReadByte();
                    matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                    matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                    matroskaVariableSizeUnsignedInt = this.GetMatroskaVariableSizeUnsignedInt(matroskaDataSize);
                }
                if (num7 == this._subtitleRipTrackNumber)
                {
                    this._subtitleRip.Paragraphs.Add(new Paragraph(text, (double)(num8 + clusterTimeCode), (double)((num8 + clusterTimeCode) + matroskaVariableSizeUnsignedInt)));
                }
            }
        }

        private void AnalyzeMatroskaCluster()
        {
            bool flag = false;
            long clusterTimeCode = 0L;
            while ((this.f.Position < this.f.Length) && !flag)
            {
                uint matroskaClusterId = this.GetMatroskaClusterId();
                if (matroskaClusterId == 0)
                {
                    flag = true;
                }
                else
                {
                    long num5;
                    byte b = (byte)this.f.ReadByte();
                    int matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                    long matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                    switch (matroskaClusterId)
                    {
                        case 0xe7:
                            {
                                num5 = this.f.Position + matroskaDataSize;
                                clusterTimeCode = this.GetMatroskaVariableSizeUnsignedInt(matroskaDataSize);
                                this.f.Seek(num5, SeekOrigin.Begin);
                                continue;
                            }
                        case 160:
                            {
                                num5 = this.f.Position + matroskaDataSize;
                                this.AnalyzeMatroskaBlock(clusterTimeCode);
                                this.f.Seek(num5, SeekOrigin.Begin);
                                continue;
                            }
                    }
                    this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                }
            }
        }

        private void AnalyzeMatroskaSegmentInformation(long endPosition)
        {
            bool flag = false;
            long num6 = 0L;
            double num7 = 0.0;
            while ((this.f.Position < this.f.Length) && !flag)
            {
                uint matroskaSegmentId = this.GetMatroskaSegmentId();
                if (matroskaSegmentId == 0)
                {
                    flag = true;
                }
                else
                {
                    long num5;
                    byte b = (byte)this.f.ReadByte();
                    int matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                    long matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                    switch (matroskaSegmentId)
                    {
                        case 0x2ad7b1:
                            {
                                num5 = this.f.Position + matroskaDataSize;
                                b = (byte)this.f.ReadByte();
                                num6 = this.GetMatroskaDataSize(matroskaDataSize, b);
                                this.f.Seek(num5, SeekOrigin.Begin);
                                continue;
                            }
                        case 0x4489:
                            {
                                num5 = this.f.Position + matroskaDataSize;
                                if (matroskaDataSize == 4L)
                                {
                                    num7 = this.GetFloat32();
                                }
                                else
                                {
                                    num7 = this.GetFloat64();
                                }
                                this.f.Seek(num5, SeekOrigin.Begin);
                                continue;
                            }
                    }
                    this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                }
            }
            if ((num6 > 0L) && (num7 > 0.0))
            {
                this._durationInMilliseconds = (num7 / ((double)num6)) * 1000000.0;
            }
            else if (num7 > 0.0)
            {
                this._durationInMilliseconds = num7;
            }
        }

        private void AnalyzeMatroskaTrackEntry()
        {
            bool flag = false;
            long num5 = 0L;
            bool flag2 = false;
            bool flag3 = false;
            long num7 = 0L;
            string matroskaString = string.Empty;
            string str2 = string.Empty;
            string str3 = string.Empty;
            string str4 = string.Empty;
            string str5 = string.Empty;
            while ((this.f.Position < this.f.Length) && !flag)
            {
                uint matroskaTrackEntryId = this.GetMatroskaTrackEntryId();
                if (matroskaTrackEntryId == 0)
                {
                    flag = true;
                }
                else
                {
                    long num6;
                    byte b = (byte)this.f.ReadByte();
                    int matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                    long matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                    switch (matroskaTrackEntryId)
                    {
                        case 0x23e383:
                            {
                                num6 = this.f.Position + matroskaDataSize;
                                b = (byte)this.f.ReadByte();
                                num5 = this.GetMatroskaDataSize(matroskaDataSize, b);
                                this.f.Seek(num6, SeekOrigin.Begin);
                                continue;
                            }
                        case 0xe0:
                            {
                                num6 = this.f.Position + matroskaDataSize;
                                this.AnalyzeMatroskaTrackVideo(num6);
                                this.f.Seek(num6, SeekOrigin.Begin);
                                flag2 = true;
                                continue;
                            }
                        case 0xd7:
                            {
                                num6 = this.f.Position + matroskaDataSize;
                                if (matroskaDataSize == 1L)
                                {
                                    num7 = (byte)this.f.ReadByte();
                                }
                                this.f.Seek(num6, SeekOrigin.Begin);
                                continue;
                            }
                        case 0x536e:
                            {
                                num6 = this.f.Position + matroskaDataSize;
                                matroskaString = this.GetMatroskaString(matroskaDataSize);
                                this.f.Seek(num6, SeekOrigin.Begin);
                                continue;
                            }
                        case 0x22b59c:
                            {
                                num6 = this.f.Position + matroskaDataSize;
                                str2 = this.GetMatroskaString(matroskaDataSize);
                                this.f.Seek(num6, SeekOrigin.Begin);
                                continue;
                            }
                        case 0x86:
                            {
                                num6 = this.f.Position + matroskaDataSize;
                                str3 = this.GetMatroskaString(matroskaDataSize);
                                this.f.Seek(num6, SeekOrigin.Begin);
                                continue;
                            }
                        case 0x83:
                            {
                                num6 = this.f.Position + matroskaDataSize;
                                if (matroskaDataSize == 1L)
                                {
                                    byte num8 = (byte)this.f.ReadByte();
                                    if (num8 == 0x11)
                                    {
                                        flag3 = true;
                                    }
                                }
                                this.f.Seek(num6, SeekOrigin.Begin);
                                continue;
                            }
                        case 0x63a2:
                            {
                                num6 = this.f.Position + matroskaDataSize;
                                str4 = this.GetMatroskaString(matroskaDataSize);
                                if (str4.Length > 20)
                                {
                                    str5 = str4.Substring(0x10, 4);
                                }
                                this.f.Seek(num6, SeekOrigin.Begin);
                                continue;
                            }
                    }
                    this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                }
            }
            if (flag2)
            {
                if (num5 > 0L)
                {
                    this._frameRate = 1.0 / (((double)num5) / 1000000000.0);
                }
                this._videoCodecId = str3 + " " + str5.Trim();
            }
            else if (flag3)
            {
                MatroskaSubtitleInfo item = new MatroskaSubtitleInfo();
                item.Name = matroskaString;
                item.TrackNumber = (int)num7;
                item.CodecId = str3;
                item.Language = str2;
                item.CodecPrivate = str4;
                this._subtitleList.Add(item);
            }
        }

        private void AnalyzeMatroskaTracks()
        {
            bool flag = false;
            this._subtitleList = new List<MatroskaSubtitleInfo>();
            while ((this.f.Position < this.f.Length) && !flag)
            {
                uint matroskaTracksId = this.GetMatroskaTracksId();
                if (matroskaTracksId == 0)
                {
                    flag = true;
                }
                else
                {
                    byte b = (byte)this.f.ReadByte();
                    int matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                    long matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                    if (matroskaTracksId == 0xae)
                    {
                        long offset = this.f.Position + matroskaDataSize;
                        this.AnalyzeMatroskaTrackEntry();
                        this.f.Seek(offset, SeekOrigin.Begin);
                        continue;
                    }
                    this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                }
            }
        }

        private void AnalyzeMatroskaTrackVideo(long endPosition)
        {
            bool flag = false;
            while ((this.f.Position < this.f.Length) && !flag)
            {
                uint matroskaTrackVideoId = this.GetMatroskaTrackVideoId();
                if (matroskaTrackVideoId == 0)
                {
                    flag = true;
                }
                else
                {
                    long num5;
                    byte b = (byte)this.f.ReadByte();
                    int matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                    long matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                    switch (matroskaTrackVideoId)
                    {
                        case 0xb0:
                            {
                                num5 = this.f.Position + matroskaDataSize;
                                b = (byte)this.f.ReadByte();
                                matroskaDataSize = this.GetMatroskaDataSize(matroskaDataSize, b);
                                this._pixelWidth = (int)matroskaDataSize;
                                this.f.Seek(num5, SeekOrigin.Begin);
                                continue;
                            }
                        case 0xba:
                            {
                                num5 = this.f.Position + matroskaDataSize;
                                b = (byte)this.f.ReadByte();
                                matroskaDataSize = this.GetMatroskaDataSize(matroskaDataSize, b);
                                this._pixelHeight = (int)matroskaDataSize;
                                this.f.Seek(num5, SeekOrigin.Begin);
                                continue;
                            }
                    }
                    this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                }
            }
        }

        private double GetFloat32()
        {
            FloatLayout32 layout = new FloatLayout32();
            layout.B4 = (byte)this.f.ReadByte();
            layout.B3 = (byte)this.f.ReadByte();
            layout.B2 = (byte)this.f.ReadByte();
            layout.B1 = (byte)this.f.ReadByte();
            return (double)layout.FloatData32;
        }

        private double GetFloat64()
        {
            FloatLayout64 layout = new FloatLayout64();
            layout.B8 = (byte)this.f.ReadByte();
            layout.B7 = (byte)this.f.ReadByte();
            layout.B6 = (byte)this.f.ReadByte();
            layout.B5 = (byte)this.f.ReadByte();
            layout.B4 = (byte)this.f.ReadByte();
            layout.B3 = (byte)this.f.ReadByte();
            layout.B2 = (byte)this.f.ReadByte();
            layout.B1 = (byte)this.f.ReadByte();
            return layout.FloatData64;
        }

        private short GetInt16()
        {
            ByteLayout16 layout = new ByteLayout16();
            layout.B2 = (byte)this.f.ReadByte();
            layout.B1 = (byte)this.f.ReadByte();
            return layout.IntData16;
        }

        private uint GetMatroskaClusterId()
        {
            uint num = (byte)this.f.ReadByte();
            switch (num)
            {
                case 0xe7:
                case 0xa7:
                case 0xab:
                case 160:
                case 0xa1:
                case 0xa2:
                case 0xa6:
                case 0xee:
                case 0xa5:
                case 0x9b:
                case 250:
                case 0xfb:
                case 0xfd:
                case 0xa4:
                case 0x8e:
               // case 0x8e:
                case 0xcc:
                case 0xcd:
                case 0xcb:
                case 0xce:
                case 0xcf:
                case 0xa3:
                    return num;
            }
            num = (num * 0x100) + ((byte)this.f.ReadByte());
            if (((num != 0x5854) && (num != 0x58d7)) && (num != 0x75a1))
            {
                return 0;
            }
            return num;
        }

        private long GetMatroskaDataSize(long sizeOfSize, byte firstByte)
        {
            byte num2;
            byte num3;
            byte num4;
            byte num5;
            byte num6;
            byte num7;
            long num8 = 0L;
            if (sizeOfSize == 8L)
            {
                byte num = (byte)this.f.ReadByte();
                num2 = (byte)this.f.ReadByte();
                num3 = (byte)this.f.ReadByte();
                num4 = (byte)this.f.ReadByte();
                num5 = (byte)this.f.ReadByte();
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return ((((((num7 + (num6 * 0x100)) + (num5 * 0x10000)) + (num4 * 0x1000000)) + (num3 * 0x100000000L)) + (num2 * 0x10000000000L)) + (num * 0x1000000000000L));
            }
            if (sizeOfSize == 7L)
            {
                firstByte = (byte)(firstByte & 1);
                num2 = (byte)this.f.ReadByte();
                num3 = (byte)this.f.ReadByte();
                num4 = (byte)this.f.ReadByte();
                num5 = (byte)this.f.ReadByte();
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return ((((((num7 + (num6 * 0x100)) + (num5 * 0x10000)) + (num4 * 0x1000000)) + (num3 * 0x100000000L)) + (num2 * 0x10000000000L)) + (firstByte * 0x1000000000000L));
            }
            if (sizeOfSize == 6L)
            {
                firstByte = (byte)(firstByte & 3);
                num3 = (byte)this.f.ReadByte();
                num4 = (byte)this.f.ReadByte();
                num5 = (byte)this.f.ReadByte();
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return (((((num7 + (num6 * 0x100)) + (num5 * 0x10000)) + (num4 * 0x1000000)) + (num3 * 0x100000000L)) + (firstByte * 0x10000000000L));
            }
            if (sizeOfSize == 5L)
            {
                firstByte = (byte)(firstByte & 7);
                num4 = (byte)this.f.ReadByte();
                num5 = (byte)this.f.ReadByte();
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return ((((num7 + (num6 * 0x100)) + (num5 * 0x10000)) + (num4 * 0x1000000)) + (firstByte * 0x100000000L));
            }
            if (sizeOfSize == 4L)
            {
                firstByte = (byte)(firstByte & 15);
                num5 = (byte)this.f.ReadByte();
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return (long)(((num7 + (num6 * 0x100)) + (num5 * 0x10000)) + (firstByte * 0x1000000));
            }
            if (sizeOfSize == 3L)
            {
                firstByte = (byte)(firstByte & 0x1f);
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return (long)((num7 + (num6 * 0x100)) + (firstByte * 0x10000));
            }
            if (sizeOfSize == 2L)
            {
                firstByte = (byte)(firstByte & 0x3f);
                num7 = (byte)this.f.ReadByte();
                return (long)(num7 + (firstByte * 0x100));
            }
            if (sizeOfSize == 1L)
            {
                num8 = firstByte & 0x7f;
            }
            return num8;
        }

        private uint GetMatroskaId()
        {
            byte firstByte = (byte)this.f.ReadByte();
            switch (firstByte)
            {
                case 0xec:
                case 0xbf:
                    return firstByte;
            }
            uint num2 = this.GetUInt32(firstByte);
            if ((((num2 != 0x1a45dfa3) && (num2 != 0x18538067)) && ((num2 != 0x114d9b74) && (num2 != 0x1549a966))) && ((((num2 != 0x1654ae6b) && (num2 != 0x1f43b675)) && ((num2 != 0x1c53bb6b) && (num2 != 0x1941a469))) && ((num2 != 0x1043a770) && (num2 != 0x1254c367))))
            {
                return 0;
            }
            return num2;
        }

        public void GetMatroskaInfo(string fileName, ref bool isValid, ref bool hasConstantFrameRate, ref double frameRate, ref int pixelWidth, ref int pixelHeight, ref double millisecsDuration, ref string videoCodec)
        {
            this._durationInMilliseconds = 0.0;
            this.f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (this.GetMatroskaId() != 0x1a45dfa3)
            {
                isValid = false;
            }
            else
            {
                isValid = true;
                byte b = (byte)this.f.ReadByte();
                int matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                long matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                bool flag = false;
                for (bool flag2 = false; !flag2 && !flag; flag2 = this.f.Position >= this.f.Length)
                {
                    uint matroskaId = this.GetMatroskaId();
                    if (matroskaId == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        long num5;
                        b = (byte)this.f.ReadByte();
                        matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                        matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                        switch (matroskaId)
                        {
                            case 0x1549a966:
                                num5 = this.f.Position + matroskaDataSize;
                                this.AnalyzeMatroskaSegmentInformation(num5);
                                this.f.Seek(num5, SeekOrigin.Begin);
                                break;

                            case 0x1654ae6b:
                                num5 = this.f.Position + matroskaDataSize;
                                this.AnalyzeMatroskaTracks();
                                this.f.Seek(num5, SeekOrigin.Begin);
                                flag = true;
                                break;

                            case 0x1f43b675:
                                num5 = this.f.Position + matroskaDataSize;
                                this.AnalyzeMatroskaCluster();
                                this.f.Seek(num5, SeekOrigin.Begin);
                                break;

                            default:
                                if (matroskaId != 0x18538067)
                                {
                                    this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                                }
                                break;
                        }
                    }
                }
            }
            this.f.Close();
            this.f.Dispose();
            this.f = null;
            pixelWidth = this._pixelWidth;
            pixelHeight = this._pixelHeight;
            frameRate = this._frameRate;
            hasConstantFrameRate = this._frameRate > 0.0;
            millisecsDuration = this._durationInMilliseconds;
            videoCodec = this._videoCodecId;
        }

        private uint GetMatroskaSegmentId()
        {
            byte num = (byte)this.f.ReadByte();
            switch (num)
            {
                case 0xec:
                    return num;

                case 0xbf:
                    return num;
            }
            uint num2 = (uint)((num * 0x100) + ((byte)this.f.ReadByte()));
            switch (num2)
            {
                case 0x73a4:
                case 0x7384:
                case 0x4444:
                case 0x6924:
                case 0x69fc:
                case 0x69bf:
                case 0x69a5:
                case 0x4489:
                case 0x4461:
                case 0x7ba9:
                case 0x4d80:
                case 0x5741:
                    return num2;
            }
            num2 = (uint)((num * 0x100) + ((byte)this.f.ReadByte()));
            if (((num2 != 0x3cb923) && (num2 != 0x3c83ab)) && (((num2 != 0x3eb923) && (num2 != 0x3e83bb)) && (num2 != 0x2ad7b1)))
            {
                return 0;
            }
            return num2;
        }

        private string GetMatroskaString(long size)
        {
            try
            {
                byte[] buffer = new byte[size];
                this.f.Read(buffer, 0, (int)size);
                return Encoding.UTF8.GetString(buffer);
            }
            catch
            {
                return string.Empty;
            }
        }

        public Subtitle GetMatroskaSubtitle(string fileName, int trackNumber, out bool isValid)
        {
            this._subtitleRipTrackNumber = trackNumber;
            this.f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (this.GetMatroskaId() != 0x1a45dfa3)
            {
                isValid = false;
            }
            else
            {
                isValid = true;
                byte b = (byte)this.f.ReadByte();
                int matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                long matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                bool flag = false;
                for (bool flag2 = false; !flag2 && !flag; flag2 = this.f.Position >= this.f.Length)
                {
                    uint matroskaId = this.GetMatroskaId();
                    if (matroskaId == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        long num5;
                        b = (byte)this.f.ReadByte();
                        matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                        matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                        switch (matroskaId)
                        {
                            case 0x1549a966:
                                num5 = this.f.Position + matroskaDataSize;
                                this.AnalyzeMatroskaSegmentInformation(num5);
                                this.f.Seek(num5, SeekOrigin.Begin);
                                break;

                            case 0x1654ae6b:
                                num5 = this.f.Position + matroskaDataSize;
                                this.AnalyzeMatroskaTracks();
                                this.f.Seek(num5, SeekOrigin.Begin);
                                break;

                            case 0x1f43b675:
                                num5 = this.f.Position + matroskaDataSize;
                                this.AnalyzeMatroskaCluster();
                                this.f.Seek(num5, SeekOrigin.Begin);
                                break;

                            default:
                                if (matroskaId != 0x18538067)
                                {
                                    this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                                }
                                break;
                        }
                    }
                }
            }
            this.f.Close();
            this.f.Dispose();
            this.f = null;
            return this._subtitleRip;
        }

        public List<MatroskaSubtitleInfo> GetMatroskaSubtitleTracks(string fileName)
        {
            bool isValid = false;

            this.f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (this.GetMatroskaId() != 0x1a45dfa3)
            {
                isValid = false;
            }
            else
            {
                isValid = true;
                byte b = (byte)this.f.ReadByte();
                int matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                long matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                bool flag = false;
                for (bool flag2 = false; !flag2 && !flag; flag2 = this.f.Position >= this.f.Length)
                {
                    uint matroskaId = this.GetMatroskaId();
                    if (matroskaId == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        long num5;
                        b = (byte)this.f.ReadByte();
                        matroskaVariableIntLength = GetMatroskaVariableIntLength(b);
                        matroskaDataSize = this.GetMatroskaDataSize((long)matroskaVariableIntLength, b);
                        switch (matroskaId)
                        {
                            case 0x1549a966:
                                num5 = this.f.Position + matroskaDataSize;
                                this.AnalyzeMatroskaSegmentInformation(num5);
                                this.f.Seek(num5, SeekOrigin.Begin);
                                break;

                            case 0x1654ae6b:
                                num5 = this.f.Position + matroskaDataSize;
                                this.AnalyzeMatroskaTracks();
                                this.f.Seek(num5, SeekOrigin.Begin);
                                flag = true;
                                break;

                            default:
                                if (matroskaId != 0x18538067)
                                {
                                    this.f.Seek(matroskaDataSize, SeekOrigin.Current);
                                }
                                break;
                        }
                    }
                }
            }
            this.f.Close();
            this.f.Dispose();
            this.f = null;
            return this._subtitleList;
        }

        private uint GetMatroskaTrackEntryId()
        {
            uint num = (uint)this.f.ReadByte();
            switch (num)
            {
                case 0xec:
                case 0xbf:
                case 0xd7:
                case 0x83:
                case 0xb9:
                case 0x88:
                case 0x9c:
                case 0x4f:
                case 170:
                case 0xe0:
                case 0xe1:
                case 0x86:
                    return num;
            }
            num = (num * 0x100) + ((byte)this.f.ReadByte());
            switch (num)
            {
                case 0x73c5:
                case 0x55aa:
                case 0x6de7:
                case 0x6df8:
                case 0x55ee:
                case 0x63a2:
                case 0x7446:
                case 0x6d80:
                case 0x537f:
                case 0x6fab:
                case 0x536e:
                case 0x6624:
                case 0x66fc:
                case 0x66bf:
                case 0x66a5:
                    return num;
            }
            num = (num * 0x100) + ((byte)this.f.ReadByte());
            if (((num != 0x23e383) && (num != 0x22b59c)) && ((num != 0x258688) && (num != 0x23314f)))
            {
                return 0;
            }
            return num;
        }

        private uint GetMatroskaTracksId()
        {
            byte num = (byte)this.f.ReadByte();
            if (((num != 0xec) && (num != 0xbf)) && (num != 0xae))
            {
                return 0;
            }
            return num;
        }

        private uint GetMatroskaTrackVideoId()
        {
            uint num = (byte)this.f.ReadByte();
            switch (num)
            {
                case 0xec:
                case 0xbf:
                case 0xb0:
                case 0xba:
                case 0x9a:
                    return num;
            }
            num = (num * 0x100) + ((byte)this.f.ReadByte());
            switch (num)
            {
                case 0x54b0:
                case 0x54ba:
                //case 0x54ba:
                case 0x54aa:
                case 0x54bb:
                case 0x54cc:
                case 0x54dd:
               // case 0x54dd:
                case 0x54b2:
                case 0x54b3:
                    return num;
            }
            num = (num * 0x100) + ((byte)this.f.ReadByte());
            if (num == 0x2eb524)
            {
                return num;
            }
            return 0;
        }

        private static int GetMatroskaVariableIntLength(byte b)
        {
            byte num = 0;
            int num2 = 0xff;
            num2 |= b;
            if (num2 == 0xff)
            {
                num = 1;
            }
            num2 = 0x7f;
            num2 |= b;
            if (num2 == 0x7f)
            {
                num = 2;
            }
            num2 = 0x3f;
            num2 |= b;
            if (num2 == 0x3f)
            {
                num = 3;
            }
            num2 = 0x1f;
            num2 |= b;
            if (num2 == 0x1f)
            {
                num = 4;
            }
            num2 = 15;
            num2 |= b;
            if (num2 == 15)
            {
                num = 5;
            }
            num2 = 7;
            num2 |= b;
            if (num2 == 7)
            {
                num = 6;
            }
            num2 = 3;
            num2 |= b;
            if (num2 == 3)
            {
                num = 7;
            }
            num2 = 1;
            num2 |= b;
            if (num2 == 1)
            {
                num = 8;
            }
            return num;
        }

        private long GetMatroskaVariableSizeUnsignedInt(long sizeOfSize)
        {
            byte num3;
            byte num4;
            byte num5;
            byte num6;
            byte num7;
            byte num = (byte)this.f.ReadByte();
            long num8 = 0L;
            if (sizeOfSize >= 8L)
            {
                throw new NotImplementedException();
            }
            if (sizeOfSize == 7L)
            {
                byte num2 = (byte)this.f.ReadByte();
                num3 = (byte)this.f.ReadByte();
                num4 = (byte)this.f.ReadByte();
                num5 = (byte)this.f.ReadByte();
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return ((((((num7 + (num6 * 0x100)) + (num5 * 0x10000)) + (num4 * 0x1000000)) + (num3 * 0x100000000L)) + (num2 * 0x10000000000L)) + (num * 0x1000000000000L));
            }
            if (sizeOfSize == 6L)
            {
                num3 = (byte)this.f.ReadByte();
                num4 = (byte)this.f.ReadByte();
                num5 = (byte)this.f.ReadByte();
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return (((((num7 + (num6 * 0x100)) + (num5 * 0x10000)) + (num4 * 0x1000000)) + (num3 * 0x100000000L)) + (num * 0x10000000000L));
            }
            if (sizeOfSize == 5L)
            {
                num4 = (byte)this.f.ReadByte();
                num5 = (byte)this.f.ReadByte();
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return ((((num7 + (num6 * 0x100)) + (num5 * 0x10000)) + (num4 * 0x1000000)) + (num * 0x100000000L));
            }
            if (sizeOfSize == 4L)
            {
                num5 = (byte)this.f.ReadByte();
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return (long)(((num7 + (num6 * 0x100)) + (num5 * 0x10000)) + (num * 0x1000000));
            }
            if (sizeOfSize == 3L)
            {
                num6 = (byte)this.f.ReadByte();
                num7 = (byte)this.f.ReadByte();
                return (long)((num7 + (num6 * 0x100)) + (num * 0x10000));
            }
            if (sizeOfSize == 2L)
            {
                num7 = (byte)this.f.ReadByte();
                return (long)(num7 + (num * 0x100));
            }
            if (sizeOfSize == 1L)
            {
                num8 = num;
            }
            return num8;
        }

        private uint GetUInt32(byte firstByte)
        {
            FloatLayout32 layout = new FloatLayout32();
            layout.B4 = firstByte;
            layout.B3 = (byte)this.f.ReadByte();
            layout.B2 = (byte)this.f.ReadByte();
            layout.B1 = (byte)this.f.ReadByte();
            return layout.UintData32;
        }

        // Nested Types
        [StructLayout(LayoutKind.Explicit, Pack = 2)]
        private struct ByteLayout16
        {
            // Fields
            [FieldOffset(0)]
            public byte B1;
            [FieldOffset(1)]
            public byte B2;
            [FieldOffset(0)]
            public short IntData16;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 2)]
        private struct FloatLayout32
        {
            // Fields
            [FieldOffset(0)]
            public byte B1;
            [FieldOffset(1)]
            public byte B2;
            [FieldOffset(2)]
            public byte B3;
            [FieldOffset(3)]
            public byte B4;
            [FieldOffset(0)]
            public float FloatData32;
            [FieldOffset(0)]
            public uint UintData32;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 2)]
        private struct FloatLayout64
        {
            // Fields
            [FieldOffset(0)]
            public byte B1;
            [FieldOffset(1)]
            public byte B2;
            [FieldOffset(2)]
            public byte B3;
            [FieldOffset(3)]
            public byte B4;
            [FieldOffset(4)]
            public byte B5;
            [FieldOffset(5)]
            public byte B6;
            [FieldOffset(6)]
            public byte B7;
            [FieldOffset(7)]
            public byte B8;
            [FieldOffset(0)]
            public double FloatData64;
        }
    }
}
