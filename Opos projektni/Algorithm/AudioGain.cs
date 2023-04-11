using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni.Algorithm
{
    [Serializable]
    public class AudioGain : IAction
    {
        private List<String> paths = new();
        private String outputFolder = "OutputFolder";
        private float gainFactor = 1.0f;

        public List<String> InputPaths { get { return this.paths; } set { this.paths = value; } }
        public String OutputFolder { get { return this.outputFolder; } set { this.outputFolder = value; } }
        public float GainFactor { get { return this.gainFactor; } set { this.gainFactor = value; } }

        public AudioGain()
        {

        }

        public AudioGain(List<String> path, String outputFolder, float gainFactor)
        {
            this.paths = path;
            this.gainFactor = gainFactor;
            this.outputFolder = outputFolder;
        }


        public void Run(ICoopApi api)
        {
            if (paths.Count != 1)
            {
                int curr = 1;
                Parallel.For(0, paths.Count, new ParallelOptions { MaxDegreeOfParallelism = api.GetMaxDegreeOfParalellism() }, j =>
                {
                    WaveFile file;
                    try
                    {
                        file = WaveFile.ReadWaveFile(paths[j]);
                        var data = file.Data;
                        for (int i = 0; i < data.Length / 2; i++)   //idemo kroz uzorke /2 16bitni kodovanje
                        {
                            api.CheckForPause();
                            api.CheckForStop();
                            api.CheckForContextSwitch();
                            short sample = BitConverter.ToInt16(data, i * 2);
                            sample = (short)(sample * gainFactor);
                            var bytes = BitConverter.GetBytes(sample);
                            Array.Copy(bytes, 0, data, i * 2, 2);
                        }
                        WaveFile.WriteWaveFile(file, this.getFileName(paths[j]));

                        curr++;
                        api.SetProgress(curr / (float)paths.Count);
                    }
                    catch (Exception e) { Console.WriteLine(e.Message); }
                });

            }
            else
            {
                try
                {
                    WaveFile file = WaveFile.ReadWaveFile(paths[0]);
                    var data = file.Data;
                    int numOfSample = 1;
                    Parallel.For(0, data.Length / 2, new ParallelOptions { MaxDegreeOfParallelism = api.GetMaxDegreeOfParalellism() }, i =>
                    {
                        api.CheckForPause();
                        api.CheckForStop();
                        api.CheckForContextSwitch();
                        short sample = BitConverter.ToInt16(data, i * 2);
                        sample = (short)(sample * gainFactor);
                        var bytes = BitConverter.GetBytes(sample);
                        numOfSample++;
                        Array.Copy(bytes, 0, data, i * 2, 2);
                        api.SetProgress(numOfSample / (float)(data.Length / 2));
                    });
                    WaveFile.WriteWaveFile(file, this.getFileName(paths[0]));
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }


        }

        private String getFileName(String inputPath)
        {
            String name = Path.GetFileNameWithoutExtension(inputPath) + "_Amplified" + ".wav";
            string path = Path.Combine(this.outputFolder, name);
            return path;
        }
    }
}
