using ApolloGraphQL.Federation.HotChocolate;

namespace Accounts;

[Key("id")]
public class User
{
    public User(string id, string name, string username)
    {
        Id = id;
        Name = name;
        Username = username;
    }

    public string Id { get; }

    public string Name { get; }

    public string Username { get; }

    [ReferenceResolver]
    public static Task<User> GetByIdAsync(
        string id,
        UserRepository userRepository)
        => userRepository.GetUserByIdAsync(id);
}