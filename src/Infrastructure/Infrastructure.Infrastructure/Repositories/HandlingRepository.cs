using Application.Constants;
using Application.Interfaces.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class HandlingRepository : IHandlingRepository
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryAsync<UploadImgProduct> _repositoryUploadImgProduct;
        public HandlingRepository(IUnitOfWork unitOfWork,
              IFormFileHelperRepository fileHelper,
            IRepositoryAsync<UploadImgProduct> repositoryUploadImgProduct)
        {
            _fileHelper = fileHelper;
            _unitOfWork = unitOfWork;
            _repositoryUploadImgProduct = repositoryUploadImgProduct;
        }
        public async Task<bool> DeleteImgProductAsync(int id, int idProduct)
        {
            var check = _repositoryUploadImgProduct.GetMulti(m => m.Id == id && m.IdProduct == idProduct).SingleOrDefault();
            if (check != null)
            {
                string filename = check.FileName;
                await _repositoryUploadImgProduct.DeleteAsync(check);
                await _unitOfWork.SaveChangesAsync(new CancellationToken());
                _fileHelper.DeleteFile(filename, FolderUploadConstants.Product);
                return true;
            }
            return false;
        }
    }
}
