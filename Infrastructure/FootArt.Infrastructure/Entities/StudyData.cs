using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootArt.Infrastructure.Entities
{
    public class StudyData
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = default!;
        public string Gender { get; set; } = default!;

        public DateTime Optime { get; set; } = DateTime.Now;

        public string FileName { get; set; } = default!;

        public StudyDataType DataType { get; set; }

        public string Side { get; set; } = default!;

        public List<StudyDataPoint> StudyDataSource { get; set; } = new List<StudyDataPoint>();
    }

    public class StudyDataPoint
    { 
        public int Id { get; set; }
        public double X { get; set; }

        public double Y { get; set; }
    }
}
