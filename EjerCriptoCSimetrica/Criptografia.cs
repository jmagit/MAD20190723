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
            CryptoStream encStream = new CryptoStream(salida, Algoritmo.CreateEncryptor(), CryptoStreamMode.Write);
            copyFile(entrada, encStream);
            salida.Close();
            //GZipStream comprimido = new GZipStream(salida, CompressionMode.Compress);
            //copyFile(entrada, comprimido);
            //salida.Close();
            //var ms = new MemoryStream();
            //GZipStream comprimido = new GZipStream(ms, CompressionMode.Compress);
            //entrada.CopyTo(comprimido);
            //ms.Position = 0;
            //CryptoStream encStream = new CryptoStream(salida, Algoritmo.CreateEncryptor(), CryptoStreamMode.Write);
            //copyFile(ms, encStream);
            //salida.Close();

            //using (CryptoStream encStream = new CryptoStream(salida, Algoritmo.CreateEncryptor(), CryptoStreamMode.Write)) {
            //    GZipStream comprimido = new GZipStream(encStream, CompressionMode.Compress);
            //    copyFile(entrada, comprimido);
            //}
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
            CryptoStream encStream = new CryptoStream(entrada, Algoritmo.CreateDecryptor(), CryptoStreamMode.Read);
            copyFile(encStream, salida);
            entrada.Close();
            //GZipStream comprimido = new GZipStream(entrada, CompressionMode.Decompress);
            //copyFile(comprimido, salida);
            //var ms = new MemoryStream();
            //CryptoStream encStream = new CryptoStream(entrada, Algoritmo.CreateDecryptor(), CryptoStreamMode.Read);
            //encStream.CopyTo(ms);
            //GZipStream comprimido = new GZipStream(ms, CompressionMode.Decompress);
            //copyFile(ms, salida);
            //salida.Close();
            //using (GZipStream comprimido = new GZipStream(entrada, CompressionMode.Decompress)) {
            //    CryptoStream encStream = new CryptoStream(comprimido, Algoritmo.CreateDecryptor(), CryptoStreamMode.Read);
            //    copyFile(encStream, salida);
            //}
            //entrada.Close();
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
    }
}
