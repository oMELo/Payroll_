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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Threading;
namespace Payroll_
{
   
    public class ScheduleList
    {
        public static List<ScheduleList> _SchedList = new List<ScheduleList>();

        public Int32 _SchedID { get; set; }
        public String _SchedName { get; set; }
        public String _SchedType { get; set; }
        public TimeSpan _SunIN { get; set; }
        public TimeSpan _SunOUT { get; set; }

        public TimeSpan _MonIN { get; set; }
        public TimeSpan _MonOUT { get; set; }

        public TimeSpan _TueIN { get; set; }
        public TimeSpan _TueOUT { get; set; }

        public TimeSpan _WedIN { get; set; }
        public TimeSpan _WedOUT { get; set; }

        public TimeSpan _ThuIN { get; set; }
        public TimeSpan _ThuOUT { get; set; }

        public TimeSpan _FriIN { get; set; }
        public TimeSpan _FriOUT { get; set; }

        public TimeSpan _SatIN { get; set; }
        public TimeSpan _SatOUT { get; set; }
    }
    public class NameToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string input = value as string;
            switch (input)
            {
                case "John":
                    return Brushes.LightGreen;
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class clsSchedule: INotifyPropertyChanged
    {

        public string SchedCount {get;set;}
        
        private string _image;
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Helpers

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
        public static List<clsSchedule> _EmpSchedList = new List<clsSchedule>();
        public static List<clsSchedule> _ScheduleList = new List<clsSchedule>();
        public int _empID { get; set; }
        public string _Name { get; set; }
        //public int _SchedCount { get; set; }
        public string _SchedType { get; set; }
        public static Int32 _SchedID { get; set; }
        public static Int32 _SelempID { get; set; }
        public static Int32 _SelSchedID { get; set;  }
        public string _SchedName { get; set; }
        public string _JobTitle { get; set; }
        public string _Department { get; set; }
        public string _Effectivity { get; set; }
        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                NotifyPropertyChanged("Image");
            }
        }
        //public string Image
        //{
        //    get
        //    {
        //        if (IsRead)
        //            return "read.png";
        //        return "unread.png";
        //    }
        //}
    }

    public class clsCurSchedule
    {

        public static List<clsCurSchedule> _CurSchedule = new List<clsCurSchedule>();
        public int _empID { get; set; }
        public string _Name { get; set; }
        public string _Effectivity { get; set; }
        public string _SchedName { get; set; }
        public string _SchedType { get; set; }

    }
    public class TempData
    {
        public static List<TempData> _Site = new List<TempData>();
        public String __EmpID { get; set; }
        public String __DateIN { get; set; }
        public String __DateOUT { get; set; }
    
    }
    public class clsDATA
    {

        public int _ID { get; set; }
        public string _Description { get; set; }
        public static List <clsDATA> _Site = new List<clsDATA>();
    }
    public class SQLData
    {
        public string _CheckDate { get; set; }
        public string _CheckTime { get; set; }
        public string _State { get; set; }

        public static List<SQLData> _SQLDATA = new List<SQLData>();
    }
    public class fillCMB
    {

        public int _ID { get; set; }
        public string _Value { get; set; }
    
        public void getCMB(ComboBox _Cmb,int _ID)
        {
   
            _Cmb.ItemsSource = "";
            List<fillCMB> ListData = new List<fillCMB>();
            using (Database _DB = new Database())
            {
                _DB.Open("SELECT fld_StaticParamDesc as SiteDesc,fld_StaticParamID as SiteID FROM admx_hrisp.tbl_staticparam where fld_CategoryID = " + _ID + " order by SiteDesc");
                while (_DB.Reader.Read())
                {          
                    ListData.Add(new fillCMB { _ID = Convert.ToInt32(_DB.Reader["SiteID"].ToString()), _Value = Convert.ToString(_DB.Reader["SiteDesc"].ToString()) });
                }

                _Cmb.ItemsSource = ListData;
                _Cmb.DisplayMemberPath = "_Value";
                _Cmb.SelectedValuePath = "_ID";
                
            }
        }
        public void getQRYCMB(ComboBox _Cmb, String _Query)
        {

            _Cmb.ItemsSource = "";
            List<fillCMB> ListData = new List<fillCMB>();
            using (Database _DB = new Database())
            {
                _DB.Open(_Query);
                while (_DB.Reader.Read())
                {
                    ListData.Add(new fillCMB { _ID = Convert.ToInt32(_DB.Reader["ID"].ToString()), _Value = Convert.ToString(_DB.Reader["Description"].ToString()) });
                }

                _Cmb.ItemsSource = ListData;
                _Cmb.DisplayMemberPath = "_Value";
                _Cmb.SelectedValuePath = "_ID";

            }
        }
    
    }
    public class fillOtherCMB
    {

        public int _ID { get; set; }
        public string _Value { get; set; }

        public void getCMB(ComboBox _Cmb, String _Query)
        {

            _Cmb.ItemsSource = "";
            List<fillCMB> ListData = new List<fillCMB>();
            using (Database _DB = new Database())
            {
                _DB.Open(_Query);
                while (_DB.Reader.Read())
                {
                    ListData.Add(new fillCMB { _ID = Convert.ToInt32(_DB.Reader["TypeID"].ToString()), _Value = Convert.ToString(_DB.Reader["Description"].ToString()) });
                }

                _Cmb.ItemsSource = ListData;
                _Cmb.DisplayMemberPath = "_Value";
                _Cmb.SelectedValuePath = "_ID";

            }
        }

    }
    public class clsEMPEARNING
    {

      
            public static int _UserID { get; set; }
            public static int _ActiveEmpID { get; set; }
            public static string _FullName { get; set; }
            public int _EarningID { get; set; }
            public int _empID { get; set; }
            public string _Amount { get; set; }
            public string _MOD { get; set; }
            public bool _Active { get; set; }
            public DateTime _effectivity;
            public String _Effectivity { get; set; }

            

          

            public static Boolean _Samp { get; set; }
        
      
            public List<clsEMPEARNING> _EARNLIST = new List<clsEMPEARNING>();

          
            public List<clsEMPEARNING> _EmpMOD(int _empID,Boolean _ShowAll)
            {
                Database _Database = new Database();
                _EARNLIST.Clear();

                string _ADDquery = _ShowAll == true ? " " : " and E.isActive = 1 ";

               
                 string tmpSql = "select E.[EARNING_ID],ST.[fld_StaticParamCode],MP.fld_IDNumber,MP.[fld_FirstName], MP.[fld_MiddleName],MP.[fld_LastName],E.[AMOUNT]"+
                                ",[fld_StaticParamDesc],convert(varchar(10),[EFFECTIVITY],101) as [EFFECTIVITY],E.isActive  from [dbo].[tbl_EmpMasterFile] MP " +
                                 "inner join [dbo].[tbl_Earning] E " +
                                     "on MP.[fld_IDNumber] = E.[fld_IDNumber] " +
                                 "inner join [dbo].[tbl_StaticParam] ST  " +
                                     "on E.[fld_StaticParamID] =ST.[fld_StaticParamID] " +
                                     "where ST.[fld_CategoryID] = 11 and  E.[fld_IDNumber] = " + _empID +  _ADDquery +
                                     " order by E.[fld_StaticParamID] ASC , E.[EFFECTIVITY] DESC";
                 _Database.Open(tmpSql);
                while (_Database.Reader.Read())
                {
                    _FullName = _Database.Reader["fld_FirstName"].ToString() + " " + _Database.Reader["fld_MiddleName"].ToString() + ", " + _Database.Reader["fld_LastName"].ToString();
                    _EARNLIST.Add(new clsEMPEARNING()
                    {
                        _EarningID = int.Parse(_Database.Reader["EARNING_ID"].ToString()),
                        _empID = int.Parse(_Database.Reader["fld_IDNumber"].ToString()),
                        _MOD = _Database.Reader["fld_StaticParamCode"].ToString(),
                        _Amount = string.Format("{0:0.00}", Convert.ToDecimal( _Database.Reader["AMOUNT"].ToString())),
                        _Effectivity = _Database.Reader["Effectivity"].ToString(),
                        _Active=Convert.ToBoolean( _Database.Reader["isActive"].ToString())
                    });

                } _Database.Reader.Close();
                return _EARNLIST;

            }
            public string _MODVal(string tmpArray,int _Row,int _Col)
            {

                var _tmpArray =  tmpArray.Remove(Convert.ToInt32(tmpArray.Length - 1)).Split(';').Select(x => x.Split(',')).ToArray();
                return _tmpArray[_Row][_Col];
            }
            
    }


   public class clsAttendance
   {
       public static List<clsAttendance> _Attendance = new List<clsAttendance>();
       public static List<clsAttendance> _SEARCHLIST = new List<clsAttendance>();
       public List<clsAttendance> _EmpAttendance = new List<clsAttendance>();
       public int _empID { get; set; }
       public string _Name { get; set; }
       public int _ID { get; set; }

       public string _DateiN { get; set; }
       public string _DateOut { get; set; }

       public static Boolean IsSelected { get; set; }
   }
   public class clsStatic 
   {
       public static string _Status { get; set; }
       public static string _dtFromTime { get; set; }
       public static string _dtToTime { get; set; }

       public static string _GeneratedFromTime { get; set; }
       public static string _GeneratedToTime { get; set; }
   }
   public class clsPPeriod : INotifyPropertyChanged
   {
       private string _image;
       
       #region INotifyPropertyChanged Members

       public event PropertyChangedEventHandler PropertyChanged;

       #endregion

       #region Private Helpers

       private void NotifyPropertyChanged(string propertyName)
       {
           if (PropertyChanged != null)
           {
               PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
           }
       }

       #endregion

       public Int32 _ID { get; set; }
       public DateTime _pPeriod { get; set; }
       public DateTime _pStartDate { get; set; }
       public DateTime _pEndDate { get; set; }
       public DateTime _mStartDate { get; set; }
       public DateTime _mEndDate { get; set; }
       public static Int32 _SelID { get; set; }
       public static List<clsPPeriod> _pPeriodList = new List<clsPPeriod>();
       
       public string Image
       {
           get { return _image; }
           set
           {
               _image = value;
               NotifyPropertyChanged("Image");
           }
       }
     
   }
   public class clsAttCal
   {
       public Int32 _ID { get; set; }
       public Int32 _PayrollID { get; set; }
       public Int32 _EmpID { get; set; }
       public string _Fname {get;set;}
       public string _EStat {get;set;}
       public decimal _Mata { get; set; }
       public decimal _Regular { get; set; }
       public decimal _LegHoliday { get; set; }
       public decimal _OTRestDay { get; set; }
       public decimal _OTRegular { get; set; }
       public decimal _OTLegHoliday { get; set; }
       public decimal _OTSpeHoliday { get; set; }
       public decimal _Absences { get; set; }
       public decimal _Late { get; set; }
       public decimal _VL { get; set; }
       public decimal _SL { get; set; }

       public String _LWOP { get; set; }
       public static List<clsAttCal> _AttCal = new List<clsAttCal>();
   
   }

   public class clsChecking
   {

       public static List<clsChecking> _EmpLate = new List<clsChecking>();
       public static List<clsChecking> _EmpRegular = new List<clsChecking>();
       public static List<clsChecking> _EmpMata = new List<clsChecking>();
       public static List<clsChecking> _Holiday = new List<clsChecking>();
       public static List<clsChecking> _OTRegularList = new List<clsChecking>();
       public static List<clsChecking> _OTRestDayList = new List<clsChecking>();
       public static List<clsChecking> _OTHolidayList = new List<clsChecking>();
       public static List<clsChecking> _OTSpecialHolidayList = new List<clsChecking>();
       public static List<clsChecking> _EmpAbsences = new List<clsChecking>();
       public static List<clsChecking> _VacationLeave = new List<clsChecking>();
       public static List<clsChecking> _SickLeave = new List<clsChecking>();
       public static List<clsChecking> _EmergencyLeave = new List<clsChecking>();
       public static List<clsChecking> _MaternityLeave = new List<clsChecking>();
       public static List<clsChecking> _PaternityLeave = new List<clsChecking>();
       public static String _SelEmpNO { get; set; }
       public String _EmpNO { get; set; }

       public Int32 _ctr{ get; set; }

       public static DateTime _StartPayroll;
       public static DateTime _EndPayroll;

       public static DateTime _StartMata;
       public static DateTime _EndMata;

       public DateTime _DateFiled { get; set; }
       public DateTime _LeaveDateFrom { get; set; }
       public DateTime _LeaveDateTo { get; set; }
       public Double _LeaveTotal { get; set; }
       public string _CurSchedule { get; set; }
       public string _Week { get; set; }
       public string _CurDateIN { get; set; }
       public string _CurDateOUT { get; set; }
       public string _Status { get; set; }
       public string _DailyStat { get; set; }
       public string _Date { get; set; }
       public string _DateIN { get; set; }
       public string _DateOUT { get; set; }
       public string _HolidayDesc { get; set; }
       public DateTime _HolidayDate { get; set; }

       public DateTime _OTDate { get; set; }
       public TimeSpan _OTStart { get; set; }
       public TimeSpan _OTEnd { get; set; }
       public Double _OTTotal { get; set; }
       public string _Reason { get; set; }

       public string _PayrollRange { get; set; }
       public string _Schedule { get; set; }


   }
   public class DataGridNumericUpDownColumn : DataGridBoundColumn
   {
       private static Style _defaultEditingElementStyle;
       private static Style _defaultElementStyle;
       private double minimum = (double)NumericUpDown.MinimumProperty.DefaultMetadata.DefaultValue;
       private double maximum = (double)NumericUpDown.MaximumProperty.DefaultMetadata.DefaultValue;
       private double interval = (double)NumericUpDown.IntervalProperty.DefaultMetadata.DefaultValue;
       private string stringFormat = (string)NumericUpDown.StringFormatProperty.DefaultMetadata.DefaultValue;
       private bool hideUpDownButtons = (bool)NumericUpDown.HideUpDownButtonsProperty.DefaultMetadata.DefaultValue;
       private double upDownButtonsWidth = (double)NumericUpDown.UpDownButtonsWidthProperty.DefaultMetadata.DefaultValue;
       private Binding foregroundBinding;

       static DataGridNumericUpDownColumn()
       {
           ElementStyleProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
           EditingElementStyleProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
       }

       public static Style DefaultEditingElementStyle
       {
           get
           {
               if (_defaultEditingElementStyle == null)
               {
                   Style style = new Style(typeof(NumericUpDown));
                   style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top));
                   style.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                   style.Setters.Add(new Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                   style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0d)));
                   style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
                   style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 0d));
                   style.Seal();
                   _defaultEditingElementStyle = style;
               }

               return _defaultEditingElementStyle;
           }
       }

       public static Style DefaultElementStyle
       {
           get
           {
               if (_defaultElementStyle == null)
               {
                   Style style = new Style(typeof(NumericUpDown));

                   style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top));
                   style.Setters.Add(new Setter(UIElement.IsHitTestVisibleProperty, false));
                   style.Setters.Add(new Setter(UIElement.FocusableProperty, false));
                   style.Setters.Add(new Setter(NumericUpDown.HideUpDownButtonsProperty, true));
                   style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0d)));
                   style.Setters.Add(new Setter(Control.BackgroundProperty, Brushes.Transparent));
                   style.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                   style.Setters.Add(new Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                   style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
                   style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 0d));
                   style.Setters.Add(new Setter(ControlsHelper.DisabledVisualElementVisibilityProperty, Visibility.Collapsed));

                   style.Seal();
                   _defaultElementStyle = style;
               }

               return _defaultElementStyle;
           }
       }

       internal void ApplyBinding(DependencyObject target, DependencyProperty property)
       {
           BindingBase binding = Binding;
           if (binding != null)
           {
               BindingOperations.SetBinding(target, property, binding);
           }
           else
           {
               BindingOperations.ClearBinding(target, property);
           }
       }

       private static void ApplyBinding(BindingBase binding, DependencyObject target, DependencyProperty property)
       {
           if (binding != null)
           {
               BindingOperations.SetBinding(target, property, binding);
           }
           else
           {
               BindingOperations.ClearBinding(target, property);
           }
       }

       internal void ApplyStyle(bool isEditing, bool defaultToElementStyle, FrameworkElement element)
       {
           Style style = PickStyle(isEditing, defaultToElementStyle);
           if (style != null)
           {
               element.Style = style;
           }
       }

       protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
       {
           return GenerateNumericUpDown(true, cell);
       }

       protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
       {
           NumericUpDown generateNumericUpDown = GenerateNumericUpDown(false, cell);
           generateNumericUpDown.HideUpDownButtons = true;
           return generateNumericUpDown;
       }

       private NumericUpDown GenerateNumericUpDown(bool isEditing, DataGridCell cell)
       {
           NumericUpDown numericUpDown = (cell != null) ? (cell.Content as NumericUpDown) : null;
           if (numericUpDown == null)
           {
               numericUpDown = new NumericUpDown();
               // create binding to cell foreground to get changed brush from selection
               foregroundBinding = new Binding("Foreground") { Source = cell, Mode = BindingMode.OneWay };
           }

           ApplyStyle(isEditing, true, numericUpDown);
           ApplyBinding(numericUpDown, NumericUpDown.ValueProperty);

           if (!isEditing)
           {
               // bind to cell foreground to get changed brush from selection
               ApplyBinding(foregroundBinding, numericUpDown, Control.ForegroundProperty);
           }
           else
           {
               // no foreground change for editing
               BindingOperations.ClearBinding(numericUpDown, Control.ForegroundProperty);
           }

           numericUpDown.Minimum = Minimum;
           numericUpDown.Maximum = Maximum;
           numericUpDown.StringFormat = StringFormat;
           numericUpDown.Interval = Interval;
           numericUpDown.InterceptArrowKeys = true;
           numericUpDown.InterceptMouseWheel = true;
           numericUpDown.Speedup = true;
           numericUpDown.HideUpDownButtons = HideUpDownButtons;
           numericUpDown.UpDownButtonsWidth = UpDownButtonsWidth;

           return numericUpDown;
       }

       private Style PickStyle(bool isEditing, bool defaultToElementStyle)
       {
           Style style = isEditing ? EditingElementStyle : ElementStyle;
           if (isEditing && defaultToElementStyle && (style == null))
           {
               style = ElementStyle;
           }

           return style;
       }

       public double Minimum
       {
           get { return minimum; }
           set { minimum = value; }
       }

       public double Maximum
       {
           get { return maximum; }
           set { maximum = value; }
       }

       public double Interval
       {
           get { return interval; }
           set { interval = value; }
       }

       public string StringFormat
       {
           get { return stringFormat; }
           set { stringFormat = value; }
       }

       public bool HideUpDownButtons
       {
           get { return hideUpDownButtons; }
           set { hideUpDownButtons = value; }
       }

       public double UpDownButtonsWidth
       {
           get { return upDownButtonsWidth; }
           set { upDownButtonsWidth = value; }
       }
       
   }

}
