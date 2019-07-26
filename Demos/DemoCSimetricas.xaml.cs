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
using System.Security.Principal;
using System.Security.Permissions;

namespace Demos {
    /// <summary>
    /// Lógica de interacción para DemoCSimetricas.xaml
    /// </summary>
    public partial class DemoCSimetricas : UserControl {
        public DemoCSimetricas() {
            InitializeComponent();
        }

       [PrincipalPermission(SecurityAction.Demand, Role = "Administradores", Authenticated = true)]
       private void BtnCrear_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            using (var algoritmo = SymmetricAlgorithm.Create(algo)) {
                txtClave.Text = Convert.ToBase64String(algoritmo.Key);
                txtVector.Text = Convert.ToBase64String(algoritmo.IV);
                algoritmo.Clear();
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Cajero", Authenticated = true)]
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

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
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

        private void BtnVerUsr_Click(object sender, RoutedEventArgs e) {
            var id = WindowsIdentity.GetCurrent();
            var winPrin = new WindowsPrincipal(id);

            consola.Text += id.Name + "\n";
            consola.Text += id.AuthenticationType + "\n";
            if(winPrin.IsInRole("BUILTIN\\Administrators"))
                consola.Text += "Soy Administradores\n";
            if(winPrin.IsInRole("RRHH"))
                consola.Text += "Soy de RR.HH\n";
           if(winPrin.IsInRole("DESKTOP-6LNNCD8\\Alumnos"))
                consola.Text += "Soy de alumno\n";
           if(winPrin.IsInRole("Alumnos"))
                consola.Text += "Soy de alumno\n";
           if(winPrin.IsInRole("BUILTIN\\Alumnos"))
                consola.Text += "Soy de alumno\n";

            var gId = new GenericIdentity("Pepito", "El mio");
            var genPrin = new GenericPrincipal(gId, new string[] { "Administradores", "Cajero" });
            consola.Text += gId.Name + "\n";
            consola.Text += gId.AuthenticationType + "\n";
            if(genPrin.IsInRole("Administradores"))
                consola.Text += "Soy Administradores\n";
            if(genPrin.IsInRole("RRHH"))
                consola.Text += "Soy de RR.HH\n";



            System.Threading.Thread.CurrentPrincipal = genPrin;


        }

    }
}
