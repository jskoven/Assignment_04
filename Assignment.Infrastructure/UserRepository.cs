namespace Assignment3.Core;

public interface UserRepository
{
    (Response Response, int UserId) Create(UserCreateDTO user);
    IReadOnlyCollection<UserDTO> ReadAll();
    UserDTO Read(int userId);
    Response Update(UserUpdateDTO user);
    Response Delete(int userId, bool force = false);
}