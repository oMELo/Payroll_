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
//using AdmereX;
using System.IO;
using MahApps;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;

namespace Payroll_
{
    /// <summary>
    /// Interaction logic for DateTime.xaml
    /// </summary>
    public partial class frmDateTime
    {

        public frmDateTime()
        {
            InitializeComponent();

            for (int i = 0; i < 24; i++)
            {
                if (i < 10) { cmbDtToHH.Items.Add("0" + i); cmbDtFromHH.Items.Add("0" + i); }
                else { cmbDtToHH.Items.Add(i); cmbDtFromHH.Items.Add(i); }
            }

            for (int i = 0; i < 60; i++)
            {
                if (i < 10) { cmbDtFromMM.Items.Add("0" + i); cmbDtToMM.Items.Add("0" + i); }
                else { cmbDtFromMM.Items.Add(i); cmbDtToMM.Items.Add(i); }
            }
            _default();


            if (clsStatic._Status.ToString() == "EDIT")
            {
                isEnable(true);

                if (clsStatic._dtFromTime == "") { dtFrom.SelectedDate = null; dtFrom.BorderBrush = Brushes.Red; cmbDtFromHH.BorderBrush = Brushes.Red; cmbDtFromMM.BorderBrush = Brushes.Red; }
                else { dtFrom.SelectedDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", clsStatic._dtFromTime)); }

                if (clsStatic._dtToTime == "") { dtTo.SelectedDate = null; dtTo.BorderBrush = Brushes.Red; cmbDtToHH.BorderBrush = Brushes.Red; cmbDtToMM.BorderBrush = Brushes.Red; }
                else { dtTo.SelectedDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", clsStatic._dtToTime)); }

                cmbDtFromHH.SelectedIndex = clsStatic._dtFromTime == "" ? 0 : Convert.ToDateTime(sString(clsStatic._dtFromTime)).Hour;
                cmbDtFromMM.SelectedIndex = clsStatic._dtFromTime == "" ? 0 : Convert.ToDateTime(sString(clsStatic._dtFromTime)).Minute;

                cmbDtToHH.SelectedIndex = clsStatic._dtToTime == "" ? 0 : Convert.ToDateTime(sString(clsStatic._dtToTime)).Hour;
                cmbDtToMM.SelectedIndex = clsStatic._dtToTime == "" ? 0 : Convert.ToDateTime(sString(clsStatic._dtToTime)).Minute;

            }
            else if (clsStatic._Status.ToString() == "ATTENDANCE UPDATE")
            {

            }
            else if (clsStatic._Status.ToString() == "GENERATE")
            {
                isEnable(false);
                _default();

            }



        }
        public string sString(string _tmp)
        {
            if (_tmp == "")
            {
                return "0";
            }

            return String.Format("{0:h hh H HH}", _tmp);


        }

        private void _default()
        {

            cmbDtFromHH.SelectedIndex = 0;
            cmbDtToHH.SelectedIndex = 23;
            cmbDtFromMM.SelectedIndex = 0;
            cmbDtToMM.SelectedIndex = 59;


            dtFrom.SelectedDate = DateTime.Now;
            dtTo.SelectedDate = DateTime.Now;

        }


        private void btOK_Click(object sender, RoutedEventArgs e)
        {

            switch (clsStatic._Status)
            {
                case "GENERATE":

                    clsStatic._GeneratedFromTime = dtFrom.Text;
                    clsStatic._GeneratedToTime = dtTo.Text;
                    //clsStatic._dtFromTime = dtFrom.Text;
                    //clsStatic._dtToTime = dtTo.Text;

                    break;
                case "ADD":
                    clsStatic._dtFromTime = String.Format("{0:G}", Convert.ToDateTime(dtFrom.Text + ' ' + cmbDtFromHH.Text + ':' + cmbDtFromMM.Text));
                    clsStatic._dtToTime = String.Format("{0:G}", Convert.ToDateTime(dtTo.Text + ' ' + cmbDtToHH.Text + ':' + cmbDtToMM.Text));

                    break;
                case "EDIT":

                    clsStatic._dtFromTime = String.Format("{0:G}", Convert.ToDateTime(dtFrom.Text + ' ' + cmbDtFromHH.Text + ':' + cmbDtFromMM.Text));
                    clsStatic._dtToTime = String.Format("{0:G}", Convert.ToDateTime(dtTo.Text + ' ' + cmbDtToHH.Text + ':' + cmbDtToMM.Text));

                    break;
                case "ATTENDANCE UPDATE":

                    //clsEMPEARNING._dtFromTime = String.For mat("{0:G}", Convert.ToDateTime( dtFrom.Text + ' ' + cmbDtFromHH.Text + ':' + cmbDtFromMM.Text));
                    //clsEMPEARNING._dtToTime = String.Format("{0:G}", Convert.ToDateTime(dtTo.Text + ' ' + cmbDtToHH.Text + ':' + cmbDtToMM.Text));

                    break;

            }

            this.Close();

        }

        private void isEnable(Boolean _ask)
        {


            cmbDtFromHH.IsEnabled = _ask;
            cmbDtFromMM.IsEnabled = _ask;
            cmbDtToHH.IsEnabled = _ask;
            cmbDtToMM.IsEnabled = _ask;

        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            //clsStatic._Status = "CANCEL";
            this.Close();


        }

        private void dtTo_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (dtTo.Text != "") dtTo.BorderBrush = Brushes.Black;
        }

        private void dtFrom_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (dtFrom.Text != "") dtFrom.BorderBrush = Brushes.Black;
        }


        private void cmbDtToHH_DropDownClosed(object sender, EventArgs e)
        {
            cmbDtToHH.BorderBrush = Brushes.Black;
        }

        private void cmbDtToMM_DropDownClosed(object sender, EventArgs e)
        {
            cmbDtToMM.BorderBrush = Brushes.Black;
        }

        private void cmbDtFromHH_DropDownClosed(object sender, EventArgs e)
        {
            cmbDtFromHH.BorderBrush = Brushes.Black;
        }

        private void cmbDtFromMM_DropDownClosed(object sender, EventArgs e)
        {
            cmbDtFromMM.BorderBrush = Brushes.Black;
        }
    }
}
