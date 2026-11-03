using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace NuciCLI.Menus.ArchitectureTests
{
    [TestFixture]
    public sealed class SourceFileConventionTests
    {
        private static readonly string sourceDirectory = Path.GetFullPath(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../NuciCLI.Menus"));

        [Test]
        public void GivenProductionSources_WhenEnumeratingHandwrittenFiles_ThenSourcesAreAvailable()
        {
            string[] sourceFiles = GetSourceFiles();

            Assert.That(sourceFiles, Is.Not.Empty, "No eligible production source files were found.");
        }

        [Test]
        public void GivenProductionSources_WhenParsingDeclarations_ThenEachFileHasOneMatchingType()
        {
            string[] sourceFiles = GetSourceFiles();

            foreach (string sourceFile in sourceFiles)
            {
                string source = File.ReadAllText(sourceFile);
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
                CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();
                List<BaseTypeDeclarationSyntax> declarations = root.DescendantNodes()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .Where(declaration => declaration.Parent is not BaseTypeDeclarationSyntax)
                    .ToList();

                Assert.That(
                    declarations,
                    Has.Count.EqualTo(1),
                    $"'{sourceFile}' must declare exactly one top-level class, interface, struct, record, or enum.");

                string expectedName = Path.GetFileNameWithoutExtension(sourceFile);
                Assert.That(
                    declarations[0].Identifier.ValueText,
                    Is.EqualTo(expectedName),
                    $"'{sourceFile}' must have a type identifier matching its filename.");
            }
        }

        [Test]
        public void GivenProductionProjectFile_WhenReadingBuildConfiguration_ThenRequiredPoliciesAreEnabled()
        {
            string projectPath = Path.Combine(sourceDirectory, "NuciCLI.Menus.csproj");
            string project = File.ReadAllText(projectPath);

            Assert.Multiple(() =>
            {
                Assert.That(project, Does.Contain("<TargetFramework>net10.0</TargetFramework>"));
                Assert.That(project, Does.Contain("<ImplicitUsings>disable</ImplicitUsings>"));
                Assert.That(project, Does.Contain("<LangVersion>latest</LangVersion>"));
                Assert.That(project, Does.Contain("<Nullable>enable</Nullable>"));
            });
        }

        [Test]
        public void GivenProductionSources_WhenInspectingNamespaceSyntax_ThenExplicitBlockNamespacesAreUsed()
        {
            foreach (string sourceFile in GetSourceFiles())
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(sourceFile));
                CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

                Assert.That(
                    root.Members.OfType<NamespaceDeclarationSyntax>().Count(),
                    Is.EqualTo(1),
                    $"'{sourceFile}' must use exactly one explicit block namespace.");
                Assert.That(
                    root.Members.OfType<FileScopedNamespaceDeclarationSyntax>().Count(),
                    Is.Zero,
                    $"'{sourceFile}' must not use a file-scoped namespace.");
            }
        }

        private static string[] GetSourceFiles()
            => Directory.EnumerateFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(file => !file.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                .Where(file => !file.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                .OrderBy(file => file, StringComparer.Ordinal)
                .ToArray();
    }
}
