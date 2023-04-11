using Opos_projektni;
using Opos_projektni.SchedulingAlgorithms;

namespace Opos_projektni
{
    public class TaskSchedulerTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ScheduleTest()
        {
            TaskScheduler scheduler = new TaskScheduler(1, new FifoScheduling());
            var task = scheduler.Schedule(new TaskSpecification(new MyTask()));
            lock (task)
            {
                Assert.AreEqual(Task.TaskState.RUNNING, task.State);
            }
        }

        [Test]
        public void ScheduleWithoutStarting()
        {
            TaskScheduler scheduler = new TaskScheduler(1, new FifoScheduling());
            var task = scheduler.ScheduleWithoutStarting(new TaskSpecification(new MyTask()));
            lock (task)
            {
                Assert.AreEqual(Task.TaskState.SCHEDULED_NOT_STARTED, task.State);

            }
        }

        [Test]
        public void ScheduleMoreThenOneTask()
        {
            TaskScheduler scheduler = new TaskScheduler(1, new FifoScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            var task2 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            var task3 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            task1.Wait(); task2.Wait(); task3.Wait();
            lock (scheduler)
            {
                Assert.AreEqual(Task.TaskState.TERMINATED, task1.State);
                Assert.AreEqual(Task.TaskState.TERMINATED, task2.State);
                Assert.AreEqual(Task.TaskState.TERMINATED, task3.State);
            }
        }

        [Test]
        public void StartSchedulingTest()
        {
            TaskScheduler scheduler = new TaskScheduler(0, new FifoScheduling());
            var task = scheduler.ScheduleWithoutStarting(new TaskSpecification(new MyTask()));
            task.StartTaskScheduling();
            lock (task)
            {
                Assert.AreEqual(Task.TaskState.SCHEDULED, task.State);

            }
        }

        [Test]
        public void PauseTest()
        {
            TaskScheduler scheduler = new TaskScheduler(1, new FifoScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            task1.RequestPause();
            lock (task1)
            {
                Assert.That(task1.State == Task.TaskState.RUNNING_WITH_PAUSE_REQUEST || task1.State == Task.TaskState.PAUSED);
            }
        }

        [Test]
        public void ContinueTest()
        {
            TaskScheduler scheduler = new TaskScheduler(1, new FifoScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            task1.RequestPause();
            task1.RequestContinue();
            //task1.Wait();//ako sve radi na kraju cemo se terminirati
            lock (task1)
            {
                Assert.That(task1.State == Opos_projektni.Task.TaskState.WAITING_TO_RESUME || task1.State == Opos_projektni.Task.TaskState.RUNNING);
            }
        }

        [Test]
        public void TerminateTest()
        {
            Opos_projektni.TaskScheduler scheduler = new Opos_projektni.TaskScheduler(1, new FifoScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            task1.RequestTerminate();
            task1.Wait();
            lock (task1)
            {
                Assert.AreEqual(Opos_projektni.Task.TaskState.TERMINATED, task1.State);
            }
        }

        [Test]
        public void WaitTest()
        {
            Opos_projektni.TaskScheduler scheduler = new Opos_projektni.TaskScheduler(1, new FifoScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            task1.Wait();
            lock (task1)
            {
                Assert.AreEqual(Opos_projektni.Task.TaskState.TERMINATED, task1.State);
            }
        }

        [Test]
        public void FifoSchedulingTest()
        {
            Opos_projektni.TaskScheduler scheduler = new Opos_projektni.TaskScheduler(1, new FifoScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            var task2 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            lock (task1)
            {
                Assert.AreEqual(task1.State, Opos_projektni.Task.TaskState.RUNNING);
            }

            lock (task2)
            {
                Assert.AreEqual(task2.State, Opos_projektni.Task.TaskState.SCHEDULED);
            }
        }

        [Test]
        public void PrioritySchedulingTest()
        {
            TaskScheduler scheduler = new TaskScheduler(1, new PriorityScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask(), priority: 1));
            var task2 = scheduler.Schedule(new TaskSpecification(new MyTask(), priority: 8));
            var task3 = scheduler.Schedule(new TaskSpecification(new MyTask(), priority: 10));
            task1.Wait();
            lock (task2)
            {
                Assert.AreEqual(Task.TaskState.SCHEDULED, task2.State);
            }
            lock (task3)
            {
                Assert.AreEqual(Task.TaskState.RUNNING, task3.State);
            }
        }

        [Test]
        public void StartingDateTimeTest()
        {
            TaskScheduler scheduler = new TaskScheduler(1, new FifoScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask(), startingDateTime: DateTime.Now.AddSeconds(3)));
            lock (task1)
            {
                Assert.AreEqual(Task.TaskState.SCHEDULED_NOT_STARTED, task1.State);
            }
            Thread.Sleep(4000);
            lock (task1)
            {
                Assert.AreEqual(Task.TaskState.RUNNING, task1.State);
            }
        }

        [Test]
        public void ExecutionTimeTest()
        {
            var scheduler = new TaskScheduler(1, new FifoScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask(), executionTime: 1000));
            Thread.Sleep(3000);
            lock (task1)
            {
                Assert.AreEqual(Task.TaskState.TERMINATED, task1.State);
                Assert.AreNotEqual(100, task1.Priority);
            }
        }

        [Test]
        public void DeadlineTest()
        {
            var scheduler = new TaskScheduler(1, new FifoScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask(), deadLine: DateTime.Now.AddSeconds(1)));
            Thread.Sleep(3000);
            lock (task1)
            {
                Assert.AreEqual(Task.TaskState.TERMINATED, task1.State);
                Assert.AreNotEqual(100, task1.Priority);
            }
        }

        [Test]
        public void RoundRobinTest()
        {
            var scheduler = new TaskScheduler(1, new RoundRobinScheduling(1000));
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            var task2 = scheduler.Schedule(new TaskSpecification(new MyTask()));
            task1.Wait(); task2.Wait();//verifikacija da ce zadaci obaviti u vecem broju slajsova
            lock (task1)
            {
                Assert.AreEqual(Task.TaskState.TERMINATED, task1.State);
            }
            lock (task2)
            {
                Assert.AreEqual(Task.TaskState.TERMINATED, task2.State);
            }
        }

        [Test]
        public void PriorityPreemptiveScheduling()
        {
            var scheduler = new TaskScheduler(1, new PriorityPreemptiveScheduling());
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask(), priority: 1));
            var task2 = scheduler.Schedule(new TaskSpecification(new MyTask(), priority: 10));
            Thread.Sleep(2000);
            lock (task1)
            {
                Assert.AreEqual(Task.TaskState.SWAPPED, task1.State);
            }
            lock (task2)
            {
                Assert.AreEqual(Task.TaskState.RUNNING, task2.State);
            }
        }

        [Test]
        public void ResourceTest()
        {
            var scheduler = new TaskScheduler(2, new FifoScheduling());
            scheduler.AddResource(new Resource("R1"));
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask2()));
            var task2 = scheduler.Schedule(new TaskSpecification(new MyTask2()));
            Thread.Sleep(2000);
            lock (task1)
            {
                Assert.AreEqual(Task.TaskState.RUNNING, task1.State);
            }

            lock (task2)
            {
                Assert.AreEqual(Task.TaskState.BLOCKED_WAITING_FOR_RESOURCE, task2.State);
            }
        }

        [Test]
        public void PcpTest()
        {
            var scheduler = new TaskScheduler(1, new PriorityPreemptiveScheduling());
            scheduler.AddResource(new Resource("R1"));
            var task1 = scheduler.Schedule(new TaskSpecification(new MyTask2(), priority: 2));
            var task2 = scheduler.Schedule(new TaskSpecification(new MyTask2(), priority: 20));
            Thread.Sleep(2000);
            Assert.AreEqual(Task.TaskState.RUNNING, task1.State);
            Assert.AreEqual(Task.TaskState.BLOCKED_WAITING_FOR_RESOURCE, task2.State);
            Assert.AreEqual(20, task1.Priority);
            var task3 = scheduler.Schedule(new TaskSpecification(new MyTask(), priority: 10));
            Thread.Sleep(2000);
            Assert.AreEqual(Task.TaskState.RUNNING, task1.State);
            Assert.AreEqual(Task.TaskState.BLOCKED_WAITING_FOR_RESOURCE, task2.State);
            Assert.AreEqual(20, task1.Priority);
            Assert.AreEqual(Task.TaskState.SCHEDULED, task3.State);
            task1.Wait();
            Assert.AreEqual(Task.TaskState.RUNNING, task2.State);
            Assert.AreEqual(Task.TaskState.SCHEDULED, task3.State);
        }

        [Test]
        public void DeadlockPreventionTest()
        {
            var scheduler = new TaskScheduler(1, new PriorityPreemptiveScheduling());
            var t1 = scheduler.ScheduleWithoutStarting(new TaskSpecification(new MyTask(), priority: 1));
            var t2 = scheduler.ScheduleWithoutStarting(new TaskSpecification(new MyTask(), priority: 10));

            Graph graph = new Graph();
            Assert.Throws<InvalidOperationException>(() =>
            {
                graph.TryAddEdge(t1, t2);
                graph.TryAddEdge(t2, t2);
            });
        }






    }
}