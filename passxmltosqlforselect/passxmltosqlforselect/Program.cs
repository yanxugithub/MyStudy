using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace passxmltosqlforselect
{
    class Program
    {
        static void Main(string[] args)
        {
            string xml = "<Employees><Employee number='1' firstname='john' lastname='smith'></Employee><Employee number='2' firstname='john1' lastname='smith1'></Employee><Employee number='3' firstname='john2' lastname='smith2'></Employee></Employees>";

            OracleConnection oc = new OracleConnection("Data Source=dev1;User ID=system;Password=oracle;");

            oc.Open();

            OracleCommand cmd = oc.CreateCommand();

            string sql =
                @"
                    SELECT 
                        xt.* 
                    FROM 
                        XMLTABLE
                        (
                            '/Employees/Employee' PASSING :l_xml
                            COLUMNS
                                empno    VARCHAR2(4)  PATH '@number',
                                firstname VARCHAR2(10) PATH '@firstname',
                                lastname VARCHAR2(9)  PATH '@lastname'
                        ) xt
                            join employees e on xt.empno = e.empno";

            cmd.CommandText = sql;

            OracleParameter p = cmd.CreateParameter();
            p.ParameterName = "l_xml";
            OracleXmlType t = new OracleXmlType(oc, xml);
            p.Value = t;
            /*p.OracleDbType = OracleDbType.Clob;
            OracleClob c = new OracleClob(oc);
            c.Read(xml.ToCharArray(), 0, xml.Length);
            p.Value = c;*/
            cmd.Parameters.Add(p);

            OracleDataReader reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                if (reader.HasRows)
                {
                    object[] values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    Console.WriteLine
                    (
                        string.Format
                        (
                            "empno='{0}' lastname={1} firstname='{2}'", 
                            reader.GetDecimal(reader.GetOrdinal("empno")), 
                            reader.GetString(reader.GetOrdinal("lastname")), 
                            reader.GetString(reader.GetOrdinal("firstname"))
                        )
                    ); 
                }
            }

            //c.Close();

            reader.Close();

            oc.Close();
        }
    }
}
