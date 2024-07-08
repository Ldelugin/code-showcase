using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace -- redacted --;

public abstract class NatsMessagesPosterTestsBase : NatsMessagesTestsBase<NatsMessagesPoster, NatsMessagesPoster>
{
    protected override NatsMessagesPoster Create() => new(this.NatsConnectionMock.Object, this.LoggerMock.Object);

    [TestMethod]
    [DataRow(null)]
    public virtual void PublishedWithTheCorrectValues(params object[] additionalArguments)
    {
        // Act
        _ = this.Post(additionalArguments);

        // Assert
        this.VerifyNatsConnectionPublish(this.Subject, this.Data(additionalArguments), Times.Once());
    }

    protected abstract bool Post(params object[] additionalArguments);
    protected abstract byte[] Data(params object[] additionalArguments);

    private void VerifyNatsConnectionPublish(string subject, byte[] data, Times times)
        => this.NatsConnectionMock.Verify(m => m.TryPublish(subject, data), times);
}