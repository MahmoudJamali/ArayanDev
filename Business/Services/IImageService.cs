using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Entities.Concrete
{
    public interface IImageService
    {
        Task<string> SaveProfileImageAsync(IFormFile file);
    }

}


