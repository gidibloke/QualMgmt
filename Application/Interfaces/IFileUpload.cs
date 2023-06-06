using Application.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFileUpload
    {
        Task<string> Upload(StaffQualificationViewModel model);

        void DeleteFile(StaffQualificationViewModel model);

        void DeleteAll(string staffId);
    }
}
