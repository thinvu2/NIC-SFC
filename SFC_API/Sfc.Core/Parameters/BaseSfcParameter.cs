using System.Collections.Generic;
namespace Sfc.Core.Parameters
{

    public class BaseParameterModel
    {
        public string CommandText { get; set; }
        public SfcCommandType SfcCommandType { get; set; }

        public IEnumerable<SfcParameter> SfcParameters { get; set; }

        public BaseParameterModel()
        {
            SfcCommandType = SfcCommandType.Text;
        }

    }
}
