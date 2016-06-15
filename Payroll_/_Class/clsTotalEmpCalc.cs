using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using AdmereX;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Threading;
namespace Payroll_
{
    public class clsTotalEmpCalc
    {

        MySqlDataReader _MataReader;
        MySqlDataReader _HolidayReader;
        MySqlDataReader _OTRegularReader;
        MySqlDataReader _OTRestdayReader;
        MySqlDataReader _OTHolidayReader;
        MySqlDataReader _Absences;
        MySqlDataReader _RestDayReader;
        MySqlDataReader _AbsentCount;
        Database _holDB = new Database();
        
        public int getMATA(string _EmpNO, DateTime MataStart, DateTime MataEnd)
        {
            int _Mata = 0;

            using (Database _dbMata = new Database())
            {
                MySqlCommand command = new MySqlCommand("SELECT  count(*) as 'Mata Count' FROM admx_hrisp.pp_empclocks where " +
                            "(date_format(_DateIN,'%m/%d/%Y') between '" + String.Format("{0:MM/dd/yyyy}", MataStart) + "' and  '" + String.Format("{0:MM/dd/yyyy}", MataEnd) + "' and _EmpID=" + _EmpNO + ") or " +
                            "(date_format(_DateOut,'%m/%d/%Y') between '" + String.Format("{0:MM/dd/yyyy}", MataStart) + "' and  '" + String.Format("{0:MM/dd/yyyy}", MataEnd) + "' and _EmpID=" + _EmpNO + ")", _dbMata.Connection);
                _MataReader = command.ExecuteReader();
                {
                    while (_MataReader.Read())
                    {
                        _Mata = Convert.ToInt32(_MataReader["Mata Count"].ToString());

                    } _MataReader.Close();
                    return _Mata;
                }
            }


        }
        public int getHoliday(DateTime PayrollStart, DateTime PayrollEnd)
        {
            int _Holiday = 0;
            using (Database _dbHoliday = new Database())
            {
                MySqlCommand command = new MySqlCommand("SELECT count(*) as 'Holiday Count' FROM admx_hrisp.pp_holidaylist where  DATE_FORMAT(HolidayDate,'%m/%d/%Y')  between   '" + String.Format("{0:MM/dd/yyyy}", PayrollStart) + "' and '" + String.Format("{0:MM/dd/yyyy}", PayrollEnd) + "'", _dbHoliday.Connection);
                _HolidayReader = command.ExecuteReader();
                {
                 
                    while (_HolidayReader.Read())
                    {
                        _Holiday = Convert.ToInt32(_HolidayReader["Holiday Count"].ToString());
                    }
                } _HolidayReader.Close();

                return _Holiday;

            }
        }
        public decimal getRegularOT(string _EmpNo,int _PayrollID)
        {
            Decimal _RegOT=0;
         
           
            using (Database _dbRegularOT = new Database())
            {
                MySqlCommand command = new MySqlCommand("select  ifnull( sum(fld_total),0)  as 'Regular OT' FROM admx_hrisp.tbl_request R " +
                                                        "inner join tbl_req_ot OT  " +
                                                        "on R.fld_req_id = OT.fld_req_id    " +
                                                        "inner join admx_hrisp.tbl_staticparam  SP1  " +
                                                        "on SP1.fld_StaticParamID = OT.fld_status  " +
                                                        "inner join  admx_hrisp.tbl_staticparam  SP2   " +
                                                        "on OT.fld_ot_type = SP2.fld_StaticParamID   " +
                                                        "where fld_emp_id = " + _EmpNo + " " +
                                                        "and OT.fld_ot_type = '115' and OT.fld_Status = 118 and fld_PayrollID=" + _PayrollID, _dbRegularOT.Connection);
                _OTRegularReader = command.ExecuteReader();
                {

                    while (_OTRegularReader.Read())
                    {
                        _RegOT = Convert.ToDecimal(_OTRegularReader["Regular OT"].ToString());
                    }
                } _OTRegularReader.Close();

                return _RegOT;
            }
        }
        public decimal getRestDayOT(string _EmpNo, int _PayrollID)
        {
            decimal _RestdayOT = 0;
            using (Database _dbRestdayOT = new Database())
            {
                MySqlCommand command = new MySqlCommand("select  ifnull( sum(fld_total),0)  as 'Holiday OT' FROM admx_hrisp.tbl_request R " +
                                                        "inner join tbl_req_ot OT  " +
                                                        "on R.fld_req_id = OT.fld_req_id    " +
                                                        "inner join admx_hrisp.tbl_staticparam  SP1  " +
                                                        "on SP1.fld_StaticParamID = OT.fld_status  " +
                                                        "inner join  admx_hrisp.tbl_staticparam  SP2   " +
                                                        "on OT.fld_ot_type = SP2.fld_StaticParamID   " +
                                                        "where fld_emp_id = " + _EmpNo + " " +
                                                        "and OT.fld_ot_type = '117' and OT.fld_Status = 118 and fld_PayrollID=" + _PayrollID, _dbRestdayOT.Connection);
                _OTRestdayReader = command.ExecuteReader();
                {

                    while (_OTRestdayReader.Read())
                    {
                        _RestdayOT = Convert.ToDecimal(_OTRestdayReader["Holiday OT"].ToString());
                    }
                } _OTRestdayReader.Close();
                return _RestdayOT;
            }
        }
        public decimal getHolidayOT(string _EmpNo, int _PayrollID)
        {
            decimal _HolidayOT = 0;
            using (Database _dbHolidayOT = new Database())
            {
                MySqlCommand command = new MySqlCommand("select  ifnull( sum(fld_total),0)  as 'Holiday OT' FROM admx_hrisp.tbl_request R " +
                                                        "inner join tbl_req_ot OT  " +
                                                        "on R.fld_req_id = OT.fld_req_id    " +
                                                        "inner join admx_hrisp.tbl_staticparam  SP1  " +
                                                        "on SP1.fld_StaticParamID = OT.fld_status  " +
                                                        "inner join  admx_hrisp.tbl_staticparam  SP2   " +
                                                        "on OT.fld_ot_type = SP2.fld_StaticParamID   " +
                                                        "where fld_emp_id = " + _EmpNo + " " +
                                                        "and OT.fld_ot_type = '116' and OT.fld_Status = 118 and fld_PayrollID=" + _PayrollID, _dbHolidayOT.Connection);
                _OTHolidayReader = command.ExecuteReader();
                {

                    while (_OTHolidayReader.Read())
                    {
                        _HolidayOT = Convert.ToDecimal(_OTHolidayReader["Holiday OT"].ToString());
                    }
                } _OTHolidayReader.Close();
                return _HolidayOT;
              }
        }
        public decimal getSpecialHolidayOT(string _EmpNo, int _PayrollID)
        {
            decimal _SpecialHolidayOT = 0;
            using (Database _dbHolidayOT = new Database())
            {
                MySqlCommand command = new MySqlCommand("select  ifnull( sum(fld_total),0)  as 'Special Holiday OT' FROM admx_hrisp.tbl_request R " +
                                                        "inner join tbl_req_ot OT  " +
                                                        "on R.fld_req_id = OT.fld_req_id    " +
                                                        "inner join admx_hrisp.tbl_staticparam  SP1  " +
                                                        "on SP1.fld_StaticParamID = OT.fld_status  " +
                                                        "inner join  admx_hrisp.tbl_staticparam  SP2   " +
                                                        "on OT.fld_ot_type = SP2.fld_StaticParamID   " +
                                                        "where fld_emp_id = " + _EmpNo + " " +
                                                        "and OT.fld_ot_type = '291' and OT.fld_Status = 118 and fld_PayrollID=" + _PayrollID, _dbHolidayOT.Connection);
                _OTHolidayReader = command.ExecuteReader();
                {

                    while (_OTHolidayReader.Read())
                    {
                        _SpecialHolidayOT = Convert.ToDecimal(_OTHolidayReader["Special Holiday OT"].ToString());
                    }
                } _OTHolidayReader.Close();
                return _SpecialHolidayOT;
            }
        }
        public int getSchedule(String _EmpNo,String _EffectiveDate)
        {  
            try
            {
                int _SchedID = 0;
                using (Database _dbSched = new Database())
                {
                    MySqlCommand command = new MySqlCommand("select admx_hrisp.NewSchedType(" + _EmpNo + ",'" + _EffectiveDate + "')", _dbSched.Connection);
                    return _SchedID =  Convert.ToInt32(command.ExecuteScalar());
        
                }
            }
            catch (Exception ex)
            {
                return 2;
            }
        }
       
        public  Tuple<Int32,Double, Double,Double,String> get_Regular_Absences_Late(string _EmpNo,  DateTime PayrollStart, DateTime PayrollEnd)
        {
            MySqlCommand command;
            Int32 Mata = 0;
            double Regular =0;
            double _TotAbsences = 0;
            string _LWOP = "";
            TimeSpan A = PayrollEnd - PayrollStart;
            TimeSpan _TotalLate = TimeSpan.Zero;

        
            using (Database _dbRD = new Database())
            {
               
                for (int _absences = 0; _absences <= A.Days; _absences++)
                {
                    
                    DateTime dtANS = PayrollStart.AddDays(_absences);


                    using (Database _HolidayDB = new Database())
                    {
                        using (MySqlCommand _MySql = new MySqlCommand("SELECT count(*) as Count FROM admx_hrisp.pp_holidaylist where date_format(HolidayDate,'%m/%d/%Y') = '" + String.Format("{0:MM/dd/yyyy}", dtANS) + "'", _HolidayDB.Connection))
                        {

                            if (Convert.ToInt32(_MySql.ExecuteScalar()) == 1) goto JUMP;
                        }
                    }


                using(command = new MySqlCommand("Select * from  admx_hrisp.pp_schedules where SchedID = " + getSchedule(_EmpNo, string.Format("{0:yyyy-MM-dd}", dtANS)), _dbRD.Connection))
               
                {
                    _Absences = command.ExecuteReader();
                    {
                       
                        while (_Absences.Read())
                        {
                                TimeSpan late = TimeSpan.Zero; 
                               
                                switch (string.Format("{0:dddd}", dtANS))
                                {
                                    case "Sunday":
                                      
                                            if (isLeave(_EmpNo, dtANS) == false)
                                            {

                                                using (Database _curDb = new Database())
                                                {
                                                    MySqlCommand _CtrAbsence = new MySqlCommand("select count(*) as Count,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _Absences["SunIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _Absences["SunIN"].ToString() + "')),'00:00:00') as Late,EC._DateIN,EC._DateOUT  from admx_hrisp.pp_empclocks EC " +
                                                                                            "where  _EmpID = " + _EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')", _curDb.Connection);
                                                    _AbsentCount = _CtrAbsence.ExecuteReader();
                                                    while (_AbsentCount.Read())
                                                    {
                                                        if (_Absences["SunIN"].ToString() == "00:00:00" && _AbsentCount["_DateIN"].ToString() != "" || _AbsentCount["_DateOUT"].ToString() != "")
                                                        {
                                                            Mata++;
                                                        }

                                                        else if (_AbsentCount["Count"].ToString() == "0")
                                                        {
                                                            _TotAbsences = _TotAbsences + 1;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Absent~" + _LWOP;
                                                        }

                                                        else if (_AbsentCount["_DateIN"].ToString() == "" || _AbsentCount["_DateOUT"].ToString() == "")
                                                        {
                                                            Mata++;
                                                            _TotAbsences = _TotAbsences + .5;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Halfday~" + _LWOP;
                                                        }
                                                        else if (Convert.ToInt32(_Absences["TypeID"].ToString()) != 3)
                                                        {
                                                            Mata++;
                                                            late = TimeSpan.Parse(_AbsentCount["Late"].ToString());
                                                        }
                                                        else Mata++;

                                                    } _AbsentCount.Close();
                                                }
                                            }
                                            else _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Leave" + _LWOP;
                                        
                                                    
                                        break;
                                    case "Monday":
                                        Regular++;
                                        
                                            if (isLeave(_EmpNo, dtANS) == false)
                                            {
                                                using (Database _curDb = new Database())
                                                {
                                                    MySqlCommand _CtrAbsence = new MySqlCommand("select count(*) as Count,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _Absences["MonIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _Absences["MonIN"].ToString() + "')),'00:00:00') as Late,EC._DateIN,EC._DateOUT from admx_hrisp.pp_empclocks EC " +
                                                                                                "where  _EmpID = " + _EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')", _curDb.Connection);
                                                    _AbsentCount = _CtrAbsence.ExecuteReader();

                                                    while (_AbsentCount.Read())
                                                    {
                                                        if (_Absences["MonIN"].ToString() == "00:00:00" && _AbsentCount["_DateIN"].ToString() != "" || _AbsentCount["_DateOUT"].ToString() != "")
                                                        {
                                                            Mata++;
                                                        }

                                                        else if (_AbsentCount["Count"].ToString() == "0")
                                                        {
                                                            _TotAbsences = _TotAbsences + 1;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Absent~" + _LWOP;
                                                        }
                                                        else if (_AbsentCount["_DateIN"].ToString() == "" || _AbsentCount["_DateOUT"].ToString() == "")
                                                        {
                                                            Mata++;
                                                            _TotAbsences = _TotAbsences + .5;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Halfday~" + _LWOP;
                                                        }
                                                        else if (Convert.ToInt32(_Absences["TypeID"].ToString()) != 3)
                                                        {
                                                            Mata++;
                                                            late = TimeSpan.Parse(_AbsentCount["Late"].ToString());
                                                        }
                                                        else Mata++;
                                                    } _AbsentCount.Close();
                                                }
                                            }
                                            else _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Leave" + _LWOP;
                                        
                                     
                                        break;
                                    case "Tuesday":
                                        Regular++;
                                      
                                            if (isLeave(_EmpNo, dtANS) == false)
                                            {
                                                using (Database _curDb = new Database())
                                                {
                                                    MySqlCommand _CtrAbsence = new MySqlCommand("select count(*) as Count,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _Absences["TueIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _Absences["TueIN"].ToString() + "')),'00:00:00') as Late,EC._DateIN,EC._DateOUT  from admx_hrisp.pp_empclocks EC " +
                                                                                                "where  _EmpID = " + _EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')", _curDb.Connection);
                                                    _AbsentCount = _CtrAbsence.ExecuteReader();

                                                    while (_AbsentCount.Read())
                                                    {
                                                        if (_Absences["TueIN"].ToString() == "00:00:00" && _AbsentCount["_DateIN"].ToString() != "" || _AbsentCount["_DateOUT"].ToString() != "")
                                                        {
                                                            Mata++;
                                                        }

                                                        else if (_AbsentCount["Count"].ToString() == "0" && Convert.ToInt32(_Absences["TypeID"].ToString()) != 3)
                                                        {
                                                            _TotAbsences = _TotAbsences + 1;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Absent~" + _LWOP;
                                                        }
                                                        else if (_AbsentCount["_DateIN"].ToString() == "" || _AbsentCount["_DateOUT"].ToString() == "")
                                                        {
                                                            Mata++;
                                                            _TotAbsences = _TotAbsences + .5;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Halfday~" + _LWOP;
                                                        }
                                                        else if (Convert.ToInt32(_Absences["TypeID"].ToString()) != 3)
                                                        {
                                                            Mata++;
                                                            late = TimeSpan.Parse(_AbsentCount["Late"].ToString());
                                                        }
                                                        else Mata++;

                                                    } _AbsentCount.Close();
                                                }
                                            }
                                        
                                                    
                                        break;
                                    case "Wednesday":
                                        Regular++;
                                       
                                            if (isLeave(_EmpNo, dtANS) == false)
                                            {
                                                using (Database _curDb = new Database())
                                                {
                                                    MySqlCommand _CtrAbsence = new MySqlCommand("select count(*) as Count,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _Absences["WedIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _Absences["WedIN"].ToString() + "')),'00:00:00') as Late,EC._DateIN,EC._DateOUT      from admx_hrisp.pp_empclocks EC " +
                                                                                                "where  _EmpID = " + _EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')", _curDb.Connection);
                                                    _AbsentCount = _CtrAbsence.ExecuteReader();

                                                    while (_AbsentCount.Read())
                                                    {
                                                        if (_Absences["WedIN"].ToString() == "00:00:00" && _AbsentCount["_DateIN"].ToString() != "" || _AbsentCount["_DateOUT"].ToString() != "")
                                                        {
                                                            Mata++;
                                                        }

                                                        else if (_AbsentCount["Count"].ToString() == "0")
                                                        {
                                                            _TotAbsences = _TotAbsences + 1;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Absent~" + _LWOP;
                                                        }
                                                        else if (_AbsentCount["_DateIN"].ToString() == "" || _AbsentCount["_DateOUT"].ToString() == "")
                                                        {
                                                            Mata++;
                                                            _TotAbsences = _TotAbsences + .5;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Halfday~" + _LWOP;
                                                        }
                                                        else if (Convert.ToInt32(_Absences["TypeID"].ToString()) != 3)
                                                        {
                                                            Mata++;
                                                            late = TimeSpan.Parse(_AbsentCount["Late"].ToString());
                                                        }
                                                        else Mata++;
                                                    } _AbsentCount.Close();
                                                }
                                            }
                                            else _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Leave" + _LWOP;
                                        
                                  
                                        break;
                                    case "Thursday":
                                        Regular++;
                                           if (isLeave(_EmpNo, dtANS) == false)
                                            {
                                                using (Database _curDb = new Database())
                                                {
                                                    MySqlCommand _CtrAbsence = new MySqlCommand("select count(*) as Count,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _Absences["ThuIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _Absences["ThuIN"].ToString() + "')),'00:00:00') as Late,EC._DateIN,EC._DateOUT  from admx_hrisp.pp_empclocks EC " +
                                                                                                "where  _EmpID = " + _EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')", _curDb.Connection);
                                                    _AbsentCount = _CtrAbsence.ExecuteReader();

                                                    while (_AbsentCount.Read())
                                                    {
                                                        if (_Absences["ThuIN"].ToString() == "00:00:00" && _AbsentCount["_DateIN"].ToString() != "" || _AbsentCount["_DateOUT"].ToString() != "")
                                                        {
                                                            Mata++;
                                                        }

                                                        else if (_AbsentCount["Count"].ToString() == "0")
                                                        {
                                                            _TotAbsences = _TotAbsences + 1;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Absent~" + _LWOP;
                                                        }
                                                        else if (_AbsentCount["_DateIN"].ToString() == "" || _AbsentCount["_DateOUT"].ToString() == "")
                                                        {
                                                            Mata++;
                                                            _TotAbsences = _TotAbsences + .5;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Halfday~" + _LWOP;
                                                        }
                                                        else if (Convert.ToInt32(_Absences["TypeID"].ToString()) != 3)
                                                        {
                                                            Mata++;
                                                            late = TimeSpan.Parse(_AbsentCount["Late"].ToString());
                                                        }
                                                        else Mata++;
                                                    } _AbsentCount.Close();
                                                }
                                            }
                                            else _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Leave" + _LWOP;
                                        
                                      
                                        break;
                                    case "Friday":
                                        Regular++;
                                       

                                            if (isLeave(_EmpNo, dtANS) == false)
                                            {
                                                using (Database _curDb = new Database())
                                                {
                                                    MySqlCommand _CtrAbsence = new MySqlCommand("select count(*) as Count,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _Absences["FriIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _Absences["FriIN"].ToString() + "')),'00:00:00') as Late,EC._DateIN,EC._DateOUT  from admx_hrisp.pp_empclocks EC " +
                                                                                                "where  _EmpID = " + _EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')", _curDb.Connection);
                                                    _AbsentCount = _CtrAbsence.ExecuteReader();

                                                    while (_AbsentCount.Read())
                                                    {
                                                        if (_Absences["FriIN"].ToString() == "00:00:00" && _AbsentCount["_DateIN"].ToString() != "" || _AbsentCount["_DateOUT"].ToString() != "")
                                                        {
                                                            Mata++;
                                                        }

                                                        else if (_AbsentCount["Count"].ToString() == "0")
                                                        {
                                                            _TotAbsences = _TotAbsences + 1;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Absent~" + _LWOP;
                                                        }
                                                        else if (_AbsentCount["_DateIN"].ToString() == "" || _AbsentCount["_DateOUT"].ToString() == "")
                                                        {
                                                            Mata++;
                                                            _TotAbsences = _TotAbsences + .5;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Halfday~" + _LWOP;
                                                        }
                                                        else if (Convert.ToInt32(_Absences["TypeID"].ToString()) != 3)
                                                        {
                                                            Mata++;
                                                            late = TimeSpan.Parse(_AbsentCount["Late"].ToString());
                                                        }
                                                        else Mata++;
                                                    } _AbsentCount.Close();
                                                }
                                            }
                                            else _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Leave" + _LWOP;
                                        
                                  
                                        break;


                                    case "Saturday":
                                        
                                            if (isLeave(_EmpNo, dtANS) == false)
                                            {
                                                using (Database _curDb = new Database())
                                                {
                                                    MySqlCommand _CtrAbsence = new MySqlCommand("select count(*) as Count,ifnull(if( date_format(EC._DateIN, '%H:%i:%s') <'" + _Absences["FriIN"].ToString() + "','00:00:00'  ,TIMEDIFF(date_format( EC._DateIN, '%H:%i:%s') ,'" + _Absences["FriIN"].ToString() + "')),'00:00:00') as Late,EC._DateIN,EC._DateOUT   from admx_hrisp.pp_empclocks EC " +
                                                                                                "where  _EmpID = " + _EmpNo + " and (Date_format( EC._DateIN,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "' or Date_format( EC._DateOUT,'%Y-%m-%d') ='" + string.Format("{0:yyyy-MM-dd}", dtANS) + "')", _curDb.Connection);
                                                    _AbsentCount = _CtrAbsence.ExecuteReader();

                                                    while (_AbsentCount.Read())
                                                    {
                                                        if (_Absences["SatIN"].ToString() == "00:00:00" && _AbsentCount["_DateIN"].ToString() != "" || _AbsentCount["_DateOUT"].ToString() != "")
                                                        {
                                                            Mata++;
                                                        }

                                                        else if (_AbsentCount["Count"].ToString() == "0")
                                                        {
                                                            _TotAbsences = _TotAbsences + 1;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Absent~" + _LWOP;
                                                        }
                                                        else if (_AbsentCount["_DateIN"].ToString() == "" || _AbsentCount["_DateOUT"].ToString() == "")
                                                        {
                                                            Mata++;
                                                            _TotAbsences = _TotAbsences + .5;
                                                            _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Halfday~" + _LWOP;
                                                        }
                                                        else if (Convert.ToInt32(_Absences["TypeID"].ToString()) != 3)
                                                        {
                                                            Mata++;
                                                            late = TimeSpan.Parse(_AbsentCount["Late"].ToString());
                                                        }
                                                        else Mata++;

                                                    } _AbsentCount.Close();
                                                }
                                            }
                                            else _LWOP = string.Format("{0:MM/dd/yyyy}", dtANS) + " : Leave" + _LWOP;
                                        
                                        
                                        break;
                                }

                                _TotalLate = _TotalLate.Add(late);
                             
                            }        
                         _Absences.Close();
                      }
                    } JUMP:;
                
                }
            }

         
            return Tuple.Create(Mata,Regular, _TotAbsences, Convert.ToDouble(string.Format("{0:0.00}", Convert.ToDouble(((_TotalLate.Hours * 60) + (_TotalLate.Minutes))) / 60)), "'" + (_LWOP.Length == 0 ? "" :_LWOP.PadRight(_LWOP.Length - 1).Substring(0, _LWOP.Length - 1).Trim() ) + "'");
           
        }
        public static Boolean isLeave(string _EmpID,DateTime DateLeave)
        {
            MySqlCommand leave;
            Boolean Ans=false;
                
                for (int i = 54; i <= 58; i++)
                {
                    if (i == 57) goto Jump;
                    using (Database _Leave = new Database())
                    {


                        using (leave = new MySqlCommand("select admx_hrisp.isLeave(" + _EmpID + ",'" + string.Format("{0:yyyy-MM-dd}", DateLeave) + "'," + i + ")", _Leave.Connection))
                        {
                            if (Convert.ToBoolean(leave.ExecuteScalar()) == true) return true;
                        }

                    }
                    Jump: ;
                   
           
                }
                return Ans;
        }
       
        public Tuple<Double, Double> getLeaves(string _EmpNO)
        {
            double _VL=0;
            double _SL=0;
            using (Database dbLeave = new Database())
            {
                dbLeave.Open("SELECT fld_VL as VL, fld_SL as SL FROM admx_hrisp.tbl_leavecreditsmgt where fld_IDNumber =" + _EmpNO + " order by dt_stamp desc limit 1");
                while (dbLeave.Reader.Read())
                {
                    _VL = Convert.ToDouble(dbLeave.Reader["VL"].ToString());
                    _SL = Convert.ToDouble(dbLeave.Reader["SL"].ToString());
                };
                return Tuple.Create(_VL, _SL);
            }
        }
        
      
    }
}
