using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace -- redacted --;

[TestClass]
public class LogMessageReceivedTests : NatsMessagesReceiverTestsBase
{
    protected override string Subject => $"{NatsMessagesSubjects.Log}.>";
}