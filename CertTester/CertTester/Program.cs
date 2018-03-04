using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace CertTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started");

            var doc = new XmlDocument();
            using (var client = new WebClient())
            {
                var xmlBytes = client.DownloadData("https://www.wibit.net/labWebService/rest/getCoursesForCurriculum/1");
                doc.LoadXml(Encoding.UTF8.GetString(xmlBytes));
            }

            // cert with private key
            var pfxPath = "files/modotech-test-cert.pfx";
            var certPassword = "123456789";
            var privateCert = new X509Certificate2(File.ReadAllBytes(pfxPath), certPassword);

            SignXmlDocumentWithCertificate(doc, privateCert);
            
            // XML document is now signed, validate it with the public key
            var signedXmlPath = "files/AfterSigned.xml";
            //signedXmlPath = "files/SignedButTampered.xml";
            var signedXmlDoc = new XmlDocument();
            signedXmlDoc.LoadXml(File.ReadAllText(signedXmlPath));
            ValidateSignedXml(signedXmlDoc);

            Console.WriteLine("Done");
            Console.Read();
        }
        
        static void SignXmlDocumentWithCertificate(XmlDocument doc, X509Certificate2 cert)
        {
            var signedXml = new SignedXml(doc);
            signedXml.SigningKey = cert.PrivateKey;

            // "" means sign entire document, you can specify what parts of XML to sign
            var reference = new Reference();
            reference.Uri = "";

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));

            signedXml.KeyInfo = keyInfo;
            signedXml.ComputeSignature();

            // sign the document with computed signature
            var xmlSig = signedXml.GetXml();
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlSig, true));
        }

        private static void ValidateSignedXml(XmlDocument doc)
        {
            // public key, has no password
            var pemPath = "files/modotech-test-cert-public.pem";
            var publicCert = new X509Certificate2(pemPath);

            try
            {
                var signedXml = new SignedXml(doc);
                var signatureNode = doc.GetElementsByTagName("Signature")[0];
                signedXml.LoadXml((XmlElement)signatureNode);

                var isValid = signedXml.CheckSignature(publicCert, true);
                Console.WriteLine("Document validity is {0}", isValid);
            }
            catch
            {
                Console.WriteLine("Failed to validate");
            }
        }
    }
}
