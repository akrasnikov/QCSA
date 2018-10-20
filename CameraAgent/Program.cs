using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace CameraAgent
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new Service1()
            //};
            //ServiceBase.Run(ServicesToRun);

            var rc = HostFactory.Run(x =>                                  
            {
                x.Service<CameraScheduler>(s =>                                   
                {
                    s.ConstructUsing(name => new CameraScheduler());                
                    s.WhenStarted(tc => tc.Start());                        
                    s.WhenStopped(tc => tc.Stop());                          
                });
                x.RunAsLocalSystem();                                      

                x.SetDescription("Sample Topshelf Host");                   
                x.SetDisplayName("Topshelf");                                 
                x.SetServiceName("Topshelf");                                 
            });                                                            

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());  
            Environment.ExitCode = exitCode;
        }
    }
}
