using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ReservasNuevas
    {
        public string FechaVueloIda { get; set; }
        public string FechaVueloVuelta { get; set; }
        public string Horario { get; set; }

        public int Clase { get; set; }
        public int TipoViaje { get; set; }
        public int Destino { get; set; }
        public int ReservaConfirmada { get; set; }
        public decimal ValorTotalPagar { get; set; }
    }

    public class TiposClases
    {
        public int CLas_Id { get; set; }
        public string Nombre { get; set; }
    }
    public class TipoViaje
    {
        public int Via_id { get; set; }
        public string Via_nombre  { get; set; }
    }
    public class Destino
    {
        public int Des_id { get; set; }
        public string Des_nombre { get; set; }
    }
    public class ResumenCompra
    {
        public string NombreUsuario { get; set; }
        public string Destino  { get; set; }
        public string FechaIda  { get; set; }
        public string FechaVuelta  { get; set; }
        public string Hora  { get; set; }
        public string TipoViaje { get; set; }
        public string  ValorTotalPagar { get; set; }
         public  int Id_Reserva { get; set; }


    }
}
