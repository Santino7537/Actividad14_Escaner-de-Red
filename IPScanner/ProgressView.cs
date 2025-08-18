using IPScanner.Model;

namespace IPScanner.ProgView
{
    // La vista de progreso del escaneo de IPs.
    public partial class ProgressView : Form
    {
        private readonly Label percentageLbl, devicesRespondedLbl;
        private readonly ProgressBar progressBar;
        private readonly RichTextBox consoleBox;
        private readonly DataGridView dgvResults;
        private readonly Button stopBtn, saveBtn;
        private readonly ComboBox orderBox;
        private readonly BindingSource bindingSource;
        private readonly List<dynamic> scanResult = new List<dynamic>(); // lista interna para almacenar todos los datos obtenidos al escanear las IPs.
        private int devicesResponded = 0;

        public ProgressView()
        {
            Text = "Progreso de escaneo"; // Título de la vista.
            StartPosition = FormStartPosition.CenterScreen; // La ventana aparece centrada en la pantalla.
            FormBorderStyle = FormBorderStyle.FixedSingle; // Impide redimensionar la ventana.
            ControlBox = false; // Quita todos los botones de control de la ventana.
            Width = 900; Height = 600; // Dimensiones de la ventana.

            consoleBox = new RichTextBox // Crea un "RichTextBox", que permite al usuario ingresar/ver una gran cantidad de texto.
            {
                Dock = DockStyle.Top, // la consola ocupa la aprte superior de la vista.
                Height = 200,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 10),
                WordWrap = false
            };
            Controls.Add(consoleBox); // Agrega la consola a la vista.

            Panel progressPanel = new Panel // Crea un panel, que permite agrupar componentes en el.
            {
                Dock = DockStyle.Top,
                Height = 50
            };
            Controls.Add(progressPanel);

            progressBar = new ProgressBar // Crea una "ProgressBar", que permite al usuario ver una barra de progresión.
            {
                Location = new Point(10, 10), // Ubicación en el panel (10 en x, 10 en y).
                Width = 650,
                Height = 25,
                Minimum = 0, // Restringe los valores menores a 0.
                Maximum = 100, // Restringe los valores mayores a 100.
                Value = 0
            };
            progressPanel.Controls.Add(progressBar); // Agrega la barra al panel.

            percentageLbl = new Label
            {
                Location = new Point(670, 10),
                Width = 100,
                Text = "0%"
            };
            progressPanel.Controls.Add(percentageLbl);

            devicesRespondedLbl = new Label
            {
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Text = "Equipos que respondieron: 0"
            };
            Controls.Add(devicesRespondedLbl);

            Panel lowerPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(lowerPanel);

            Panel tablePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 700
            };
            lowerPanel.Controls.Add(tablePanel); // Agrega el "tablePanel" en el "lowerPanel".

            dgvResults = new DataGridView // Crea una "DataGridViwe", que permite al usuario usar/ver una grilla.
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, // Al seleccionar una celda, se selecciona su fila completa.
                MaximumSize = new Size(700, 0) // 700 píxeles de ancho como máximo, altura ilimitada.
            };

            dgvResults.Columns.Add(new DataGridViewTextBoxColumn // Crea una "DataGridViewTextBoxColumn", que permite agregar una columna al "DataGridView".
            {
                HeaderText = "Dirección IP",
                DataPropertyName = "IP", // Se asigna un nombre de propiedad, para luego trabajar con las columnas y relacionarlos con sus datos.
                SortMode = DataGridViewColumnSortMode.Automatic,
                Width = 100
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nombre del equipo",
                DataPropertyName = "Name",
                SortMode = DataGridViewColumnSortMode.Automatic,
                Width = 200
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Conectado",
                DataPropertyName = "Answer",
                SortMode = DataGridViewColumnSortMode.Automatic,
                Width = 150
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tiempo Real (ms)",
                DataPropertyName = "RealTime",
                SortMode = DataGridViewColumnSortMode.Automatic,
                Width = 110
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tiempo (ms)",
                DataPropertyName = "Time",
                SortMode = DataGridViewColumnSortMode.Automatic,
                Width = 80
            });

            bindingSource = new BindingSource(); // Crea un "BindingSource", que es un intermediario para filtrar y ordenar datos.
            dgvResults.DataSource = bindingSource; // Establece al "BindingSource" como la fuente de datos.

            tablePanel.Controls.Add(dgvResults);
            dgvResults.BringToFront(); // Hace que un componente se dibuje encima de los demás componentes que estén en la misma área del panel.

            TableLayoutPanel optionPanel = new TableLayoutPanel // Crea un "TableLayoutPanel", que permite establecer una especie de grilla para los componentes que almacenará.
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };
            optionPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Crea una "RowStyle", que permite representar una fila del "TableLayoutPanel".
            optionPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            optionPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            lowerPanel.Controls.Add(optionPanel);

            Label lblOrdenar = new Label
            {
                Text = "Ordenar/filtrar por:",
                AutoSize = true
            };
            optionPanel.Controls.Add(lblOrdenar, 0, 0); // Agrega el "Label" en la primera columna, primera fila, del "optionPanel".

            orderBox = new ComboBox // Crea una "ComboBox", que permite al usuario seleccionar una opción de las tantas que puede almacenar el "ComboBox".
            {
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList // Aclara que el "ComboBox" tendrá el estilo de un dropdown.
            };
            orderBox.Items.AddRange(new string[] // Agrega en una lista nuevas opciones para el "ComboBox".
            {
                "Ip ascendente",
                "Ip descendente",
                "Hubo respuesta",
                "Host Inaccesible",
                "No hubo respuesta",
                "Error, sin respuesta"
            });

            orderBox.SelectedIndex = 0; // Por defecto, se selecciona la primera opción del "ComboBox".
            optionPanel.Controls.Add(orderBox, 0, 1);

            TableLayoutPanel buttonPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2
            };
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            optionPanel.Controls.Add(buttonPanel, 0, 2);

            stopBtn = new Button
            {
                Text = "Parar",
                Width = 70,
                Height = 30
            };
            stopBtn.Anchor = AnchorStyles.Bottom;
            // Controla el comportamiento de un componente ante el cambio de tamaño de un panel, manteniendo una distancia constante entre un borde del panel y el mismo borde del componente.
            buttonPanel.Controls.Add(stopBtn, 0, 0);

            saveBtn = new Button
            {
                Text = "Guardar",
                Width = 70,
                Height = 30
            };
            saveBtn.Anchor = AnchorStyles.Bottom;
            buttonPanel.Controls.Add(saveBtn, 1, 0);

            buttonPanel.BringToFront();
            lowerPanel.BringToFront();
            tablePanel.BringToFront();
            optionPanel.BringToFront();
        }

        public void AppendToConsole(string text)
        {
            if (InvokeRequired) // El "InvokeRequired" es una propiedad de "Control" (padre de los componentes), que indica si el hilo actual no es el hilo de la interfaz gráfica.
                Invoke(new Action<string>(AppendToConsole), text); // "Invoke" ejecuta un delegado en el hilo al que pertenece. "Action" crea un delegado, que es un puntero a una función.
            else
            {
                consoleBox.AppendText(text + Environment.NewLine); // Agrega el texto al "consoleBox", y salta de línea.
                consoleBox.ScrollToCaret(); // Asegura que la vista haga scroll automáticamente hasta el final.
            }
        }

        public void UpdateProgress(int done, int total)
        {
            if (InvokeRequired)
                Invoke(new Action<int, int>(UpdateProgress), done, total);
            else
            {
                if (total == 0)
                {
                    progressBar.Value = 0;
                    percentageLbl.Text = "0%";
                }
                else
                {
                    int porcentaje = (int)((done / (double)total) * 100);
                    progressBar.Value = Math.Min(Math.Max(porcentaje, 0), 100); // Aseguramos que el valor si este entre 0 y 100.
                    percentageLbl.Text = $"{porcentaje}%";
                }
            }
        }

        public void AddResult(string ip, string name, string answer, string realTime, string time)
        {
            if (InvokeRequired)
                Invoke(new Action<string, string, string, string, string>(AddResult), ip, name, answer, realTime, time);
            else
            {
                var fila = new // Crea un objeto anónimo para meter los datos de escaneo en la tabla.
                {
                    IP = ip,
                    Name = name,
                    Answer = answer,
                    RealTime = realTime,
                    Time = time
                };

                scanResult.Add(fila);
                bindingSource.Add(fila);

                if (time != "N/A") devicesResponded++;

                devicesRespondedLbl.Text = $"Equipos que respondieron: {devicesResponded}";
            }
        }

        public void SortFilterTable(string sortBy, List<dynamic> data)
        {
            if (InvokeRequired)
                Invoke(new Action<string, List<dynamic>>(SortFilterTable), sortBy, data);
            else
            {
                bindingSource.Clear();

                IEnumerable<dynamic> sortedData = data; // "IEnumerable" es una interfaz que representa una colección de elementos que se pueden recorrer secuencialmente.

                if (sortBy == "Ip ascendente")
                {
                    sortedData = data.OrderBy(x => (string)x.IP, Comparer<string>.Create(IPv4Validator.CompareIPs));
                    // Ordena los objetos anónimos de "data" ascendentemente según el atributo "IP", utilizando un Comparer (comparador) creado a partir del método "CompareIPs".
                }
                else if (sortBy == "Ip descendente")
                {
                    sortedData = data.OrderByDescending(x => (string)x.IP, Comparer<string>.Create(IPv4Validator.CompareIPs));
                    // Ordena los objetos anónimos de "data" descendentemente según el atributo "IP", utilizando un Comparer (comparador) creado a partir del método "CompareIPs".
                }
                else if (sortBy == "Hubo respuesta")
                {
                    sortedData = data.Where(x => x.Answer == "Hubo respuesta");
                    // Filtra los objetos anónimos de "data" según el atributo "Answer", devolviendo solo aquellos cuyo atributo "Answer" sea exactamente igual a "Hubo respuesta".
                }
                else if (sortBy == "Host Inaccesible")
                {
                    sortedData = data.Where(x => x.Answer == "Host Inaccesible");
                    // Filtra los objetos anónimos de "data" según el atributo "Answer", devolviendo solo aquellos cuyo atributo "Answer" sea exactamente igual a "Host Inaccesible".
                }
                else if (sortBy == "No hubo respuesta")
                {
                    sortedData = data.Where(x => x.Answer == "No hubo respuesta");
                    // Filtra los objetos anónimos de "data" según el atributo "Answer", devolviendo solo aquellos cuyo atributo "Answer" sea exactamente igual a "No hubo respuesta".
                }
                else if (sortBy == "Error, sin respuesta")
                {
                    sortedData = data.Where(x => x.Answer == "Error, sin respuesta");
                    // Filtra los objetos anónimos de "data" según el atributo "Answer", devolviendo solo aquellos cuyo atributo "Answer" sea exactamente igual a "Error, sin respuesta".
                }

                foreach (var fila in sortedData)
                {
                    bindingSource.Add(fila); // Agregamos los datos ordenados/filtrados al "bindingSource".
                }
            }
        }

        public void SaveScanResults()
        {
            SaveFileDialog sfd = new SaveFileDialog // Ventana para guardar archivos.
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Guardar resultados del escaneo"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var writer = new StreamWriter(sfd.FileName))
                    {
                        // Escribir encabezados.
                        writer.WriteLine("IP,Name,Answer,RealTime,Time");

                        // Escribir cada fila.
                        foreach (var fila in scanResult)
                        {
                            string line = $"{fila.IP},{fila.Name},{fila.Answer},{fila.RealTime},{fila.Time}";
                            writer.WriteLine(line);
                        }
                    }
                    MessageBox.Show("Archivo guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Error al abrir ventana.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void CleanAll()
        {
            if (InvokeRequired)
                Invoke(new Action(CleanAll));
            else
            {
                orderBox.SelectedIndex = 0;

                progressBar.Value = 0;
                percentageLbl.Text = "0%";
                devicesResponded = 0;

                consoleBox.Clear();
                scanResult.Clear();
                bindingSource.Clear();

                devicesRespondedLbl.Text = "Equipos que respondieron: 0";

                SetOrderBoxEnabled(false);
                SetSaveBtnEnabled(false);
            }
        }

        public Label GetPercentageLbl() { return percentageLbl; }
        public Label GetDevicesRespondedLbl() { return devicesRespondedLbl; }
        public ProgressBar GetProgressBar() { return progressBar; }
        public RichTextBox GetConsoleBox() { return consoleBox; }
        public DataGridView GetDgvResults() { return dgvResults; }
        public Button GetStopBtn() { return stopBtn; }
        public Button GetSaveBtn() { return saveBtn; }
        public ComboBox GetOrderBox() { return orderBox; }
        public BindingSource GetBindingSource() { return bindingSource; }
        public List<dynamic> GetScanResult() { return scanResult; }
        public int GetDevicesResponded() { return devicesResponded; }

        public void SetStopBtnEnabled(bool enabled) { stopBtn.Enabled = enabled; }
        public void SetSaveBtnEnabled(bool enabled) { saveBtn.Enabled = enabled; }
        public void SetOrderBoxEnabled(bool enabled) { orderBox.Enabled = enabled; }
    }
}
