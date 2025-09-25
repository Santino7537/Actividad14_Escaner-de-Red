using IPScanner.Controller;

namespace IPScanner.MnView
{
    // La vista principal.
    public class MainForm : Form
    {
        private readonly TextBox startIPField, endIPField;
        private readonly NumericUpDown timeOutField;
        private readonly Button scanBtn, cleanBtn, netScanBtn;

        public MainForm()
        {
            Text = "Escaneo de Red"; // Título de la vista.
            FormBorderStyle = FormBorderStyle.FixedSingle; // Impide redimensionar la ventana.
            MaximizeBox = false; // Deshabilita el botón de maximizar.
            Width = 400; Height = 250; // Dimensiones de la ventana.

            var layout = new FlowLayoutPanel // Diseño con "FlowLayoutPanel" (una columna).
            {
                Dock = DockStyle.Fill, // El panel ocupa todo el espacio posible.
                FlowDirection = FlowDirection.TopDown, // Los componentes se apilan de arriba hacia abajo.
                AutoScroll = true // Agrega scroll automático si los controles no caben en la ventana.
            };

            layout.Controls.Add(new Label // Agrega un "Label" al "FlowLayoutPanel".
            {
                Text = "Bienvenido, ingrese el rango de IPs que desee escanear",
                AutoSize = true // El tamaño del "Label" se ajusta automáticamente al texto.
            });

            layout.Controls.Add(new Label
            {
                Text = "IP Inicio:",
                AutoSize = true
            });

            startIPField = new TextBox { Width = 100 }; // Crea un "TextBox", que permite al usuario ingresar la IP inicial.
            layout.Controls.Add(startIPField);

            layout.Controls.Add(new Label
            {
                Text = "IP Fin:",
                AutoSize = true
            });

            endIPField = new TextBox { Width = 100 }; // Crea un "TextBox", que permite al usuario ingresar la IP final.
            layout.Controls.Add(endIPField);

            layout.Controls.Add(new Label
            {
                Text = "Tiempo límite (segundos):",
                AutoSize = true
            });

            timeOutField = new NumericUpDown // Crea un "NumericUpDown", que permite seleccionar un número dentro de un rango.
            {
                Width = 100,
                Minimum = 10, // Restringe los valores menores a 10.
                Maximum = 1800 // Restringe los valores mayores a 1800.
            };
            layout.Controls.Add(timeOutField);

            scanBtn = new Button // Crea un "Button", que permite al usuario precionarlo.
            {
                Text = "Escanear",
                Enabled = false // Por ahora el botón esta deshabilitado.
            };
            layout.Controls.Add(scanBtn);

            cleanBtn = new Button { Text = "Limpiar" };
            layout.Controls.Add(cleanBtn);

            netScanBtn = new Button { Text = "Ejecutar netScan" };
            layout.Controls.Add(netScanBtn);

            Controls.Add(layout); // Agrega el "FlowLayoutPanel" a la ventana.

            new MainController(this); // Crea el controlador de las vistas.
        }

        public TextBox GetStartIPField() { return startIPField; }
        public TextBox GetEndIPField() { return endIPField; }
        public NumericUpDown GetTimeOutField() { return timeOutField; }
        public Button GetScanBtn() { return scanBtn; }
        public Button GetCleanBtn() { return cleanBtn; }
        public Button GetNetScanBtn() { return netScanBtn; }

        public void SetStartIPFieldEnabled(bool enabled) { startIPField.Enabled = enabled; }
        public void SetEndIPFieldEnabled(bool enabled) { endIPField.Enabled = enabled; }
        public void SetTimeOutFieldEnabled(bool enabled) { timeOutField.Enabled = enabled; }
        public void SetScanBtnEnabled(bool enabled) { scanBtn.Enabled = enabled; }
        public void SetCleanBtnEnabled(bool enabled) { cleanBtn.Enabled = enabled; }
        public void SetNetScanBtnEnabled(bool enabled) { netScanBtn.Enabled = enabled; }
    }
}
