using System.Diagnostics;
using System.IO;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.BuildPipeline);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution _solution;
    [GitRepository] readonly GitRepository _gitRepository;

    static readonly string _solutionName = "BaseCliTool";
    static readonly string _runtimeId = "win-x64";
    static readonly string _targetFramework = "net5.0";
    static readonly int _coveragePercentMinimum = 0;

    static readonly string _coverageFilters =
        $"+:type={_solutionName}.Commands.*;+:type={_solutionName}.Data.*;+:type={_solutionName}.Events.*;+:type={_solutionName}.Map.*;+:type={_solutionName}.Messages.*;-:module={_solutionName}.Data.Lookup";

    Target BuildPipeline => _ => _
        .DependsOn(CleanSolution, RestoreSolution, BuildSolution)
        .Executes(() =>
        {
        });

    Target CleanSolution => _ => _
        .Executes(() =>
        {
            DotNetClean(s => s
                .SetProject(_solution)
                .SetVerbosity(DotNetVerbosity.Quiet));
        });

    Target RestoreSolution => _ => _
        .After(CleanSolution)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(_solution));
        });

    Target BuildSolution => _ => _
        .After(RestoreSolution)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(_solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target PublishSolution => _ => _
        .Executes(() =>
        {
            DotNetClean(s => s
                .SetProject(_projectDir)
                .SetVerbosity(DotNetVerbosity.Quiet)
                .SetConfiguration("Release"));

            DotNetPublish(s => s
                .SetProject(_projectDir)
                .SetConfiguration("Release")
                .SetRuntime(_runtimeId)
                .SetSelfContained(false));
        });

    Process Run(string exePath, string args = null, bool fromOwnDirectory = false)
    {
        string directory = null;

        if (fromOwnDirectory)
        {
            directory = Directory.GetParent(exePath).FullName;
        }

        return Run(exePath, args, directory);
    }

    Process Run(string exePath, string args, string workingDirectory)
    {
        var startInfo = new ProcessStartInfo(exePath);

        if (workingDirectory != null)
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        startInfo.Arguments = args ?? string.Empty;
        startInfo.UseShellExecute = true;

        var process = Process.Start(startInfo);

        return process;
    }
}
