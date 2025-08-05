using System.Text.RegularExpressions;

namespace IPScanner.Model
{
    // Clase encargada de validar si una cadena tiene formato IPv4 correcto
    public class IPv4Validator
    {
        // Método público que valida una dirección IP
        public static bool EsDireccionValida(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

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
    }
}
