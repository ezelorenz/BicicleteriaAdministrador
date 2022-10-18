using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicicleteria_Gestor.BLL.Interfaces
{
    public interface IUtilidadesService
    {
        string GenerarClave();
        string CovertirSha256(string texto);
    }
}
