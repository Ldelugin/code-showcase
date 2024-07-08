using System;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace -- redacted --;

[TestClass]
public class PostLogTests : NatsMessagesPosterTestsBase
{
    private const byte ServiceName = (byte)ServiceNames.-- redacted --;
    private readonly Log log = new("", DateTime.UtcNow, processId: 1, (byte)LogLevel.Debug, ServiceName);

    protected override string Subject => $"{NatsMessagesSubjects.Log}.{ServiceName}";
    protected override byte[] Data(params object[] _) => this.log.ToByteArray();
    protected override bool Post(params object[] _) => this.NatsMessagesBase.PostLog(this.log);
}