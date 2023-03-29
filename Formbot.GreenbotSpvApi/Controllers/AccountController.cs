using Formbot.GreenbotSpvApi.Models;
using FormBot.BAL;
using FormBot.Entity;
using FormBot.Helper.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using static FormBot.Helper.SystemEnums;

namespace Formbot.GreenbotSpvApi.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// Verify Product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage VerifyProduct(HttpRequestMessage request)
        {
            string sSyncData = "";
            string errorContent = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/XmlFileTemplate/error.xml"));
            try
            {
                string data = request.Content.ReadAsStringAsync().Result;
                if (data != "")
                {
                    string rawHtml = System.Net.WebUtility.HtmlDecode(data);
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.LoadXml(rawHtml);
                    #region 104 (Xml not valid)
                    int ProductListCount = 0;
                    //string["TagName", "Length of tags"] here 0 length stands for multiple tags allow
                    string[,] MandatoryTagsList = { { "ProductsVerificationRequest", "1" }, { "ProductsVerified", "1" }, { "Products", "1" }, { "Product", "0" }, { "Requestor", "1" }, { "Signature", "1" } };
                    string MandatoryTagsMissingList = CheckMandatoryTagsList(MandatoryTagsList, xmlDoc, ref ProductListCount);
                    if (!string.IsNullOrEmpty(MandatoryTagsMissingList))
                    {
                        SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.XMLNotValid, "", MandatoryTagsMissingList);
                        goto ErrorFound;
                    }
                    #endregion 104 (Xml not valid)

                    #region 103 (Mandatory field xxx missing)
                    string[,] MandatoryFieldsList = { { "RequestedDateTime", "0" }, { "SerialNumber", "1" }, { "ModelNumber", "1" }, { "Manufacturer", "1" }, { "ResponsibleSupplier", "1" }, { "Name", "0" }, { "ABN", "0" } };
                    string MandatoryFieldsMissingList = CheckMandatoryFieldsList(MandatoryFieldsList, xmlDoc, ref ProductListCount);
                    if (!string.IsNullOrEmpty(MandatoryFieldsMissingList))
                    {
                        SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.MandatoryFieldMissing, "", MandatoryFieldsMissingList);
                        goto ErrorFound;
                    }
                    #endregion 103 (Mandatory field xxx missing)


                    if (checksigntureFromXML(xmlDoc))
                    {
                        SpvVerificationRequest spvVerificationRequest = new SpvVerificationRequest();
                        SpvVerificationBAL _spvVerificationBAL = new SpvVerificationBAL();



                        #region 101 (Verification of Manufacturer not available)
                        XmlNodeList nodelist = xmlDoc.GetElementsByTagName("Product");
                        spvVerificationRequest.ProductList = spvProductVerifyManufacturer(nodelist);
                        if (spvVerificationRequest.ProductList.Count == 0)
                        {
                            SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.VerificationOfManufacturerNotAvailable);
                            goto ErrorFound;
                        }
                        #endregion 101 (Verification of Manufacturer not available)

                        #region 102 (Serial Number not valid)
                        spvVerificationRequest.ModelList = spvVerificationModelDetails(spvVerificationRequest.ProductList);

                      





                        if (spvVerificationRequest.ModelList.Any())
                        {
                            //if(spvVerificationRequest.ModelList.Count>0 && spvVerificationRequest.ModelList.FirstOrDefault().IsInstallationVerified)
                            if (spvVerificationRequest.ModelList.Any(x => x.IsInstallationVerified))
                            {
                                string serialNumbers = string.Join(",", spvVerificationRequest.ModelList.Select(i => i.SerialNumber).ToArray());
                                SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.SerialNumberAlreadyVerified, "", serialNumbers);
                                goto ErrorFound;
                            }
                            //if (spvVerificationRequest.ModelList.Count > 0 && !spvVerificationRequest.ModelList.FirstOrDefault().IsValid)
                            if (!spvVerificationRequest.ModelList.Any(x => x.IsValid))
                            {
                                string serialNumbers = string.Join(",", spvVerificationRequest.ModelList.Select(i => i.SerialNumber).ToArray());
                                SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.SerialNumberNotValid, "", serialNumbers);
                                goto ErrorFound;
                            }
                        }
                        #endregion 102 (Serial Number not valid)

                        #region Generate Installer Verification Response

                        XmlDocument docNew = new XmlDocument();
                        XmlElement newRoot = docNew.CreateElement("ProductsVerificationResponse");
                        docNew.AppendChild(newRoot);
                        XmlNodeList ProductsVerificationRequestNode = xmlDoc.GetElementsByTagName("ProductsVerificationRequest");
                        newRoot.InnerXml = ProductsVerificationRequestNode[0].InnerXml;
                        xmlDoc.DocumentElement.ParentNode.RemoveAll();
                        xmlDoc = docNew;
                        xmlDoc.DocumentElement.RemoveChild(xmlDoc.DocumentElement.LastChild);

                        XmlNodeList productverification = xmlDoc.GetElementsByTagName("ProductsVerified");
                        XmlNode verifieddateNode = xmlDoc.CreateElement("VerifiedDateTime");
                        verifieddateNode.InnerText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK");
                        productverification[0].ReplaceChild(verifieddateNode, productverification[0].ChildNodes[1]);
                        XmlNode nodeVerified = xmlDoc.SelectSingleNode("ProductsVerificationResponse/ProductsVerified");

                        nodeVerified.Attributes[0].Value = "verifiedproducts";

                        XmlNode modelElement = xmlDoc.CreateElement("Models");
                        productverification[0].AppendChild(modelElement);
                        productverification[0].AppendChild(modelElement).InnerText = "\n";


                        XmlNodeList productlist = xmlDoc.GetElementsByTagName("Product");
                        //for(int i = 0; i < spvVerificationRequest.ProductList.Count; i++)
                        //{
                        //    var newelement = xmlDoc.CreateElement("FlashTest");
                        //    newelement.InnerText = "Name, Voc(V), Isc(A), Pm(W), Vm(V), Im(A), FF(%) \r\n " + spvVerificationRequest.ProductList[i].SerialNumber + ", " + spvVerificationRequest.ProductList[i].VOC + ", " + spvVerificationRequest.ProductList[i].ISC + ", " + spvVerificationRequest.ProductList[i].PM + ", " + spvVerificationRequest.ProductList[i].VM + ", " + spvVerificationRequest.ProductList[i].IM + ", " + spvVerificationRequest.ProductList[i].FF + "\r\n";
                        //    productlist[i].AppendChild(newelement);

                        //}

                        for (int j = 0; j < spvVerificationRequest.ModelList.Count; j++)
                        {
                            var newelement = xmlDoc.CreateElement("FlashTest");
                            newelement.InnerText = "Name, Voc(V), Isc(A), Pm(W), Vm(V), Im(A), FF(%) \r\n " + spvVerificationRequest.ModelList[j].SerialNumber + ", " + spvVerificationRequest.ModelList[j].VOC + ", " + spvVerificationRequest.ModelList[j].ISC + ", " + spvVerificationRequest.ModelList[j].PM + ", " + spvVerificationRequest.ModelList[j].VM + ", " + spvVerificationRequest.ModelList[j].IM + ", " + spvVerificationRequest.ModelList[j].FF + "\r\n";
                            productlist[j].AppendChild(newelement);
                        }
                        List<SpvPanelDetails> lstDistinctSpvPanelDetails = new List<SpvPanelDetails>();
                        lstDistinctSpvPanelDetails = spvVerificationRequest.ModelList.GroupBy(x => new { x.ModelNumber,
                            x.Manufacturer,
                            x.BillOfMaterials,
                            x.FactoryLocation,
                            x.FactoryName,
                            x.Wattage }).Select(x => x.First()).Distinct().ToList();



                        //lstDistinctSpvPanelDetails = spvVerificationRequest.ModelList.Select(x => new SpvPanelDetails
                        //{
                        //    ModelNumber = x.ModelNumber
                        //    ,
                        //    Manufacturer = x.Manufacturer
                        //    ,
                        //    BillOfMaterials = x.BillOfMaterials,
                        //    FactoryLocation = x.FactoryLocation,
                        //    FactoryName = x.FactoryName,
                        //    Wattage = x.Wattage
                        //}).Distinct().ToList();
                        

                        for (int j = 0; j < lstDistinctSpvPanelDetails.Count; j++)
                        {
                            XmlNode model = xmlDoc.CreateElement("Model");
                            model.InnerText = "\n";
                            XmlNode modelnumber = xmlDoc.CreateElement("ModelNumber");
                            modelnumber.InnerText = spvVerificationRequest.ModelList[j].ModelNumber;
                            XmlNode manufacturer = xmlDoc.CreateElement("Manufacturer");
                            manufacturer.InnerText = spvVerificationRequest.ModelList[j].Manufacturer;
                            XmlNode BillOfMaterials = xmlDoc.CreateElement("BillOfMaterials");
                            BillOfMaterials.InnerText = spvVerificationRequest.ModelList[j].BillOfMaterials;
                            XmlNode FactoryLocation = xmlDoc.CreateElement("FactoryLocation");
                            FactoryLocation.InnerText = spvVerificationRequest.ModelList[j].FactoryLocation;
                            XmlNode FactoryName = xmlDoc.CreateElement("FactoryName");
                            FactoryName.InnerText = spvVerificationRequest.ModelList[j].FactoryName;
                            XmlNode Wattage = xmlDoc.CreateElement("Wattage");
                            Wattage.InnerText = spvVerificationRequest.ModelList[j].Wattage.ToString();

                            modelElement.AppendChild(model);
                            model.AppendChild(modelnumber);
                            model.AppendChild(manufacturer);
                            model.AppendChild(BillOfMaterials);
                            model.AppendChild(FactoryName);
                            model.AppendChild(FactoryLocation);
                            model.AppendChild(Wattage);
                        }

                        /*for signature in doc*/
                        SignXml(xmlDoc);
                        /*end signature */

                        #endregion Generate Installer Verification Response

                        sSyncData = xmlDoc.OuterXml;
                    }
                    else
                    {
                        #region 100 (Invalid Signature)
                        SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.SignatureNotValid);
                        goto ErrorFound;
                        #endregion 100 (Invalid Signature)
                    }
                }
            ErrorFound:
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StringContent(sSyncData, Encoding.UTF8, "application/xml");
                return response;
            }
            catch (Exception ex)
            {
                #region 500(Internal Server Error)
                string msg = ex.Message;
                SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.InternalError, "", msg);
                #endregion 500(Internal Server Error)

                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StringContent(sSyncData, Encoding.UTF8, "application/xml");
                return response;
            }
        }

        /// <summary>
        /// Verify Installation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage VerifyInstallation(HttpRequestMessage request)
        {
            string sSyncData = "", errorContent = "";
            try
            {
                string data = request.Content.ReadAsStringAsync().Result;
                errorContent = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/XmlFileTemplate/error.xml"));

                if (data != "")
                {
                    string rawHtml = System.Net.WebUtility.HtmlDecode(data);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.LoadXml(rawHtml);
                    #region 104 (Xml not valid)
                    int ProductListCount = 0;
                    //string["TagName", "Length of tags"] here 0 length stands for multiple tags allow
                    string[,] MandatoryTagsList = { { "InstallationProductVerificationRequest", "1" }, { "InstallationProducts", "1" }, { "Installer", "1" }, { "InstallationAddress", "1" }, { "Location", "1" }, { "Products", "1" }, { "Product", "0" }, { "Retailer", "1" }, { "Signature", "1" } };
                    string MandatoryTagsMissingList = CheckMandatoryTagsList(MandatoryTagsList, xmlDoc, ref ProductListCount);
                    if (!string.IsNullOrEmpty(MandatoryTagsMissingList))
                    {
                        SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.XMLNotValid, "", MandatoryTagsMissingList);
                        goto ErrorFound;
                    }
                    #endregion 104 (Xml not valid)

                    #region 103 (Mandatory field xxx missing)
                    string[,] MandatoryFieldsList = { { "ID", "0" }, { "FirstName", "0" }, { "LastName", "0" }, { "StreetName", "0" }, { "StreetType", "0" }, { "Suburb", "0" }, { "Postcode", "0" }, { "State", "0" }, { "ManuallyEntered", "0" }, { "InstallationDate", "0" }, { "SerialNumber", "1" }, { "ModelNumber", "1" }, { "Manufacturer", "1" }, { "ResponsibleSupplier", "1" }, { "Name", "0" }, { "ABN", "0" } };
                    string MandatoryFieldsMissingList = CheckMandatoryFieldsList(MandatoryFieldsList, xmlDoc, ref ProductListCount);
                    if (!string.IsNullOrEmpty(MandatoryFieldsMissingList))
                    {
                        SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.MandatoryFieldMissing, "", MandatoryFieldsMissingList);
                        goto ErrorFound;
                    }
                    #endregion 103 (Mandatory field xxx missing)

                    #region 105(Town/State/PostCode not valid)
                    string ret = string.Empty;
                    string Town = xmlDoc.GetElementsByTagName("Suburb")[0].InnerXml;
                    string State = xmlDoc.GetElementsByTagName("State")[0].InnerXml;
                    string Postcode = xmlDoc.GetElementsByTagName("Postcode")[0].InnerXml;
                    var webRequest = System.Net.WebRequest.Create(string.Format("https://auspost.com.au/api/postcode/search.json?q=" + Town + "&excludePostBoxFlag=true"));

                    if (webRequest != null)
                    {
                        webRequest.Headers.Add("AUTH-KEY", "0344e02f-843b-49a7-8fd6-d35acd471480");
                        webRequest.Method = "GET";
                        webRequest.Timeout = 20000;
                        webRequest.ContentType = "application/json";
                    }

                    HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
                    Stream resStream = resp.GetResponseStream();
                    StreamReader reader = new StreamReader(resStream);
                    ret = reader.ReadToEnd();
                    ret = ret.Replace(@"\", "");
                    if (!ret.Contains("["))
                    {
                        ret = ret.Replace("{\"c", "[{\"c");
                        ret = ret.Replace("}}}", "}]}}");
                    }
                    var res = JsonConvert.DeserializeObject<RootObject>(ret);

                    
                    if (res.localities == null || !ret.Contains(Town.ToUpper()) || !ret.Contains(State.ToUpper()) || !ret.Contains(Postcode))
                    {
                        SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.Town_State_PostCodeNotValid);

                    }
                    else
                    {
                       if(!res.localities.locality.Any(x => x.location == Town.ToUpper() && x.postcode == Postcode && x.state == State.ToUpper()))
                        {
                            SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.Town_State_PostCodeNotValid);
                            goto ErrorFound;
                        }
                    }
                    
                   
                    #endregion
                    if (checksigntureFromXML(xmlDoc))
                    {
                        SpvInstallationVerification spvInstallationVerification = new SpvInstallationVerification();
                        SpvVerificationBAL _spvVerificationBAL = new SpvVerificationBAL();



                        #region 101 (Verification of Manufacturer not available)
                        XmlNodeList productNodes = xmlDoc.GetElementsByTagName("Product");
                        spvInstallationVerification.ProductList = spvProductVerifyManufacturer(productNodes);
                        if (spvInstallationVerification.ProductList.Count == 0)
                        {
                            SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.VerificationOfManufacturerNotAvailable);
                            goto ErrorFound;
                        }
                        #endregion 101 (Verification of Manufacturer not available)

                        #region 102 (Serial Number not valid)
                        spvInstallationVerification.ModelList = spvVerificationModelDetails(spvInstallationVerification.ProductList);
                        if (spvInstallationVerification.ModelList.Any())
                        {
                            if (spvInstallationVerification.ModelList.Any(x => x.IsInstallationVerified))
                            {
                                string serialNumbers = string.Join(",", spvInstallationVerification.ModelList.Select(i => i.SerialNumber).ToArray());
                                SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.SerialNumberAlreadyVerified, "", serialNumbers);
                                goto ErrorFound;
                            }
                            if (spvInstallationVerification.ModelList.Count > 0 && !spvInstallationVerification.ModelList.FirstOrDefault().IsValid)
                            {
                                //SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.SerialNumberNotValid, "Serial number " + spvVerificationRequest.ProductList[i].SerialNumber + " was not found");

                                string serialNumbers = string.Join(",", spvInstallationVerification.ModelList.Select(i => i.SerialNumber).ToArray());
                                SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.SerialNumberNotValid, "", serialNumbers);
                                goto ErrorFound;
                            }
                        }
                        
                        
                        #endregion 102 (Serial Number not valid)

                        #region Generate Installer Verification Response
                        XmlNodeList nodeListInstallationProducts = xmlDoc.GetElementsByTagName("InstallationProducts");


                        XmlDocument docNew = new XmlDocument();
                        XmlElement eleInstallationProductVerificationResponse = docNew.CreateElement("InstallationProductVerificationResponse");
                        docNew.AppendChild(eleInstallationProductVerificationResponse);

                        XmlNode nodeInstallationProductVerification = docNew.CreateElement("InstallationProductVerification");
                        nodeInstallationProductVerification.InnerXml = nodeListInstallationProducts[0].InnerXml;
                        eleInstallationProductVerificationResponse.AppendChild(nodeInstallationProductVerification);
                        XmlAttribute attrId = docNew.CreateAttribute("id");
                        attrId.Value = "verifiedproducts";
                        nodeInstallationProductVerification.Attributes.Append(attrId);

                        XmlNode nodeVerifiedDateTime = docNew.CreateElement("VerifiedDateTime");
                        nodeVerifiedDateTime.InnerText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK");
                        XmlNode nodeInstaller = nodeInstallationProductVerification.SelectSingleNode("Installer");//docNew.SelectSingleNode("Installer");
                        nodeInstallationProductVerification.InsertBefore(nodeVerifiedDateTime, nodeInstaller);

                        xmlDoc = docNew;

                        XmlNodeList nodeListInstallationProductsVerification = xmlDoc.GetElementsByTagName("InstallationProductVerification");
                        XmlNodeList nodeListProduct = xmlDoc.GetElementsByTagName("Product");

                        XmlNode modelElement = xmlDoc.CreateElement("Models");
                        nodeListInstallationProductsVerification[0].AppendChild(modelElement);
                        nodeListInstallationProductsVerification[0].AppendChild(modelElement).InnerText = "\n";


                        for (int i = 0; i < spvInstallationVerification.ModelList.Count; i++)
                        {
                            var newelement = xmlDoc.CreateElement("FlashTest");
                            newelement.InnerText = "Name, Voc(V), Isc(A), Pm(W), Vm(V), Im(A), FF(%) \r\n " + spvInstallationVerification.ModelList[i].SerialNumber + ", " + spvInstallationVerification.ModelList[i].VOC + ", " + spvInstallationVerification.ModelList[i].ISC + ", " + spvInstallationVerification.ModelList[i].PM + ", " + spvInstallationVerification.ModelList[i].VM + ", " + spvInstallationVerification.ModelList[i].IM + ", " + spvInstallationVerification.ModelList[i].FF + "\r\n";
                            nodeListProduct[i].AppendChild(newelement);
                        }
                        List<SpvPanelDetails> lstDistinctSpvPanelDetails = new List<SpvPanelDetails>();
                        lstDistinctSpvPanelDetails = spvInstallationVerification.ModelList.GroupBy(x => new {
                            x.ModelNumber,
                            x.Manufacturer,
                            x.BillOfMaterials,
                            x.FactoryLocation,
                            x.FactoryName,
                            x.Wattage
                        }).Select(x => x.First()).Distinct().ToList();


                        for (int j = 0; j < lstDistinctSpvPanelDetails.Count; j++)
                        { 
                            XmlNode model = xmlDoc.CreateElement("Model");
                            model.InnerText = "\n";
                            XmlNode modelnumber = xmlDoc.CreateElement("ModelNumber");
                            modelnumber.InnerText = spvInstallationVerification.ModelList[j].ModelNumber;
                            XmlNode manufacturer = xmlDoc.CreateElement("Manufacturer");
                            manufacturer.InnerText = spvInstallationVerification.ModelList[j].Manufacturer;
                            XmlNode BillOfMaterials = xmlDoc.CreateElement("BillOfMaterials");
                            BillOfMaterials.InnerText = spvInstallationVerification.ModelList[j].BillOfMaterials;
                            XmlNode FactoryLocation = xmlDoc.CreateElement("FactoryLocation");
                            FactoryLocation.InnerText = spvInstallationVerification.ModelList[j].FactoryLocation;
                            XmlNode FactoryName = xmlDoc.CreateElement("FactoryName");
                            FactoryName.InnerText = spvInstallationVerification.ModelList[j].FactoryName;
                            XmlNode Wattage = xmlDoc.CreateElement("Wattage");
                            Wattage.InnerText = spvInstallationVerification.ModelList[j].Wattage.ToString();

                            modelElement.AppendChild(model);
                            model.AppendChild(modelnumber);
                            model.AppendChild(manufacturer);
                            model.AppendChild(BillOfMaterials);
                            model.AppendChild(FactoryName);
                            model.AppendChild(FactoryLocation);
                            model.AppendChild(Wattage);
                        }

                        _spvVerificationBAL.InsertSpvVerifiedSerialNos(string.Join(",",spvInstallationVerification.ModelList.Select(X=>X.SerialNumber)),0);

                        SignXml(xmlDoc);
                        #endregion Generate Installer Verification Response

                        sSyncData = xmlDoc.OuterXml;
                    }
                    else
                    {
                        #region 100 (Invalid Signature)
                        SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.SignatureNotValid);
                        goto ErrorFound;
                        #endregion 100 (Invalid Signature)
                    }

                }
            ErrorFound:
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StringContent(sSyncData, Encoding.UTF8, "application/xml");
                return response;
            }
            catch (Exception ex)
            {
                #region 500(Internal Server Error)
                string msg = ex.Message;
                SetErrorDetailsInResponse(ref errorContent, ref sSyncData, SpvServiceReponceStatus.InternalError, "", msg);
                #endregion 500(Internal Server Error)

                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StringContent(sSyncData, Encoding.UTF8, "application/xml");
                return response;
            }
        }

        /// <summary>
        /// Set Error Details in Response
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="sSyncData"></param>
        /// <param name="status"></param>
        /// <param name="Details"></param>
        /// <param name="parametersOfDescription"></param>
        public void SetErrorDetailsInResponse(ref string xml, ref string sSyncData, SpvServiceReponceStatus status, string Details = "", params object[] parametersOfDescription)
        {
            xml = xml.Replace("[[Code]]", ((int)status).ToString());
            xml = xml.Replace("[[Description]]", GetDescription(status));
            if (parametersOfDescription.Any())
                xml = xml.Replace("[[Details]]", string.Format(GetSubDescription(status), parametersOfDescription));
            else
                xml = xml.Replace("<Details>[[Details]]</Details>", "");
            sSyncData = xml;
        }
        [System.Web.Http.HttpPost]

        /// <summary>
        /// Check signature from XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        [NonAction]
        public bool checksigntureFromXML(XmlDocument xmlDoc)
        {
            try
            {
                bool passes = false;
                SignedXml signedXml = new SignedXml(xmlDoc);
                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
                XmlNodeList certificates = xmlDoc.GetElementsByTagName("X509Certificate");
                X509Certificate2 dcert2 = new X509Certificate2(Convert.FromBase64String(certificates[0].InnerText));
                foreach (XmlElement element in nodeList)
                {
                    signedXml.LoadXml(element);
                    passes = signedXml.CheckSignature(dcert2, true);
                }
                return passes;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// SPV Product Verify Manufacturer
        /// </summary>
        /// <param name="productNodes"></param>
        /// <returns></returns>
        public List<SpvVerificationProduct> spvProductVerifyManufacturer(XmlNodeList productNodes)
        {
            List<SpvVerificationProduct> lstproduct = new List<SpvVerificationProduct>();
            SpvVerificationBAL _spvVerificationBAL = new SpvVerificationBAL();

            foreach (XmlElement element in productNodes)
            {
                SpvVerificationProduct verificationProduct = new SpvVerificationProduct();
                verificationProduct.SerialNumber = element.GetElementsByTagName("SerialNumber").Count > 0 ? (element.GetElementsByTagName("SerialNumber"))[0].InnerXml : "";
                verificationProduct.Manufacturer = element.GetElementsByTagName("Manufacturer").Count > 0 ? (element.GetElementsByTagName("Manufacturer"))[0].InnerXml : "";
                verificationProduct.ModelNumber = element.GetElementsByTagName("ModelNumber").Count > 0 ? (element.GetElementsByTagName("ModelNumber"))[0].InnerXml : "";
                verificationProduct.Supplier = element.GetElementsByTagName("ResponsibleSupplier").Count > 0 ? (element.GetElementsByTagName("ResponsibleSupplier"))[0].InnerXml : "";

                if (_spvVerificationBAL.CheckManufacturerIsExsistOrNot(verificationProduct.Manufacturer))
                {
                    lstproduct.Add(verificationProduct);
                }
            }
            return lstproduct;
        }

        /// <summary>
        /// SPV Verification of Model Details
        /// </summary>
        /// <param name="productList"></param>
        /// <returns></returns>
        public List<SpvPanelDetails> spvVerificationModelDetails(List<SpvVerificationProduct> productList)
        {
            List<SpvPanelDetails> lstValidModel = new List<SpvPanelDetails>();
            List<SpvPanelDetails> lstInvalidModel = new List<SpvPanelDetails>();
            SpvVerificationBAL _spvVerificationBAL = new SpvVerificationBAL();

            for (int i = 0; i < productList.Count; i++)
            {
                string ModelNumber = productList[i].ModelNumber;
                string Manufacturer = productList[i].Manufacturer;
                string SerialNumber = productList[i].SerialNumber;

                SpvPanelDetails paneldata = _spvVerificationBAL.GetSpvPanelDetails(SerialNumber, ModelNumber, productList[i].Supplier, Manufacturer).FirstOrDefault();
                if (paneldata.IsVerified == 0)
                {
                    if (paneldata != null && paneldata.Result == 1)
                    {
                        //var newelement = xmlDoc.CreateElement("FlashTest");
                        //newelement.InnerText = "Name, Voc(V), Isc(A), Pm(W), Vm(V), Im(A), FF(%) \r\n " + paneldata.SerialNumber + ", " + paneldata.VOC + ", " + paneldata.ISC + ", " + paneldata.PM + ", " + paneldata.VM + ", " + paneldata.IM + ", " + paneldata.FF + "\r\n";
                        //nodelist[i].AppendChild(newelement);

                        if (!lstValidModel.Any(x => x.Manufacturer == Manufacturer && x.ModelNumber == ModelNumber && x.SerialNumber == SerialNumber))
                        {
                            SpvPanelDetails spvValidPanelDetails = new SpvPanelDetails();
                            spvValidPanelDetails.Manufacturer = Manufacturer;
                            spvValidPanelDetails.ModelNumber = ModelNumber;
                            spvValidPanelDetails.BillOfMaterials = paneldata.BillOfMaterials;
                            spvValidPanelDetails.FactoryLocation = paneldata.FactoryLocation;
                            spvValidPanelDetails.FactoryName = paneldata.FactoryName;
                            spvValidPanelDetails.Wattage = paneldata.Wattage;
                            spvValidPanelDetails.VOC = paneldata.VOC;
                            spvValidPanelDetails.ISC = paneldata.ISC;
                            spvValidPanelDetails.PM = paneldata.PM;
                            spvValidPanelDetails.VM = paneldata.VM;
                            spvValidPanelDetails.IM = paneldata.IM;
                            spvValidPanelDetails.FF = paneldata.FF;
                            spvValidPanelDetails.IsValid = true;
                            spvValidPanelDetails.SerialNumber = SerialNumber;
                            lstValidModel.Add(spvValidPanelDetails);
                        }
                    }
                    //if(paneldata!=null && paneldata.IsVerified == 1)
                    //{
                    //    if (!lstInvalidModel.Any(x => x.Manufacturer == Manufacturer && x.ModelNumber == ModelNumber))
                    //    {
                    //        SpvPanelDetails spvInvalidPanelDetails = new SpvPanelDetails();
                    //        spvInvalidPanelDetails.SerialNumber = SerialNumber;
                    //        spvInvalidPanelDetails.IsInstallationVerified = true;

                    //        lstInvalidModel.Add(spvInvalidPanelDetails);

                    //    }

                    //}
                    else
                    {
                        if (!lstInvalidModel.Any(x => x.Manufacturer == Manufacturer && x.ModelNumber == ModelNumber))
                        {
                            SpvPanelDetails spvInvalidPanelDetails = new SpvPanelDetails();
                            spvInvalidPanelDetails.SerialNumber = SerialNumber;
                            spvInvalidPanelDetails.IsValid = false;
                            lstInvalidModel.Add(spvInvalidPanelDetails);
                        }
                    }
                }
                else
                {
                    if (!lstInvalidModel.Any(x => x.Manufacturer == Manufacturer && x.ModelNumber == ModelNumber))
                    {
                        SpvPanelDetails spvInvalidPanelDetails = new SpvPanelDetails();
                        spvInvalidPanelDetails.SerialNumber = SerialNumber;
                        spvInvalidPanelDetails.IsInstallationVerified = true;

                        lstInvalidModel.Add(spvInvalidPanelDetails);

                    }
                }

            }
            return lstInvalidModel.Count > 0 ? lstInvalidModel : lstValidModel;
        }

        /// <summary>
        /// Check Mandatory Tags List
        /// </summary>
        /// <param name="MandatoryTagsList"></param>
        /// <param name="xmlDoc"></param>
        /// <param name="ProductListCount"></param>
        /// <returns></returns>
        public string CheckMandatoryTagsList(string[,] MandatoryTagsList, XmlDocument xmlDoc, ref int ProductListCount)
        {
            string MandatoryTagsMissingList = "";
            int loop = 0;
            {
            AgainStartMendetoryTagsCheck:
                if (loop < MandatoryTagsList.GetLength(0))
                {
                    XmlNodeList MandatoryTagsFalseResult = xmlDoc.GetElementsByTagName(MandatoryTagsList[loop, 0]);
                    if (MandatoryTagsFalseResult.Cast<XmlNode>().Any())
                    {
                        List<XmlNode> result = (from x in MandatoryTagsFalseResult.Cast<XmlNode>()
                                                where x.InnerText == ""
                                                select x)
                                        .ToList();
                        if (MandatoryTagsList[loop, 0] == "Product")
                            ProductListCount = MandatoryTagsFalseResult.Cast<XmlNode>().Count();
                        if (MandatoryTagsList[loop, 1] != "0" && MandatoryTagsFalseResult.Cast<XmlNode>().Count().ToString() != MandatoryTagsList[loop, 1])
                            MandatoryTagsMissingList += (string.IsNullOrEmpty(MandatoryTagsMissingList) ? "" : ",") + MandatoryTagsList[loop, 0];
                        else if (result.Any())
                            MandatoryTagsMissingList += (string.IsNullOrEmpty(MandatoryTagsMissingList) ? "" : ",") + MandatoryTagsList[loop, 0];
                    }
                    else
                        MandatoryTagsMissingList += (string.IsNullOrEmpty(MandatoryTagsMissingList) ? "" : ",") + MandatoryTagsList[loop, 0];
                    loop++;
                    goto AgainStartMendetoryTagsCheck;
                }
            }
            return MandatoryTagsMissingList;
        }

        /// <summary>
        /// Check Mandatory Fields List
        /// </summary>
        /// <param name="MandatoryFieldsList"></param>
        /// <param name="xmlDoc"></param>
        /// <param name="ProductListCount"></param>
        /// <returns></returns>
        public string CheckMandatoryFieldsList(string[,] MandatoryFieldsList, XmlDocument xmlDoc, ref int ProductListCount)
        {
            string MandatoryFieldsMissingList = "";
            int loop = 0;
            {
            AgainStartMendetoryFieldsCheck:
                if (loop < MandatoryFieldsList.GetLength(0))
                {
                    XmlNodeList MandatoryTagsFalseResult = xmlDoc.GetElementsByTagName(MandatoryFieldsList[loop, 0]);
                    if (MandatoryTagsFalseResult.Cast<XmlNode>().Any())
                    {
                        List<XmlNode> result = (from x in MandatoryTagsFalseResult.Cast<XmlNode>()
                                                where x.InnerText == ""
                                                select x)
                                        .ToList();
                        //if multiple product tag then check that all tag(i.e.serial number,supplier,manufacturer,model number) count and  product tag count is same(like if 4 product tag then 4 times all the mandatory tags available or not)
                        if (MandatoryFieldsList[loop, 1] == "1")
                            if (MandatoryTagsFalseResult.Cast<XmlNode>().Count() != ProductListCount)
                                MandatoryFieldsMissingList += (string.IsNullOrEmpty(MandatoryFieldsMissingList) ? "" : ",") + MandatoryFieldsList[loop, 0];
                        //if any tag having null value then check it
                        if (result.Any() && string.IsNullOrEmpty(MandatoryFieldsMissingList) && string.IsNullOrWhiteSpace(MandatoryFieldsMissingList)) //|| (MandatoryFieldsList[loop, 1] == "1" && MandatoryTagsFalseResult.Cast<XmlNode>().Count() == ProductListCount))
                            MandatoryFieldsMissingList += (string.IsNullOrEmpty(MandatoryFieldsMissingList) ? "" : ",") + MandatoryFieldsList[loop, 0];
                    }
                    else
                        MandatoryFieldsMissingList += (string.IsNullOrEmpty(MandatoryFieldsMissingList) ? "" : ",") + MandatoryFieldsList[loop, 0];
                    loop++;
                    goto AgainStartMendetoryFieldsCheck;
                }
            }
            return MandatoryFieldsMissingList;
        }

        /// <summary>
        /// Sign Xml Document
        /// </summary>
        /// <param name="xmlDoc"></param>
        public void SignXml(XmlDocument xmlDoc)
        {
            var ServerCertificate = System.Web.HttpContext.Current.Server.MapPath("~/Certificates/server.pfx");
            string certPath = ServerCertificate;
            string certPass = "tatva123";
            X509Certificate2 cert = new X509Certificate2(certPath, certPass, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            RSACryptoServiceProvider Key = new RSACryptoServiceProvider();
            Key.PersistKeyInCsp = false;
            // store the private key for later (signing)
            var exportedKeyMaterial = cert.PrivateKey.ToXmlString(true);
            Key.FromXmlString(exportedKeyMaterial);

            string id = "#verifiedproducts";

            if (xmlDoc == null)
                throw new ArgumentException("xmlDoc");
            if (Key == null)
                throw new ArgumentException("Key");
            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = cert.PrivateKey;
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
            signedXml.AddReference(reference);
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;
            signedXml.ComputeSignature();
            RSACryptoServiceProvider rsaEncryptor = (RSACryptoServiceProvider)cert.PrivateKey;
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
        }
    }
}
