using Bicicleteria_Gestor.ENTIDADES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicicleteria_Gestor.BLL.Interfaces
{
    public interface IRolService
    {
        Task<List<Rol>>Lista();
    }
}
