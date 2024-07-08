using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace TestsSupport.TestClasses;

/// <summary>
/// A custom proxy generator.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestProxyGenerator"/> class.
/// </remarks>
/// <param name="target">The target object to proxy.</param>
/// <exception cref="System.ArgumentNullException">Thrown when the target object is null.</exception>
public class TestProxyGenerator(object target) : ProxyGenerator
{
    private readonly object target = target ?? throw new ArgumentNullException(nameof(target));

    /// <summary>
    ///   Creates proxy object intercepting calls to virtual members of type <paramref name="classToProxy" /> on newly created instance of that type with given <paramref name="interceptors" />.
    /// </summary>
    /// <param name="classToProxy">Type of class which will be proxied.</param>
    /// <param name="additionalInterfacesToProxy">Additional interface types. Calls to their members will be proxied as well.</param>
    /// <param name="options">The proxy generation options used to influence generated proxy type and object.</param>
    /// <param name="constructorArguments">Arguments of constructor of type <paramref name="classToProxy" /> which should be used to create a new instance of that type.</param>
    /// <param name="interceptors">The interceptors called during the invocation of proxied methods.</param>
    /// <returns>
    ///   New object of type <paramref name="classToProxy" /> proxying calls to virtual members of <paramref name="classToProxy" /> and <paramref name="additionalInterfacesToProxy" /> types.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">Thrown when given <paramref name="classToProxy" /> object is a null reference (Nothing in Visual Basic).</exception>
    /// <exception cref="T:System.ArgumentNullException">Thrown when given <paramref name="options" /> object is a null reference (Nothing in Visual Basic).</exception>
    /// <exception cref="T:System.ArgumentException">Thrown when given <paramref name="classToProxy" /> or any of <paramref name="additionalInterfacesToProxy" /> is a generic type definition.</exception>
    /// <exception cref="T:System.ArgumentException">Thrown when given <paramref name="classToProxy" /> is not a class type.</exception>
    /// <exception cref="T:System.ArgumentException">Thrown when no constructor exists on type <paramref name="classToProxy" /> with parameters matching <paramref name="constructorArguments" />.</exception>
    /// <exception cref="T:System.Reflection.TargetInvocationException">Thrown when constructor of type <paramref name="classToProxy" /> throws an exception.</exception>
    /// <remarks>
    ///   This method uses <see cref="T:Castle.DynamicProxy.IProxyBuilder" /> implementation to generate a proxy type.
    ///   As such caller should expect any type of exception that given <see cref="T:Castle.DynamicProxy.IProxyBuilder" /> implementation may throw.
    /// </remarks>
    public override object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options,
        object[] constructorArguments, params IInterceptor[] interceptors)
    {
        // Validate input arguments.
        if (classToProxy == null)
        {
            throw new ArgumentNullException(nameof(classToProxy));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        // Ensure the proxy can be created for the class.
        if (!classToProxy.GetTypeInfo().IsClass)
        {
            throw new ArgumentException(@"'classToProxy' must be a class", nameof(classToProxy));
        }

        // Validate the class and interfaces aren't generic type definitions.
        this.CheckNotGenericTypeDefinition(classToProxy, nameof(classToProxy));
        this.CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, nameof(additionalInterfacesToProxy));

        // Create the proxy type with the target.
        var proxyType = this.CreateClassProxyTypeWithTarget(classToProxy, additionalInterfacesToProxy, options);

        // Build the argument list.
        var list = this.BuildArgumentListForClassProxyWithTarget(this.target, options, interceptors);
        if (constructorArguments != null && constructorArguments.Any())
        {
            list.AddRange(constructorArguments);
        }

        // Create and return the proxy instance.
        return this.CreateClassProxyInstance(proxyType, list, classToProxy, constructorArguments);
    }
}