using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CQC
{
    /// <summary>
    /// Interaction logic for SystemWindow.xaml
    /// </summary>
    public partial class SystemWindow : Window
    {
        INIFile ini = new INIFile("SFIS.ini");
        public SystemWindow(bool _chkbRejectReason, bool _chkbInsertQCSN, bool _chkbCheckRoute, bool _chkbUpdateQANoResult, bool _chkbRemoveFailSSN, bool _chkbInsertSNtoPallet, bool _chkbTransferbyPiece, bool _chkbWarehouseNO, bool _chkbPO, bool _chkbClearPallet, bool _chkbClearCarton, bool _chkbSamplingPlan, string _editPalletFullFlag, string _editCompany)
        {
            InitializeComponent();
            chkbRejectReason.IsChecked = _chkbRejectReason;
            chkbInsertQCSN.IsChecked = _chkbInsertQCSN;
            chkbInsertQCSN.IsChecked = _chkbInsertQCSN;
            chkbCheckRoute.IsChecked = _chkbCheckRoute;
            chkbUpdateQANoResult.IsChecked = _chkbUpdateQANoResult;
            chkbRemoveFailSSN.IsChecked = _chkbRemoveFailSSN;
            chkbInsertSNtoPallet.IsChecked = _chkbInsertSNtoPallet;
            chkbTransferbyPiece.IsChecked = _chkbTransferbyPiece;
            chkbWarehouseNO.IsChecked = _chkbWarehouseNO;
            chkbPO.IsChecked = _chkbPO;
            chkbClearPallet.IsChecked = _chkbClearPallet;
            chkbClearCarton.IsChecked = _chkbClearCarton;
            chkbSamplingPlan.IsChecked = _chkbSamplingPlan;
            editPalletFullFlag.Text = _editPalletFullFlag;
            editCompany.Text = _editCompany;
        }

        private void BitBtn2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bbtnOK_Click(object sender, RoutedEventArgs e)
        {
            //Company
            ini.Write("CQC", "Company", editCompany.Text.ToUpper());
            editCompany.Text = editCompany.Text.ToUpper();

            //Reject Reason
            if (chkbRejectReason.IsChecked == true)
                ini.Write("CQC", "Reject Reason", "Y");
            else
                ini.Write("CQC", "Reject Reason", "N");

            //Insert QC_SN_T , R_CQC_REC_T line name,section name ,group name, station name
            if (chkbInsertQCSN.IsChecked == true)
                ini.Write("CQC", "Insert Sec&Grp", "Y");
            else
                ini.Write("CQC", "Insert Sec&Grp", "Y");

            //Pallet Full Flag
            editPalletFullFlag.Text = editPalletFullFlag.Text.ToUpper();
            if (editPalletFullFlag.Text != "Y" && editPalletFullFlag.Text != "N")
                editPalletFullFlag.Text = "Y";
            ini.Write("CQC", "Pallet Full Flag", editPalletFullFlag.Text);

            //Check Route
            if (chkbCheckRoute.IsChecked == true)
                ini.Write("CQC", "Check Route", "Y");
            else
                ini.Write("CQC", "Check Route", "N");

            //Update R107 QA_NO,QA_RESULT='N/A'
            if (chkbUpdateQANoResult.IsChecked == true)
                ini.Write("CQC", "Update R107 QA_NO&Result", "Y");
            else
                ini.Write("CQC", "Update R107 QA_NO&Result", "N");

            //Remove SSN
            if (chkbRemoveFailSSN.IsChecked == true)
                ini.Write("CQC", "Remove Fail SSN/SN", "Y");
            else
                ini.Write("CQC", "Remove Fail SSN/SN", "N");

            //Insert new SSN to Pallet
            if (chkbInsertSNtoPallet.IsChecked == true)
                ini.Write("CQC", "Insert New SN to Pallet / Carton", "Y");
            else
                ini.Write("CQC", "Insert New SN to Pallet / Carton", "N");

            //Transfer by piece
            if (chkbTransferbyPiece.IsChecked == true)
                ini.Write("CQC", "Transfer by Piece", "Y");
            else
                ini.Write("CQC", "Transfer by Piece", "N");

            //Warehouse NO
            if (chkbWarehouseNO.IsChecked == true)
                ini.Write("CQC", "Warehouse NO", "Y");
            else
                ini.Write("CQC", "Warehouse NO", "N");

            //po no + po line
            if (chkbPO.IsChecked == true)
                ini.Write("CQC", "BY PO NO+PO Line", "Y");
            else
                ini.Write("CQC", "BY PO NO+PO Line", "N");

            //clear pallet no
            if (chkbClearPallet.IsChecked == true)
                ini.Write("CQC", "CLEAR PALLET NO", "Y");
            else
                ini.Write("CQC", "CLEAR PALLET NO", "N");


            //clear carton no
            if (chkbClearCarton.IsChecked == true)
                ini.Write("CQC", "CLEAR CARTON NO", "Y");
            else
                ini.Write("CQC", "CLEAR CARTON NO", "N");

            //OQA SAMPLING PLAN
            if (chkbSamplingPlan.IsChecked == true)
                ini.Write("CQC", "OQA SAMPLING PLAN", "Y");
            else
                ini.Write("CQC", "OQA SAMPLING PLAN", "N");
            this.Close();
        }
    }
}
