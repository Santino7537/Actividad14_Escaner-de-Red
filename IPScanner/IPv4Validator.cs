using IPScanner.IPRangeException;

namespace IPScanner.Model
{
    // Clase encargada de validar si una cadena tiene formato IPv4.
    public class IPv4Validator
    {
        public static bool IsValid(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) return false;

            if (ip.Contains(' ')) return false;

            // Divide la cadena en partes separadas por ".".
            string[] slicedIP = ip.Split('.');
            if (slicedIP.Length != 4) return false;

            foreach (string octet in slicedIP)
            {
                // Rechaza si no es un número.
                if (!int.TryParse(octet, out int intOctet)) return false;

                // Rechaza si su valor no está entre 0 y 255.
                if (intOctet < 0 || intOctet > 255) return false;

                // Rechaza ceros a la izquierda (excepto el número 0 solo).
                if (octet != "0" && octet.StartsWith('0')) return false;
            }
            return true;
        }

        public static bool IPLessOrEqual(string minorIP, string majorIP)
        {
            if (!IsValid(minorIP) || !IsValid(majorIP)) return false;

            var slicedMinorIP = minorIP.Split('.').Select(int.Parse).ToArray(); // Divide la cadena en partes separadas por ".", y convierte cada parte en "int".
            var slicedMajorIP = majorIP.Split('.').Select(int.Parse).ToArray();

            for (int i = 0; i < 4; i++) // Se compara cada octeto para saber cual IP es más grande.
            {
                if (slicedMinorIP[i] < slicedMajorIP[i]) return true;
                if (slicedMinorIP[i] > slicedMajorIP[i]) return false;
            }

            return true; // Todos los octetos son iguales.
        }

        public static int CompareIPs(string ip1, string ip2)
        {
            var slicedIP1 = ip1.Split('.').Select(int.Parse).ToArray();
            var slicedIP2 = ip2.Split('.').Select(int.Parse).ToArray();

            for (int i = 0; i < 4; i++)
            {
                int cmp = slicedIP1[i].CompareTo(slicedIP2[i]); // Si es menor, devuelve algo menor a 0, si es igual, devuelve 0, y si es mayor, devuelve algo mayor a 0.
                if (cmp != 0) return cmp;
            }
            return 0;
        }

        public static List<string> GetIPRange(string startIP, string endIP)
        {
            var slicedStartIP = startIP.Split('.').Select(int.Parse).ToList();
            var slicedEndIP = endIP.Split('.').Select(int.Parse).ToList();

            int[] octetMultipliers = [16777216, 65536, 256, 1]; // El multiplicador de cada octeto, usado para contar las vueltas antes de hacer demasiadas.
            int IPRangeLength = 0;

            for (int i = 0; i < 4; i++)
            {
                IPRangeLength += Math.Abs(slicedEndIP[i] - slicedStartIP[i]) * octetMultipliers[i]; // Se cuenta la cantidad de vueltas necesarias para hacer el rango por cada octeto.
                if (IPRangeLength > 5000)
                {
                    throw new IPRangeOutOfBoundsException("El rango de IPs que solicita es muy grande.");
                }
            }


            // IP actual como copia de la inicial.
            var currentIP = new List<int>(slicedStartIP);
            var IPRange = new List<string>() { string.Join(".", currentIP) };

            // Repetir hasta que la IP actual sea igual a la final.
            while (!currentIP.SequenceEqual(slicedEndIP))
            {
                // Incrementar el último octeto.
                currentIP[3]++;

                // Si el octeto actual es mayor a 255, cambia su valor a 0, y al siguiente octeto se le suma 1, y se comprueba lo mismo hasta llegar al primer octeto.
                if (currentIP[3] > 255)
                {
                    currentIP[3] = 0;
                    currentIP[2]++;
                    if (currentIP[2] > 255)
                    {
                        currentIP[2] = 0;
                        currentIP[1]++;
                        if (currentIP[1] > 255)
                        {
                            currentIP[1] = 0;
                            currentIP[0]++;
                        }
                    }
                }
                IPRange.Add(string.Join(".", currentIP));
            }

            return IPRange;
        }

    }
}
