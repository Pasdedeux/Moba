public class UnityLogReport : AbstractLogReport
{
    private int logLv;
    private FileWriterLogReport fwr;
    public UnityLogReport(int logLv, FileWriterLogReport fwr)
    {
        this.logLv = logLv;
        this.fwr = fwr;
    }

    public override void OnDebugReport(string msg)
    {
        if (logLv <= Log.LOG_LV_DEBUG)
            UnityEngine.Debug.Log(msg);
        fwr.OnDebugReport(msg);
    }

    public override void OnErrReport(string msg)
    {
        if (logLv <= Log.LOG_LV_ERR)
            UnityEngine.Debug.LogError(msg);
        fwr.OnErrReport(msg);
    }

    public override void OnLogReport(string msg)
    {
        if (logLv <= Log.LOG_LV_LOG)
            UnityEngine.Debug.Log(msg);
        fwr.OnLogReport(msg);
    }

    public override void OnWarningReport(string msg)
    {
        if (logLv <= Log.LOG_LV_WARN)
            UnityEngine.Debug.LogWarning(msg);
        fwr.OnWarningReport(msg);
    }
}