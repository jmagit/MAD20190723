using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Security.Cryptography;

namespace Demos {
    /// <summary>
    /// Lógica de interacción para DemoCAsimetricas.xaml
    /// </summary>
    public partial class DemoCAsimetricas : UserControl {
        public DemoCAsimetricas() {
            InitializeComponent();
        }

        private void BtnCrear_Click(object sender, RoutedEventArgs e) {
            using(var algoritmo = RSA.Create()) {
                txtCPublica.Text = algoritmo.ToXmlString(false);
                txtCCompleta.Text = algoritmo.ToXmlString(true);
                algoritmo.Clear();
            }

        }

        private void BtnEncripta_Click(object sender, RoutedEventArgs e) {
            using (var algoritmo = RSA.Create()) {
                algoritmo.FromXmlString(txtCPublica.Text);
                txtSalida.Text = Convert.ToBase64String(algoritmo.Encrypt(
                    Encoding.UTF8.GetBytes(txtClaro.Text),
                    RSAEncryptionPadding.Pkcs1));
                algoritmo.Clear();
            }
        }
        private void BtnDesEncripta_Click(object sender, RoutedEventArgs e) {
            using (var algoritmo = RSA.Create()) {
                algoritmo.FromXmlString(txtCCompleta.Text);
                consola.Text =  Encoding.UTF8.GetString(algoritmo.Decrypt(
                   Convert.FromBase64String(txtSalida.Text),
                    RSAEncryptionPadding.Pkcs1));
                algoritmo.Clear();
            }
        }
        private void BtnFirmar_Click(object sender, RoutedEventArgs e) {
            using (var algoritmo = RSA.Create()) {
                algoritmo.FromXmlString(txtCCompleta.Text);
                txtSalida.Text = Convert.ToBase64String(algoritmo.SignData(
                    Encoding.UTF8.GetBytes(txtClaro.Text),
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1));
                algoritmo.Clear();
            }
        }
        private void BtnVerifica_Click(object sender, RoutedEventArgs e) {
            using (var algoritmo = RSA.Create()) {
                algoritmo.FromXmlString(txtCPublica.Text);
                consola.Text = algoritmo.VerifyData(
                    Encoding.UTF8.GetBytes(txtClaro.Text),
                    Convert.FromBase64String(txtSalida.Text),
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1) ? "Valido" : "Incorrecto";
                algoritmo.Clear();
            }
        }
        private void BtnCreaRecupera_Click(object sender, RoutedEventArgs e) {
            var param = new CspParameters();
            param.KeyContainerName = txtStore.Text;
            using (var algoritmo = new RSACryptoServiceProvider(param)) {
                txtCPublica.Text = algoritmo.ToXmlString(false);
                txtCCompleta.Text = algoritmo.ToXmlString(true);
                algoritmo.Clear();
            }
        }
        private void BtnBorrar_Click(object sender, RoutedEventArgs e) {
            var param = new CspParameters();
            param.KeyContainerName = txtStore.Text;
            using (var algoritmo = new RSACryptoServiceProvider(param)) {
                txtCPublica.Text = "";
                txtCCompleta.Text = "";
                algoritmo.PersistKeyInCsp = false;
                algoritmo.Clear();
            }
        }
    }
}
