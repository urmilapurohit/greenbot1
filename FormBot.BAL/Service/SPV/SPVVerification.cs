using FormBot.Entity.SPV;
using FormBot.Helper;
using FormBot.Helper.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static FormBot.Helper.SystemEnums;

namespace FormBot.BAL.Service.SPV
{
    public class SPVVerification
    {
        #region Field
        public ISpvLogBAL _spvLog;
        public Logger _log;
        public ICreateJobBAL _job;
        #endregion
        #region Constructor
        public SPVVerification(ISpvLogBAL spvLog,ICreateJobBAL createJobBAL)
        {
            _spvLog = spvLog;
            _job = createJobBAL;
            _log = new Logger();
        }
        #endregion

        #region Methods
        /// <summary>
        /// SPV Product Verification from priority service and app side in background
        /// </summary>
        /// <param name="dsSPV"></param>
        /// <param name="ProductVerificationXMLPath"></param>
        /// <param name="ServerCertificate"></param>
        /// <returns></returns>
        public DataTable SPVProductVerification(DataSet dsSPV,string ProductVerificationXMLPath,string ServerCertificate,bool reProductSPVVerification=false)
        {
            DataTable checkSerialnumberPhoto = CheckSerialnumberPhotoDatatable();
            DataTable VerifiedSerialNumber = CreateDatatable();
            if (dsSPV.Tables.Count > 0)
            {
                //List<string> ManufactureUrl = dsSPV.Tables[0].AsEnumerable().Select(s => s.Field<string>("SPVManufactureProductVerificationUrl")).Distinct().ToList();
                //foreach (var url in ManufactureUrl)
                //{
                    DataTable ManuFactureWiseVerifiedSerialNumber = CreateDatatable();
                    
                    //if (!string.IsNullOrEmpty(url))
                    if(dsSPV.Tables[0].Rows.Count>0)
                    {
                        DataTable dtSerialNumberPhotos = new DataTable();
                        if (dsSPV.Tables.Count > 1 && dsSPV.Tables[1] != null)
                        {
                            dtSerialNumberPhotos = dsSPV.Tables[1];
                        }

                    //var filterDt = dsSPV.Tables[0].Select("SPVManufactureProductVerificationUrl = '" + url+"'").CopyToDataTable();
                    var url = dsSPV.Tables[0].AsEnumerable().Select(s => s.Field<string>("SPVManufactureProductVerificationUrl")).Distinct().FirstOrDefault();
                    int spvManufacturerid = dsSPV.Tables[0].AsEnumerable().Select(s => s.Field<int>("SPVManufactureId")).Distinct().FirstOrDefault();
                    string defaultSupplier = dsSPV.Tables[0].Rows[0]["Supplier"].ToString();
                    int JobId =Convert.ToInt32(dsSPV.Tables[0].Rows[0]["JobId"].ToString());
                    string Model = dsSPV.Tables[0].Rows[0]["Model"].ToString();
                    string Brand = dsSPV.Tables[0].Rows[0]["Manufacturer"].ToString();
                    var dataProduct = string.Empty;
                        for (int i = 0; i < dsSPV.Tables[0].Rows.Count; i++)
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.PreserveWhitespace = true;
                            xmlDoc.Load(ProductVerificationXMLPath);
                            var dr = dsSPV.Tables[0].Rows[i];
                           // var strXmlSrNum = new XCData(dr["SerialNumber"].ToString());
                            if (Convert.ToString(dr["ServiceAdministrator"]) != "FormBay")
                            {
                                XmlNodeList nodes = xmlDoc.GetElementsByTagName("Image");
                                xmlDoc.GetElementsByTagName("Products")[0].ChildNodes[1].RemoveChild(nodes[0]);
                            }
                            var element = xmlDoc.GetElementsByTagName("Products");
                            dataProduct = element[0].InnerXml.Replace("[[SerialNumber]]", Convert.ToString(new XCData( dr["SerialNumber"].ToString())))
                                                .Replace("[[ModelNumber]]", Convert.ToString(new XCData(dr["Model"].ToString())))
                                                .Replace("[[Manufacturer]]", Convert.ToString(new XCData(dr["Manufacturer"].ToString())))
                                                .Replace("[[Supplier]]", Convert.ToString(new XCData(dr["Supplier"].ToString())
                                                ));
                            //var reqDateTimeElement = xmlDoc.GetElementsByTagName("RequestedDateTime");
                           xmlDoc.InnerXml=xmlDoc.InnerXml.Replace("[[RequestedDateTime]]", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

                            if (Convert.ToString(dr["ServiceAdministrator"]) == "FormBay")
                            {
                                if(dtSerialNumberPhotos.Rows.Count > 0)
                                {
                                    string fullPath = string.Empty;
                                    string serialNumberPhotoPath = null;
                                    string latitude = string.Empty;
                                    string longitude = string.Empty;
                                    string imageTakenDate = string.Empty;
                                //DataRow imageData = dtSerialNumberPhotos.AsEnumerable().Where(a => a.Field<string>("Path").Contains(@"\" + Convert.ToString(dr["SerialNumber"]) + ".")
                                //|| a.Field<string>("Path").Contains(@"\" + Convert.ToString(dr["SerialNumber"]) + "(")
                                //|| a.Field<string>("Path").Contains(@"r_" + Convert.ToString(dr["SerialNumber"]) + ".")).FirstOrDefault();
                                
                                DataRow imageData = dtSerialNumberPhotos.AsEnumerable().Where(a =>
                                Path.GetFileName(a.Field<string>("Path")).Contains(Convert.ToString(dr["SerialNumber"]))).FirstOrDefault();

                                if (imageData != null)
                                    {
                                        serialNumberPhotoPath = imageData["Path"] != null ? Convert.ToString(imageData["Path"]) : null;
                                        latitude = imageData["Latitude"] != null ? Convert.ToString(imageData["Latitude"]) : string.Empty;
                                        longitude = imageData["Longitude"] != null ? Convert.ToString(imageData["Longitude"]) : string.Empty;
                                        imageTakenDate = imageData["CreatedDate"] != null ? Convert.ToString(imageData["CreatedDate"]) : string.Empty;
                                    }

                                    if (serialNumberPhotoPath == null)
                                    {
                                        checkSerialnumberPhoto = SetSerialnumberWithPhotoUnavaibility(checkSerialnumberPhoto,Convert.ToString(dr["SerialNumber"]));
                                        //return checkSerialnumberPhoto;
                                        //dataProduct = dataProduct.Replace("[[Image]]", "");
                                    }
                                else
                                {
                                    string optimizedImagePath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, Path.GetDirectoryName(serialNumberPhotoPath), ProjectConfiguration.ShortNameOfSerialNumberPhoto + Path.GetFileNameWithoutExtension(serialNumberPhotoPath) + Path.GetExtension(serialNumberPhotoPath));
                                    if (reProductSPVVerification == true)
                                    {
                                        /*create new thumbnails when re-spv product verify*/
                                        string[] Filepath = serialNumberPhotoPath.Split('\\');
                                        string deletedphotodirectorypath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, Filepath[0], Filepath[1], "DeletedPhotos", Filepath[2], Filepath[3]);
                                        if (!Directory.Exists(deletedphotodirectorypath))
                                        {
                                            Directory.CreateDirectory(deletedphotodirectorypath);
                                        }
                                        string filename = ProjectConfiguration.ShortNameOfSerialNumberPhoto + Path.GetFileNameWithoutExtension(serialNumberPhotoPath);
                                        string deletedphotofile = deletedphotodirectorypath + "\\" + ProjectConfiguration.ShortNameOfSerialNumberPhoto + Path.GetFileNameWithoutExtension(serialNumberPhotoPath) + Path.GetExtension(serialNumberPhotoPath);



                                        if (File.Exists(deletedphotofile))
                                        {
                                            //int count = 1;
                                            //while (File.Exists(deletedphotofile))
                                            //{
                                            //    string temp = string.Format("{0}({1})", filename, count++);
                                            //    deletedphotofile = Path.Combine(deletedphotodirectorypath, temp + Path.GetExtension(deletedphotofile));

                                            //}
                                            File.Delete(deletedphotofile);
                                        }
                                        //else
                                        //{
                                        //    deletedphotofile = deletedphotodirectorypath + "\\" + ProjectConfiguration.ShortNameOfSerialNumberPhoto + Path.GetFileNameWithoutExtension(serialNumberPhotoPath) + Path.GetExtension(serialNumberPhotoPath);
                                        //}
                                        if (File.Exists(optimizedImagePath))
                                        {
                                            File.Copy(optimizedImagePath, deletedphotofile,false);
                                            File.Delete(optimizedImagePath);
                                            //File.Move(optimizedImagePath, deletedphotofile);
                                            //fullPath = optimizedImagePath;
                                        }
                                    }
                                    
                                        if (File.Exists(optimizedImagePath))
                                        {
                                           //File.Move(optimizedImagePath, deletedphotofile);
                                            fullPath = optimizedImagePath;
                                        }
                                        else
                                        {
                                            bool isCompressed = ReduceSizeOfSerialnumberPhoto(Path.Combine(ProjectConfiguration.ProofDocumentsURL, serialNumberPhotoPath), latitude, longitude, imageTakenDate);
                                            if (isCompressed && File.Exists(optimizedImagePath))
                                            {
                                                fullPath = optimizedImagePath;
                                            }
                                            else
                                            {
                                                fullPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, serialNumberPhotoPath);
                                            }
                                        }
                                        string base64 = Common.ImageToBase64(fullPath);
                                        dataProduct = dataProduct.Replace("[[Image]]", base64);
                                    }
                                
                               
                            }
                                else
                                {
                                    checkSerialnumberPhoto = SetSerialnumberWithPhotoUnavaibility(checkSerialnumberPhoto, Convert.ToString(dr["SerialNumber"]));
                                    //return checkSerialnumberPhoto;
                                    //dataProduct = dataProduct.Replace("[[Image]]", "");
                                }
                            }
                            //else
                            //{
                            //    XmlNodeList nodes = xmlDoc.GetElementsByTagName("Image");
                            //    xmlDoc.GetElementsByTagName("Products")[0].ChildNodes[1].RemoveChild(nodes[0]);
                            //}

                            SpvLog objeSpvLog = new SpvLog();
                            objeSpvLog.SerialNumber = Convert.ToString(dr["SerialNumber"]);
                            objeSpvLog.ModelNumber = Convert.ToString(dr["Model"]);
                            objeSpvLog.Manufacturer = Convert.ToString(dr["Manufacturer"]);
                            objeSpvLog.Supplier = Convert.ToString(dr["Supplier"]);
                            objeSpvLog.JobId = Convert.ToInt32(dr["JobId"]);
                            objeSpvLog.ServiceAdministrator = Convert.ToString(dr["ServiceAdministrator"]);
                            objeSpvLog.SPVMethod = SpvRequest.ProductVerification.GetHashCode();

                            var newRow = ManuFactureWiseVerifiedSerialNumber.NewRow();
                            newRow["SerialNumber"] = Convert.ToString(dr["SerialNumber"]);
                       
                        var lst = ProductVerificationSPV(xmlDoc, ServerCertificate, dataProduct, url, objeSpvLog);
                        string validSupplier = string.Empty;
                            if (lst.Count > 0)
                                {
                                List<string> lstSupplier = _spvLog.GetSupplierList(spvManufacturerid);
                                if(lstSupplier!=null && lstSupplier.Count > 1)
                                    {
                                        for(int k=0;k<lstSupplier.Count;k++)
                                        {
                                            if (lstSupplier[k] != defaultSupplier)
                                            {
                                      
                                        XmlNode root = xmlDoc.DocumentElement;
                                        root.RemoveChild(root.LastChild);
                                        XmlNodeList nodes = xmlDoc.GetElementsByTagName("ResponsibleSupplier");
                                        XmlNode newNode = xmlDoc.CreateElement("ResponsibleSupplier");
                                        newNode.InnerText = Convert.ToString(new XCData(lstSupplier[k].ToString()));
                                        newNode.InnerXml= Convert.ToString(new XCData(lstSupplier[k].ToString()));
                                        xmlDoc.GetElementsByTagName("Products")[0].ChildNodes[1].ReplaceChild(newNode,nodes[0]);
                                        var element1 = xmlDoc.GetElementsByTagName("Products");
                                        //element1[0].InnerXml = Convert.ToString(new XCData(lstSupplier[k].ToString()));
                                        dataProduct = element1[0].InnerXml.Replace("[[SerialNumber]]", Convert.ToString(new XCData(dr["SerialNumber"].ToString())))
                                              .Replace("[[ModelNumber]]", Convert.ToString(new XCData(dr["Model"].ToString())))
                                              .Replace("[[Manufacturer]]", Convert.ToString(new XCData(dr["Manufacturer"].ToString())))
                                              .Replace("[[Supplier]]", Convert.ToString(lstSupplier[k].ToString()));
                                                var lsterrorcnt = ProductVerificationSPV(xmlDoc, ServerCertificate, dataProduct, url, objeSpvLog);
                                                if (lsterrorcnt.Count > 0)
                                                {
                                                   // k = k + 1;
                                                    if (k+1 == lstSupplier.Count)
                                                        newRow["Verified"] = false;
                                                    
                                                  }
                                                else
                                                {
                                                        validSupplier = lstSupplier[k].ToString();
                                            _job.UpdateSupplierForJob(JobId, Brand, Model,validSupplier);
                                                        newRow["Verified"] = true;
                                                         k = lstSupplier.Count;
                                                }
                                            }
                                        

                                     }
                                  }
                               else
                                 newRow["Verified"] = false;
                                }
                               
                            else
                                newRow["Verified"] = true;
                            ManuFactureWiseVerifiedSerialNumber.Rows.Add(newRow);
                        }
                    }
                if (checkSerialnumberPhoto.Rows.Count > 0 && checkSerialnumberPhoto != null)
                {
                    return checkSerialnumberPhoto;
                }
                VerifiedSerialNumber.Merge(ManuFactureWiseVerifiedSerialNumber);
                //}
                
                return VerifiedSerialNumber;
            }
            return new DataTable();
        }
        public string AddCommaIfStringIsNotNull(string str)
        {
            return (string.IsNullOrEmpty(str) ? "" : ",") + str;
        }
        /// <summary>
        /// Sign xml document to verify
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="Key"></param>
        /// <param name="cert"></param>
        public void SignXml(XmlDocument xmlDoc, RSA Key, X509Certificate2 cert,int signType)
        {
            string id = string.Empty;
            if (signType == 1)
            {
                id = "#products";
            }
            else 
            {
                id = "#Installation";
            }
            if (xmlDoc == null)
                throw new ArgumentException("xmlDoc");
            if (Key == null)
                throw new ArgumentException("Key");
            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = cert.GetRSAPrivateKey();
            Reference reference = new Reference { Uri = id, DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256" };
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
           // XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            XmlDsigC14NTransform env1 = new XmlDsigC14NTransform();
            reference.AddTransform(env);
           // reference.AddTransform(env1);
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
            signedXml.AddReference(reference);
            signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
            signedXml.ComputeSignature();
            RSACryptoServiceProvider rsaEncryptor = (RSACryptoServiceProvider)cert.PrivateKey;
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
            //xmlDoc.Save(HttpContext.Current.Server.MapPath("~/XmlwithSignature.xml"));
            ////XmlDocument xmlDoc1 = xmlDoc;
            //var xmlPath = HttpContext.Current.Server.MapPath("~/XmlwithSignature.xml");

            //XmlDocument xmlDoc1 = new XmlDocument();
            //xmlDoc1.Load(xmlPath);
            //xmlDoc1.PreserveWhitespace = true;
            ////XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");
            ////foreach (XmlElement element in nodeList)
            ////{

            ////    SignedXml signedXml1 = new SignedXml(xmlDoc1);
            ////    signedXml1.LoadXml(element);
            ////    bool passes = signedXml1.CheckSignature();

            ////}


            //XmlNodeList nodeList = xmlDoc1.GetElementsByTagName("Signature");
            //XmlNodeList certificates = xmlDoc1.GetElementsByTagName("X509Certificate");
            //X509Certificate2 dcert2 = new X509Certificate2(Convert.FromBase64String(certificates[0].InnerText));
            ////foreach (XmlElement element in nodeList)
            ////{
            ////    signedXml.LoadXml(element);
            //// bool passes = signedXml.CheckSignature(dcert2, true);

            ////}
            //SignedXml signedXml1 = new SignedXml(xmlDoc1);
            //signedXml1.LoadXml((XmlElement)nodeList[0]);
            //signedXml1.KeyInfo = keyInfo;
            //signedXml1.SigningKey = cert.PublicKey.Key;
            //bool passes = signedXml1.CheckSignature();
            //// Check the signature and return the result.
            //var abc = signedXml1.CheckSignature(Key);

            //var i = signedXml.CheckSignature();

            //checksigntureFromXML();

        }
        public void checksigntureFromXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(HttpContext.Current.Server.MapPath("~/XmlwithSignature.xml"));
            SignedXml signedXml = new SignedXml(xmlDoc);
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
            XmlNodeList certificates = xmlDoc.GetElementsByTagName("X509Certificate");
            X509Certificate2 dcert2 = new X509Certificate2(Convert.FromBase64String(certificates[0].InnerText));
            foreach (XmlElement element in nodeList)
            {
                signedXml.LoadXml(element);
                bool passes = signedXml.CheckSignature(dcert2, true);
            }
        }
        public DataTable SPVProductVerificationAPI(DataSet dsSPV, string ProductVerificationXMLPath, string ServerCertificate,List<string> notVerifiedSerialNumber, List<string> verifiedSerialNumber)
        {
            //var _log = new Logger();
            DataTable VerifiedSerialNumber = CreateDatatable();
            if (dsSPV.Tables.Count > 0)
            {
                DataRow dr = dsSPV.Tables[0].Rows[0];
                //_log.Log(SystemEnums.Severity.Debug, "1");
                 var url = Convert.ToString(dr["SPVManufactureProductVerificationUrl"]);
                //_log.Log(SystemEnums.Severity.Debug, "2 url = " + url);
                if (!string.IsNullOrEmpty(url))
                {
                    DataTable dtSerialNumberPhotos = new DataTable();
                    if (dsSPV.Tables.Count > 1 && dsSPV.Tables[1] != null)
                    {
                        dtSerialNumberPhotos = dsSPV.Tables[1];
                    }
                    var dataProduct = string.Empty;
                    //_log.Log(SystemEnums.Severity.Debug, "3 notVerifiedSerialNumber.Count = " + notVerifiedSerialNumber.Count);
                    for (int i = 0; i < notVerifiedSerialNumber.Count; i++)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.PreserveWhitespace = true;
                        xmlDoc.Load(ProductVerificationXMLPath);
                        var element = xmlDoc.GetElementsByTagName("Products");
                        dataProduct += element[0].InnerXml.Replace("[[SerialNumber]]", notVerifiedSerialNumber[i])
                                            .Replace("[[ModelNumber]]", Convert.ToString(dr["Model"]))
                                            .Replace("[[Manufacturer]]", Convert.ToString(dr["Brand"]))
                                            .Replace("[[Supplier]]", Convert.ToString(dr["Supplier"]));

                        if (Convert.ToString(dr["ServiceAdministrator"]) == "FormBay" && dtSerialNumberPhotos.Rows.Count > 0)
                        {
                            if (dtSerialNumberPhotos.Rows.Count > 0)
                            {
                                //string serialNumberPhotoPath = dtSerialNumberPhotos.AsEnumerable().Where(a => a.Field<string>("Path").Contains(@"\" + Convert.ToString(dr["SerialNumber"]) + ".")).Select(a => a.Field<string>("Path")).FirstOrDefault();
                                string serialNumberPhotoPath = dtSerialNumberPhotos.AsEnumerable().Where(a => Path.GetFileName(a.Field<string>("Path")).Contains(Convert.ToString(dr["SerialNumber"]))).Select(a => a.Field<string>("Path")).FirstOrDefault();                               

                                string optimizedImagePath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, Path.GetDirectoryName(serialNumberPhotoPath), Path.GetFileNameWithoutExtension(serialNumberPhotoPath) + ProjectConfiguration.ShortNameOfSerialNumberPhoto + Path.GetExtension(serialNumberPhotoPath));
                                string fullPath = File.Exists(optimizedImagePath) ? optimizedImagePath : Path.Combine(ProjectConfiguration.ProofDocumentsURL, serialNumberPhotoPath);
                                string base64 = Common.ImageToBase64(fullPath);
                                dataProduct = dataProduct.Replace("[[Image]]", base64);
                            }
                            else
                            {
                                dataProduct = dataProduct.Replace("[[Image]]", "");
                            }
                        }

                        SpvLog objeSpvLog = new SpvLog();
                        objeSpvLog.SerialNumber = notVerifiedSerialNumber[i];
                        objeSpvLog.ModelNumber = Convert.ToString(dr["Model"]);
                        objeSpvLog.Manufacturer = Convert.ToString(dr["Brand"]);
                        objeSpvLog.Supplier = Convert.ToString(dr["Supplier"]);
                        objeSpvLog.JobId = Convert.ToInt32(dr["JobID"]);
                        objeSpvLog.ServiceAdministrator = Convert.ToString(dr["ServiceAdministrator"]);
                        objeSpvLog.SPVMethod = SpvRequest.ProductVerification.GetHashCode();

                        var newRow = VerifiedSerialNumber.NewRow();
                        newRow["SerialNumber"] = notVerifiedSerialNumber[i];
                        //_log.Log(SystemEnums.Severity.Debug, "4 ServerCertificate= " + ServerCertificate);
                        //_log.Log(SystemEnums.Severity.Debug, "5 xmlDoc= " + JsonConvert.SerializeObject(xmlDoc));
                        //_log.Log(SystemEnums.Severity.Debug, "6 dataProduct= " + JsonConvert.SerializeObject(dataProduct));
                        var lst = ProductVerificationSPV(xmlDoc, ServerCertificate, dataProduct, url, objeSpvLog);
                        //_log.Log(SystemEnums.Severity.Debug, "7 lst= " + JsonConvert.SerializeObject(lst));
                        if (lst.Count > 0)
                            newRow["Verified"] = false;
                        else
                            newRow["Verified"] = true;
                        VerifiedSerialNumber.Rows.Add(newRow);
                    }
                    //_log.Log(SystemEnums.Severity.Debug, "8 verifiedSerialNumber= " + JsonConvert.SerializeObject(verifiedSerialNumber));
                    foreach (var item in verifiedSerialNumber)
                    {
                        var newRow = VerifiedSerialNumber.NewRow();
                        newRow["SerialNumber"] = item;
                        newRow["Verified"] = true;
                        VerifiedSerialNumber.Rows.Add(newRow);
                    }
                }
                return VerifiedSerialNumber;
            }
            return new DataTable();
        }
        private DataTable CreateDatatable()
        {
            DataTable VerifiedSerialNumber = new DataTable();
            VerifiedSerialNumber.Columns.Add("SerialNumber", typeof(string));
            VerifiedSerialNumber.Columns.Add("Verified", typeof(bool));
            return VerifiedSerialNumber;
        }

        private DataTable CheckSerialnumberPhotoDatatable()
        {
            DataTable checkSerialnumberPhoto = new DataTable();
            checkSerialnumberPhoto.Columns.Add("SerialNumber", typeof(string));
            checkSerialnumberPhoto.Columns.Add("IsPhotoAvailable", typeof(bool));
            return checkSerialnumberPhoto;
        }

        private XmlNodeList ProductVerificationSPV(XmlDocument xmlDoc,string ServerCertificate,string dataProduct,string url,SpvLog objSpvLog)
        {
            var lstProductElement = xmlDoc.GetElementsByTagName("Products");
            //var xDataProduct = new XCData(dataProduct);
            //lstProductElement[0].InnerXml = Convert.ToString(xDataProduct);
            //dataProduct = dataProduct.Replace("&", "&amp;");
            lstProductElement[0].InnerXml = dataProduct;
            XDocument doc = XDocument.Parse(xmlDoc.InnerXml);
            xmlDoc.InnerXml = doc.ToString();
            string certPath = ServerCertificate;
            string certPass = ProjectConfiguration.ServerCertificatePassword;
            //CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
            X509Certificate2 cert = new X509Certificate2(certPath, certPass, X509KeyStorageFlags.Exportable);
           // RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider();
            var rsaCsp = new RSACryptoServiceProvider(
                new CspParameters(24 /* PROV_RSA_AES */));
            rsaCsp.PersistKeyInCsp = false;
            // store the private key for later (signing)
            var exportedKeyMaterial = cert.PrivateKey.ToXmlString(true);
            rsaCsp.FromXmlString(exportedKeyMaterial);
            SignXml(xmlDoc, rsaCsp, cert,1);
            Console.WriteLine("XML file signed.");
            var content = new StringContent(xmlDoc.OuterXml, Encoding.UTF8, "application/xml");
            string outerxml = xmlDoc.OuterXml;
            var content1 = new StringContent(outerxml, Encoding.UTF8, "application/xml");
            var client = new HttpClient();

            #region Add spv product verification request log
            objSpvLog.RequestTime = DateTime.Now;
            objSpvLog.SPVLogId = _spvLog.InsertOrUpdate(objSpvLog);
            SpvRequestResponseXMLDataStoreInTextFile(objSpvLog, xmlDoc.OuterXml);
            #endregion

            var result = client.PostAsync(url, content1).Result;
            var data = result.Content.ReadAsStringAsync().Result;

            XmlDocument xmlSPVVerificationResult = new XmlDocument();
            xmlSPVVerificationResult.LoadXml(data);

            #region Add spv product verification response log
            var lst = SPVProductVerificationAndInstallationVerificationResponseLog(data, xmlSPVVerificationResult, ref objSpvLog);
            #endregion

            return lst;
        }
        public XmlDocument SPVInstallationVerification(DataSet dsSPV,string InstallationVerificationXMLPath,string ServerCertificate,string url)
        {
            List<string> lstSerialNumbers = new List<string>();
            SpvLog objSpvLog = new SpvLog();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(InstallationVerificationXMLPath);
            var filterDt = dsSPV.Tables[0].Select("SPVManufactureInstallationVerificationUrl = '" + url + "'").CopyToDataTable();
            var dataProduct = string.Empty;

            DataTable dtSerialNumberPhotos = new DataTable();
            if (dsSPV.Tables.Count > 1 && dsSPV.Tables[1] != null)
            {
                dtSerialNumberPhotos = dsSPV.Tables[1];
            }

            for (int i = 0; i < filterDt.Rows.Count; i++)
            {
                var dr = filterDt.Rows[i];
                if (Convert.ToString(dr["ServiceAdministrator"]) != "FormBay")
                {
                    XmlNodeList nodes = xmlDoc.GetElementsByTagName("Image");
                    if(nodes.Count > 0)
                    {
                        xmlDoc.GetElementsByTagName("Products")[0].ChildNodes[1].RemoveChild(nodes[0]);
                    }
                }
               var element = xmlDoc.GetElementsByTagName("Products");
                //var strXml = new XCData(dr["CertificateHolder"].ToString());
                dataProduct += element[0].InnerXml.Replace("[[SerialNumber]]", Convert.ToString(new XCData(dr["SerialNumber"].ToString())))
                                    .Replace("[[ModelNumber]]", Convert.ToString(new XCData(dr["ModelNumber"].ToString())))
                                    .Replace("[[Manufacturer]]", Convert.ToString(new XCData(dr["CertificateHolder"].ToString())))
                                    .Replace("[[Supplier]]", Convert.ToString(new XCData(dr["Supplier"].ToString())));

                if (Convert.ToString(dr["ServiceAdministrator"]) == "FormBay" && dtSerialNumberPhotos.Rows.Count > 0)
                {
                    if (dtSerialNumberPhotos.Rows.Count > 0)
                    {
                        string fullPath = string.Empty;
                        //string serialNumberPhotoPath = dtSerialNumberPhotos.AsEnumerable().Where(a => a.Field<string>("Path").Contains(@"\" + Convert.ToString(dr["SerialNumber"]) + ".")
                        //            || a.Field<string>("Path").Contains(@"\" + Convert.ToString(dr["SerialNumber"]) + "(")
                        //            || a.Field<string>("Path").Contains(@"r_" + Convert.ToString(dr["SerialNumber"]) + ".")).Select(a => a.Field<string>("Path")).FirstOrDefault();

                        string serialNumberPhotoPath = dtSerialNumberPhotos.AsEnumerable().Where(a => Path.GetFileName(a.Field<string>("Path")).Contains(Convert.ToString(dr["SerialNumber"]))).Select(a => a.Field<string>("Path")).FirstOrDefault();

                        if (serialNumberPhotoPath == null)
                        {
                            dataProduct = dataProduct.Replace("[[Image]]", "");
                        }
                        else
                        {
                            string optimizedImagePath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, Path.GetDirectoryName(serialNumberPhotoPath), ProjectConfiguration.ShortNameOfSerialNumberPhoto + Path.GetFileNameWithoutExtension(serialNumberPhotoPath) + Path.GetExtension(serialNumberPhotoPath));
                            if (File.Exists(optimizedImagePath))
                            {
                                fullPath = optimizedImagePath;
                                string base64 = Common.ImageToBase64(fullPath);
                                dataProduct = dataProduct.Replace("[[Image]]", base64);
                            }
                            else
                            {
                                dataProduct = dataProduct.Replace("[[Image]]", "");
                            }
                        }
                    }
                    else
                    {
                        dataProduct = dataProduct.Replace("[[Image]]", "");
                    }
                }
                    lstSerialNumbers.Add(Convert.ToString(dr["SerialNumber"]));
            }
            
            DataRow dataRow = filterDt.Rows[0];
            
            var lstProductElement = xmlDoc.GetElementsByTagName("Products");
            //dataProduct = dataProduct.Replace("&", "&amp;");
            //dataRow["FirstName"] = Convert.ToString(dataRow["FirstName"]).Replace("’", "&apos;");
            //dataRow["LastName"] = Convert.ToString(dataRow["LastName"]).Replace("’", "&apos;");
            lstProductElement[0].InnerXml = dataProduct;
            string solarCompany="", companyABN ="";

            if(Convert.ToBoolean(dataRow["isAllowedMasking"]) == false)
            {
                if (ConfigurationManager.AppSettings["IsSCAStaticInSPV"] == "true")
                {
                    solarCompany = "GREENBOT PTY. LTD.";
                    companyABN = "83614837124";
                }
                else
                {
                    solarCompany = Convert.ToString(new XCData(dataRow["CompanyName"].ToString()));//.Replace("&", "&amp;");
                    companyABN = Convert.ToString(dataRow["CompanyABN"]);
                }
            }
            else
            {
                solarCompany = Convert.ToString(dataRow["ActualCompanyName"]);
                companyABN = Convert.ToString(dataRow["ActualCompanyABN"]);
            }

            string OwnerEmail = (Convert.ToString(dataRow["Email"]) == null || Convert.ToString(dataRow["Email"]) == string.Empty) ? "noemail@greenbot.com.au": Convert.ToString(dataRow["Email"]);
            
            XDocument doc = XDocument.Parse(xmlDoc.InnerXml);
            xmlDoc.InnerXml = doc.ToString();
            xmlDoc.InnerXml = xmlDoc.InnerXml.Replace("[[InstallerAccerdiationNumber]]", Convert.ToString(dataRow["CECAccreditationNumber"]))
                            .Replace("[[InstallerFirstName]]", Convert.ToString(new XCData(dataRow["FirstName"].ToString())))
                            .Replace("[[InstallerLastName]]", Convert.ToString(new XCData(dataRow["LastName"].ToString())))
                            .Replace("[[OwnerEmail]]", Convert.ToString(new XCData(OwnerEmail)))
                            .Replace("[[InstallationStreetNumber]]", Convert.ToString(dataRow["StreetNumber"]))
                            .Replace("[[InstallationStreetName]]", Convert.ToString(dataRow["StreetName"]))
                            .Replace("[[InstallationStreetType]]", Convert.ToString(dataRow["StreetType"]))
                            .Replace("[[InstallationTown]]", Convert.ToString(dataRow["Town"]))
                            .Replace("[[InstallationPostCode]]", Convert.ToString(dataRow["PostCode"]))
                            .Replace("[[InstallationState]]", Convert.ToString(dataRow["State"]))
                            .Replace("[[InstallationDate]]", Convert.ToDateTime(dataRow["InstallationDate"]).ToString("yyyy-MM-ddThh:mm:sszzz"))
                            .Replace("[[Solarcompany]]", Convert.ToString(new XCData(solarCompany)))
                            .Replace("[[CompanyABN]]", companyABN)
                            .Replace("[[Latitude]]", Convert.ToString(dataRow["Latitude"]))
                            .Replace("[[Longitude]]", Convert.ToString(dataRow["Longitude"]))
                            .Replace("[[Altitude]]", Convert.ToString(dataRow["Altitude"]))
                            .Replace("[[Accuracy]]", Convert.ToString(dataRow["Accuracy"]));
            
            string certPath = ServerCertificate;
            string certPass = ProjectConfiguration.ServerCertificatePassword;
            X509Certificate2 cert = new X509Certificate2(certPath, certPass, X509KeyStorageFlags.Exportable);
            RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider();
            rsaCsp.PersistKeyInCsp = false;
            // store the private key for later (signing)
            var exportedKeyMaterial = cert.PrivateKey.ToXmlString(true);
            rsaCsp.FromXmlString(exportedKeyMaterial);
            SignXml(xmlDoc, rsaCsp, cert,2);
            Console.WriteLine("XML file signed.");
            var content = new StringContent(xmlDoc.OuterXml, Encoding.UTF8, "application/xml");
            var client = new HttpClient();

            if (lstSerialNumbers.Any())
            {
                objSpvLog.JobId = Convert.ToInt32(dataRow["JobID"]);
                objSpvLog.Manufacturer = Convert.ToString(dataRow["CertificateHolder"]);
                objSpvLog.ModelNumber = Convert.ToString(dataRow["ModelNumber"]);
                objSpvLog.RequestTime = DateTime.Now;
                objSpvLog.SerialNumber = string.Join(",", lstSerialNumbers);
                objSpvLog.SPVMethod = SpvRequest.InstallationVerification.GetHashCode();
                objSpvLog.Supplier = Convert.ToString(dataRow["Supplier"]);
                objSpvLog.ServiceAdministrator = Convert.ToString(dataRow["ServiceAdministrator"]);
                objSpvLog.SPVLogId = _spvLog.InsertOrUpdate(objSpvLog);
                SpvRequestResponseXMLDataStoreInTextFile(objSpvLog, xmlDoc.OuterXml);
            }
            client.Timeout = TimeSpan.FromSeconds(ProjectConfiguration.SPVTimeOut);
            var result = client.PostAsync(url, content).Result;
            var data = result.Content.ReadAsStringAsync().Result;

            XDocument xdoc = XDocument.Parse(data.ToString(), LoadOptions.PreserveWhitespace);
            //doc.InnerXml = doc.();
            // XmlDocument xmldoc = new XmlDocument();
            XmlDocument xmlSPVVerificationResult = new XmlDocument();
            using (var xmlReader = xdoc.CreateReader())
            {
                xmlSPVVerificationResult.Load(xmlReader);
                xmlReader.Dispose();
            }
            //XmlDocument xmlSPVVerificationResult = new XmlDocument();
            //xmlSPVVerificationResult.LoadXml(data);

            #region Add spv product verification response log
            SPVProductVerificationAndInstallationVerificationResponseLog(data, xmlSPVVerificationResult, ref objSpvLog);
            #endregion
            return xmlSPVVerificationResult;
        }
        /// <summary>
        /// This method will add response log for Installation and product verification request.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xmlSPVVerificationResult"></param>
        /// <param name="objSpvLog"></param>
        /// <returns></returns>
        public XmlNodeList SPVProductVerificationAndInstallationVerificationResponseLog(string data, XmlDocument xmlSPVVerificationResult, ref SpvLog objSpvLog)
        {
            //check if any Code tag is there from response.
            var lst = xmlSPVVerificationResult.GetElementsByTagName("Code");
            try
            {
                if (objSpvLog.SPVLogId > 0)
                {
                    XDocument doc = new XDocument();
                    if (!string.IsNullOrEmpty(data))
                    {
                        doc = XDocument.Parse(data);
                    }
                    //Response is not null and we got any code tag from the response then this code will execute.
                    if (data != null && !string.IsNullOrEmpty(data) && lst != null && lst.Count > 0)
                    {
                        //Get inner xml from errors tag.
                        //var errorxmltag = xmlSPVVerificationResult.GetElementsByTagName("Errors");
                        //XmlSerializer serializer = new XmlSerializer(typeof(SpvErrorCode));
                        //SpvErrorCode objSpvErrorCode = null;


                        var errorxmltag = xmlSPVVerificationResult.GetElementsByTagName("Error");
                        XmlSerializer serializer = new XmlSerializer(typeof(List<SpvErrorCode>));
                        List<SpvErrorCode> objSpvErrorCode = new List<SpvErrorCode>();

                        if (errorxmltag.Count > 0)
                        {
                            //using (TextReader reader = new StringReader(errorxmltag[0].InnerXml))
                            //{
                            //    objSpvErrorCode = (SpvErrorCode)serializer.Deserialize(reader);

                            //}
                            if (!string.IsNullOrEmpty(data))
                            {
                                objSpvErrorCode = doc.Descendants("Error").Select(d =>
                                new SpvErrorCode
                                {
                                    Code = Convert.ToInt32(d.Element("Code").Value),
                                    Details = d.Element("Details").Value,
                                    Description = d.Element("Description").Value

                                }).ToList();
                            }
                        }
                        if (objSpvErrorCode != null)
                        {
                            string code = String.Join(", ", objSpvErrorCode.Select(v => v.Code).Distinct().ToList());
                            string message ="Details:"+ String.Join(", Details: ", objSpvErrorCode.Select(a=>a.Details + " Description: " + a.Description).Distinct().ToList());
                            objSpvLog.ResponseCode = code;
                            objSpvLog.ResponseMessage = message;
                            //objSpvLog.ResponseCode = objSpvErrorCode.Code.ToString();
                            //objSpvLog.ResponseMessage = "Details: " + objSpvErrorCode.Details + (!string.IsNullOrEmpty(objSpvErrorCode.Description) ? " Description: " + objSpvErrorCode.Description : "");
                        }
                        else
                        {
                            objSpvLog.ResponseMessage = $"Response formart is not proper. Responce data = {JsonConvert.SerializeObject(data)}";
                        }
                        objSpvLog.VerificationStatus = SpvVerificationStatus.Fail.GetHashCode();
                    }
                    else if (data == null || data == "")
                    {
                        objSpvLog.ResponseMessage = "Get empty response.";
                        objSpvLog.VerificationStatus = SpvVerificationStatus.Fail.GetHashCode();
                    }
                    else
                    {
                        objSpvLog.VerificationStatus = SpvVerificationStatus.Success.GetHashCode();
                    }
                    objSpvLog.ResponseTime = DateTime.Now;
                    objSpvLog.SPVLogId = _spvLog.InsertOrUpdate(objSpvLog);
                    SpvRequestResponseXMLDataStoreInTextFile(objSpvLog, xmlSPVVerificationResult.InnerXml);
                }
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"Method Name: SPVProductVerificationAndInstallationVerificationResponseLog :: JobId = {objSpvLog.JobId}", ex);
            }
            return lst;
        }
        public void SpvRequestResponseXMLDataStoreInTextFile(SpvLog objSpvLog,string XMLContent)
        {
            try
            {
                if (ProjectConfiguration.IsSpvRequestResponseResultSaveInTextFile)
            {
                string path = $"{ProjectConfiguration.ProofDocumentsURL}JobDocuments/{objSpvLog.JobId}/SpvLogs";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = $"{path}/{objSpvLog.SPVLogId}_{(SpvRequest.InstallationVerification.GetHashCode() == objSpvLog.SPVMethod ? "IV" : "PV")}{(objSpvLog.ResponseTime == null ?"Request": "Response")}.txt";
                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                        //StreamWriter writer = new StreamWriter(path);
                        //writer.Write(XMLContent);
                        //writer.Close();

                        // Create a file to write to.
                        using (FileStream sw = File.Create(path))
                        {
                            Byte[] data = new UTF8Encoding(true).GetBytes(XMLContent);
                            sw.Write(data,0,data.Length);
                            //sw.WriteLine(XMLContent);
                            sw.Close();
                        }
                    }
            }
            }
            catch (Exception ex)
            {
                _log.LogException(Severity.Error, "MethodName: SpvRequestResponseXMLDataStoreInTextFile", ex);
            }
        }
        public bool ReduceSizeOfSerialnumberPhoto(string imagePath, string latitude =null, string longitude = null, string imageTakenDate=null)
        {
            FileStream fromStream = null;
            FileStream toStream = null;
            try
            {
                double scaleFactor = ProjectConfiguration.scaleFactor;
                fromStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                toStream = new FileStream(Path.Combine(Path.GetDirectoryName(imagePath), ProjectConfiguration.ShortNameOfSerialNumberPhoto + Path.GetFileNameWithoutExtension(imagePath) + Path.GetExtension(imagePath)), FileMode.Create);

                var image = Image.FromStream(fromStream);
                var newWidth = (int)(image.Width * scaleFactor);
                var newHeight = (int)(image.Height * scaleFactor);
                var thumbnailBitmap = new Bitmap(newWidth, newHeight);

                var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
                thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbnailGraph.DrawImage(image, imageRectangle);

                try
                {
                    Image imageWithProperties = Geotag(thumbnailBitmap, Double.Parse(string.IsNullOrEmpty(latitude) ? "0" : latitude), Double.Parse(string.IsNullOrEmpty(longitude) ? "0" : longitude), imageTakenDate);
                    imageWithProperties.Save(toStream, image.RawFormat);
                    thumbnailGraph.Dispose();
                    thumbnailBitmap.Dispose();
                    image.Dispose();
                    fromStream.Dispose();
                    toStream.Dispose();
                }
                catch (Exception ex)
                {
                    thumbnailGraph.Dispose();
                    thumbnailBitmap.Dispose();
                    image.Dispose();
                    fromStream.Dispose();
                    toStream.Dispose();
                    return false;
                }

                //thumbnailBitmap.Save(toStream, image.RawFormat);

                //thumbnailGraph.Dispose();
                //thumbnailBitmap.Dispose();
                //image.Dispose();
                //fromStream.Dispose();
                //toStream.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                fromStream.Dispose();
                _log.LogException(SystemEnums.Severity.Error, $"Method Name: ReduceSizeOfSerialnumberPhoto :: ImagePath = {imagePath}", ex);
                return false;
            }
        }

        public DataTable SetSerialnumberWithPhotoUnavaibility(DataTable checkSerialnumberPhoto, string serialnumber)
        {
            //DataTable checkSerialnumberPhoto = CheckSerialnumberPhotoDatatable();
            var newRow = checkSerialnumberPhoto.NewRow();
            newRow["SerialNumber"] = serialnumber;
            newRow["IsPhotoAvailable"] = false;
            checkSerialnumberPhoto.Rows.Add(newRow);
            return checkSerialnumberPhoto;
        }

        public Image Geotag(Image original, double lat, double lng, string ImageTakenDate = "")
        {
            // These constants come from the CIPA DC-008 standard for EXIF 2.3
            const short ExifTypeByte = 1;
            const short ExifTypeAscii = 2;
            const short ExifTypeRational = 5;

            const int ExifTagGPSVersionID = 0x0000;
            const int ExifTagGPSLatitudeRef = 0x0001;
            const int ExifTagGPSLatitude = 0x0002;
            const int ExifTagGPSLongitudeRef = 0x0003;
            const int ExifTagGPSLongitude = 0x0004;
            const int ExifTagDateTimeOriginal = 0x9003;
            //const int ExifTagDateTaken = 0x0132;

            char latHemisphere = 'N';
            if (lat < 0)
            {
                latHemisphere = 'S';
                lat = -lat;
            }
            char lngHemisphere = 'E';
            if (lng < 0)
            {
                lngHemisphere = 'W';
                lng = -lng;
            }

            MemoryStream ms = new MemoryStream();
            original.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);

            Image img = Image.FromStream(ms);
            AddProperty(img, ExifTagGPSVersionID, ExifTypeByte, new byte[] { 2, 3, 0, 0 });
            AddProperty(img, ExifTagGPSLatitudeRef, ExifTypeAscii, new byte[] { (byte)latHemisphere, 0 });
            AddProperty(img, ExifTagGPSLatitude, ExifTypeRational, ConvertToRationalTriplet(lat));
            AddProperty(img, ExifTagGPSLongitudeRef, ExifTypeAscii, new byte[] { (byte)lngHemisphere, 0 });
            AddProperty(img, ExifTagGPSLongitude, ExifTypeRational, ConvertToRationalTriplet(lng));


            // ImageTakenDate : Date Format must be "yyyy:MM:dd HH:mm:ss"
            byte[] arrProp = null;
            if (string.IsNullOrEmpty(ImageTakenDate))
                ImageTakenDate = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");

            arrProp = Encoding.ASCII.GetBytes(ImageTakenDate);
            AddProperty(img, ExifTagDateTimeOriginal, ExifTypeAscii, arrProp);

            return img;
        }

        public void AddProperty(Image img, int id, short type, byte[] value)
        {
            try
            {
                PropertyItem pi = img.PropertyItems[0];
                pi.Id = id;
                pi.Type = type;
                pi.Len = value.Length;
                pi.Value = value;
                img.SetPropertyItem(pi);
            }
            catch (Exception ex)
            {
                _log.LogException(Severity.Error, "AddProperty: store lat lon in compressed image", ex);
            }
        }

        public byte[] ConvertToRationalTriplet(double value)
        {
            int degrees = (int)Math.Floor(value);
            value = (value - degrees) * 60;
            int minutes = (int)Math.Floor(value);
            value = (value - minutes) * 60 * 100;
            int seconds = (int)Math.Round(value);
            byte[] bytes = new byte[3 * 2 * 4]; // Degrees, minutes, and seconds, each with a numerator and a denominator, each composed of 4 bytes
            int i = 0;
            Array.Copy(BitConverter.GetBytes(degrees), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(1), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(minutes), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(1), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(seconds), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(100), 0, bytes, i, 4);
            return bytes;
        }

        #endregion
    }
}
