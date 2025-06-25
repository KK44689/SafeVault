public static class UserStore
{
    public static readonly List<UserModel> Users = new List<UserModel>
    {
        new UserModel { Username = "testUser", Password = BCrypt.Net.BCrypt.HashPassword("securepassword123"), Role = "Admin" }
    };
}