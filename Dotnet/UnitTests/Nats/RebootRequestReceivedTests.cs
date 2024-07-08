using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace -- redacted --;

[TestClass]
public class RebootRequestReceivedTests : NatsMessagesReceiverTestsBase
{
    protected override string Subject => $"{NatsMessagesSubjects.Reboot}.*";
}