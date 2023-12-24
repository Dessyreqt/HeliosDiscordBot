using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.FileSystemTasks;

[UnsetVisualStudioEnvironmentVariables]
partial class Build : NukeBuild
{
    static readonly string _solutionName = "HeliosDiscordBot";
    static readonly string _runtimeId = "win-x64";
    static readonly string _targetFramework = "net6.0";
    static readonly int _coveragePercentMinimum = 0;
    static readonly string _databaseName = "HeliosDiscordBot";
    static readonly string _databaseServer = "localhost";

    static readonly string _coverageFilters =
        $"+:type={_solutionName}.Commands.*;+:type={_solutionName}.Data.*;+:type={_solutionName}.Events.*;+:type={_solutionName}.Map.*;+:type={_solutionName}.Messages.*;-:module={_solutionName}.Data.Lookup";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution _solution;
    [GitRepository] readonly GitRepository _gitRepository;

    Target CiPipeline => _ => _
        .Triggers(Clean, Restore, Compile, CiCoverageReport);

    Target DropAndRestoreDatabase => _ => _
        .Before(Clean)
        .DependsOn(DropDatabase, RestoreDatabase)
        .Executes(() =>
        {
        });

    Target RestoreDatabase => _ => _
        .Before(Clean)
        .Executes(() =>
        {
            var process = Run(_roundhouseExePath,
                $"/d=\"{_databaseName}\" /f=\"{_databaseDirectory}\" /s=\"{_databaseServer}\" /cds=\"{_createDatabaseScript}\" /silent /transaction");
            process.WaitForExit();

            if (process.ExitCode != 0) { throw new Exception($"Problem running database migrations:\n{process.StandardOutput.ReadToEnd()}"); }
        });

    Target DropDatabase => _ => _
        .Before(RestoreDatabase)
        .Executes(() =>
        {
            var process = Run(_roundhouseExePath, $"/d=\"{_databaseName}\" /s=\"{_databaseServer}\" /silent /drop");
            process.WaitForExit();

            if (process.ExitCode != 0) { throw new Exception($"Problem running database migrations:\n{process.StandardOutput.ReadToEnd()}"); }
        });

    Target Clean => _ => _
        .Executes(() =>
        {
            DotNetClean(s => s
                .SetProject(_solution)
                .SetVerbosity(DotNetVerbosity.Quiet));
        });

    Target Restore => _ => _
        .After(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(_solution));
        });

    Target Compile => _ => _
        .After(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(_solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Publish => _ => _
        .Executes(() =>
        {
            _publishPath.DeleteDirectory();

            DotNetClean(s => s
                .SetProject(_projectDir)
                .SetVerbosity(DotNetVerbosity.Quiet)
                .SetConfiguration("Release")
                .SetOutput(_publishPath));

            DotNetPublish(s => s
                .SetProject(_projectDir)
                .SetConfiguration("Release")
                .SetRuntime(_runtimeId)
                .SetOutput(_publishBinPath));

            CopyDirectoryRecursively(_databaseDirectory, _publishDbScriptsPath);

            CopyFileToDirectory(_roundhouseExePath, _publishDbPath);
            CopyFileToDirectory(_dbPackageDeployScript, _publishDbPath);
        });

    Target Test => _ => _
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(_backendTestDir)
                .SetVerbosity(DotNetVerbosity.Quiet));
        });

    Target CiCoverageReport => _ => _
        .After(Compile)
        .Executes(() =>
        {
            DotNet($"dotcover test --dcOutput=\"{_coverageReportXmlPath}\" --dcReportType=\"XML\" --dcFilters=\"{_coverageFilters}\"",
                _backendTestDir);

            var coverageReport = new XmlDocument();
            coverageReport.Load(_coverageReportXmlPath);

            var coveragePercent = int.Parse(coverageReport.SelectSingleNode("/Root/@CoveragePercent").Value);
            if (coveragePercent < _coveragePercentMinimum) { throw new Exception($"Code coverage is {coveragePercent}%. Minimum allowed is {_coveragePercentMinimum}%."); }

            Logger.Info($"{coveragePercent}% code coverage!");
        });


    public static int Main() => Execute<Build>(x => x.CiPipeline);

    Process Run(string exePath, string args = null, bool fromOwnDirectory = false)
    {
        string directory = null;

        if (fromOwnDirectory) { directory = Directory.GetParent(exePath).FullName; }

        return Run(exePath, args, directory);
    }

    Process Run(string exePath, string args, string workingDirectory)
    {
        var startInfo = new ProcessStartInfo(exePath);

        if (workingDirectory != null) { startInfo.WorkingDirectory = workingDirectory; }

        startInfo.Arguments = args ?? string.Empty;
        startInfo.UseShellExecute = false;

        var process = Process.Start(startInfo);

        return process;
    }
}
