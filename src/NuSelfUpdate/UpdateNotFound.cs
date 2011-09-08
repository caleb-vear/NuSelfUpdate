﻿using NuGet;

namespace NuSelfUpdate
{
    public class UpdateNotFound : IUpdateCheck
    {
        public bool UpdateAvailable
        {
            get { return false; }
        }

        public IPackage UpdatePackage
        {
            get { return null; }
        }
    }
}