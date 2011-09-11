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

Source Repositories
-------------------
git: [github](https://github.com/caleb-vear/NuSelfUpdate)
 hg: [bitbucket](https://bitbucket.org/calebvear/nuselfupdate)

An Example
----------

NuSelfUpdate lets you write code like this:

```c#
    
    static void Main()
    {
        var appUpdater = CreateAppUpdater();
        
		// This will run the UpdateDatabase method if this is the first 
		// time we have run the application after an update is installed.
        if (appUpdater.JustInstalledUpdates)
		{
			UpgradeDatabase();
			appUpdater.RemoveOldFilesAndClearJustInstalledUpdates();
		}
        
        var updateCheck = appUpdater.CheckForUpdate();
        
        if (updateCheck.UpdateAvailable)
        {
            var preparedUpdate = appUpdater.PrepareUpdate(updateCheck.Update);
            var installedUpdate = appUpdater.ApplyPreparedUpdate(preparedUpdate);
            // Runs the new version of the application with the same command
            // line arguments that we were initially given.
            selfUpdated.LaunchInstalledUpdate(installedUpdate);
            return;
        }
        
        DoYourApplicationStuff();
    }    

    AppUpdater CreateSelfUpdater()
    {
        // Some configuration code here:
        return new AppUpdater(new AppUpdaterConfig("NuSelfUpdate.Sample"));
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