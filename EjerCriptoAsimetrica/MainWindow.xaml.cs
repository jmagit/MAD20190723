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

namespace EjerCriptoCAsimetrica {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        CSimetrica srv;
        public MainWindow() {
            InitializeComponent();
            BtnCrear_Click(null, null);
        }
        private void BtnCrear_Click(object sender, RoutedEventArgs e) {
            var algo = (cbAlgoritmos.SelectedValue as ComboBoxItem).Content.ToString();
            srv = new CSimetrica(SymmetricAlgorithm.Create(algo));
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
            txtEntrada.Text = @"..\..\Fichero.bin";
            txtSalida.Text = @"..\..\Fichero.bin.txt";
        }
        private void BtnGuarda_Click(object sender, RoutedEventArgs e) {
            srv.save(@"data.bin");
            consola.Text = "Guardado";
        }
        private void BtnRecupera_Click(object sender, RoutedEventArgs e) {
            srv.load(@"data.bin");
            txtClave.Text = Convert.ToBase64String(srv.Key);
            txtVector.Text = Convert.ToBase64String(srv.IV);
            BtnCrear_Click(null, null);
            consola.Text = "Recuperado";
        }
        private void BtnLimpia_Click(object sender, RoutedEventArgs e) {
            txtClave.Text = "";
            txtVector.Text = "";
        }

        private void write(string msg) {
            consola.Text += msg + "\n";
        }
        private void write(byte[] msg) {
            consola.Text += Convert.ToBase64String(msg) + "\n";
        }

        private byte[] msgClave;
        private byte[] msgFirma;

        private void EjecutaEmisor() {
            write("Soy el emisor");
            var asim = new CAsimetrica();
            var miClave = asim.DameCPrivada("Emisor");
            var suClavePublica = asim.DameCPublica("Receptor");
            write("Creo la clave simetrica");
            var sim = new CSimetrica();
            write(sim.Key);
            write(sim.IV);
            var newClaveSimetrica = sim.dameResume();
            write(newClaveSimetrica);
            write("Encripto la clave simetrica con la C.Publica del receptor");
            var encClaveSimetrica = asim.Encipta(suClavePublica, newClaveSimetrica);
            write("Envio la clave simetrica encriptada");
            msgClave = encClaveSimetrica;
            write(encClaveSimetrica);
            write("Encripto el fichero");
            sim.Encripta(@"..\..\Fichero.txt", @"..\..\Fichero.bin");
            write("Envio el fichero");
            write("Firmo el fichero con mi clave privada");
            var firma = asim.Firma(miClave, File.OpenRead(@"..\..\Fichero.txt"));
            write("Envio la firma");
            msgFirma = firma;
            write("Termine");

        }
        private void EjecutaReceptor() {
            write("Soy el receptor");
            var asim = new CAsimetrica();
            var miClave = asim.DameCPrivada("Receptor");
            var suClavePublica = asim.DameCPublica("Emisor");
            write("Recupero la clave simetrica encriptada");
            var encClaveSimetrica = msgClave;
            write("Desencripto la clave simetrica con mi C.Privada");
            var newClaveSimetrica = asim.Desencipta(miClave, encClaveSimetrica);
            write(newClaveSimetrica);
            var sim = new CSimetrica();
            sim.decodeResume(newClaveSimetrica);
            write(sim.Key);
            write(sim.IV);
            write("Recupero el fichero");
            write("Desencripto el fichero");
            sim.DesEncripta(@"..\..\Fichero.bin", @"..\..\Fichero.bin.txt");
            write("Recupero la firma");
            var firma = msgFirma;
            write("Valido la firma con la C.Publica del emisor");
            var val = asim.Verifica(suClavePublica, File.OpenRead(@"..\..\Fichero.bin.txt"), firma);
            write("Es " + (val ? "Valido" : "Incorrecto"));
            write("Termine");

        }
        private void BtnSecuenciaCifrado_Click(object sender, RoutedEventArgs e) {
            EjecutaEmisor();
            write("\n ---------------------------------- \n");
            EjecutaReceptor();
        }

    }
}
