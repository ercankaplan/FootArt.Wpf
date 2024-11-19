using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootArt.Data.Entities
{
    public class StudyData
    {
        public Guid Id { get; set; }

        public string Title { get; set; } 
        public string Gender { get; set; }

        public DateTime Optime { get; set; } = DateTime.Now;

        public string FileName { get; set; } 

        public int DataType { get; set; }

        public string Side { get; set; } 

        public double StudyDataValue { get; set; }

        public string StudyDataText { get; set; }
        public List<StudyDataPoint> StudyDataSource { get; set; } = new List<StudyDataPoint>();

    }

    public class StudyDataPoint
    { 
        public int Id { get; set; }

        public Guid StudyDataId { get; set; }
        public double X { get; set; }

        public double Y { get; set; }
    }
}
