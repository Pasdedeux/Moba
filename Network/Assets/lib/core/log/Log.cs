using System;
public class Log
{
    /// <summary>
	/// 日志级别-debug
	/// </summary>
	public const int LOG_LV_DEBUG = 1;
    /// <summary>
    /// 日志级别-log
    /// </summary>
    public const int LOG_LV_LOG = 2;
    /// <summary>
    /// 日志级别-警告
    /// </summary>
    public const int LOG_LV_WARN = 3;
    /// <summary>
    /// 日志级别-错误
    /// </summary>
    public const int LOG_LV_ERR = 4;
    private int lv;
    public int Lv { get { return lv; } }
    private string lvinfo;
    public string Lvinfo { get { return lvinfo; } }
    private string msg;
    public string Msg { get { return msg; } }
    private DateTime time; 
    public DateTime Time { get { return time; } }
    public string info;
    public string Info { get { return info; } }
    public Log(string msg,int lv)
    {
        this.msg = msg;
        time = DateTime.Now;
        this.lv = lv;
        lvinfo = getLogLvInfo();
        info = time + " " + lvinfo + " " + msg;
    }
    public string getLogLvInfo()
    {
        switch (lv)
        {
            case LOG_LV_DEBUG:
                return "debug";
            case LOG_LV_LOG:
                return "log";
            case LOG_LV_WARN:
                return "waring";
            case LOG_LV_ERR:
                return "erro";
            default:
                return "";
        }
    }
}
