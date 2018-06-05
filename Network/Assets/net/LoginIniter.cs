using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class LoginIniter
{
    AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
    private static LoginIniter inst = new LoginIniter();
    public LoginIniter()
    {
    }
    public static LoginIniter getInst()
    {
        return inst;
    }

    [DllImport("__Internal")]
    private static extern string _GetUUID();
    AppCfg appCfg;
    public AppCfg AppCfg
    {
        set { appCfg = value; }
    }
    public void init()
    {
        int sdkcode;
        string IMEI = PlayerPrefs.GetString(CodeMap.Filed.Filed_IMEI.ToString(), "");

        if (!"".Equals(IMEI))
        {
            sdkcode = PlayerPrefs.GetInt(CodeMap.Filed.Filed_SDK_CODE.ToString(), AccountSrvCodeMap.SDKCode.DEVICE_PC);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            sdkcode = AccountSrvCodeMap.SDKCode.DEVICE_IOS;
            //1、keychain中查找imei.2、找到返回imei.3、没找到生成imei，存入keychain，返回imei
            IMEI = _GetUUID();
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            sdkcode = AccountSrvCodeMap.SDKCode.DEVICE_ANDROID;
            IMEI = SystemInfo.deviceUniqueIdentifier;
        }
        else
        {
            sdkcode = AccountSrvCodeMap.SDKCode.DEVICE_PC;
        }


        PlayerPrefs.SetString(CodeMap.Filed.Filed_IMEI.ToString(), IMEI);
        PlayerPrefs.SetInt(CodeMap.Filed.Filed_SDK_CODE.ToString(), sdkcode);

        //测试模式
        string IMEItest = appCfg.TestPlayerIMEI;
        if (IMEItest != null && !IMEItest.Equals(""))
        {
            if (!IMEI.Equals(IMEItest))//不同设备清除cookie
            {
                logReport.OnLogReport("change device,clear cookie.");
                CookieData.clear();
                PlayerPrefs.SetString(CodeMap.Filed.Filed_IMEI.ToString(), IMEItest);
                PlayerPrefs.SetInt(CodeMap.Filed.Filed_SDK_CODE.ToString(), AccountSrvCodeMap.SDKCode.DEVICE_PC);
            }
            return;
        }
    }
}