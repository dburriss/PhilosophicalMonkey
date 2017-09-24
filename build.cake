#addin "Newtonsoft.Json"
#tool "nuget:?package=GitVersion.CommandLine"
#addin "Cake.Powershell"

using Newtonsoft.Json;
using System.Linq;
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var solutionDir = Directory("./");
var solutionFile = File(GetFiles("./*.sln").First().FullPath);
var globalFile = solutionDir + File("global.json");

bool print = true;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
void Print(object obj)
{
    if(!print)
        return;
    
    if(obj == null)
        Information("{0}", "obj is null");

    var objAsJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
    Information("{0}", objAsJson);
}

Task("Clean")
    .Does(() =>
{
    var projects = GetFiles("./**/*.csproj");
    foreach(var proj in projects)
    {
        var projDir = proj.GetDirectory();
        var buildDir = Directory(projDir + "/bin");
        CleanDirectory(buildDir);
    }
    CleanDirectory("./artifacts");
});


Task("Restore")
    .Does(() =>
{    
    DotNetCoreRestore();
});


// Task("Update-Version")
//     .Does(() => 
// {
//     GitVersion(
//         new GitVersionSettings {
//             UpdateAssemblyInfo = true,
//             UpdateAssemblyInfoFilePath = new FilePath(sensuProjDir + "Properties/AssemblyInfo.cs")
//         });
//     string version = GitVersion().FullSemVer;
//     Information(version);

// }).OnError(exception =>
// {
//     Error(exception.ToString());
// });


Task("Build")
    .IsDependentOn("Restore")
    //.IsDependentOn("Update-Version")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild(solutionFile, settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild(solutionFile, settings =>
        settings.SetConfiguration(configuration));
    }
});


Task("Rebuild")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .Does(() =>
{ });


Task("Package")
    .Does(() => 
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = "Release",
        OutputDirectory = "./artifacts/"
    };
    if(System.IO.File.Exists(globalFile))
    {
        var project = Newtonsoft.Json.Linq.JObject.Parse(
                System.IO.File.ReadAllText(globalFile, Encoding.UTF8)
            );
        //Print(project);
        
        foreach(var folder in project["projects"])
        {
            Information(folder);
            var csproj = GetFiles(solutionDir + folder + "/*.csproj").First();
            DotNetCorePack(csproj.FullPath, settings);
        }
    }
    else
    {
        var projects = GetFiles("./**/*.csproj");
        foreach(var proj in projects)
        {
            var makePackage = XmlPeek(proj.FullPath, "/Project/PropertyGroup[1]/GeneratePackageOnBuild");
            bool bMakePackage = false;
            if(bool.TryParse(makePackage, out bMakePackage) && bMakePackage)
            {
                DotNetCorePack(proj.FullPath, settings);
            }            
        }
    }
});


Task("Test")
    .Does(() =>
{
    var testProjects = GetFiles("./**/*.Tests.csproj");
    Information("Test Projects: " + testProjects.Count());
    foreach(var testProj in testProjects)
    {
        Information(testProj.FullPath);
        DotNetCoreTest(testProj.FullPath);
    }
});

Task("Run")
    .Does(() =>
{
    //DotNetCoreRun("./Examples/.../x.csproj");
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Rebuild")
    .IsDependentOn("Test")
    .IsDependentOn("Package");
    
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);