using INICIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registro
{
    // Clase principal del formulario de registro
    public partial class registro : Form
    {
        // Constructor de la clase
        public registro()
        {
            InitializeComponent();
        }
        // Evento del botón "Registrarse"
        private void btnRegistrarse(object sender, EventArgs e)
        {
            // Creación de la conexión a la base de datos
            SqlConnection conexion = new SqlConnection("Server=ISABELLA\\SQLEXPRESS01 ; Database=Usuario ; Integrated Security=True;");
            try
            {
                conexion.Open();
                // Consulta SQL para insertar datos en la tabla Usuario
                string query = "insert into Usuario(Nombre,Contraseña) values('" + Usuario.Text + "','" + Contraseña.Text + "')";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.ExecuteNonQuery();
                // Mensaje de registro exitoso
                MessageBox.Show("Registrado correctamente");
                // Ocultar el formulario actual y mostrar el formulario de inicio de sesión
                this.Hide();
                frmInicio f1 = new frmInicio(); // Asumiendo que tienes un formulario de inicio de sesión con el nombre frmInicioSesion
                f1.Show();
            }
            finally
            // Cierre de la conexión a la base de datos
            { conexion.Close(); }

        }

    }
}

