using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace Support.Intellect
{
    public class JobSheduler : IJob
    {
        private static Action Action;
        public static Task MainTask { get; set; }

        public static async void Start(Action action)
        {
            Action = action;

            // construct a scheduler factory
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);

            // get a scheduler
            IScheduler sched = await factory.GetScheduler();
            await sched.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<JobSheduler>()
                .WithIdentity("myJob", "group1")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(20)
                    .RepeatForever())
            .Build();

            await sched.ScheduleJob(job, trigger);
        }

        public Task Execute(IJobExecutionContext context)
        {
            if (MainTask == null || MainTask.Status != TaskStatus.Running)
            {
                MainTask = Task.Factory.StartNew(Action);             
            }

            return MainTask;
        }
    }
}