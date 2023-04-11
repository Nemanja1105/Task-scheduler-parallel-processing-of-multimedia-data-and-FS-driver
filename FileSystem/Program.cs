// See https://aka.ms/new-console-template for more information
using DokanNet;
using DokanNet.Logging;
char driveLetter = 'G';

using (ConsoleLogger consoleLogger = new("[Dokan]"))
using (Dokan dokan = new(consoleLogger))
{
    string mountPoint = $"{driveLetter}:\\";
    FileSystem.FS myFs = new();
    DokanInstanceBuilder dokanInstanceBuilder = new DokanInstanceBuilder(dokan)
        .ConfigureLogger(() => consoleLogger)
        .ConfigureOptions(options =>
        {
            options.Options = DokanOptions.DebugMode;
            options.MountPoint = mountPoint;
        });
    using DokanInstance dokanInstance = dokanInstanceBuilder.Build(myFs);
    Console.ReadLine();
}
