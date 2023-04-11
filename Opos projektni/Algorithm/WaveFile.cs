using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni.Algorithm
{
    public class WaveFile
    {
        public int ChunkId { get; set; }
        public int ChunkSize { get; set; }
        public int Format { get; set; }
        public int SubChunk1Id { get; set; }
        public int SubChunk1Size { get; set; }
        public short AudioFormat { get; set; }
        public short NumChannels { get; set; }
        public int SampleRate { get; set; }
        public int ByteRate { get; set; }
        public short BlockAlign { get; set; }
        public short BitsPerSample { get; set; }
        public int SubChunk2Id { get; set; }
        public int SubChunk2Size { get; set; }
        public byte[] Data { get; set; }

        public WaveFile()
        {

        }

        public WaveFile(int chunkId, int chunkSize, int format, int subChunk1Id, int subChunk1Size, short audioFormat, short numChannels, int sampleRate, int byteRate, short blockAlign, short bitsPerSample, int subChunk2Id, int subChunk2Size, byte[] data)
        {
            this.ChunkId = chunkId;
            this.ChunkSize = chunkSize;
            this.Format = format;
            this.SubChunk1Id = subChunk1Id;
            this.SubChunk1Size = subChunk1Size;
            this.AudioFormat = audioFormat;
            this.NumChannels = numChannels;
            this.SampleRate = sampleRate;
            this.ByteRate = byteRate;
            this.BlockAlign = blockAlign;
            this.BitsPerSample = bitsPerSample;
            this.SubChunk2Id = subChunk2Id;
            this.SubChunk2Size = subChunk2Size;
            this.Data = data;
        }

        public static WaveFile ReadWaveFile(String path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("Fajl ne postoji na zadatoj putanji");
            if(!path.EndsWith(".wav"))
                throw new ArgumentException("Nepodrzani tip fajla!!");
            WaveFile file = new WaveFile();
            using(var stream=File.Open(path,FileMode.Open))
            {
                using(var reader=new BinaryReader(stream))
                {
                    file.ChunkId = reader.ReadInt32();
                    file.ChunkSize = reader.ReadInt32();
                    file.Format = reader.ReadInt32();
                    if (file.ChunkId != 0x46464952 || file.Format != 0x45564157) //RAFF i WAVE u big endian
                        throw new ArgumentException("Pogresan format fajla");


                    file.SubChunk1Id = reader.ReadInt32();
                    file.SubChunk1Size = reader.ReadInt32();
                    file.AudioFormat = reader.ReadInt16();
                    if (file.AudioFormat != 1)
                        throw new ArgumentException("Pogresan format fajla");

                    file.NumChannels = reader.ReadInt16();
                   /* if (file.NumChannels != 1)
                        throw new ArgumentException("Pogresan format fajla. Algoritam podrzava rad sa samo jednim kanalom");*/
                    file.SampleRate = reader.ReadInt32();
                   /* if (file.SampleRate != 48000 && file.SampleRate != 44100 && file.SampleRate != 32000)
                        throw new ArgumentException("Pogresan format fajla. Algoritam podrzava sample rate 32000,44100 i 48000");*/
                    file.ByteRate = reader.ReadInt32();
                    file.BlockAlign = reader.ReadInt16();
                    file.BitsPerSample = reader.ReadInt16();
                    if(file.BitsPerSample!=16)
                        throw new ArgumentException("Pogresan format fajla. Algoritam podrzava 16bitne odmjerke");
                    file.SubChunk2Id = reader.ReadInt32();
                    file.SubChunk2Size=reader.ReadInt32();
                    file.Data = reader.ReadBytes(file.SubChunk2Size);
                }
            }
            return file;
        }

        public static void WriteWaveFile(WaveFile file,String path)
        {
            if (!path.EndsWith(".wav"))
                throw new ArgumentException("Nepodrzani tip fajla!!");
            using(var stream=File.Open(path,FileMode.Create))
            {
                using(var writer=new BinaryWriter(stream))
                {
                    writer.Write(file.ChunkId);
                    writer.Write(file.ChunkSize);
                    writer.Write(file.Format);
                    writer.Write(file.SubChunk1Id);
                    writer.Write(file.SubChunk1Size);
                    writer.Write(file.AudioFormat);
                    writer.Write(file.NumChannels);
                    writer.Write(file.SampleRate);
                    writer.Write(file.ByteRate);
                    writer.Write(file.BlockAlign);
                    writer.Write(file.BitsPerSample);
                    writer.Write(file.SubChunk2Id);
                    writer.Write(file.SubChunk2Size);
                    writer.Write(file.Data);
                }
            }

        }





    }
}
