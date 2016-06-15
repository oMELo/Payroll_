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
using MahApps;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using AdmereX;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.IO;

using Microsoft.Win32;

namespace Payroll_
{
    /// <summary>
    /// Interaction logic for CalAttendance.xaml
    /// </summary>
    public partial class CalAttendance 
    {
        string Dept = "";
        string Site="";
        string FullSearch = "";
        string selEmpNo = "";
        Database _Database = new Database();
      
        public CalAttendance()
        {

            InitializeComponent();
            //pPeriod("");
            //dtCalPeriod.ItemsSource = clsAttCal._AttCal;

        }

        public List<clsAttCal> pPeriod(string Condition)
        {

                dtCalPeriod.ItemsSource = "";
                clsAttCal._AttCal.Clear();
                MySqlDataReader _Reader;
                //concat(fld_FirstName, ' ' , fld_MiddleName,' ',fld_LastName) 
                MySqlCommand command = new MySqlCommand("select concat(ifnull(fld_FirstName,'') ,' ', ifnull(fld_MiddleName,'') , ' ' ,ifnull(fld_LastName,'')) as 'FullName', tA.* " +
                            "from admx_hrisp.tbl_empmasterfile M " +
                            "inner join admx_hrisp.pp_tempattendances tA " +
                            "on M.fld_IDNumber = tA.EmployeeNo " +
                            "where tA.payrollID =" + clsPPeriod._SelID + Condition,_Database.Connection);
                _Reader = command.ExecuteReader();
        
                {
                    while (_Reader.Read())
                    {
                        clsAttCal._AttCal.Add(new clsAttCal()
                        {
                            _ID = Convert.ToInt32(_Reader["ID"].ToString()),
                            _Fname = Convert.ToString(_Reader["FullName"].ToString()),
                            _EmpID = Convert.ToInt32(_Reader["EmployeeNo"].ToString()),
                            _EStat = Convert.ToString(_Reader["EmpStatus"].ToString()),
                            _Mata = Convert.ToDecimal(_Reader["Total"].ToString()),
                            _Regular = Convert.ToDecimal(_Reader["Regular"].ToString()),
                            _LegHoliday = Convert.ToDecimal(_Reader["Legalholiday"].ToString()),
                            _OTRegular = Convert.ToDecimal(_Reader["OTRegular"].ToString()),
                            _OTRestDay = Convert.ToDecimal(_Reader["OTRestDay"].ToString()),
                            _OTLegHoliday = Convert.ToDecimal(_Reader["OTLegalholiday"].ToString()),
                            _OTSpeHoliday = Convert.ToDecimal(_Reader["OTSpecialHoliday"].ToString()),
                            _Absences = Convert.ToDecimal(_Reader["Absences"].ToString()),
                            _Late = Convert.ToDecimal(_Reader["Tardiness"].ToString()),
                            _VL = Convert.ToDecimal(_Reader["VL"].ToString()),
                            _SL = Convert.ToDecimal(_Reader["SL"].ToString()),
                            _LWOP = Convert.ToString(_Reader["LWOP"].ToString()),
                        });

                    } _Reader.Close();
                }

                return clsAttCal._AttCal;
              
          
        }

  
        private Boolean isNum(string _Input)
        {
            int test;
            return int.TryParse(_Input, out test);

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if ((isNum(txtSearch.Text)) == true)
            //{
        
            //    pPeriod(" and fld_IDNumber like '%" + txtSearch.Text + "%' ");
            //    dtCalPeriod.ItemsSource = "";
            //    dtCalPeriod.ItemsSource = clsAttCal._AttCal;
            //}
            //else
            //{
                //btViewDetails.IsEnabled = true;
            //FullSearch = " and concat(fld_FirstName, ' ' , fld_MiddleName,' ',fld_LastName) like '%" + txtSearch.Text + "%' " + Site + Dept;
            FullSearch = " and concat(ifnull(fld_FirstName,'') ,' ', ifnull(fld_MiddleName,'') , ' ' ,ifnull(fld_LastName,'')) like '%" + txtSearch.Text + "%' " + Site + Dept;

            pPeriod(FullSearch);
           
            dtCalPeriod.ItemsSource = clsAttCal._AttCal;
            //}
        }
        private void dtCalPeriod_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (dtCalPeriod.SelectedIndex >= 0)
            {
                object item = dtCalPeriod.SelectedItem;
                clsChecking._SelEmpNO = Convert.ToString((dtCalPeriod.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text);
                TName.Content = Convert.ToString((dtCalPeriod.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text);
                btViewDetails.IsEnabled = true;
                btReCompute.IsEnabled = true;
                btDelete.IsEnabled = true;
                
            }
            else
            {
                btViewDetails.IsEnabled = false;
                btReCompute.IsEnabled = false;
                btDelete.IsEnabled = false;
            }
       
        }
        private void dtCalPeriod_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            int AttID = 0;
            object item = dtCalPeriod.SelectedItem;
            AttID = Convert.ToInt32((dtCalPeriod.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text);
          
             //MessageBoxResult Ans =  MessageBox.Show("Are you sure you want to save this","Save",MessageBoxButton.YesNo);
             //if (Ans = MessageBoxResult.Yes)
             //{
                 _Database.Execute("UPDATE admx_hrisp.pp_tempattendances set " +
                      "Total=" + Convert.ToDecimal((dtCalPeriod.SelectedCells[3].Column.GetCellContent(item) as NumericUpDown).Value) +
                     ",Regular=" + Convert.ToDecimal((dtCalPeriod.SelectedCells[4].Column.GetCellContent(item) as NumericUpDown).Value) +
                     ",LegalHoliday=" + Convert.ToDecimal((dtCalPeriod.SelectedCells[5].Column.GetCellContent(item) as NumericUpDown).Value) +
                     ",OTRegular=" + Convert.ToDecimal((dtCalPeriod.SelectedCells[6].Column.GetCellContent(item) as NumericUpDown).Value) +
                     ",OTRestday=" + Convert.ToDecimal((dtCalPeriod.SelectedCells[7].Column.GetCellContent(item) as NumericUpDown).Value) +
                     ",OTLegalHoliday=" + Convert.ToDecimal((dtCalPeriod.SelectedCells[8].Column.GetCellContent(item) as NumericUpDown).Value) +
                     ",OTSpecialHoliday=" + Convert.ToDecimal((dtCalPeriod.SelectedCells[9].Column.GetCellContent(item) as NumericUpDown).Value) +
                     ",Absences=" + Convert.ToDecimal((dtCalPeriod.SelectedCells[10].Column.GetCellContent(item) as NumericUpDown).Value) +
                     ",Tardiness=" + Convert.ToDecimal((dtCalPeriod.SelectedCells[11].Column.GetCellContent(item) as NumericUpDown).Value) +
                     ",VL=" + Convert.ToDecimal((dtCalPeriod.SelectedCells[12].Column.GetCellContent(item) as NumericUpDown).Value) +
                     ",SL= " + Convert.ToDecimal((dtCalPeriod.SelectedCells[13].Column.GetCellContent(item) as NumericUpDown).Value) +
                     " where ID = " + AttID);
             //}

        }
        private void btSYNC_Click(object sender, RoutedEventArgs e)
        {
            _Database.Execute("INSERT INTO admx_hrisp.pp_attendances ( PayrollId,EmployeeNo,Total,Regular,LegalHoliday,OTRegular,OTRestday,OTLegalHoliday,OTSpecialHoliday,Absences,Tardiness,VL,SL ) " +
            "select PayrollId,EmployeeNo,Total,Regular,LegalHoliday,OTRegular,OTRestday,OTLegalHoliday,OTSpecialHoliday,Absences,Tardiness,VL,SL  from admx_hrisp.pp_tempattendances where EmployeeNo not in (select EmployeeNo from admx_hrisp.pp_attendances where payrollid = "+ clsPPeriod._SelID+")");
            
            _Database.Execute("update admx_hrisp.pp_attendances as A1 " +
                                "inner join admx_hrisp.pp_tempattendances as A2 " +
                                "on A1.EmployeeNo=A2.EmployeeNo " +
                                "SET  " +
                                "A1.Total=A2.Total, " +
                                "A1.Regular=A2.Regular, " +
                                "A1.LegalHoliday=A2.LegalHoliday, " +
                                "A1.OTRegular=A2.OTRegular, " +
                                "A1.OTRestday=A2.OTRestday, " +
                                "A1.OTLegalHoliday=A2.OTLegalHoliday, " +
                                "A1.OTSpecialHoliday=A2.OTSpecialHoliday, " +
                                "A1.Absences=A2.Absences, " +
                                "A1.Tardiness=A2.Tardiness, " +
                                "A1.VL=A2.VL, " +
                                "A1.SL=A2.SL " +
                                "where A2.PAYROLLID = "+ clsPPeriod._SelID+" and A1.PAYROLLID ="+ clsPPeriod._SelID);
            
            MessageBox.Show("Success");
        }

        private void dtCalPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
    
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
           // txtPARAM.Text = "X : " + System.Windows.Forms.Control.MousePosition.X + "Y : " + System.Windows.Forms.Control.MousePosition.Y;
            //if (Convert.ToUInt32(System.Windows.Forms.Control.MousePosition.X) >= 1279 && Convert.ToUInt32(System.Windows.Forms.Control.MousePosition.Y) >= 64 && selEmpNo.Length > 0)
            //{
            //    FlyOut.IsOpen = true;
            //}
        }


        private void btViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (btViewDetails.Content.ToString() == "View Details")
            {
               // FlyOut.IsOpen = true;
                MainFrame.Visibility = Visibility.Visible;
                btViewDetails.Content = "Close";
                MainFrame.Navigate(new Uri("EmpInfo.xaml", UriKind.Relative));
            }
            else
            {
                clsChecking._SelEmpNO = "";
                MainFrame.Content = null;
                TName.Content = "";
                btViewDetails.IsEnabled = false;
                MainFrame.Visibility = Visibility.Hidden;
                btViewDetails.Content = "View Details";

            
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {

        }

        

        private void cmbSite_DropDownClosed(object sender, EventArgs e)
        {
            dtCalPeriod.ItemsSource = "";
            Site=" and fld_site=" + Convert.ToInt32(cmbSite.SelectedValue);
            dtCalPeriod.ItemsSource = pPeriod(Site + Dept);
 
        }

        private void cmbSite_DropDownOpened(object sender, EventArgs e)
        {
            fillCMB _CMBData = new fillCMB();
            _CMBData.getCMB(cmbSite, 9);
        }

        private void cmbSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog _SaveFileDialog = new SaveFileDialog();
            _SaveFileDialog.DefaultExt = ".xls";
            _SaveFileDialog.Filter = "Excel Files (*.xls)|*.xls";

            Nullable<bool> result = _SaveFileDialog.ShowDialog();


            if (result == true && clsAttCal._AttCal.Count > 0)
            {

                string filename = _SaveFileDialog.FileName;

                Microsoft.Office.Interop.Excel.Application oXL;
                Microsoft.Office.Interop.Excel._Workbook oWB;
                Microsoft.Office.Interop.Excel._Worksheet oSheet;
                Microsoft.Office.Interop.Excel.Range oRng;
                object misvalue = System.Reflection.Missing.Value;


                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = true;


                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

                oSheet.Cells[1, 1] = "PayrollID";
                oSheet.Cells[1, 2] = "Employee Name";
                oSheet.Cells[1, 3] = "EmployeeNo";
                oSheet.Cells[1, 4] = "Employee Status";
                oSheet.Cells[1, 5] = "Total";
                oSheet.Cells[1, 6] = "Regular";
                oSheet.Cells[1, 7] = "LegalHoliday";
                oSheet.Cells[1, 8] = "OTRegular";
                oSheet.Cells[1, 9] = "OTRestday";
                oSheet.Cells[1, 10] = "OTLegalHoliday";
                oSheet.Cells[1, 11] = "OTSpecialHoliday";
                oSheet.Cells[1, 12] = "Absences";
                oSheet.Cells[1, 13] = "Tardiness";
                oSheet.Cells[1, 14] = "VL";
                oSheet.Cells[1, 15] = "SL";
                oSheet.Cells[1, 16] = "LWOP";

                oSheet.get_Range("A1", "P1").Font.Bold = true;
           

                string[,] EmployeePayroll = new string[clsAttCal._AttCal.Count, 16];
                for (int i = 0; i < clsAttCal._AttCal.Count; i++)
                {
                    EmployeePayroll[i, 0] = clsPPeriod._SelID.ToString();
                    EmployeePayroll[i, 1] = clsAttCal._AttCal[i]._Fname.ToString();
                    EmployeePayroll[i, 2] = clsAttCal._AttCal[i]._EmpID.ToString();
                    EmployeePayroll[i, 3] = clsAttCal._AttCal[i]._EStat.ToString();
                    EmployeePayroll[i, 4] = clsAttCal._AttCal[i]._Mata.ToString();
                    EmployeePayroll[i, 5] = clsAttCal._AttCal[i]._Regular.ToString();
                    EmployeePayroll[i, 6] = clsAttCal._AttCal[i]._LegHoliday.ToString();
                    EmployeePayroll[i, 7] = clsAttCal._AttCal[i]._OTRegular.ToString();
                    EmployeePayroll[i, 8] = clsAttCal._AttCal[i]._OTRestDay.ToString();
                    EmployeePayroll[i, 9] = clsAttCal._AttCal[i]._OTLegHoliday.ToString();
                    EmployeePayroll[i, 10] = clsAttCal._AttCal[i]._OTSpeHoliday.ToString();
                    EmployeePayroll[i, 11] = clsAttCal._AttCal[i]._Absences.ToString();
                    EmployeePayroll[i, 12] = clsAttCal._AttCal[i]._Late.ToString();
                    EmployeePayroll[i, 13] = clsAttCal._AttCal[i]._VL.ToString();
                    EmployeePayroll[i, 14] = clsAttCal._AttCal[i]._SL.ToString();
                    EmployeePayroll[i, 15] = (clsAttCal._AttCal[i]._LWOP.ToString().Length == 0 ? "'" : clsAttCal._AttCal[i]._LWOP.ToString()) ;

                }

                oSheet.get_Range("A2", Convert.ToString(String.Format("{0}", "P" + (clsAttCal._AttCal.Count + 1)))).Value2 = EmployeePayroll;


                oRng = oSheet.get_Range("A1", "N1");
                oRng.EntireColumn.AutoFit();
                oXL.Visible = false;
                oXL.UserControl = false;
                oWB.SaveAs(_SaveFileDialog.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                oWB.Close();
                await this.ShowMessageAsync("Exported", "Successfully Exported");

    
            }
            else await this.ShowMessageAsync("Exported", "No Record Found");

        }

        private async void btUpload_Click(object sender, RoutedEventArgs e)
        {
            int _Updated = 0;
            int _Inserted = 0;
            Database _DBPayroll = new Database();
            OpenFileDialog _openFileDialog = new OpenFileDialog();

        
            _openFileDialog.Filter = "Excel Files (.xls)|*.xls|All Files (*.xls)|*.xls";
            _openFileDialog.FilterIndex = 1;

            _openFileDialog.Multiselect = true;

            bool? userClickedOK = _openFileDialog.ShowDialog();

            if (userClickedOK == true)


            {
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook;
                Excel.Worksheet worksheet;
                Excel.Range range;
                workbook = excelApp.Workbooks.Open(_openFileDialog.FileName);
                worksheet = (Excel.Worksheet)workbook.Sheets["Sheet1"];

                int column = 0;
                int row = 0;

                range = worksheet.UsedRange;

                DataTable dt = new DataTable();
                dt.Columns.Add("PayrollID");
                dt.Columns.Add("Employee Name");
                dt.Columns.Add("EmployeeNo");
                dt.Columns.Add("Employee Status");
                dt.Columns.Add("Total");
                dt.Columns.Add("Regular");
                dt.Columns.Add("LegalHoliday");
                dt.Columns.Add("OTRegular");
                dt.Columns.Add("OTRestday");
                dt.Columns.Add("OTLegalHoliday");
                dt.Columns.Add("OTSpecialHoliday");
                dt.Columns.Add("Absences");
                dt.Columns.Add("Tardiness");
                dt.Columns.Add("VL");
                dt.Columns.Add("SL");
                dt.Columns.Add("LWOP");

                var controller = await this.ShowProgressAsync("Please wait...", "UPLOADING!");
                controller.SetIndeterminate();
                await TaskEx.Delay(2000);
                controller.SetCancelable(true);    

                for (row = 2; row <= range.Rows.Count; row++)
                {
                    DataRow dr = dt.NewRow();
                    for (column = 1; column <= range.Columns.Count; column++)
                    {
                        dr[column - 1] = (range.Cells[row, column] as Excel.Range).Value2.ToString();
                    }

                    _DBPayroll.Open("Select count(*) as 'Count' from admx_hrisp.pp_tempattendances where PayrollId = " + dr[0] + " and EmployeeNo = " + dr[2]);
                    while (_DBPayroll.Reader.Read())
                    {
                        if (Convert.ToInt32(_DBPayroll.Reader["Count"].ToString()) > 0)
                        {

                                await Task.Run(() => _DBPayroll.Execute("UPDATE admx_hrisp.pp_tempattendances set " +
                                   "Total=" + dr[3] +
                                  ",Regular=" + dr[4] +
                                  ",LegalHoliday=" + dr[5] +
                                  ",OTRegular=" + dr[6] +
                                  ",OTRestday=" + dr[7] +
                                  ",OTLegalHoliday=" + dr[8] +
                                  ",OTSpecialHoliday=" + dr[9] +
                                  ",Absences=" + dr[10] +
                                  ",Tardiness=" + dr[11] +
                                  ",VL=" + dr[12] +
                                  ",SL= " + dr[13] +
                                  ",LWOP= '" + dr[14] +
                                  "' where PayrollId = " + dr[0] + " and EmployeeNo = " + dr[2]));
                            _Updated++;

                        }
                        else
                        {
                            await Task.Run(() => _DBPayroll.Execute("INSERT INTO admx_hrisp.pp_tempattendances ( PayrollId,EmployeeNo,Total,Regular,LegalHoliday,OTRegular,OTRestday,OTLegalHoliday,OTSpecialHoliday,Absences,Tardiness,VL,SL) VALUES (" + clsPPeriod._SelID.ToString() + "," + dr[2] + "," + dr[3] + "," + dr[4] + "," + dr[5] + "," + dr[6] + "," + dr[7] + "," + dr[8] + "," + dr[9] + "," + dr[10] + "," + dr[11] + "," + dr[12] + "," + dr[13] + "," + dr[14] + ")"));
                            _Inserted++;
                        }
                        controller.SetMessage("Employee ID : " + Convert.ToString( dr[2]));
                        await TaskEx.Delay(50);
                    }
                }
                workbook.Close(true, Type.Missing, Type.Missing);
                excelApp.Quit();
                dtCalPeriod.ItemsSource = "";
                dtCalPeriod.ItemsSource = pPeriod("");
                await controller.CloseAsync();
                await this.ShowMessageAsync("Uploaded", Environment.NewLine + Environment.NewLine + "Total Updated: " + _Updated + Environment.NewLine + "Total Inserted: " + _Inserted);
            }
        }

        private async void btReCompute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clsTotalEmpCalc _clsTotalEmpCalc = new clsTotalEmpCalc();
                object item = dtCalPeriod.SelectedItem;
            
                using (Database _Database = new Database())
                {
                    // "Total="+ _clsTotalEmpCalc.getMATA(clsChecking._SelEmpNO,  clsChecking._StartMata, clsChecking._EndMata) +
                                var _Val = _clsTotalEmpCalc.get_Regular_Absences_Late(clsChecking._SelEmpNO, clsChecking._StartMata, clsChecking._EndMata);
                                var _LeaveVal = _clsTotalEmpCalc.getLeaves(clsChecking._SelEmpNO);
                                _Database.Execute("UPDATE admx_hrisp.pp_tempattendances SET " + 
                                        "Total="+ _Val.Item1 +
                                        ", Regular=" + _Val.Item2 +
                                        ", OTRegular=" + _clsTotalEmpCalc.getRegularOT(clsChecking._SelEmpNO,clsPPeriod._SelID) + 
                                        ", OTRestday="+ _clsTotalEmpCalc.getRestDayOT(clsChecking._SelEmpNO,clsPPeriod._SelID) +
                                        ", OTLegalHoliday=" + _clsTotalEmpCalc.getHolidayOT(clsChecking._SelEmpNO, clsPPeriod._SelID) +
                                        ", OTSpecialHoliday=" + _clsTotalEmpCalc.getSpecialHolidayOT(clsChecking._SelEmpNO, clsPPeriod._SelID) +
                                        ", Absences=" + _Val.Item3 +
                                        ", Tardiness=" + _Val.Item4 +
                                        ", VL=" + _LeaveVal.Item1 +
                                        ", SL=" + _LeaveVal.Item2 +
                                        ", LWOP=" + _Val.Item5 +
                                        " WHERE Id=" + Convert.ToString((dtCalPeriod.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text));
                              cmbSite_DropDownClosed(null, null);
                }
                await this.ShowMessageAsync("Re-Compute", "Successfully Computed");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message,"Recompute", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object item = dtCalPeriod.SelectedItem;
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No",
                    FirstAuxiliaryButtonText = "Cancel",
                    ColorScheme = MetroDialogOptions.ColorScheme
                };

                MessageDialogResult result = await this.ShowMessageAsync("Delete", "Do you want to delete this ? " + Convert.ToString((dtCalPeriod.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text),
                    MessageDialogStyle.AffirmativeAndNegative, mySettings);

                if (result == MessageDialogResult.Affirmative)
                {
                    using (Database _Database = new Database())
                    {
                        
                        _Database.Execute("delete from admx_hrisp.pp_tempattendances where ID = " + Convert.ToString((dtCalPeriod.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text));
                        btViewDetails.Content = "Close";
                        cmbSite_DropDownClosed(null, null);
                        await this.ShowMessageAsync("Delete", "Successfully Deleted");
                    }
                }
            }
           
         
            catch (MySqlException ex)
            {
               
                MessageBox.Show(ex.Message, "Delete", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
 
        private async void btAdd_Click(object sender, RoutedEventArgs e)
        {
            
            var _EMP = await this.ShowInputAsync("EMPLOYEE ID", "Enter Employee ID");

            if (_EMP == null) 
            return;
            using (Database _Database = new Database())
            {
                _Database.Open("select count(*) as Count from admx_hrisp.tbl_empmasterfile E inner join admx_hrisp.pp_tempattendances T " +
                                "on E.fld_idnumber = T.EmployeeNO where T.Employeeno = '" + _EMP + "' and PayrollID = '" + clsPPeriod._SelID + "'");

                while (_Database.Reader.Read())
                {
                    if (_Database.Reader["Count"].ToString() == "0")
                    {
                        _Database.Execute("INSERT INTO admx_hrisp.pp_tempattendances (PayrollId, EmployeeNo) VALUES ('" + clsPPeriod._SelID + "', '" + _EMP + "')");
                        await this.ShowMessageAsync("ADD", "Successfully Added");
                        cmbSite_DropDownClosed(null, null);
                        return;
                    }
                    await this.ShowMessageAsync("Employee ID", _EMP +" Not Found");
                }
            }
            
            
        }

       

    }
}
