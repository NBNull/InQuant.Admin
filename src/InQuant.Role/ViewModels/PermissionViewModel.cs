using InQuant.Authorization.Permissions;
using System.Collections.Generic;
using System.Linq;

namespace InQuant.Security.ViewModels
{
    public class PermissionViewModel
    {
        public PermissionViewModel(Permission r)
        {
            Name = r.Name;
            Description = r.Description;
            Category = r.Category;
            ImpliedBy = r.ImpliedBy?.Select(x => new PermissionViewModel(x))?.ToList();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public IEnumerable<PermissionViewModel> ImpliedBy { get; set; }
    }
}
