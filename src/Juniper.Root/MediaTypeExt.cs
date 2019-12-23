using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Juniper.IO;

namespace Juniper
{

    public static class MediaTypeExt
    {
        public static string AddExtension(this MediaType contentType, string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (contentType?.PrimaryExtension != null)
            {
                var currentExtension = PathExt.GetShortExtension(fileName);
                if (contentType.Extensions.IndexOf(currentExtension) == -1)
                {
                    fileName += "." + contentType.PrimaryExtension;
                }
            }

            return fileName;
        }
    }
}
