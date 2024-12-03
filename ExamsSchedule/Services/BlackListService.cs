using Azure.Storage.Blobs;
using ExamsSchedule.Models.DB;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Services
{
    public class BlackListService
    {
        private readonly DeepsensorClientContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _configuration;
        public BlackListService(DeepsensorClientContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        public async Task<BlackList> AddAsync(BlackList model, IList<IBrowserFile> imageFile)
        {
            foreach (var item in imageFile)
            {
                var entity = new BlackList
                {
                    Name = model.Name,
                    UploadPicture = item.Name,
                    Notes = model.Notes,
                    AddedOn = DateTime.Now
                };
                await _dbContext.BlackList.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
            var fileNames = imageFile.Select(m => Path.GetFileNameWithoutExtension(m.Name)).ToList();

            foreach (var item in imageFile)
            {
                var imageName = item.Name;

                var stream = item.OpenReadStream();
                var container = await GetContainer();

                var blob = container.GetBlobClient(imageName);
                await blob.UploadAsync(stream, overwrite: true);
            }
            var entity1 = await _dbContext.BlackList.Where(m => m.Id == model.Id).FirstOrDefaultAsync();
            return entity1;
        }
        public async Task<BlobContainerClient> GetContainer()
        {
            string storageAccount_connectionString = _configuration["blobConnection"];
            string blobContainerName = "studentidentity";/*_configuration.GetValue<string>("AzureBlobStorage:studentidentity");*/
            var container = new BlobContainerClient(storageAccount_connectionString, blobContainerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }
        public async Task<List<BlackList>> GetAllAsync()
        {
            return await _dbContext.BlackList.ToListAsync();
        }
        public async Task Delete(BlackList model)
        {
            var record = await _dbContext.BlackList.Where(m => m.Id == model.Id).FirstOrDefaultAsync();
            _dbContext.Remove(record);
            await _dbContext.SaveChangesAsync();
        }
    }
}
