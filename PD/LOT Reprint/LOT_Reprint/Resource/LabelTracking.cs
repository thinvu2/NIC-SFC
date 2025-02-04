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
using LOT_REPRINT.Resource;

namespace LOT_REPRINT.Resource
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
        public string LastError = "PASS";
        public SfcHttpClient sfcHttpClient { get; set; }
        public async Task<bool> doLabelTracking(string LabelName, int PathIndex, LabelManager2.Application LabApp, Document doc)
        {
            #region Check label file version
            string wipLabelMd5 = await this.getWipLabelMD5(LabelName, PathIndex);
            string CurrentFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + LabelName;
            string str1 = this.CRC322(CurrentFileName);
            if (!wipLabelMd5.Equals(str1))
            {
                LastError = "Label file Version error.\n\r File name=" + LabelName + ",\n\rpathIndex= " + PathIndex + " ,\n\rCurrentMD5=" + str1 + ",\n\rWipMD5=" + wipLabelMd5;
                return false;
            }
            #endregion

            #region Check label values
            try
            {
                LabApp.Documents.Open(CurrentFileName, false);
                LabApp.ActiveDocument.ViewMode = enumViewMode.lppxViewModeName;
                LabApp.ActiveDocument.ViewMode = enumViewMode.lppxViewModeValue;
                doc = LabApp.ActiveDocument;
                doc.ViewMode = LabApp.ActiveDocument.ViewMode;
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
                return false;
            }
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("VAR_NAME", typeof(string));
            dt1.Columns.Add("VALUE", typeof(string));
            dt1.Columns.Add("TYPE", typeof(string));

            //Check PQE pass label file
            string labeStatus = await this.getLabeStatus(LabelName, PathIndex);
            if (labeStatus != "11")
            {
                LastError = "Label file not confirmed!" + (labeStatus == "10" ? "Need PQE Confirm" : " Need Labelroom Confirm") + "\n\rCurrentFileName=" + LabelName + "\n\rMd5=" + wipLabelMd5 + "\n\rpathIndex=" + PathIndex + "\n\rLabel Flag=" + labeStatus + "\n\r";
                return false;
            }

            //Check labeltracking values
            try
            {
                int count1 = (int)((IDocument)doc).DocObjects.Barcodes.Count;
                int count2 = (int)((IDocument)doc).DocObjects.Texts.Count;
                int count3 = (int)((IDocument)doc).Variables.FormVariables.Count;
                int count4 = (int)((IDocument)doc).Variables.FreeVariables.Count;
                int count5 = (int)((IDocument)doc).Variables.Formulas.Count;
                for (int index = 1; index <= count1; ++index)
                {
                    string name = ((IDocument)doc).DocObjects.Barcodes.Item((object)index)._Name;
                    if (name != null)
                    {
                        string variableName = ((IDocument)doc).DocObjects.Barcodes.Item((object)index).VariableName;
                        string str3 = ((IDocument)doc).DocObjects.Barcodes.Item((object)index).Symbology.ToString().Replace("lppx", "").Trim();
                        string str4 = ((IDocument)doc).DocObjects.Barcodes.Item((object)index).Value;
                        dt1.Rows.Add((object)name, (object)str4, (object)str3);
                    }
                }

                for (int index = 1; index <= count2; ++index)
                {
                    string name1 = ((IDocument)doc).DocObjects.Texts.Item((object)index)._Name;
                    if (name1 != null)
                    {
                        string variableName = ((IDocument)doc).DocObjects.Texts.Item((object)index).VariableName;
                        string str2 = ((IDocument)doc).DocObjects.Texts.Item((object)index).Value;
                        string name2 = ((IDocument)doc).DocObjects.Texts.Item((object)index).Font.Name;
                        dt1.Rows.Add((object)name1, (object)str2, (object)name2);
                    }
                }

                //DataTable result = dt1;
                DataTable dataTable1 = new DataTable();
                DataTable pqeSetup = await this.getPQESetup(LabelName, PathIndex);
                int count6 = dt1.Rows.Count;
                int count7 = pqeSetup.Rows.Count;
                int num = int.Parse(await this.getLabelSetupPercent());
                if ((int)Math.Floor((double)count7 / (double)count6 * 100.0) < num)
                {
                    DataTable dataTable2 = this.DatatableMinus(dt1, pqeSetup, "VAR_NAME");
                    LastError = LastError + "\n\rPQE setup label not enough " + num.ToString() + "%.\n\rCall PQE continue setup on label tracking! Label name:" + LabelName + "\n\rList Object not setup:\n\r";
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        LastError = LastError + row["VAR_NAME"].ToString() + "\n\r";
                    return false;
                }
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
                return false;
            }

            DataTable dataTable3 = new DataTable("PQECONFIG");
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT VAR_NAME, VAR_REG, FLAG   FROM SFIS1.C_LABEL_CONFIG_T  WHERE VAR_REG NOT LIKE '@%' AND UPPER (LABEL_NAME) ='" + LabelName + "' AND PATH_NAME = " + PathIndex + " UNION SELECT A.VAR_NAME, B.REG_EXP, B.MEMO   FROM SFIS1.C_LABEL_CONFIG_T A, SFIS1.C_LABEL_REGEXP_T B  WHERE     UPPER (A.LABEL_NAME) ='" + LabelName + "' AND A.PATH_NAME = " + PathIndex + "   AND A.VAR_REG LIKE '@%'    AND A.VAR_REG = '@' || B.REG_NAME",
                SfcCommandType = SfcCommandType.Text
            });
            DataTable dataTable4 = new DataTable();
            ResouceClass rc = new ResouceClass();
            if (result.Data != null)
            {
                dataTable4 = rc.ToDataTable<LabelConfigNEW>(result.Data.ToListObject<LabelConfigNEW>());
            }
            //result1 = dataTable4;
            string _obName = "";
            string _obValue = "";
            string _obtype = "";
            string _pVarname = "";
            string _pVareg = "";
            string _pFlag = "";
            foreach (DataRow row1 in (InternalDataCollectionBase)dt1.Rows)
            {
                _obName = row1["VAR_NAME"].ToString();
                _obValue = row1["VALUE"].ToString();
                _obtype = row1["TYPE"].ToString();

                if (_obValue.StartsWith("#"))
                {
                    LastError = LastError + "[ERRO start with # character]" + _obName + ":" + _obValue + "\n\r";
                }
                foreach (DataRow row2 in (InternalDataCollectionBase)dataTable4.Rows)
                {
                    _pVarname = row2["VAR_NAME"].ToString();
                    _pVareg = row2["VAR_REG"].ToString();
                    _pFlag = row2["FLAG"].ToString();
                    if (_obName.Equals(_pVarname) && _pVareg.Substring(0, 1) != "=")
                    {
                        if (_pFlag == "LINKBARCODE")
                        {
                            var _datarow = dt1.AsEnumerable().Where(x => x.Field<string>("VAR_NAME") == _pVareg).FirstOrDefault();
                            string _linkvalue = _datarow["VALUE"].ToString();
                            if (_obValue != _linkvalue)
                                LastError = LastError + "[ERRO]" + _obName + ":" + _obValue + "," + _linkvalue + "," + _pFlag + "," + _pVareg + "\n\r";
                        }
                        else
                        {
                            string pattern = "^" + _pVareg + "$";
                            if (!new Regex(pattern, RegexOptions.Multiline).GetMatch(_obValue))
                                LastError = LastError + "[ERRO]" + _obName + ":" + _obValue + "," + pattern + "," + _pFlag + "\n\r";
                        }
                    }
                    else if (_obName.Equals(_pVarname) && _pVareg.Substring(0, 1) == "=" && _obtype != "")
                    {
                        if (_obName.Contains( "Barcode") && _pVarname.Contains ( "Barcode"))
                        {
                            if (_pVareg.Replace("=", "") != _obtype)
                                LastError = LastError + "[BARCODE ERROR]" + _obName + ":" + _obValue + "," + _obtype + "," + _pFlag + "\n\r";
                        }
                        else if (_obName.Contains( "Text" )&& _pVarname.Contains( "Text") && _pVareg.Replace("=", "").Trim() != _obtype)
                            LastError = LastError + "[FONT ERROR]" + _obName + ":" + _obValue + "," + _obtype + "," + _pFlag + "\n\r";
                    }
                }
            }
            if (LastError != "PASS")
            {
                return false;
            }
            #endregion

            return true;
        }

        public async Task<DataTable> getPQESetup(string LabelName, int PathIndex)
        {
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
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
        public async Task<string> getLabelSetupPercent()
        {
            var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = "SELECT NVL(VR_VALUE,0) VR_VALUE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='LABELTRACKING'",
                SfcCommandType = SfcCommandType.Text
            });
            return result.Data["vr_value"].ToString();
        }
        public async Task<string> getWipLabelMD5(string LabelName, int PathIndex)
        {
            var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
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
        public async Task<string> getLabeStatus(string LabelName, int PathIndex)
        {
            string sql = PathIndex == 6 ? "select NVL(PD,'0')*10+NVL(IPQC,'0') bb from sfis1.c_label_wip_t where UPPER(label_name)='" + LabelName.ToUpper().Trim() + "' and path_index=" + PathIndex.ToString() : "select decode(sfis,null,'0',sfis)||decode(ipqc,null,'0',ipqc) bb from sfis1.c_label_wip_t where UPPER(label_name)='" + LabelName.ToUpper().Trim() + "' and path_index=" + PathIndex.ToString();
            var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
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
            return (c ^ 0xffffffff).ToString();
        }
    }
    public class LabelConfig
    {
        public string VAR_NAME { get; set; }

    }
    public class LabelConfigNEW
    {
        public string VAR_NAME { get; set; }
        public string VAR_REG { get; set; }
        public string FLAG { get; set; }
    }

    public static class RegexExtensions
    {
        public static bool GetMatch(this Regex regex, string input)
        {
            Match match = regex.Match(input);
            if (match.Success)
            {

                return true;
            }
            return false;
        }
    }
}
