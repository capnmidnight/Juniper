# Juniper

Juniper is a smorgasbord of functionality for building Web and desktop applications. It provides
TypeScript libraries for Web-based front-ends and WebWorkers, C# libraries for .NET 7.0 applications--
including CLI tools, desktop apps, ASP.NET Core Web apps, and hybrid ASP.NET Core/MS Edge WebView2 
desktop apps--and a build manager for managing bundled TypeScript/JavaScript projects
consuming NPM packages and managing simple asset files for ASP.NET Core Web apps.

## For .NET 7.0+

 - (**Juniper**): Useful extensions to the the most common .NET standard library classes, such as Strings, Arrays, Dates, etc.
 - (**Juniper.AppShell**): A system for running ASP.NET Core within the same process as a .NET desktop application.
 - (**Juniper.AppShell.WPF**): An implementation of Juniper.AppShell using a Windows Presentation Foundation window hosting MS Edge WebView2 to interface with the internal server.
 - (**Juniper.Collections**): A few implementations of Tree and Graph structures.
 - (**Juniper.Data**): Utility functions for working with Entity Framework.
 - (**Juniper.Data.NpgSql**): Adapters for Juniper to use PostgreSQL.
 - (**Juniper.Data.Sqlite**): Adapters for Juniper to use SQLite.
 - (**Juniper.Data.SpatiaLite**): Adapters for Juniper to use SQLite with the SpatiaLite GIS extension package.
 - (**Juniper.Emoji**): A database of Unicode code sequences for being able to include pictograms in string literals by name, rather than having to manually look up character codes.
 - (**Juniper.HTTP**): Extension Methods for System.Net.Http.HttpClient and utility functions for working with HTTP Requests/Responses.
 - (**Juniper.IO**): Abstratcions for building data encoders/decoders and serializers/deserializers into file and network stream processing.
 - (**Juniper.Logic**): Functions for declaritively composing complex, executable logic-testing functions from simple lambda expressions.
 - (**Juniper.Mathematics**): Utility functions for basic operations in geometry, linear algebra, and statistics.
 - (**Juniper.MediaType**): A database of [Content--or "MIME"--Types](https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types) for auto-completable recall of content type strings without having to manually look them up.
 - (**Juniper.Processes**): An abstraction around System.Diagnostics.Process to create unified command queues that contain both .NET code and external process execution.
 - (**Juniper.ProcessManager**): A runtime process for use in development that ensures build processes started by the currently debugged application are shut down correctly in the event of an application crash.
 - (**Juniper.Progress**): An abstraction for tracking, subdividing, and combining long process progress trackers for displaying loading bars in UIs.
 - (**Juniper.Server**): Common ASP.NET Core configuration flows for standardization and reuse across projects.
 - (**Juniper.TSBuild**): An NPM project and TypeScript compiler/JavaScript bundle manager that ensures the front-end build processes start before the ASP.NET Core web server starts.
 - (**Juniper.Units**): Functions for formatting and managing conversions between different Units of Measure.

 
## For TypeScript/JavaScript

 - (**[@juniper-lib/util]({@link @juniper-lib/util})**): A smorgasboard of utility classes and functions for:
   - String manipulation, including generating randomized strings and comparing string similarity.
   - Array and Maps manipulation.
   - Blob and Buffered writing tools.
   - Date manipulation.
   - URL manipulation.
   - Disposable object lifecycle management.
   - Feature detection for broad cross-platform support in browsers.
   - Functional Programming.
   - Object-Oriented Programming.
   - Extended math operations including basic statistical analysis.
 - (**[@juniper-lib/appshell]({@link @juniper-lib/appshell})**): An API wrapper for working with Juniper AppShell.
 - (**[@juniper-lib/collections]({@link @juniper-lib/collections})**): Tree and directed Graph structures, as well as Priority lists, maps, and sets.
 - (**[@juniper-lib/dom]({@link @juniper-lib/dom})**): [Document Object Model](https://developer.mozilla.org/en-US/docs/Web/API/Document_Object_Model/Introduction) manipulation functions, providing an explorable, declarative interface for creating HTML document modifications in client code.
 - (**[@juniper-lib/emoji]({@link @juniper-lib/emoji})**): A database of Unicode code sequences for being able to include pictograms in string literals by name, rather than having to manually look up character codes.
 - (**[@juniper-lib/esbuild]({@link @juniper-lib/esbuild})**): A [Node.js](https://nodejs.org/en) package wrapping [esbuild](https://esbuild.github.io/) to standardize usage across projects.
 - (**[@juniper-lib/events]({@link @juniper-lib/events})**): Various utilities for working with and more strongly typing Events, EventTargets, and Promises.
 - (**[@juniper-lib/fetcher]({@link @juniper-lib/fetcher})**): An [XMLHttpRequest](https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest) wrapper with a literate interface that can perform progress tracking, basic caching, and advanced response translations.
 - (**[@juniper-lib/indexdb]({@link @juniper-lib/indexdb})**): A wrapper for [IndexedDB](https://developer.mozilla.org/en-US/docs/Web/API/IndexedDB_API) to provide a typed, Promise-based API.
 - (**[@juniper-lib/mediatypes]({@link @juniper-lib/mediatypes})**): A database of [Content--or "MIME"--Types](https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types) for auto-completable recall of content type strings without having to manually look them up.
 - (**[@juniper-lib/progress]({@link @juniper-lib/progress})**): An abstraction for tracking, subdividing, and combining long process progress trackers for displaying loading bars in UIs.
 - (**[@juniper-lib/sharepoint]({@link @juniper-lib/sharepoint})**): A [SharePoint Classic REST API](https://learn.microsoft.com/en-us/sharepoint/dev/sp-add-ins/get-to-know-the-sharepoint-rest-service?tabs=csom) wrapper that provides auto-completeable recall of SharePoint API methods.
 - (**[@juniper-lib/testing]({@link @juniper-lib/testing})**): A minimal testing framework, with some other useful functions for building test shims.
 - (**[@juniper-lib/timers]({@link @juniper-lib/timers})**): A unified API for timers ([setTimeout](https://developer.mozilla.org/en-US/docs/Web/API/setTimeout), [setInterval](https://developer.mozilla.org/en-US/docs/Web/API/setInterval), [requestAnimationFrame](https://developer.mozilla.org/en-US/docs/Web/API/window/requestAnimationFrame)) in the browser, that make starting and stopping timers easier.
 - (**[@juniper-lib/widgets]({@link @juniper-lib/widgets})**): A collection of custom HTML elements, including Data Tables, Dialogs, Progrss Bars, Tab Panels, Force Directed Graph Drawings, and asynchronous loading of HTML snippets in pages.
 - (**[@juniper-lib/workers]({@link @juniper-lib/workers})**): An API for creating asynchronous, Object-Oriented, message-passing interfaces to multithreaded code running in WebWorkers.
 - (**[@juniper-lib/zip]({@link @juniper-lib/zip})**): Some utility code for manipulating compressed ZIP files.

# Creating a new Project using Juniper

## Step 1: Create a repository

If you already have a repository, skip this step.

```bash
# create a directory for your repo
mkdir MyRepo
cd MyRepo

# make it a git repo
git init

# create a few files for the initial commit
echo 'bin/' > .gitignore # and whatever additional items your project should ignore
echo '# MyRepo' > README.md

# create the commit
git add -A
git commit -m "Initial commit"
```

## Step 2: Add the Juniper submodule to the repository

It's best to put this in the repository root directory.

```bash
git submodule add -b "main" "https://github.com/capnmidnight/Juniper.git" "juniper"
```

## Step 3: Copy the Juniper Visual Studio Solution Template

If you are not using ASP.NET Core, skip this step.

If you do not already have a VS Solution File for your project.

```bash
cp .\juniper\template-solution.sln .\MySolution.sln
```

If you do already have a VS Solution File for your project, refer to `template-solution.sln` to see what projects in Juniper you should add to your solution. It may be easier to backup your existing solution file, copy the template into place, and add your specific projects to the template.

## Step 4: Create an ASP.NET Core Project

Again, if you're not using ASP.NET Core, skip this step.

```bash
mkdir MyProject
cd MyProject
dotnet new webapp
cd ..\
dotnet sln add MyProject\MyProject.csproj
```

## Step 5: Add Base Juniper Project References to your ASP.NET Core Project

 - Choose:
   - `Option A` for apps that will run on the user's desktop computer, or
   - `Option B` for strictly Web-only apps.
 - Additionally, include `Option C` for apps with a heavy TypeScript/JavaScript component.

```bash
cd MyProject # the directory holding your .csproj file

# Option A: for desktop apps
dotnet add reference ..\juniper\NETCore\AppShell.WPF\Juniper.AppShell.WPF.csproj

# Option B: for web-only apps
dotnet add reference ..\juniper\NETCore\Server\Juniper.Server.csproj

# Option C: for JavaScript/TypeScript projects
dotnet add reference ..\juniper\NETCore\TSBuild\Juniper.TSBuild.csproj
```

## Step 6: Create an NPM Project in your ASP.NET Core Project.

### Create file `package.json`. 

*(NOTE: the `@juniper-lib/appshell` dependency is only necessary for desktop applications. Do not include it for Web-only applications.)*
```json
{
  "name": "myproject",
  "version": "0.0.1",
  "private": true,
  "description": "My First Juniper Project",
  "type": "module",
  "scripts": {
    "build": "node esbuild.config.mjs",
    "watch": "node esbuild.config.mjs --watch",
    "juniper-build": "npm run build",
    "juniper-watch": "npm run watch"
  },
  "dependencies": {
    "@juniper-lib/util": "file:../juniper/JS/util",
    "@juniper-lib/appshell": "file:../juniper/JS/appshell",
    "@juniper-lib/collections": "file:../juniper/JS/collections",
    "@juniper-lib/dom": "file:../juniper/JS/dom",
    "@juniper-lib/fetcher": "file:../juniper/JS/fetcher",
    "@juniper-lib/mediatypes": "file:../juniper/JS/mediatypes"
  },
  "devDependencies": {
    "typescript": "5.3.3"
  }
}
```

### Create file `esbuild.config.mjs`

```JavaScript
import { Build, runBuilds } from "@juniper-lib/esbuild";

const args = process.argv.slice(2);

// Builds can run in parallel.
await runBuilds(args,
    // This will iterate through a folder and find all
    // of the `index.ts` files.
    findBundles(false, "Pages"),

    // We can also build bundles meant to be ran as WebWorkers
    findBundles(true, "workers")
);

/**
 * Use this function to customize build settings common across your project.
 * 
 * @param {boolean} isWorker the discovered bundles are meant to be WebWorkers.
 * @param {...string} dirs the directories to search for `index.ts` files.
 */
function findBundles(isWorker, ...dirs) {
    return new Build(args, isWorker)
        .find(...dirs)

        // the common base directory between inputs and outputs.
        .outBase("./")

        // the output location will be the input.ts file's full path, 
        // minus the path of the outBase, prefixed with the path for
        // the outDir.
        .outDir("wwwroot/js/")

        // remove this line if you would prefer minified release bundles
        // to overwrite unminified development bundles.
        .seperateMinifiedFiles(true) 

        // ESBuild can split output bundles into chunks that are common
        // between bundles to help leverage browser caching.
        .splitting(!isWorker);
}
```

### Create file `BuildConfig.cs`

```csharp
using Juniper.TSBuild;

namespace MyNamspace;

/// <summary>
/// The Juniper build system will instantiate this class to retrieve the `Options` property.
/// 
/// Do not add any constructor other than the default constructor.
/// </summary>
internal class BuildConfig : IBuildConfig
{
    /// <summary>
    /// Configuration options for the Juniper build system.
    /// </summary>
    public BuildSystemOptions Options
    {
        get
        {
            var here = BuildSystemOptions.FindSolutionRoot(ProjectName);

            // Juniper has a number of useful assets to include in projects.
            var juniper = here.CD("juniper");
            var assets = new JuniperAssetHelper(juniper);

            // The directory definitions are convenient for referring to script
            // bundle inputs and outputs.
            var project = here.CD(ProjectName);
            var projectWwwRoot = project.CD("wwwroot");
            var projectCSS = projectWwwRoot.CD("css");
            var projectWebfonts = projectWwwRoot.CD("webfonts");
            var projectPagesInput = project.CD("Pages");
            var projectJSOutput = projectWwwRoot.CD("js");
            var projectPagesOutput = projectJSOutput.CD("Pages");

            // The basic design of Juniper projects has client TypeScript code
            // living in the same directory as its related Razor Pages code. We
            // can also define a series of other assets that get reused and
            // translated into different locations.
            var projectAssets = projectPagesInput
                // Allows one to store addtional files not part of the TS/JS bundling
                // along-side your pages.
                .CopyFiles(
                    projectPagesOutput, SearchOption.AllDirectories,
                    file => file.Extension != ".cshtml"
                        && file.Extension != ".cs"
                        && file.Extension != ".ts"
                        && file.Extension != ".js"
                        && file.Extension != ".css")
                // Don't forget an application logo!
                .Append(here.Touch("MyProjectLogo.ico").CopyFile(projectWwwRoot.Touch("favicon.ico")));

            // Some optional dependencies you may find useful in your project
            var dependencies = projectAssets
                .Union(assets.WebFonts.Noto.CopyFiles(projectWebfonts.CD("Noto")))
                .Union(assets.CSS.FontAwesome.CopyFiles(projectCSS))
                .Append(assets.CSS.Bootstrap.CopyFile(projectCSS));

            return new BuildSystemOptions
            {
                Project = project,
                CleanDirs = new[] { projectJSOutput },
                Dependencies = dependencies
            };
        }
    }
}
```

### Create file `appsettings.json`. 


*(NOTE: The `Classification` section accepts any string value, but is a convenient place to record and later read in your application for displaying appropriate banners.)*

*(NOTE: The `Version` section is a convient place to record and later read in your application for displaying as "About" information. Juniper will automatically synchronize this value with the value in `package.json`)*

*(NOTE: The `AllowedHosts`, `UseSession`, and `AppShell` sections are required when using Juniper for desktop applications.)*

*(NOTE: The `Logging`, `DefaultFiles`, and `ContentTypes` sections are only provided as an example.)*

```json
{
  "Classification": "NONE",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Error",
      "Microsoft.EntityFrameworkCore.Database.Command": "Error"
    }
  },
  "ContentTypes": {
    ".geojson": "application/geo+json",
    ".kml": "application/vnd.google-earth.kml+xml",
    ".kmz": "vnd.google-earth.kmz",
    ".webmanifest": "application/manifest+json"
  },
  "DefaultFiles": [
    "index.html"
  ],
  "AllowedHosts": "*",
  "UseSession": true,
  "AppShell": {
    "Window": {
      "Title": "MyProject",
      "Maximized": true
    },
    "SplashScreenPath": "splash.html"
  },
  "Version": "0.0.1"
}
```

### Create file `Properties\launchSettings.json`


*(NOTE: If you are not using `Juniper.AppShell` to create a desktop application, only the `(Browser)` profile is necessary, and it does not need the line `"commandLineArgs": "--no-appshell"`.)*

```json
{
  "profiles": {
    "MyProject (Browser)": {
      "commandName": "Project",
      "commandLineArgs": "--no-appshell",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation"
      },
      "dotnetRunMessages": true,
      "applicationUrl": "http://localhost:5001"
    },
    "MyProject (AppShell)": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation"
      },
      "dotnetRunMessages": true,
      "externalUrlConfiguration": true
    }
  }
}
```

### Create file `Program.cs`

*(NOTE: There are additional, Juniper-specific configuration lines when using Entity Framework, discussed later in this document. You may also perform your own ASP.NET Core configuration here.)*

```csharp
using Juniper.AppShell;
using Juniper.Services;

namespace MyNamespace;

await WebApplication
    // Standard ASP.NET Core method
    .CreateBuilder(args)
    // Configures a slew of defaults that a good for a wide variety of applications
    .ConfigureJuniperWebApplication()
    // Reads your `BuildConfig.cs` file to manage assets and build JavaScript bundles.
    .AddJuniperBuildSystem<BuildConfig>()
    // (optional) only necessary for desktop applications
    .ConfigureJuniperAppShell<WpfAppShellFactory>()
    // Standard ASP.NET Core method
    .Build()
    .ConfigureJuniperRequestPipeline()
    // Waits for the Build System before starting the application, then waits for 
    // the application to exit.
    .BuildAndRunAsync();
```

# Suggested base .gitignore for ASP.NET Core Projects using Juniper
```
node_modules/
bin/
obj/
.vs/
*.user
```

# FAQ

## Why do some or all references to Juniper JS packages break after pulling the latest version from the repository?

While Juniper attempts to bundle the built output of its TypeScript packages in the repository, occasionally the TypeScript compiler
loses track of where the files are located and complains about them missing, when really they are not.

In this case, forcing a complete rebuild of Juniper usually fixes the issue.

```sh
powershell .\juniper\fix.ps1
```


# TODO

[Setup DocFX to use TypeDoc to merge the two sets of code documentation together](https://xxred.gitee.io/docfx/tutorial/universalreference/gen_doc_for_ts.html)

