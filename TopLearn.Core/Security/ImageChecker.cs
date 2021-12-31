using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TopLearn.Core.Security
{
    public static class ImageChecker
    {
        public static bool IsImage(this IFormFile file)
        {
            if (file == null)
            {
                return true;
            }

            try
            {
                var img = System.Drawing.Image.FromStream(file.OpenReadStream());
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
