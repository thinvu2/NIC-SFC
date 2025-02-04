using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TURNPCB.Class;
using System.Data;
using DBLib;
using Oracle.ManagedDataAccess.Client;
using System.Net;
using System.IO;

namespace PRINT_ALLPARTS._3Layer
{
    public class PrintAllpartsDAL : SessionManager
    {
        public PrintAllpartsDAL()
        {

        }
        public string strSQL = string.Empty;
        public bool DoCheckMoNumber(string monumber)
        {
            strSQL = "select model_name,target_qty,mo_number from sfism4.r_bpcs_moplan_t where mo_number='"+monumber+"' and SUBSTR(MO_NUMBER,0,2)='00'";
            DataTable dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
            if (dt.Rows.Count > 0)
            {
                PrintAllpartsDTO.Instance.iMoNumber = dt.Rows[0]["mo_number"].ToString();
                PrintAllpartsDTO.Instance.iModelName = dt.Rows[0]["model_name"].ToString();
                PrintAllpartsDTO.Instance.iTargetQty = dt.Rows[0]["target_qty"].ToString();
            }
            return dt.Rows.Count > 0;
        }
        public bool DoGetPrefixLabel(string bu)
        {
            strSQL = "select BARCODE_PREFIX,YEAR_MONTH,BARCODE_LENGTH,VALID_CHAR,MACID_STEP from SFIS1.C_BARCODE_MODEL_RULE_T where model_name ='PRINT_ALLPART' AND SKU_NAME ='"+bu+"'";
            DataTable dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
            if (dt.Rows.Count > 0)
            {
                PrintAllpartsDTO.Instance.iBarcodePrefix = dt.Rows[0]["BARCODE_PREFIX"].ToString();
                PrintAllpartsDTO.Instance.iValidChar = dt.Rows[0]["VALID_CHAR"].ToString();
                PrintAllpartsDTO.Instance.iYearMonth = dt.Rows[0]["YEAR_MONTH"].ToString();
                PrintAllpartsDTO.Instance.iBarcodeLength = dt.Rows[0]["BARCODE_LENGTH"].ToString();
                PrintAllpartsDTO.Instance.iMacIdStep = dt.Rows[0]["MACID_STEP"].ToString();
            }
            return dt.Rows.Count > 0;
        }
        public bool DoGetLabelPrefix(string monumber,string yearmonth,string barcodeprefix,string bu)
        {
            DataTable dt = new DataTable();
            string YY = string.Empty;
            if (bu != "ROKU")
            {
                strSQL = "select to_char (sysdate,'" + yearmonth + "') YY from dual ";
                dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
                YY = dt.Rows[0]["YY"].ToString();
            }
            else
            {
                strSQL = @"SELECT    TO_CHAR (SYSDATE, 'Y')   
                            || CASE 
                            WHEN TO_CHAR (SYSDATE, 'MM') = '01' THEN '1' 
                            WHEN TO_CHAR (SYSDATE, 'MM') = '02' THEN '2' 
                            WHEN TO_CHAR (SYSDATE, 'MM') = '03' THEN '3'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '04' THEN '4' 
                            WHEN TO_CHAR (SYSDATE, 'MM') = '05' THEN '5' 
                            WHEN TO_CHAR (SYSDATE, 'MM') = '06' THEN '6'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '07' THEN '7'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '08' THEN '8'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '09' THEN '9'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '10' THEN 'A'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '11' THEN 'B'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '12' THEN 'C'  
                            ELSE TO_CHAR (SYSDATE, 'MM') 
                             END
                             || CASE 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '01' THEN '1' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '02' THEN '2' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '03' THEN '3'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '04' THEN '4'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '05' THEN '5' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '06' THEN '6'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '07' THEN '7'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '08' THEN '8 ' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '09' THEN '9' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '10' THEN 'A'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '11' THEN 'B' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '12' THEN 'C' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '13' THEN 'D'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '14' THEN 'E' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '15' THEN 'F'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '16' THEN 'G'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '17' THEN 'H'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '18' THEN 'I'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '19' THEN 'J'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '20' THEN 'K'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '21' THEN 'L'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '22' THEN 'M'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '23' THEN 'N'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '24' THEN 'O'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '25' THEN 'P'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '26' THEN 'Q'    
                            WHEN TO_CHAR (SYSDATE, 'DD') = '27' THEN 'R'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '28' THEN 'S'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '29' THEN 'T'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '30' THEN 'U'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '31' THEN 'V'   
                            ELSE TO_CHAR (SYSDATE, 'DD') 
                             END 
                            AS YY  
                            FROM DUAL ";
                dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
                YY = dt.Rows[0]["YY"].ToString();
            }

            strSQL = "select '"+ barcodeprefix + "'||'"+ YY + "' prefix,nvl(max(ssn1),'No data') lastdata, COUNT(1) TOTAL from sfism4.r_data_input_t where mo_number='"+ monumber + "'";
            dt.Clear();
            dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
            if (dt.Rows.Count > 0)
            {
                PrintAllpartsDTO.Instance.iLabelPrefix = dt.Rows[0]["prefix"].ToString();
                PrintAllpartsDTO.Instance.iLastData = dt.Rows[0]["lastdata"].ToString();
                PrintAllpartsDTO.Instance.iQtyPrinted = dt.Rows[0]["TOTAL"].ToString();
            }
            return dt.Rows.Count > 0;
        }
        public string DoGetVerSion(string modelname, string monumber, string bu)
        {
            if (bu !="NIC")
            {
                strSQL = "select version From SFIS1.tblBSProductData Where ProductNo = '" + modelname + "'";
            }
            else
            {
                strSQL = "SELECT HH_VER as version FROM SFIS1.C_MODEL_BRCM_VER_T WHERE MO_NUMBER =SUBSTR ('" + monumber+"',3) AND MODEL_NAME ='"+ modelname + "'";
            }
            
            DataTable dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
            if (dt.Rows.Count > 0)
            {
                return PrintAllpartsDTO.Instance.iVerSion = dt.Rows[0]["version"].ToString();
            }
            else
            {
                return "N/A";
            }
        }
        public string DoGetNextSN (string firstsnbeforeprint, string constprefix, int barcodelength, string validchar, int step)
        {
            var parmas = new OracleParameter[6];

            parmas[0] = new OracleParameter("firstsn", firstsnbeforeprint) { Direction = ParameterDirection.Input };
            parmas[1] = new OracleParameter("constprefix", constprefix) { Direction = ParameterDirection.Input };
            parmas[2] = new OracleParameter("barcodelength", barcodelength) { Direction = ParameterDirection.Input };
            parmas[3] = new OracleParameter("validchar", validchar) { Direction = ParameterDirection.Input };
            parmas[4] = new OracleParameter("macid_step", step) { Direction = ParameterDirection.Input };
            parmas[5] = new OracleParameter()
            {
                ParameterName = "res",
                Direction = ParameterDirection.Output,
                Size = 1024
            };
            OracleHelper.ExecuteNonQuery(ORACONN_SFIS, CommandType.StoredProcedure, "sfis1.get_nextsn_allpart", parmas);
            var res = parmas[5].Value.ToString();

            return res;
        }

        public string DoGetFirstSnWhenPrint(string constprefix, int barcodelength)
        {
            string var_change = string.Empty;
            string totalsn = string.Empty;
            int i,j = 0;
            j = barcodelength;
            i = j - constprefix.Length;
            while (0 < i)
            {
                var_change = var_change + "0";
                i--;
            }
            totalsn = constprefix + var_change;
            strSQL = "select nvl(max(ssn1),'"+ totalsn + "') bb from sfism4.r_data_input_t where ssn1 like '"+ constprefix + "%' ";
            DataTable dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
            if (dt.Rows.Count > 0)
            {
                return PrintAllpartsDTO.Instance.iFirstSnBeforePrint = dt.Rows[0]["bb"].ToString();
            }
            else
            {
                return "N/A";
            }
        }

        public string downloadFile(string UrlLabelFile,string LabelName,string FilePath)
        {
            string a = UrlLabelFile + LabelName;
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(a, FilePath);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string downloadFile1(string ftpfilepath, string outputfilepath)
        {
             string ftp_userUpload = "amsupload";
             string ftp_passUpload = "uploadap168!";
             string ftp_userDownload = "amsdownload";
             string ftp_passDownload = "getap168!";
            FtpWebRequest ftpRequest = null;
            FtpWebResponse ftpResponse = null;
            Stream ftpStream = null;
            byte[] downloadedData;
            downloadedData = new byte[0];

            try
            {
                //Optional
                string fullOutputfilepath = outputfilepath;
                string ftpfullpath = ftpfilepath.Replace("\\", "//");
                // ftpfullpath = "ftp://" + this.ftpAdd + ftpfullpath;

                ftpRequest = FtpWebRequest.Create(ftpfullpath) as FtpWebRequest;
                //Get the file size first (for progress bar)
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;

                ftpRequest.Credentials = new NetworkCredential(ftp_userDownload, ftp_passDownload);
                ftpRequest.UsePassive = false;
                ftpRequest.UseBinary = true;
                ftpRequest.Proxy = null;

                int dataLength = (int)ftpRequest.GetResponse().ContentLength;


                //Now get the actual data
                ftpRequest = FtpWebRequest.Create(ftpfullpath) as FtpWebRequest;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpRequest.Credentials = new NetworkCredential(ftp_userDownload, ftp_passDownload);
                ftpRequest.UsePassive = false;
                ftpRequest.UseBinary = true;
                // request.KeepAlive = false; //close the connection when done
                ftpRequest.Proxy = null;

                //Set up progress bar
                //Streams
                ftpResponse = ftpRequest.GetResponse() as FtpWebResponse;
                Stream reader = ftpResponse.GetResponseStream();

                //Download to memory
                //Note: adjust the streams here to download directly to the hard drive
                MemoryStream memStream = new MemoryStream();
                byte[] buffer = new byte[1024]; //downloads in chuncks
                int isRead = 0;
                while (true)
                {
                    //Try to read the data
                    int bytesRead = reader.Read(buffer, 0, buffer.Length);
                    isRead = isRead + bytesRead;
                    if (bytesRead == 0)
                    {
                        //Nothing was read, finished downloading
                        break;
                    }
                    else
                    {
                        //Write the downloaded data
                        memStream.Write(buffer, 0, bytesRead);
                        //Update the progress bar
                    }
                }

                //Convert the downloaded stream to a byte array
                downloadedData = memStream.ToArray();
                FileStream newFile = new FileStream(outputfilepath, FileMode.Create);
                newFile.Write(downloadedData, 0, downloadedData.Length);
                newFile.Close();

                //Clean up
                reader.Close();
                memStream.Close();
                ftpResponse.Close();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string DoGetFtpLabel()
        {
            strSQL = "SELECT MODEL_SERIAL || CUSTOMER as url  FROM  SFIS1.C_MODEL_DESC_t WHERE UPPER (MODEL_NAME) = 'Z_LABELROOM_LH'";
            DataTable dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
            if (dt.Rows.Count > 0)
            {
                return PrintAllpartsDTO.Instance.iUrl = dt.Rows[0]["url"].ToString();
            }
            else
            {
                return "N/A";
            }
        }
        public string DoSaveMoExt(string monumber,string modelname,string version,string beginsn,string endsn,string lengthsn)
        {
            strSQL = "select 1 from sfism4.r_mo_ext_t where ('"+ beginsn + "' between item_1 and item_2) or ('"+ endsn + "' between item_1 and item_2)";
            DataTable dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
            if (dt.Rows.Count > 0)
            {
                return "DUP range SN. Please check again!";
            }
            else
            {
                int length = Convert.ToInt32(lengthsn) + 1;
                strSQL = string.Format(@"Insert into SFISM4.R_MO_EXT_T (MO_NUMBER, ITEM_1, VER_1, ITEM_2, VER_2, ITEM_3, VER_3, VER_4, ITEM_5, VER_5, VER_6)
             Values  ('{0}', '{1}', '{2}', '{3}', '{4}', to_char(sysdate,'YYYYMMDDHH24MI'),to_char(sysdate,'YYYYMMDDHH24MI'),'{5}', '0', '{6}', '{7}')",monumber, beginsn, version, endsn, version, modelname, modelname, length);
                try
                {
                    OracleHelper.ExecuteNonQuery(ORACONN_SFIS, CommandType.Text, strSQL);
                    return "OK";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }
        public string DoCheckMoExt(string monumber, string modelname, string version, string beginsn, string endsn, string lengthsn)
        {
            string res = string.Empty;
            strSQL = "select 1 from sfism4.r_mo_ext_t where ('" + beginsn + "' between item_1 and item_2) or ('" + endsn + "' between item_1 and item_2)";
            DataTable dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
            if (dt.Rows.Count > 0)
            {
                res = "DUP range SN. Please check again!";
            }
            else
            {
                res = "OK";
            }
            strSQL = "select 1 from sfism4.R_DATA_INPUT_T where ssn1 between '" + beginsn + "' and '" + endsn + "' ";
            DataTable dt1 = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
            if (dt1.Rows.Count > 0)
            {
                res = "DUP range SN. Please check again!";
            }
            else
            {
                res = "OK";
            }
            return res;
        }
        
        public string DoSaveInputData(string monumber, string sn)
        {
                strSQL = string.Format(@"insert into sfism4.r_data_input_t(mo_number,ssn1,print_flag,input_time,print_time) values ('{0}','{1}','Y',sysdate,sysdate)", monumber, sn);
                try
                {
                    OracleHelper.ExecuteNonQuery(ORACONN_SFIS, CommandType.Text, strSQL);
                    return "OK";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
        }
        public string DoCheckVersion(string programname, string version)
        {
            DataTable dt = new DataTable();
            strSQL = string.Format(@"SELECT * FROM SFISM4.AMS_AP     WHERE 1=1     AND AP_NAME ='{0}'", programname);
            try
            {
                dt =  OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
                if (dt.Rows.Count < 1)
                {
                    return "Function not exist";
                }
                else
                {
                   string DBversion =  dt.Rows[0]["AP_VERSION"].ToString();
                    if (DBversion!= version.Trim())
                    {
                        return "Wrong version, system is "+DBversion+ ",program is "+ version;
                    }
                    else
                    {
                        return "OK";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public DataTable DoGetBuCode()
        {
            strSQL = "select distinct VR_VALUE from SFIS1.C_PARAMETER_INI where prg_name = 'PRINT_ALLPART' AND VR_NAME = 'BU'";
            DataTable dt = OracleHelper.ExecuteDataTable(ORACONN_SFIS, CommandType.Text, strSQL);
            return dt;
        }
    }
}
