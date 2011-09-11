using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuSelfUpdate.Tests.Helpers;
using Shouldly;

namespace NuSelfUpdate.Tests.ApplyPreparedUpdateBehaviour
{
    public abstract class ApplyUpdateTest : BddifyTest
    {
        protected const string PrepDir = @"c:\app\.updates\1.1\";
        protected const string AppDirectory = @"c:\app\";
        protected const string OldDir = @"c:\app\.old\";

        protected MockFileSystem FileSystem { get; set; }

        protected void VerifyFile(string file, Version version)
        {
            FileSystem.ReadAllText(file).ShouldBe(MockFileContent(Path.GetFileName(file), version));
        }

        protected void VerifyDirectoryFiles(string directory, IDictionary<string, Version> expectedFiles)
        {
            foreach (var file in expectedFiles)
            {
                VerifyFile(Path.Combine(directory, file.Key), file.Value);
            }

            FileSystem.GetFiles(directory)
                .Select(Path.GetFileName)
                .All(f => expectedFiles.ContainsKey(f)).ShouldBe(true);
        }

        protected static string MockFileContent(string file, Version version)
        {
            return file + " - v" + version;
        }
    }
}