using System.Collections.Generic;
using -- redacted --;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestsSupport.Extensions;

namespace -- redacted --;

[TestClass]
public class AuditingTests
{
    private AuditManager auditManager;
    private Mock<IAuditStore> auditStoreMock;
    private Mock<ILogger<AuditManager>> loggerMock;

    [TestInitialize]
    public void Initialize()
    {
        this.loggerMock = new Mock<ILogger<AuditManager>>();
        this.auditStoreMock = new Mock<IAuditStore>();
        this.auditManager = new AuditManager(this.loggerMock.Object, [this.auditStoreMock.Object]);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.loggerMock = null;
        this.auditManager = null;
    }

    public static IEnumerable<object[]> AuditInfos()
    {
        yield return new object[] { new AuditInfo { Description = "Blabla01", UserId = "User01" } };
        yield return new object[] { new AuditInfo { Description = "Blabla02", UserId = "User02" } };
        yield return new object[] { null };
    }

    [TestMethod]
    public void ConstructorThrowsExceptionWhenAnyArgumentIsNull() => Assert.That.ConstructorThrowsArgumentNullException<AuditManager>();

    [TestMethod]
    public void ThrowsWarningWhenAuditInfoIsNullWhileSaving()
    {
        // Act
        this.auditManager.SaveAudit(auditInfo: null);

        // Assert
        this.loggerMock.VerifyLog(LogLevel.Warning, s => s.Contains("Can not save a audit info that is null"),
            Times.Once);
    }

    [TestMethod]
    public void ThrowsWarningWhenNoAuditStoresAreRegisteredWhileSaving()
    {
        // Arrange
        this.auditManager = new AuditManager(this.loggerMock.Object, []);

        // Act
        this.auditManager.SaveAudit(new AuditInfo());

        // Assert
        this.loggerMock.VerifyLog(LogLevel.Warning,
            s => s.Contains("Can not save the audit info due to no audit stores being registered"), Times.Once);
    }

    [DataTestMethod]
    [DynamicData(nameof(AuditInfos), DynamicDataSourceType.Method)]
    public void VerifyAuditInfoCorrectlyPassedToAuditStores(AuditInfo auditInfo)
    {
        // Arrange
        var hasCorrectDescription = false;
        var hasCorrectUserId = false;

        _ = this.auditStoreMock.Setup(m => m.SaveAudit(auditInfo))
            .Callback<AuditInfo>(info =>
            {
                hasCorrectDescription = info.Description == auditInfo.Description;
                hasCorrectUserId = info.UserId == auditInfo.UserId;
            });

        // Act
        this.auditManager.SaveAudit(auditInfo);

        // Assert
        var expectedAuditStoreInvocation = Times.Exactly(auditInfo != null ? 1 : 0);
        this.auditStoreMock.Verify(m => m.SaveAudit(auditInfo), expectedAuditStoreInvocation);

        if (auditInfo != null)
        {
            Assert.IsTrue(hasCorrectDescription, "The audit info passed to the registered audit stores does not have the correct description.");
            Assert.IsTrue(hasCorrectUserId, "The audit info passed to the registered audit stores does not have the correct user id.");
        }
    }

    [TestMethod]
    public void VerifyCorrectSetup()
    {
        // Arrange
        var auditInfo = new AuditInfo { Description = "Blabla", UserId = "User01" };

        // Act
        this.auditManager.SaveAudit(auditInfo);

        // Assert
        this.auditStoreMock.Verify(m => m.SaveAudit(auditInfo), Times.Once);
    }
}
