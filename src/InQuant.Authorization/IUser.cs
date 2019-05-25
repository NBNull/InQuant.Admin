namespace InQuant.Authorization
{
    public interface IUser
    {
        int Id { get; set; }

        string Email { get; set; }
    }
}
