using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfTarjeta.Utilidades
{
    [DataContract]
    public class Respuesta
    {
        private bool dato;
        private string descripcion;

        [DataMember]
        public bool Dato { get => dato; set => dato = value; }
        [DataMember]
        public string Descripcion { get => descripcion; set => descripcion = value; }
    }
}