using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using -- redacted --;
using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using -- redacted --;
using -- redacted --;
using TestsSupport.Attributes;

namespace -- redacted --;

[TestClass]
public class AuthorizationTests : AuthorizationTestsBase
{
    private static -- redacted --;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext _1)
    {
        WebApplicationFactory = new -- redacted --<Startup>();
        WebApplicationFactory.Server.BaseAddress = new Uri(BaseAddress);
        await WebApplicationFactory.AddUserAsync(UserName, Email, Password);

        Channel = CreateGrpcChannel();
        -- redacted --
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        WebApplicationFactory?.Dispose();
        Channel?.Dispose();
    }

    private static IEnumerable<object[]> GetData()
    {
        yield return new object[]
        {
            -- redacted --,
            new Action<-- redacted -->(-- redacted --)
        };

        yield return new object[]
        {
            -- redacted --,
            new Action<-- redacted -->(-- redacted --)
        };

        yield return new object[]
        {
            -- redacted --,
            new Action<-- redacted -->(-- redacted --)
        };
    }

    [TestMethod]
    [DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
    public async Task ReturnsPermissionDenied_IfUserDoesNotHave_TheRequiredPermission(string requiredPermission,
        Action<-- redacted --> execute) =>
        await AssertCorrectRequiredPermission(requiredPermission, execute);

    [AssertionMethod]
    private static async Task AssertCorrectRequiredPermission(string requiredPermission,
        Action<-- redacted --> execute)
    {
        // Arrange
        await WebApplicationFactory.AddAllClaimsToUserExcept(Email, requiredPermission);
        RpcException grpcException = null;

        // Act
        try
        {
            execute.Invoke(-- redacted --);
        }
        catch (RpcException e)
        {
            grpcException = e;
        }
        catch (Exception e)
        {
            Assert.Fail("Was expecting a RpcException but got a different exception {0}", e);
        }

        // Assert
        Assert.IsNotNull(grpcException);
        Assert.AreEqual(StatusCode.PermissionDenied, grpcException.StatusCode);
    }
}