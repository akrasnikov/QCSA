using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ECAS.Microservice.Recognizer.Agent
{
    static class Program
    {
        private static bool Debug = true;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            if (Debug == true)
            {
                var rc = HostFactory.Run(x =>
                {
                    x.Service<Service>(s =>
                    {
                        s.ConstructUsing(name => new Service());
                        s.WhenStarted(tc => tc.StartService());
                        s.WhenStopped(tc => tc.StopService());

                    });
                    x.RunAsLocalSystem();
                    x.SetDescription("Recognizer Description");
                    x.SetDisplayName("Recognizer DisplayName");
                    x.SetServiceName("Recognizer ServiceName");
                });
                var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
                Environment.ExitCode = exitCode;
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new Service()
                };
                ServiceBase.Run(ServicesToRun);
            }


        }
        }
    }
