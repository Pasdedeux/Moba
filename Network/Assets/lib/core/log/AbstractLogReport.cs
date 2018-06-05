/// <summary>
/// 日志报告接口
/// </summary>
public abstract class AbstractLogReport
{
    public abstract void OnDebugReport(string msg);
	public abstract void OnLogReport(string msg);
	public abstract void OnWarningReport(string msg);
	public abstract void OnErrReport(string msg);
}