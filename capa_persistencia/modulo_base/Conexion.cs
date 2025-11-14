using System;
using System.Data;
using System.Data.SqlClient;

namespace capa_persistencia.modulo_base
{
    public class AccesoSQLServer
    {
        private SqlConnection conexion;
        private SqlTransaction transaccion;


        // Configuración de conexión para Azure SQL
        private readonly string servidor = "nominas02calidad.database.windows.net";
        private readonly string baseDatos = "nominas02_calidad";
        //private readonly string baseDatos = "bdProcesarNomina";


        private readonly string usuario = "nominas02@nominas02calidad";
        private readonly string contrasena = "Grupo02_2025";

        // ConnectionString completo
        private string ConnectionString =>
            $"Server={servidor};" +
            $"Database={baseDatos};" +
            $"User ID={usuario};" +
            $"Password={contrasena};"+
            "Encrypt=True;" +
            "TrustServerCertificate=True;" +
            "Connection Timeout=30;";

        public void AbrirConexion()
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                conexion = new SqlConnection(ConnectionString);
                conexion.Open();
                Console.WriteLine("✅ Conexión con Azure SQL establecida correctamente.");
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("❌ Error SQL: " + sqlEx.Message);
                throw new Exception("❌ Error en la conexión con la Base de Datos. Verifica el firewall o las credenciales.", sqlEx);
            }
            catch (Exception err)
            {
                Console.WriteLine("❌ Error general: " + err.Message);
                throw new Exception("❌ Error en la conexión con la Base de Datos.", err);
            }
        }

        // Cerrar conexión
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

        // Transacciones
        public void IniciarTransaccion()
        {
            try
            {
                AbrirConexion();
                transaccion = conexion.BeginTransaction();
            }
            catch (Exception err)
            {
                throw new Exception("Error al iniciar la transacción.", err);
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
                throw new Exception("Error al terminar la transacción.", err);
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
                throw new Exception("Error al cancelar la transacción.", err);
            }
        }

        // Ejecutar consultas SQL
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

        // Obtener comando SQL
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
                throw new Exception("Error al obtener comando SQL.", err);
            }
        }

        // Obtener comando de procedimiento almacenado
        public SqlCommand ObtenerComandoDeProcedimiento(string procedimientoAlmacenado)
        {
            try
            {
                if (conexion == null)
                    throw new InvalidOperationException("La conexión es nula. Debes llamar a AbrirConexion() antes de usar ObtenerComandoDeProcedimiento.");

                if (conexion.State != ConnectionState.Open)
                    throw new InvalidOperationException("La conexión no está abierta. Llama a AbrirConexion() primero.");

                SqlCommand comandoSQL = conexion.CreateCommand();

                if (transaccion != null)
                    comandoSQL.Transaction = transaccion;

                comandoSQL.CommandText = procedimientoAlmacenado;
                comandoSQL.CommandType = CommandType.StoredProcedure;

                return comandoSQL;
            }
            catch (Exception err)
            {
                throw new Exception("Error al obtener comando de procedimiento.", err);
            }
        }

    }
}
