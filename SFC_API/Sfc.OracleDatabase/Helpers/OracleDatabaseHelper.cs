using Dapper.Oracle;
using Sfc.Core.Parameters;
using System.Data;
using System.Linq;

namespace Sfc.OracleDatabase.Helpers
{
    public static class OracleDatabaseHelper
    {
        public static OracleDynamicParameters GetOracleDynamicParameters(BaseParameterModel model)
        {
            OracleDynamicParameters parameters = new OracleDynamicParameters();
            if (model.SfcParameters != null && model.SfcParameters.Count() > 0)
            {
      
                foreach (var sfcParam in model.SfcParameters)
                {
  
                    OracleMappingType? dbType = GetOracleMappingType(sfcParam.SfcParameterDataType);
                    int? size = GetDataTypeSize(sfcParam.SfcParameterDataType);
                    ParameterDirection? direction = GetParameterDirection(sfcParam.SfcParameterDirection);
                    parameters.Add(name: sfcParam.Name, value: sfcParam.Value, dbType: dbType, direction: direction, size: size);
                }
            }
            return parameters;

        }

        private static OracleMappingType? GetOracleMappingType(SfcParameterDataType? sfcParameterDataType)
        {
            try
            {
                OracleMappingType oracleMappingType = (OracleMappingType)sfcParameterDataType;
                return oracleMappingType;
            }
            catch
            {
                return null;
            }
        }
        private static ParameterDirection? GetParameterDirection(SfcParameterDirection? sfcParameterDirection)
        {
            try
            {
                ParameterDirection parameterDirection = (ParameterDirection)sfcParameterDirection;


                return parameterDirection;
            }
            catch
            {
                return null;
            }
        }
        private static int? GetDataTypeSize(SfcParameterDataType? sfcParameterDataType)
        {
            try
            {
                OracleMappingType oracleMappingType = (OracleMappingType)sfcParameterDataType;
                switch (oracleMappingType)
                {
                    case OracleMappingType.Varchar2:
                    case OracleMappingType.NVarchar2:
                    case OracleMappingType.Clob:
                    case OracleMappingType.NClob:
                    case OracleMappingType.Blob:
                        return 4000;
                    default:
                        return null;

                }
            }
            catch
            {
                return null;
            }
        }
    }
}
