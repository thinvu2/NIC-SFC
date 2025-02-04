using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Model
{
    public class ListPrivilege
    {
        public string EMP { get; set; }
        public string PASSW { get; set; }
        public string FUN { get; set; }
        public int PRIVILEGE { get; set; }
        public string PRG_NAME { get; set; }
    }
    public class ListMO
    {
        public string MO_NUMBER { get; set; }
        public string MO_TYPE { get; set; }
        public string MODEL_NAME { get; set; }
        public string VERSION_CODE { get; set; }
        public string TARGET_QTY { get; set; }
        public string MO_CREATE_DATE { get; set; }
        public string MO_SCHEDULE_DATE { get; set; }
        public string MO_DUE_DATE { get; set; }
        public string MO_START_DATE { get; set; }
        public string MO_TARGET_DATE { get; set; }
        public string MO_CLOSE_DATE { get; set; }
        public string ROUTE_CODE { get; set; }
        public string INPUT_QTY { get; set; }
        public string OUTPUT_QTY { get; set; }
        public string TURN_OUT_QTY { get; set; }
        public string TOTAL_SCRAP_QTY { get; set; }
        public string START_SN { get; set; }
        public string END_SN { get; set; }
        public string SHIPPING_START_SN { get; set; }
        public string SHIPPING_QTY { get; set; }
        public string WORK_FLAG { get; set; }
        public string CLOSE_FLAG { get; set; }
        public string DEFAULT_LINE { get; set; }
        public string DEFAULT_GROUP { get; set; }
        public string CUST_CODE { get; set; }
        public string ORDER_NO { get; set; }
        public string BOM_NO { get; set; }
        public string MASTER_FLAG { get; set; }
        public string MASTER_MO { get; set; }
        public string END_GROUP { get; set; }
        public string PO_NO { get; set; }
        public string HW_BOM { get; set; }
        public string SW_BOM { get; set; }
        public string UPC_CO { get; set; }
        public string OPTION_DESC { get; set; }
        public string KEY_PART_NO { get; set; }
        public string CUST_PART_NO { get; set; }
        public string TRANS_DATE { get; set; }
        public string MO_OPTION { get; set; }
        public string SHIFT { get; set; }
        public string UPDATE_D { get; set; }
        public string REMARK { get; set; }
        public string CUSTPN { get; set; }
        public string PMCC { get; set; }
        public string MRP_CLOSE_FLAG { get; set; }
        public string KITTY_CLOSE_FLAG { get; set; }
        public string PCBA_CLOSE_FLAG { get; set; }
        public string BI_CLOSE_FLAG { get; set; }
        public string MANUAL_CLOSE_FLAG { get; set; }
        public string MSN_MO_OPTION { get; set; }
        public string IMEI_MO_OPTION { get; set; }
        public string START_MSN { get; set; }
        public string END_MSN { get; set; }
        public string START_IMEI { get; set; }
        public string END_IMEI { get; set; }
        public string JOB_MO_OPTION { get; set; }
        public string START_JOB { get; set; }
        public string END_JOB { get; set; }
        public string JOB_QTY { get; set; }
        public string SIGN_EMP { get; set; }
        public string SO_NUMBER { get; set; }
        public string SO_LINE { get; set; }
        public string MO_KP_START_DATE { get; set; }
        public string MO_KP_FINISH_DATE { get; set; }
        public string MO_FIRST_PCS_DATE { get; set; }
        public string MO_LAST_PCS_DATE { get; set; }
        public string PO_ITEM { get; set; }
        public string VENDER_PART_NO { get; set; }
        public string LOT_FLAG { get; set; }
        public string SITE { get; set; }
        public string MOVE_FLAG { get; set; }

    }
    public class ListRange
    {
        public string MO_NUMBER { get; set; }
        public string ITEM_1 { get; set; }
        public string VER_1 { get; set; }
        public string ITEM_2 { get; set; }
        public string VER_2 { get; set; }
        public string ITEM_3 { get; set; }
        public string VER_3 { get; set; }
        public string ITEM_4 { get; set; }
        public string VER_4 { get; set; }
        public string ITEM_5 { get; set; }
        public string VER_5 { get; set; }
        public string ITEM_6 { get; set; }
        public string VER_6 { get; set; }
    }
    public class LoadMO
    {
        public string MO_NUMBER { get; set; }
        public string MO_TYPE { get; set; }
        public string MODEL_NAME { get; set; }
        public int TARGET_QTY { get; set; }
        public string MO_SCHEDULE_DATE { get; set; }
        public string MO_DUE_DATE { get; set; }
        public string SO_NUMBER { get; set; }
        public string SO_LINE { get; set; }
        public string KEY_PART_NO { get; set; }
        public string CUST_CODE { get; set; }
        public string CUSTPN { get; set; }
        public string MO_KP_START_DATE { get; set; }
        public string MO_KP_FINISH_DATE { get; set; }
        public string CIS_SO { get; set; }
        public string CIS_SO_LINE { get; set; }
        public string TO846 { get; set; }
        public string WHS { get; set; }
        public string LOC { get; set; }
        public string SITE { get; set; }
        public string CUST_PO { get; set; }
        public string REFERENCE_MO { get; set; }
        public string SAP_MODEL_NAME { get; set; }
        public string SAP_MO_TYPE { get; set; }
        public string MODEL_SERIAL { get; set; }
        public string MODEL_TYPE { get; set; }
        public string BOM_NO { get; set; }
        public string STANDARD { get; set; }
        public string CUSTOMER { get; set; }
        public int MODEL_RANGE1 { get; set; }
        public int MODEL_RANGE2 { get; set; }
        //public int ROUTE_CODE { get; set; }
        public string DEFAULT_GROUP { get; set; }
        public string END_GROUP { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string LEAD_FREE { get; set; }
        public string REPAIR_1A2A_PROCESS { get; set; }
        public string VERSION_CODE { get; set; }
        public string CHECKFLAG { get; set; }
        public string ROUTE_NAME { get; set; }
        public string ROUTE_DESC { get; set; }
        public string PASSWD { get; set; }
        public string INI_FNAME { get; set; }
        public string INI_FVER { get; set; }
        public string BOARD_TYPE { get; set; }
    }
    public class ListDataWIP
    {
        public string SERIAL_NUMBER { get; set; }
        public string SECTION_FLAG { get; set; }
        public string MO_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string TYPE { get; set; }
        public string VERSION_CODE { get; set; }
        public string LINE_NAME { get; set; }
        public string SECTION_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public string LOCATION { get; set; }
        public string STATION_SEQ { get; set; }
        public string ERROR_FLAG { get; set; }
        public string IN_STATION_TIME { get; set; }
        public string IN_LINE_TIME { get; set; }
        public string OUT_LINE_TIME { get; set; }
        public string SHIPPING_SN { get; set; }
        public string WORK_FLAG { get; set; }
        public string FINISH_FLAG { get; set; }
        public string ENC_CNT { get; set; }
        public string SPECIAL_ROUTE { get; set; }
        public string PALLET_NO { get; set; }
        public string CONTAINER_NO { get; set; }
        public string QA_NO { get; set; }
        public string QA_RESULT { get; set; }
        public string SCRAP_FLAG { get; set; }
        public string NEXT_STATION { get; set; }
        public string CUSTOMER_NO { get; set; }
        public string BOM_NO { get; set; }
        public string BILL_NO { get; set; }
        public string TRACK_NO { get; set; }
        public string PO_NO { get; set; }
        public string KEY_PART_NO { get; set; }
        public string CARTON_NO { get; set; }
        public string WARRANTY_DATE { get; set; }
        public string REWORK_NO { get; set; }
        public string REPAIR_CNT { get; set; }
        public string EMP_NO { get; set; }
        public string PO_LINE { get; set; }
        public string PALLET_FULL_FLAG { get; set; }
        public string PMCC { get; set; }
        public string GROUP_NAME_CQC { get; set; }
        public string MO_NUMBER_OLD { get; set; }
        public string ERP_MO { get; set; }
        public string ATE_STATION_NO { get; set; }
        public string MSN { get; set; }
        public string IMEI { get; set; }
        public string JOB { get; set; }
        public string MCARTON_NO { get; set; }
        public string SO_NUMBER { get; set; }
        public string SO_LINE { get; set; }
        public string STOCK_NO { get; set; }
        public string TRAY_NO { get; set; }
        public string SHIP_NO { get; set; }
        public string WIP_GROUP { get; set; }
        public string SHIPPING_SN2 { get; set; }
    }
    public class ListBOM
    {
        public string Kp_relation { get; set; }
        public string Kp_name { get; set; }
        public string Key_part_no { get; set; }
        public string Group_name { get; set; }
        public string Kp_count { get; set; }
    }
    public class Root
    {
        public Root()
        {
            this.ItemsRoot = new ObservableCollection<Family>();
        }
        public string TitleRoot { get; set; }
        public ObservableCollection<Family> ItemsRoot { get; set; }
    }
    public class Family
    {
        public Family()
        {
            this.Members = new ObservableCollection<FamilyMemebr>();
        }
        public string Title { get; set; }
        public ObservableCollection<FamilyMemebr> Members { get; set; }
    }
    public class FamilyMemebr
    {
        public string Name { get; set; }
    }
    public class ListDepartment
    {
        public string ITEM_1 { get; set; }
        public string ITEM_2 { get; set; }
        public string ITEM_3 { get; set; }
    }
    public class STEP
    {
        public string MO_NUMBER { get; set; }
        public string REASON { get; set; }
    }
}
