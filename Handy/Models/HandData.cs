using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Handy.Models
{
    public enum EnumRightOrLeft
    {
        Right = 0,
        Left = 1
    }

    [DataContract]
    public class HandData
    {

        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Surname { get; set; }
        [DataMember]
        public byte Gender { get; set; }
        [DataMember]
        public byte Dominant { get; set; }
        [DataMember]
        public byte RightOrLeft { get; set; }
        /// <summary>
        /// Baş Parmak
        /// </summary>
        [DataMember]
        public double Thumb_X { get; set; }
        [DataMember]
        public double Thumb_Y { get; set; }
        /// <summary>
        /// İşaret Parmağı
        /// </summary>
        [DataMember]
        public double Index_X { get; set; }
        [DataMember]
        public double Index_Y { get; set; }
        /// <summary>
        /// Orta Parmak
        /// </summary>
        [DataMember]
        public double Middle_X { get; set; }
        [DataMember]
        public double Middle_Y { get; set; }
        /// <summary>
        /// Yüzük Parmağı
        /// </summary>
        [DataMember]
        public double Ring_X { get; set; }
        [DataMember]
        public double Ring_Y { get; set; }
        /// <summary>
        /// Küçük Parmak
        /// </summary>
        [DataMember]
        public double Little_X { get; set; }
        [DataMember]
        public double Little_Y { get; set; }

        [DataMember]
        public DateTime Optime { get; set; }

        [DataMember]
        public double P1 { get; set; }
        [DataMember]
        public double P2 { get; set; }
        [DataMember]
        public double P3 { get; set; }
        [DataMember]
        public double P4 { get; set; }
        [DataMember]
        public double P5 { get; set; }
        [DataMember]
        public double FS1 { get; set; }
        [DataMember]
        public double FS2 { get; set; }
        [DataMember]
        public double FS3 { get; set; }
        [DataMember]
        public double FS4 { get; set; }
        [DataMember]
        public double TL { get; set; }
        [DataMember]
        public double IFL { get; set; }
        [DataMember]
        public double MFL { get; set; }
        [DataMember]
        public double RFL { get; set; }
        [DataMember]
        public double LFL { get; set; }



    }

 
}
