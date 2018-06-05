/// <summary>
/// file log report.
/// </summary>
public class FileWriterLogReport : AbstractLogReport
{
    private LogFileWriter logFileWriter;
    public FileWriterLogReport(LogFileWriter logFileWriter)
    {
        this.logFileWriter = logFileWriter;
    }
    override public void OnDebugReport(string msg)
    {
        logFileWriter.WriteLog(msg, Log.LOG_LV_DEBUG);
    }
	override public void OnErrReport(string msg)
    {
        logFileWriter.WriteLog(msg, Log.LOG_LV_ERR);
    }
	override public void OnLogReport(string msg)
    {
        logFileWriter.WriteLog(msg, Log.LOG_LV_LOG);
    }
	override public void OnWarningReport(string msg)
    {
        logFileWriter.WriteLog(msg, Log.LOG_LV_WARN);
    }
}