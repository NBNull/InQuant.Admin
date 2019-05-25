namespace InQuant.Security.Models.AdminUsers
{
    public class AdminUserUpdateModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public bool IsAdmin { get; set; }
    }
}
