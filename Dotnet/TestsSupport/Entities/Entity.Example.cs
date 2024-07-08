using TestsSupport.Entities.Factories;

namespace TestsSupport.Entities;

public static partial class Entity
{
    public static Factory<User> User { get; } = Factory<User>();
}