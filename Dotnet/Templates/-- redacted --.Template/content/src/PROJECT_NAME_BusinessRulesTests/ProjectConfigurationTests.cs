using -- redacted --;
#if (-- redacted --)
using -- redacted --;
#endif
using -- redacted --;
using Microsoft.Extensions.Options;
using Moq;
using PROJECT_NAME_BusinessRules.Configuration;
using PROJECT_NAME_BusinessRules.Engines;
using PROJECT_NAME_BusinessRulesTests.Extensions;

namespace PROJECT_NAME_BusinessRulesTests;

[TestClass]
public class ProjectConfigurationTests
{
    private readonly Mock<-- redacted --> -- redacted -- = new();
    private readonly Mock<IOptionsMonitor<PROJECT_NAME_BusinessRulesOptions>> pluginOptionsMonitorMock = new();
    private readonly Mock<-- redacted --> -- redacted -- = new();
    private readonly -- redacted -- = new();
    private readonly -- redacted -- = new();
    private readonly PROJECT_NAME_ProjectConfiguration projectConfiguration;

    public ProjectConfigurationTests()
    {
        this.projectConfiguration = new PROJECT_NAME_ProjectConfiguration(
            -- redacted --);
    }

    [TestMethod]
    public void ValueValidationRulesIsNotNull()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();

        // Act
        var valueValidationRules = this.projectConfiguration.ValueValidationRules;

        // Assert
        Assert.IsNotNull(valueValidationRules);
    }

    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted --;

        // Assert
        Assert.IsNotNull(-- redacted --);
    }

    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted --;

        // Assert
        Assert.IsNotNull(-- redacted --);
    }

#if (-- redacted --)
#if (-- redacted --)
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        const string -- redacted -- = "-- redacted --";
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted -- as -- redacted --;

        // Assert
        Assert.IsNotNull(-- redacted --);
        Assert.IsTrue(-- redacted --.Contains("-- redacted --"));
#if (-- redacted --)
        Assert.IsTrue(-- redacted --);
#else
        Assert.IsFalse(-- redacted --);
#endif
#if (-- redacted --)
        Assert.IsTrue(-- redacted --);
#else
        Assert.IsFalse(-- redacted --);
#endif
        Assert.AreEqual(-- redacted --, -- redacted --);
    }
#endif
#if (-- redacted --)
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        const string -- redacted -- = "-- redacted --";
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted -- as -- redacted --;

        // Assert
        Assert.IsNotNull(-- redacted --);
        Assert.AreEqual(-- redacted --, -- redacted --);
    }
#endif
#if (-- redacted --)
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        const string -- redacted -- = "-- redacted --";
        const string -- redacted -- = "-- redacted --";
        const string -- redacted -- = "-- redacted --";
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted -- as -- redacted --;

        // Assert
        Assert.IsNotNull(workflow);
        Assert.AreEqual(-- redacted --, -- redacted --.-- redacted --);
        Assert.AreEqual(-- redacted --, -- redacted --.-- redacted --);
        Assert.AreEqual(-- redacted --, -- redacted --.-- redacted --);
    }
#endif
#if (-- redacted --)
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        const string -- redacted -- = "-- redacted --";
        const string -- redacted -- = "-- redacted --";
        const string -- redacted -- = "-- redacted --";
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted -- as -- redacted --;

        // Assert
        Assert.IsNotNull(workflow);
        Assert.AreEqual(-- redacted --, -- redacted --.-- redacted --);
        Assert.AreEqual(-- redacted --, -- redacted --.-- redacted --);
        Assert.AreEqual(-- redacted --, -- redacted --.-- redacted --);
    }
#endif
#if (-- redacted --)
    [TestMethod]
    public void PROJECT_NAME_-- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted -- as PROJECT_NAME_-- redacted --;

        // Assert
        Assert.IsNotNull(-- redacted --);
        // TODO: Add asserts if needed
    }
#endif
#endif

#if (-- redacted --)
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();

        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted --;

        // Assert
        Assert.IsNotNull(-- redacted --);
#if (-- redacted --)
        Assert.IsTrue(-- redacted -- is -- redacted --);
#elif (-- redacted --)
        Assert.IsTrue(-- redacted -- is -- redacted --);
#elif (-- redacted --)
        Assert.IsTrue(-- redacted -- is -- redacted --);
#elif (-- redacted --)
        Assert.IsTrue(-- redacted -- is -- redacted --);
#elif (-- redacted --)
        Assert.IsTrue(-- redacted -- is PROJECT_NAME_-- redacted --);
#endif
    }
#else
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted --;

        // Assert
        Assert.IsNull(-- redacted --);
    }
#endif

    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted --;

        // Assert
        Assert.IsNotNull(-- redacted --);
    }

    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted --;

        // Assert
        Assert.IsNotNull(-- redacted --);
    }

    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var -- redacted -- = this.projectConfiguration.-- redacted --;

        // Assert
        Assert.IsNotNull(-- redacted --);
    }

    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var isEnabled = this.projectConfiguration.-- redacted --;
        
        // Assert
#if (-- redacted --)
        Assert.IsTrue(isEnabled);
#else
        Assert.IsFalse(isEnabled);
#endif
    }
    
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var isEnabled = this.projectConfiguration.-- redacted --;

        // Assert
#if (-- redacted --)
        Assert.IsTrue(isEnabled);
#else
        Assert.IsFalse(isEnabled);
#endif
    }
    
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var isEnabled = this.projectConfiguration.-- redacted --;

        // Assert
#if (-- redacted --)
        Assert.IsTrue(isEnabled);
#else
        Assert.IsFalse(isEnabled);
#endif
    }
    
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var count = this.projectConfiguration.-- redacted --;

        // Assert
        Assert.AreEqual(00000002, count);
    }
    
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var isEnabled = this.projectConfiguration.-- redacted --;

        // Assert
#if (-- redacted --)
        Assert.IsTrue(isEnabled);
#else
        Assert.IsFalse(isEnabled);
#endif
    }
    
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var isEnabled = this.projectConfiguration.-- redacted --;

        // Assert
#if (-- redacted --)
        Assert.IsTrue(isEnabled);
#else
        Assert.IsFalse(isEnabled);
#endif
    }
    
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var isEnabled = this.projectConfiguration.-- redacted --;

        // Assert
#if (-- redacted --)
        Assert.IsTrue(isEnabled);
#else
        Assert.IsFalse(isEnabled);
#endif
    }
    
    [TestMethod]
    public void -- redacted --()
    {
        // Arrange
        this.pluginOptionsMonitorMock.SetupAsDefault();
        
        // Act
        var isEnabled = this.projectConfiguration.-- redacted --;

        // Assert
#if (-- redacted --)
        Assert.IsTrue(isEnabled);
#else
        Assert.IsFalse(isEnabled);
#endif
    }
}