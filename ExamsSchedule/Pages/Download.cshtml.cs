using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace ExamsSchedule.Pages
{
    public class DownloadModel : PageModel
    {
        public IConfiguration _configuration { get; }
        public DownloadModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<BlobContainerClient> GetContainer()
        {
            //string storageAccount_connectionString = "DefaultEndpointsProtocol=https;AccountName=proctorimagesus;AccountKey=xv75fUkdnNc1nKHhyJfRwsf09pKC+Bqt34x5YCKJ96btpHq2DG03v+Hv9JrniPVn/gGgpjncdLK5+uKTQpYJCQ==;EndpointSuffix=core.windows.net";

            string storageAccount_connectionString = _configuration["blobConnection"];
            string blobContainerName = "studentidentity";/*_configuration.GetValue<string>("AzureBlobStorage:studentidentity");*/

            var container = new BlobContainerClient(storageAccount_connectionString, blobContainerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }

        
        public async Task<IActionResult> OnGet(string Filename)
        {
            //string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=proctorimagesus;AccountKey=xv75fUkdnNc1nKHhyJfRwsf09pKC+Bqt34x5YCKJ96btpHq2DG03v+Hv9JrniPVn/gGgpjncdLK5+uKTQpYJCQ==;EndpointSuffix=core.windows.net";

            string blobConnectionString = _configuration["blobConnection"];
            var storageAccount1 = CloudStorageAccount.Parse(blobConnectionString);
            var blobClient1 = storageAccount1.CreateCloudBlobClient();
            var container = await GetContainer();
            CloudBlobContainer container2 = blobClient1.GetContainerReference(container.Name);
            CloudBlockBlob blob = container2.GetBlockBlobReference(Filename);
            var blobStream = await blob.OpenReadAsync();
            return File(blobStream, blob.Properties.ContentType, Filename);
        }
    }
}




