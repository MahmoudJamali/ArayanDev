using Entities.Concrete;


    namespace Business.Services.Authentication
    {
    public interface IPasswordHasher
    {
        (string hash, string salt) Hash(string password);
        bool Verify(string password, string storedHash, string storedSalt);
    }
}



