using EmployeePortal.Application;
using EmployeePortal.Infrastructure;
using Xunit;

namespace EmployeePortal.Tests;

/// <summary>
/// Guards the Implementation View mapping: the source layout follows the component
/// boundaries fixed by the Software Architecture Document (Logical View).
/// </summary>
public class ComponentMapTests
{
    [Fact]
    public void ApplicationLayer_DeclaresTheFiveApplicationComponents()
    {
        Assert.Equal(5, ApplicationComponentMap.All.Count);
    }

    [Fact]
    public void InfrastructureLayer_DeclaresTheFourInfrastructureComponents()
    {
        Assert.Equal(4, InfrastructureComponentMap.All.Count);
    }
}
