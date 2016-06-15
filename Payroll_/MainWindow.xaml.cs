
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new VM();
            //Database _database = new Database();
          
        }
        //*****************************************
        public class VM
        {
            public List<myItem> Source { get; set; }

            public VM()
            {
                Source = new List<myItem>();
                Source.Add(new myItem { Field1 = "some Text", Field2 = "some other Text", ColorSwitch = false });
                Source.Add(new myItem { Field1 = "some Text", Field2 = "some other Text", ColorSwitch = false });
                Source.Add(new myItem { Field1 = "some Text", Field2 = "some other Text", ColorSwitch = true });
                Source.Add(new myItem { Field1 = "some Text", Field2 = "some other Text", ColorSwitch = false });
                Source.Add(new myItem { Field1 = "some Text", Field2 = "some other Text", ColorSwitch = true });
            }
        }
        public class myItem
        {
            public string Field1 { get; set; }
            public string Field2 { get; set; }
            public bool ColorSwitch { get; set; }
        }
        //*****************************************
    }
}
