using IPScanner.Comparator;
using IPScanner.ConsoleI;

namespace IPScanner.netStView
{
    public class NetStatView : Form, ConsoleInterface
    {
        private readonly RichTextBox consoleBox;
        private readonly DataGridView dgvResults;
        private readonly ComboBox orderBox;
        private readonly BindingSource bindingSource;
        private readonly Button netStatBtn;
        private readonly CheckBox flagABtn, flagNBtn, flagOBtn;
        private readonly List<dynamic> scanResult = new List<dynamic>(); // lista interna para almacenar todos los datos obtenidos al escanear las IPs.

        public NetStatView()
        {
            Text = "NetStat"; // Título de la vista.
            StartPosition = FormStartPosition.CenterScreen; // La ventana aparece centrada en la pantalla.
            FormBorderStyle = FormBorderStyle.FixedSingle; // Impide redimensionar la ventana.
            ControlBox = false; // Quita todos los botones de control de la ventana.
            Width = 900; Height = 600; // Dimensiones de la ventana.

            flagABtn = new CheckBox
            {
                Text = "Agregar -a",
                AutoSize = true,
                Size = new Size(80, 30), // Tamaño
                Location = new Point(10, 0) // Establece la ubicación
            };
            Controls.Add(flagABtn);

            flagNBtn = new CheckBox
            {
                Text = "Agregar -n",
                AutoSize = true,
                Size = new Size(80, 30),
                Location = new Point(100, 0)
            };
            Controls.Add(flagNBtn);

            flagOBtn = new CheckBox
            {
                Text = "Agregar -o",
                AutoSize = true,
                Size = new Size(80, 30),
                Location = new Point(190, 0)
            };
            Controls.Add(flagOBtn);

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
                HeaderText = "Protocolo",
                DataPropertyName = "Protocolo", // Se asigna un nombre de propiedad, para luego trabajar con las columnas y relacionarlos con sus datos.
                SortMode = DataGridViewColumnSortMode.Automatic,
                Width = 70
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Dirección Local",
                DataPropertyName = "IP_Local",
                SortMode = DataGridViewColumnSortMode.Automatic,
                Width = 190
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Dirección Remota",
                DataPropertyName = "IP_Remota",
                SortMode = DataGridViewColumnSortMode.Automatic,
                Width = 190
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Estado",
                DataPropertyName = "Estado",
                SortMode = DataGridViewColumnSortMode.Automatic,
                Width = 110
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "PID",
                DataPropertyName = "PID",
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
                Width = 170,
                DropDownStyle = ComboBoxStyle.DropDownList // Aclara que el "ComboBox" tendrá el estilo de un dropdown.
            };
            orderBox.Items.AddRange(new string[] // Agrega en una lista nuevas opciones para el "ComboBox".
            {
                "Dirección Local Ascendente",
                "Dirección Local Descendente",
                "Dirección Remota Ascendente",
                "Dirección Remota Descendente",
                "PID Ascendente",
                "PID Descendente",
                "LISTENING",
                "ESTABLISHED",
                "CLOSE_WAIT",
                "TIME_WAIT"
            });

            orderBox.SelectedIndex = 0; // Por defecto, se selecciona la primera opción del "ComboBox".
            optionPanel.Controls.Add(orderBox, 0, 1);

            Label lblNetStat = new Label
            {
                Text = "Ejecutar netstat:",
                AutoSize = true
            };
            optionPanel.Controls.Add(lblNetStat, 0, 2);

            netStatBtn = new Button
            {
                Text = "netstat",
                Width = 60,
                Height = 24
            };
            optionPanel.Controls.Add(netStatBtn, 0, 3);

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

        public void AddResult(string protocolo, string ip_local, string ip_remota, string estado, string pid)
        {
            if (InvokeRequired)
                Invoke(new Action<string, string, string, string, string>(AddResult), protocolo, ip_local, ip_remota, estado, pid);
            else
            {
                var fila = new // Crea un objeto anónimo para meter los datos de escaneo en la tabla.
                {
                    Protocolo = protocolo,
                    IP_Local = ip_local,
                    IP_Remota = ip_remota,
                    Estado = estado,
                    PID = pid
                };

                scanResult.Add(fila);
                bindingSource.Add(fila);
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

                if (sortBy == "Dirección Local Ascendente")
                {
                    sortedData = data.OrderBy(x => (string)x.IP_Local, Comparer<string>.Create(IPComparator.Compare));
                    // Ordena los objetos anónimos de "data" ascendentemente según el atributo "Dirección Local Ascendente", utilizando un Comparer (comparador) creado a partir del método "Compare".
                }
                else if (sortBy == "Dirección Local Descendente")
                {
                    sortedData = data.OrderByDescending(x => (string)x.IP_Local, Comparer<string>.Create(IPComparator.Compare));
                    // Ordena los objetos anónimos de "data" descendentemente según el atributo "Dirección Local Descendente", utilizando un Comparer (comparador) creado a partir del método "Compare".
                }
                else if (sortBy == "Dirección Remota Ascendente")
                {
                    sortedData = data.OrderBy(x => (string)x.IP_Remota, Comparer<string>.Create(IPComparator.Compare));
                }
                else if (sortBy == "Dirección Remota Descendente")
                {
                    Console.WriteLine(data[0].IP_Remota);
                    sortedData = data.OrderByDescending(x => (string)x.IP_Remota, Comparer<string>.Create(IPComparator.Compare));
                }
                else if (sortBy == "PID Ascendente")
                {
                    sortedData = data.OrderBy(x => int.TryParse(x.PID, out int n) ? n : int.MaxValue).ToList();
                }
                else if (sortBy == "PID Descendente")
                {
                    sortedData = data.OrderByDescending(x => int.TryParse(x.PID, out int n) ? n : int.MaxValue).ToList();
                }
                else if (sortBy == "LISTENING")
                {
                    sortedData = data.Where(x => x.Estado == "LISTENING");
                    // Filtra los objetos anónimos de "data" según el atributo "estado", devolviendo solo aquellos cuyo atributo "estado" sea exactamente igual a "LISTENING".
                }
                else if (sortBy == "ESTABLISHED")
                {
                    sortedData = data.Where(x => x.Estado == "ESTABLISHED");
                }
                else if (sortBy == "CLOSE_WAIT")
                {
                    sortedData = data.Where(x => x.Estado == "CLOSE_WAIT");
                }
                else if (sortBy == "TIME_WAIT")
                {
                    sortedData = data.Where(x => x.Estado == "TIME_WAIT");
                }

                foreach (var fila in sortedData)
                {
                    bindingSource.Add(fila); // Agregamos los datos ordenados/filtrados al "bindingSource".
                }
            }
        }

        public void CleanAll()
        {
            if (InvokeRequired)
                Invoke(new Action(CleanAll));
            else
            {
                orderBox.SelectedIndex = 0;

                consoleBox.Clear();
                scanResult.Clear();
                bindingSource.Clear();

                SetOrderBoxEnabled(false);
            }
        }

        public RichTextBox GetConsoleBox() { return consoleBox; }
        public DataGridView GetDgvResults() { return dgvResults; }
        public ComboBox GetOrderBox() { return orderBox; }
        public Button GetNetStatBtn() { return netStatBtn; }
        public BindingSource GetBindingSource() { return bindingSource; }
        public List<dynamic> GetScanResult() { return scanResult; }
        public CheckBox GetFlagABtn() { return flagABtn; }
        public CheckBox GetFlagNBtn() { return flagNBtn; }
        public CheckBox GetFlagOBtn() { return flagOBtn; }

        public void SetOrderBoxEnabled(bool enabled) { orderBox.Enabled = enabled; }
        public void SetNetStatBtnEnabled(bool enabled) { netStatBtn.Enabled = enabled; }
    }
}