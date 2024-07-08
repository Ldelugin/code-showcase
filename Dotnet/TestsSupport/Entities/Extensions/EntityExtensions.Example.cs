using TestsSupport.Extensions;
using TestsSupport.Entities.Factories;

namespace TestsSupport.Entities.Extensions;

public static partial class EntityExtensions
{
    public static Factory<User> DefaultUser(this Factory<User> factory)
    {
        return factory.Create()
            .With(u => u.Id, 1)
            .With(u => u.Name, "John Doe");
    }
}