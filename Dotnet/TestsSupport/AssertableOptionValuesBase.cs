using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using -- redacted --;
using -- redacted --;
using TestsSupport.Extensions;

namespace TestsSupport;

public abstract class AssertableOptionValuesBase(string key)
{
    protected static string GetFullQualifiedKeyFromPropertyName<TOptions, TProp>(
        Expression<Func<TOptions, TProp>> selector, params Type[] types)
    {
        var memberInfo = selector.GetMemberInfoFromSelector();
        var builder = new StringBuilder().Append(-- redacted --).Append(value: ':');

        foreach (var type in types)
        {
            _ = builder.Append(type.Name).Append(value: ':');
        }

        _ = builder.Append(memberInfo.Name);
        return builder.ToString();
    }

    public string Key { get; } = key;
    public List<(string Value, bool ExpectedToFail)> Arguments { get; } = [];

    public AssertableOptionValuesBase Assert(string value, bool expectedToFail)
    {
        this.Arguments.Add((value, expectedToFail));
        return this;
    }

    public AssertableOptionValuesBase AssertAllowedToBeUncPath()
        => this.Assert("//MachineName/-- redacted --/DirectoryName", expectedToFail: false);

    public AssertableOptionValuesBase AssertAllowedToBeAbsolutePath()
        => this.Assert("c:/-- redacted --/DirectoryName", expectedToFail: false);

    public AssertableOptionValuesBase AssertNotAllowedToBeRelativePath()
        => this.Assert("./-- redacted --/DirectoryName", expectedToFail: true);

    public AssertableOptionValuesBase AssertAllowedToBeEmpty()
        => this.Assert("", expectedToFail: false);

    public AssertableOptionValuesBase AssertAllowedToBeEmptyString()
        => this.Assert(string.Empty, expectedToFail: false);

    public AssertableOptionValuesBase AssertAllowedToBeNull()
        => this.Assert(value: null, expectedToFail: false);

    public AssertableOptionValuesBase AssertAllowedToBeAnAstrix()
        => this.Assert("*", expectedToFail: false);

    public AssertableOptionValuesBase AssertNotAllowedToBeEmpty()
        => this.Assert("", expectedToFail: true);

    public AssertableOptionValuesBase AssertNotAllowedToBeEmptyString()
        => this.Assert(string.Empty, expectedToFail: true);

    public AssertableOptionValuesBase AssertNotAllowedToBeNull()
        => this.Assert(value: null, expectedToFail: true);

    public AssertableOptionValuesBase AssertNotAllowedToBeWhitespace()
        => this.Assert(" ", expectedToFail: true);

    public AssertableOptionValuesBase AssertUrl(bool alsoAssertWithPort, string[] schemes)
    {
        var urlsWithSchemes = new List<string>();
        if (schemes != null && schemes.Any())
        {
            foreach (var scheme in schemes)
            {
                urlsWithSchemes.Add($"{scheme}://localhost");
                urlsWithSchemes.Add($"{scheme}://domain-with-hyphen");
            }
        }
        _ = this.Assert("//localhost", expectedToFail: true)
            .AssertAllowedToBeEmpty()
            .AssertAllowedToBeEmptyString()
            .AssertAllowedToBeNull();

        if (alsoAssertWithPort)
        {
            _ = this.Assert("//localhost:1234", expectedToFail: true);
        }

        foreach (var urlWithScheme in urlsWithSchemes)
        {
            _ = this.Assert(urlWithScheme, expectedToFail: false);
            if (alsoAssertWithPort)
            {
                _ = this.Assert($"{urlWithScheme}:8080", expectedToFail: false);
            }
        }

        return this;
    }

    public AssertableOptionValuesBase AssertRequired() => this.AssertNotAllowedToBeNull()
        .AssertNotAllowedToBeEmpty()
        .AssertNotAllowedToBeWhitespace()
        .AssertNotAllowedToBeEmptyString();

    public AssertableOptionValuesBase AssertNotRequired() => this.AssertAllowedToBeEmpty()
        .AssertAllowedToBeEmptyString()
        .AssertAllowedToBeNull();

    public AssertableOptionValuesBase AssertSoapEndpoint()
        => this.Assert("SomeAddress", expectedToFail: true)
            .Assert("/SomeAddress", expectedToFail: false);

    public AssertableOptionValuesBase AssertPassword(bool assertExamplePassword = true)
    {
        _ = this.Assert(ConfigurationItem.PasswordMask, expectedToFail: true);
        if (assertExamplePassword)
        {
            _ = this.Assert("S0m3P@ssw0rd", expectedToFail: false);
        }

        return this;
    }

    public AssertableOptionValuesBase AssertValidServiceWorkerConfiguration() => this
        .Assert("test", expectedToFail: true) // no digits
        .Assert("a;1,b;2,c;3", expectedToFail: true) // invalid separator machine name and number of workers
        .Assert("a:1.b:2.c:3", expectedToFail: true) // invalid separator (machine name and workers) elements
        .Assert("0", expectedToFail: false)
        .Assert("1", expectedToFail: false)
        .Assert("machine1:2", expectedToFail: false)
        .Assert("machine-1:2", expectedToFail: false)
        .Assert("machine-1:2,machine-2:3", expectedToFail: false)
        .Assert("a:1,b:2,c:3", expectedToFail: false);
}
