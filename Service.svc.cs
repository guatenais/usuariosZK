using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfTarjeta.Utilidades;

namespace WcfTarjeta
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.

    public class Service : IService
    {
        private static string archivoLog;
        private static string path;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static Service(){
          
            archivoLog = Properties.Settings.Default.ArchivoLog;
            path = Properties.Settings.Default.PathLog;
            if (!log4net.LogManager.GetRepository().Configured)
            {
                var logConfigPath = System.IO.Path.Combine(path, archivoLog);
                if (System.IO.File.Exists(logConfigPath))
                {
                    try
                    {
                        log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(logConfigPath));
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("Error {0}", e.Message);
                    }
                }

            }
            log.Info("SE INICIA EL WEBSERVICE");
            Operaciones.InicioDB();
        }
        public Respuesta DeshabilitarUsuario(int value,string ip="")
        {
          
            return Operaciones.DeshabilitarTarjeta(value,ip);
        }

        public Respuesta HabilitarUsuario(int value,string ip="")
        {
           
            return Operaciones.HabilitarTarjeta(value,ip);
        }
    }

}
