using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class TestPriorityAttribute : Attribute
{
    public int Priority { get; }
    public TestPriorityAttribute(int priority) => Priority = priority;
}

public sealed class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var buckets = new SortedDictionary<int, List<TTestCase>>();

        foreach (var tc in testCases)
        {
            var priority = 0;
            var attrs = tc.TestMethod.Method
                .GetCustomAttributes(typeof(TestPriorityAttribute).AssemblyQualifiedName);

            if (attrs.Any())
                priority = attrs.First().GetNamedArgument<int>(nameof(TestPriorityAttribute.Priority));

            if (!buckets.TryGetValue(priority, out var list))
                buckets[priority] = list = new List<TTestCase>();

            list.Add(tc);
        }

        foreach (var list in buckets.Values)
            foreach (var tc in list.OrderBy(x => x.TestMethod.Method.Name))
                yield return tc;
    }
}