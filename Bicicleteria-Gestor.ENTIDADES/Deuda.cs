using System;
using System.Collections.Generic;

namespace Bicicleteria_Gestor.ENTIDADES
{
    public partial class Deuda
    {
        public int IdDeuda { get; set; }
        public int? IdCliente { get; set; }
        public int? CantidadCuotas { get; set; }
        public int? ValorCuota { get; set; }
        public int? PrecioFinanciado { get; set; }
        public int? CuotasCanceladas { get; set; }
        public int? SaldoAdeudado { get; set; }

        public virtual Cliente? IdClienteNavigation { get; set; }
    }
}
