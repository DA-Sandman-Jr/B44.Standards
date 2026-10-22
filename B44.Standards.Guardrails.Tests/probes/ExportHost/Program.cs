using System;
using System.Diagnostics;
using System.Reflection;
using ExportCore;

Report(typeof(CoreMarker).Assembly);
Report(Assembly.GetExecutingAssembly());

static void Report(Assembly assembly)
{
    bool disabled = assembly.GetCustomAttribute<DebuggableAttribute>()?.IsJITOptimizerDisabled ?? false;
    Console.WriteLine(assembly.GetName().Name + (disabled ? "=unoptimized" : "=optimized"));
}
