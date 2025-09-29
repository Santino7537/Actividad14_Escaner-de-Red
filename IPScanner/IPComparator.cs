namespace IPScanner.Comparator
{
    // Clase encargada de comprarar dos IPs.
    public class IPComparator
    {
        // Diccionario de prioridades. Menor número es mayor prioridad.
        static Dictionary<string, int> typePriority = new Dictionary<string, int>()
        {
            { "IPV4", 1 },
            { "IPV6", 2 },
            { "All IPs and ports", 3 }
        };

        public static int Compare(string ip1, string ip2)
        {
            string ip1Type = IPType(ip1);
            string ip2Type = IPType(ip2);
            int ip1TypePriority = typePriority[ip1Type];
            int ip2TypePriority = typePriority[ip2Type];

            int cmp = ip1TypePriority.CompareTo(ip2TypePriority); // Si es menor, devuelve algo menor a 0, si es igual, devuelve 0, y si es mayor, devuelve algo mayor a 0.
            
            if (cmp != 0) return cmp;

            if (ip1Type == "IPV4") { return CompareIPV4s(ip1, ip2); }
            else if (ip1Type == "IPV6") { return CompareIPV6s(ip1, ip2); } 
            else { return 0; } // (ip1Type == "All IPs and ports") 
        }

        private static string IPType(string ip)
        {
            if (ip.Count(c => c == '.') == 4 && ip.Contains(':'))
            {
                return "IPV4";
            }
            else if (ip.Count(c => c == ':') >= 3 && ip.Contains('['))
            {
                return "IPV6";
            }
            else // (ip.Contains("*:*"))
            {
                return "All IPs and ports";
            }
        }

        private static string IPV6Fixed(string ipv6Completa)
        {
            int inicio = ipv6Completa.IndexOf('[') + 1; // posición después del '['
            int fin = ipv6Completa.IndexOf(']');        // posición del ']'

            string ipv6 = ipv6Completa.Substring(inicio, fin - inicio);

            if (ipv6 == "::") return "[0000:0000:0000:0000:0000:0000:0000:0000]" + ipv6Completa.Substring(fin + 1);

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

            var slicedIP1 = ip1.Split(':');
            var slicedIP2 = ip2.Split(':');

            for (int i = 0; i < 8 - 1; i++)
            {
                for (int j = 0; j < 4 - 1; i++) {
                    int cmp = (int)slicedIP1[i][j].CompareTo((int)slicedIP2[i][j]);
                    if (cmp != 0) return cmp;
                }
            }
            return int.Parse(ip1Completa.Substring(fin + 2)).CompareTo(int.Parse(ip2Completa.Substring(fin + 2)));
        }
    }
}