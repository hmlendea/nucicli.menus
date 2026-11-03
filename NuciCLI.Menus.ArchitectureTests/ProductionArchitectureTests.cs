using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ArchUnitNet;

using NUnit.Framework;

namespace NuciCLI.Menus.ArchitectureTests
{
    [TestFixture]
    public sealed class ProductionArchitectureTests
    {
        private static readonly string productionProjectPath = "/" + Path.GetFullPath(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../NuciCLI.Menus/NuciCLI.Menus.csproj"));

        [Test]
        public async Task GivenProductionProject_WhenCheckingForCycles_ThenNoCyclesExist()
        {
            object rule = ArchUnit.ProjectFiles(productionProjectPath)
                .InPath("**/*.cs")
                .Should()
                .HaveNoCycles();

            object violations = await CheckRuleAsync(rule);

            Assert.That(violations, Is.Empty, "Production source files must not contain dependency cycles.");
        }

        [Test]
        public async Task GivenProductionProject_WhenCheckingSourceFiles_ThenAllFilesAreUnderTheProductionProject()
        {
            object rule = ArchUnit.ProjectFiles(productionProjectPath)
                .InPath("**/*.cs")
                .Should()
                .BeInPath("**/*.cs");

            object violations = await CheckRuleAsync(rule);

            Assert.That(violations, Is.Empty, "Production architecture input must exclude test and generated sources.");
        }

        [Test]
        public void GivenProductionAssembly_WhenInspectingItsReferences_ThenOnlyDocumentedDependenciesAreReferenced()
        {
            Assembly assembly = typeof(Menu).Assembly;
            string[] allowedReferences = ["System.Runtime", "NuciCLI", "NuciExtensions"];
            string[] referencedAssemblies = assembly.GetReferencedAssemblies()
                .Select(reference => reference.Name ?? string.Empty)
                .Where(name => !name.StartsWith("System.", StringComparison.Ordinal))
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

            Assert.That(referencedAssemblies, Is.Not.Empty, "The production assembly reference selector matched no assemblies.");
            Assert.That(referencedAssemblies, Is.SubsetOf(allowedReferences));
        }

        [Test]
        public void GivenProductionAssembly_WhenInspectingTypes_ThenAllExpectedPublicComponentsExist()
        {
            Type[] expectedTypes =
            [
                typeof(Menu),
                typeof(MenuManager)
            ];

            Type[] productionTypes = typeof(Menu).Assembly.GetTypes();

            foreach (Type expectedType in expectedTypes)
            {
                Assert.That(
                    productionTypes,
                    Does.Contain(expectedType),
                    $"Expected production type '{expectedType.FullName}' was not loaded.");
            }
        }

        private static async Task<object> CheckRuleAsync(object rule)
        {
            MethodInfo? checkAsync = rule.GetType().GetMethod("CheckAsync", BindingFlags.Public | BindingFlags.Instance);
            Assert.That(checkAsync, Is.Not.Null, "The architecture rule does not expose CheckAsync().");

            object? taskObject = checkAsync!.Invoke(rule, [null]);
            Assert.That(taskObject, Is.Not.Null, "The architecture rule returned no task.");

            Task task = (Task)taskObject!;
            await task;

            PropertyInfo? resultProperty = task.GetType().GetProperty("Result");
            Assert.That(resultProperty, Is.Not.Null, "The architecture rule returned no result.");
            return resultProperty!.GetValue(task)!;
        }
    }
}
