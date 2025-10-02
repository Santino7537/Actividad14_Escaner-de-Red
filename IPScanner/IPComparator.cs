namespace IPScanner.Comparator
{
    // Clase encargada de comprarar dos IPs.
    public class IPComparator
    {
        // Diccionario de prioridades. Menor número es mayor prioridad.

        public static int Compare(string ip1, string ip2)
        {
            // "*:*" siempre al final
            if (ip1.Contains("*:*") && ip2.Contains("*:*")) return 0;
            if (ip1.Contains("*:*")) return 1;
            if (ip2.Contains("*:*")) return -1;

            string type1 = IPType(ip1);
            string type2 = IPType(ip2);

            if (type1 == "IPV4" && type2 == "IPV4")
                return CompareIPV4s(ip1, ip2);

            if (type1 == "IPV6" && type2 == "IPV6")
                return CompareIPV6s(IPV6Fixed(ip1), IPV6Fixed(ip2));

            // IPv4 < IPv6
            return type1 == "IPV4" ? -1 : 1;
        }

        private static string IPType(string ip)
        {
            if (ip.Count(c => c == '.') == 3 && ip.Contains(':'))
            {
                return "IPV4";
            }
            else //(ip.Count(c => c == ':') >= 3 && ip.Contains('['))
            {
                return "IPV6";
            }
        }

        private static string IPV6Fixed(string ipv6Completa)
        {
            Console.WriteLine(ipv6Completa);
            int inicio = ipv6Completa.IndexOf('[') + 1; // posición después del '['
            int fin = ipv6Completa.IndexOf(']');        // posición del ']'

            string ipv6 = ipv6Completa.Substring(inicio, fin - inicio);

            // Separar la interfaz si existe
            string interfaz = "";
            int idxInterfaz = ipv6.IndexOf('%');
            if (idxInterfaz != -1)
            {
                interfaz = ipv6.Substring(idxInterfaz); // ej: %9
                ipv6 = ipv6.Substring(0, idxInterfaz);  // dirección sin interfaz
            }

            if (ipv6 == "::") return $"[0000:0000:0000:0000:0000:0000:0000:0000{interfaz}]{ipv6Completa.Substring(fin + 1)}";

            string[] bloques = new string[8];

            if (ipv6.Contains("::"))
            {
                string[] partes = ipv6.Split("::");

                string[] izquierda = partes[0].Split(':', StringSplitOptions.RemoveEmptyEntries);
                string[] derecha = partes.Length > 1 ? partes[1].Split(':', StringSplitOptions.RemoveEmptyEntries) : new string[0];

                int cerosFaltantes = 8 - (izquierda.Length + derecha.Length);

                int idx = 0;

                foreach (var bloque in izquierda) { bloques[idx++] = bloque.PadLeft(4, '0'); }

                for (int i = 0; i < cerosFaltantes; i++) { bloques[idx++] = "0000"; }

                foreach (var bloque in derecha) { bloques[idx++] = bloque.PadLeft(4, '0'); }
            }
            else
            {
                string[] partes = ipv6.Split(':');
                for (int i = 0; i < 8; i++) { bloques[i] = partes[i].PadLeft(4, '0'); }
            }

            // Reagregar la interfaz al final del último bloque
            bloques[7] += interfaz;

            return $"[{string.Join(':', bloques)}]{ipv6Completa.Substring(fin + 1)}";
        }

        private static int CompareIPV4s(string ip1, string ip2)
        {
            var splitedPortIP1 = ip1.Split(':');
            var splitedPortIP2 = ip2.Split(':');
            var slicedIP1 = splitedPortIP1[0].Split('.').Select(int.Parse).ToArray();
            var slicedIP2 = splitedPortIP2[0].Split('.').Select(int.Parse).ToArray();

            for (int i = 0; i < 4; i++)
            {
                int cmp = slicedIP1[i].CompareTo(slicedIP2[i]); // Si es menor, devuelve algo menor a 0, si es igual, devuelve 0, y si es mayor, devuelve algo mayor a 0.
                if (cmp != 0) return cmp;
            }
            return int.Parse(splitedPortIP1[1]).CompareTo(int.Parse(splitedPortIP2[1]));
        }

        private static int CompareIPV6s(string ip1Completa, string ip2Completa)
        {
            int inicio = ip1Completa.IndexOf('[') + 1; // posición después del '['
            int fin = ip1Completa.IndexOf(']');        // posición del ']'

            string ip1 = ip1Completa.Substring(inicio, fin - inicio);
            string ip2 = ip2Completa.Substring(inicio, fin - inicio);

            Console.WriteLine("1er ip " + ip1);
            Console.WriteLine("2do ip " + ip2);

            var slicedIP1 = ip1.Split(':');
            var slicedIP2 = ip2.Split(':');

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int cmp = ((int)slicedIP1[i][j]).CompareTo((int)slicedIP2[i][j]);
                    if (cmp != 0) return cmp;
                }
            }
            return int.Parse(ip1Completa.Substring(fin + 2)).CompareTo(int.Parse(ip2Completa.Substring(fin + 2)));
        }
    }
}