using Nuke.Common.IO;

partial class Build
{
    AbsolutePath _baseDir => _solution.Directory;
    AbsolutePath _projectDir => _solution.Directory / _solutionName;
    AbsolutePath _backendTestDir => _solution.Directory / $"{_solutionName}.Tests";
    AbsolutePath _testExeDir => _backendTestDir / "bin" / "Debug" / _targetFramework;
    AbsolutePath _testExePath => _testExeDir / $"{_solutionName}.Tests.exe";
    AbsolutePath _dotCoverDir => _baseDir / "tools" / "dotCover";
    AbsolutePath _dotCoverExePath => _dotCoverDir / "dotCover.exe";
    AbsolutePath _coverageReportHtmlPath => _baseDir / "CoverageReports" / "CoverageReport.html";
    AbsolutePath _coverageReportXmlPath => _baseDir / "CoverageReports" / "CoverageReport.xml";
    AbsolutePath _roundhouseDir => _baseDir / "tools" / "roundhouse";
    AbsolutePath _roundhouseExePath => _roundhouseDir / "rh.exe";
    AbsolutePath _dbPackageDeployScript => _roundhouseDir / "Deploy.ps1";
    AbsolutePath _databaseDirectory => _baseDir / "db";
    AbsolutePath _createDatabaseScript => _databaseDirectory / "create.sql";
    AbsolutePath _publishPath => _baseDir / "publish";
    AbsolutePath _publishBinPath => _baseDir / "publish" / "bin";
    AbsolutePath _publishDbPath => _baseDir / "publish" / "db";
    AbsolutePath _publishDbScriptsPath => _baseDir / "publish" / "db" / "scripts";
}
