using System.Reflection;
using TheChg.Application;
using TheChg.Contracts;
using TheChg.Domain;
using TheChg.Infrastructure;

namespace TheChg.Architecture.Tests;

public sealed class DependencyBoundaryTests
{
    private static readonly string[] OuterProjects =
    [
        "TheChg.Infrastructure",
        "TheChg.Web",
        "TheChg.Worker"
    ];

    [Fact]
    public void Domain_does_not_depend_on_outer_projects() =>
        AssertNoReferences(typeof(DomainAssembly).Assembly, OuterProjects);

    [Fact]
    public void Contracts_do_not_depend_on_outer_projects() =>
        AssertNoReferences(typeof(ContractsAssembly).Assembly, OuterProjects);

    [Fact]
    public void Application_does_not_depend_on_infrastructure_or_hosts() =>
        AssertNoReferences(typeof(ApplicationAssembly).Assembly, OuterProjects);

    [Fact]
    public void Infrastructure_does_not_depend_on_hosts() =>
        AssertNoReferences(typeof(InfrastructureAssembly).Assembly, "TheChg.Web", "TheChg.Worker");

    private static void AssertNoReferences(Assembly assembly, params string[] forbidden)
    {
        var references = assembly.GetReferencedAssemblies().Select(reference => reference.Name).ToArray();
        Assert.DoesNotContain(references, reference => forbidden.Contains(reference));
    }
}
