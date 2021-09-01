using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfTarjeta.Utilidades
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web;

    namespace WcfTarjeta.Utilidades
    {
        public class FuncionesZK
        {
            private static readonly log4net.ILog log = log4net.LogManager.GetLogger("FUNCIONES -->");
            static IntPtr h = IntPtr.Zero;
           [DllImport("plcommpro.dll", EntryPoint = "PullLastError")]
            public static extern int PullLastError();
            public static int LastError_Pull()
            {
                return PullLastError();
            }
            [DllImport(@"C:\Windows\SysWOW64\plcommpro.dll", EntryPoint = "Connect")] //C:\espark\dllZK
           // [DllImport(@"C:\ESPark\dllZK\plcommpro.dll", EntryPoint = "Connect")]
            private static extern IntPtr Connect(string Parameters);
            public static bool Connect_Pull(string str= "protocol=TCP,ipaddress=192.168.10.250,port=1433,timeout=5000,passwd=")
            {
                

                int ret = 0;        // Error ID number
                bool dato = true;                 //    Cursor = Cursors.WaitCursor;
                h = IntPtr.Zero;
                if (IntPtr.Zero == h)
                {
                    try
                    {
                        h = Connect(str);
                    
                    if (h != IntPtr.Zero)
                    {
                       // Console.WriteLine("Conectado a [{0}]", str);

                        dato = true;
                           
                    }
                    else
                    {
                        ret = PullLastError();
                            log.ErrorFormat("Error en Connect Error No.{0}", ret);
                        dato = false;
                    }
                    }
                    catch (Exception ex) {
                        log.ErrorFormat("Error en el metodo connect {0}", ex.Message);
                        dato = false; 
                    }
                }
                return dato;
            }
            [DllImport(@"C:\Windows\SysWOW64\plcommpro.dll", EntryPoint = "Disconnect")]
            public static extern void Disconnect(IntPtr h);
            public static void Disconect_Pull()
            {
                if (IntPtr.Zero != h)
                {
                    Disconnect(h);
                    h = IntPtr.Zero;

                }
            }
            [DllImport(@"C:\Windows\SysWOW64\plcommpro.dll", EntryPoint = "GetDeviceData")]
            private static extern int GetDeviceData(IntPtr h, ref byte buffer, int buffersize, string tablename, string filename, string filter, string options);
            public static int GetDeviceData_Pull(string tabla, int pin)
            {   //Buscar datos en el panel segun la tabla y devuelve el valor de filas encontradas
             
                DataTable _tableResult = new DataTable(tabla);
                string[] tmp = null;
                int ret = 0;
                int BUFFERSIZE = 10 * 1024 * 1024;
                byte[] buffer = new byte[BUFFERSIZE];
                string str = Data(tabla);//"Cardno\tPin\tVerified\tDoorID\tEventType\tInOutState\tTime_second";
                string fil = "Pin=" + pin;
                if (pin==0) {
                    fil = "";
                }


                if (IntPtr.Zero != h)
                {
                    
                    ret = GetDeviceData(h, ref buffer[0], BUFFERSIZE, tabla, str, fil, "");
                
                    if (ret > 0)
                    {
                        str = Encoding.Default.GetString(buffer);
                        string[] stringSeparators = new string[] { "\r\n" };
                        tmp = str.Split(stringSeparators, StringSplitOptions.None);

                        for (int i = 1; i <= ret; i++)
                        {
                            string s = tmp[i];
                            string[] ss = s.Split(',');
                            log.InfoFormat("DATOS ENCONTRADOS EN LA TABLA {0} - {1} - {2}", tabla, ss[0], ss[1]);

                        }
                    }
                    else {
                        log.InfoFormat("NO SE ENCONTRARON DATOS EN LA TABLA {0}", tabla);
                    }
                }
                return ret;

            }
            [DllImport(@"C:\Windows\SysWOW64\plcommpro.dll", EntryPoint = "SetDeviceData")]
            public static extern int SetDeviceData(IntPtr h, string tablename, string data, string options);
            public static bool SetDeviceData_Pull(int pin) {
                int ret = 0;
                bool dato = false;
                string data = "Pin="+pin+"\tAuthorizeTimezoneId=1\tAuthorizeDoorId=15";
                if (IntPtr.Zero != h)
                {
                    ret = SetDeviceData(h, "userauthorize", data, "");

                    if (ret >= 0) {
                        log.InfoFormat("Datos Asignados [{0}] ", data);
                        dato = true;
                    }
                }
                return dato;
            }
            [DllImport(@"C:\Windows\SysWOW64\plcommpro.dll", EntryPoint = "DeleteDeviceData")]
            public static extern int DeleteDeviceData(IntPtr h, string tablename, string data, string options);
            public static bool DeleteDeviceData_Pull(int pin)
            {
                bool dato = false;
                int ret = 0;
            
                

                string data = "Pin="+pin;
                if (IntPtr.Zero != h)
                {
                    ret = DeleteDeviceData(h, "userauthorize", data, "");
                    if (ret >= 0)
                    {
                        log.InfoFormat("Datos Eliminados [{0}] ", data);
                        dato = true;
                    }

                }

                return dato;
            }


            private static DateTime EnteroAFechaHora(long numero)
            {
                long segundos = numero % 60;
                numero = numero / 60;
                long minutos = numero % 60;
                numero = numero / 60;
                long horas = numero % 24;
                numero = numero / 24;
                long dia = numero % 31 + 1;
                numero = numero / 31;
                long mes = numero % 12 + 1;
                numero = numero / 12;
                long anio = numero + 2000;
                string fecha = anio + "/" + mes + "/" + dia + " " + horas + ":" + minutos + ":" + segundos;
                return Convert.ToDateTime(fecha);

            }
            private static DataTable Tabla(string tabla) {
                DataTable _tableResult=new DataTable();
                if (tabla == "user")
                {
                               
                    _tableResult.Columns.Add("CardNo", typeof(long));
                    _tableResult.Columns.Add("Pin", typeof(int));
                    _tableResult.Columns.Add("Password", typeof(string));
                    _tableResult.Columns.Add("Group", typeof(string));
                    _tableResult.Columns.Add("StartTime", typeof(string));
                    _tableResult.Columns.Add("EndTime", typeof(string));
                }
                else if (tabla == "transaction")
                {
                    _tableResult.Columns.Add("Numero", typeof(long));
                    _tableResult.Columns.Add("CardNo", typeof(long));
                    _tableResult.Columns.Add("Pin", typeof(int));
                    _tableResult.Columns.Add("Verified", typeof(int));
                    _tableResult.Columns.Add("DoorId", typeof(int));
                    _tableResult.Columns.Add("EventType", typeof(int));
                    _tableResult.Columns.Add("InOutState", typeof(int));
                    _tableResult.Columns.Add("Date", typeof(DateTime));
                    _tableResult.Columns.Add("IdPanel", typeof(int));
                }
                else if (tabla == "userauthorize") { }
                return _tableResult;
            }
            private static string Data(string data) {
                string dato = "";
                if (data == "user")
                {
                    dato = "CardNo\tPin\tPassword\tGroup\tStartTime\tEndTime";
                }
                else if (data == "userauthorize") {
                    dato = "Pin\tAuthorizeTimezoneId\tAuthorizeDoorId";
                }
                return dato;
            }
            
        }
    }
}