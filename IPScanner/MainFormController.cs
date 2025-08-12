using System.Windows.Forms;
using IPScanner.Model;
using System.Diagnostics;
using System.Text;

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

        private async Task<string> RunCommandAsync(string command, string args) // Devuelve un Task<string> por ser un proceso asíncrono.
        {
            var proceso = new Process // Permite iniciar y controlar procesos externos.
            {
                configuracionInicial = new ProcessStartInfo // Configura cómo se va a iniciar el proceso.
                {
                    FileName = command, // Comando a ejecutar.
                    Arguments = args, // Parámetros para el comando.
                    RedirectStandardOutput = true, // Permiten capturar lo que el proceso escriba en la consola.
                    RedirectStandardError = true, // Permiten capturar lo que el proceso escriba en el flujo de errores.
                    UseShellExecute = false, // Evita que se use el shell de Windows por defecto, ya que no es necesario.
                    CreateNoWindow = true // Evita que aparezca una ventana de consola.
                },
                EnableRaisingEvents = true // Permite que el proceso genere eventos, como "Exited" cuando termina.
            };

            var salidaComando = new StringBuilder(); // Clase de System.Text usada para construir texto de manera eficiente sin crear muchos Strings nuevos.

            proceso.OutputDataReceived += (disparador, texto) => // Evento que se dispara cada vez que el proceso genera una línea de salida.
            {
                if (texto.Data != null)
                {
                    salidaComando.AppendLine(texto.Data);
                    AppendToConsole(texto.Data); // Método que actualiza el TextBox/RichTextBox
                }
            };
            proceso.ErrorDataReceived += (disparador, texto) =>
            {
                if (texto.Data != null)
                {
                    salidaComando.AppendLine(texto.Data);
                    AppendToConsole(texto.Data);
                }
            };

            proceso.Start();
            proceso.BeginOutputReadLine(); // Comienzan la lectura asíncrona de las salidas.
            proceso.BeginErrorReadLine(); // Comienzan la lectura asíncrona de los errores.

            await Task.Run(() => proceso.WaitForExit()); // Espera hasta que el proceso termine, envuelto en Task.Run para no bloquear la interfaz.
            return salidaComando.ToString(); // Se devuelve el texto acumulado como un String.
        }

        private async Task ScanRangeAsync(List<string> ips, int limiteTiempo)
        {
            var cronometro = Stopwatch.StartNew(); // Inicia un cronómetro para medir cuánto tiempo ha pasado.
            var resultado = new List<ScanResult>();

            foreach (var ip in ips)
            {
                if (cronometro.Elapsed.TotalSeconds > limiteTiempo)
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

                resultado.Add(new ScanResult
                {
                    IP = ip,
                    HostName = hostName,
                    PingOk = pingOk,
                    PingTime = pingTime
                });

                UpdateProgress(resultado.Count, ips.Count);
            }

            cronometro.Stop();
            ShowResults(resultado);
        }

    }
}
