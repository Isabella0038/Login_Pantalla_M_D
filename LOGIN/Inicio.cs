using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Pantalla_M_D;
using Registro;

namespace INICIO
{
    // Clase principal del formulario de inicio
    public partial class frmInicio : Form
    {
        // Constructor de la clase
        public frmInicio()
        {
            InitializeComponent();
        }
        // Declaración de variables globales
        int contErr = 0;

        // Evento del botón "Aceptar"
        private void btnOk_Click(object sender, EventArgs e)
        {
            // Cadena de conexión a la base de datos
            string connectionString = "Data Source= ISABELLA\\SQLEXPRESS01; Database = Usuario; Integrated Security = True";
            // Obtención de valores del formulario
            string username = txtNum.Text;
            string password = txtPass.Text;

            // Uso de la conexión a la base de datos
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Usuario WHERE Nombre = @Nombre AND Contraseña = @Contraseña";

                // Uso de la consulta SQL
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Parámetros de la consulta
                    command.Parameters.AddWithValue("@Nombre", username);
                    command.Parameters.AddWithValue("@Contraseña", password);
                    int count = (int)command.ExecuteScalar();

                    // Verificación de credenciales
                    if (count > 0)
                    {
                        MessageBox.Show("Inicio de sesión exitoso");
                        // Mostrar el formulario Pantalla_M_D
                        this.Hide(); // Ocultar el formulario de inicio de sesión 
                        frmPantalla_M_D f2 = new frmPantalla_M_D (); // Asumiendo que tienes un formulario llamado Pantalla_M_D
                        f2.Show();
                    }
                    else
                    {
                        // Manejo de intentos fallidos
                        contErr++;
                        if (contErr >= 3)
                        {
                            MessageBox.Show("Demasiados intentos fallidos. Cerrando la aplicación.");
                            Application.Exit();
                        }
                        else
                        {
                            MessageBox.Show($"Nombre de usuario o contraseña incorrectos. Intentos restantes: {3 - contErr}");
                        }
                    }
                }
            }
        }

        // Evento del botón "Cancelar"
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Evento del botón "Registrarse"
        private void btnRegistrarse_Click(object sender, EventArgs e)
        {
            // Ocultar el formulario actual y mostrar el formulario de registro
            this.Hide();
            registro frm = new registro();
            frm.Show();
        }
    }
}
