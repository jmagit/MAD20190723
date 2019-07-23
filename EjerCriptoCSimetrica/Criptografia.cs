using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;

namespace EjerCriptoCSimetrica {
    public class Criptografia {
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

        public Criptografia(SymmetricAlgorithm algoritmo) {
            Algoritmo = algoritmo;
        }

        public Criptografia() {
            Algoritmo = Aes.Create();
        }
        public Criptografia(byte[] key, byte[] iV) {
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
    }
}
