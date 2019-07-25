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
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.Xml;

namespace Demos {
    /// <summary>
    /// Lógica de interacción para DemoPKCS.xaml
    /// </summary>
    public partial class DemoPKCS : UserControl {
        public DemoPKCS() {
            InitializeComponent();
        }

        private void BtnFirma_Click(object sender, RoutedEventArgs e) {
            X509Certificate2 cert = LeerCertificado(txtCertificado.Text);
            var content = new ContentInfo(Encoding.UTF8.GetBytes(txtClaro.Text));
            var signedCms = new SignedCms(content, false);
            var signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, cert);
            signedCms.ComputeSignature(signer);
            txtSalida.Text = Convert.ToBase64String(signedCms.Encode());
        }
        private void BtnVerifica_Click(object sender, RoutedEventArgs e) {
            SignedCms signedCms = new SignedCms();
            try {
                signedCms.Decode(Convert.FromBase64String(txtSalida.Text));
                signedCms.CheckSignature(true);
                txtVerificar.Text = "Valido";
                consola.Text = Encoding.UTF8.GetString(signedCms.ContentInfo.Content);
            } catch (Exception ex) {
                txtVerificar.Text = "Invalido";
            }
        }
        private void BtnEnvolver_Click(object sender, RoutedEventArgs e) {
            X509Certificate2 cert = LeerCertificado(txtCertificado.Text);
            var content = new ContentInfo(Encoding.UTF8.GetBytes(txtClaro.Text));
            var envelopedCms = new EnvelopedCms(content);
            var recipient = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, cert);
            envelopedCms.Encrypt(recipient);
            txtSalida.Text = Convert.ToBase64String(envelopedCms.Encode());
        }

        private void BtnDesEnvolver_Click(object sender, RoutedEventArgs e) {
            var envelopedCms = new EnvelopedCms();
            try {
                envelopedCms.Decode(Convert.FromBase64String(txtSalida.Text));
                envelopedCms.Decrypt(envelopedCms.RecipientInfos[0]);
                txtVerificar.Text = "Valido";
                consola.Text = Encoding.UTF8.GetString(envelopedCms.ContentInfo.Content);
            } catch (Exception ex) {
                txtVerificar.Text = "Invalido";
            }
        }


        private static X509Certificate2 LeerCertificado(string sujeto) {
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var rslt = store.Certificates.Find(X509FindType.FindBySubjectName, sujeto,
#if DEBUG      
            false
#else
            true
#endif
);
            store.Close();
            return rslt.Count > 0 ? rslt[0] : null;
        }

        private void WriteLine(string msg) {
            consola.Text += msg + "\n";
        }

        private void BtnSimetrico_Click(object sender, RoutedEventArgs e) {
            consola.Text = "";

            SymmetricAlgorithm key = Rijndael.Create();
            EncSim(key);
            DesEncSim(key);

        }

        private void EncSim(SymmetricAlgorithm key) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(txtClaro.Text);
            ESign.Encrypt(xmlDoc, txtEtiqueta.Text, key);
            txtSalida.Text = xmlDoc.InnerXml;
        }
        private void DesEncSim(SymmetricAlgorithm key) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(txtSalida.Text);
            ESign.Decrypt(xmlDoc, key);
            WriteLine(xmlDoc.InnerXml);
        }

        private void BtnASimetrico_Click(object sender, RoutedEventArgs e) {
            consola.Text = "";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(txtClaro.Text);
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_ENC_RSA_KEY";
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);
            ESign.Encrypt(xmlDoc, txtEtiqueta.Text, "EncryptedElement1", rsaKey, "rsaKey");
            txtSalida.Text = xmlDoc.InnerXml;

            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.PreserveWhitespace = true;
            xmlDoc2.LoadXml(txtSalida.Text);

            ESign.Decrypt(xmlDoc2, rsaKey, "rsaKey");
            WriteLine(xmlDoc2.InnerXml);
            rsaKey.Clear();
        }

        private void BtnCertificado_Click(object sender, RoutedEventArgs e) {
            consola.Text = "";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(txtClaro.Text);

            var store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Find(X509FindType.FindBySubjectName, txtCertificado.Text, false)[0];
            store.Close();
            ESign.Encrypt(xmlDoc, "creditcard", cert);
            txtSalida.Text = xmlDoc.InnerXml;

            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.PreserveWhitespace = true;
            xmlDoc2.LoadXml(txtSalida.Text);
            ESign.Decrypt(xmlDoc2);
            WriteLine(xmlDoc2.InnerXml);
        }

        private void BtnXMLFirma_Click(object sender, RoutedEventArgs e) {
            consola.Text = "";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(txtClaro.Text);
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_ENC_RSA_KEY";
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);
            ESign.SignXml(xmlDoc, rsaKey);
            txtSalida.Text = xmlDoc.InnerXml;
        }
        private void BtnXMLVerifica_Click(object sender, RoutedEventArgs e) {
            consola.Text = "";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(txtSalida.Text);
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_ENC_RSA_KEY";
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);

            WriteLine($"The XML signature is {(ESign.VerifyXml(xmlDoc, rsaKey) ? "" : "not ")}valid.");
        }

    }
}