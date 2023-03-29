using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace Formbot.SPV.Controllers
{
    public class HomeController : Controller
    {
        const string RsaSha256Uri = "http://www.w3.org/2000/09/xmldsig#sha1";

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Create(int signType = 0)
        {
            string msg = string.Empty;

            try
            {
                string fileName = string.Empty;

                if (signType == 1)
                {
                    msg = "Product Signature created successfully.";
                    fileName = Server.MapPath("~/product.xml");
                }
                else if (signType == 2)
                {
                    msg = "Installation Signature created successfully.";
                    fileName = Server.MapPath("~/installation.xml");
                }
                else
                {
                    msg = "REC Signature created successfully.";
                    fileName = Server.MapPath("~/REC Test.xml");
                }

                //CryptoConfig.AddAlgorithm(
                //    //typeof(System.Deployment.Internal.CodeSigning.RSAPKCS1SHA256SignatureDescription),
                //    typeof(RSAPKCS1SHA256SignatureDescription),
                //    RsaSha256Uri);

                //string certPath = @"C:\Users\pca\Desktop\server.pfx";

                string certPath = Server.MapPath("~/server.pfx");
                string certPass = "tatva123";
                X509Certificate2 cert = new X509Certificate2(certPath, certPass, X509KeyStorageFlags.Exportable);
                RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider();
                rsaCsp.PersistKeyInCsp = false;

                // store the private key for later (signing)
                var exportedKeyMaterial = cert.PrivateKey.ToXmlString(true);
                rsaCsp.FromXmlString(exportedKeyMaterial);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(fileName);
                //xmlDoc.Load(Server.MapPath("~/Rec Test.xml"));

                //xmlDoc.Load(@"D:\Projects\XmlSignatureGenerate\XmlSignatureGenerate\test.xml");
                SignXml(xmlDoc, rsaCsp, cert, signType);
                Console.WriteLine("XML file signed.");
                xmlDoc.Save(fileName);
                var content = new StringContent(xmlDoc.OuterXml, Encoding.UTF8, "text/xml");
                var client = new HttpClient();
                var result = client.PostAsync("https://test-api.solarscope.com.au/cer_product", content).Result;
                var data = result.Content.ReadAsStringAsync().Result;
                XmlDocument xmlSPVVerificationResult = new XmlDocument();
                xmlSPVVerificationResult.LoadXml(data);
                //xmlDoc.Save(Server.MapPath("~/Rec Test.xml"));

                if (signType == 3)
                {
                    XmlDocument Doc = new XmlDocument();
                    Doc.PreserveWhitespace = true;
                    Doc.Load(fileName);
                    //Doc.Load(Server.MapPath("~/Rec Test.xml"));
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding = new UTF8Encoding(false);
                    settings.Indent = true;
                    using (XmlWriter writer = XmlWriter.Create(Server.MapPath("~/Rec Test(1).xml"), settings))
                    //using (XmlWriter writer = XmlWriter.Create(fileName, settings))
                    {
                        Doc.Save(writer);
                    }
                }

                ViewBag.Message = string.Format(msg);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ViewBag.Message = string.Format(e.Message);
            }
            return View("Index");
        }

        // public ActionResult Index1()
        //{
        //     try
        //     {

        //         //CryptoConfig.AddAlgorithm(
        //         //    //typeof(System.Deployment.Internal.CodeSigning.RSAPKCS1SHA256SignatureDescription),
        //         //    typeof(RSAPKCS1SHA256SignatureDescription),
        //         //    RsaSha256Uri);
        //         string certPath = @"C:\Users\pca\Desktop\server.pfx";
        //         string certPass = "tatva123";
        //         X509Certificate2 cert = new X509Certificate2(certPath, certPass, X509KeyStorageFlags.Exportable);
        //         RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider();
        //         rsaCsp.PersistKeyInCsp = false;
        //         // store the private key for later (signing)
        //         var exportedKeyMaterial = cert.PrivateKey.ToXmlString(true);
        //         rsaCsp.FromXmlString(exportedKeyMaterial);
        //         XmlDocument xmlDoc = new XmlDocument();
        //         xmlDoc.PreserveWhitespace = true;
        //         xmlDoc.Load(Server.MapPath("~/Rec Test.xml"));

        //         //xmlDoc.Load(@"D:\Projects\XmlSignatureGenerate\XmlSignatureGenerate\test.xml");
        //         SignXml(xmlDoc, rsaCsp, cert);
        //         Console.WriteLine("XML file signed.");
        //         xmlDoc.Save(Server.MapPath("~/Rec Test.xml"));
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e.Message);
        //     }
        //     XmlDocument Doc = new XmlDocument();
        //     Doc.PreserveWhitespace = true;
        //     Doc.Load(Server.MapPath("~/Rec Test.xml"));
        //     XmlWriterSettings settings = new XmlWriterSettings();
        //     settings.Encoding = new UTF8Encoding(false);
        //     settings.Indent = true;
        //     using (XmlWriter writer = XmlWriter.Create(Server.MapPath("~/Rec Test(1).xml"), settings))
        //     {
        //         Doc.Save(writer);
        //     }
        //     return this.Content("ok");
        // }


        public void SignXml(XmlDocument xmlDoc, RSA Key, X509Certificate2 cert, int signType)
        {
            string id = string.Empty;
            if (signType == 1)
            {
                id = "#products";
            }
            else if (signType == 2)
            {
                id = "#Installation";
            }
            else
            {
                id = "#Installation";
            }

            // Check arguments. 
            if (xmlDoc == null)
                throw new ArgumentException("xmlDoc");
            if (Key == null)
                throw new ArgumentException("Key");

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Add the key to the SignedXml document.
            signedXml.SigningKey = cert.PrivateKey;
            //Reference reference = new Reference { Uri = "#Installation", DigestMethod = SignedXml.XmlDsigSHA1Url };
            Reference reference = new Reference { Uri = id, DigestMethod = SignedXml.XmlDsigSHA1Url };
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            XmlDsigC14NTransform env1 = new XmlDsigC14NTransform();
            reference.AddTransform(env);
            reference.AddTransform(env1);
            KeyInfo keyInfo = new KeyInfo();
            KeyInfoX509Data keyInfoData = new KeyInfoX509Data();
            keyInfoData.AddIssuerSerial(cert.Issuer, cert.GetSerialNumberString());
            keyInfoData.AddCertificate(cert);
            keyInfo.AddClause(keyInfoData);
            KeyInfoName kin = new KeyInfoName();
            kin.Value = cert.Issuer;
            keyInfo.AddClause(kin);
            keyInfo.AddClause(new RSAKeyValue(Key));
            signedXml.KeyInfo = keyInfo;
            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;
            // Compute the signature.
            signedXml.ComputeSignature();
            RSACryptoServiceProvider rsaEncryptor = (RSACryptoServiceProvider)cert.PrivateKey;

            //byte[] signatureData = rsaEncryptor.SignData(Encoding.Default.GetBytes(xmlDoc.OuterXml), new SHA1CryptoServiceProvider());
            //var isProper = rsaEncryptor.VerifyData(Encoding.Default.GetBytes(xmlDoc.OuterXml), new SHA1CryptoServiceProvider(), signatureData);
            //string Text = Convert.ToBase64String(signatureData);
            // Get the XML representation of the signature and save 
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class RSAPKCS1SHA256SignatureDescription : SignatureDescription
    {
        private string _hashAlgorithm;

        public RSAPKCS1SHA256SignatureDescription()
        {
            KeyAlgorithm = "System.Security.Cryptography.RSA";
            DigestAlgorithm = "System.Security.Cryptography.SHA1CryptoServiceProvider";
            FormatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureFormatter";
            DeformatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureDeformatter";
            _hashAlgorithm = "SHA1";

            Console.WriteLine("Constructed");
        }

        public sealed override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
        {
            AsymmetricSignatureDeformatter item = base.CreateDeformatter(key);
            item.SetHashAlgorithm(_hashAlgorithm);
            Console.WriteLine("Created a deformatter");
            return item;
        }

        public sealed override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
        {
            AsymmetricSignatureFormatter item = base.CreateFormatter(key);
            item.SetHashAlgorithm(_hashAlgorithm);
            Console.WriteLine("Created a formatter");
            return item;
        }
    }
}