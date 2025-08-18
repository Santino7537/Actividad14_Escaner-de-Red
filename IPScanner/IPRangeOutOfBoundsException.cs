namespace IPScanner.IPRangeException
{
    public class IPRangeOutOfBoundsException : Exception // Al heredar de "Exception", se estaria creando una excepción personalizada.
    {
        public IPRangeOutOfBoundsException(string message) : base(message) { } // Se crea un constructor que llama al constructor padre para mandarle como argumento un mensaje de error.
    }
}