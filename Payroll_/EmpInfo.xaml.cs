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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdmereX;
using MySql.Data.MySqlClient;
namespace Payroll_
{
    /// <summary>
    /// Interaction logic for EmpInfo.xaml
    /// </summary>
    public partial class EmpInfo 
    {
        Database _Database = new Database();
        public EmpInfo()
        {
            InitializeComponent();
            if (clsChecking._SelEmpNO != "")  fill();
        }

        private void fill ()
        { 

         clsTotalEmpCalc _TotalEmpCalc = new clsTotalEmpCalc();

                dtLate.ItemsSource = "";
                //dtRegular.ItemsSource = "";
                dtMata.ItemsSource = "";
                dtHoliday.ItemsSource="";
                dtRegularOT.ItemsSource = "";
                dtRestDayOT.ItemsSource = "";
                dtHoliday.ItemsSource = "";            
                dtSickLeave.ItemsSource="";
                TileLate.Content = "Date from " +  String.Format("{0:MM/dd/yyyy}",clsChecking._StartMata) +" to "+ String.Format("{0:MM/dd/yyyy}",clsChecking._EndMata);
                TileMata.Content = "Date from " + String.Format("{0:MM/dd/yyyy}",clsChecking._StartMata) + " to " +  String.Format("{0:MM/dd/yyyy}",clsChecking._EndMata);
                TileOT.Content = "Date from " + String.Format("{0:MM/dd/yyyy}", clsChecking._StartMata) + " to " + String.Format("{0:MM/dd/yyyy}", clsChecking._EndMata);

                dtLate.ItemsSource = getEmpLate(clsChecking._SelEmpNO,  clsChecking._StartMata, clsChecking._EndMata);
                dtMata.ItemsSource = GetMata(clsChecking._SelEmpNO, clsChecking._StartMata, clsChecking._EndMata);
                dtHoliday.ItemsSource=GetHoliday(clsChecking._StartMata, clsChecking._EndMata);
                dtRegularOT.ItemsSource = GetRegularOT(clsChecking._SelEmpNO, clsPPeriod._SelID);
                dtRestDayOT.ItemsSource = GetRestDayOT(clsChecking._SelEmpNO, clsPPeriod._SelID);
                dtLegalHolidayOT.ItemsSource = GetHolidayOT(clsChecking._SelEmpNO, clsPPeriod._SelID);
                dtLegalHolidayOT.ItemsSource = GetSpecialHolidayOT(clsChecking._SelEmpNO, clsPPeriod._SelID);
                dtVacationLeave.ItemsSource = GetVacationLeave(clsChecking._SelEmpNO, clsChecking._StartMata, clsChecking._EndMata);
                dtSickLeave.ItemsSource = GetSickLeave(clsChecking._SelEmpNO, clsChecking._StartMata, clsChecking._EndMata);
                dtEmergencyLeave.ItemsSource = GetEmergencyLeave(clsChecking._SelEmpNO, clsChecking._StartMata, clsChecking._EndMata);
                dtMaternityLeave.ItemsSource = GetMaternityLeave(clsChecking._SelEmpNO, clsChecking._StartMata, clsChecking._EndMata);
                dtPaternityLeave.ItemsSource = GetPaternityLeave(clsChecking._SelEmpNO, clsChecking._StartMata, clsChecking._EndMata);

        }
        public List<clsChecking> getEmpLate(string EmpNo,DateTime PayrollStart, DateTime PayrollEnd)
        {
          
            clsChecking._EmpLate.Clear();
            clsTotalEmpCalc _clsTotalEmpCalc = new clsTotalEmpCalc();
           
            TimeSpan A = PayrollEnd - PayrollStart;
            TimeSpan _TotalLate = TimeSpan.Zero;

            using (Database _dbRD = new Database())
            {
                int i=1;
                for (int _absences = 0; _absences <= A.Days; _absences++)
                {
                  
                    string _In = "";
                    string _Out = "";
                    string _Sched = "";
                    DateTime dtANS = PayrollStart.AddDays(_absences);


                    _dbRD.Open("Select * from  admx_hrisp.pp_schedules where SchedID = " + _clsTotalEmpCalc.getSchedule(EmpNo, string.Format("{0:yyyy-MM-dd}", dtANS)));
          
                    {
                        string __DailyStat = "";
                        string _Stat = "";
                        while (_dbRD.Reader.Read())
                        {
                            TimeSpan late = TimeSpan.Zero;
                            switch (string.Format("{0:dddd}", dtANS))
                            {
                                case "Sunday":
                                    _Sched = _dbRD.Reader["SunIN"].ToString();
                                    if (_dbRD.Reader["SunIN"].ToString() != "00:00:00")
                                    {
                                        
                                            if (clsTotalEmpCalc.isLeave(EmpNo, dtANS) == false)
                                            {
                                                using (Database _curDb = new Database())
                                                {
                                                    _curDb.Open("select EC._DateIN as DateIN, EC._DateOUT as DateOUT,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _dbRD.Reader["SunIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _dbRD.Reader["SunIN"].ToString() + "')),'00:00:00') as Late  from admx_hrisp.pp_empclocks EC " +
                                                                                        "where  _EmpID = " + EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')");
                                                    if (_curDb.Reader.HasRows == false) __DailyStat = "Absent";
                                                    else
                                                    {
                                                        while (_curDb.Reader.Read())
                                                        {
                                                            if (Convert.ToInt32(_dbRD.Reader["TypeID"].ToString()) != 3) _Stat = _curDb.Reader["Late"].ToString();
                                                            else _Stat ="00:00:00";
                                                            _In = _curDb.Reader["DateIN"].ToString();
                                                            _Out = _curDb.Reader["DateOUT"].ToString();
                                                            if (_curDb.Reader["DateIN"].ToString() == "" || _curDb.Reader["DateOUT"].ToString() == "") __DailyStat = "Half Day";
                                                            else __DailyStat = "Regular";
                                                            //__DailyStat = "Regular";
                                                        }
                                                    }
                                                }

                                            }
                                            else __DailyStat = "Leave";
                                        
                                    }
                                    else __DailyStat = "RestDay";

                                    break;
                                case "Monday":
                                    _Sched = _dbRD.Reader["MonIN"].ToString();
                                    if (_dbRD.Reader["MonIN"].ToString() != "00:00:00")
                                    {
                                        if (clsTotalEmpCalc.isLeave(EmpNo, dtANS) == false)
                                        {
                                            using (Database _curDb = new Database())
                                            {
                                                _curDb.Open("select EC._DateIN as DateIN, EC._DateOUT as DateOUT,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _dbRD.Reader["MonIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _dbRD.Reader["MonIN"].ToString() + "')),'00:00:00') as Late  from admx_hrisp.pp_empclocks EC " +
                                                                                    "where  _EmpID = " + EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')");

                                                if (_curDb.Reader.HasRows == false) __DailyStat = "Absent";
                                                
                                                else
                                                {
                                                    while (_curDb.Reader.Read())
                                                    {
                                                        if (Convert.ToInt32(_dbRD.Reader["TypeID"].ToString()) != 3) _Stat = _curDb.Reader["Late"].ToString();
                                                        else _Stat ="00:00:00";
                                                        _In = _curDb.Reader["DateIN"].ToString();
                                                        _Out = _curDb.Reader["DateOUT"].ToString();
                                                        if (_curDb.Reader["DateIN"].ToString() == "" || _curDb.Reader["DateOUT"].ToString() == "") __DailyStat = "Half Day";
                                                        else __DailyStat = "Regular";
                                                    }
                                                }
                                            }
                                        }
                                        else __DailyStat = "Leave";
                                    }
                                    else __DailyStat = "RestDay";
                                    break;
                                case "Tuesday":
                                    _Sched = _dbRD.Reader["TueIN"].ToString();
                                    if (_dbRD.Reader["TueIN"].ToString() != "00:00:00")
                                    {
                                        if (clsTotalEmpCalc.isLeave(EmpNo, dtANS) == false)
                                        {
                                            using (Database _curDb = new Database())
                                            {
                                                _curDb.Open("select EC._DateIN as DateIN, EC._DateOUT as DateOUT, ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _dbRD.Reader["TueIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _dbRD.Reader["TueIN"].ToString() + "')),'00:00:00') as Late  from admx_hrisp.pp_empclocks EC " +
                                                                                    "where  _EmpID = " + EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')");

                                                if (_curDb.Reader.HasRows == false) __DailyStat = "Absent";
                                                else
                                                {
                                                    while (_curDb.Reader.Read())
                                                    {
                                                        if (Convert.ToInt32(_dbRD.Reader["TypeID"].ToString()) != 3) _Stat = _curDb.Reader["Late"].ToString();
                                                        else _Stat ="00:00:00";
                                                        _In = _curDb.Reader["DateIN"].ToString();
                                                        _Out = _curDb.Reader["DateOUT"].ToString();
                                                        if (_curDb.Reader["DateIN"].ToString() == "" || _curDb.Reader["DateOUT"].ToString() == "") __DailyStat = "Half Day";
                                                        else __DailyStat = "Regular";
                                                    }
                                                }
                                            }
                                        }
                                        else __DailyStat = "Leave";
                                    }
                                    else __DailyStat = "RestDay";

                                    break;
                                case "Wednesday":
                                    _Sched = _dbRD.Reader["WedIN"].ToString();
                                    if (_dbRD.Reader["WedIN"].ToString() != "00:00:00")
                                    {
                                        if (clsTotalEmpCalc.isLeave(EmpNo, dtANS) == false)
                                        {
                                            using (Database _curDb = new Database())
                                            {
                                                _curDb.Open("select EC._DateIN as DateIN, EC._DateOUT as DateOUT,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _dbRD.Reader["WedIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _dbRD.Reader["WedIN"].ToString() + "')),'00:00:00') as Late  from admx_hrisp.pp_empclocks EC " +
                                                                                    "where  _EmpID = " + EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')");
                                                if (_curDb.Reader.HasRows == false) __DailyStat = "Absent";
                                                else
                                                {
                                                    while (_curDb.Reader.Read())
                                                    {
                                                        if (Convert.ToInt32(_dbRD.Reader["TypeID"].ToString()) != 3) _Stat = _curDb.Reader["Late"].ToString();
                                                        else _Stat = "00:00:00";
                                                        _In = _curDb.Reader["DateIN"].ToString();
                                                        _Out = _curDb.Reader["DateOUT"].ToString();
                                                        if (_curDb.Reader["DateIN"].ToString() == "" || _curDb.Reader["DateOUT"].ToString() == "") __DailyStat = "Half Day";
                                                        else __DailyStat = "Regular";
                                                    }
                                                }
                                            }
                                        }
                                        else __DailyStat = "Leave";
                                    }
                                     else __DailyStat = "RestDay";
                                    break;
                                case "Thursday":
                                    _Sched = _dbRD.Reader["ThuIN"].ToString();
                                    if (_dbRD.Reader["ThuIN"].ToString() != "00:00:00")
                                    {
                                        if (clsTotalEmpCalc.isLeave(EmpNo, dtANS) == false)
                                        {
                                            using (Database _curDb = new Database())
                                            {
                                                _curDb.Open("select EC._DateIN as DateIN, EC._DateOUT as DateOUT,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _dbRD.Reader["ThuIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _dbRD.Reader["ThuIN"].ToString() + "')),'00:00:00') as Late  from admx_hrisp.pp_empclocks EC " +
                                                                                    "where  _EmpID = " + EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')");
                                                if (_curDb.Reader.HasRows == false) __DailyStat = "Absent";
                                                else
                                                {
                                                    while (_curDb.Reader.Read())
                                                    {
                                                        if (Convert.ToInt32(_dbRD.Reader["TypeID"].ToString()) != 3) _Stat = _curDb.Reader["Late"].ToString();
                                                        else _Stat ="00:00:00";
                                                        _In = _curDb.Reader["DateIN"].ToString();
                                                        _Out = _curDb.Reader["DateOUT"].ToString();
                                                        if (_curDb.Reader["DateIN"].ToString() == "" || _curDb.Reader["DateOUT"].ToString() == "") __DailyStat = "Half Day";
                                                        else __DailyStat = "Regular";
                                                    }
                                                }
                                            }
                                        }
                                        else __DailyStat = "Leave";
                                    }
                                    else __DailyStat = "RestDay";

                                    break;
                                case "Friday":
                                    _Sched = _dbRD.Reader["FriIN"].ToString();
                                    if (_dbRD.Reader["FriIN"].ToString() != "00:00:00")
                                    {

                                        if (clsTotalEmpCalc.isLeave(EmpNo, dtANS) == false)
                                        {
                                            using (Database _curDb = new Database())
                                            {
                                                _curDb.Open("select EC._DateIN as DateIN, EC._DateOUT as DateOUT,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _dbRD.Reader["FriIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _dbRD.Reader["FriIN"].ToString() + "')),'00:00:00') as Late  from admx_hrisp.pp_empclocks EC " +
                                                                                    "where  _EmpID = " + EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')");
                                                if (_curDb.Reader.HasRows == false) __DailyStat = "Absent";
                                                else
                                                {
                                                    while (_curDb.Reader.Read())
                                                    {
                                                        if (Convert.ToInt32(_dbRD.Reader["TypeID"].ToString()) != 3) _Stat = _curDb.Reader["Late"].ToString();
                                                        else _Stat ="00:00:00";
                                                        _In = _curDb.Reader["DateIN"].ToString();
                                                        _Out = _curDb.Reader["DateOUT"].ToString();
                                                        if (_curDb.Reader["DateIN"].ToString() == "" || _curDb.Reader["DateOUT"].ToString() == "") __DailyStat = "Half Day";
                                                        else __DailyStat = "Regular";
                                                    }
                                                }
                                            }
                                        }
                                        else __DailyStat = "Leave";
                                    }
                                    else __DailyStat = "RestDay";

                                    break;


                                case "Saturday":
                                    _Sched = _dbRD.Reader["SatIN"].ToString();
                                    if (_dbRD.Reader["SatIN"].ToString() != "00:00:00")
                                    {
                                        if (clsTotalEmpCalc.isLeave(EmpNo, dtANS) == false)
                                        {
                                            using (Database _curDb = new Database())
                                            {
                                                _curDb.Open("select EC._DateIN as DateIN, EC._DateOUT as DateOUT,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _dbRD.Reader["SatIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _dbRD.Reader["SatIN"].ToString() + "')),'00:00:00') as Late  from admx_hrisp.pp_empclocks EC " +
                                                                                    "where  _EmpID = " + EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')");

                                                if (_curDb.Reader.HasRows == false) __DailyStat = "Absent";
                                                else
                                                {
                                                    while (_curDb.Reader.Read())
                                                    {
                                                        if (Convert.ToInt32(_dbRD.Reader["TypeID"].ToString()) != 3) _Stat = _curDb.Reader["Late"].ToString();
                                                        else _Stat ="00:00:00";
                                                        _In = _curDb.Reader["DateIN"].ToString();
                                                        _Out = _curDb.Reader["DateOUT"].ToString();
                                                        if (_curDb.Reader["DateIN"].ToString() == "" || _curDb.Reader["DateOUT"].ToString() == "") __DailyStat = "Half Day";
                                                        else __DailyStat = "Regular";
                                                    }
                                                }
                                            }
                                        }
                                        else __DailyStat = "Leave";
                                    }
                                    else __DailyStat = "RestDay";

                                    break;
                            }
                            using (Database _holDB = new Database())
                            {
                                _holDB.Open("SELECT count(*) as Count FROM admx_hrisp.pp_holidaylist where date_format(HolidayDate,'%m/%d/%Y') = '" + String.Format("{0:MM/dd/yyyy}", dtANS) + "'");
                                while (_holDB.Reader.Read()) if (_holDB.Reader["Count"].ToString() == "1") __DailyStat = "Holiday";

                            }
              
                            clsChecking._EmpLate.Add(new clsChecking()
                            {
                                _ctr = i++,
                                _CurSchedule = _Sched,
                                _Week = string.Format("{0:dddd}", dtANS),
                                _Date = string.Format("{0:MM/dd/yyyy}", dtANS),
                                _CurDateIN= _In,
                                _CurDateOUT=_Out,
                                _Status =_Stat,
                                _DailyStat = __DailyStat
                            });
                        }
                        _dbRD.Reader.Close();

                    }
              
                }
            }
            
            

            return clsChecking._EmpLate;
        }

        //public List<clsChecking> GetRegular(string _EmpNo, DateTime PayrollStart, DateTime PayrollEnd)
        //{
        //    clsTotalEmpCalc _clsTotalEmpCalc = new clsTotalEmpCalc();
        //    clsChecking._EmpRegular.Clear();
        //    _Database.Open("SELECT  _EmpID,_DateIN,_DateOut,date_format(_DateIN,'%W') as Week FROM admx_hrisp.pp_empclocks EC where " +
        //                    "(date_format(EC._DateIN,'%m/%d/%Y') between'" + String.Format("{0:MM/dd/yyyy}", PayrollStart) + "' and '" + String.Format("{0:MM/dd/yyyy}", PayrollEnd) + "' and EC._EmpID=" + _EmpNo +  " " + _clsTotalEmpCalc.getRD(_EmpNo, _clsTotalEmpCalc.getSchedule(_EmpNo)) + " and date_format(EC._DateIN,'%m/%d%/%Y') not in (SELECT  date_format(HolidayDate,'%m/%d%/%Y') FROM admx_hrisp.pp_holidaylist) ) " +
        //                    " or " +
        //                    "(date_format(EC._DateOut,'%m/%d/%Y') between '" + String.Format("{0:MM/dd/yyyy}", PayrollStart) + "' and '" + String.Format("{0:MM/dd/yyyy}", PayrollEnd) + "' and EC._EmpID=" + _EmpNo + " " + _clsTotalEmpCalc.getRD(_EmpNo, _clsTotalEmpCalc.getSchedule(_EmpNo)) + " and date_format(EC._DateIN,'%m/%d%/%Y') not in (SELECT  date_format(HolidayDate,'%m/%d%/%Y') FROM admx_hrisp.pp_holidaylist) )");
        //    int i = 1;
        //    while (_Database.Reader.Read())
        //    {
        //        clsChecking._EmpRegular.Add(new clsChecking()
        //        {
        //            _ctr = i++,
        //            _EmpNO = Convert.ToString(_Database.Reader["_EmpID"].ToString()),
        //            _DateIN = Convert.ToString(_Database.Reader["_DateIN"].ToString()),
        //            _DateOUT = Convert.ToString(_Database.Reader["_DateOut"].ToString()),
        //            _Week = Convert.ToString(_Database.Reader["Week"].ToString())
        //        });
        //    }
        //    return clsChecking._EmpRegular;
        //}
        public List<clsChecking> GetMata(string _EmpNo, DateTime MataStart, DateTime MataEnd)
        {
            clsChecking._EmpMata.Clear();
            _Database.Open("SELECT  _EmpID,_DateIN,_DateOut,date_format(_DateIN,'%W') as Week FROM admx_hrisp.pp_empclocks where " +
                            "(date_format(_DateIN,'%m/%d/%Y') between'" + String.Format("{0:MM/dd/yyyy}", MataStart) + "' and '" + String.Format("{0:MM/dd/yyyy}", MataEnd) + "' and _EmpID=" + _EmpNo + ") " +
                            " or " +
                            "(date_format(_DateOut,'%m/%d/%Y') between '" + String.Format("{0:MM/dd/yyyy}", MataStart) + "' and '" + String.Format("{0:MM/dd/yyyy}", MataEnd) + "' and _EmpID=" + _EmpNo + ")");
            int i = 1;
            while (_Database.Reader.Read())
            {
                clsChecking._EmpMata.Add(new clsChecking()
                {
                    _ctr = i++,
                    _EmpNO = Convert.ToString(_Database.Reader["_EmpID"].ToString()),
                    _DateIN = Convert.ToString(_Database.Reader["_DateIN"].ToString()),
                    _DateOUT = Convert.ToString(_Database.Reader["_DateOut"].ToString()),
                    _Week = Convert.ToString(_Database.Reader["Week"].ToString())
                });
            }
            return clsChecking._EmpMata;
        }


        public List<clsChecking> GetHoliday(DateTime PayrollStart, DateTime PayrollEnd)
        {
            clsChecking._Holiday.Clear();
            _Database.Open("SELECT holidaydesc,date_format(holidaydate,'%m/%d/%Y') as holidaydate FROM admx_hrisp.pp_holidaylist where (date_format(HolidayDate,'%m/%d/%Y') between '" + String.Format("{0:MM/dd/yyyy}", PayrollStart) + "' and '" + String.Format("{0:MM/dd/yyyy}", PayrollEnd) + "')");
            int i = 1;
            while (_Database.Reader.Read())
            {
                clsChecking._Holiday.Add(new clsChecking()
                {
                    _ctr = i++,
                    _HolidayDesc = Convert.ToString(_Database.Reader["holidaydesc"].ToString()),
                    _HolidayDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", _Database.Reader["holidaydate"].ToString()))

                });
            }
            return clsChecking._Holiday;
        }
        public List<clsChecking> GetRegularOT(string _EmpNo,int _PayrollID)
        {
            clsChecking._OTRegularList.Clear();
            _Database.Open("select " +
                            "OT.fld_ot_date as 'OT Date', " +
                            "OT.fld_ot_start as 'OT Start' ,  " +
                            "OT.fld_ot_end as 'OT END', " +
                            "OT.fld_total as 'Total OT', " +
                            "fld_reason as 'Reason', " +
                            "SP1.fld_StaticParamDesc as 'Status' " +
                            "FROM admx_hrisp.tbl_request R  " +
                            "inner join tbl_req_ot OT  " +
                            "on R.fld_req_id = OT.fld_req_id  " +
                            "inner join admx_hrisp.tbl_staticparam  SP1 " +
                            "on SP1.fld_StaticParamID = OT.fld_status " +
                            "inner join  admx_hrisp.tbl_staticparam  SP2 " +
                            "on OT.fld_ot_type = SP2.fld_StaticParamID " +
                                "where R.fld_emp_id = " + _EmpNo + " and SP2.fld_StaticParamID = 115 and  fld_PayrollID=" + _PayrollID);
            int i = 1;
            while (_Database.Reader.Read())
            {
                clsChecking._OTRegularList.Add(new clsChecking()
                {
                    _ctr = i++,
                    _OTDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", _Database.Reader["OT Date"].ToString())),
                    _OTStart = TimeSpan.Parse(String.Format("{0:hh:mm:ss}", _Database.Reader["OT Start"].ToString())),
                    _OTEnd = TimeSpan.Parse(String.Format("{0:hh:mm:ss}", _Database.Reader["OT END"].ToString())),
                    _OTTotal = Convert.ToDouble(_Database.Reader["Total OT"].ToString()),
                    _Reason = Convert.ToString(_Database.Reader["Reason"].ToString()),
                    _Status = Convert.ToString(_Database.Reader["Status"].ToString())

                });
            }
            return clsChecking._OTRegularList;
        }
        public List<clsChecking> GetRestDayOT(string _EmpNo, int _PayrollID)
        {
            clsChecking._OTRestDayList.Clear();
            _Database.Open("SELECT " +
                             "OT.fld_ot_date as 'OT Date', " +
                            "OT.fld_ot_start as 'OT Start' ,  " +
                            "OT.fld_ot_end as 'OT END', " +
                            "OT.fld_total as 'Total OT', " +
                            "fld_reason as 'Reason', " +
                            "SP1.fld_StaticParamDesc as 'Status' " +
                            "FROM admx_hrisp.tbl_request R   " +
                            "inner join tbl_req_ot OT   " +
                            "on R.fld_req_id = OT.fld_req_id   " +
                            "inner join admx_hrisp.tbl_staticparam  SP1 " +
                            "on SP1.fld_StaticParamID = OT.fld_status " +
                            "inner join  admx_hrisp.tbl_staticparam  SP2  " +
                            "on OT.fld_ot_type = SP2.fld_StaticParamID  " +
                                "where  R.fld_emp_id = " + _EmpNo + " and SP2.fld_StaticParamID = 117  and  fld_PayrollID=" + _PayrollID);
            int i = 1;
            while (_Database.Reader.Read())
            {
                clsChecking._OTRestDayList.Add(new clsChecking()
                {
                    _ctr = i++,
                    _OTDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", _Database.Reader["OT Date"].ToString())),
                    _OTStart = TimeSpan.Parse(String.Format("{0:hh:mm:ss}", _Database.Reader["OT Start"].ToString())),
                    _OTEnd = TimeSpan.Parse(String.Format("{0:hh:mm:ss}", _Database.Reader["OT END"].ToString())),
                    _OTTotal = Convert.ToDouble(_Database.Reader["Total OT"].ToString()),
                    _Reason = Convert.ToString(_Database.Reader["Reason"].ToString()),
                    _Status = Convert.ToString(_Database.Reader["Status"].ToString())

                });
            }
            return clsChecking._OTRestDayList;
        }
        public List<clsChecking> GetSpecialHolidayOT(string _EmpNo, int _PayrollID)
        {
            clsChecking._OTSpecialHolidayList.Clear();
            _Database.Open("SELECT " +
                             "OT.fld_ot_date as 'OT Date', " +
                            "OT.fld_ot_start as 'OT Start' ,  " +
                            "OT.fld_ot_end as 'OT END', " +
                            "OT.fld_total as 'Total OT', " +
                            "fld_reason as 'Reason', " +
                            "SP1.fld_StaticParamDesc as 'Status' " +
                            "FROM admx_hrisp.tbl_request R   " +
                            "inner join tbl_req_ot OT   " +
                            "on R.fld_req_id = OT.fld_req_id   " +
                            "inner join admx_hrisp.tbl_staticparam  SP1 " +
                            "on SP1.fld_StaticParamID = OT.fld_status " +
                            "inner join  admx_hrisp.tbl_staticparam  SP2  " +
                            "on OT.fld_ot_type = SP2.fld_StaticParamID  " +
                                "where  R.fld_emp_id = " + _EmpNo + " and SP2.fld_StaticParamID = 291  and  fld_PayrollID=" + _PayrollID);
            int i = 1;
            while (_Database.Reader.Read())
            {
                clsChecking._OTSpecialHolidayList.Add(new clsChecking()
                {
                    _ctr = i++,
                    _OTDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", _Database.Reader["OT Date"].ToString())),
                    _OTStart = TimeSpan.Parse(String.Format("{0:hh:mm:ss}", _Database.Reader["OT Start"].ToString())),
                    _OTEnd = TimeSpan.Parse(String.Format("{0:hh:mm:ss}", _Database.Reader["OT END"].ToString())),
                    _OTTotal = Convert.ToDouble(_Database.Reader["Total OT"].ToString()),
                    _Reason = Convert.ToString(_Database.Reader["Reason"].ToString()),
                    _Status = Convert.ToString(_Database.Reader["Status"].ToString())

                });
            }
            return clsChecking._OTSpecialHolidayList;
        }
        public List<clsChecking> GetHolidayOT(string _EmpNo, int _PayrollID)
        {
            clsChecking._OTHolidayList.Clear();
            _Database.Open("SELECT " +
                            "OT.fld_ot_date as 'OT Date', " +
                            "OT.fld_ot_start as 'OT Start' ,  " +
                            "OT.fld_ot_end as 'OT END', " +
                            "OT.fld_total as 'Total OT', " +
                            "fld_reason as 'Reason', " +
                            "SP1.fld_StaticParamDesc as 'Status' " +
                            "FROM admx_hrisp.tbl_request R   " +
                            "inner join tbl_req_ot OT   " +
                            "on R.fld_req_id = OT.fld_req_id   " +
                            "inner join admx_hrisp.tbl_staticparam  SP1 " +
                            "on SP1.fld_StaticParamID = OT.fld_status " +
                            "inner join  admx_hrisp.tbl_staticparam  SP2  " +
                            "on OT.fld_ot_type = SP2.fld_StaticParamID  " +
                                "where  R.fld_emp_id = " + _EmpNo + " and SP2.fld_StaticParamID = 116  and  fld_PayrollID=" + _PayrollID);
            int i = 1;
            while (_Database.Reader.Read())
            {
                clsChecking._OTHolidayList.Add(new clsChecking()
                {
                    _ctr = i++,
                    _OTDate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", _Database.Reader["OT Date"].ToString())),
                    _OTStart = TimeSpan.Parse(String.Format("{0:hh:mm:ss}", _Database.Reader["OT Start"].ToString())),
                    _OTEnd = TimeSpan.Parse(String.Format("{0:hh:mm:ss}", _Database.Reader["OT END"].ToString())),
                    _OTTotal = Convert.ToDouble(_Database.Reader["Total OT"].ToString()),
                    _Reason = Convert.ToString(_Database.Reader["Reason"].ToString()),
                    _Status = Convert.ToString(_Database.Reader["Status"].ToString())

                });
            }
            return clsChecking._OTHolidayList;
        }

        public List<clsChecking> GetSched(string EmpNo, int SchedID, DateTime PayrollStart, DateTime PayrollEnd)
        {
   
            clsChecking._EmpAbsences.Clear();
            TimeSpan TotDays = PayrollEnd - PayrollStart;
            int i = 1;
            for (int x = 0; x <= TotDays.Days; x++)
            {
                _Database.Open("Select * from  admx_hrisp.pp_schedules where SchedID = " + SchedID);

                string status = "";
                string _Sched = "";
                //string _IN = "";
                //string _OUT = "";
                {
                    while (_Database.Reader.Read())
                    {

                        switch (string.Format("{0:dddd}", PayrollStart.AddDays(x)))
                        {
                            case "Sunday":
                                if (_Database.Reader["SunIN"].ToString() == "00:00:00") status = "Rest Day";
                                else 
                                    if (clsTotalEmpCalc.isLeave(EmpNo, PayrollStart.AddDays(x)) == true) status = "Leave";
                                    else {status = "Regular";} _Sched= _Database.Reader["SunIN"].ToString();

                                break;
                            case "Monday":
                                if (_Database.Reader["MonIN"].ToString() == "00:00:00") status = "Rest Day";
                                else
                                    if (clsTotalEmpCalc.isLeave(EmpNo, PayrollStart.AddDays(x)) == true) status = "Leave";
                                    else {status = "Regular";} _Sched= _Database.Reader["MonIN"].ToString();
                                break;
                            case "Tuesday":
                                if (_Database.Reader["TueIN"].ToString() == "00:00:00") status = "Rest Day";
                                else 
                                    if (clsTotalEmpCalc.isLeave(EmpNo, PayrollStart.AddDays(x)) == true) status = "Leave";
                                    else {status = "Regular";} _Sched= _Database.Reader["TueIN"].ToString();
                                break;
                            case "Wednesday":
                                if (_Database.Reader["WedIN"].ToString() == "00:00:00") status = "Rest Day";
                                else 
                                    if (clsTotalEmpCalc.isLeave(EmpNo, PayrollStart.AddDays(x)) == true) status = "Leave";
                                    else {status = "Regular";} _Sched= _Database.Reader["WedIN"].ToString();

                                break;
                            case "Thursday":
                                if (_Database.Reader["ThuIN"].ToString() == "00:00:00") status = "Rest Day";
                                else 
                                    if (clsTotalEmpCalc.isLeave(EmpNo, PayrollStart.AddDays(x)) == true) status = "Leave";
                                    else {status = "Regular";} _Sched= _Database.Reader["ThuIN"].ToString();

                                break;
                            case "Friday":
                                if (_Database.Reader["FriIN"].ToString() == "00:00:00") status = "Rest Day";
                                else   
                                    if (clsTotalEmpCalc.isLeave(EmpNo, PayrollStart.AddDays(x)) == true) status = "Leave";
                                    else {status = "Regular";} _Sched= _Database.Reader["FriIN"].ToString();
                                break;
                            case "Saturday":
                                if (_Database.Reader["SatIN"].ToString() == "00:00:00") status = "Rest Day";
                                else 
                                    if (clsTotalEmpCalc.isLeave(EmpNo, PayrollStart.AddDays(x)) == true) status = "Leave";
                                    else {status = "Regular";} _Sched= _Database.Reader["SatIN"].ToString();

                                break;

                        }
                    }
                }
                _Database.Open("SELECT holidaydesc as holidaydesc FROM admx_hrisp.pp_holidaylist where date_format(HolidayDate,'%m/%d/%Y') = '" + string.Format("{0:MM/dd/yyyy}", PayrollStart.AddDays(x)) + "'");
                {
                    while (_Database.Reader.Read()) status = _Database.Reader["holidaydesc"].ToString();
                   
                }
                clsChecking._EmpAbsences.Add(new clsChecking()
                {
                    _ctr = i++,
                    _PayrollRange = String.Format("{0:MM/dd/yyyy}", PayrollStart.AddDays(x)),
                    _Status = status,
                    _Schedule=_Sched
                    

                }); status = "";
            }

                return clsChecking._EmpAbsences;
        }
        public List<clsChecking> GetVacationLeave(string _EmpNo, DateTime PayrollStart, DateTime PayrollEnd)
        {
            TimeSpan PayrollDays = PayrollEnd - PayrollStart;
         
            clsChecking._VacationLeave.Clear();
            _Database.Open("select " +
                            "fld_emp_id as 'EMPNO', " +
                            "fld_date_filed as 'DATEFILED', " +
                            "fld_date_from as 'DATEFROM', " +
                            "fld_date_to as 'DATETO', " +
                            "fld_total_days as 'LEAVETOTAL', " +
                            "fld_reason as 'REASON', " +
                            "S2.fld_StaticParamDesc as 'STATUS'  " +
                            "from admx_hrisp.tbl_request R " +
                            "inner join admx_hrisp.tbl_req_leave L " +
	                            "on L.fld_req_id = R.fld_req_id " +
                            "inner join admx_hrisp.tbl_staticparam S1 " +
	                            "on S1.fld_StaticParamID = L.fld_leave_type " +
                            "inner join admx_hrisp.tbl_staticparam S2 " +
                                "on S2.fld_StaticParamID = L.fld_status " +
                            "where L.fld_leave_type = 54 and R.fld_emp_id =" + _EmpNo);
            int i = 1;
            while (_Database.Reader.Read())
            {
              
                for (int x = 0; x <= PayrollDays.Days; x++)
               
                {
                        if (string.Format("{0:MM/dd/yyyy}",  PayrollStart.AddDays(x)) ==  string.Format("{0:MM/dd/yyyy}",Convert.ToDateTime(_Database.Reader["DATEFROM"].ToString())))
                        {
                            clsChecking._VacationLeave.Add(new clsChecking()
                            {
                                _ctr = i++,
                                _EmpNO = Convert.ToString(_Database.Reader["EMPNO"].ToString()),
                                _DateFiled = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", _Database.Reader["DATEFILED"].ToString())),
                                _LeaveDateFrom = Convert.ToDateTime(String.Format("{0:hh:mm:ss}", _Database.Reader["DATEFROM"].ToString())),
                                _LeaveDateTo = Convert.ToDateTime(String.Format("{0:hh:mm:ss}", _Database.Reader["DATETO"].ToString())),
                                _LeaveTotal = Convert.ToDouble(_Database.Reader["LEAVETOTAL"].ToString()),
                                _Reason = Convert.ToString(_Database.Reader["REASON"].ToString()),
                                _Status = Convert.ToString(_Database.Reader["STATUS"].ToString())

                            });

                        //}
                    }
                }

              
            }
            return clsChecking._VacationLeave;
        }

        public List<clsChecking> GetSickLeave(string _EmpNo, DateTime PayrollStart, DateTime PayrollEnd)
        {
            TimeSpan PayrollDays = PayrollEnd - PayrollStart;
            clsChecking._SickLeave.Clear();
            _Database.Open("select " +
                            "fld_emp_id as 'EMPNO', " +
                            "fld_date_filed as 'DATEFILED', " +
                            "fld_date_from as 'DATEFROM', " +
                            "fld_date_to as 'DATETO', " +
                            "fld_total_days as 'LEAVETOTAL', " +
                            "fld_reason as 'REASON', " +
                            "S2.fld_StaticParamDesc as 'STATUS'  " +
                            "from admx_hrisp.tbl_request R " +
                            "inner join admx_hrisp.tbl_req_leave L " +
                                "on L.fld_req_id = R.fld_req_id " +
                            "inner join admx_hrisp.tbl_staticparam S1 " +
                                "on S1.fld_StaticParamID = L.fld_leave_type " +
                            "inner join admx_hrisp.tbl_staticparam S2 " +
                                "on S2.fld_StaticParamID = L.fld_status " +
                            "where L.fld_leave_type = 55 and R.fld_emp_id =" + _EmpNo);
            int i = 1;
            while (_Database.Reader.Read())
            {

                for (int x = 0; x <= PayrollDays.Days; x++)
                {
                    if (string.Format("{0:MM/dd/yyyy}", PayrollStart.AddDays(x)) == string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(_Database.Reader["DATEFROM"].ToString())))
                    { 
                        clsChecking._SickLeave.Add(new clsChecking()
                        {
                            _ctr = i++,
                            _EmpNO = Convert.ToString(_Database.Reader["EMPNO"].ToString()),
                            _DateFiled = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", _Database.Reader["DATEFILED"].ToString())),
                            _LeaveDateFrom = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", PayrollStart.AddDays(x))),
                            _LeaveTotal = Convert.ToDouble(_Database.Reader["LEAVETOTAL"].ToString()),
                            _Reason = Convert.ToString(_Database.Reader["REASON"].ToString()),
                            _Status = Convert.ToString(_Database.Reader["STATUS"].ToString())

                        });
                    }
                }
            }
            return clsChecking._SickLeave;

        }
        public List<clsChecking> GetEmergencyLeave(string _EmpNo, DateTime PayrollStart, DateTime PayrollEnd)
        {
            TimeSpan PayrollDays = PayrollEnd - PayrollStart;
            clsChecking._EmergencyLeave.Clear();
            _Database.Open("select " +
                            "fld_emp_id as 'EMPNO', " +
                            "fld_date_filed as 'DATEFILED', " +
                            "fld_date_from as 'DATEFROM', " +
                            "fld_date_to as 'DATETO', " +
                            "fld_total_days as 'LEAVETOTAL', " +
                            "fld_reason as 'REASON', " +
                            "S2.fld_StaticParamDesc as 'STATUS'  " +
                            "from admx_hrisp.tbl_request R " +
                            "inner join admx_hrisp.tbl_req_leave L " +
                                "on L.fld_req_id = R.fld_req_id " +
                            "inner join admx_hrisp.tbl_staticparam S1 " +
                                "on S1.fld_StaticParamID = L.fld_leave_type " +
                            "inner join admx_hrisp.tbl_staticparam S2 " +
                                "on S2.fld_StaticParamID = L.fld_status " +
                            "where L.fld_leave_type = 56 and R.fld_emp_id =" + _EmpNo);
            int i = 1;
            while (_Database.Reader.Read())
            {

                for (int x = 0; x <= PayrollDays.Days; x++)
                {
                    if (string.Format("{0:MM/dd/yyyy}", PayrollStart.AddDays(x)) == string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(_Database.Reader["DATEFROM"].ToString())))
                    {
                        clsChecking._EmergencyLeave.Add(new clsChecking()
                        {
                            _ctr = i++,
                            _EmpNO = Convert.ToString(_Database.Reader["EMPNO"].ToString()),
                            _DateFiled = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", _Database.Reader["DATEFILED"].ToString())),
                            _LeaveDateFrom = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", PayrollStart.AddDays(x))),
                            _LeaveTotal = Convert.ToDouble(_Database.Reader["LEAVETOTAL"].ToString()),
                            _Reason = Convert.ToString(_Database.Reader["REASON"].ToString()),
                            _Status = Convert.ToString(_Database.Reader["STATUS"].ToString())

                        });
                    }
                }
            }
            return clsChecking._EmergencyLeave;

        }
        public List<clsChecking> GetMaternityLeave(string _EmpNo, DateTime PayrollStart, DateTime PayrollEnd)
        {
            TimeSpan PayrollDays = PayrollEnd - PayrollStart;
            clsChecking._EmergencyLeave.Clear();
            _Database.Open("select " +
                            "fld_emp_id as 'EMPNO', " +
                            "fld_date_filed as 'DATEFILED', " +
                            "fld_date_from as 'DATEFROM', " +
                            "fld_date_to as 'DATETO', " +
                            "fld_total_days as 'LEAVETOTAL', " +
                            "fld_reason as 'REASON', " +
                            "S2.fld_StaticParamDesc as 'STATUS'  " +
                            "from admx_hrisp.tbl_request R " +
                            "inner join admx_hrisp.tbl_req_leave L " +
                                "on L.fld_req_id = R.fld_req_id " +
                            "inner join admx_hrisp.tbl_staticparam S1 " +
                                "on S1.fld_StaticParamID = L.fld_leave_type " +
                            "inner join admx_hrisp.tbl_staticparam S2 " +
                                "on S2.fld_StaticParamID = L.fld_status " +
                            "where L.fld_leave_type = 57 and R.fld_emp_id =" + _EmpNo);
            int i = 1;
            while (_Database.Reader.Read())
            {

                for (int x = 0; x <= PayrollDays.Days; x++)
                {
                    if (string.Format("{0:MM/dd/yyyy}", PayrollStart.AddDays(x)) == string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(_Database.Reader["DATEFROM"].ToString())))
                    {
                        clsChecking._EmergencyLeave.Add(new clsChecking()
                        {
                            _ctr = i++,
                            _EmpNO = Convert.ToString(_Database.Reader["EMPNO"].ToString()),
                            _DateFiled = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", _Database.Reader["DATEFILED"].ToString())),
                            _LeaveDateFrom = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", PayrollStart.AddDays(x))),
                            _LeaveTotal = Convert.ToDouble(_Database.Reader["LEAVETOTAL"].ToString()),
                            _Reason = Convert.ToString(_Database.Reader["REASON"].ToString()),
                            _Status = Convert.ToString(_Database.Reader["STATUS"].ToString())

                        });
                    }
                }
            }
            return clsChecking._MaternityLeave;

        }
        public List<clsChecking> GetPaternityLeave(string _EmpNo, DateTime PayrollStart, DateTime PayrollEnd)
        {
            TimeSpan PayrollDays = PayrollEnd - PayrollStart;
            clsChecking._EmergencyLeave.Clear();
            _Database.Open("select " +
                            "fld_emp_id as 'EMPNO', " +
                            "fld_date_filed as 'DATEFILED', " +
                            "fld_date_from as 'DATEFROM', " +
                            "fld_date_to as 'DATETO', " +
                            "fld_total_days as 'LEAVETOTAL', " +
                            "fld_reason as 'REASON', " +
                            "S2.fld_StaticParamDesc as 'STATUS'  " +
                            "from admx_hrisp.tbl_request R " +
                            "inner join admx_hrisp.tbl_req_leave L " +
                                "on L.fld_req_id = R.fld_req_id " +
                            "inner join admx_hrisp.tbl_staticparam S1 " +
                                "on S1.fld_StaticParamID = L.fld_leave_type " +
                            "inner join admx_hrisp.tbl_staticparam S2 " +
                                "on S2.fld_StaticParamID = L.fld_status " +
                            "where L.fld_leave_type = 57 and R.fld_emp_id =" + _EmpNo);
            int i = 1;
            while (_Database.Reader.Read())
            {

                for (int x = 0; x <= PayrollDays.Days; x++)
                {
                    if (string.Format("{0:MM/dd/yyyy}", PayrollStart.AddDays(x)) == string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(_Database.Reader["DATEFROM"].ToString())))
                    {
                        clsChecking._EmergencyLeave.Add(new clsChecking()
                        {
                            _ctr = i++,
                            _EmpNO = Convert.ToString(_Database.Reader["EMPNO"].ToString()),
                            _DateFiled = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", _Database.Reader["DATEFILED"].ToString())),
                            _LeaveDateFrom = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", PayrollStart.AddDays(x))),
                            _LeaveTotal = Convert.ToDouble(_Database.Reader["LEAVETOTAL"].ToString()),
                            _Reason = Convert.ToString(_Database.Reader["REASON"].ToString()),
                            _Status = Convert.ToString(_Database.Reader["STATUS"].ToString())

                        });
                    }
                }
            }
            return clsChecking._PaternityLeave;

        }
        private void dtMata_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dtSickLeave_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

       


    }
}
