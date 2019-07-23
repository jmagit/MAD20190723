﻿using System;
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
    /// Lógica de interacción para DemosHash.xaml
    /// </summary>
    public partial class DemosHash : UserControl {
        RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();

        private string toHex(byte[] rslt) {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < rslt.Length; i++) {
                sBuilder.Append(rslt[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public DemosHash() {
            InitializeComponent();
        }

        private void BtnAleatorio_Click(object sender, RoutedEventArgs e) {
            byte[] rslt = new byte[256];
            rnd.GetBytes(rslt);
            consola.Text += Convert.ToBase64String(rslt) + "\n";
            consola.Text += toHex(rslt) + "\n";
            var cad = Convert.ToBase64String(rslt) + "\n";
            rslt = Convert.FromBase64String(cad);
            rslt = Encoding.UTF8.GetBytes("Hola Mundo");
            consola.Text += Encoding.UTF8.GetString(rslt) + "\n";
        }

        private void BtnGeneraHash_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            byte[] contenido = Encoding.UTF8.GetBytes(entrada.Text);
            try {
                HashAlgorithm algoritmo = HashAlgorithm.Create(algo);
                byte[] rslt = algoritmo.ComputeHash(contenido);
                consola.Text += Convert.ToBase64String(rslt) + "\n";
            } catch (Exception ex) {
                consola.Text = ex.Message;
            }
        }
    }
}
