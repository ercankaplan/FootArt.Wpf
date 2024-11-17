using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Footart.Models
{


    public enum EnumRightOrLeft
    {
        [Description("Sağ")]
        Right = 0,
        [Description("Sol")]
        Left = 1
    }

    public enum EnumSideOrBack
    {
        [Description("Arka")]
        Back = 0,
        [Description("Yan")]
        Side = 1,
        [Description("Genişlik")]
        Width = 2,
    }

    [DataContract]
    public class FootData
    {

        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Surname { get; set; }
        [DataMember]
        public byte Gender { get; set; }
        //[DataMember]
        //public byte Dominant { get; set; }
        [DataMember]
        public byte RightOrLeft { get; set; }

        private string _RightOrLeftText;
        public string RightOrLeftText { get { return Utilities.Util.GetEnumDescription(typeof(EnumRightOrLeft), this.RightOrLeft.ToString()); } set { _RightOrLeftText = value; } }


        [DataMember]
        public byte SideOrBack { get; set; }

        private string _SideOrBackText;

        public string SideOrBackText { get { return Utilities.Util.GetEnumDescription(typeof(EnumSideOrBack), this.SideOrBack.ToString()); } set { _SideOrBackText = value; } }
        [DataMember]
        public double LAA { get; set; }
        [DataMember]
        public double LAA_X1 { get; set; }
        [DataMember]
        public double LAA_Y1 { get; set; }
        [DataMember]
        public double LAA_X2 { get; set; }
        [DataMember]
        public double LAA_Y2 { get; set; }
        [DataMember]
        public double LAA_X3 { get; set; }
        [DataMember]
        public double LAA_Y3 { get; set; }
        [DataMember]
        public double Width { get; set; }
        [DataMember]
        public double Width_X1 { get; set; }
        [DataMember]
        public double Width_Y1 { get; set; }
        [DataMember]
        public double Width_X2 { get; set; }
        [DataMember]
        public double Width_Y2 { get; set; }
        [DataMember]
        public DateTime Optime { get; set; }

        public double Calcaneal { get; set; }
        public double Calcaneal_V_X1 { get; set; }
        public double Calcaneal_V_Y1 { get; set; }
        public double Calcaneal_V_X2 { get; set; }
        public double Calcaneal_V_Y2 { get; set; }
        public double Calcaneal_X1 { get; set; }
        public double Calcaneal_Y1 { get; set; }
        public double Calcaneal_X2 { get; set; }
        public double Calcaneal_Y2 { get; set; }

        public double QAngle_Left { get; set; }
        public double QAngle_Left_V_X1 { get; set; }
        public double QAngle_Left_V_Y1 { get; set; }
        public double QAngle_Left_V_X2 { get; set; }
        public double QAngle_Left_V_Y2 { get; set; }
        public double QAngle_Left_X1 { get; set; }
        public double QAngle_Left_Y1 { get; set; }
        public double QAngle_Left_X2 { get; set; }
        public double QAngle_Left_Y2 { get; set; }

        public double QAngle_Right { get; set; } 
        public double QAngle_Right_V_X1 { get; set; }
        public double QAngle_Right_V_Y1 { get; set; }
        public double QAngle_Right_V_X2 { get; set; }
        public double QAngle_Right_V_Y2 { get; set; }
        public double QAngle_Right_X1 { get; set; }
        public double QAngle_Right_Y1 { get; set; }
        public double QAngle_Right_X2 { get; set; }
        public double QAngle_Right_Y2 { get; set; }

    }

}
