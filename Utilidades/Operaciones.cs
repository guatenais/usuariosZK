
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WcfTarjeta.Utilidades.WcfTarjeta.Utilidades;

namespace WcfTarjeta.Utilidades
{
    public class Operaciones
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("OPERACION -->");
        public static void InicioDB()
        {
            //GeneralDB._instanceName = Properties.Settings.Default.InstanceName;
            //GeneralDB._databaseName = Properties.Settings.Default.DataBaseName;
            //GeneralDB._userName = Properties.Settings.Default.UserName;
            //GeneralDB._serverName = Properties.Settings.Default.ServerName;
            //GeneralDB.connectionString_Builder();
        }

        public static Respuesta HabilitarTarjeta(int pin,string ip="")
        {
            return OperarUsuario(pin, Estado.HABILITAR,ip);
        }
        public static Respuesta DeshabilitarTarjeta(int pin,string ip="")
        {
            return OperarUsuario(pin, Estado.DESHABILITAR,ip);
        }
        private static Respuesta OperarUsuario(int pin, Estado operacion,string ip="") { //Operacion = 1 -Habilitar 2-Deshabilitar
            bool conectado = false;
            Respuesta r = new Respuesta();
            r.Dato = false;
            r.Descripcion = string.Empty;
            try
            {
              
                //DataTable dtMolinetes = DBRegistrosZK.GeneralDB.ListaPanelesTipo(false,ip);
                //if (dtMolinetes.Rows.Count > 0)
                //{
                //    foreach (DataRow row in dtMolinetes.Rows)
                //    {
                if (!string.IsNullOrEmpty(ip))
                {
                    string p = "protocol=TCP,ipaddress=" +ip+ ",port=4370 ,timeout=5000,passwd=";
                    log.InfoFormat("{0}  el  usuario [{1}] en el panel con ip [{2}]", operacion, pin, p);
                    conectado = FuncionesZK.Connect_Pull(p);
                    if (conectado)
                    {
                        try
                        {
                            if (FuncionesZK.GetDeviceData_Pull("user", pin) > 0) //se busca el usuario
                            {
                                if (FuncionesZK.GetDeviceData_Pull("userauthorize", pin) > 0)
                                { //se busca si tiene autorizacion en la zona y con el metodo es habilitar insertamos informacion en esa tabla
                                    switch (operacion)
                                    {
                                        case Estado.HABILITAR:
                                            if (FuncionesZK.SetDeviceData_Pull(pin))
                                            {
                                                r.Dato = true;
                                                r.Descripcion += "Se habilito el usuario " + pin + " correctamente en el panel " + ip ;
                                            }
                                            else
                                            {
                                                log.InfoFormat("No se pudo Habilitar el usuario [{0}] en el panel [{1}] LastError {2}", pin, p, FuncionesZK.LastError_Pull());
                                                r.Descripcion += "No se pudo Habilitar el usuario en el panel " + ip ;
                                            }
                                            break;
                                        case Estado.DESHABILITAR:
                                            if (FuncionesZK.DeleteDeviceData_Pull(pin))
                                            {
                                                r.Dato = true; 
                                                r.Descripcion += "Se deshabilito el usuario " + pin + " correctamente en el panel " + ip;
                                            }
                                            else
                                            {
                                                FuncionesZK.PullLastError();
                                                log.InfoFormat("No se pudo Deshabilitar el usuario [{0}] en el panel [{1}] LastError {2}", pin, p, FuncionesZK.LastError_Pull());
                                                r.Descripcion += "No se pudo Deshabilitar el usuario en el panel " +ip;
                                            }
                                            break;
                                    }

                                }
                                else//
                                {
                                    if (Estado.HABILITAR == operacion)
                                    {
                                        if (FuncionesZK.SetDeviceData_Pull(pin))
                                        {
                                            r.Dato = true;
                                            r.Descripcion += "Se habilito el usuario " + pin + " correctamente en el panel " + ip;
                                        }
                                        else
                                        {
                                            log.InfoFormat("No se pudo Habilitar el usuario [{0}] en el panel [{1}] lastError {2}", pin, p, FuncionesZK.LastError_Pull());
                                            r.Descripcion += "No se pudo Habilitar el usuario en el panel " + ip;
                                        }
                                    }
                                    else
                                    {
                                        log.InfoFormat("No se encontraron datos para [{0}] el usuario {1} en  el panel [{2}] ", operacion, pin, p);
                                        r.Descripcion += "No se encontraron datos para " + operacion + " el usuario " + pin + " en el panel " +ip;
                                    }
                                }
                            }
                            else
                            {
                                log.InfoFormat("No se encontro el usuario [{0}] en el panel con ip [{1}] lastError {2} ", pin, p, FuncionesZK.LastError_Pull());
                                r.Descripcion += "No se encontro el usuario en el panel " + ip;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("Error en proceso de {0} mensaje de error {1}", operacion, ex.Message);
                        }
                        FuncionesZK.Disconect_Pull();
                    }
                    else
                    {
                        int error = FuncionesZK.LastError_Pull();
                        log.Info("No se pudo conectar con el panel " +ip  + " error " + error);
                        r.Descripcion += "No se pudo conectar con el panel " +ip + " error " + error;
                    }
                }
                else {
                    log.Info("No ingreso la ip del servidor la ip del servidor");
                    r.Descripcion += "No ingreso la ip del servidor";
                }
                //    }
                //}
                //else
                //{
                //    r.Descripcion += "No se encontro ningun panel con la ip "+ip;
                //}

                log.InfoFormat("Termina La Operacion [{0}]", operacion);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Erron en proceso de [{0}] mensaje [{1}]", operacion, ex.Message);
                r.Dato = false;
                r.Descripcion = ex.Message+" | ";
            }
            return r;
        }
    }
}
public enum Estado{ 
    HABILITAR,
    DESHABILITAR
}
public enum Paneles { 
     ENT=1

}
