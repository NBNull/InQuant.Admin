namespace InQuant.Security.ViewModels
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public RoleViewModel(Models.Entities.AdminRole r)
        {
            Id = r.Id;
            Name = r.Name;
        }
    }
}
