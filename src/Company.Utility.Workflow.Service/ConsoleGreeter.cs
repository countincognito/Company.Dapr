﻿using Elsa.Scheduling.Activities;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Attributes;

namespace Company.Utility.Workflow.Service
{
    [Activity("Demo", "Writes a greeting to the console window... in a delayed fashion.")]
    public class ConsoleGreeter : Composite
    {
        public ConsoleGreeter()
        {
            Root = new Sequence
            {
                Activities =
            {
                new WriteLine("Hello...") { Id = "WriteLine1" },
                new Delay
                {
                    Id = "Delay1",
                    TimeSpan = new(TimeSpan.FromSeconds(1))
                },
                new WriteLine("world!") { Id = "WriteLine2" },
            }
            };
        }
    }
}
