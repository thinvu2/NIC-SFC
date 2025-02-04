namespace Sfc.Core.Parameters
{
    public class SfcParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public SfcParameterDataType? SfcParameterDataType { get; set; }

        public SfcParameterDirection? SfcParameterDirection { get; set; }

        public SfcParameter()
        {
        }
    }
}
