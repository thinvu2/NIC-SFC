using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMSLabel
{
    public class LabelInfo
    {
        public LabelInfo(string name, string size, string type, string modified, string group, string user, string permisions)
        {
            Name = name;
            Size = size;
            Type = type;
            Modified = modified;
            Group = group;
            User = user;
            Permisions = permisions;
           
        }

        public string Name { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Modified { get; set; }
        public string Group { get; set; }
        public string User { get; set; }
        public string Permisions { get; set; }


    }
    public class LabelIndex
    {
        public LabelIndex() { }
        public LabelIndex(string no, string label_path, string path_index)
        {
            NO = no;
            LABEL_PATH = label_path;
            PATH_INDEX = path_index;
 
        }

        public string NO { get; set; }
        public string LABEL_PATH { get; set; }
        public string PATH_INDEX { get; set; }


    }
}
