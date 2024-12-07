using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootArt.Data.Entities
{
    public class FaceRatioData
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal FaceRatio { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }

        public DateTime Optime { get; set; }
    }
}
