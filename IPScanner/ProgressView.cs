using System;
using System.Windows.Forms;

namespace IPScanner.View
{
    public partial class ProgressView : Form
    {
        // Controles declarados en el diseñador o código (RichTextBox y ProgressBar)
        private RichTextBox richTextBoxConsole;
        private ProgressBar progressBar;

        public ProgressView()
        {
            InitializeComponent();
            SetupControls();
        }

        private void SetupControls()
        {
            // Crear y configurar RichTextBox
            richTextBoxConsole = new RichTextBox
            {
                Dock = DockStyle.Top,
                Height = 300,
                ReadOnly = true,
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.LightGreen,
                Font = new System.Drawing.Font("Consolas", 10),
                WordWrap = false
            };
            this.Controls.Add(richTextBoxConsole);

            // Crear y configurar ProgressBar
            progressBar = new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 25,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };
            this.Controls.Add(progressBar);

            // Configuración general del Form
            this.Text = "Progreso de escaneo";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 600;
            this.Height = 400;
        }

        // Método para agregar texto a la consola de forma segura desde cualquier hilo
        public void AppendToConsole(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendToConsole), text);
            }
            else
            {
                richTextBoxConsole.AppendText(text + Environment.NewLine);
                richTextBoxConsole.ScrollToCaret();
            }
        }

        // Método para actualizar la barra de progreso de forma segura desde cualquier hilo
        public void UpdateProgress(int done, int total)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, int>(UpdateProgress), done, total);
            }
            else
            {
                if (total == 0) progressBar.Value = 0;
                else
                {
                    int porcentaje = (int)((done / (double)total) * 100);
                    progressBar.Value = Math.Min(Math.Max(porcentaje, 0), 100);
                }
            }
        }
    }
}
