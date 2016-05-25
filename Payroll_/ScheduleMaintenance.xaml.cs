
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
    /// Interaction logic for ScheduleMaintenance.xaml
    /// </summary>
    public partial class ScheduleMaintenance 
    {
        
        public ScheduleMaintenance()
        {
            InitializeComponent();
            fillOtherCMB _CMBData = new fillOtherCMB();
            _CMBData.getCMB(cmbSchedType, "SELECT * FROM admx_hrisp.pp_scheduletype");// where isActive ="+true);

            dtSchedules.ItemsSource= SchedList("SELECT * FROM admx_hrisp.pp_schedules S inner join admx_hrisp.pp_scheduletype T on S.TypeID = T.TypeID");
        }

        public List<ScheduleList> SchedList(string SQL)
        {
            ScheduleList._SchedList.Clear();
            dtSchedules.ItemsSource = "";
            using (Database _Database = new Database())
            {
                _Database.Open(SQL);
                while (_Database.Reader.Read())
                {
                    ScheduleList._SchedList.Add(new ScheduleList()
                    {
                        _SchedID = Convert.ToInt32(_Database.Reader["SchedID"].ToString()),
                        _SchedName =_Database.Reader["Name"].ToString(),
                        _SchedType =_Database.Reader["Description"].ToString(),
                        _SunIN = TimeSpan.Parse(_Database.Reader["SunIN"].ToString()),
                        _SunOUT = TimeSpan.Parse(_Database.Reader["SunOUT"].ToString()),
                        _MonIN = TimeSpan.Parse(_Database.Reader["MonIN"].ToString()),
                        _MonOUT = TimeSpan.Parse(_Database.Reader["MonOUT"].ToString()),
                        _TueIN = TimeSpan.Parse(_Database.Reader["TueIN"].ToString()),
                        _TueOUT = TimeSpan.Parse(_Database.Reader["TueOUT"].ToString()),
                        _WedIN = TimeSpan.Parse(_Database.Reader["WedIN"].ToString()),
                        _WedOUT = TimeSpan.Parse(_Database.Reader["WedOUT"].ToString()),
                        _ThuIN = TimeSpan.Parse(_Database.Reader["ThuIN"].ToString()),
                        _ThuOUT = TimeSpan.Parse(_Database.Reader["ThuOUT"].ToString()),
                        _FriIN = TimeSpan.Parse(_Database.Reader["FriIN"].ToString()),
                        _FriOUT = TimeSpan.Parse(_Database.Reader["FriOUT"].ToString()),
                        _SatIN = TimeSpan.Parse(_Database.Reader["SatIN"].ToString()),
                        _SatOUT = TimeSpan.Parse(_Database.Reader["SatOUT"].ToString())

                    });
                }
                return ScheduleList._SchedList;
            }
        }

        private void txtSearchSchedName_TextChanged(object sender, TextChangedEventArgs e)
        {
            dtSchedules.ItemsSource = SchedList("SELECT * FROM admx_hrisp.pp_schedules  S inner join admx_hrisp.pp_scheduletype T on S.TypeID = T.TypeID where Name like '%" + txtSearchSchedName.Text + "%'");
        }

        private void dtSchedules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dtSchedules.SelectedIndex >= 0)
            {
                txtSchedName.Text = ScheduleList._SchedList[dtSchedules.SelectedIndex]._SchedName;
                cmbSchedType.Text = ScheduleList._SchedList[dtSchedules.SelectedIndex]._SchedType;
                txtSunIN.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._SunIN);
                txtSunOut.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._SunOUT);
                txtMonIN.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._MonIN);
                txtMonOUT.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._MonOUT);
                txtTueIN.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._TueIN);
                txtTueOUT.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._TueOUT);
                txtWedIN.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._WedIN);
                txtWedOUT.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._WedOUT);
                txtThuIN.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._ThuIN);
                txtThuOUT.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._ThuOUT);
                txtFriIN.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._FriIN);
                txtFriOUT.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._FriOUT);
                txtSatIN.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._SatIN);
                txtSatOUT.Text = Convert.ToString(ScheduleList._SchedList[dtSchedules.SelectedIndex]._SatOUT);
                _Type("");
            }
        }

        private void dtSchedules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbSchedType_DropDownClosed(object sender, EventArgs e)
        {
            _Type(" where T.TypeID =" + Convert.ToInt32(cmbSchedType.SelectedValue));
        }
        private void _Type(String Condition)
        { 
            Thickness margin;
            switch (Convert.ToInt32(cmbSchedType.SelectedValue))
            {
                
                
                case 2:

                    lbl7.Content = "Sunday Out";
                    lbl1.Content = "Monday Out";
                    lbl2.Content = "Tuesday Out";
                    lbl3.Content = "Wednesday Out";
                    lbl4.Content = "Thursday Out";
                    lbl5.Content = "Friday Out";
                    lbl6.Content = "Saturday Out";

                    margin = txtMonOUT.Margin;
                    margin.Top = 129 ;
                    margin.Right = 15;
                    txtMonOUT.Margin = margin;

                    margin = txtTueOUT.Margin;
                    margin.Top =163 ;
                    margin.Right = 15;
                    txtTueOUT.Margin = margin;

                    margin = txtWedOUT.Margin;
                    margin.Top =197 ;
                    margin.Right = 15;
                    txtWedOUT.Margin = margin;

                    margin = txtThuOUT.Margin;
                    margin.Top =231 ;
                    margin.Right = 15;
                    txtThuOUT.Margin = margin;

                    margin = txtFriOUT.Margin;
                    margin.Top =265 ;
                    margin.Right = 15;
                    txtFriOUT.Margin = margin;

                    margin = txtSatOUT.Margin;
                    margin.Top =299 ;
                    margin.Right = 15;
                    txtSatOUT.Margin = margin;

                    margin = txtSunOut.Margin;
                    margin.Top = 333;
                    margin.Right = 15;
                    txtSunOut.Margin = margin;
                    dtSchedules.ItemsSource = SchedList("SELECT * FROM admx_hrisp.pp_schedules  S inner join admx_hrisp.pp_scheduletype T on S.TypeID = T.TypeID " + Condition);

                    ICollectionView dataView = CollectionViewSource.GetDefaultView(dtSchedules.ItemsSource);


                    break;

                default:
                    lbl1.Content = "Sunday Out";
                    lbl2.Content = "Monday Out";
                    lbl3.Content = "Tuesday Out";
                    lbl4.Content = "Wednesday Out";
                    lbl5.Content = "Thursday Out";
                    lbl6.Content = "Friday Out";
                    lbl7.Content = "Saturday Out";

                    margin = txtSunOut.Margin;
                    margin.Top = 129;
                    margin.Right = 15;
                    txtSunOut.Margin = margin;

                    margin = txtMonOUT.Margin;
                    margin.Top = 163;
                    margin.Right = 15;
                    txtMonOUT.Margin = margin;

                    margin = txtTueOUT.Margin;
                    margin.Top = 197;
                    margin.Right = 15;
                    txtTueOUT.Margin = margin;

                    margin = txtWedOUT.Margin;
                    margin.Top = 231;
                    margin.Right = 15;
                    txtWedOUT.Margin = margin;

                    margin = txtThuOUT.Margin;
                    margin.Top = 265;
                    margin.Right = 15;
                    txtThuOUT.Margin = margin;

                    margin = txtFriOUT.Margin;
                    margin.Top = 299;
                    margin.Right = 15;
                    txtFriOUT.Margin = margin;

                    margin = txtSatOUT.Margin;
                    margin.Top = 333;
                    margin.Right = 15;
                    txtSatOUT.Margin = margin;
                    dtSchedules.ItemsSource = SchedList("SELECT * FROM admx_hrisp.pp_schedules  S inner join admx_hrisp.pp_scheduletype T on S.TypeID = T.TypeID "+Condition);
                    break;
            }
        }
        private void cmbSchedType_DropDownOpened(object sender, EventArgs e)
        {
                 }

        private void cmbSchedType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btADD_Click(object sender, RoutedEventArgs e)
        {
            if (btADD.Content.ToString() == "ADD NEW")
            {
                isEnable(true, "00:00:00");
                txtSchedName.Text = "";
                btADD.Content = "SAVE";
            }
            else
            {
               
                using (Database _Database = new Database())
                {
                    _Database.Execute("INSERT INTO admx_hrisp.pp_schedules (TypeID, Name,SunIN, SunOUT, MonIN, MonOUT, TueIN, TueOUT, WedIN, WedOUT, ThuIN, ThuOUT, FriIN, FriOUT, SatIN, SatOUT) VALUES " +
                        "('"+Convert.ToInt32(cmbSchedType.SelectedValue) +"', '"+txtSchedName.Text+"',"+
                        "'"+TimeSpan.Parse(txtSunIN.Text)+"', '"+TimeSpan.Parse(txtSunOut.Text)+"', "+
                        "'"+TimeSpan.Parse(txtMonIN.Text)+"', '"+TimeSpan.Parse(txtMonOUT.Text)+"', "+
                        "'"+TimeSpan.Parse(txtTueIN.Text)+"', '"+TimeSpan.Parse(txtTueOUT.Text)+"', "+
                        "'"+TimeSpan.Parse(txtWedIN.Text)+"', '"+TimeSpan.Parse(txtWedOUT.Text)+"', "+
                        "'"+TimeSpan.Parse(txtThuIN.Text)+"', '"+TimeSpan.Parse(txtThuOUT.Text)+"', "+
                        "'"+TimeSpan.Parse(txtFriIN.Text)+"', '"+TimeSpan.Parse(txtFriOUT.Text)+"', "+
                        "'"+TimeSpan.Parse(txtSatIN.Text)+"', '"+TimeSpan.Parse(txtSatOUT.Text)+"')");
                    
                    await this.ShowMessageAsync("Schedule", "Successfully Added");
                    btADD.Content = "ADD NEW";
                    dtSchedules.ItemsSource = SchedList("SELECT * FROM admx_hrisp.pp_schedules S inner join admx_hrisp.pp_scheduletype T on S.TypeID = T.TypeID");
                    isEnable(false, "00:00:00");
                }
              

            }
        }

        private void btCANCEL_Click(object sender, RoutedEventArgs e)
        {
            btADD.Content = "ADD NEW";
            isEnable(false, "00:00:00");
        }

        private void isEnable(Boolean _Ans,String _Content)
        {
            cmbSchedType.IsEnabled = _Ans;
            txtSchedName.IsEnabled = _Ans;
            txtSunIN.IsEnabled = _Ans;
            txtSunOut.IsEnabled = _Ans;
            txtMonIN.IsEnabled = _Ans;
            txtMonOUT.IsEnabled = _Ans;
            txtTueIN.IsEnabled = _Ans;
            txtTueOUT.IsEnabled = _Ans;
            txtWedIN.IsEnabled = _Ans;
            txtWedOUT.IsEnabled = _Ans;
            txtThuIN.IsEnabled = _Ans;
            txtThuOUT.IsEnabled = _Ans;
            txtFriIN.IsEnabled = _Ans;
            txtFriOUT.IsEnabled = _Ans;
            txtSatIN.IsEnabled = _Ans;
            txtSatOUT.IsEnabled = _Ans;

            txtSchedName.Text = "";
            txtSunIN.Text = _Content;
            txtSunOut.Text =  _Content;
            txtMonIN.Text =  _Content;
            txtMonOUT.Text =  _Content;
            txtTueIN.Text =  _Content;
            txtTueOUT.Text =  _Content;
            txtWedIN.Text =  _Content;
            txtWedOUT.Text =  _Content;
            txtThuIN.Text =  _Content;
            txtThuOUT.Text =  _Content;
            txtFriIN.Text =  _Content;
            txtFriOUT.Text =  _Content;
            txtSatIN.Text =  _Content;
            txtSatOUT.Text =  _Content;



        }
       
     
    }
}
