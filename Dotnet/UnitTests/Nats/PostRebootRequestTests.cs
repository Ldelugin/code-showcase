using System.Text;
using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace -- redacted --;

[TestClass]
public class PostRebootRequestTests : NatsMessagesPosterTestsBase
{
    private const string MachineName = "machine1";

    protected override string Subject => $"{NatsMessagesSubjects.Reboot}.{MachineName}";
    protected override byte[] Data(params object[] additionalArguments)
        => Encoding.UTF8.GetBytes(((object[])additionalArguments[0])[0].ToString()!);
    protected override bool Post(params object[] additionalArguments)
        => this.NatsMessagesBase.PostRebootRequest(MachineName, (bool)((object[])additionalArguments[0])[0]);

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public override void PublishedWithTheCorrectValues(params object[] additionalArguments)
        => base.PublishedWithTheCorrectValues(additionalArguments);
}