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
            Pwd = "P@$$w0rd";
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
        private void BtnGeneraPwd_Click(object sender, RoutedEventArgs e) {
            consola.Text = GetHashPasswordRfc2898(entrada.Text, "Hola") + "\n";
            consola.Text += GetHashPasswordRfc2898(entrada.Text, "Otra") + "\n";
            consola.Text += GetHashPasswordRfc2898(entrada.Text, "Otra", 1000) + "\n";
            consola.Text += GetHashPasswordRfc2898(entrada.Text, "Otra", 1000, 64) + "\n";
        }
        public string GetHashPasswordRfc2898(string password, string salt, int iterationCount = 100, int lenght = 32) {
            byte[] pwd = Encoding.UTF8.GetBytes(password);
            byte[] _salt = Encoding.UTF8.GetBytes(salt.PadRight(8, '0'));
            var pdb = new Rfc2898DeriveBytes(pwd, _salt, iterationCount);
            return Convert.ToBase64String(pdb.GetBytes(lenght));
        }

        private byte[] pwd;

        public string Pwd {
            get => Encoding.UTF8.GetString(pwd);
            set {
                pwd = Encoding.UTF8.GetBytes(value);
                if (pwd.Length % 16 > 0)
                    Array.Resize<byte>(ref pwd, pwd.Length + (16 - pwd.Length % 16));
            }
        }

        public void btnVerPwd(object sender, RoutedEventArgs e) {
            consola.Text = Pwd;
        }
        public void btnProtectedPwd(object sender, RoutedEventArgs e) {
            ProtectedMemory.Protect(pwd, MemoryProtectionScope.SameProcess);
        }
        public void btnDesProtectedPwd(object sender, RoutedEventArgs e) {
            ProtectedMemory.Unprotect(pwd, MemoryProtectionScope.SameProcess);
        }

    }
}
