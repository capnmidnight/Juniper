using Juniper.Cedrus.Controllers.V1;
using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IEnumerable<KeyValuePair<Template, PropertyType>> GetTemplateProperties(int? entityTypeId = null) =>
        from et in insecure.EntityTypes
            .Include(et => et.Parent)
            .Include(et => et.Template)
            .AsEnumerable()
        where entityTypeId == null || et.Id == entityTypeId
        from et2 in et.GetChain().Reverse()
        where et2.Template is not null
        from prop in et2.Template!.PropertyTypes
        select new KeyValuePair<Template, PropertyType>(et2.Template!, prop);

    public void RemovePropertyFromTemplate(Template template, PropertyType propertyType)
    {
        if (!template.PropertyTypes.Contains(propertyType))
        {
            throw new FileNotFoundException();
        }

        template.PropertyTypes.Remove(propertyType);
    }
}
