﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FG_CHECK.Model
{
    class ExecuteResult
    {
        private string message = "";
        private int type = 0;
        private bool status = true;
        private object anything;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public Object Anything
        {
            get { return anything; }
            set { anything = value; }
        }

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
