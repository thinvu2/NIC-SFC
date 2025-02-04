using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOT_REPRINT
{
    class UisPassLabel
    {
        static string DBChecksum;
        private static string sql = string.Empty;
        public static SfcHttpClient sfcClient;
        public static async Task<bool> compareToDb(string Labelname, string station)
        {
            sql = "SELECT checksum from sfism4.r_fqa_checklabel_t where upd_date= (SELECT   max(upd_date) " +
                " FROM sfism4.r_fqa_checklabel_t WHERE label_name = '"+ Labelname .ToUpper()+ "' and Group_Name='" + station.ToUpper() + "' ) " +
                " and label_name = '" + Labelname.ToUpper() + "' and  Group_Name='" + station.ToUpper() + "' ";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                DBChecksum = _result.Data["checksum"]?.ToString() ?? "";
                if (DBChecksum.Trim().Length > 0)
                {

                }
            }
            return true;
            /*strsql:=' SELECT * from sfism4.r_fqa_checklabel_t '+
                          ' where upd_date= (SELECT   max(upd_date) ' +
                       ' FROM sfism4.r_fqa_checklabel_t '+
                      ' WHERE label_name = :Label_Name and Group_Name=:Group_Name ) '+
                      ' and label_name = :Label_Name and  Group_Name=:Group_Name';
              formReprint.qryLabelpass.Close;
              formReprint.qryLabelpass.SQL.Clear;
              formReprint.qryLabelpass.SQL.Add(strsql);
              formReprint.qryLabelpass.parambyname('Label_Name').AsString:=UPPERCASE(Labelname);
              formReprint.qryLabelpass.ParamByName('Group_Name').AsString:=UPPERCASE(station);
              formReprint.qryLabelpass.Open;
              if not formReprint.qryLabelpass.Eof  then
              begin
                DBChecksum:=formReprint.qryLabelpass.FieldByName('CHECKSUM').AsString;
                if length(trim(DBChecksum))>0 then
                begin
                    printchecksum:=formCodeSoft.SYANT_GET_MD5;
                    IF printchecksum='' THEN
                    BEGIN
                      EXIT;
                    END;
                    if printchecksum<>DBChecksum  then
                    begin
                      //PasswdForm.panlMessage.Caption := 'Label¤w­×§ï,½Ð¨î§@­º¥ó¡M¦}½ÐFQA¤H­ûÂIPASS!';
                      //PasswdForm.Showmodal;
                      PasswdForm.panlMessage.Caption :=GetPubMessage('00201',formReprint.dbReprint)+' '+ GetPubMessage('00203',formReprint.dbReprint);
                      PasswdForm.Showmodal;
                      result:=false;
                    end else
                    begin
                     result:=true;
                    end;
                end else
                begin
                  //PasswdForm.panlMessage.Caption := 'LABEL¶i¦æ¥dÃö,½ÐFQA¬ÛÃö¤H­ûÂIPASS!';
                  //PasswdForm.Showmodal;
                  PasswdForm.panlMessage.Caption := GetPubMessage('00202',formReprint.dbReprint);
                  PasswdForm.Showmodal;
                  result:=false;
                end;
              end else
              begin
                //PasswdForm.panlMessage.Caption := '½Ð¨î§@­º¥ó,¦}½ÐFQA¬ÛÃö¤H­ûÂIPASS!';
                //PasswdForm.Showmodal;
                PasswdForm.panlMessage.Caption := GetPubMessage('00203',formReprint.dbReprint);
                PasswdForm.Showmodal;
                result:=false;
              end;*/
        }
    }
}
