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
}
