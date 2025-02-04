using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOT_REPRINT
{
    class dm2
    {
        public static SfcHttpClient sfcClient;
        public static async Task<bool> checkModelpo(string i_model_name)
        {
            string sql = "select * from sfis1.c_parameter_ini where prg_name='" + i_model_name + "' and vr_class='PO CONTROL'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                return true;
            }
            return false;
        }
        public static async Task<string> getPOBySn(string i_sn)
        {
            string model_name, mo_number, wip_group;
            string sql = "select * from sfism4.r_wip_tracking_t where serial_number='" + i_sn + "' and rownum=1";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                model_name = _result.Data["model_name"]?.ToString() ?? "";
                mo_number = _result.Data["mo_number"]?.ToString() ?? "";
                wip_group = _result.Data["wip_group"]?.ToString() ?? "";

                sql = "select * from sfis1.c_po_config_t where model_name='" + model_name + "' and active_flag='1'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    return _result.Data["po_no"]?.ToString() ?? "";
                }
                else
                {
                    return "1";
                }
            }
            return "0";
        }
        public static async Task<bool> chkPOValid(string i_po_no, string i_serial_number, string i_model_name)
        {
            string tmppo, tmpactiveflag, tmporder;
            string sql = "select * from sfism4.r_po_sn_detail_t where  serial_number='" + i_serial_number + "' and rownum=1";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                tmppo = _result.Data["po_no"]?.ToString() ?? "";
                tmpactiveflag = _result.Data["check_flag"]?.ToString() ?? "";
                tmporder = _result.Data["po_order"]?.ToString() ?? "";
                if(tmppo== i_po_no)
                {
                    if(tmpactiveflag=="0" || tmpactiveflag == "1")
                    {
                        return true;
                    }
                }
            }
            else
            {
                sql = "select * from sfism4.r_po_sn_detail_t where po_no='" + i_po_no + "' and model_name='" + i_model_name + "' and serial_number is null and rownum=1";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    sql = "update sfism4.r_po_sn_detail_t set serial_number='" + i_serial_number + "',check_flag='0' where po_no='" + i_po_no + "' and model_name='" + i_model_name + "' and serial_number is null and rownum=1";
                    var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });

                    sql = "select * from sfism4.r_po_sn_detail_t where po_no='" + i_po_no + "' and serial_number='" + i_serial_number + "' and rownum=1";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        tmpactiveflag = _result.Data["check_flag"]?.ToString() ?? "";
                        tmporder = _result.Data["po_order"]?.ToString() ?? "";
                        if (tmpactiveflag == "0")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static async Task<bool> UpdateSninPO(string i_sn, string pubPo_no)
        {
            string model_name = "", mo_number = "", wip_group = "";
            try
            {
                string sql = "select * from sfism4.r107 where serial_number='" + i_sn + "' and rownum=1";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    model_name = _result.Data["model_name"]?.ToString() ?? "";
                    mo_number = _result.Data["mo_number"]?.ToString() ?? "";
                    wip_group = _result.Data["wip_group"]?.ToString() ?? "";
                }

                sql = "update sfism4.r_po_sn_detail_t set check_flag='1',in_station_time=to_char(sysdate, 'mm/dd/yyyy hh24:mi:ss')," +
                    "model_name='" + model_name + "',mo_number='" + mo_number + "',wip_group='" + wip_group + "' where serial_number='" + i_sn + "' and check_flag='0' and rownum=1";
                var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });

                sql = "update sfism4.r_wip_tracking_t set track_no='" + pubPo_no + "' where serial_number='" + i_sn + "' and rownum=1";
                query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
