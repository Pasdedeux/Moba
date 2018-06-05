/// <summary>
/// 玩家登录cookie
/// </summary>
using System;
public class Cookie
{
    string accountid;
    string accountToken;
    int srvId;
    string srvIp;
    int srvPort;
    long tokenTimestamp;
	bool isBind;
    public string Accountid
    {
        get { return accountid; }
        set { accountid = value; }
    }
    public string AccountToken
    {
        get { return accountToken; }
        set { accountToken = value; }
    }
    public int SrvId
    {
        get { return srvId; }
        set { srvId = value; }
    }
    public long TokenTimestamp
    {
        get { return tokenTimestamp; }
        set { tokenTimestamp = value; }
    }
	public bool IsBind
	{
		get{ return isBind;}
		set{ isBind = value;}
	}
}
