using System;
using System.Windows.Forms;
using IPScanner.View;

namespace IPScanner
{
    static class Program
    {
        [STAThread] //Usa el modelo de subprocesamiento STA (Single Threaded Apartment), usada para que funcionen los componentes de interfaz.
        static void Main()
        {
            Application.EnableVisualStyles(); //Esta línea le dice a la aplicación que use el estilo visual del sistema operativo actual.
            Application.SetCompatibleTextRenderingDefault(false); //Al poner false, se usa el sistema moderno de renderizado de texto.

            Application.Run(new MainForm());
        }
    }
}