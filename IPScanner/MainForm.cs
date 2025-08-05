using System.Windows.Forms;
using IPScanner.Controller;

namespace IPScanner.View
{
    // La vista principal, hereda de Form
    public class MainForm : Form
    {
        private TextBox txtIPInicio;
        private TextBox txtIPFin;
        private NumericUpDown numTiempoEspera;
        private Button btnEscanear;
        private Button btnLimpiar;

        public MainForm()
        {
            this.Text = "Escaneo de Red"; //Título de ventana
            this.Width = 400;
            this.Height = 250;

            var layout = new FlowLayoutPanel(); //Diseño con FlowLayoutPanel (una columna)
            layout.Dock = DockStyle.Fill;
            layout.FlowDirection = FlowDirection.TopDown;
            layout.AutoScroll = true;

            layout.Controls.Add(new Label { Text = "Bienvenido, ingrese el rango de IPs que desee escanear", AutoSize = true });

            // Campo 1
            layout.Controls.Add(new Label { Text = "IP Inicio:", AutoSize = true });
            txtIPInicio = new TextBox { Width = 100 };
            layout.Controls.Add(txtIPInicio);

            // Campo 2
            layout.Controls.Add(new Label { Text = "IP Fin:", AutoSize = true });
            txtIPFin = new TextBox { Width = 100 };
            layout.Controls.Add(txtIPFin);

            // Campo 3 (número)
            layout.Controls.Add(new Label { Text = "Tiempo límite (segundos):", AutoSize = true });
            numTiempoEspera = new NumericUpDown { Width = 100, Minimum = 10, Maximum = 300 };
            layout.Controls.Add(numTiempoEspera);

            // Botones
            btnEscanear = new Button { Text = "Escanear", Enabled = false };
            btnLimpiar = new Button { Text = "Limpiar" };
            layout.Controls.Add(btnEscanear);
            layout.Controls.Add(btnLimpiar);

            this.Controls.Add(layout);

            // Controlador que se encarga de las validaciones
            var controller = new MainFormController(txtIPInicio, txtIPFin, numTiempoEspera, btnEscanear);
        }
    }
}
