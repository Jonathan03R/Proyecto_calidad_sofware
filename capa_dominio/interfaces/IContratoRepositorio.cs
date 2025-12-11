using capa_dominio.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_dominio.interfaces
{
    public interface IContratoRepositorio
    {
        List<Contrato> ObtenerContratosPorTrabajador(int trabajadorId);
        List<ContratoPorPeriodoDTO> ListarContratosPorPeriodo(int periodoId);
    }
}
