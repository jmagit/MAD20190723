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
using System.IO;

namespace Demos {
    /// <summary>
    /// Lógica de interacción para DemoCSimetricas.xaml
    /// </summary>
    public partial class DemoCSimetricas : UserControl {
        public DemoCSimetricas() {
            InitializeComponent();
        }

        private void BtnCrear_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            using (var algoritmo = SymmetricAlgorithm.Create(algo)) {
                txtClave.Text = Convert.ToBase64String(algoritmo.Key);
                txtVector.Text = Convert.ToBase64String(algoritmo.IV);
                algoritmo.Clear();
            }
        }

        private void BtnEncripta_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            using (var algoritmo = SymmetricAlgorithm.Create(algo)) {
                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, 
                        algoritmo.CreateEncryptor(
                            Convert.FromBase64String(txtClave.Text), 
                            Convert.FromBase64String(txtVector.Text)
                            ), CryptoStreamMode.Write)) {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                            swEncrypt.Write(txtClaro.Text);
                        }
                        consola.Text = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }
        private void BtnDesEncripta_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            using (var algoritmo = SymmetricAlgorithm.Create(algo)) {
                using (MemoryStream msEncrypt = new MemoryStream(Convert.FromBase64String(consola.Text))) {
                    using (CryptoStream csDecrypt = new CryptoStream(msEncrypt, 
                        algoritmo.CreateDecryptor(
                            Convert.FromBase64String(txtClave.Text),
                            Convert.FromBase64String(txtVector.Text)
                            ), CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                            txtClaro.Text = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
