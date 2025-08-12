using System.Text.RegularExpressions;

namespace IPScanner.Model
{
    // Clase encargada de validar si una cadena tiene formato IPv4 correcto
    public class IPv4Validator
    {
        // Método público que valida una dirección IP
        public static bool EsDireccionValida(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) return false;

            if (ip.Contains(" ")) return false;

            // Divide la cadena en partes separadas por "."
            string[] partes = ip.Split('.');
            if (partes.Length != 4) return false;

            foreach (string parte in partes)
            {
                // Rechaza si no es número
                if (!int.TryParse(parte, out int numero)) return false;

                // Rechaza si no está entre 0 y 255
                if (numero < 0 || numero > 255) return false;

                // Rechaza ceros a la izquierda (excepto el número 0 solo)
                if (parte != "0" && parte.StartsWith("0")) return false;
            }
            return true;
        }

        private List<string> GetIPRange(string startIP, string endIP)
        {
            // Convertir las IPs a listas de enteros, separados por "."
            var start = startIP.Split('.').Select(int.Parse).ToList();
            var end = endIP.Split('.').Select(int.Parse).ToList();

            var ips = new List<string>();

            // IP actual como copia de la inicial
            var current = new List<int>(start);

            // Repetir hasta que la IP actual sea igual a la final
            do
            {
                ips.Add(string.Join(".", current));

                // Incrementar el último octeto
                current[3]++;

                // Si el último número es mayor a 255, se pone en 0, y al siguiente número se le suma 1, y se comprueba lo mismo hasta el número final
                if (current[3] > 255)
                {
                    current[3] = 0;
                    current[2]++;
                    if (current[2] > 255)
                    {
                        current[2] = 0;
                        current[1]++;
                        if (current[1] > 255)
                        {
                            current[1] = 0;
                            current[0]++;
                        }
                    }
                }
            } while (!current.SequenceEqual(end));

            return ips;
        }

    }
}
