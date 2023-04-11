using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    public class Graph
    {
        private Dictionary<Task, Task> transition = new Dictionary<Task, Task>();

        public void TryAddEdge(Task t1, Task t2)
        {
            transition.Add(t1, t2);
            if (CycleDecetion(t1))
            {
                transition.Remove(t1);
                throw new InvalidOperationException("Dead lock prevention");
            }
        }

        public void RemoveEdge(Task t1)
        {
            transition.Remove(t1);
        }

        private bool CycleDecetion(Task t)
        {
            HashSet<Task> visited = new HashSet<Task>();
            Stack<Task> cycleStack = new Stack<Task>();
            return dfs(visited, cycleStack, t);
        }

        private bool dfs(HashSet<Task> visited, Stack<Task> cycleStack, Task node)
        {
            visited.Add(node);
            cycleStack.Push(node);
            Task? nextNode = null;
            bool status = transition.TryGetValue(node, out nextNode);
            if (status)
            {

                if (!visited.Contains(nextNode) && dfs(visited, cycleStack, nextNode))
                    return true;
                else if (cycleStack.Contains(nextNode))
                    return true;
            }
            cycleStack.Pop();
            return false;

        }
    }
}
