using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOLD.LogInfo
{
    class infHoldQuery
    {
        //public infHoldQuery(string serial_number, string model_name, string main_desc, string hold_emp_no, DateTime hold_time, string hold_reason, string hold_program, string unhold_emp_no, 
        //    DateTime unhold_time, string unhold_reason, string unhold_program, string finish_flag, string data1, string data2, string data3)
        //{
        //    SERIAL_NUMBER = serial_number;
        //    MODEL_NAME = model_name;
        //    MAIN_DESC = main_desc;
        //    HOLD_EMP_NO = hold_emp_no;
        //    HOLD_TIME = hold_time;
        //    HOLD_REASON = hold_reason;
        //    HOLD_PROGRAM = hold_program;
        //    UNHOLD_EMP_NO = unhold_emp_no;
        //    UNHOLD_TIME = unhold_time;
        //    UNHOLD_REASON = unhold_reason;
        //    UNHOLD_PROGRAM = unhold_program;
        //    FINISH_FLAG = finish_flag;
        //    DATA1 = data1;
        //    DATA2 = data2;
        //    DATA3 = data3;
        //}
       
        public string SERIAL_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string MAIN_DESC { get; set; }
        public string HOLD_EMP_NO { get; set; }
        public string HOLD_TIME { get; set; }
        public string HOLD_REASON { get; set; }
        public string HOLD_PROGRAM { get; set; }
        public string UNHOLD_EMP_NO { get; set; }
        public string UNHOLD_TIME { get; set; }
        public string UNHOLD_REASON { get; set; }
        public string UNHOLD_PROGRAM { get; set; }
        public string FINISH_FLAG { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
    }
}
