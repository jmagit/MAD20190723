using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Demos {
    public class ESign {
        public static void Encrypt(XmlDocument Doc, string ElementName, SymmetricAlgorithm Key) {
            if (Doc == null) throw new ArgumentNullException("Doc");
            if (ElementName == null) throw new ArgumentNullException("ElementToEncrypt");
            if (Key == null) throw new ArgumentNullException("Alg");

            XmlElement elementToEncrypt = Doc.GetElementsByTagName(ElementName)[0] as XmlElement;
            if (elementToEncrypt == null) throw new XmlException("The specified element was not found");

            EncryptedXml eXml = new EncryptedXml();
            byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, Key, false);
            EncryptedData encryptedData = new EncryptedData();
            encryptedData.Type = EncryptedXml.XmlEncElementUrl;
            string encryptionMethod = null;
            if (Key is TripleDES) {
                encryptionMethod = EncryptedXml.XmlEncTripleDESUrl;
            } else if (Key is DES) {
                encryptionMethod = EncryptedXml.XmlEncDESUrl;
            } else if (Key is Rijndael) {
                switch (Key.KeySize) {
                    case 128: encryptionMethod = EncryptedXml.XmlEncAES128Url; break;
                    case 192: encryptionMethod = EncryptedXml.XmlEncAES192Url; break;
                    case 256: encryptionMethod = EncryptedXml.XmlEncAES256Url; break;
                }
            } else
                throw new CryptographicException("Algorithm is not supported for XML Encryption.");
            encryptedData.EncryptionMethod = new EncryptionMethod(encryptionMethod);
            encryptedData.CipherData.CipherValue = encryptedElement;
            EncryptedXml.ReplaceElement(elementToEncrypt, encryptedData, false);
        }
        public static void Decrypt(XmlDocument Doc, SymmetricAlgorithm Alg) {
            if (Doc == null) throw new ArgumentNullException("Doc");
            if (Alg == null) throw new ArgumentNullException("Alg");

            XmlElement encryptedElement = Doc.GetElementsByTagName("EncryptedData")[0] as XmlElement;

            if (encryptedElement == null) throw new XmlException("The EncryptedData element was not found.");
            EncryptedData edElement = new EncryptedData();
            edElement.LoadXml(encryptedElement);
            EncryptedXml exml = new EncryptedXml();
            byte[] rgbOutput = exml.DecryptData(edElement, Alg);
            exml.ReplaceData(encryptedElement, rgbOutput);
        }
        public static void Encrypt(XmlDocument Doc, string ElementToEncrypt, string EncryptionElementID, RSA Alg, string KeyName) {
            XmlElement elementToEncrypt = Doc.GetElementsByTagName(ElementToEncrypt)[0] as XmlElement;
            if (elementToEncrypt == null) throw new XmlException("The specified element was not found");
            RijndaelManaged sessionKey = null;

            try {
                sessionKey = new RijndaelManaged();
                sessionKey.KeySize = 256;

                EncryptedXml eXml = new EncryptedXml();
                byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, sessionKey, false);
                EncryptedData edElement = new EncryptedData();
                edElement.Type = EncryptedXml.XmlEncElementUrl;
                edElement.Id = EncryptionElementID;
                edElement.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);
                EncryptedKey ek = new EncryptedKey();
                byte[] encryptedKey = EncryptedXml.EncryptKey(sessionKey.Key, Alg, false);
                ek.CipherData = new CipherData(encryptedKey);
                ek.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url);
                DataReference dRef = new DataReference();
                dRef.Uri = "#" + EncryptionElementID;
                ek.AddReference(dRef);
                edElement.KeyInfo.AddClause(new KeyInfoEncryptedKey(ek));
                KeyInfoName kin = new KeyInfoName();
                kin.Value = KeyName;
                ek.KeyInfo.AddClause(kin);
                edElement.CipherData.CipherValue = encryptedElement;
                EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);
            } catch (Exception e) {
                throw e;
            } finally {
                if (sessionKey != null) {
                    sessionKey.Clear();
                }
            }
        }
        public static void Decrypt(XmlDocument Doc, RSA Alg, string KeyName) {
            if (Doc == null) throw new ArgumentNullException("Doc");
            if (Alg == null) throw new ArgumentNullException("Alg");
            if (KeyName == null) throw new ArgumentNullException("KeyName");

            EncryptedXml exml = new EncryptedXml(Doc);
            exml.AddKeyNameMapping(KeyName, Alg);
            exml.DecryptDocument();
        }
        public static void Encrypt(XmlDocument Doc, string ElementToEncrypt, X509Certificate2 Cert) {
            XmlElement elementToEncrypt = Doc.GetElementsByTagName(ElementToEncrypt)[0] as XmlElement;
            if (elementToEncrypt == null) throw new XmlException("The specified element was not found");
            EncryptedXml eXml = new EncryptedXml();
            EncryptedData edElement = eXml.Encrypt(elementToEncrypt, Cert);
            EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);
        }
        public static void Decrypt(XmlDocument Doc) {
            if (Doc == null) throw new ArgumentNullException("Doc");
            EncryptedXml exml = new EncryptedXml(Doc);
            exml.DecryptDocument();
        }
        public static void SignXml(XmlDocument xmlDoc, RSA rsaKey) {
            SignedXml signedXml = new SignedXml(xmlDoc ?? throw new ArgumentException(nameof(xmlDoc)));
            signedXml.SigningKey = rsaKey ?? throw new ArgumentException(nameof(rsaKey));
            Reference reference = new Reference();
            reference.Uri = "";
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
        }
        public static Boolean VerifyXml(XmlDocument xmlDoc, RSA key) {
            SignedXml signedXml = new SignedXml(xmlDoc ?? throw new ArgumentException("xmlDoc"));
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
            if (nodeList.Count <= 0) {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            } else if (nodeList.Count >= 2) {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }
            signedXml.LoadXml((XmlElement)nodeList[0]);
            return signedXml.CheckSignature(key ?? throw new ArgumentException("key"));
        }
    }
}
