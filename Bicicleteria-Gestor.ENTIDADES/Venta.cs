﻿using System;
using System.Collections.Generic;

namespace Bicicleteria_Gestor.ENTIDADES
{
    public partial class Venta
    {
        public Venta()
        {
            DetalleVenta = new HashSet<DetalleVenta>();
        }

        public int IdVenta { get; set; }
        public string? NumeroVenta { get; set; }
        public int? IdUsuario { get; set; }
        public decimal? Total { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? IdCliente { get; set; }
        public string? DocumentoCliente { get; set; }
        public string? NombreCliente { get; set; }

        public virtual Cliente? IdClienteNavigation { get; set; }
        public virtual Usuario? IdUsuarioNavigation { get; set; }
        public virtual ICollection<DetalleVenta> DetalleVenta { get; set; }
    }
}
