using System;
using System.Collections.Generic;

using Effanville.FPD.Logic.ViewModels.Common;

using NUnit.Framework;

namespace Effanville.FPD.Logic.Tests;

public class UriHelpersTests
{
    private static IEnumerable<TestCaseData> TestUriCases()
    {
        yield return new TestCaseData("http://www.google.com", true, new Uri("http://www.google.com"));
        yield return new TestCaseData("c:\\users\\docs\\text.html", true, new Uri("file:///c:/users/docs/text.html"));
        yield return new TestCaseData("hi-hi", false, null);
    }

    [TestCaseSource(nameof(TestUriCases))]
    public void TestUri(string text, bool created, Uri expectedUri)
    {
        bool actualCreate = UriHelpers.IsValidUri(text, out Uri actual);
        Assert.That(actualCreate, Is.EqualTo(created));
        Assert.That(actual, Is.EqualTo(expectedUri));
    }
}