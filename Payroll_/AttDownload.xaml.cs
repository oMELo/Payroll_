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
using System.Data.SqlClient;

namespace Payroll_
{
    /// <summary>
    /// Interaction logic for AttDownload.xaml
    /// </summary>
    public partial class AttDownload 
    {
       
        int SelSiteID = 0;
        public AttDownload()
        {
            InitializeComponent();
            dtDate.SelectedDate = DateTime.Now;
            //  String.Format("{0:MM/dd/yyyy}", dtDate.SelectedDate);
            dtSite.ItemsSource = getList();
            string up = "UPDATE HRISP " +
                                                           "SET    HRISP._DateOut = C.CHECKTIME  " +
                                                           "FROM   OPENQUERY(" + Properties.Settings.Default.ODBC + ", " +
                                                                  "'select * from  admx_hrisp.pp_EmpClocks where  " +
                                                                                "_DateOut is null and " +
                                                                                "DATE_FORMAT(_DateIN,\"%m/%d/%Y\") = \"" + String.Format("{0:MM/dd/yyyy}", dtDate.SelectedDate) + "\"') HRISP  " +
                                                                   "INNER JOIN ATT_db.dbo.USERINFO U " +
                                                                                   "on HRISP._EmpID = U.BADGENUMBER " +
                                                                   "INNER JOIN (select  *, RANK()over(partition by UserID order by CHECKTIME desc) as r  FROM ATT_db.[dbo].[CHECKINOUT]  " +
                                                                               "where (convert(varchar(10),CHECKTIME,101) ='" + String.Format("{0:MM/dd/yyyy}", dtDate.SelectedDate) + "' and CHECKTYPE in('O'))) C " +
                                                                                   "on C.UserID = U.UserID " +
                                                                                       "where   " +
                                                                                       "C.r =1 ";
            string samp = ";with att as ( " +
                            "select  " +
                            "distinct convert(varchar(10),C.CHECKTIME,101) as cur " +
                            ",U.BADGENUMBER AS EmpID " +
                            ", convert(varchar(10),CHECKTIME,101) + ' ' + (select top 1  convert(varchar(10),CHECKTIME,108)  " +
                                                "FROM ATT_db.[dbo].[CHECKINOUT]  " +
                                                    "where   " +
                                                    "convert(varchar(10),CHECKTIME,101) = convert(varchar(10),C.CHECKTIME,101)  " +
                                                    "and CHECKTYPE = Upper('I') COLLATE Latin1_General_CS_AI " +
                                                    "and UserID = C.UserID order by CHECKTIME) as '_DateIN' " +
                            ",convert(varchar(10),CHECKTIME,101) + ' ' +(select top 1   convert(varchar(10),CHECKTIME,108)  " +
                                                "FROM ATT_db.[dbo].[CHECKINOUT]  " +
                                                    "where   " +
                                                    "convert(varchar(10),CHECKTIME,101) = convert(varchar(10),C.CHECKTIME,101)  " +
                                                    "and CHECKTYPE = Upper('O') COLLATE Latin1_General_CS_AI " +
                                                    "and UserID = C.UserID order by CHECKTIME)  as '_DateOUT' " +
                            "FROM ATT_db.dbo.[CHECKINOUT] C  " +
                            "inner join ATT_db.dbo.[USERINFO] U " +
                            "on C.UserID = U.UserID " +
                            "where convert(varchar(10),C.CHECKTIME,101) = '" + String.Format("{0:MM/dd/yyyy}", dtDate.SelectedDate) + "' " +
                            "and U.BADGENUMBER not in ( " +
                            "select _EmpID from OPENQUERY (" + Properties.Settings.Default.ODBC + ",  " +
                            "'SELECT *  FROM admx_hrisp.pp_empclocks where  " +
                            "(date_format(_DateIn,\"%Y-%m-%d\") =\"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\") " +
                            "or  " +
                            "(date_format(_DateOut,\"%Y-%m-%d\") = \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\" ) " +
                            "or  " +
                            "(date_format(_DateIn,\"%Y-%m-%d\") = \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate.Value.AddDays(-1)) + "\" ) " +
                            "or  " +
                            "(date_format(_DateOut,\"%Y-%m-%d\") = \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate.Value.AddDays(-1)) + "\" )')) " +
                            ") " +
                            "INSERT OPENQUERY (" + Properties.Settings.Default.ODBC + ", 'select _EmpID,_DateIN,_DateOUT from admx_hrisp.pp_empclocks')  " +
                            "select  " +
                            "att.EmpID " +
                            ",cast(att._DateIN as datetime) as _DateIN " +
                            ",cast(att._DateOUT as datetime) as _DateOUT " +
                            "from att ";

        }


        private List <clsDATA> getList()
        { 
            clsDATA _ClsData = new clsDATA();
            using (Database _Database = new Database())
            {
                _Database.Open("select fld_StaticParamID as ID , fld_StaticParamDesc as Description from admx_hrisp.tbl_staticparam where fld_CategoryID = 9 and isactive = true limit 2");
                    while ( _Database.Reader.Read())
                       {
                        clsDATA._Site.Add(new clsDATA()
                        {
                            _ID = Convert.ToInt32(_Database.Reader["ID"].ToString()),
                            _Description = Convert.ToString(_Database.Reader["Description"].ToString())
                        });
                       }
                    return clsDATA._Site;
            }
        }

        private void btDOWNLOAD_Click(object sender, RoutedEventArgs e)
        {
            try     
            {
                int i = 0;
                object item = dtSite.SelectedItems[i];
                string strConn = "";
                switch (Convert.ToInt32((dtSite.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text))
                {
                    case 36:            //          MAKATI KINAMOT          //
                        strConn = "server=" + Functions.Cryptor("NoneThought", Properties.Settings.Default.Makati, true) + ";database=ATT_db;user id=sysdev;password=Admx5299";
                        break;
                    case 37:            //          MANDALUYONG LABAS       //
                        strConn = "server=" + Functions.Cryptor("NoneThought",Properties.Settings.Default.Mandaluyong,true) + ";database=ATT_db;user id=sysdev;password=Admx5299";


                        break;

                }
                using (SqlConnection _SqlConn = new SqlConnection(strConn))
                {
                    _SqlConn.Open();
                    using (SqlCommand _SqlInsert = new SqlCommand(";with att as ( " +
                                                                        "select  " +
                                                                        "distinct convert(varchar(10),(CHECKTIME ),101) as cur " +
                                                                        ",U.BADGENUMBER AS EmpID " +
                                                                        ", convert(varchar(10),(CHECKTIME ),101) + ' ' + (select top 1  convert(varchar(10),(CHECKTIME-HRISP.TypeID),108)  " +
                                                                                        "FROM ATT_db.[dbo].[CHECKINOUT] " +
                                                                                            "where " +
                                                                                            "convert(varchar(10),(CHECKTIME-HRISP.TypeID ),101) = convert(varchar(10),(C.CHECKTIME-HRISP.TypeID ),101) " +
                                                                                            "and CHECKTYPE = Upper('I') COLLATE Latin1_General_CS_AI " +
                                                                                            "and UserID = C.UserID) as '_DateIN' " +
                                                                        ",convert(varchar(10),(CHECKTIME+HRISP.TypeID),101) + ' ' +(select top 1   convert(varchar(10),(CHECKTIME),108) " +
                                                                                        "FROM ATT_db.[dbo].[CHECKINOUT] " +
                                                                                            "where " +
                                                                                            "convert(varchar(10),(CHECKTIME),101) = convert(varchar(10),(C.CHECKTIME+HRISP.TypeID),101) " +
                                                                                            "and CHECKTYPE = Upper('O') COLLATE Latin1_General_CS_AI " +
                                                                                            "and UserID = C.UserID order by CHECKTIME)  as '_DateOUT' " +
                                                                        ",HRISP.TypeID " +
                                                                        "FROM ATT_db.dbo.[CHECKINOUT] C " +
                                                                        "inner join ATT_db.dbo.[USERINFO] U " +
                                                                        "on C.UserID = U.UserID " +
                                                                        "inner join ( " +
                                                                        "select * from OPENQUERY (ADMX,  " +
                                                                        "'select EmpNO,Effectivity,S.SchedID, " +
                                                                        "(case TypeID " +
                                                                        "when 2 then 1 " +
                                                                        "else 0 " +
                                                                        "end " +
                                                                        ") as TypeID " +
                                                                        "from (select distinct(EmpNO) , Effectivity,Schedid from admx_hrisp.pp_empschedules  " +
                                                                        "where Effectivity <= \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\"  order by effectivity desc) E  " +
                                                                        "Inner join admx_hrisp.pp_schedules S  " +
                                                                        "on E.Schedid= S.Schedid  " +
                                                                        "where Effectivity " +
                                                                        "group by E.EmpNo') " +
                                                                        ") as HRISP " +
                                                                        "on HRISP.EmpNO = U.BADGENUMBER " +
                                                                        "where convert(varchar(10),C.CHECKTIME,101)    =  '" + String.Format("{0:MM/dd/yyyy}", dtDate.SelectedDate) + "' " +
                                                                        "and U.BADGENUMBER not in ( " +
                                                                        "select _EmpID from OPENQUERY (" + Properties.Settings.Default.ODBC + ",  " +
                                                                        "'SELECT *  FROM admx_hrisp.pp_empclocks where  " +
                                                                        "(date_format(_DateIn,\"%Y-%m-%d\") =\"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\"  and  _DateOut is null) " +
                                                                        "or  " +
                                                                        "(date_format(_DateOut,\"%Y-%m-%d\") = \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\"  and  _DateIn is null ) " +
                                                                        "or  " +
                                                                        "(date_format(_DateIn,\"%Y-%m-%d\") = \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\"  and  date_format(_DateOut,\"%Y-%m-%d\") = \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\") " +
                                                                        "or  " +
                                                                        "(date_format(_Datein,\"%Y-%m-%d\") =\"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\"  and  _DateOut is not null)')) " +
                                                                        ") " +
                                                                        "INSERT OPENQUERY (" + Properties.Settings.Default.ODBC + ", 'select _EmpID,_DateIN,_DateOUT from admx_hrisp.pp_empclocks')  " +
                                                                        "select  " +
                                                                        "att.EmpID " +
                                                                        ",cast(att._DateIN as datetime) as _DateIN " +
                                                                        ",cast(att._DateOUT as datetime) as _DateOUT " +
                                                                        "from att " +
                                                                        "where   _DateIN is not null or _DateOUT is not null", _SqlConn)) _SqlInsert.ExecuteNonQuery();

                    using (SqlCommand _SqlUpdateDayShift = new SqlCommand("UPDATE HRISP " +
                                                                            "SET HRISP._DateOut = C.CHECKTIME " +
                                                                            "FROM   OPENQUERY(" + Properties.Settings.Default.ODBC + ", " +
                                                                                   "'select * from  admx_hrisp.pp_EmpClocks where  " +
                                                                                                 "_DateOut is null and " +
                                                                                                 "DATE_FORMAT(_DateIN,\"%Y-%m-%d\") =\"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\"') HRISP  " +
                                                                                    "INNER JOIN ATT_db.dbo.USERINFO U " +
                                                                                                    "on HRISP._EmpID = U.BADGENUMBER " +
                                                                                    "INNER JOIN ( select * from OPENQUERY (" + Properties.Settings.Default.ODBC + ", 'select EmpNO,Effectivity,S.SchedID,  " +
                                                                                                "(case TypeID when 2 then 1 else 0 end ) as TypeID from (select distinct(EmpNO) ,  " +
                                                                                                "Effectivity,Schedid from admx_hrisp.pp_empschedules  where Effectivity <= \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\"  order by effectivity desc) E " +
                                                                                                "Inner join admx_hrisp.pp_schedules S  on E.Schedid= S.Schedid  where Effectivity <= \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\" and TypeID <> 2 group by E.EmpNo') ) as HRISPSched  " +
                                                                                                "on HRISPSched.EmpNO = U.BADGENUMBER " +
                                                                                    "INNER JOIN (select  *, RANK()over(partition by UserID order by CHECKTIME desc) as r  FROM ATT_db.[dbo].[CHECKINOUT] " +
                                                                                                "where (convert(varchar(10),CHECKTIME,101) = '" + String.Format("{0:MM/dd/yyyy}", dtDate.SelectedDate) + "' and CHECKTYPE in('O'))) C " +
                                                                                                    "on C.UserID = U.UserID  where C.r =1 ", _SqlConn)) _SqlUpdateDayShift.ExecuteNonQuery();

                    using (SqlCommand _SqlUpdateNightShift = new SqlCommand("UPDATE HRISP " +
                                                                           "SET HRISP._DateIn = C.CHECKTIME " +
                                                                           "FROM   OPENQUERY(" + Properties.Settings.Default.ODBC + ", " +
                                                                                  "'select * from  admx_hrisp.pp_EmpClocks where  " +
                                                                                                "_DateIn is null and " +
                                                                                                "DATE_FORMAT(_DateOut,\"%Y-%m-%d\") =\"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\"') HRISP  " +
                                                                                   "INNER JOIN ATT_db.dbo.USERINFO U " +
                                                                                                   "on HRISP._EmpID = U.BADGENUMBER " +
                                                                                   "INNER JOIN ( select * from OPENQUERY (" + Properties.Settings.Default.ODBC + ", 'select EmpNO,Effectivity,S.SchedID,  " +
                                                                                               "(case TypeID when 2 then 1 else 0 end ) as TypeID from (select distinct(EmpNO) ,  " +
                                                                                               "Effectivity,Schedid from admx_hrisp.pp_empschedules  where Effectivity <= \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\"  order by effectivity desc) E " +
                                                                                               "Inner join admx_hrisp.pp_schedules S  on E.Schedid= S.Schedid  where Effectivity <= \"" + String.Format("{0:yyyy-MM-dd}", dtDate.SelectedDate) + "\" and TypeID = 2 group by E.EmpNo') ) as HRISPSched  " +
                                                                                               "on HRISPSched.EmpNO = U.BADGENUMBER " +
                                                                                   "INNER JOIN (select  *, RANK()over(partition by UserID order by CHECKTIME desc) as r  FROM ATT_db.[dbo].[CHECKINOUT] " +
                                                                                               "where (convert(varchar(10),CHECKTIME,101) = '" + String.Format("{0:MM/dd/yyyy}", dtDate.SelectedDate.Value.AddDays(-1)) + "' and CHECKTYPE in('I'))) C " +
                                                                                                   "on C.UserID = U.UserID  where C.r =1 ", _SqlConn)) _SqlUpdateNightShift.ExecuteNonQuery();



                }
            }
            catch (Exception _eX)
            {
                MessageBox.Show(_eX.Message);
            }

        }

        private void dtSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dtSite_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
            //object item = dtSite.SelectedItem;
            //SelSiteID = Convert.ToInt32((dtSite.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text);

        }
       

      

       

       
    }
}
