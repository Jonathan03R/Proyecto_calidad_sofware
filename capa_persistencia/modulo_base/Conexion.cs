using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace capa_persistencia.modulo_base
{
    public class AccesoSQLServer
    {
        private SqlConnection conexion;
        private SqlTransaction transaccion;

        // Variables de conexión
        private readonly string servidor = "nominas02calidad.database.windows.net";
        private readonly string puerto = "1433";
        private readonly string baseDatos = "nominas02_calidad";
        private readonly string usuario = "nominas02@nominas02calidad";
        private readonly string contrasena = "Grupo02_2025";

        private string ConnectionString =>
            $"Server={servidor},{puerto};" +
            $"Database={baseDatos};" +
            $"User ID={usuario};" +
            $"Password={contrasena};" +
            "Encrypt=True;" +
            "TrustServerCertificate=False;" +
            "Connection Timeout=30;";

        public void AbrirConexion()
        {
            try
            {
                conexion = new SqlConnection(ConnectionString);
                conexion.Open();
                Console.WriteLine("Conexión con la Base de Datos Azure establecida correctamente.");
            }
            catch (Exception err)
            {
                throw new Exception("Error en la conexión con la Base de Datos.", err);
            }
        }

        public void CerrarConexion()
        {
            try
            {
                if (conexion != null && conexion.State == ConnectionState.Open)
                    conexion.Close();
            }
            catch (Exception err)
            {
                throw new Exception("Error al cerrar la conexión con la Base de Datos.", err);
            }
        }

        public void IniciarTransaccion()
        {
            try
            {
                AbrirConexion();
                transaccion = conexion.BeginTransaction();
            }
            catch (Exception err)
            {
                throw new Exception("Error al iniciar la transacción con la Base de Datos.", err);
            }
        }

        public void TerminarTransaccion()
        {
            try
            {
                transaccion.Commit();
                CerrarConexion();
            }
            catch (Exception err)
            {
                throw new Exception("Error al terminar la transacción con la Base de Datos.", err);
            }
        }

        public void CancelarTransaccion()
        {
            try
            {
                transaccion.Rollback();
                CerrarConexion();
            }
            catch (Exception err)
            {
                throw new Exception("Error al cancelar la transacción con la Base de Datos.", err);
            }
        }

        public SqlDataReader EjecutarConsulta(string sentenciaSQL)
        {
            try
            {
                SqlCommand comandoSQL = conexion.CreateCommand();
                if (transaccion != null)
                    comandoSQL.Transaction = transaccion;
                comandoSQL.CommandText = sentenciaSQL;
                comandoSQL.CommandType = CommandType.Text;
                return comandoSQL.ExecuteReader();
            }
            catch (Exception err)
            {
                throw new Exception("Error al ejecutar consulta.", err);
            }
        }

        public SqlCommand ObtenerComandoSQL(string sentenciaSQL)
        {
            try
            {
                SqlCommand comandoSQL = conexion.CreateCommand();
                if (transaccion != null)
                    comandoSQL.Transaction = transaccion;
                comandoSQL.CommandText = sentenciaSQL;
                comandoSQL.CommandType = CommandType.Text;
                return comandoSQL;
            }
            catch (Exception err)
            {
                throw new Exception("Error al obtener comando de ejecución.", err);
            }
        }

        public SqlCommand ObtenerComandoDeProcedimiento(string procedimientoAlmacenado)
        {
            try
            {
                SqlCommand comandoSQL = conexion.CreateCommand();
                if (transaccion != null)
                    comandoSQL.Transaction = transaccion;
                comandoSQL.CommandText = procedimientoAlmacenado;
                comandoSQL.CommandType = CommandType.StoredProcedure;
                return comandoSQL;
            }
            catch (Exception err)
            {
                throw new Exception("Error al obtener comando de ejecución.", err);
            }
        }
    }
}