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
using AdmereX;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
namespace Payroll_
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1:MetroWindow
    {
        public Window1()
        {
            InitializeComponent();
            //CurrentSettings();
            //User.Login(this, "test", "");
            //User.SetAccess(1, false, "20130038");
        }
        private void LaunchSettings(object sender, RoutedEventArgs e)
        {
            FlyOut.IsModal = true;
            FlyOut.IsOpen = true;

        }

        private void TileEmpSched_Click(object sender, RoutedEventArgs e)
        {
           
            MainFrame.Navigate(new Uri("EmpSchedule.xaml", UriKind.Relative));
      
        }

        private void TileEmpAtt_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("CurAttendance.xaml", UriKind.Relative));
        }

        private void TileDownload_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("AttDownload.xaml", UriKind.Relative));

        }

        private  void TilePayroll_Click(object sender, RoutedEventArgs e)
        {
      
            PPeriod _payroll=new PPeriod();
            //_payroll.Parent();
            _payroll.Show();
         
           
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
   
            Settings.Server = txtServerName.Text;
            Settings.Database = txtDatabase.Text;
            Settings.Username = txtHRISPUsername.Text;
            Settings.Password = txtHRISPPassword.Password;
           
            Properties.Settings.Default.Makati = txtAMSMakati.Text;
            Properties.Settings.Default.Mandaluyong =txtAMSMandaluyong.Text ;
            Properties.Settings.Default.Cebu = txtAMSCebu.Text;
            Properties.Settings.Default.AMS_Username = txtAMSUsername.Text;
            Properties.Settings.Default.AMS_Password = txtAMSPassword.Password;
            Properties.Settings.Default.Save();

            CurrentSettings();

        }

        private async void btConnection_Click(object sender, RoutedEventArgs e)
        {
            
            String Conn = "";
            try
            {
                using (MySqlConnection _Database = new MySqlConnection("server=" + txtServerName.Text + ";database= "+txtDatabase.Text+";user id="+txtHRISPUsername.Text+";password="+txtHRISPPassword.Password+""))
                {
                    Conn = "HRISP";
                    _Database.Open();
                    errTxtBox.Text = errTxtBox.Text + Conn+ " Connected" + Environment.NewLine;
                    await TaskEx.Delay(1000);
                }

             
                using (SqlConnection _SqlConn = new SqlConnection("server=" + txtAMSMakati.Text + ";database=ATT_db;user id="+txtAMSUsername.Text+";password="+txtAMSPassword.Password+""))
                {
                    Conn = "MAKATI AMS";
                    _SqlConn.Open();
                    errTxtBox.Text = errTxtBox.Text + Conn + " Connected" + Environment.NewLine;
                    await TaskEx.Delay(1000);
                   
                }

                using (SqlConnection _SqlConn = new SqlConnection("server=" + Properties.Settings.Default.Mandaluyong + ";database=ATT_db;user id=" + txtAMSUsername.Text + ";password=" + txtAMSPassword.Password + ""))
                {
                    Conn = "MANDALUYONG AMS";
                    _SqlConn.Open();
                    errTxtBox.Text = errTxtBox.Text + Conn +" Connected"+ Environment.NewLine;
                    await TaskEx.Delay(1000);
                }
                using (SqlConnection _SqlConn = new SqlConnection("Data Source=" + Properties.Settings.Default.Cebu + ";database=ATT_db;user id=" + txtAMSUsername.Text + ";password=" + txtAMSPassword.Password + ""))
                {
                    Conn = "CEBU AMS";
                    _SqlConn.Open();
                    errTxtBox.Text = errTxtBox.Text + Conn + " Connected" + Environment.NewLine;
                    await TaskEx.Delay(1000);
                }

                await this.ShowMessageAsync("Connection", "Status Connected");
            }
            catch (Exception err)
            {
                errTxtBox.Text = errTxtBox.Text + Conn  +" Failed to Connect"+ Environment.NewLine + "Error Message: " + err.Message + Environment.NewLine + "Error Number: " + err.HResult.ToString();
            
            }

        }

        private void CurrentSettings()

        {

            txtServerName.Text = Settings.Server;
            txtDatabase.Text=Settings.Database;
            txtHRISPUsername.Text= Settings.Username;
            txtHRISPPassword.Password=Settings.Password;

            //Functions.Cryptor("aRxyzL", Properties.Settings.Default.Makati, false);
            txtAMSMakati.Text =  Properties.Settings.Default.Makati;
            txtAMSMandaluyong.Text = Properties.Settings.Default.Mandaluyong;
            txtAMSCebu.Text = Properties.Settings.Default.Cebu;
            txtAMSUsername.Text=   Properties.Settings.Default.AMS_Username;
            txtAMSPassword.Password = Properties.Settings.Default.AMS_Password;
        }
     
      
    }
    
}
