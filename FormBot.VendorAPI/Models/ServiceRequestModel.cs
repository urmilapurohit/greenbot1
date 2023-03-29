using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormBot.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FormBot.VendorAPI.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public class LoginResponseModel
    {
        public Token TokenData { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class GetUserDetailRequest
    {
        public int UserId { get; set; }
    }

    public class GetUserDetailResponse
    {
        public UserVendor UserData { get; set; }

        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        //spublic int expires_in { get; set; }
        //public string userName { get; set; }
        //[JsonProperty(".issued")]
        //public string issued { get; set; }
        //[JsonProperty(".expires")]
        //public string expires { get; set; }
    }

    public class ServiceRequestModel
    {
    }


    public class OutputJobResponse
    {
        public JobResponse obj { get; set; }
        public int JobId { get; set; }
    }

    public class JobResponse
    {       
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<locality> locality { get; set; }
    }

    public class JobPhotoRequest
    {
        [Required(ErrorMessage = "JobId is required.")]
        public string VendorJobId { get; set; }

        [Required(ErrorMessage = "IsClassic is required.")]
        public bool IsClassic { get; set; }

        [Required(ErrorMessage = "ImageBase64 is required.")]
        public string ImageBase64 { get; set; }

        [Required(ErrorMessage = "PhotoType is required.")]
        public int PhotoType { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        [Required(ErrorMessage = "Filename is required.")]
        public string Filename { get; set; }

        [Required(ErrorMessage = "VendorJobPhotoId is required.")]
        public string VendorJobPhotoId { get; set; }

        public int JobId { get; set; }

        [Required(ErrorMessage = "IsDefault is required.")]
        public bool IsDefault { get; set; }
    }

    public class JobDocumentRequest
    {
        [Required(ErrorMessage = "JobId is required.")]
        public string VendorJobId { get; set; }

        [Required(ErrorMessage = "IsClassic is required.")]
        public bool IsClassic { get; set; }

        [Required(ErrorMessage = "DocumentBase64 is required.")]
        public string DocumentBase64 { get; set; }

        [Required(ErrorMessage = "DocumentType is required.")]
        public int DocumentType { get; set; }

        [Required(ErrorMessage = "DocumentName is required.")]
        public string DocumentName { get; set; }

        [Required(ErrorMessage = "JobType is required.")]
        public int JobType { get; set; }

        [Required(ErrorMessage = "VendorJobDocumentId is required.")]
        public string VendorJobDocumentId { get; set; }

        public int JobId { get; set; }
    }

    public class JobPhotoDeleteRequest
    {
        [Required(ErrorMessage = "JobId is required.")]
        public string VendorJobId { get; set; }

        [Required(ErrorMessage = "VendorJobPhotoId is required.")]
        public string VendorJobPhotoId { get; set; }

        [Required(ErrorMessage = "IsClassic is required.")]
        public bool IsClassic { get; set; }

        public int JobId { get; set; }
    }

    public class JobDocumentDeleteRequest
    {
        [Required(ErrorMessage = "JobId is required.")]
        public string VendorJobId { get; set; }

        [Required(ErrorMessage = "VendorJobDocumentId is required.")]
        public string VendorJobDocumentId { get; set; }

        [Required(ErrorMessage = "IsClassic is required.")]
        public bool IsClassic { get; set; }

        public int JobId { get; set; }
    }

    public class PanelResponseModel
    {
        public List<PanelModel> panel { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }
    public class InverterResponseModel
    {
        public List<Inverter> inverter { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class SystemBrandResponseModel
    {
        public List<SystemBrandModel> systemBrand { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class GetJobsModel
    {
        public List<JobListModel> lstJobData { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        //public string VendorJobId { get; set; }
    }

    public class PostCodeValidation
    {
        //public int JobId { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public List<locality> locality { get; set; }
    }

    public class RootObject
    {
        public localities localities { get; set; }
    }

    public class localities
    {
        public List<locality> locality { get; set; }
    }

    public class locality
    {
        public string location { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
    }

    public class CustomFieldResponseModel
    {
        public List<VendorCustomField> CustomField { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class LocationResponseModel
    {
        public List<locality> locality { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class GetStcSubmissionModel
    {
        public List<FormBot.Entity.Job.STCBasicDetails.STCSubmissionModel> lstStcSubmissionData { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        //public string VendorJobId { get; set; }
    }

    public class GetJobPhotosModel
    {
        public List<VendorJobPhotoList> lstJobPhoto { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }
}