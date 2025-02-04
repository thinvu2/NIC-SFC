using LabelManager2;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PACK_PALT.Resource;

namespace PACK_PALT.Resource
{
    public class LabelTrackingErrorCode
    {
        public const string Error_MD5 = "MD5";
        public const string Error_NOT_CONFIRM = "NOT_CONFIRM";
        public const string Error_NOT_ENOUGHT = "NOT_ENOUGHT";
        public const string Error_EXCEPTION = "EXCEPTION";
        public const string Error_FOUND_ERROR = "FOUND_ERROR";
    }
    public class LabelTracking
    {
        public SfcHttpClient sfcHttpClient { get; set; }
        public string ErrorCode = "";
        //private static OracleConnection conn = null;
        //private static string connString = CommonString.getConnStrAlias_SFC("SFIS.WORLD");// "Data Source=sfis.world;Persist Security Info=True;User ID=SFIS1;Password=vnsfis2014#!";
        /// <summary>
        /// Check MD5 of label file 
        /// </summary>
        /// <param name="LabelName">Name of file label</param>
        /// <param name="PathIndex">PATH_NAME of label</param>
        /// <returns></returns>
        public bool  doMD5Label(string LabelName, int PathIndex, bool isShowErrorMessage = false)
        {
            string wipMD5 = getWipLabelMD5(LabelName, PathIndex);
            if (string.IsNullOrEmpty(wipMD5))
            {
                ErrorCode = LabelTrackingErrorCode.Error_MD5;
                if (isShowErrorMessage)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageEnglish = string.Format("Not found MD5 of label file in db. " + Environment.NewLine
                    + "File name: {0}, PathIndex: {1}" + Environment.NewLine + "Table: C_LABEL_WIP_T", LabelName.ToLower(), PathIndex);
                    _sh.MessageVietNam = string.Format("Không tìm thấy dữ liệu MD5 của file trên server." + Environment.NewLine
                    + "File name: {0}, PathIndex: {1}" + Environment.NewLine + "Table: C_LABEL_WIP_T", LabelName.ToLower(), PathIndex);
                    _sh.ShowDialog();
                }
                return false;
            }
            string CurrentFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + LabelName;
            string CurrentFileMD5 = CRC322(CurrentFileName);
            if (!wipMD5.Equals(CurrentFileMD5))
            {
                ErrorCode = LabelTrackingErrorCode.Error_MD5;
                if (isShowErrorMessage)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = string.Format("MD5 của file label hiện tại khác với file trên server." + Environment.NewLine
                    + "File name: {0}, PathIndex: {1}" + Environment.NewLine  + "ServerMD5: {2} <> CurrentMD5: {3}"
                     , LabelName.ToLower(), PathIndex, wipMD5, CurrentFileMD5);
                    _sh.MessageEnglish = string.Format("Label file Version error. " + Environment.NewLine 
                    + "File name: {0}, PathIndex: {1}" + Environment.NewLine  + "ServerMD5: {2} <> CurrentMD5: {3}"
                     , LabelName.ToLower(), PathIndex, wipMD5, CurrentFileMD5);
                    _sh.ShowDialog();
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// Check label content with LabelTracking system, Add parameters value before check
        /// </summary>
        /// <param name="LabelName">Name of file label</param>
        /// <param name="PathIndex">Type PATH_NAME of label</param>
        /// <param name="paramTable">List parameter will add to label file, have 2 columns: Name and Value. Name is Parameter name in label, Value is value set into parameter</param>
        /// <returns>true: </returns>
        public async Task<bool> doLabelTracking_AddParamValue_1(Document doc, string LabelName, int PathIndex, DataTable paramTable, bool isShowErrorMessage = false)
        {
            string _param_Name = "";
            // Get value of label items to compare with label tracking
            DataTable dtbVarName = new DataTable();
            dtbVarName.Columns.Add("VAR_NAME", typeof(string));
            dtbVarName.Columns.Add("VALUE", typeof(string));
            dtbVarName.Columns.Add("TYPE", typeof(string));
            int TotalBarcode = doc.DocObjects.Barcodes.Count;
            int TotalText = doc.DocObjects.Texts.Count;
            int TotalVar = doc.Variables.FormVariables.Count;
            int TotalFreeVar = doc.Variables.FreeVariables.Count;
            int TotalFomula = doc.Variables.Formulas.Count;

            for (int i = 1; i <= TotalBarcode; i++)
            {
                var _name = doc.DocObjects.Barcodes.Item(i).Name;
                if (_name != null)
                {
                    var _var = doc.DocObjects.Barcodes.Item(i).VariableName;
                    var _bar = doc.DocObjects.Barcodes.Item(i).Symbology.ToString().Replace("lppx", "").Trim();
                    var _data = doc.DocObjects.Barcodes.Item(i).Value;
                    dtbVarName.Rows.Add(new object[] { _name, _data, _bar });
                }
            }

            for (int i = 1; i <= TotalText; i++)
            {
                var _name = doc.DocObjects.Texts.Item(i).Name;
                if (_name != null)
                {
                    var _var = doc.DocObjects.Texts.Item(i).VariableName;
                    var _font = doc.DocObjects.Texts.Item(i).Font.Name;
                    var _data = doc.DocObjects.Texts.Item(i).Value;
                    dtbVarName.Rows.Add(new object[] { _name, _data, _font });

                }
            }
            string CurrentFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + LabelName;
            string LastErrorEng = "";
            string LastErrorVN = "";
            string flag =  getLabeStatus(LabelName, PathIndex);
            if (flag != "11")
            {
                string errstr = "";
                if (flag.Length != 2 || flag.Substring(0, 1) != "1")
                {
                    errstr = "PE-Label";
                }
                else
                {
                    errstr = "PQE";
                };

                LastErrorEng = "Label file not confirmed! Please call " + errstr + " confirm" + Environment.NewLine
                    + "FileName: " + LabelName + Environment.NewLine
                    + "Path Index: " + PathIndex + Environment.NewLine
                    + "Label Flag: " + flag + Environment.NewLine;
                LastErrorVN = "File label chưa được xác nhận! Gọi " + errstr + " xác nhận" + Environment.NewLine
                    + "FileName: " + LabelName + Environment.NewLine
                    + "Path Index: " + PathIndex + Environment.NewLine
                    + "Label Flag: " + flag + Environment.NewLine;
                //lbl.Quit();
                //lbl = null;
                if (isShowErrorMessage)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = LastErrorVN;
                    _sh.MessageEnglish = LastErrorEng;
                    _sh.ShowDialog();
                }
                return false;
            }
            //Get label file content value
            try
            {
                //check PQE setup not enough varial
                DataTable dtbPQE = new DataTable();
                dtbPQE = getPQESetup(LabelName, PathIndex);
                int totalValue = dtbVarName.Rows.Count;
                int totalPQESetup = dtbPQE.Rows.Count;
                int perCent = int.Parse( getLabelSetupPercent());
                int setup = (int)Math.Floor(((double)totalPQESetup / (double)totalValue) * 100);
                if (setup < perCent)
                {
                    DataTable dtbNotSetup = DatatableMinus(dtbVarName, dtbPQE, "VAR_NAME");
                    LastErrorEng += "PQE setup label not enough " + perCent + "%." + Environment.NewLine
                                + "Call PQE continue setup on label tracking! Label name:" + LabelName + Environment.NewLine
                                + "List Object not setup:" + Environment.NewLine;
                    LastErrorVN += "PQE thiết lập chưa đủ " + perCent + "%." + Environment.NewLine
                                + "Gọi PQE tiếp tục thiết lập trên web Label tracking! Label name:" + LabelName + Environment.NewLine
                                + "List Object not setup:" + Environment.NewLine;
                    foreach (DataRow dtrow in dtbNotSetup.Rows)
                    {
                        LastErrorEng = LastErrorEng + dtrow["VAR_NAME"].ToString() + Environment.NewLine;
                        LastErrorVN = LastErrorVN + dtrow["VAR_NAME"].ToString() + Environment.NewLine;
                    }
                    if (isShowErrorMessage)
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageEnglish = LastErrorEng;
                        _sh.MessageVietNam = LastErrorVN;
                        _sh.ShowDialog();
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                LastErrorEng = "Label tracking exception " + Environment.NewLine + ex.Message;
                LastErrorVN = "Label tracking lỗi ngoại lệ: " + Environment.NewLine + ex.Message;
                if (isShowErrorMessage)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageEnglish = LastErrorEng;
                    _sh.MessageVietNam = LastErrorVN;
                    _sh.ShowDialog();
                }
                return false;
            }
            //Label value
            string _obName = "";
            string _obValue = "";
            string _obType = "";
            //PQE config value
            string _cValue = "";
            string _cType = "";
            int _cFromPos, _cToPos;

            Regex regex;
            const string errorLengthEng = "[Length error] {0} : PQE config length: {1} but Label value: {2} length: {3}";
            const string errorLengthVN = "[Lỗi độ dài] {0} : PQE thiết lập dài: {1} kí tự nhưng Giá trị biến label {2} dài {3} kí tự";
            const string errorTypeEng = "[{0} error] {1} : PQE config {2}: {3} but Label value {2}: {4}";
            const string errorTypeVN = "[Lỗi {0}] {1} : PQE thiết lập {2}: {3} nhưng Giá trị biến label {2}: {4}";
            const string errorPostFixEng = "[Profix] {0} > PQE config : *.{1} but Label value: {2}";
            const string errorPostFixVN = "[Lỗi hậu tố] {0} : PQE thiết lập giá trị kết thúc bằng {1} nhưng Giá trị biến label {2}";
            const string errorPreFixEng = "[Prefix] {0} > PQE config : *.{1} but Label value: {2}";
            const string errorPreFixVN = "[Lỗi tiền tố] {0} : PQE thiết lập biến bắt đầu bằng {1} nhưng Giá trị biến label {2}";
            const string errorFixPositionEng = "[Content error] {0} : PQE config : *{1}* (from position {2} to {3}) but Label value: {4}";
            const string errorFixPositionVN = "[Lỗi nội dung] {0} : PQE thiết lập kí tự từ {2} tới {3} là {1} nhưng Giá trị biến label là {4}";
            const string errorFixContentEng = "[Fixed Content] {0} : PQE config fix content: {1} but Label value: {2}";
            const string errorFixContentVN = "[Lỗi nội dung] {0} : PQE thiết lập nội dung cố định là {1} nhưng Giá trị biến label là {2}";
            const string errorNoStringEng = "[Not Contained] {0} : PQE config Not Contained character: {1} but Label value: {2}";
            const string errorNoStringVN = "[Kí tự không được phép có] {0} : PQE thiết lập không được phép có kí tự: {1} nhưng Giá trị biến label {2}";
            const string errorStringEng = "[Consist of] {0} : PQE config content ONLY include character: {1} but Label value: {2}";
            const string errorStringVN = "[Kí tự được phép có] {0} : PQE thiết lập chỉ được phép gồm các kí tự: {1} nhưng Giá trị biến label: {2}";
            foreach (DataRow labelRow in dtbVarName.Rows)
            {
                _obName = labelRow["VAR_NAME"].ToString();
                _obValue = labelRow["VALUE"].ToString();
                _obType = labelRow["TYPE"].ToString();
                try
                {
                    var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = string.Format("SELECT * FROM SFIS1.C_LABEL_CONFIG_T WHERE UPPER(LABEL_NAME) = '{0}' AND PATH_NAME={1} AND VAR_NAME='{2}'", LabelName.Trim().ToUpper(), PathIndex, _obName),
                        SfcCommandType = SfcCommandType.Text
                    });
                    foreach (var row in result.Data)
                    {
                        _cValue = row["web"].ToString();
                        _cType = row["flag"].ToString();
                        switch (_cType)
                        {
                            case "LENGTH":
                                //Độ dài biến
                                if (_obValue.Length != Int16.Parse(_cValue))
                                {
                                    LastErrorEng += string.Format(errorLengthEng, _obName, _cValue, _obValue, _obValue.Length) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorLengthVN, _obName, _cValue, _obValue, _obValue.Length) + Environment.NewLine;
                                }
                                break;
                            case "STRING":
                                //Kí tự được xuất hiện trong biến
                                regex = new Regex("[^" + _cValue + "]", RegexOptions.Multiline);
                                //Nếu tồn tại bất kì 1 kí tự nào trong chuỗi ko được phép  (getMatch = true)
                                // -> báo lỗi
                                if (regex.IsMatch(_obValue))
                                {
                                    LastErrorEng += string.Format(errorStringEng, _obName, _cValue.Replace(" ", "<space>"), _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorStringVN, _obName, _cValue.Replace(" ", "<dấu cách>"), _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                break;
                            case "NOSTRING":
                                //Kí tự không được xuất hiện trong biến
                                regex = new Regex("[" + _cValue + "]", RegexOptions.Multiline);
                                //Nếu tồn tại bất kì 1 kí tự nào trong chuỗi ko được phép  (getMatch = true)
                                // -> báo lỗi
                                if (regex.IsMatch(_obValue))
                                {
                                    LastErrorEng += string.Format(errorNoStringEng, _obName, _cValue.Replace(" ", "<space>"), _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorNoStringVN, _obName, _cValue.Replace(" ", "<dấu cách>"), _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                break;
                            case "PREFIX":
                                //Prefix - Tiền tố
                                if (_obValue.Length < _cValue.Length || !_cValue.Equals(_obValue.Substring(0, _cValue.Length)))
                                {
                                    LastErrorEng += string.Format(errorPreFixEng, _obName, _cValue.Replace(" ", "<space>"), _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorPreFixVN, _obName, _cValue.Replace(" ", "<dấu cách>"), _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                break;
                            case "FIXED":
                                //Giá trị cố định - fix cứng
                                if (!_obValue.Equals(_cValue))
                                {
                                    LastErrorEng += string.Format(errorFixContentEng, _obName, _cValue.Replace(" ", "<space>"), _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorFixContentVN, _obName, _cValue.Replace(" ", "<dấu cách>"), _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                break;
                            case "FIXED POSITION":
                                _cFromPos = Int16.Parse(row["FROM_POSITION"].ToString());
                                _cToPos = Int16.Parse(row["TO_POSITION"].ToString());
                                if (_obValue.Length < _cValue.Length || !_cValue.Equals(_obValue.Substring(_cFromPos, _cToPos - _cFromPos + 1)))
                                {
                                    LastErrorEng += string.Format(errorFixPositionEng, _obName, _cValue, _cFromPos, _cToPos, _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorFixPositionVN, _obName, _cValue, _cFromPos, _cToPos, _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                //Fix giá trị tại vị trí
                                break;
                            case "LINKBARCODE":
                                //Text cùng giá trị với barcode 

                                break;
                            case "PROFIX":
                                //PostFix - Hậu tố
                                if (_obValue.Length < _cValue.Length || !_cValue.Equals(_obValue.Substring(_obValue.Length - _cValue.Length, _cValue.Length)))
                                {
                                    LastErrorEng += string.Format(errorPostFixEng, _obName, _cValue, _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorPostFixVN, _obName, _cValue, _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                break;
                            case "TYPE":
                                //Loại Font- Barcode
                                if (!_cValue.Contains(_obType))
                                {
                                    if (_obName.Contains("Barcode"))
                                    {
                                        LastErrorEng += string.Format(errorTypeEng, "BARCODE", _obName, "barcode type", _cValue, _obType) + Environment.NewLine;
                                        LastErrorVN += string.Format(errorTypeVN, "Mã vạch", _obName, "loại mã", _cValue, _obType) + Environment.NewLine;
                                    }
                                    else
                                    {
                                        LastErrorEng += string.Format(errorTypeEng, "FONT", _obName, "font", _cValue, _obType) + Environment.NewLine;
                                        LastErrorVN += string.Format(errorTypeVN, "Phông chữ", _obName, "kiểu phông", _cValue, _obType) + Environment.NewLine;
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception EX)
                { string error = EX.Message; }
            }
            if (!string.IsNullOrEmpty(LastErrorEng))
            {
                LastErrorEng = "Label tracking system found some errors." + Environment.NewLine + "Please call PE/Label & PQE check:" + Environment.NewLine + LastErrorEng;
                LastErrorVN = "Hệ thống label tracking phát hiện lỗi." + Environment.NewLine + "Hãy gọi PE/Label & PQE kiểm tra lỗi sau:" + Environment.NewLine + LastErrorVN;
                if (isShowErrorMessage)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageEnglish = LastErrorEng;
                    _sh.MessageVietNam = LastErrorVN;
                    _sh.ShowDialog();
                }
                return false;
            }
            return true;
        }
        public bool doLabelTracking_AddParamValue(string LabelName, int PathIndex, DataTable paramTable,Document doc, DataTable dtbVarName, bool isShowErrorMessage = false)
        {
            //LabelManager2.Application lbl = new LabelManager2.Application();
            //DataTable dtbVarName = new DataTable();
            //dtbVarName.Columns.Add("VAR_NAME", typeof(string));
            //dtbVarName.Columns.Add("VALUE", typeof(string));
            //dtbVarName.Columns.Add("TYPE", typeof(string));
            string CurrentFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + LabelName;
            string LastErrorEng = "";
            string LastErrorVN = "";
            string flag =  getLabeStatus(LabelName, PathIndex);
            if (flag != "11")
            {
                string errstr = "";
                if (flag.Length != 2 || flag.Substring(0, 1) != "1")
                {
                    errstr = "PE-Label";
                }
                else
                {
                    errstr = "PQE";
                };

                LastErrorEng = "Label file not confirmed! Please call " + errstr + " confirm" + Environment.NewLine
                    + "FileName: " + LabelName + Environment.NewLine
                    + "Path Index: " + PathIndex + Environment.NewLine
                    + "Label Flag: " + flag + Environment.NewLine;
                LastErrorVN = "File label chưa được xác nhận! Gọi " + errstr + " xác nhận" + Environment.NewLine
                    + "FileName: " + LabelName + Environment.NewLine
                    + "Path Index: " + PathIndex + Environment.NewLine
                    + "Label Flag: " + flag + Environment.NewLine;
                //lbl.Quit();
                //lbl = null;
                if (isShowErrorMessage)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = LastErrorVN;
                    _sh.MessageEnglish = LastErrorEng;
                    _sh.ShowDialog();
                }
                return false;
            }
            //Get label file content value
            try
            {              
                //check PQE setup not enough varial
                DataTable dtbPQE = new DataTable();
                dtbPQE =  getPQESetup(LabelName, PathIndex);
                int totalValue = dtbVarName.Rows.Count;
                int totalPQESetup = dtbPQE.Rows.Count;
                int perCent = int.Parse( getLabelSetupPercent());
                int setup = (int)Math.Floor(((double)totalPQESetup / (double)totalValue) * 100);
                if (setup < perCent)
                {
                    DataTable dtbNotSetup = DatatableMinus(dtbVarName, dtbPQE, "VAR_NAME");
                    LastErrorEng += "PQE setup label not enough " + perCent + "%." + Environment.NewLine
                                + "Call PQE continue setup on label tracking! Label name:" + LabelName + Environment.NewLine
                                + "List Object not setup:" + Environment.NewLine;
                    LastErrorVN += "PQE thiết lập chưa đủ " + perCent + "%." + Environment.NewLine
                                + "Gọi PQE tiếp tục thiết lập trên web Label tracking! Label name:" + LabelName + Environment.NewLine
                                + "List Object not setup:" + Environment.NewLine;
                    foreach (DataRow dtrow in dtbNotSetup.Rows)
                    {
                        LastErrorEng = LastErrorEng + dtrow["VAR_NAME"].ToString() + Environment.NewLine;
                        LastErrorVN = LastErrorVN + dtrow["VAR_NAME"].ToString() + Environment.NewLine;
                    }
                    if (isShowErrorMessage)
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageEnglish = LastErrorEng;
                        _sh.MessageVietNam = LastErrorVN;
                        _sh.ShowDialog();
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {               
                LastErrorEng = "Label tracking exception " + Environment.NewLine + ex.Message;
                LastErrorVN = "Label tracking lỗi ngoại lệ: " + Environment.NewLine + ex.Message;
                if (isShowErrorMessage)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageEnglish = LastErrorEng;
                    _sh.MessageVietNam = LastErrorVN;
                    _sh.ShowDialog();
                }
                return false;
            }
            //Label value
            string _obName = "";
            string _obValue = "";
            string _obType = "";
            //PQE config value
            string _cValue = "";
            string _cType = "";
            int _cFromPos, _cToPos;

            Regex regex;
            const string errorLengthEng = "[Length error] {0} : PQE config length: {1} but Label value: {2} length: {3}";
            const string errorLengthVN = "[Lỗi độ dài] {0} : PQE thiết lập dài: {1} kí tự nhưng Giá trị biến label {2} dài {3} kí tự";
            const string errorTypeEng = "[{0} error] {1} : PQE config {2}: {3} but Label value {2}: {4}";
            const string errorTypeVN = "[Lỗi {0}] {1} : PQE thiết lập {2}: {3} nhưng Giá trị biến label {2}: {4}";
            const string errorPostFixEng = "[Profix] {0} > PQE config : *.{1} but Label value: {2}";
            const string errorPostFixVN = "[Lỗi hậu tố] {0} : PQE thiết lập giá trị kết thúc bằng {1} nhưng Giá trị biến label {2}";
            const string errorPreFixEng = "[Prefix] {0} > PQE config : *.{1} but Label value: {2}";
            const string errorPreFixVN = "[Lỗi tiền tố] {0} : PQE thiết lập biến bắt đầu bằng {1} nhưng Giá trị biến label {2}";
            const string errorFixPositionEng = "[Content error] {0} : PQE config : *{1}* (from position {2} to {3}) but Label value: {4}";
            const string errorFixPositionVN = "[Lỗi nội dung] {0} : PQE thiết lập kí tự từ {2} tới {3} là {1} nhưng Giá trị biến label là {4}";
            const string errorFixContentEng = "[Fixed Content] {0} : PQE config fix content: {1} but Label value: {2}";
            const string errorFixContentVN = "[Lỗi nội dung] {0} : PQE thiết lập nội dung cố định là {1} nhưng Giá trị biến label là {2}";
            const string errorNoStringEng = "[Not Contained] {0} : PQE config Not Contained character: {1} but Label value: {2}";
            const string errorNoStringVN = "[Kí tự không được phép có] {0} : PQE thiết lập không được phép có kí tự: {1} nhưng Giá trị biến label {2}";
            const string errorStringEng = "[Consist of] {0} : PQE config content ONLY include character: {1} but Label value: {2}";
            const string errorStringVN = "[Kí tự được phép có] {0} : PQE thiết lập chỉ được phép gồm các kí tự: {1} nhưng Giá trị biến label: {2}";
            foreach (DataRow labelRow in dtbVarName.Rows)
            {
                _obName = labelRow["VAR_NAME"].ToString();
                _obValue = labelRow["VALUE"].ToString();
                _obType = labelRow["TYPE"].ToString();
                try
                {
                    var result =  sfcHttpClient.QueryList(new QuerySingleParameterModel()
                    {                       
                        CommandText = string.Format("SELECT * FROM SFIS1.C_LABEL_CONFIG_T WHERE UPPER(LABEL_NAME) = '{0}' AND PATH_NAME={1} AND VAR_NAME='{2}'", LabelName.Trim().ToUpper(), PathIndex, _obName),
                        SfcCommandType = SfcCommandType.Text
                    });
                    foreach (var row in result.Data)
                    {
                        _cValue = row["web"].ToString();
                        _cType = row["flag"].ToString();
                        switch (_cType)
                        {
                            case "LENGTH":
                                //Độ dài biến
                                if (_obValue.Length != Int16.Parse(_cValue))
                                {
                                    LastErrorEng += string.Format(errorLengthEng, _obName, _cValue, _obValue, _obValue.Length) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorLengthVN, _obName, _cValue, _obValue, _obValue.Length) + Environment.NewLine;
                                }
                                break;
                            case "STRING":
                                //Kí tự được xuất hiện trong biến
                                regex = new Regex("[^" + _cValue + "]", RegexOptions.Multiline);
                                //Nếu tồn tại bất kì 1 kí tự nào trong chuỗi ko được phép  (getMatch = true)
                                // -> báo lỗi
                                if (regex.IsMatch(_obValue))
                                {
                                    LastErrorEng += string.Format(errorStringEng, _obName, _cValue.Replace(" ", "<space>"), _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorStringVN, _obName, _cValue.Replace(" ", "<dấu cách>"), _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                break;
                            case "NOSTRING":
                                //Kí tự không được xuất hiện trong biến
                                regex = new Regex("[" + _cValue + "]", RegexOptions.Multiline);
                                //Nếu tồn tại bất kì 1 kí tự nào trong chuỗi ko được phép  (getMatch = true)
                                // -> báo lỗi
                                if (regex.IsMatch(_obValue))
                                {
                                    LastErrorEng += string.Format(errorNoStringEng, _obName, _cValue.Replace(" ", "<space>"), _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorNoStringVN, _obName, _cValue.Replace(" ", "<dấu cách>"), _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                break;
                            case "PREFIX":
                                //Prefix - Tiền tố
                                if (_obValue.Length < _cValue.Length || !_cValue.Equals(_obValue.Substring(0, _cValue.Length)))
                                {
                                    LastErrorEng += string.Format(errorPreFixEng, _obName, _cValue.Replace(" ", "<space>"), _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorPreFixVN, _obName, _cValue.Replace(" ", "<dấu cách>"), _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                break;
                            case "FIXED":
                                //Giá trị cố định - fix cứng
                                if (!_obValue.Equals(_cValue))
                                {
                                    LastErrorEng += string.Format(errorFixContentEng, _obName, _cValue.Replace(" ", "<space>"), _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorFixContentVN, _obName, _cValue.Replace(" ", "<dấu cách>"), _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                break;
                            case "FIXED POSITION":
                                _cFromPos = Int16.Parse(row["FROM_POSITION"].ToString());
                                _cToPos = Int16.Parse(row["TO_POSITION"].ToString());
                                if (_obValue.Length < _cValue.Length || !_cValue.Equals(_obValue.Substring(_cFromPos, _cToPos - _cFromPos + 1)))
                                {
                                    LastErrorEng += string.Format(errorFixPositionEng, _obName, _cValue, _cFromPos, _cToPos, _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorFixPositionVN, _obName, _cValue, _cFromPos, _cToPos, _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                //Fix giá trị tại vị trí
                                break;
                            case "LINKBARCODE":
                                //Text cùng giá trị với barcode 

                                break;
                            case "PROFIX":
                                //PostFix - Hậu tố
                                if (_obValue.Length < _cValue.Length || !_cValue.Equals(_obValue.Substring(_obValue.Length - _cValue.Length, _cValue.Length)))
                                {
                                    LastErrorEng += string.Format(errorPostFixEng, _obName, _cValue, _obValue.Replace(" ", "<space>")) + Environment.NewLine;
                                    LastErrorVN += string.Format(errorPostFixVN, _obName, _cValue, _obValue.Replace(" ", "<dấu cách>")) + Environment.NewLine;
                                }
                                break;
                            case "TYPE":
                                //Loại Font- Barcode
                                if (!_cValue.Contains(_obType))
                                {
                                    if (_obName.Contains("Barcode"))
                                    {
                                        LastErrorEng += string.Format(errorTypeEng, "BARCODE", _obName, "barcode type", _cValue, _obType) + Environment.NewLine;
                                        LastErrorVN += string.Format(errorTypeVN, "Mã vạch", _obName, "loại mã", _cValue, _obType) + Environment.NewLine;
                                    }
                                    else
                                    {
                                        LastErrorEng += string.Format(errorTypeEng, "FONT", _obName, "font", _cValue, _obType) + Environment.NewLine;
                                        LastErrorVN += string.Format(errorTypeVN, "Phông chữ", _obName, "kiểu phông", _cValue, _obType) + Environment.NewLine;
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception EX)
                { string error = EX.Message; }                               
            }
            if (!string.IsNullOrEmpty(LastErrorEng))
            {
                LastErrorEng = "Label tracking system found some errors." + Environment.NewLine + "Please call PE/Label & PQE check:" + Environment.NewLine + LastErrorEng;
                LastErrorVN = "Hệ thống label tracking phát hiện lỗi." + Environment.NewLine + "Hãy gọi PE/Label & PQE kiểm tra lỗi sau:" + Environment.NewLine + LastErrorVN;
                if (isShowErrorMessage)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageEnglish = LastErrorEng;
                    _sh.MessageVietNam = LastErrorVN;
                    _sh.ShowDialog();
                }                    
                return false;
            }
            return true;
        }

        public DataTable getPQESetup(string LabelName, int PathIndex)
        {
            var result =  sfcHttpClient.QueryList(new QuerySingleParameterModel()
            {
                CommandText = "SELECT DISTINCT VAR_NAME FROM SFIS1.C_LABEL_CONFIG_T WHERE UPPER(LABEL_NAME)='" + LabelName.ToUpper().Trim() + "' AND PATH_NAME=" + PathIndex,
                SfcCommandType = SfcCommandType.Text
            });
            DataTable dt = new DataTable();
            ResouceClass rc = new ResouceClass();
            if (result.Data != null)
            {                
                dt = rc.ToDataTable<LabelConfig>(result.Data.ToListObject<LabelConfig>());
            }
            return dt;
        }
        public string getLabelSetupPercent()
        {
            var result = sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = "SELECT NVL(VR_VALUE,0) VR_VALUE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='LABELTRACKING'",
                SfcCommandType = SfcCommandType.Text
            });
            return result.Data["vr_value"].ToString();
        }

        public string getWipLabelMD5(string LabelName, int PathIndex)
        {
            var result =  sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = "select md5 from sfis1.c_label_wip_t where UPPER(label_name)='" + LabelName.ToUpper().Trim() + "' and path_index=" + PathIndex,
                SfcCommandType = SfcCommandType.Text
            });
            string _md5 = "NULL";
            if (result.Data != null)
            {
                 _md5 = result.Data["md5"].ToString();
            }               
            return _md5;
        }

        public DataTable DatatableMinus(DataTable dt1, DataTable dt2, string field)
        {
            DataTable dtMerged =
                 (from a in dt1.AsEnumerable()
                  join b in dt2.AsEnumerable()
                                    on
          a[field].ToString() equals b[field].ToString()
                                  into g
                  where g.Count() == 0
                  select a).CopyToDataTable();
            return dtMerged;
        }
        public static DataTable CompareTwoDataTable(DataTable dt1, DataTable dt2)
        {
            dt1.Merge(dt2);
            DataTable d3 = dt2.GetChanges();
            return d3;
        }
        public string getLabeStatus(string LabelName, int PathIndex)
        {
            string mysql;
            if (PathIndex != 6)
            {
                mysql = "select 1*10+NVL(IPQC,'0') bb  from sfis1.c_label_wip_t where UPPER(label_name)='" + LabelName.ToUpper().Trim() + "' and path_index=" + PathIndex;
            }
            else
            {   //pathindex=6 must PD confirm!
                mysql = "select NVL(PD,'0')*10+NVL(IPQC,'0') bb from sfis1.c_label_wip_t where UPPER(label_name)='" + LabelName.ToUpper().Trim() + "' and path_index=" + PathIndex;
            }
            var result =  sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = mysql,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null)
            {
                return result.Data["bb"].ToString();
            }
            else
            {
                return "NULL";
            }
        }        

        public string CRC322(string FilePath)
        {

            uint[] crctab = new uint[] {
          0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419,
          0x706af48f, 0xe963a535, 0x9e6495a3, 0x0edb8832, 0x79dcb8a4,
          0xe0d5e91e, 0x97d2d988, 0x09b64c2b, 0x7eb17cbd, 0xe7b82d07,
          0x90bf1d91, 0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de,
          0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7, 0x136c9856,
          0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9,
          0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4,
          0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,
          0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3,
          0x45df5c75, 0xdcd60dcf, 0xabd13d59, 0x26d930ac, 0x51de003a,
          0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599,
          0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,
          0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 0x76dc4190,
          0x01db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f,
          0x9fbfe4a5, 0xe8b8d433, 0x7807c9a2, 0x0f00f934, 0x9609a88e,
          0xe10e9818, 0x7f6a0dbb, 0x086d3d2d, 0x91646c97, 0xe6635c01,
          0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed,
          0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950,
          0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3,
          0xfbd44c65, 0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2,
          0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a,
          0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5,
          0xaa0a4c5f, 0xdd0d7cc9, 0x5005713c, 0x270241aa, 0xbe0b1010,
          0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
          0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17,
          0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6,
          0x03b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x04db2615,
          0x73dc1683, 0xe3630b12, 0x94643b84, 0x0d6d6a3e, 0x7a6a5aa8,
          0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1, 0xf00f9344,
          0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb,
          0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a,
          0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
          0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1,
          0xa6bc5767, 0x3fb506dd, 0x48b2364b, 0xd80d2bda, 0xaf0a1b4c,
          0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef,
          0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
          0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe,
          0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31,
          0x2cd99e8b, 0x5bdeae1d, 0x9b64c2b0, 0xec63f226, 0x756aa39c,
          0x026d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x05005713,
          0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38, 0x92d28e9b,
          0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242,
          0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1,
          0x18b74777, 0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c,
          0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45, 0xa00ae278,
          0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7,
          0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc, 0x40df0b66,
          0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
          0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605,
          0xcdd70693, 0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8,
          0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b,
          0x2d02ef8d
        };
            FileStream fs = null;
            byte[] buffer = null;
            try
            {
                fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                int length = (int)fs.Length;  // get file length
                buffer = new byte[length];    // create buffer
                // read until Read method returns 0 (end of the stream has been reached)
                fs.Read(buffer, 0, length);
            }
            catch (Exception ex)
            {
                return "";

            }
            finally
            {
                if (fs != null)
                    fs.Close();

            }

            uint c = 0xffffffff;  // begin at shift register contents 
            int i, n = buffer.Length;
            for (i = 0; i < n; i++)
            {
                c = crctab[((int)c ^ buffer[i]) & 0xff] ^ (c >> 8);
            }
            string temp = (c ^ 0xffffffff).ToString();
            return (c ^ 0xffffffff).ToString();
        }
    }

    public class LabelConfig
    {
        public string VAR_NAME { get; set; }

    }
}
