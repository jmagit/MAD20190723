using System;
using System.Collections.Generic;
using System.IO;
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

namespace EjerCriptoHash {
    /// <summary>
    /// Lógica de interacción para Uno.xaml
    /// </summary>
    public partial class Uno : UserControl {
        public Uno() {
            InitializeComponent();
        }
        private void BtnFirma_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            try {
                using (Stream fich = new FileStream(txtFichero.Text, FileMode.Open)) {
                    txtFirma.Text = Convert.ToBase64String(HashAlgorithm.Create(algo).ComputeHash(fich));
                }
            } catch (Exception ex) {
                consola.Text = ex.Message;
            }
        }
        private void BtnVerifica_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            try {
                using (Stream fich = new FileStream(txtFichero.Text, FileMode.Open)) {
                    HashAlgorithm algoritmo = HashAlgorithm.Create(algo);
                    var nueva = Convert.ToBase64String(HashAlgorithm.Create(algo).ComputeHash(fich));
                    consola.Text = nueva + "\n" + (txtFirma.Text == nueva ? "Igual" : "Cambiado");
                }
            } catch (Exception ex) {
                consola.Text = ex.Message;
            }
        }
    }
}
