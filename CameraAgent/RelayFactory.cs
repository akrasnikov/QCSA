using System;
using System.Threading.Tasks;
using Quartz;

namespace CameraAgent
{
    internal class RelayFactory : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}