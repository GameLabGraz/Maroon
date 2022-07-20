using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class TaskRunner: MonoBehaviour
    {
        [SerializeField] private int time_budget_ms = 500;

        private readonly Stopwatch _stop_watch = new Stopwatch();
        
        private readonly Queue<QueueElementBase> queue = new Queue<QueueElementBase>(); 
        
        private static TaskRunner _instance;

        public static TaskRunner Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindObjectOfType<TaskRunner>();
                return _instance;
            }
        }

        public void AddTask<T>(Action<T> a, T o)
        {
            queue.Enqueue(new QueueElement<T>(o, a));
        }

        public void AddTask(Action a) => queue.Enqueue(new QueueElement(a));

        public void ClearTasks() => queue.Clear();
        
        private void Update()
        {
            _stop_watch.Restart();

            while (true)
            {
                if(queue.Count == 0)
                    break;
                var element = queue.Dequeue();
                element.Invoke();
                
                if(_stop_watch.ElapsedMilliseconds > time_budget_ms)
                    break;
            }
        }

        private abstract class QueueElementBase
        {
            public abstract void Invoke();
        }

        private class QueueElement : QueueElementBase
        {
            private readonly Action _action;

            public QueueElement(Action action)
            {
                _action = action;
            }

            public override void Invoke()
            {
                _action.Invoke();
            }
        }

        private class QueueElement<T> : QueueElementBase
        {
            private readonly T param;
            private readonly Action<T> _action;

            public QueueElement(T param, Action<T> action)
            {
                this.param = param;
                _action = action;
            }

            public override void Invoke() => _action.Invoke(param);
        }
    }
}