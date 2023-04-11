# Task-scheduler-parallel-processing-of-multimedia-data-and-FS-driver
The project consists of implementing a programmatic API for a task scheduler and a logically separated GUI application that uses this API. Also, a type of task that performs parallel processing of multimedia data has been defined.

The basic functionality of the task scheduler is adding tasks to the scheduler queue, scheduling tasks while already scheduled tasks are being executed, starting, pausing, resuming, and stopping tasks, waiting for a task to complete, and specifying the scheduling algorithm.

It is possible to perform priority scheduling with preemption, as well as scheduling based on time slices, where higher priority tasks are given more time slices to execute. Cooperative scheduling mechanisms simulate preemption.

Optionally, a task can be specified with a start date and time, allowed total execution time, and a deadline by which the task must be completed or terminated. Also, for each task, the allowed level of parallelism can be specified (e.g., allowed number of utilized processor cores).

In the concrete implementation of the project, an algorithm for enhancing audio recordings is used. A task of the defined type can be scheduled for execution using the scheduler, and it is also possible to simultaneously process a larger amount of input files.

This project achieves faster parallel processing of multimedia files, thereby achieving more efficient use of processor resources.

The task scheduler should enable resource handling, detection, and resolution of deadlock situations and priority inversion. Additionally, the application should have a non-blocking graphical user interface for task scheduling.

The task scheduler allows locking of resources during execution, where resources are identified by a unique string. The priority inversion problem is solved using PIP (Priority Inheritance Protocol) or PCP (Priority Ceiling Protocol) algorithms. Also, mechanisms for detecting and resolving deadlock situations are implemented.

The application is constructed in such a way that it can be easily expanded with new types of tasks. The application user can specify the scheduling method and all properties of the created task, including allowed execution time and deadlines.

The graphical user interface of the application allows viewing of tasks in progress, along with reporting task progress through a progress bar. The user can create, start, pause, resume, stop, and remove completed or stopped tasks.

Serialization and deserialization of tasks are enabled, including an array of input file paths and the path where output files should be generated. The internal state of the application can also be serialized (number of tasks and processing state). In the event of application termination, tasks can be resumed from the point where they were previously stopped.

The file system driver is implemented in user space. The file system uses the scheduler API and enables processing of multimedia files. The file system is implemented in Dokan.net. 
![image](https://user-images.githubusercontent.com/93669392/231273338-eb0044e3-e902-43f2-9909-3b363ff153d6.png)
![image](https://user-images.githubusercontent.com/93669392/231273435-261cf5a5-ff52-4c0e-8c15-8b1979b332a3.png)
![image](https://user-images.githubusercontent.com/93669392/231273894-6cdf95c1-3145-4d19-b1da-36a6622d61b0.png)


