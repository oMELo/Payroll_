

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

namespace Payroll_
{
    /// <summary>
    /// Interaction logic for PPeriod.xaml
    /// </summary>
    public partial class PPeriod 
    {
        
        MySqlDataReader MyReader;
        Database _Database = new Database();
        string condition = "";
        Double Count;
        Double totEmpCount;
        //private BackgroundWorker worker = null;
        public PPeriod()
        {
         
           

            InitializeComponent();
            if (ckTOPFive.IsChecked == true) condition = "Limit 5 ";
            
        }

      

        public List<clsPPeriod> pPeriod(String _Query)
        {
           

            clsPPeriod._pPeriodList.Clear();
            MySqlCommand command = new MySqlCommand(_Query, _Database.Connection);
            MyReader = command.ExecuteReader();
            {
               
                while (MyReader.Read())
              
                {
                    clsPPeriod._pPeriodList.Add(new clsPPeriod()
                    {
                        _ID = Convert.ToInt32(MyReader["ID"].ToString()),
                        _pPeriod = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", MyReader["PayrollDate"].ToString())),
                        _pStartDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}",MyReader["StartDate"].ToString())),
                        _pEndDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}",MyReader["EndDate"].ToString())),
                        _mStartDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}",MyReader["MataStart"].ToString())),
                        _mEndDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}",MyReader["MataEnd"].ToString())),
                        Image = "Images/Payment.ico"
                    });
                 
                } MyReader.Close();
            }

            return clsPPeriod._pPeriodList;
        }

        private async void btProceed_Click(object sender, RoutedEventArgs e)
        {
         
            int i = 0;
            object item = dtPperiod.SelectedItems[i];
            using (Database _Database = new Database())
            {

                _Database.Open("Select count(*) as 'Count' from pp_tempattendances where PayrollId = " + Convert.ToInt32((dtPperiod.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text));
                while (_Database.Reader.Read())
                {
                    Count=Convert.ToInt32(_Database.Reader["Count"].ToString());
                }
            }

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Create New",
                NegativeButtonText = "No",
                FirstAuxiliaryButtonText = "Cancel",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            MessageDialogResult result = await this.ShowMessageAsync("Total Record Found : " + Count, "If you want New Transaction click 'Create New' " +
                                            Environment.NewLine + "NO if transaction is already existing",
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {

                using (Database _CountDB = new Database())
                {
                    MySqlCommand _Mysql = new MySqlCommand("select Count(*) from admx_hrisp.tbl_empmasterfile where fld_IsActive = true " + //" and fld_IDNumber in ('20110020') " + // " and fld_IDNumber in ('20160054','20140101','20130042','20140108','20130033','20140126','20160179','20150233','20140171','20130045') " +
                                                        "order by fld_FirstName", _CountDB.Connection);
                   totEmpCount=Convert.ToDouble( _Mysql.ExecuteScalar());
              
                }
                clsPPeriod._SelID = Convert.ToInt32((dtPperiod.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text);
                _Database.Execute("Delete From admx_hrisp.pp_tempattendances where PAYROLLID = " + clsPPeriod._SelID);
                clsChecking._StartPayroll = Convert.ToDateTime((dtPperiod.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text);
                clsChecking._EndPayroll = Convert.ToDateTime((dtPperiod.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text);
                clsChecking._StartMata = Convert.ToDateTime((dtPperiod.SelectedCells[6].Column.GetCellContent(item) as TextBlock).Text);
                clsChecking._EndMata = Convert.ToDateTime((dtPperiod.SelectedCells[7].Column.GetCellContent(item) as TextBlock).Text);

                empLISt(Convert.ToInt32((dtPperiod.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text),
                                        Convert.ToDateTime((dtPperiod.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text),
                                        Convert.ToDateTime((dtPperiod.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text),
                                        Convert.ToDateTime((dtPperiod.SelectedCells[6].Column.GetCellContent(item) as TextBlock).Text),
                                        Convert.ToDateTime((dtPperiod.SelectedCells[7].Column.GetCellContent(item) as TextBlock).Text)
                                        );

            }
            else if (result==MessageDialogResult.Negative)
            {
                clsPPeriod._SelID = Convert.ToInt32((dtPperiod.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text);
                clsChecking._StartPayroll = Convert.ToDateTime((dtPperiod.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text);
                clsChecking._EndPayroll = Convert.ToDateTime((dtPperiod.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text);
                clsChecking._StartMata = Convert.ToDateTime((dtPperiod.SelectedCells[6].Column.GetCellContent(item) as TextBlock).Text);
                clsChecking._EndMata = Convert.ToDateTime((dtPperiod.SelectedCells[7].Column.GetCellContent(item) as TextBlock).Text);


                CalAttendance _CalAtt = new CalAttendance();
                _CalAtt.ShowDialog();
            }


        }
        
        public  async void empLISt(int _PayrollID, DateTime PayrollStart, DateTime PayrollEnd, DateTime MataStart, DateTime MataEnd)
        {
            Double CTR=0;
            var controller = await this.ShowProgressAsync("Please wait...", "LOADING!");
            controller.SetIndeterminate();
            await TaskEx.Delay(1500);
            controller.SetCancelable(true);    
            Database _empCount = new Database();
            clsTotalEmpCalc _clsTotalEmpCalc = new clsTotalEmpCalc();
            
            string _Ins="";
          
            int _HolidayCount = 0;
            _HolidayCount = _clsTotalEmpCalc.getHoliday(MataStart, MataEnd);


            _Database.Open("select concat(ifnull(fld_FirstName,'') ,' ', ifnull(fld_MiddleName,'') , ' ' ,ifnull(fld_LastName,'')) as 'FullName' ,fld_IDNumber from admx_hrisp.tbl_empmasterfile where fld_IsActive = true " + //" and fld_IDNumber in ('20110020') " + // " and fld_IDNumber in ('20160054','20140101','20130042','20140108','20130033','20140126','20160179','20150233','20140171','20130045') " +
                "order by fld_FirstName");
            {
                while (_Database.Reader.Read())
                {
                    var _Val = await Task.Run(() => _clsTotalEmpCalc.get_Regular_Absences_Late(_Database.Reader["fld_IDNumber"].ToString(), MataStart, MataEnd));
                    var _LeaveVal = await Task.Run(() =>  _clsTotalEmpCalc.getLeaves(_Database.Reader["fld_IDNumber"].ToString()));

                    _Ins = await Task.Run(() => _Ins + "(" +
                    _PayrollID + "," +
                    _Database.Reader["fld_IDNumber"].ToString() + "," +
                    _clsTotalEmpCalc.getMATA(_Database.Reader["fld_IDNumber"].ToString(), MataStart, MataEnd) + "," +
                    _Val.Item1 + "," +
                    _HolidayCount + "," +
                    _clsTotalEmpCalc.getRegularOT(_Database.Reader["fld_IDNumber"].ToString(), _PayrollID) + "," +
                    _clsTotalEmpCalc.getRestDayOT(_Database.Reader["fld_IDNumber"].ToString(), _PayrollID) + "," +
                    _clsTotalEmpCalc.getHolidayOT(_Database.Reader["fld_IDNumber"].ToString(), _PayrollID) + "," +
                    _clsTotalEmpCalc.getSpecialHolidayOT(_Database.Reader["fld_IDNumber"].ToString(), _PayrollID) + "," +
                    _Val.Item2 + "," +
                    _Val.Item3 + "," +
                    _LeaveVal.Item1 + "," +
                    _LeaveVal.Item2 + "," +
                    _Val.Item4 +"),");


                    CTR++;
                    await Task.Run(() => controller.SetMessage("(" + String.Format("{0:0.##}", (CTR / totEmpCount) * 100) + "%) Employee : " + _Database.Reader["FullName"].ToString() + "..."));

                    if (controller.IsCanceled) break; 
                    await TaskEx.Delay(50);
                                      
                } _Database.Reader.Close();
                if (_Ins.Length > 0) using (Database _DBins = new Database()) _DBins.Execute("INSERT INTO admx_hrisp.pp_tempattendances ( PayrollId,EmployeeNo,Total,Regular,LegalHoliday,OTRegular,OTRestday,OTLegalHoliday,OTSpecialHoliday,Absences,Tardiness,VL,SL,LWOP) VALUES " +
                                     _Ins.PadRight(_Ins.Length - 1).Substring(0, _Ins.Length - 1).Trim());
                
                }
            await controller.CloseAsync();
            CalAttendance _CalAtt = new CalAttendance();
            _CalAtt.ShowDialog();
        }
       
        private void dtPperiod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
     
        private void ckTOPFive_Unchecked(object sender, RoutedEventArgs e)
        {
            dtPperiod.ItemsSource = null;
            dtPperiod.ItemsSource = pPeriod("SELECT ID, date_format(PayrollDate,'%m/%d/%Y') as PayrollDate,date_format(StartDate,'%m/%d/%Y') as StartDate,date_format(EndDate,'%m/%d/%Y') as EndDate,date_format(MataStart,'%m/%d/%Y') as MataStart,date_format(MataEnd,'%m/%d/%Y') as MataEnd FROM admx_hrisp.pp_payrolls where status = false order by ID ");
        }

        private void ckTOPFive_Checked(object sender, RoutedEventArgs e)
        {

            dtPperiod.ItemsSource = null;
            dtPperiod.ItemsSource = pPeriod("SELECT ID, date_format(PayrollDate,'%m/%d/%Y') as PayrollDate,date_format(StartDate,'%m/%d/%Y') as StartDate,date_format(EndDate,'%m/%d/%Y') as EndDate,date_format(MataStart,'%m/%d/%Y') as MataStart,date_format(MataEnd,'%m/%d/%Y') as MataEnd FROM admx_hrisp.pp_payrolls where status = false order by ID limit 5");

        }

     
      
    }
}
