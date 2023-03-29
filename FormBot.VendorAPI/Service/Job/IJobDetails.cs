using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.VendorAPI.Models;
using FormBot.Entity;

namespace FormBot.VendorAPI.Service.Job
{
    public interface IJobDetails
    {

        JobResponse JobResponse(string id, string msg);

        /// <summary>
        /// Creates the job.
        /// </summary>
        /// <param name="createJob">The create job.</param>
        /// <returns></returns>
        OutputJobResponse CreateJob(CreateJob createJob,User userdata);

        /// <summary>
        /// Jobs the photo.
        /// </summary>
        /// <param name="jobPhotoRequest">The job photo request.</param>
        /// <returns></returns>
        OutputJobResponse JobPhoto(JobPhotoRequest jobPhotoRequest,User userdata);

        /// <summary>
        /// Jobs the document.
        /// </summary>
        /// <param name="jobDocumentRequest">The job document request.</param>
        /// <returns></returns>
        OutputJobResponse JobDocument(JobDocumentRequest jobDocumentRequest,User userdata);

        /// <summary>
        /// Jobs the photo delete.
        /// </summary>
        /// <param name="jobPhotoDeleteRequest">The job photo delete request.</param>
        /// <returns></returns>
        JobResponse JobPhotoDelete(JobPhotoDeleteRequest jobPhotoDeleteRequest);

        /// <summary>
        /// Jobs the document delete.
        /// </summary>
        /// <param name="jobDocumentDeleteRequest">The job document delete request.</param>
        /// <returns></returns>
        JobResponse JobDocumentDelete(JobDocumentDeleteRequest jobDocumentDeleteRequest);

        /// Gets the panel.
        /// </summary>
        /// <returns></returns>
        PanelResponseModel GetPanel();

        /// <summary>
        /// Gets the jobs.
        /// </summary>
        /// <returns></returns>
        GetJobsModel GetJobs(string CreatedDate, string FromDate, string ToDate, string RefNumber, User userdata, string VendorJobId, string CompanyABN);

        /// <summary>
        /// Gets the inverter.
        /// </summary>
        /// <returns></returns>
        InverterResponseModel GetInverter();

        /// <summary>
        /// Gets the system brand.
        /// </summary>
        /// <returns></returns>
        SystemBrandResponseModel GetSystemBrand();

        /// <summary>
        /// Gets the custom field.
        /// </summary>
        /// <param name="userdata">The userdata.</param>
        /// <returns></returns>
        CustomFieldResponseModel GetCustomField(User userdata);

        GetStcSubmissionModel GetStcSubmission(string VendorJobId);

        //GetJobPhotosModel GetJobPhotos(string VendorJobId);
    }
}
