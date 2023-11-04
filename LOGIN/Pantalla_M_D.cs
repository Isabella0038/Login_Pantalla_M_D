using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;

namespace Pantalla_M_D
{
    public partial class frmPantalla_M_D : Form
    {   // Cadena de conexión a la base de datos SQL Server
        string connectionString = "Data Source=ISABELLA\\SQLEXPRESS01;Database=Datos;Integrated Security=True";
        SqlConnection sqlConnection; // Objeto de conexión SQL
        SqlDataAdapter dataAdapter; // Adaptador de datos para la base de datos
        DataSet dataSet; // Conjunto de datos para contener los resultados de la consulta
        int position = 0; // Posición actual en el conjunto de datos

        // Constructor de la clase Form1
        public frmPantalla_M_D()
        {
            InitializeComponent();
            dataGridViewAbonos.AutoGenerateColumns = true; // Generar automáticamente las columnas del DataGridView
            sqlConnection = new SqlConnection(connectionString); // Inicializar la conexión a la base de datos
            dataSet = new DataSet(); // Inicializar el conjunto de datos
            GetData("SELECT Id, Nombre, Apellido, Contraseña, Telefono FROM Registros"); // Obtener los datos iniciales de la tabla "Registros"
            DisplayData(position); // Mostrar los datos en la posición actual
            dataGridView1.CellClick += dataGridView1_CellClick;  // Asignar un controlador de eventos para el clic en celdas del DataGridView
        }

        // Método para obtener datos de la base de datos
        private void GetData(string selectCommand)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    dataAdapter = new SqlDataAdapter(selectCommand, sqlConnection);
                    dataSet = new DataSet();
                    dataAdapter.Fill(dataSet, "Registros");
                    dataGridView1.DataSource = dataSet.Tables["Registros"];
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex.Message);  // Mostrar un mensaje de error en caso de excepción
            }
            finally
            {
                sqlConnection.Close(); // Cerrar la conexión a la base de datos
            }
        }

        // Método para mostrar los datos en los controles de la interfaz de usuario
        private void DisplayData(int position)
        {
            DataTable dataTable = dataSet.Tables["Registros"];
            if (dataTable.Rows.Count > 0)
            {
                DataRow dataRow = dataTable.Rows[position];
                // Asignar valores a los controles de la interfaz de usuario
                textBoxID.Text = dataRow["Id"].ToString();
                textBoxNombre.Text = dataRow["Nombre"].ToString();
                textBoxApellido.Text = dataRow["Apellido"].ToString();
                textBoxContraseña.Text = dataRow["Contraseña"].ToString();
                textBoxTelefono.Text = dataRow["Telefono"].ToString();
                label1.Text = "Registros " + (position + 1) + " de " + dataTable.Rows.Count;

                int personaId;
                if (int.TryParse(textBoxID.Text, out personaId))
                {
                    LoadAbonos(personaId); // Cargar los abonos asociados a la persona seleccionada
                }
            }
        }

        // Método para guardar registros en la base de datos
        private void BtnGuardarRegistr(object sender, EventArgs e)
        {
            try
            {
                using (SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter))
                {
                    DataRow dataRow = dataSet.Tables["Registros"].NewRow();
                    // Asignar valores de los controles de la interfaz de usuario al nuevo registro
                    dataRow["Id"] = textBoxID.Text;
                    dataRow["Nombre"] = textBoxNombre.Text;
                    dataRow["Apellido"] = textBoxApellido.Text;
                    dataRow["Contraseña"] = textBoxContraseña.Text;
                    dataRow["Telefono"] = textBoxTelefono.Text;

                    dataSet.Tables["Registros"].Rows.Add(dataRow); // Agregar el nuevo registro al conjunto de datos
                    dataAdapter.Update(dataSet, "Registros"); // Actualizar la base de datos
                }
                GetData("SELECT * FROM Registros");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los datos: " + ex.Message); // Mostrar un mensaje de error en caso de excepción
            }
        }

        // Método para agregar abonos a la base de datos
        private void BtnAgregarAbno(object sender, EventArgs e)
        {
            // Obtener el ID de la persona seleccionada
            int personaId;
            if (int.TryParse(textBoxID.Text, out personaId))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            // Consulta SQL para insertar un nuevo abono en la tabla "Abonos"
                            command.CommandText = "INSERT INTO Abonos (Id, Nombre, NomProducto, Precio, NumCuotas, Abono, FechaAbono, Total) VALUES (@Id, @Nombre, @NomProducto, @Precio, @NumCuotas, @Abono, @FechaAbono, @Total)";
                            // Asignar parámetros a la consulta SQL
                            command.Parameters.AddWithValue("@Id", personaId);
                            // Agregar los demás parámetros según corresponda en tu aplicación
                            command.Parameters.AddWithValue("@Nombre", textBoxNombre.Text);
                            command.Parameters.AddWithValue("@NomProducto", textBoxNomProducto.Text);
                            command.Parameters.AddWithValue("@Precio", textBoxPrecio.Text);
                            command.Parameters.AddWithValue("@NumCuotas", textBoxNumCoutas.Text);
                            command.Parameters.AddWithValue("@Abono", textBoxAbono.Text);
                            command.Parameters.AddWithValue("@FechaAbono", dateTimePickerFechaAbono.Value);

                            double precio, abono, totalRestante;
                            if (double.TryParse(textBoxPrecio.Text, out precio) && double.TryParse(textBoxAbono.Text, out abono))
                            {
                                totalRestante = precio - abono;
                                command.Parameters.AddWithValue("@Total", totalRestante);
                            }

                            command.ExecuteNonQuery(); // Ejecutar la consulta SQL
                            MessageBox.Show("Abono agregado exitosamente."); // Mostrar un mensaje de éxito
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el abono: " + ex.Message); // Mostrar un mensaje de error en caso de excepción
                }
            }
            else
            {
                MessageBox.Show("ID de persona no válido."); // Mostrar un mensaje de error si el ID de la persona no es válido
            }
        }

        // Método para cargar abonos desde la base de datos
        private void LoadAbonos(int personaId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    // Consulta SQL para obtener los abonos asociados a una persona específica
                    command.CommandText = "SELECT Id, Nombre, NomProducto, Precio, NumCuotas, Abono, FechaAbono, Total FROM Abonos WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", personaId);

                    SqlDataAdapter abonosDataAdapter = new SqlDataAdapter(command);
                    DataSet abonosDataSet = new DataSet();
                    abonosDataAdapter.Fill(abonosDataSet, "Abonos");
                    dataGridViewAbonos.DataSource = abonosDataSet.Tables["Abonos"]; // Establecer el origen de datos del DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los abonos: " + ex.Message); // Mostrar un mensaje de error en caso de excepción
            }
        }

        // Método para manejar el evento de clic en celdas del DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                int personaId;
                if (int.TryParse(row.Cells["Id"].Value.ToString(), out personaId))
                {
                    LoadAbonos(personaId); // Cargar los abonos asociados a la persona seleccionada
                }
            }
        }

        // Métodos para realizar operaciones CRUD en la base de datos
        private void BtnNuevoRegistr(object sender, EventArgs e)
        {
            textBoxID.Text = "";
            textBoxNombre.Text = "";
            textBoxApellido.Text = "";
            textBoxContraseña.Text = "";
            textBoxTelefono.Text = "";
        }

        private void BtnActualizarRegistr(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    int selectedIndex = dataGridView1.SelectedRows[0].Index;
                    DataRow selectedDataRow = dataSet.Tables["Registros"].Rows[selectedIndex];
                    selectedDataRow["Id"] = textBoxID.Text;
                    selectedDataRow["Nombre"] = textBoxNombre.Text;
                    selectedDataRow["Apellido"] = textBoxApellido.Text;
                    selectedDataRow["Contraseña"] = textBoxContraseña.Text;
                    selectedDataRow["Telefono"] = textBoxTelefono.Text;

                    using (SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter))
                    {
                        dataAdapter.Update(dataSet, "Registros");
                    }
                    GetData("SELECT * FROM Registros");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar los datos: " + ex.Message);
                }
            }
        }

        private void BtnEliminarRegistr(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    int selectedIndex = dataGridView1.SelectedRows[0].Index;
                    dataSet.Tables["Registros"].Rows[selectedIndex].Delete();
                    using (SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter))
                    {
                        dataAdapter.Update(dataSet, "Registros");
                    }
                    GetData("SELECT * FROM Registros");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar los datos: " + ex.Message);
                }
            }
        }

        private void BtnPrimerRegistr(object sender, EventArgs e)
        {
            position = 0;
            DisplayData(position);
        }

        private void BtnAnteriorRegistr(object sender, EventArgs e)
        {
            if (position > 0)
            {
                position--;
                DisplayData(position);
            }
        }

        private void BtnSiguienteRegistr(object sender, EventArgs e)
        {
            DataTable dataTable = dataSet.Tables["Registros"];
            if (position < dataTable.Rows.Count - 1)
            {
                position++;
                DisplayData(position);
            }
        }

        private void BtnUltimoRegistr(object sender, EventArgs e)
        {
            DataTable dataTable = dataSet.Tables["Registros"];
            if (dataTable.Rows.Count > 0)
            {
                position = dataTable.Rows.Count - 1;
                DisplayData(position);
            }
        }

        // Método para manejar errores de datos en el DataGridView
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Error de datos en el DataGridView: " + e.Exception.Message); // Mostrar un mensaje de error en caso de excepción
        }

    }
}