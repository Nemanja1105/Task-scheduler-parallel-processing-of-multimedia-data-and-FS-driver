using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Opos_projektni
{
    [Serializable]
    public class TaskSpecification
    {
        private String name;
        private IAction action;
        private int priority;
        [NonSerialized]
        private DateTime? startingDateTime;
        [NonSerialized]
        private DateTime? deadLine;
        private int? executionTime;
        private int maxDegreeOfParallelism;

        public IAction Action
        {
            get { return this.action; }
            set { this.action = value; }
        }
        public int Priority
        {
            get { return this.priority; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Prioritet mora da bude nenegativan broj");
                this.priority = value;
            }
        }

        public int MaxDegreeOfParallelism
        {
            get { return this.maxDegreeOfParallelism; }
            set { this.maxDegreeOfParallelism = value; }
        }

        [JsonIgnore]
        public DateTime? StartingDateTime
        {
            get { return this.startingDateTime; }
            set { this.startingDateTime = value; }
        }

        [JsonIgnore]
        public DateTime? DeadLine
        {
            get { return this.deadLine; }
            set { this.deadLine = value; }
        }

        public int? ExecutionTime
        {
            get { return this.executionTime; }
            set { this.executionTime = value; }
        }

        public String Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public TaskSpecification()
        {

        }


        public TaskSpecification(IAction action, String name = "", DateTime? startingDateTime = null, DateTime? deadLine = null, int? executionTime = null, int maxDegreeOfParallelism = 1, int priority = 0)
        {
            this.Action = action;
            this.Priority = priority;
            this.StartingDateTime = startingDateTime;
            this.DeadLine = deadLine;
            this.ExecutionTime = executionTime;
            this.MaxDegreeOfParallelism = maxDegreeOfParallelism;
            this.name = name;
        }




    }
}
