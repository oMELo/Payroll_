﻿
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
using System.IO;
using MahApps;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using AdmereX;
namespace Payroll_
{
    /// <summary>
    /// Interaction logic for EmpSchedule.xaml
    /// </summary>
    /// 
    public partial class EmpSchedule 
    {

        string Site="";
     
        public EmpSchedule()
        {
            InitializeComponent();
            dtEffectivity.SelectedDate = DateTime.Now;
            fillOtherCMB _CMBData = new fillOtherCMB();
            _CMBData.getCMB(cmbSchedType, "SELECT * FROM admx_hrisp.pp_scheduletype");
        }
        public void getEMP(string _Condition)
        {

            SEARCHLIST("select fld_IDNumber as _EmpID , concat(ifnull(fld_FirstName,'') ,' ', ifnull(fld_MiddleName,'') , ' ' ,ifnull(fld_LastName,'')) as _Name, " +
                            "(select Count( SchedID) from admx_hrisp.pp_empschedules where empNO = fld_idnumber ) as Count from  admx_hrisp.tbl_empmasterfile " + _Condition + " order by fld_FirstName");
            dt.ItemsSource = "";
            dt.ItemsSource = clsSchedule._EmpSchedList;
           
           
        }

       
        public void getCurSched(string _Condition)
        { 
        dtCurSched.ItemsSource="";
        dtCurSched.ItemsSource = _clsCurSchedule("select M.fld_IDNumber as EmpNO, concat(ifnull(fld_FirstName,'') ,' ', ifnull(fld_MiddleName,'') , ' ' ,ifnull(fld_LastName,'')) as FullName,ES.Effectivity,S.Name as SchedName,T.Description from admx_hrisp.tbl_empmasterfile M " +
                           "inner join admx_hrisp.pp_empschedules ES on M.fld_idnumber = ES.EmpNo inner join admx_hrisp.pp_schedules S on ES.SchedID = S.SchedID " +
                           "inner join admx_hrisp.pp_scheduletype T on S.TypeID = T.TypeID where ES.EmpNO= " + _Condition + " order by ES.Effectivity desc");
        
        }
        public List<clsSchedule> SEARCHLIST(String _Query)
        {
            using (Database _Database = new Database())
            {
                clsSchedule._EmpSchedList.Clear();
                _Database.Open(_Query);
                while (_Database.Reader.Read())
                {
                    {

                        clsSchedule._EmpSchedList.Add(new clsSchedule()
                        {
                            _empID = Convert.ToInt32(_Database.Reader["_EmpID"].ToString()),
                            _Name = Convert.ToString(_Database.Reader["_Name"].ToString()),
                            _SchedCount = Convert.ToInt32(_Database.Reader["Count"].ToString())
                        });

                    } 
                }
            
                return clsSchedule._EmpSchedList;
            }
        }
    
        public List<ScheduleList> _SchedList(string Query)
        {
            using (Database _Database = new Database())
            {
                ScheduleList._SchedList.Clear();
                _Database.Open(Query);
                while (_Database.Reader.Read())
                {
                    ScheduleList._SchedList.Add(new ScheduleList() 
                    {
                        _SchedID=Convert.ToInt32(_Database.Reader["SchedID"].ToString()),
                        _SchedName =_Database.Reader["Name"].ToString(),
                        _SchedType=_Database.Reader["Description"].ToString(),
                        _SunIN = TimeSpan.Parse(_Database.Reader["SunIN"].ToString()),
                        _SunOUT = TimeSpan.Parse(_Database.Reader["SunOUT"].ToString()),
                        _MonIN = TimeSpan.Parse(_Database.Reader["MonIN"].ToString()),
                        _MonOUT = TimeSpan.Parse(_Database.Reader["MonOUT"].ToString()),
                        _TueIN = TimeSpan.Parse(_Database.Reader["TueIN"].ToString()),
                        _TueOUT = TimeSpan.Parse(_Database.Reader["TueOUT"].ToString()),
                        _WedIN = TimeSpan.Parse(_Database.Reader["TueIN"].ToString()),
                        _WedOUT = TimeSpan.Parse(_Database.Reader["TueOUT"].ToString()),
                        _ThuIN = TimeSpan.Parse(_Database.Reader["ThuIN"].ToString()),
                        _ThuOUT = TimeSpan.Parse(_Database.Reader["ThuOUT"].ToString()),
                        _FriIN = TimeSpan.Parse(_Database.Reader["FriIN"].ToString()),
                        _FriOUT = TimeSpan.Parse(_Database.Reader["FriOUT"].ToString()),
                        _SatIN = TimeSpan.Parse(_Database.Reader["SatIN"].ToString()),
                        _SatOUT = TimeSpan.Parse(_Database.Reader["SatOUT"].ToString())
                    });
                }
            
            }

            return ScheduleList._SchedList;
        }

        public List<clsCurSchedule> _clsCurSchedule(string Query)
        {
            clsCurSchedule._CurSchedule.Clear();
            using (Database _Database = new Database())
            {

                _Database.Open(Query);
                while (_Database.Reader.Read())
                { 
                    clsCurSchedule._CurSchedule.Add (new clsCurSchedule() 
                    {
                        _empID=Convert.ToInt32(_Database.Reader["EmpNO"].ToString()),
                        _Name=_Database.Reader["FullName"].ToString(),
                        _Effectivity=_Database.Reader["Effectivity"].ToString(),
                        _SchedName = _Database.Reader["SchedName"].ToString(),
                        _SchedType = Convert.ToString(_Database.Reader["Description"].ToString())
                    });
                }
            }

            return clsCurSchedule._CurSchedule;
        }

        private Boolean isNum(string _Input)
        {
            int test;
            return int.TryParse(_Input, out test);

        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string temp = "";
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

        private void cmbSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbSite_DropDownClosed(object sender, EventArgs e)
        {


            if (cmbSite.SelectedIndex >= 0)
            {
                txtSearch.Text = "";
                Site = " where fld_site=" + Convert.ToInt32(cmbSite.SelectedValue);
                getEMP(Site);
                dt.Columns[0].Visibility = Visibility.Visible;
                dt.Columns[3].Visibility = Visibility.Visible;

            }

        }
        private void cmbSite_DropDownOpened(object sender, EventArgs e)
        {
            fillCMB _CMBData = new fillCMB();
            _CMBData.getCMB(cmbSite, 9);
        }

       

        private void btAddSchedule_Click(object sender, RoutedEventArgs e)
        {

                dtSched.ItemsSource = _SchedList("SELECT * FROM admx_hrisp.pp_schedules S inner join admx_hrisp.pp_scheduletype T on S.TypeID = T.TypeID");
                FlyOut.IsOpen = true;
   
        }

        private void dtSched_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtSched.SelectedIndex >= 0)
            {
                btEffectivity.IsEnabled = true;
            }
           
           
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            dt.ItemsSource = "";
            dtSched.ItemsSource = "";
            cmbSite.Text = "";
            TName.Content = "";

            clsSchedule._SelempID = 0;
        

            dt.Visibility = Visibility.Visible;
            dtCurSched.Visibility = Visibility.Hidden;
            btViewSched.IsEnabled = true;
            //btAddNew.IsEnabled = false;
            FlyOut.IsOpen = false;
            btAddSchedule.IsEnabled = false;
        }

        private void FlyOut_ClosingFinished(object sender, RoutedEventArgs e)
        {
            getCurSched(Convert.ToString(clsSchedule._SelempID));
            dtCurSched.Visibility = Visibility.Visible;
            //btCancel_Click(null, null);
        }

        private void btAddNew_Click(object sender, RoutedEventArgs e)
        {
            ScheduleMaintenance _SchedMaintenance = new ScheduleMaintenance();
            _SchedMaintenance.ShowDialog();
        }

        private void dt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dt.SelectedIndex >= 0)
            {
                TName.Content = clsSchedule._EmpSchedList[dt.SelectedIndex]._Name;
                clsSchedule._SelempID = clsSchedule._EmpSchedList[dt.SelectedIndex]._empID;
            }
          
        }

        private void btViewSched_Click(object sender, RoutedEventArgs e)
        {
            getCurSched(Convert.ToString(clsSchedule._SelempID));
            dtCurSched.Visibility = Visibility.Visible;
            btAddSchedule.IsEnabled = true;
            btViewSched.IsEnabled = false;
        }

        private void btEffectivity_Click(object sender, RoutedEventArgs e)
        {
         
            MessageBoxResult ans = MessageBox.Show("Are you sure you want to save this Schedule?", "Effectivity Schedule", MessageBoxButton.YesNo);
            if (ans.ToString() == "Yes" && dtSched.SelectedIndex >= 0)
            {
                using (Database _Database = new Database())
                {
                    _Database.Execute("insert into admx_hrisp.pp_empschedules (EmpNo, SchedID, Effectivity) Values (" + clsSchedule._SelempID + ",'" + ScheduleList._SchedList[dtSched.SelectedIndex]._SchedID + "','" + String.Format("{0:yyyy-MM-dd '00:00:00.000000'}", dtEffectivity.SelectedDate.Value) + "')");
                    getCurSched(Convert.ToString(clsSchedule._SelempID));
            
                }

            }
            else if (ans.ToString() == "Yes" && dtSched.SelectedIndex < 0) MessageBox.Show("No selected schedule");

        }

        private void txtSearchSched_TextChanged(object sender, TextChangedEventArgs e)
        {
            dtSched.ItemsSource = "";
            dtSched.ItemsSource = _SchedList("SELECT * FROM admx_hrisp.pp_schedules S inner join admx_hrisp.pp_scheduletype T on S.TypeID = T.TypeID where T.TypeID=" + Convert.ToInt32(cmbSchedType.SelectedValue) + " and Name like '%"+txtSearchSched.Text+"%'");

        }

        private void cmbSchedType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbSchedType_DropDownClosed(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cmbSchedType.SelectedValue) >= 0)
            {
                dtSched.ItemsSource = "";
                txtSearchSched.IsEnabled = true;
                dtSched.ItemsSource = _SchedList("SELECT * FROM admx_hrisp.pp_schedules S inner join admx_hrisp.pp_scheduletype T on S.TypeID = T.TypeID where T.TypeID=" + Convert.ToInt32(cmbSchedType.SelectedValue));

            }
        }

        private void cmbSchedType_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
        


    }
}
