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

An Example
----------

This example shows what we are aiming for.

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