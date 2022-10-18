using System;
using System.Collections.Generic;

namespace Bicicleteria_Gestor.ENTIDADES
{
    public partial class Cliente
    {
        public Cliente()
        {
            Deuda = new HashSet<Deuda>();
            Venta = new HashSet<Venta>();
        }

        public int IdCliente { get; set; }
        public string? Documento { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Ciudad { get; set; }
        public string? Direccion { get; set; }
        public bool? Deudor { get; set; }
        public DateTime? FechaRegistro { get; set; }

        public virtual ICollection<Deuda> Deuda { get; set; }
        public virtual ICollection<Venta> Venta { get; set; }
    }
}
