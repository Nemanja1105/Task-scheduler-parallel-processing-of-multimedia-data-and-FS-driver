using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Opos_projektni.Serialization
{
    public class SchedulerSerialization
    {
        private static readonly string SAVED_CONTEXTS_PATH = "SavedContexts";
        private static readonly string SAVED_TASKS = "SavedTasks";
        private static readonly string SCHEDULER_FILE_NAME = "scheduler.bin";


        public static String Serialize(Opos_projektni.TaskScheduler scheduler)
        {
            if (!File.Exists(SAVED_CONTEXTS_PATH))
                Directory.CreateDirectory(SAVED_CONTEXTS_PATH);
            lock (scheduler)
            {
                String outputPath = SAVED_CONTEXTS_PATH + Path.DirectorySeparatorChar + DateTime.Now.Ticks;
                String taskFolder = outputPath + Path.DirectorySeparatorChar + SAVED_TASKS;
                Directory.CreateDirectory(taskFolder);
                foreach (var task in scheduler.allTask)
                {
                    lock (task)
                    {
                        if (scheduler.stopWatches.ContainsKey(task))
                            task.specification.ExecutionTime = task.specification.ExecutionTime - (int)scheduler.stopWatches[task].Elapsed.TotalMilliseconds;
                        SerializeTask(taskFolder + Path.DirectorySeparatorChar + task.Name + ".bin", task);
                    }
                }
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(outputPath + Path.DirectorySeparatorChar + SCHEDULER_FILE_NAME, FileMode.Create))
                {
                    formatter.Serialize(stream, scheduler);
                }
                return outputPath;
            }
        }



        public static TaskScheduler Deserialize(String inputPath)
        {
            IFormatter formatter = new BinaryFormatter();
            TaskScheduler? scheduler = null;
            using (Stream stream = new FileStream(inputPath + Path.DirectorySeparatorChar + SCHEDULER_FILE_NAME, FileMode.Open))
            {
                scheduler = (TaskScheduler)formatter.Deserialize(stream);
            }
            if (scheduler == null)
                return null;
            String taskFolder = inputPath + Path.DirectorySeparatorChar + SAVED_TASKS;
            var files = Directory.GetFiles(taskFolder);
            foreach (var file in files)
            {
                Task? task = DeserializeTask(file);
                if (task != null)
                {
                    if (task.state != Task.TaskState.TERMINATED)
                        scheduler.Schedule(task.specification);
                }
            }
            return scheduler;
        }

        public static String[] GetSavedContexts()
        {
            if (!File.Exists(SAVED_CONTEXTS_PATH))
                Directory.CreateDirectory(SAVED_CONTEXTS_PATH);

            return Directory.GetDirectories(SAVED_CONTEXTS_PATH);
        }

        internal static void SerializeTask(string outputPath, Opos_projektni.Task task)
        {

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(outputPath, FileMode.Create))
            {
                formatter.Serialize(stream, task);
            }

        }

        internal static Task DeserializeTask(string inputPath)
        {
            IFormatter formatter = new BinaryFormatter();
            Task task = null;
            using (Stream stream = new FileStream(inputPath, FileMode.Open))
            {
                task = (Task)formatter.Deserialize(stream);
                return task;
            }
        }
    }
}
