using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SposVietPlugin_net_4._6._1
{
    public class X509Certificate2Helper
    {
        private X509Certificate2 x509Certificate2;
        public   X509Certificate2 GetCertFromStore()
        {
            //to access to store we need to specify store name and location    
            X509Store x509Store = new X509Store(StoreLocation.CurrentUser);
            x509Store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            X509Certificate2Collection selectedCertificates =
                    X509Certificate2UI.SelectFromCollection(x509Store.Certificates, "Chứng thư số", "Lựa chọn chứng thư số", X509SelectionFlag.SingleSelection);

            // Get the first certificate that has a primary key
            foreach (var certificate in selectedCertificates)
            {
                if (certificate.HasPrivateKey)
                    x509Certificate2 = certificate;
                return certificate;
            }
            if (x509Certificate2==null)
            {
                return null;
            }
            return x509Store.Certificates[0];
        }
        public  byte[] GetDigitalSignature(byte[] hashBytes)
        {
            // X509Certificate2 certificate = GetCertFromStore();
            X509Certificate2 certificate;
            if (x509Certificate2 != null)
            {
                certificate = x509Certificate2;
            }
            else
            {
                certificate = GetCertFromStore();
            }

            /*use any asymmetric crypto service provider for encryption of hash   
            with private key of cert.   
            */
            //--------------------trường hợp cũ
            // using (RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PrivateKey)
            try
            {
                
                LogControl.Write(certificate.SerialNumber);
                LogControl.Write(certificate.PrivateKey.ToString());



                //-------------------trường hợp cts mới SH256
                if (certificate.PrivateKey.ToString().Contains("RSACryptoServiceProvider"))
                {
                    //-----------cách 1
                    RSACryptoServiceProvider rsaCryptoService = (RSACryptoServiceProvider)certificate.PrivateKey;//mặc định cái này
                    return rsaCryptoService.SignHash(hashBytes, CryptoConfig.MapNameToOID("SHA1"));

                    //----------end
                }
                else
                {
                    using (System.Security.Cryptography.RSACng rsaCryptoService = (System.Security.Cryptography.RSACng)certificate.PrivateKey)
                    {
                        return rsaCryptoService.SignHash(hashBytes, System.Security.Cryptography.HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                    }
                }

            }
            catch (Exception e)
            {
                LogControl.Write(e.ToString());
                return null;
            }
           
            //--------------------trường hợp mới
            

            // return rsaCryptoService.SignHash(hashBytes);
        }
    }
}
