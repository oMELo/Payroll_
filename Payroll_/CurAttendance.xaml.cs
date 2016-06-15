
using System;
using System.IO;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Data.SqlClient;
using System.Reflection;
namespace Payroll_
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 
    {
      
        string  SiteStat="";
        string tmpSql;
        string eID = "";
        string Site = "";
        Int32 AttID;
        clsEMPEARNING _getInOut = new clsEMPEARNING();
        Database _DB = new Database();
       
        private static List<string> empName = new List<string>();
        public UserControl1()
        {
            InitializeComponent();

        
        }
        public void getEMP(string _Condition)
        {

            SEARCHLIST("select fld_IDNumber as _EmpID , concat(ifnull(fld_FirstName,'') ,' ', ifnull(fld_MiddleName,'') , ' ' ,ifnull(fld_LastName,'')) as _Name, " +
                            "fld_FirstName,fld_MiddleName,fld_LastName from  admx_hrisp.tbl_empmasterfile " + _Condition + " order by fld_FirstName");
            dt.ItemsSource = "";
            dt.ItemsSource = clsAttendance._SEARCHLIST;
            dt.Columns[0].Visibility = Visibility.Visible;
            dt.Columns[1].Visibility = Visibility.Hidden;
            dt.Columns[4].Visibility = Visibility.Hidden;
            dt.Columns[5].Visibility = Visibility.Hidden;
        }
        public void getData(string _Condition)
        {

            tmpSql = "select _ID,_EmpID, concat(ifnull(fld_FirstName,'') ,' ', ifnull(fld_MiddleName,'') , ' ' ,ifnull(fld_LastName,'')) as _Name,_DateIn,_DateOut " +
           "from admx_hrisp.tbl_EmpMasterFile M " +
           "inner join admx_hrisp.pp_EmpClocks E  on fld_IDNumber = _EmpID  " +
           "where " + _Condition + "  DATE_FORMAT(_DateIn,'%m/%d/%Y')>= '" + String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(clsStatic._GeneratedFromTime)) + "' and DATE_FORMAT(_DateOut,'%m/%d/%Y')   <= '" + String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(clsStatic._GeneratedToTime)) + "' " +
           "or (" + _Condition + " _DateOut IS NULL and  DATE_FORMAT(_DateIn,'%m/%d/%Y') >= '" + String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(clsStatic._GeneratedFromTime)) + "' and  DATE_FORMAT(_DateIn,'%m/%d/%Y') <= '" + String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(clsStatic._GeneratedToTime)) + "' )  " +
           "or (" + _Condition + " _DateIn IS NULL and DATE_FORMAT(_DateOut,'%m/%d/%Y')  >= '" + String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(clsStatic._GeneratedFromTime)) + "' and DATE_FORMAT(_DateOut,'%m/%d/%Y')  <= '" + String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(clsStatic._GeneratedToTime)) + "' ) " +
           "order by  CONCAT(fld_FirstName ,' ' ,fld_MiddleName,' ',  fld_LastName ),_DateIn ";
            Attendance(tmpSql);
            dt.ItemsSource = "";
            dt.ItemsSource = clsAttendance._Attendance;
            dt.Columns[0].Visibility = Visibility.Hidden;
            dt.Columns[1].Visibility = Visibility.Visible;
            dt.Columns[4].Visibility = Visibility.Visible;
            dt.Columns[5].Visibility = Visibility.Visible;

        }
        public List<clsAttendance> Attendance(string SQL)
        {
            Database _Database = new Database();
            MySqlDataReader _Reader;
            clsAttendance._Attendance.Clear();

            MySqlCommand command = new MySqlCommand(SQL, _Database.Connection);
            _Reader = command.ExecuteReader();
            {
                while (_Reader.Read())
                {

                    clsAttendance._Attendance.Add(new clsAttendance()
                    {
                        _ID = Convert.ToInt32(_Reader["_ID"].ToString()),
                        _empID = Convert.ToInt32(_Reader["_empID"].ToString()),
                        _Name = Convert.ToString(_Reader["_Name"].ToString()),
                        _DateiN = Convert.ToString(_Reader["_DateIn"].ToString()),
                        _DateOut = Convert.ToString(_Reader["_DateOut"].ToString())

                    });

                } _Reader.Close();
            }
            return clsAttendance._Attendance;
        }
        public List<clsAttendance> SEARCHLIST(String _Query)
        {

            MySqlDataReader MyReader;
            Database _Database = new Database();
            clsAttendance._SEARCHLIST.Clear();
            MySqlCommand command = new MySqlCommand(_Query, _Database.Connection);
            MyReader = command.ExecuteReader();
            {
                while (MyReader.Read())
                {
                    clsAttendance._SEARCHLIST.Add(new clsAttendance()
                    {
                        _empID = Convert.ToInt32(MyReader["_EmpID"].ToString()),
                        _Name = Convert.ToString(MyReader["_Name"].ToString())
                    });

                } MyReader.Close();
            }
            return clsAttendance._SEARCHLIST;
        }
        private void btGenerate_Click(object sender, RoutedEventArgs e)
        {
            clsStatic._Status = "GENERATE";
            frmDateTime _frmDateTime = new frmDateTime();
            _frmDateTime.ShowDialog();

            if (clsStatic._Status.ToString() == "CANCEL") { btCANCEL_Click(null, null); return; }
            _Sel._SelUnit.Clear();
            _Sel._SelUnit.Add("FullRow");
            _Sel._SelUnit.Add("Cell");
            cmbSelection.ItemsSource = _Sel._SelUnit;
            cmbSelection.SelectedIndex = 0;
            cmbSelection.IsEnabled = true;
            getData("_EmpID in (" + toARR(eID) + ") and ");
            btGenerate.IsEnabled = false;

        }
        private string toARR(string tmpSTR)
        {

            return tmpSTR.Substring(0, (tmpSTR.Length - 1));
        }
        private void dt_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton.ToString() == "Right")
            {



            }
        }
        private void dt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                empName.Clear();
                eID = "";
                if (ckSelectALL.IsChecked == false)
                {

                    //for (int i = 0; i < dt.SelectedItems.Count; i++)

                    for (int i = 0; i < dt.SelectedItems.Count; i++)
                    {

                        object item = dt.SelectedItems[i];
                        eID = eID + (dt.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text + ",";
                        empName.Add((dt.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text);
                        btGenerate.IsEnabled = true;

                    }
                }

            }
            catch (Exception _eX)
            {
                MessageBox.Show(_eX.Message);
            }
        }
        private async  void btAdd_Click(object sender, RoutedEventArgs e)
        {

            object item = dt.SelectedItem;
            AttID = Convert.ToInt32((dt.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text);
            if (btAdd.Content.ToString() == "ADD NEW")
            {
                clsStatic._Status = "ADD";
                btAdd.Content = "SAVE";
                frmDateTime _frmDateTime = new frmDateTime();
                _frmDateTime.ShowDialog();

                btGenerate.IsEnabled = false;
                btEdit.IsEnabled = false;
                btDelete.IsEnabled = false;
            }
            else 
            {
                btAdd.Content = "ADD NEW";
                _DB.Execute("INSERT INTO admx_hrisp.pp_EmpClocks (_EmpID ,_DateIn,_DateOUT) values ('" + AttID + "' ,'" + String.Format("{0:yyyy-MM-dd hh:mm:ss}", Convert.ToDateTime(clsStatic._dtFromTime)) + "','" + String.Format("{0:yyyy-MM-dd hh:mm:ss}", Convert.ToDateTime(clsStatic._dtToTime)) + "')");
                enable(false);
                getData("_EmpID in (" + toARR(eID) + ") and ");
                await (Window1.GetWindow(this) as Window1).ShowMessageAsync("Successfully", "Added");
              
            }
        }
        private async void btEdit_Click(object sender, RoutedEventArgs e)
        {

            if (btEdit.Content.ToString() == "EDIT" && dt.SelectedItem != null)
            {

                object item = dt.SelectedItem;
                AttID = Convert.ToInt32((dt.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text);
                string ID = (dt.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;

                clsStatic._Status = "EDIT";
                
                btGenerate.IsEnabled = false;
                btDelete.IsEnabled = false;
                btAdd.IsEnabled = false;

                clsStatic._dtFromTime = (dt.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text;
                clsStatic._dtToTime = (dt.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text;
                frmDateTime _frmDateTime = new frmDateTime();
                _frmDateTime.ShowDialog();
                if (clsStatic._Status == "CANCEL") { btCANCEL_Click(null, null); return; }
                
                btEdit.Content = "UPDATE";
                mnuEdit.Header = btEdit.Content;
                mnuDelete.IsEnabled = false;
                (dt.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text = clsStatic._dtFromTime;
                (dt.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text = clsStatic._dtToTime;
            }
            else
            {
                btEdit.Content = "EDIT";
                _DB.Execute("UPDATE admx_hrisp.pp_EmpClocks set _DateIn ='" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(clsStatic._dtFromTime)) + "', _DateOut='" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(clsStatic._dtToTime)) + "' where _ID=" + AttID);
                enable(false);
                getData("_EmpID in (" + toARR(eID) + ") and ");
                await (Window1.GetWindow(this) as Window1).ShowMessageAsync("Successfully", "Edited");

            }

        }
        public void enable(Boolean _Ans)
        {
            btAdd.IsEnabled = _Ans;
            btEdit.IsEnabled = _Ans;
            btDelete.IsEnabled = _Ans;
            cmbSelection.IsEnabled = _Ans;

        }
        private void dt_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //string selectedData = "";
            //if (cmbSelection.Text =="Cell")
            //{
            //    foreach (var dataGridCellInfo in dt.SelectedCells)
            //    {

            //        object x = dataGridCellInfo;
            //        PropertyInfo pi = dataGridCellInfo.Item.GetType().GetProperty(dataGridCellInfo.Column.Header.ToString());
            //        var value = pi.GetValue(dataGridCellInfo.Item, null);
            //        selectedData += dataGridCellInfo.Column.Header + ": " + value.ToString() + "\n";
            //    }
            //    MessageBox.Show(selectedData);
            //}

            if (clsAttendance._Attendance.Count != 0)
            {
                btGenerate.IsEnabled = false;
                btAdd.IsEnabled = true;
                btEdit.IsEnabled = true;
                btDelete.IsEnabled = true;
                btViewLogs.IsEnabled = true;
                mnuAddNew.IsEnabled = true;
                mnuEdit.IsEnabled = true;
                AttID = Payroll_.clsAttendance._Attendance[dt.SelectedIndex]._ID;
                lblName.Content = Payroll_.clsAttendance._Attendance[dt.SelectedIndex]._Name;               
                clsStatic._dtFromTime = Payroll_.clsAttendance._Attendance[dt.SelectedIndex]._DateiN;
                clsStatic._dtToTime = Payroll_.clsAttendance._Attendance[dt.SelectedIndex]._DateOut;
             
            }
           
        }
        private void btCANCEL_Click(object sender, RoutedEventArgs e)
        {
            btGenerate.IsEnabled = false;
            btAdd.IsEnabled = false;
            btEdit.IsEnabled = false;
            btDelete.IsEnabled = false;
            btAdd.Content = "ADD NEW";
            btEdit.Content = "EDIT";
            btViewLogs.IsEnabled = false;

            mnuAddNew.IsEnabled = false;
            mnuEdit.IsEnabled = false;
            mnuDelete.IsEnabled = false;
            mnuAddNew.Header = "ADD NEW";
            mnuEdit.Header = "EDIT";
            eID = "";
            getEMP("");
            txtSearch.Text = "";

        }
        private async void btDelete_Click(object sender, RoutedEventArgs e)
        {
            object item = dt.SelectedItem;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
             };
            MessageDialogResult result = await(Application.Current.MainWindow as Window1).ShowMessageAsync("Delete" , 
                                        Environment.NewLine + "Are you sure?",
            MessageDialogStyle.AffirmativeAndNegative, mySettings);
            if (result == MessageDialogResult.Affirmative)
            {

                _DB.Execute("delete from admx_hrisp.pp_EmpClocks where _ID=" + Convert.ToInt32((dt.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text));
                await (Window1.GetWindow(this) as Window1).ShowMessageAsync("Successfully", "Deleted");

                getData("_EmpID in (" + toARR(eID) + ") and ");
                enable(false);
               
            }
        }
        private void btATTUPDATE_Click(object sender, RoutedEventArgs e)
        {
            clsStatic._Status = "ATTENDANCE UPDATE";
            frmDateTime _frmDateTime = new frmDateTime();
            _frmDateTime.ShowDialog();
        }
        private Boolean isNum(string _Input)
        {
            int test;
            return int.TryParse(_Input, out test);

        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FlyOut.IsOpen = false;
            string temp="";
            if (cmbSite.Text == "") Site = " where ";
            else temp = " and ";
            if ((isNum(txtSearch.Text)) == true)
            {
                getEMP(Site + temp + "  fld_IDNumber like '%" + txtSearch.Text + "%' ");

            }
            else
            {
                getEMP(Site + temp + "  concat(ifnull(fld_FirstName,'') ,' ', ifnull(fld_MiddleName,'') , ' ' ,ifnull(fld_LastName,'')) like '%" + txtSearch.Text + "%' ");
            }


        }
        private async void btExport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".xls";
            dlg.Filter = "Excel Files (*.xls)|*.xls";

            Nullable<bool> result = dlg.ShowDialog();


            if (result == true)
            {
              
                string filename = dlg.FileName;
             
                Microsoft.Office.Interop.Excel.Application oXL;
                Microsoft.Office.Interop.Excel._Workbook oWB;
                Microsoft.Office.Interop.Excel._Worksheet oSheet;
                Microsoft.Office.Interop.Excel.Range oRng;
                object misvalue = System.Reflection.Missing.Value;

       
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = true;

          
                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

      
           
                oSheet.Cells[1, 1] = "EmployeeNo";
                oSheet.Cells[1, 2] = "Name";
                oSheet.Cells[1, 3] = "Date IN";
                oSheet.Cells[1, 4] = "Date OUT";

       
                oSheet.get_Range("A1", "D1").Font.Bold = true;
                oSheet.get_Range("A1", "D1").VerticalAlignment =
                Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                string[,] EmployeeAttendance = new string[clsAttendance._Attendance.Count, 4];
                for (int i = 0; i < clsAttendance._Attendance.Count; i++)
                {

                    EmployeeAttendance[i, 0] = clsAttendance._Attendance[i]._empID.ToString();
                    EmployeeAttendance[i, 1] = clsAttendance._Attendance[i]._Name.ToString();
                    EmployeeAttendance[i, 2] = clsAttendance._Attendance[i]._DateiN.ToString();
                    EmployeeAttendance[i, 3] = clsAttendance._Attendance[i]._DateOut.ToString();

                }

                oSheet.get_Range("A2",Convert.ToString(String.Format("{0}", "D" + (clsAttendance._Attendance.Count+1)))).Value2 = EmployeeAttendance;

   
                oRng = oSheet.get_Range("A1", "D1");
                oRng.EntireColumn.AutoFit();

                oXL.Visible = false;
                oXL.UserControl = false;
                oWB.SaveAs(dlg.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                
                oWB.Close();
                await(Window1.GetWindow(this) as Window1).ShowMessageAsync("Successfully", "Exported");

            }
        }
        private void cmbSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void cmbSite_DropDownClosed(object sender, EventArgs e)
        {
         
           
            if (cmbSite.SelectedIndex >= 0)
            {
                FlyOut.IsOpen = false;
                txtSearch.Text = "";
                Site = " where fld_site=" + Convert.ToInt32(cmbSite.SelectedValue);
                getEMP(Site);
            }

        }
        private void cmbSite_DropDownOpened(object sender, EventArgs e)
        {
            fillCMB _CMBData = new fillCMB();
            _CMBData.getCMB(cmbSite, 9);
        }
        private void btViewLogs_Click(object sender, RoutedEventArgs e)
        {
            if (dt.SelectedIndex >= 0)
            {
          
                
                    FlyOut.IsOpen = true;
                
            }
        }

        
        private void dt_AMS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            object item = dt.SelectedItem;
            if (clsStatic._dtFromTime == "" && dt_AMS.SelectedIndex >= 0)
            {
                (dt.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text = SQLData._SQLDATA[dt_AMS.SelectedIndex]._CheckTime + " AM";
              
               
            }
            else if (clsStatic._dtToTime == "" && dt_AMS.SelectedIndex >= 0)
            {
                (dt.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text = SQLData._SQLDATA[dt_AMS.SelectedIndex]._CheckTime + " PM";

            }
          
        }

        private void dt_MouseDown(object sender, MouseButtonEventArgs e)
        {
         
        }

        private async void FlyOut_ClosingFinished(object sender, RoutedEventArgs e)
        {
            object item = dt.SelectedItem;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",

            };
            MessageDialogResult result = await (Application.Current.MainWindow as Window1).ShowMessageAsync("SAVE",
                                        Environment.NewLine + "Do you want to Save this?",
            MessageDialogStyle.AffirmativeAndNegative, mySettings);

              if (result == MessageDialogResult.Affirmative)
              {
                  
                  clsStatic._dtFromTime = (dt.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text;
                  clsStatic._dtToTime = (dt.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text;
                  btEdit.Content = "UPDATE";
                  btEdit_Click(null, null);
              }
              else if (result == MessageDialogResult.Negative)
              {
                  (dt.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text = clsStatic._dtFromTime;
                  (dt.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text = clsStatic._dtToTime;
              }
        }

        private void dt_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void ckSelectALL_Checked(object sender, RoutedEventArgs e)
        {
            if (ckSelectALL.IsChecked == true)
            {
                dt.SelectAll();
                btGenerate.IsEnabled = true;
                for (int i = 0; i < clsAttendance._SEARCHLIST.Count; i++)
                {
                    eID = eID + clsAttendance._SEARCHLIST[i]._empID + ",";
                }
            }
   
        }

        private void ckSelectALL_Unchecked(object sender, RoutedEventArgs e)
        {
            dt.UnselectAll(); 
        }

        

        private void cmbAMSSite_DropDownOpened(object sender, EventArgs e)
        {
            fillCMB _CMBData = new fillCMB();
            _CMBData.getQRYCMB(cmbAMSSite, "select fld_StaticParamID as ID , fld_StaticParamDesc as Description from admx_hrisp.tbl_staticparam where fld_CategoryID = 9 and fld_StaticParamID <> 136");
        }

        private void cmbAMSSite_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbAMSSite.SelectedIndex >= 0)
            {
                switch (cmbAMSSite.Text)
                {
                    case "MAKATI SITE":
                        SiteStat = Properties.Settings.Default.Makati;
                        break;
                    case "MANDALUYONG SITE":
                        SiteStat = Properties.Settings.Default.Mandaluyong;
                        break;
                    case "CEBU SITE":
                        SiteStat = Properties.Settings.Default.Cebu;
                        break;

                }


                SqlDataReader sqlReader;
                SqlCommand _SqlCommand;
                SQLData._SQLDATA.Clear();
                dt_AMS.ItemsSource = "";
                using (SqlConnection _SqlConn = new SqlConnection("server=" + SiteStat + ";database=ATT_db;user id=sysdev;password=Admx5299"))
                {
                    _SqlConn.Open();

                    _SqlCommand = new SqlCommand("select  " +
                                                           "distinct " +
                                                           "convert(varchar(10),Checktime,101) as 'Date', " +
                                                           "(convert(varchar(10),Checktime,101)  + ' ' + convert(varchar(10),CHECKTIME,108))  as 'CheckDate', " +
                                                           "case " +
                                                           "When CHECKTYPE = Upper('I') COLLATE Latin1_General_CS_AI  then 'C / In' " +
                                                           "When CHECKTYPE = Upper('O') COLLATE Latin1_General_CS_AI then 'C / Out' " +
                                                           "When CHECKTYPE = '1' then 'Break In' " +
                                                           "When CHECKTYPE = '0' then 'Break Out' " +
                                                           "When CHECKTYPE = lower('i') COLLATE Latin1_General_CS_AI then 'O.T. / In' " +
                                                           "When CHECKTYPE = lower('o') COLLATE Latin1_General_CS_AI then 'O.T / Out' " +
                                                           "Else '	-	' " +
                                                           "END as 'State' " +
                                                           "from [ATT_db].[dbo].[USERINFO] U " +
                                                           "inner join [ATT_db].[dbo].[CHECKINOUT] C " +
                                                               "on U.[USERID] = C.[USERID]  " +
                                                           "where Checktime between '" + clsStatic._GeneratedFromTime + "' and '" + clsStatic._GeneratedToTime + "' " +
                                                           "and badgenumber = '" + Payroll_.clsAttendance._Attendance[dt.SelectedIndex]._empID + "' " +
                                                           "order by convert(varchar(10),Checktime,101)", _SqlConn);
                    sqlReader = _SqlCommand.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        SQLData._SQLDATA.Add(new SQLData()

                        {

                            _CheckDate = sqlReader["Date"].ToString(),
                            _CheckTime = String.Format("{0:MM/dd/yyyy hh:mm:ss}", Convert.ToDateTime(sqlReader["CheckDate"].ToString())),
                            _State = sqlReader["State"].ToString()

                        });
                    }
                    dt_AMS.ItemsSource = SQLData._SQLDATA;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dt.SelectionUnit = DataGridSelectionUnit.Cell;
            string selectedData = "";
            foreach (var dataGridCellInfo in dt.SelectedCells)
            {
                PropertyInfo pi = dataGridCellInfo.Item.GetType().GetProperty(dataGridCellInfo.Column.Header.ToString());
                var value = pi.GetValue(dataGridCellInfo.Item, null);
                selectedData += dataGridCellInfo.Column.Header + ": " + value.ToString() + "\n";
            }
            MessageBox.Show(selectedData);
        }

      

        private void cmbSelection_DropDownClosed(object sender, EventArgs e)
        {
        
            //switch (cmbSelection.Text)
            //{
            //    case "FullRow":
            //         dt.SelectionUnit = DataGridSelectionUnit.FullRow;
                      
            //        break;
            //    case "Cell":
            //         dt.SelectionUnit = DataGridSelectionUnit.Cell;
                     
            //        break;

            //}
        }

        private void cmbSelection_DropDownOpened(object sender, EventArgs e)
        {
            
            
        }
     
        public class  _Sel
        {
            public static List<String> _SelUnit = new List<String>();
           
            public  string _Type { get; set; }

          

          
        }

        private void cmbSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
