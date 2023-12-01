using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Management;
using System.Net.NetworkInformation;
using System.IO;
using Microsoft.VisualBasic.Devices;
namespace Caracteristicas
{
    public partial class Municipalidad : Form
    {
        public Municipalidad()
        {
            InitializeComponent();
            Comenzar.Enabled = false;         
        }


        private void btn_Click(object sender, EventArgs e)
        {
       
            try
            {
                //--- Fecha
                DateTime currentDate = DateTime.Now;
                textBox10.Text = currentDate.ToString("yyyy-MM-dd");
                //Console.WriteLine(currentDate.ToString("yyyy-MM-dd"));

                // Obtén el nombre de usuario
                string username = Environment.UserName;
                textBox1.Text = Convert.ToString(username);

                // Limpia el ComboBox1 antes de llenarlo con las redes disponibles
                comboBox1.Items.Clear();

                // Obtiene una lista de todas las interfaces de red disponibles
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface nic in networkInterfaces)
                {
                    // Verifica que la interfaz esté activa y no sea virtual
                    if (nic.OperationalStatus == OperationalStatus.Up && !nic.Description.ToLower().Contains("virtual"))
                    {
                        // Obtiene las direcciones IP de la interfaz
                        IPInterfaceProperties ipProps = nic.GetIPProperties();
                        foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
                        {
                            // Verifica si es una dirección IPv4
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                // Agrega la dirección IP al ComboBox1
                                comboBox1.Items.Add(ip.Address);
                            }
                        }
                    }
                }
                // Verifica si se encontraron direcciones IP disponibles en ComboBox1
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.SelectedIndex = 0; // Establece el primer elemento como seleccionado
                }
                else
                {
                    // Puedes mostrar un mensaje en lugar de usar textBox3 si lo deseas
                    // MessageBox.Show("No se encontraron direcciones IP disponibles.");
                }

                // Limpia el ComboBox2 antes de llenarlo con la información del almacenamiento
                checkedListBox1.Items.Clear();

                // Define una variable para realizar un seguimiento del tamaño total de los discos
                ulong totalTamanioDiscos = 0;

                // Obtiene una lista de todas las unidades de disco disponibles
                DriveInfo[] drives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady)
                    {
                        // Obtiene el tamaño de la unidad de disco en bytes
                        ulong tamanioUnidad = Convert.ToUInt64(drive.TotalSize);

                        // Agrega el nombre de la partición y su tamaño en GB a ComboBox2
                        checkedListBox1.Items.Add($"{drive.Name} - {tamanioUnidad / (1024 * 1024 * 1024)} GB");

                        // Agrega el tamaño de esta unidad al tamaño total
                        totalTamanioDiscos += tamanioUnidad;
                    }
                }

                // Agrega el tamaño total de todos los discos al final de la lista en ComboBox2
                checkedListBox1.Items.Add($"Total: {totalTamanioDiscos / (1024 * 1024 * 1024)} GB");

                // Verifica si se encontraron unidades de disco disponibles en ComboBox2
                if (checkedListBox1.Items.Count > 0)
                {
                    checkedListBox1.SelectedIndex = 0; // Establece el primer elemento como seleccionado
                }
                else
                {
                    // Puedes mostrar un mensaje en lugar de usar textBox6 si lo deseas
                    // MessageBox.Show("No se encontraron unidades de disco disponibles.");
                }

                // Resto de tu código para obtener y mostrar otras características

                //---------------------------------------------------------------------------
                try
                {
                    // Obtiene la información de la memoria RAM del computador
                    ObjectQuery query = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                    ManagementObjectCollection collection = searcher.Get();

                    ulong totalCapacity = 0;

                    foreach (ManagementObject obj in collection)
                    {
                        totalCapacity += Convert.ToUInt64(obj["Capacity"]);
                    }

                    // Muestra el tamaño total de la RAM en el textBox3
                    textBox3.Text = Convert.ToString((totalCapacity / (1024 * 1024 * 1024)) + " GB");
                }
                catch (Exception ex)
                {
                    textBox3.Text = Convert.ToString("An error occurred: " + ex.Message);
                }
                //---------------------------------------------------------------------------
                // Comprueba si se ha seleccionado una IP y un disco después de obtener la información
                bool ipSeleccionada = comboBox1.SelectedItem != null;
                bool discoSeleccionado = checkedListBox1.CheckedItems.Count > 0;

                if (!ipSeleccionada || !discoSeleccionado)
                {
                    MessageBox.Show("Por favor, seleccione una IP y al menos un disco antes de hacer click en Comenzar.", "Selección requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    // Si ambas selecciones son válidas, habilita el botón "Comenzar"
                    Comenzar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                textBox1.Text = Convert.ToString("An error occurred: " + ex.Message);
            }


            // Resto de tu código para obtener y mostrar otras características

            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface nic in networkInterfaces)
                {
                    if (nic.OperationalStatus == OperationalStatus.Up && !nic.Description.ToLower().Contains("virtual"))
                    {
                        PhysicalAddress macAddress = nic.GetPhysicalAddress();
                        textBox2.Text = Convert.ToString(macAddress);
                        break; // Mostramos la primera interfaz activa y no virtual
                    }
                }
            }
            catch (Exception ex)
            {
                textBox2.Text = Convert.ToString("An error occurred: " + ex.Message);
            }


            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                ManagementObjectCollection collection = searcher.Get();


                foreach (ManagementObject obj in collection)
                {
                    string processorName = obj["Name"].ToString();

                    //
                    string maxSpeed = obj["MaxClockSpeed"].ToString();
                    int maxSpeedMHz = Convert.ToInt32(maxSpeed);
                    double maxSpeedGHz = maxSpeedMHz / 1000.0;
                    //
                    textBox4.Text = Convert.ToString(processorName + maxSpeedGHz + " GHz");
                }
            }
            catch (Exception ex)
            {
                textBox4.Text = Convert.ToString("An error occurred: " + ex.Message);
            }


            try
            {
                string computerName = Environment.MachineName;
                textBox7.Text = Convert.ToString(computerName);
            }
            catch (Exception ex)
            {
                textBox7.Text = Convert.ToString("An error occurred: " + ex.Message);
            }
            try
            {
                ComputerInfo computerInfo = new ComputerInfo();
                string sistemaOperativo = computerInfo.OSFullName;

                // Asignar el valor al TextBox (reemplaza 'textBox9' con el nombre de tu TextBox)
                textBox9.Text = sistemaOperativo;

            }
            catch (Exception ex)
            {
                // Maneja cualquier excepción que pueda ocurrir
                textBox9.Text = "Error al obtener el tipo de sistema operativo: " + ex.Message;
            }

        }
        
        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            HabilitarComenzarSiSeleccionado();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            HabilitarComenzarSiSeleccionado();
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void HabilitarComenzarSiSeleccionado()
        {
            // Verifica si se ha seleccionado una IP y un disco
            bool ipSeleccionada = comboBox1.SelectedItem != null;
            bool discoSeleccionado = checkedListBox1.CheckedItems.Count > 0;

            // Habilita o deshabilita el botón Comenzar según las selecciones
            Comenzar.Enabled = ipSeleccionada && discoSeleccionado;
        }

        private void Comenzar_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow fila = new DataGridViewRow();
                fila.CreateCells(dgv);

                
                //--- Usuario
                string username = Environment.UserName;
                fila.Cells[0].Value = Convert.ToString(username);

                //--- MAC
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface nic in networkInterfaces)
                {
                    if (nic.OperationalStatus == OperationalStatus.Up && !nic.Description.ToLower().Contains("virtual"))
                    {
                        PhysicalAddress macAddress = nic.GetPhysicalAddress();
                        fila.Cells[1].Value = Convert.ToString(macAddress);
                        break; // Mostramos la primera interfaz activa y no virtual
                    }
                }

                //--- IP

                // Obtén la IP seleccionada del ComboBox1
                string selectedIP = comboBox1.SelectedItem?.ToString();

                // Obtén el almacenamiento seleccionado del CheckedListBox
                string selectedStorage = checkedListBox1.SelectedItem?.ToString();

                // Verifica si se ha seleccionado una IP y almacenamiento
                if (!string.IsNullOrEmpty(selectedIP) && !string.IsNullOrEmpty(selectedStorage))
                {
                    // Agrega los datos al DataGridView en las celdas correspondientes
                    fila.Cells[2].Value = selectedIP; // Asigna la IP seleccionada en la columna 2

                    dgv.Rows.Add(fila);

                    // Limpia la selección en ComboBox1 y CheckedListBox
                    comboBox1.SelectedIndex = -1;
                    checkedListBox1.ClearSelected();
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione una IP y un almacenamiento antes de hacer clic en Comenzar.");
                }

                //--- Procesador
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                ManagementObjectCollection collection = searcher.Get();


                foreach (ManagementObject obj in collection)
                {
                    string processorName = obj["Name"].ToString();

                    //
                    string maxSpeed = obj["MaxClockSpeed"].ToString();
                    int maxSpeedMHz = Convert.ToInt32(maxSpeed);
                    double maxSpeedGHz = maxSpeedMHz / 1000.0;
                    //
                    fila.Cells[3].Value = Convert.ToString(processorName + maxSpeedGHz + " GHz");
                }

                //--- RAM
                ObjectQuery query = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
                ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(query);
                ManagementObjectCollection collection1 = searcher1.Get();

                ulong totalCapacity = 0;

                foreach (ManagementObject obj in collection1)
                {
                    totalCapacity += Convert.ToUInt64(obj["Capacity"]);
                }
                  fila.Cells[4].Value = Convert.ToString((totalCapacity / (1024 * 1024 * 1024)) + " GB");

                //--- Almacenamiento
                DriveInfo[] drives = DriveInfo.GetDrives();
                ulong totalStorage = 0;

                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady && drive.DriveType != DriveType.Removable)
                    {
                        totalStorage += Convert.ToUInt64(drive.TotalSize);
                        Console.WriteLine("Drive: " + drive.Name);
                        Console.WriteLine("Total Size: " + drive.TotalSize + " bytes");
                        Console.WriteLine("Total Size: " + (drive.TotalSize / (1024 * 1024 * 1024)) + " GB");
                    }
                }
                //fila.Cells[5].Value = Convert.ToString((totalStorage / (1024 * 1024 * 1024)) + " GB");
                // Obtén las opciones de almacenamiento seleccionadas del CheckedListBox
               // Obtén las opciones de almacenamiento seleccionadas del CheckedListBox
                
                List<string> discosSeleccionados = new List<string>();

                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                {
                    string opcion = checkedListBox1.CheckedItems[i].ToString();

                    if (opcion == "Tamaño total:") // Verifica si la opción es "Tamaño total"
                    {
                        // Ignorar aquí, procesaremos "Tamaño total" al final
                    }
                    else
                    {
                        discosSeleccionados.Add(opcion);
                    }
                }

                // Asigna las opciones de almacenamiento a las celdas de disco_1, disco_2, etc.
                for (int i = 0; i < discosSeleccionados.Count && i < 3; i++)
                {
                    fila.Cells[i + 5].Value = discosSeleccionados[i];
                }

                // Si hay más de 3 discos seleccionados, coloca la última opción en la celda 8
                if (discosSeleccionados.Count > 3)
                {
                    fila.Cells[8].Value = discosSeleccionados[3];
                }

                //--- Nombre del equipo
                string computerName = Environment.MachineName;

                fila.Cells[9].Value = Convert.ToString(computerName);

                // codigo Patrimonial
                string textoPersonalizado = textBox5.Text;
                fila.Cells[10].Value = textoPersonalizado;
                textBox5.Text = "";
                // codigo Impresora
                string Impresora= textBox6.Text;
                fila.Cells[11].Value = Impresora;
                textBox6.Text = "";
                //tipo window
                // Obtener el nombre y la versión del sistema operativo
                
                ComputerInfo computerInfo = new ComputerInfo();
                string sistemaOperativo = computerInfo.OSFullName;

                // Asignar el valor al TextBox (reemplaza 'textBox9' con el nombre de tu TextBox)
                fila.Cells[12].Value = sistemaOperativo;
                //Observaciones
                string Observacion = textBox8.Text;
                fila.Cells[13].Value = Observacion;
                textBox8.Text = "";

                dgv.Rows.Add(fila);
            }
            catch (Exception ex)
            {
                textBox2.Text = Convert.ToString("An error occurred: " + ex.Message);
            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }
    }
}