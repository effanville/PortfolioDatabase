﻿using System;

namespace Effanville.FPD.Logic.Tests.TestHelpers
{
    public static class TestConstants
    {
        public static readonly string CurrentPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public const string ExampleDatabaseFolder = "ExampleDatabases";

        public static readonly string ExampleDatabaseLocation = $"{CurrentPath}\\{ExampleDatabaseFolder}";
    }
}
