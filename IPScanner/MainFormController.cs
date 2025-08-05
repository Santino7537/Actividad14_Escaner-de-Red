using System.Windows.Forms;
using IPScanner.Model;

namespace IPScanner.Controller
{
    // Clase que conecta la Vista (MainForm) con la lógica (validaciones)
    public class MainFormController
    {
        private TextBox campo1;
        private TextBox campo2;
        private NumericUpDown campo3;
        private Button botonHola;

        public MainFormController(TextBox c1, TextBox c2, NumericUpDown c3, Button bHola)
        {
            campo1 = c1;
            campo2 = c2;
            campo3 = c3;
            botonHola = bHola;

            campo1.TextChanged += ValidarCampos;
            campo2.TextChanged += ValidarCampos;
            campo3.ValueChanged += ValidarCampos;

            campo2.Enabled = false;
            campo3.Enabled = false;
            botonHola.Enabled = false;
        }

        private void ValidarCampos(object sender, System.EventArgs e)
        {
            bool campo1Valido = IPv4Validator.EsDireccionValida(campo1.Text);
            campo2.Enabled = campo1Valido;

            bool campo2Valido = IPv4Validator.EsDireccionValida(campo2.Text);
            campo3.Enabled = campo2Valido;

            bool campo3Valido = campo3.Value > 0;

            botonHola.Enabled = campo1Valido && campo2Valido && campo3Valido;
        }
    }
}
