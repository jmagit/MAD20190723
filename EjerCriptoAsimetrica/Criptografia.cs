using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;

namespace EjerCriptoCAsimetrica {
    public class CSimetrica {
        public byte[] Key {
            get {
                return Algoritmo.Key;
            }
            set {
                Algoritmo.Key = value;
            }
        }
        public byte[] IV {
            get {
                return Algoritmo.IV;
            }
            set {
                Algoritmo.IV = value;
            }
        }
        public SymmetricAlgorithm Algoritmo { get; set; }

        public CSimetrica(SymmetricAlgorithm algoritmo) {
            Algoritmo = algoritmo;
        }

        public CSimetrica() {
            Algoritmo = Aes.Create();
        }
        public CSimetrica(byte[] key, byte[] iV) {
            Key = key;
            IV = iV;
        }

        public void Encripta(string entrada, string salida) {
            Encripta(
                new FileStream(entrada, FileMode.Open, FileAccess.Read),
                new FileStream(salida, FileMode.Create, FileAccess.Write),
                Key, IV);
        }
        public void Encripta(Stream entrada, Stream salida) {
            Encripta(entrada, salida, Key, IV);
        }
        public void Encripta(Stream entrada, Stream salida, byte[] key, byte[] iv) {
            // Encripta
            //CryptoStream encStream = new CryptoStream(salida, Algoritmo.CreateEncryptor(), CryptoStreamMode.Write);
            //copyFile(entrada, encStream);
            //salida.Close();

            // Comprime
            //GZipStream comprimido = new GZipStream(salida, CompressionMode.Compress);
            //copyFile(entrada, comprimido);
            //salida.Close();

            // Comprime y Encripta
            using (CryptoStream encStream = new CryptoStream(salida, Algoritmo.CreateEncryptor(), CryptoStreamMode.Write)) {
                GZipStream comprimido = new GZipStream(encStream, CompressionMode.Compress);
                copyFile(entrada, comprimido);
            }
            salida.Close();
        }
        public void DesEncripta(string entrada, string salida) {
            DesEncripta(
                new FileStream(entrada, FileMode.Open, FileAccess.Read),
                new FileStream(salida, FileMode.Create, FileAccess.Write)
                );
        }
        public void DesEncripta(Stream entrada, Stream salida) {
            DesEncripta(entrada, salida, Key, IV);
        }
        public void DesEncripta(Stream entrada, Stream salida, byte[] key, byte[] iv) {
            // Desencripta
            //CryptoStream encStream = new CryptoStream(entrada, Algoritmo.CreateDecryptor(), CryptoStreamMode.Read);
            //copyFile(encStream, salida);
            //entrada.Close();

            // Descomprime
            //GZipStream comprimido = new GZipStream(entrada, CompressionMode.Decompress);
            //copyFile(comprimido, salida);
            //entrada.Close();

            // Desencripta y Descomprime
            using (CryptoStream encStream = new CryptoStream(entrada, Algoritmo.CreateDecryptor(), CryptoStreamMode.Read)) {
                GZipStream comprimido = new GZipStream(encStream, CompressionMode.Decompress);
                copyFile(comprimido, salida);
            }
            entrada.Close();
        }

        private static void copyFile(Stream entrada, Stream salida) {
            entrada.CopyTo(salida);
            //int bufferLen = 1024;
            //byte[] buffer = new byte[bufferLen];
            //int len;

            //len = entrada.Read(buffer, 0, bufferLen);
            //while (len > 0) {
            //    salida.Write(buffer, 0, len);
            //    len = entrada.Read(buffer, 0, bufferLen);
            //}
            salida.Close();
            entrada.Close();
        }

        static byte[] entropy = { 3, 14, 15, 92, 65, 35, 9 };
        public void save(string salida) {
            FileStream fStream = new FileStream(salida, FileMode.OpenOrCreate);
            byte[] buffer = new byte[Key.Length + IV.Length];
            Key.CopyTo(buffer, 0);
            IV.CopyTo(buffer, Key.Length);
            byte[] encryptedData = ProtectedData.Protect(buffer, entropy, DataProtectionScope.CurrentUser);
            fStream.Write(encryptedData, 0, encryptedData.Length);
            fStream.Close();
        }
        public void load(string entrada) {
            FileStream fStream = new FileStream(entrada, FileMode.Open);
            byte[] buffer = new byte[fStream.Length];
            fStream.Read(buffer, 0, (int)fStream.Length);
            fStream.Close();
            buffer = ProtectedData.Unprotect(buffer, entropy, DataProtectionScope.CurrentUser);
            Array.Copy(buffer, Key, Key.Length);
            Array.Copy(buffer, Key.Length, IV, 0, IV.Length);
        }

        public byte[] dameResume() {
            byte[] buffer = new byte[Key.Length + IV.Length];
            Key.CopyTo(buffer, 0);
            IV.CopyTo(buffer, Key.Length);
            return buffer;
        }
        public void decodeResume(byte[] buffer) {
            var newKey = new byte[Key.Length];
            Array.Copy(buffer, newKey, Key.Length);
            Key = newKey;
            var newIV = new byte[IV.Length];
            Array.Copy(buffer, Key.Length, newIV, 0, IV.Length);
            IV = newIV;
        }
    }

    public class CAsimetrica {
        private string dameClave(string store, bool privada) {
            string rslt;
            var param = new CspParameters();
            param.KeyContainerName = store;
            using (var algoritmo = new RSACryptoServiceProvider(param)) {
                rslt = algoritmo.ToXmlString(privada);
                algoritmo.Clear();
            }
            return rslt;
        }

        public string DameCPublica(string store) {
            return dameClave(store, false);
        }
        public string DameCPrivada(string store) {
            return dameClave(store, true);
        }

        public byte[] Encipta(string clave, byte[] entrada) {
            byte[] rslt;
            using (var algoritmo = RSA.Create()) {
                algoritmo.FromXmlString(clave);
                rslt = algoritmo.Encrypt(
                    entrada,
                    RSAEncryptionPadding.Pkcs1);
                algoritmo.Clear();
            }
            return rslt;
        }
        public string Encipta(string clave, string entrada) {
            return Convert.ToBase64String(
                Encipta(clave, Encoding.UTF8.GetBytes(entrada))
                );
        }
        public byte[] Desencipta(string clave, byte[] cifrado) {
            byte[] rslt;
            using (var algoritmo = RSA.Create()) {
                algoritmo.FromXmlString(clave);
                rslt = algoritmo.Decrypt(
                   cifrado,
                    RSAEncryptionPadding.Pkcs1);
                algoritmo.Clear();
            }
            return rslt;
        }
        public string Desencipta(string clave, string cifrado) {
            return Encoding.UTF8.GetString(
                Desencipta(clave, Convert.FromBase64String(cifrado))
                );
        }
        public byte[] Firma(string clave, byte[] entrada) {
            byte[] rslt;
            using (var algoritmo = RSA.Create()) {
                algoritmo.FromXmlString(clave);
                rslt = algoritmo.SignData(
                    entrada,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1); ;
                algoritmo.Clear();
            }
            return rslt;
        }
        public string Firma(string clave, string entrada) {
            return Convert.ToBase64String(
                Firma(clave, Encoding.UTF8.GetBytes(entrada))
                );
        }
        public byte[] Firma(string clave, Stream entrada) {
            byte[] rslt;
            using (var algoritmo = RSA.Create()) {
                algoritmo.FromXmlString(clave);
                rslt = algoritmo.SignData(
                    entrada,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1); ;
                algoritmo.Clear();
            }
            return rslt;
        }
        public bool Verifica(string clave, byte[] entrada, byte[] firma) {
            bool rslt;
            using (var algoritmo = RSA.Create()) {
                algoritmo.FromXmlString(clave);
                rslt = algoritmo.VerifyData(
                    entrada,
                    firma,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);
                algoritmo.Clear();
            }
            return rslt;
        }
        public bool Verifica(string clave, Stream entrada, byte[] firma) {
            bool rslt;
            using (var algoritmo = RSA.Create()) {
                algoritmo.FromXmlString(clave);
                rslt = algoritmo.VerifyData(
                    entrada,
                    firma,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);
                algoritmo.Clear();
            }
            return rslt;
        }
        public string Verifica(string clave, string entrada, string firma) {
            return Verifica(
                clave, 
                Encoding.UTF8.GetBytes(entrada), 
                Convert.FromBase64String(firma))
                ? "Valido" : "Incorrecto";
        }

    }
}
