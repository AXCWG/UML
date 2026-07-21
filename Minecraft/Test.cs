#!/usr/bin/env dotnet

#:package CmlLib.Core@4.0.6
#:package AXExpansion@*
#:property PublishAot=false
using AXExpansion;
using CmlLib.Core;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;

Console.WriteLine("Hello. ");

var launcher = new MinecraftLauncher(new MinecraftLauncherParameters()
{
    
});
foreach (var versionMetadata in await launcher.GetAllVersionsAsync())
{
    versionMetadata.Name.PrintLn();
}