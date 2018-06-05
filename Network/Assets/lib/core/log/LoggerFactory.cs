/// <summary>
/// 日志工厂
/// </summary>
public class LoggerFactory
{
	private static LoggerFactory inst = new LoggerFactory();
    private static LogFileWriter lgw;
    private  string logDir = "c:/unityLogs";
    private int unityLogLv = Log.LOG_LV_LOG;
	private LoggerFactory()
	{
        lgw = new LogFileWriter(logDir);
        lgw.Start();
    }
    public void OnDestroy()
    {
        lgw.Stop();
    }
	public static LoggerFactory getInst()
	{
		return inst;
	}
	public FileWriterLogReport getLogger()
	{
        FileWriterLogReport logReport = new FileWriterLogReport (lgw);
		return logReport;
	}
    public UnityLogReport getUnityLogger()
    {
        FileWriterLogReport logReport = getLogger();
        return new UnityLogReport(unityLogLv,logReport);
    }
}

