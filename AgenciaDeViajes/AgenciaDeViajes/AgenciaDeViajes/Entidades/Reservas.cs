using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ReservasNuevas
    {
        public DateTime FechaVueloIda { get; set; }
        public DateTime FechaVueloVuelta { get; set; }
        public string Horario { get; set; }
        public int Clase { get; set; }
        public int ReservaConfirmada { get; set; }
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
}
