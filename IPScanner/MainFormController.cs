using System.Windows.Forms;
using IPScanner.Model;

namespace IPScanner.Controller
{
    // Clase que conecta la Vista (MainForm) con la lógica
    public class MainFormController
    {
        private TextBox txtIPInicio;
        private TextBox txtIPFin;
        private NumericUpDown numTiempoEspera;
        private Button btnEscanear;

        public MainFormController(TextBox txtIPInicio, TextBox txtIPFin, NumericUpDown numTiempoEspera, Button btnEscanear)
        {
            this.txtIPInicio = txtIPInicio;
            this.txtIPFin = txtIPFin;
            this.numTiempoEspera = numTiempoEspera;
            this.btnEscanear = btnEscanear;

            txtIPInicio.TextChanged += ValidarCampos; // TextChanged es un evento, y le estamos pasando ValidarCampos para un posterior uso de este
            txtIPFin.TextChanged += ValidarCampos;
            numTiempoEspera.ValueChanged += ValidarCampos;

            txtIPFin.Enabled = false; // Los deshabilito, por ahora
            numTiempoEspera.Enabled = false;
            btnEscanear.Enabled = false;
        }

        private void ValidarCampos(object sender, System.EventArgs e)
        {
            bool txtIPInicioValido = IPv4Validator.EsDireccionValida(txtIPInicio.Text);
            txtIPFin.Enabled = txtIPInicioValido;

            bool txtIPFinValido = IPv4Validator.EsDireccionValida(txtIPFin.Text);
            numTiempoEspera.Enabled = txtIPFinValido;

            bool numTiempoEsperaValido = numTiempoEspera.Value > 9 && numTiempoEspera.Value < 301;

            btnEscanear.Enabled = txtIPInicioValido && txtIPFinValido && numTiempoEsperaValido;
        }

        private async Task<string> RunCommandAsync(string command, string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            var output = new StringBuilder();

            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    output.AppendLine(e.Data);
                    AppendToConsole(e.Data); // Método que actualiza el TextBox/RichTextBox
                }
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    output.AppendLine(e.Data);
                    AppendToConsole(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.Run(() => process.WaitForExit());
            return output.ToString();
        }

        private async Task ScanRangeAsync(List<string> ips, int timeLimitSeconds)
        {
            var sw = Stopwatch.StartNew();
            var results = new List<ScanResult>();

            foreach (var ip in ips)
            {
                if (sw.Elapsed.TotalSeconds > timeLimitSeconds)
                {
                    AppendToConsole("Tiempo límite excedido. Cancelando...");
                    break;
                }

                AppendToConsole($"Escaneando {ip}...");

                // Ping
                string pingOutput = await RunCommandAsync("ping", $"-n 1 {ip}");
                bool pingOk = pingOutput.Contains("tiempo=") || pingOutput.Contains("time=");

                // nslookup
                string nsOutput = await RunCommandAsync("nslookup", ip);
                string hostName = ExtractHostName(nsOutput);

                // Tiempo de respuesta
                double? pingTime = ExtractPingTime(pingOutput);

                results.Add(new ScanResult
                {
                    IP = ip,
                    HostName = hostName,
                    PingOk = pingOk,
                    PingTime = pingTime
                });

                UpdateProgress(results.Count, ips.Count);
            }

            sw.Stop();
            ShowResults(results);
        }

    }
}
