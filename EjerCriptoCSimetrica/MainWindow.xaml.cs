using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EjerCriptoCSimetrica {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        Criptografia srv;
        public MainWindow() {
            InitializeComponent();
        }
        private void BtnCrear_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            srv = new Criptografia(SymmetricAlgorithm.Create(algo));
            if (string.IsNullOrWhiteSpace(txtClave.Text))
                txtClave.Text = Convert.ToBase64String(srv.Key);
            else
                srv.Key = Convert.FromBase64String(txtClave.Text);
            if (string.IsNullOrWhiteSpace(txtVector.Text))
                txtVector.Text = Convert.ToBase64String(srv.IV);
            else
                srv.IV = Convert.FromBase64String(txtVector.Text);
        }

        private void BtnEncripta_Click(object sender, RoutedEventArgs e) {
            if (srv == null) return;
            srv.Encripta(txtEntrada.Text, txtSalida.Text);
            consola.Text = $"Encriptado {txtEntrada.Text} -> {txtSalida.Text}";
        }
        private void BtnDesEncripta_Click(object sender, RoutedEventArgs e) {
            srv.DesEncripta(txtEntrada.Text, txtSalida.Text);
            consola.Text = $"DesEncriptado {txtEntrada.Text} -> {txtSalida.Text}";
        }
        private void BtnRenombra_Click(object sender, RoutedEventArgs e) {
            txtEntrada.Text = @"C:\dotnet\Seguridad.net\Curso\EjerCriptoCSimetrica\Fichero.bin";
            txtSalida.Text = @"C:\dotnet\Seguridad.net\Curso\EjerCriptoCSimetrica\Fichero.bin.txt";
        }
    }
}
