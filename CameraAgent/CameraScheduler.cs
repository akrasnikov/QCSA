using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraAgent
{
    public class CameraScheduler
    {
       
        public void Stop() { }
        public async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail jobCamera = JobBuilder.Create<CameraRecipient>().Build();
            IJobDetail jobRelay = JobBuilder.Create<RelayFactory>().Build();

            ITrigger triggerCamera = TriggerBuilder.Create()  
                .WithIdentity("Trigger->Camera", "Group->Camera")    
                .StartNow()                           
                .WithSimpleSchedule(x => x            
                    .WithIntervalInSeconds(30)          
                    .RepeatForever())                 
                .Build();

            ITrigger triggerRelay = TriggerBuilder.Create()
           .WithIdentity("Trigger->Relay", "Group->Relay")
           .StartNow()
           .WithSimpleSchedule(x => x
               .WithIntervalInSeconds(30)
               .RepeatForever())
           .Build();

            await scheduler.ScheduleJob(jobCamera, triggerCamera);
            await scheduler.ScheduleJob(jobRelay, triggerRelay);
        }
    }
}
