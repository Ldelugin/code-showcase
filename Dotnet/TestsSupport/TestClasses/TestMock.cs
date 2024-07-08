using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Moq;

namespace TestsSupport.TestClasses;

/// <summary>
/// A specialized form of mock class with a specified target type, extending the Moq.Mock class.
/// </summary>
/// <typeparam name="T">The specified type targeted by the mock class.</typeparam>
[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public sealed class TestMock<T> : Mock<T>, IDisposable where T : class
{
    private static readonly FieldInfo GeneratorFieldInfo;
    private static readonly object CastleProxyFactoryInstance;
    private static readonly object OriginalProxyGeneratorInstance;

    /// <summary>
    /// Initializes a new instance of the TestMock class.
    /// </summary>
    /// <param name="targetInstance">The target instance to use with the built-in proxy generator.</param>
    public TestMock(T targetInstance)
    {
        GeneratorFieldInfo.SetValue(CastleProxyFactoryInstance, new IssProxyGenerator(targetInstance));
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() => GeneratorFieldInfo.SetValue(CastleProxyFactoryInstance, OriginalProxyGeneratorInstance);

    /// <summary>
    /// Static constructor to initialize the static fields and benefit from caching the reflection objects.
    /// </summary>
    static TestMock()
    {
        var moqAssembly = Assembly.Load(nameof(Moq));
        var proxyFactoryType = moqAssembly.GetType("Moq.ProxyFactory");
        var castleProxyFactoryType = moqAssembly.GetType("Moq.CastleProxyFactory");
        var proxyFactoryInstanceProperty = proxyFactoryType!.GetProperty("Instance");
        GeneratorFieldInfo = castleProxyFactoryType!.GetField("generator", BindingFlags.NonPublic | BindingFlags.Instance);
        CastleProxyFactoryInstance = proxyFactoryInstanceProperty!.GetValue(obj: null);
        OriginalProxyGeneratorInstance = GeneratorFieldInfo!.GetValue(CastleProxyFactoryInstance);// save default value to restore it later
    }
}