using System;
using UnityEngine;

///<summary>
///玩家cookie操作，用于登录
/// </summary>
class CookieData
{
	private static AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
	/// <summary>
	/// 保存cookie
	/// </summary>
	/// <param name="cookie">Cookie.</param>
    public static void Save(Cookie cookie)
    {
		PlayerPrefs.SetString(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_ID.ToString(), cookie.Accountid.ToString());
		PlayerPrefs.SetString(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_TOKEN.ToString(), cookie.AccountToken);
		PlayerPrefs.SetInt(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_SRV_ID.ToString(), cookie.SrvId);
		PlayerPrefs.SetString(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_TOKEN_TIMESTAMP.ToString(), cookie.TokenTimestamp.ToString());
		PlayerPrefs.SetInt (CodeMap.Filed.FIELED_COOKIE_ACCOUNT_IS_BIND.ToString (), cookie.IsBind?1:0);
		string loginfo = "save cookie to db(accountid:" + PlayerPrefs.GetString(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_ID.ToString(), "") + ","
			+ "accountToken:" + PlayerPrefs.GetString(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_TOKEN.ToString(), "") + ","
			+ "srvid:" + PlayerPrefs.GetInt(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_SRV_ID.ToString(), 0) + ","
			+ "tokenTimestamp:" + PlayerPrefs.GetString(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_TOKEN_TIMESTAMP.ToString(), "") + ","
			+ "isBind:" + PlayerPrefs.GetInt(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_IS_BIND.ToString(), 0) + ")";
		logReport.OnDebugReport(loginfo);
    }
	/// <summary>
	/// 加载cookie
	/// </summary>
    public static Cookie Load()
    {
        Cookie cookie = new Cookie();
		cookie.Accountid = PlayerPrefs.GetString(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_ID.ToString(), "0");
		cookie.AccountToken = PlayerPrefs.GetString(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_TOKEN.ToString(), "");
		cookie.SrvId = PlayerPrefs.GetInt(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_SRV_ID.ToString(), 0);
		string tokenTimestampString = PlayerPrefs.GetString(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_TOKEN_TIMESTAMP.ToString(), "");
        if (tokenTimestampString != "")
            cookie.TokenTimestamp = long.Parse(tokenTimestampString);
		int isBindValue = PlayerPrefs.GetInt(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_IS_BIND.ToString(), 0);
		cookie.IsBind = isBindValue == 0 ? false : true;
        if (string.IsNullOrEmpty(cookie.Accountid) || cookie.AccountToken == "" || cookie.SrvId == 0 || tokenTimestampString == "")
            return null; 
        return cookie;
    }
	/// <summary>
	/// 清除cookie
	/// </summary>
    public static void clear()
    {
		PlayerPrefs.DeleteKey(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_ID.ToString());
		PlayerPrefs.DeleteKey(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_TOKEN.ToString());
		PlayerPrefs.DeleteKey(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_SRV_ID.ToString());
		PlayerPrefs.DeleteKey(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_TOKEN_TIMESTAMP.ToString());
		PlayerPrefs.DeleteKey (CodeMap.Filed.FIELED_COOKIE_ACCOUNT_IS_BIND.ToString ());
    }
	/// <summary>
	/// 获取该账号是否被绑定
	/// </summary>
	/// <returns><c>true</c>, if bind was ised, <c>false</c> otherwise.</returns>
	public static bool isBind()
	{
		int isBindValue = PlayerPrefs.GetInt(CodeMap.Filed.FIELED_COOKIE_ACCOUNT_IS_BIND.ToString(), 0);
		return isBindValue == 0 ? false : true;
	}
}
