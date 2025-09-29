using IPScanner.Model;
using IPScanner.MnView;
using IPScanner.IPRangeException;
using IPScanner.ProgView;
using IPScanner.netScnView;

using System.Diagnostics;
using System.Text;

namespace IPScanner.Controller
{
    // Clase que conecta las vistas (MainForm, ProgressView) con la lógica de la aplicación.
    public class MainController
    {
        private readonly MainForm mainForm; // Se contiene la vista principal, y se hace readonly para evitar reescrituras.
        private readonly ProgressView progressView; // Se contiene la vista de progreso.
        private readonly NetScanView netScanView; // Se contiene la vista para ejecutar comandos "netscan".
        private CancellationTokenSource? cts; // "CancellationTokenSource" se usa para controlar la cancelación de procesos asíncronos de manera segura y coordinada.

        public MainController(MainForm mnForm)
        {
            mainForm = mnForm;
            progressView = new ProgressView(); // Crea la vista de progreso.
            netScanView = new NetScanView(); // Crea la vista de "netscan".

            mainForm.GetStartIPField().TextChanged += ValidateIPs; // "TextChanged" es un evento, y le estamos enviando (suscribir) el método "ValidateIPs", para cuando se ejecute el evento, este use el método.
            mainForm.GetEndIPField().TextChanged += ValidateIPs;

            mainForm.GetScanBtn().Click += CleanProgressView; // "Click" es un evento, y le estamos enviando el método "CleanProgressView", para cuando se ejecute el evento, este use el método.
            mainForm.GetScanBtn().Click += ScanIPs;
            mainForm.GetCleanBtn().Click += CleanProgressView;
            mainForm.GetNetScanBtn().Click += OpenNetScan;

            progressView.GetStopBtn().Click += (s, e) => { if (cts != null && !cts.IsCancellationRequested) cts.Cancel(); }; // Si "cts" no esta cancelado y existe, se cancela (para terminar con el hilo de escaneo).
            progressView.GetSaveBtn().Click += (s, e) => { progressView.SaveScanResults(); };
            progressView.GetOrderBox().SelectedIndexChanged += (s, e) => { progressView.SortFilterTable(progressView.GetOrderBox().SelectedItem.ToString(), progressView.GetScanResult()); };
            netScanView.GetNetScanBtn().Click += ExecuteNetScan;
            netScanView.GetOrderBox().SelectedIndexChanged += (s, e) => { netScanView.SortFilterTable(netScanView.GetOrderBox().SelectedItem.ToString(), netScanView.GetScanResult()); };
            // Se reordena/filtra la tabla cada vez que se cambia de opción.

            mainForm.SetEndIPFieldEnabled(false); // Los deshabilito, por ahora.
            mainForm.SetTimeOutFieldEnabled(false);
            mainForm.SetScanBtnEnabled(false);
            mainForm.SetCleanBtnEnabled(false);
        }

        private void ValidateIPs(object? sender, System.EventArgs e)
        {
            bool txtIPInicioValido = IPv4Validator.IsValid(mainForm.GetStartIPField().Text);
            mainForm.SetEndIPFieldEnabled(txtIPInicioValido);

            bool txtIPFinValido = IPv4Validator.IsValid(mainForm.GetEndIPField().Text) && IPv4Validator.IPLessOrEqual(mainForm.GetStartIPField().Text, mainForm.GetEndIPField().Text);
            mainForm.SetTimeOutFieldEnabled(txtIPFinValido);

            mainForm.SetScanBtnEnabled(txtIPInicioValido && txtIPFinValido); // Valida el formato de las IPs ingresadas, y si el rango es correcto para dejar escanearlo.
        }

        private void CleanProgressView(object? sender, System.EventArgs e)
        {
            progressView.CleanAll();
            mainForm.SetCleanBtnEnabled(false);
        }

        private void OpenNetScan(object? sender, System.EventArgs e)
        {
            if (!netScanView.Visible)
            {
                netScanView.Show();
            }
            else {
                netScanView.Hide();
            }
        }

        private async void ScanIPs(object? sender, System.EventArgs e)
        {
            List<string> IPRange;
            try
            {
                IPRange = IPv4Validator.GetIPRange(mainForm.GetStartIPField().Text, mainForm.GetEndIPField().Text);
            }
            catch (IPRangeOutOfBoundsException ex)
            {
                MessageBox.Show(
                    ex.Message, // Se muestra el texto de error.
                    "Error", // Título de la ventana de error.
                    MessageBoxButtons.OK, // Botón (en este caso es el de "OK").
                    MessageBoxIcon.Error // Icono de Error.
                );
                return;
            }
            mainForm.SetScanBtnEnabled(false);
            mainForm.Enabled = false;

            // Muestra la vista de progreso.
            progressView.Show();
            progressView.SetStopBtnEnabled(true);

            await ScanRangeAsync(IPRange, (int)mainForm.GetTimeOutField().Value); // Cuando se termine de escanear el rango, se sigue ejecutando el "ScanIPs".

            mainForm.Enabled = true;
            mainForm.SetScanBtnEnabled(true);
            mainForm.SetCleanBtnEnabled(true);

            progressView.SetOrderBoxEnabled(true);
            progressView.SetSaveBtnEnabled(true);
            progressView.SetStopBtnEnabled(false);
        }

        private async Task ScanRangeAsync(List<string> ips, int timeOut) // Devuelve un "Task", que representa un resultado futuro de una operación que puede ejecutarse de forma asíncrona.
        {
            cts = new CancellationTokenSource(); // Se crea un nuevo "CancellationTokenSource" para reemplazar el viejo (por si hubo una cancelación anterior).
            var token = cts.Token; // "token" es un objeto de tipo "CancellationToken", que se pasa a métodos asincrónicos o "Tasks" para que sepan que pueden ser cancelados.

            var startScan = Stopwatch.StartNew(); // Inicia un cronómetro para medir cuánto tiempo ha pasado desde que inició el escaneo.

            for (int i = 0; i < ips.Count; i++)
            {
                if (token.IsCancellationRequested) // Si se canceló, se detiene el escaneo.
                {
                    progressView.AppendToConsole("Escaneo detenido por el usuario.");
                    break;
                }
                if (startScan.Elapsed.TotalSeconds > timeOut) // Si el tiempo transcurrido desde que inició el escaneo es mayor al solicitado por el usuario, se detiene el escaneo.
                {
                    progressView.AppendToConsole("Tiempo límite excedido. Escaneo cancelado.");
                    break;
                }

                string ip = ips[i];

                progressView.AppendToConsole($"Escaneando {ip}...");

                var realPingTime = Stopwatch.StartNew(); // Con este contador se va a tomar cuanto tarda verdaderamente en ejecutarse el comando "ping".
                string pingOutput = await RunCommandAsync("ping", $"-n 1 {ip}");
                realPingTime.Stop();

                string pingResponse = PingResponse(pingOutput);

                string nsOutput = await RunCommandAsync("nslookup", ip);
                string hostName = ExtractHostName(nsOutput);

                string pingTime = ExtractPingTime(pingOutput); // Se obtiene el tiempo que el comando "ping" midió.

                progressView.UpdateProgress(i + 1, ips.Count);
                progressView.AddResult(ip, hostName, pingResponse, $"{realPingTime.ElapsedMilliseconds}ms", pingTime);
            }
            startScan.Stop();
        }

        private async Task<string> RunCommandAsync(string command, string args)
        {
            var process = new Process // Permite iniciar y controlar procesos externos.
            {
                StartInfo = new ProcessStartInfo // Configura cómo se va a iniciar el proceso.
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

            var commandOutput = new StringBuilder(); // Clase de System.Text usada para construir texto de manera eficiente sin crear muchos Strings nuevos.

            process.OutputDataReceived += (trigger, text) => // Evento que se dispara cada vez que el proceso genera una línea de salida.
            {
                if (text.Data != null)
                {
                    commandOutput.AppendLine(text.Data);
                    progressView.AppendToConsole(text.Data);
                }
            };
            process.ErrorDataReceived += (trigger, text) => // Evento que se dispara cada vez que el proceso genera un error.
            {
                if (text.Data != null)
                {
                    commandOutput.AppendLine(text.Data);
                    progressView.AppendToConsole(text.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine(); // Comienzan la lectura asíncrona de las salidas.
            process.BeginErrorReadLine(); // Comienzan la lectura asíncrona de los errores.

            await Task.Run(() => process.WaitForExit()); // Espera hasta que el proceso termine, envuelto en "Task.Run" para no bloquear el hilo actual.
            return commandOutput.ToString(); // Se devuelve el texto acumulado como un String.
        }

        private static string ExtractHostName(string nsOutput)
        {
            string host;
            if (nsOutput.Contains("Nombre:"))
            {
                string slicedHost = nsOutput.Split("Nombre:")[1].Trim(); // Obtiene el host que indica "Nombre" en el comando "nslookup".
                host = slicedHost.Substring(0, slicedHost.IndexOf('\n')).Trim(); // Manipula un poco más "slicedHost".
            }
            else
            {
                host = "No se encontró el nombre";
            }
            return host;
        }

        private static string ExtractPingTime(string pingOutput)
        {
            string time;
            if (pingOutput.Contains("tiempo="))
            {
                string slicedTime = pingOutput.Split("tiempo=")[1].Trim(); // Obtiene la cantidad de milisegundos que indica "tiempo" en el comando "ping".
                time = slicedTime.Substring(0, slicedTime.IndexOf('m')) + "ms"; // Manipula un poco más "slicedTime", y le agrega "ms" al final.
            }
            else if (pingOutput.Contains("tiempo<"))
            {
                time = "<1ms"; // Es la única respuesta posible cuando "tiempo" está acompañado de un "<" a su lado en el comando "ping".
            }
            else
            {
                time = "N/A";
            }
            return time;
        }

        private static string PingResponse(string pingOutput)
        {
            string answer;

            if (pingOutput.Contains("Tiempos aproximados de ida y vuelta en milisegundos:"))
            {
                answer = "Hubo respuesta";
            }
            else if (pingOutput.Contains("Host de destino inaccesible"))
            {
                answer = "Host Inaccesible";
            }
            else if (pingOutput.Contains("Tiempo de espera agotado para esta solicitud"))
            {
                answer = "No hubo respuesta";
            }
            else
            {
                answer = "Error, sin respuesta";
            }
            return answer;
        }

        private async void ExecuteNetScan(object? sender, System.EventArgs e) {
            netScanView.AppendToConsole("Escaneando red...");

            string joinedFlags = "-";
            if (netScanView.GetFlagABtn().Checked) joinedFlags += "a";
            if (netScanView.GetFlagNBtn().Checked) joinedFlags += "n";
            if (netScanView.GetFlagOBtn().Checked) joinedFlags += "o";

            string netscanOutput = await RunCommandAsync("netscan", $"{joinedFlags}");

            Console.Write(netscanOutput);

            // progressView.AddResult(ip, hostName, pingResponse, $"{realPingTime.ElapsedMilliseconds}ms", pingTime);
        }
    }
}
