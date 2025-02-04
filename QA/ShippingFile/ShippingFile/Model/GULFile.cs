using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ShippingFile.Model
{
    public class GULFile
    {
        public Header Header { get; set; }
        public List<Module> Body { get; set; }
    }

    public class Header
    {
        [Column("PRODUCT_LOCATION")]
        public string Factory { get; set; }
        [Column("PO_NO")]
        public string PoNumber { get; set; }
        [Column("DN_NO")]
        public string ShipNo { get; set; }
        [Column("MANUALINVOICE")]
        public string InvoiceNo { get; set; }
        [Column("DELIVERY_DATE")]
        public string ShipDate { get; set; }
        [Column("WARRANTY_EXPIRED_DATE")]
        public string WarrExpDate { get; set; }
    }

    public class Module
    {
        [Column("SERIAL_NUMBER")]//Z107
        public string ID { get; set; }

        [Column("CUST_MODEL_NAME")] //C_CINTERION_SHIP_T.DIVICE_NUMBER || '-' || SUBSTR (R105.SW_BOM, 1, 2)
        public string PartNo { get; set; }

        [Column("REVISION")]//R_MO_BASE_T.HW_BOM
        public string Revision { get; set; }

        [Column("IMEI")]//Z107
        public string Pallet { get; set; }

        [Column("MCARTON_NO")]//Z107
        public string Box { get; set; }

        [Column("TRAY_NO")]//Z107
        public string Reel { get; set; }

        [Column("PRODUCT_DATE")]//TO_CHAR (R105.MO_KP_START_DATE, 'YYYYMMDD')
        public string ProdDate { get; set; }

        [Column("PRODUCT_TIME")]//TO_CHAR (R105.MO_KP_START_DATE, 'HH24MISS')
        public string ProdTime { get; set; }

        [Column("PRODUCT_DC")]//Fix YM from YYYYMM in sql with IN_LINE_TIME value
        public string ProdDateCode { get; set; }

        [Column("PACKING_DATE")]//TO_CHAR (R117.IN_STATION_TIME, 'YYYYMMDD') : PACK_CTN/CHECK_MSL
        public string PackDate { get; set; }

        [Column("FT_DATE")]//TO_CHAR (R117.IN_STATION_TIME, 'YYYYMMDD') : FT
        public string TestDate { get; set; }

        [Column("PRODUCT_TYPE")]//C_CINTERION_SHIP_T.CUST_MODEL_NAME
        public string Product { get; set; }

        [Column("MATNOFACTORY")]//Z107.model_name
        public string MatNoFactory { get; set; }

        [Column("BOARDNO")]
        public string BoardNo { get; set; }

        [Column("VERSION_CODE")]
        public string Version_code { get; set; }

        [Column("PANEL_NO")]
        public string PanelNo { get; set; }

        [Column("SW")]
        public string SwRel { get; set; }

        [Column("A23")]
        public string nc { get; set; }

        [Column("A24")]
        public string spc { get; set; }

        [Column("CERTIFICATE_ID")]
        public string tlscid { get; set; }

        [Column("BBSN")]
        public string bbsn { get; set; }

        [Column("A27")]
        public string mspc { get; set; }

        [Column("MPCKEY")]
        public string mpc { get; set; }

        [Column("PWFACTORY")]
        public string nmc { get; set; }

        [Column("PWPRETLS")]
        public string spmc { get; set; }

        [Column("CERTIFICATE_PROFILE")]
        public string tlspr { get; set; }

        [Column("A32")]
        public string passw { get; set; }

        [Column("A33")]
        public string nsmc { get; set; }

        [Column("ACCKEY")]
        public string srvcd { get; set; }

        [Column("TEST_LINE")]
        public string ProdLine { get; set; }

        [Column("PART_NUMBER_SHIELD")]
        public string PartnoShield { get; set; }

        [Column("PART_NUMBER_SHIELD")]
        public string CUID { get; set; }

        [Column("PART_NUMBER_CUSTOMER")]
        public string PartnoTest { get; set; }

        [Column("A42")]
        public string simnr { get; set; }

        [Column("FLOOR_TIME")]
        public string FloorTime { get; set; }

        [Column("SERIAL_NUMBER")]
        public string DataMatrix { get; set; }

        [Column("BAG_TIME")]
        public string BagSealDateTime { get; set; }

        [Column("A45")]
        public string Meid { get; set; }

        [Column("A46")]
        public string Akey { get; set; }

        [Column("FLASH_KEY")]
        public string FlashId { get; set; }

        [Column("EAN")]
        public string EAN { get; set; }

        [Column("INFO01")]
        public string Inf01 { get; set; }

        [Column("INF02")]
        public string Inf02 { get; set; }

        [Column("A51")]
        public string Inf03 { get; set; }

        [Column("INFO04")]
        public string Inf04 { get; set; }

        [Column("INFO05")]
        public string Inf05 { get; set; }

        [Column("INFO06")]
        public string Inf06 { get; set; }

        [Column("A55")]
        public string Inf07 { get; set; }

        [Column("A56")]
        public string Inf08 { get; set; }

        [Column("A57")]
        public string Inf09 { get; set; }

        [Column("A58")]
        public string Inf10 { get; set; }

        [Column("A59")]
        public string EUICCID { get; set; }

        public string UIM { get; set; }

        public string MAC { get; set; }

        public string MBSN { get; set; }

        [Column("MODULE_IMEI")]
        public string IMEI { get; set; }

        public string FWVERSION { get; set; }

        [Column("QR_CODE")]
        public string QRCODE { get; set; }

        public string ACTIVDATE { get; set; }

        public string DOCREEL { get; set; }

        public string IMSI { get; set; }

        [Column("RESET_CODE")]
        public string CIRC { get; set; }

        public string DTLSPSK { get; set; }
    }

    /// <summary>
    /// 屬性別名，用來與查詢結果中的字段名映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ColumnAttribute : Attribute
    {
        public string Name;

        public ColumnAttribute(string Name)
        {
            this.Name = Name;

        }
    }
}
