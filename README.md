### NuSelfUpdate ###
NuSelfUpdate allows you to easily update your applications by simply 
publishing a new version to a NuGet package repository.

NuSelfUpdate will handle everything that is needed to download the new
version, extract it and switch the application's executable files with
the new version.  It will do all this while your application is running;
so you can install updates in the background and next time the application
is run it will be the new version. To top it all off you can easily relaunch
the new version of the application with the same command line arguments the
app was originally with.

Contributing
------------
Contributions are welcome; fork NuSelfUpdate and submit a pull request :) 
If your looking for something to do you can always check out [issues page](https://github.com/caleb-vear/NuSelfUpdate/issues)
to see what features or bugs need some work.  If you have an awesome idea feel free to submit an [issue](https://github.com/caleb-vear/NuSelfUpdate/issues/new), but pull requests are best.

You can clone/fork NuSelfUpdate with either git or mercurial:

* [github](https://github.com/caleb-vear/NuSelfUpdate)
* [bitbucket](https://bitbucket.org/calebvear/nuselfupdate)

An Example
----------

NuSelfUpdate lets you write code like this:

```c#
    
    static void Main()
    {
        var appUpdater = CreateAppUpdater();
        
		// This will run the UpdateDatabase method if this is the first 
		// time we have run the application after an update is installed.
        if (appUpdater.OldVersionExists)
		{
			UpgradeDatabase();
			appUpdater.RemoveOldVersionFiles();
		}
        
        var updateCheck = appUpdater.CheckForUpdate();
        
        if (updateCheck.UpdateAvailable)
        {
            var preparedUpdate = appUpdater.PrepareUpdate(updateCheck.Update);
            var installedUpdate = appUpdater.ApplyPreparedUpdate(preparedUpdate);
            // Runs the new version of the application with the same command
            // line arguments that we were initially given.
            appUpdater.LaunchInstalledUpdate(installedUpdate);
            return;
        }
        
        DoYourApplicationStuff();
    }    

    AppUpdater CreateSelfUpdater()
    {
        // Some configuration code here:
        return new AppUpdaterBuilder("NuSelfUpdate.Sample").Build();
    }

    void UpgradeDatabase()
    {
        // Do some important stuff here.
    }
    
    void DoYourApplicationStuff()
    {
        // What you would normally have in your main method goes here
    }
```

Building The Project
--------------------
While the simplest way to get your hands on NuSelfUpdate is via nuget it is possible you might like 
to make your own modifications and then build them.  This can useful if for example you complete
a new feature and it hasn't been pulled into the main repository yet.

Running the build is simple, all you have to do is fire up powershell navigate to the source directory root
and run the following:

    PS> .\psake.sp1
	
A directory named build will be created which will contain a brand new nupkg file.  It also contains the raw
binary files in the /bin/ folder and all build log output in a folder named /output/.

Running The Sample
------------------
The NuSelfUpdate source includes a working sample of the library in action.
A psake powershell script has been provided to facilitate the running of the sample.
The following instructions assume that you will have powershell open and the current directory
is the root of the source repository.

The first step is to build and run the sample.  To build the sample and start it run the following:

    PS> .\psake.ps1 sample.ps1 RunSample
	
A new console window will open up and start checking for updates.  Not the version number shown on the first line.
Next up to publish a new version run the following:

    PS> .\psake.ps1 sample.ps1 PublishNewVersion
	
Running PowerShell Scripts
--------------------------
Depending on your powershell script execution policy you may have difficulty running the script.  
See [this page](http://technet.microsoft.com/en-us/library/ee176949.aspx) for more information.
The simplest way to enable the script is to run the following from an admin powershell console:

    PS> Set-ExecutionPolicy Unrestricted

You should however understand that can pose security risks so you might like to set it back to _Restricted_ afterwards