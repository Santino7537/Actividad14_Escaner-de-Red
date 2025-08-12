namespace IPScanner.Result
{
    public class ScanResult
    {
        private string ip;
        private string host;
        private bool pingOk;
        private double? pingTiempo;

        public MainForm(string ip, string host, bool pingOk, double? pingTiempo)
        {
            this.ip = ip,
            this.host = host,
            this.pingOk = pingOk,
            this.pingTiempo = pingTiempo
        }
    }
}
