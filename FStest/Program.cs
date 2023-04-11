
string sourceFolder = "Test";
string targetFolder = "G:\\input\\";

foreach (string filePath in Directory.EnumerateFiles(sourceFolder).ToList())
{
    string fileName = Path.GetFileName(filePath);
    string targetPath = Path.Combine(targetFolder, fileName);
    File.Copy(filePath, targetPath);
}
List<string> files = new List<string>();
files = Directory.EnumerateFiles("G:\\input\\").ToList();


Opos_projektni.TaskScheduler ts = new Opos_projektni.TaskScheduler(1, new Opos_projektni.FifoScheduling());

ts.Schedule(new Opos_projektni.TaskSpecification(new Opos_projektni.Algorithm.AudioGain(files, "G:\\output\\", 5.0f)));

Console.ReadLine();


