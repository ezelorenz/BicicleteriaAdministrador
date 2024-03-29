﻿using Bicicleteria_Gestor.ENTIDADES;

namespace Bicicleteria_Gestor.APP.Models.ViewModels
{
    public class VMVenta
    {
        public int? IdVenta { get; set; }
        public string? NumeroVenta { get; set; }
        public int? IdUsuario { get; set; }
        public string? Usuario { get; set; }
        public string? Total { get; set; }
        public string? FechaRegistro { get; set; }
        public string? DocumentoCliente { get; set; }
        public string? NombreCliente { get; set; }
        public virtual ICollection<VMDetalleVenta>? DetalleVenta { get; set; }

    }
}
