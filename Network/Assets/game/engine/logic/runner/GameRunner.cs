using System;
using System.Threading;
namespace common.game.engine.logic.runner
{
    /// <summary>
    /// 引擎逻辑线程驱动类
    /// </summary>
    public class GameRunner 
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        private bool isWorking = false;
        IRunable run;
        public GameRunner(IRunable run)
        {
            this.run = run;
        }
        Thread exec;
        public void Start()
        {
            isWorking = true;
            exec.Start();
        }
        public void Stop()
        {
            isWorking = false;
            if (exec.IsAlive)
                exec.Join();
            logReport.OnLogReport("Runner stoped ...");
        }
        public void Run()
        {
            DateTime runtime = DateTime.Now;
            while (isWorking)
            {
                runtime = DateTime.Now;
                run.RunInLogic(runtime);
            }
        }
    }
}