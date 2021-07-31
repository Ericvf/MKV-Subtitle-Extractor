using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fex.Matroska 
{
    public class MatroskaSubtitleInfo
    {
        public string CodecId { get; set; }
        public string CodecPrivate { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public int TrackNumber { get; set; }
    }
}
