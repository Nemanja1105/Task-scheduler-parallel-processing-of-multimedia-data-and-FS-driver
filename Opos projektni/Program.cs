using Opos_projektni;
using Opos_projektni.Algorithm;
using Opos_projektni.SchedulingAlgorithms;
using Opos_projektni.Serialization;
using System.Diagnostics;
using System.Text.Json;
using System.Timers;

namespace Opos
{
    class Program
    {
        public static void Main(String[] args)
        {

            //Opos_projektni.TaskScheduler scheduler = new Opos_projektni.TaskScheduler(1, new FifoScheduling());
            // var task=scheduler.Schedule(new TaskSpecification(new MyTask(numOfIteration:10,"AAA"),name:"task1"));
            //scheduler.Schedule(new TaskSpecification(new MyTask(numOfIteration: 15, "message"),name:"task2"));
            // Thread.Sleep(3000);
            //SchedulerSerialization.Serialize(scheduler);
            //Opos_projektni.Serialization.SchedulerSerialization.Serialize(scheduler);
            // var scheduler=SchedulerSerialization.Deserialize("C:\\Users\\neman\\Desktop\\OPOS projektni\\Opos projektni\\Opos projektni\\bin\\Debug\\net6.0\\SavedContexts\\638113275400834354");



            /* // IAction task = new MyTask();
              var str = JsonSerializer.Serialize<Opos_projektni.Task>(task,new JsonSerializerOptions
              {
                  Converters =
                  {
                      new PolymorphicJsonConverter<Opos_projektni.Task>()
                  }
              });
              Console.WriteLine(str);

              IAction action = JsonSerializer.Deserialize<IAction>(str, new JsonSerializerOptions
              {
                  Converters =
                  {
                      new PolymorphicJsonConverter<Opos_projektni.Task>()
                  }
              });
              Console.WriteLine(action.GetType());*/
            /* Opos_projektni.TaskScheduler scheduler = new Opos_projektni.TaskScheduler(2, new PriorityPreemptiveScheduling());
             scheduler.AddResource(new Resource("fajl1"));
             scheduler.AddResource(new Resource("fajl2"));
             var task1 = scheduler.Schedule(new TaskSpecification(new MyTask(numOfIteration: 5, "AAA"),priority:1));
             Thread.Sleep(500);
             var task2 = scheduler.Schedule(new TaskSpecification(new MyTask(numOfIteration: 6, "AAA"),priority:8));
             Thread.Sleep(500);
             var task5 = scheduler.Schedule(new TaskSpecification(new MyTask2(), priority: 15));
             Thread.Sleep(500);
             var task6 = scheduler.Schedule(new TaskSpecification(new MyTask2(), priority: 16));
             Thread.Sleep(500);
             var task3 = scheduler.Schedule(new TaskSpecification(new MyTask(numOfIteration: 7, "AAA"),priority:5)); //problematicni slucaj task4 ne moze da se rasporedi
             Thread.Sleep(500);
             var task4 = scheduler.Schedule(new TaskSpecification(new MyTask(numOfIteration: 7, "AAA"), priority: 10));*/

            // task1.Wait();


            Console.ReadLine();
            /*read.Sleep(2000);
            task.StartTaskScheduling();
            task.Wait();*/
            //scheduler.Schedule(new TaskSpecification(new MyTask(), priority: 1));
            /*var list = typeof(SchedulingAlgorithm).Assembly.GetTypes().Where((t) => t.IsSubclassOf(typeof(SchedulingAlgorithm))).ToList();
            list.ForEach((t) => { Console.WriteLine(t.Name); });*/


            /* scheduler.Schedule(new TaskSpecification(new MyTask(), priority: 1));
             scheduler.Schedule(new TaskSpecification(new MyTask(), priority: 3, startingDateTime: DateTime.Now.AddSeconds(2)));
             scheduler.Schedule(new TaskSpecification(new MyTask(),priority:2,startingDateTime: DateTime.Now.AddSeconds(3)));*/
            /*Opos_projektni.TaskScheduler scheduler = new Opos_projektni.TaskScheduler(1, new PriorityPreemptiveScheduling());
            scheduler.Schedule(new TaskSpecification(new MyTask(),priority:2));
            scheduler.Schedule(new TaskSpecification(new MyTask(),priority:1));*/
            //TaskSpecification spec1 = new TaskSpecification(action:new MyTask(), startingDateTime:DateTime.Now.AddSeconds(5));
            /*  TaskSpecification spec2 = new TaskSpecification(new MyTask(),executionTime:4000);
              TaskSpecification spec3 = new TaskSpecification(new MyTask(),priority:2);
              TaskSpecification spec4 = new TaskSpecification(new MyTask(),priority: 3);
              TaskSpecification spec5 = new TaskSpecification(new MyTask(),priority:4);*/

            /* var t2=scheduler.Schedule(spec2);
             Thread.Sleep(1000);
            t2.RequestPause();
           Thread.Sleep(5000);
           t2.RequestContinue();
             t2.Wait();
             Console.WriteLine(scheduler.stopWatches[t2].ElapsedMilliseconds);*/


            /*var t1 = scheduler.Schedule(spec1);
             var t2 = scheduler.Schedule(spec2);

             var t3 = scheduler.Schedule(spec3);
             var t4 = scheduler.Schedule(spec4);
             var t5 = scheduler.Schedule(spec5);*/
            //WaveFile file = WaveFile.ReadWaveFile("test.wav");
            //  WaveFile.WriteWaveFile(file, "test2.wav");
            /* List<String> path = new List<String>();
             path.Add("C:\\Users\\neman\\Desktop\\OPOS projektni\\Opos projektni\\Opos projektni\\test.wav");
             AudioGain alg = new AudioGain(path, "C:\\Users\\neman\\Desktop", 5.0f);
             Opos_projektni.TaskScheduler scheduler = new Opos_projektni.TaskScheduler(1,new RoundRobinScheduling(1000));
             var spec = new TaskSpecification(alg,maxDegreeOfParallelism:4);
             var t1=scheduler.Schedule(spec);
             scheduler.Schedule(new TaskSpecification(new MyTask()));*/









        }

    }


    }

