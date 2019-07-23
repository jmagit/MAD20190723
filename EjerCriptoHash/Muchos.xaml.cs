using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class Firmas {
        public string Fichero { get; set; }
        public string Firma { get; set; }
        public bool Valido { get; set; }

        public Firmas(string fichero, string firma, bool valido = true) {
            Fichero = fichero;
            Firma = firma;
            Valido = valido;
        }
    }
    /// <summary>
    /// Lógica de interacción para Muchos.xaml
    /// </summary>
    public partial class Muchos : UserControl {
        ObservableCollection<Firmas> lista = new ObservableCollection<Firmas>();
        public Muchos() {
            InitializeComponent();
        }
        private void BtnFirma_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            try {
                lista.Clear();
                var dir = new DirectoryInfo(txtDirectorio.Text);
                using (var algoritmo = KeyedHashAlgorithm.Create(algo)) {
                    algoritmo.Key = Encoding.UTF8.GetBytes(txtClave.Text);
                    foreach (FileInfo fInfo in dir.GetFiles()) {
                        using (Stream fich = fInfo.Open(FileMode.Open)) {
                            lista.Add(new Firmas(
                                fInfo.Name,
                                Convert.ToBase64String(algoritmo.ComputeHash(fich))
                            ));
                        }
                    }
                }
                gFirmas.ItemsSource = lista;
            } catch (Exception ex) {
                consola.Text = ex.Message;
            }
        }
        private void BtnVerifica_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            try {
                var dir = new DirectoryInfo(txtDirectorio.Text);
                using (var algoritmo = KeyedHashAlgorithm.Create(algo)) {
                    algoritmo.Key = Encoding.UTF8.GetBytes(txtClave.Text);
                    foreach (FileInfo fInfo in dir.GetFiles()) {
                        using (Stream fich = fInfo.Open(FileMode.Open)) {
                            var nueva = Convert.ToBase64String(algoritmo.ComputeHash(fich));
                            var firma = lista.FirstOrDefault(o => o.Fichero == fInfo.Name);
                            if (firma != null)
                                firma.Valido = firma.Firma == nueva;
                        }
                    }
                }
                gFirmas.ItemsSource = null;
                gFirmas.ItemsSource = lista;
            } catch (Exception ex) {
                consola.Text = ex.Message;
            }
        }
    }
}
