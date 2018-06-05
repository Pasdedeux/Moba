using UnityEngine;
using common.net.socket;
using System.Collections.Generic;
using common.net.http;
using common.core;
using common.net.socket.codec;
using System;
using System.Runtime.InteropServices;
using common.core.codec;
using common.net.socket.session;
using common.net.socket.acceptor;
public delegate void OnClosed();
public delegate void BeginReconnect();
public delegate void ConnectSuccess();
public class NetServiceCmdCallbackWraper:CmdCallbackWraper
{
    private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
    private NetService netService;
	public NetServiceCmdCallbackWraper(NetService netService,CmdCallback cmdCallback):base(cmdCallback)
	{
		this.netService = netService;
	}
	public override void DoInWraper(Msg msg)
	{
        var _rsCode = msg.GetParam(BaseCodeMap.BaseParam.RS_CODE);
        if(_rsCode != null)
        {
            short rsCode = short.Parse(_rsCode.ToString());
            if (rsCode == BaseCodeMap.BaseRsCode.TIME_OUT)
            {
                logReport.OnWarningReport("cmd:" + msg.Cmd+" timeOut," + DateTime.Now);
                netService.Close();
            }   
        }
	}
    public override bool IsCallback()
    {
        return CmdCallback==null?false:true;
    }
}
public class ConnectionProcessorProxy:ConnectedProcessor
{
    private ConnectedProcessor connectedProcessor;
    private int clientCode;
    private Client client;
    private Queue<KeyValuePair<int, Client>> removeClients;
    public ConnectionProcessorProxy(ConnectedProcessor connectedProcessor, int clientCode,Client client, Queue<KeyValuePair<int, Client>> removeClients) 
    {
        this.connectedProcessor = connectedProcessor;
        this.clientCode = clientCode;
        this.client = client;
        this.removeClients = removeClients;
    }
    public void OnConnected(Boolean isConnected,Client client) 
    {
        connectedProcessor.OnConnected(isConnected,client);
        if (!isConnected)
            removeClients.Enqueue(new KeyValuePair<int, Client>(this.clientCode, this.client));
    }
    public void OnOffline()
    {
        connectedProcessor.OnOffline();
    }
}
public class GameSrvConnectionProcessor : ConnectedProcessor
{
    private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
    private Client client;
    private BeginReconnect beginReconnect;
    private OnClosed onClosed;
    public GameSrvConnectionProcessor(Client client,BeginReconnect beginReconnect,OnClosed onClosed)
    {
        this.client = client;
        this.beginReconnect = beginReconnect;
        this.onClosed = onClosed;
    }
    public void OnConnected(bool isConnected, Client client)
    {
        if (isConnected)
        {
            logReport.OnLogReport("GameSrv connect fail");
            onClosed();
        }
        else
            logReport.OnLogReport("GameSrv connect success");

    }
    public void OnOffline()
    {
        beginReconnect();
        client.DoConnect();
    }
}
public class NetService{
    private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
    private const int DEFAULT_TEST_LOGIN_TIME = 3;
    private static int testLoginTimes = DEFAULT_TEST_LOGIN_TIME;
    /// <summary>
    /// 验证码类型_注册
    /// </summary>
    public const int VERIFY_CODE_TYPE_REGIST = 1;
    /// <summary>
    /// 验证码类型_密码找回
    /// </summary>
    public const int VERIFY_CODE_TYPE_FIND_PWD = 2;
    private NetService() { }
    private ISocketDecoder decoder;
    private ISocketEncoder encoder;
    private IDecoder httpDecoder;
    private IEncoder httpEncoder;
    private Msg heartBeatPackage;
    private string accountid;
    private string accountToken;
    private int srvId;
    private string srvIp;
    private int srvPort;
    private long tokenTimestamp;
    private string playerid;
    public string Playerid{get { return playerid; }}
    private AppCfg appCfg;
    public AppCfg AppCfg{set { appCfg = value; }}
    Queue<KeyValuePair<int, Client>> addClients = new Queue<KeyValuePair<int, Client>>();
    Queue<KeyValuePair<int, Client>> removeClients = new Queue<KeyValuePair<int, Client>>();
	public delegate void ConnectInternetResultHandler(bool rs,Msg msg);
    private ConnectInternetResultHandler ConnectSocketComplete;
    private ConnectInternetResultHandler accountSrvComplete;
    private OnMsgPush onMsgPush;
    private System.Collections.Generic.Dictionary<int, Client> otherClients = new Dictionary<int, Client>();
    private static Client gameSrvClient;
    public BeginReconnect GameSrvBeginReconnect { get; set; }
    public ConnectSuccess GameSrvConnectSuccess { get; set; }
    public Client GameSrvClient { get { return gameSrvClient; } }
    private Acceptor acceptor;
    private System.Collections.Generic.Dictionary<string, HttpClient> httpClients = new Dictionary<string, HttpClient>();
    private bool isLogined = false;
    public bool IsLogined { get { return isLogined; } }
    public OnMsgPush OnMsgPush
	{
		set{ onMsgPush = value;gameSrvClient.OnMsgPush = onMsgPush;}
	}
	private OnClosed onClosed;
	public OnClosed OnClosed
	{
		set{ onClosed = value;}
	}
	private static NetService inst = new NetService();
    public static NetService getInstance()
    {
        return inst;
    }
    public void Start()
    {
        acceptor = new Acceptor();
        acceptor.Start();
        decoder = new DefaultSocketDecoder();
		encoder = new DefaultSocketEncoder();
        httpDecoder = new DefaultDecoder();
        httpEncoder = new DefaultEncoder();
        heartBeatPackage = new Msg(BaseCodeMap.BaseCmd.CMD_HEART_BEAT);
        SessionConfig sessionConfig = new SessionConfig(10, 10);
        gameSrvClient = getClient(null, onMsgPush, sessionConfig);
    }
    public void OnDestroy()
    {
        acceptor.Stop();
    }
    public void Pause()
    {
        if(acceptor != null)
            acceptor.Pause();
    }
    public void Active()
    {
        if (acceptor != null)
            acceptor.Active();
    }
	public void Update () {
        DateTime now = DateTime.Now;
        if (gameSrvClient == null)
            return;
        gameSrvClient.Update(now);
        while(addClients.Count!=0)
        {
            KeyValuePair<int,Client> o = addClients.Dequeue();
            int key = o.Key;
            Client v = o.Value;
            otherClients[key] = v;
        }
        while(removeClients.Count!=0) 
        {
            KeyValuePair<int, Client> o = removeClients.Dequeue();
            int key = o.Key;
            Client v = o.Value;
            if (otherClients.ContainsKey(key))
                otherClients.Remove(key);
        }
        foreach (var v in otherClients)
        {
            Client client = v.Value;
            client.Update(now);
        }
		foreach (var v in httpClients) 
		{
			HttpClient httpClient = v.Value;
			httpClient.Update();
		}
    }
    public void OnPackagesTimeOut(Msg[] msgs)
    {
        foreach (Msg msg in msgs)
            msg.AddParam(BaseCodeMap.BaseParam.RS_CODE, BaseCodeMap.BaseRsCode.TIME_OUT);
    }
	internal void Close()
    {
        isLogined = false;
        gameSrvClient.Close();
        foreach (var v in otherClients)
        {
            Client client = v.Value;
            client.Close();
        }
    }
	public void close(int clientCode)
	{
        if (!otherClients.ContainsKey(clientCode))
            return;
        Client client = otherClients [clientCode];
		client.Close ();
        removeClients.Enqueue(new KeyValuePair<int, Client>(clientCode,client));
    }
    public Client getClient(int clientCode)
    {
        if (otherClients.ContainsKey(clientCode))
            return otherClients[clientCode];
        else
            return null;
    }
	public Client getClient(Msg loginPackage,OnMsgPush onMsgPush,SessionConfig sessionCfg)
	{
        Client client = ClientFactory.getInstance ().get (sessionCfg,encoder,decoder, heartBeatPackage);
		client.OnMsgPush = onMsgPush;
        client.OnLoginReturn = OnSocketLoginReturn;
        client.Acceptor = acceptor;
		return client;
	}
    public void Connect(int clientCode, String ip, int port, Client client,ConnectedProcessor connectedProcessor) 
    {
        if (otherClients.ContainsKey(clientCode))
        {
            Client oldClient = otherClients[clientCode];
            oldClient.Close();
        }
        AddClient(clientCode, client);
        client.Connect(ip, port, new ConnectionProcessorProxy(connectedProcessor, clientCode, client, this.removeClients));
    }
    public void BgConnect(string ip, int port, Client client, ConnectedProcessor connectedProcessor)
    {
        client.Connect(ip, port, connectedProcessor);
    }
    public void AddClient(int clientCode, Client client)
    {
        this.addClients.Enqueue(new KeyValuePair<int,Client>(clientCode,client));
    }
    public bool Check()
    {
        return gameSrvClient.IsOpen();
    }
    public bool OtherClientCheck(int clientKey)
    {
        Client client = otherClients[clientKey];
        return client.IsOpen();
    }
    public bool sendMessage(Msg msg, CmdCallback callback)
    {
		bool rs = gameSrvClient.Send(msg, new NetServiceCmdCallbackWraper(this,callback));
        return rs;
    }
    public bool sendMessage(Msg msg) 
    {
        bool rs = sendMessage(msg, null);
        return rs;
    }
    public bool sendMessage(int clientKey,Msg msg, CmdCallback callback)
    {
        if (!otherClients.ContainsKey(clientKey))
        {
            logReport.OnWarningReport("send:" + msg.ToString() + " fail,invalide clientKey:" + clientKey);
            return false;
        }
        bool rs = otherClients[clientKey].Send(msg, new NetServiceCmdCallbackWraper(this, callback));
        logReport.OnLogReport("send:" + msg.ToString() + ",time:" + gameSrvClient.Runtime);
        return rs;
    }
    public bool sendMessage(int clientKey,Msg msg)
    {
        bool rs = sendMessage(clientKey, msg,null);
        return rs;
    }
    public void login(ConnectInternetResultHandler SocketComplete, ConnectInternetResultHandler accountSrvComplete)
    {
        this.ConnectSocketComplete = SocketComplete;
        this.accountSrvComplete = accountSrvComplete;
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_LOGIN);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SRV_VERSION, appCfg.Version);
        testLoginTimes = DEFAULT_TEST_LOGIN_TIME;
        Cookie cookie = CookieData.Load();
        LoginWithCookie(cookie, msg);
    }
	public void accountAuthLogin(String userName,String pwd,ConnectInternetResultHandler SocketComplete, ConnectInternetResultHandler accountSrvComplete)
	{
		CookieData.clear ();
		PlayerPrefs.SetString(CodeMap.Filed.Filed_IMEI.ToString(), userName);
		PlayerPrefs.SetInt(CodeMap.Filed.Filed_SDK_CODE.ToString(), AccountSrvCodeMap.SDKCode.ACCOUNT_AUTH);
		PlayerPrefs.SetString (CodeMap.Filed.FIELDE_EXT1.ToString(), pwd);
		login (SocketComplete, accountSrvComplete);
	}
    public void registAuthAccount(String userName,String pwd,CmdCallback onRegistReturn)
	{
		Msg msg = new Msg (AccountSrvCodeMap.Cmd.CMD_ACCOUNT_REGIST);
		msg.AddParam (AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID,userName);
		msg.AddParam (AccountSrvCodeMap.Param.ACCOUNT_PWD, pwd);
		sendHttpMessage( appCfg.LoginUrl , msg , onRegistReturn );
	}
    public void RegistAuthAccount(string userName, string pwd,int sdkCode,string verificationCode, CmdCallback onRegistReturn)
    {
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_REGIST);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, userName);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_PWD, pwd);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SDK_CODE,sdkCode);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_VERIFICATION_CODE, verificationCode);
        sendHttpMessage(appCfg.LoginUrl, msg, onRegistReturn);
    }
    [DllImport("__Internal")]
    private static extern string _GetUUID();
    private void LoginWithCookie(Cookie cookie, Msg loginMsg)
    {
        if (cookie == null)
        {
            logReport.OnDebugReport("cookie is null");
			string IMEI = PlayerPrefs.GetString( CodeMap.Filed.Filed_IMEI.ToString() , "" );
            int sdkCode = PlayerPrefs.GetInt( CodeMap.Filed.Filed_SDK_CODE.ToString() , -1 );
            string pwd = PlayerPrefs.GetString( CodeMap.Filed.FIELDE_EXT1.ToString() , "" );
			if ( "".Equals( IMEI ) || -1 == sdkCode )
			{
				logReport.OnWarningReport( "IMEI and sdkid error!" );
				return;
			}
			loginMsg.AddParam( AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, IMEI );//设备号
			loginMsg.AddParam( AccountSrvCodeMap.Param.ACCOUNT_SDK_CODE, sdkCode );
			if (!"".Equals (pwd))
				loginMsg.AddParam (AccountSrvCodeMap.Param.ACCOUNT_PWD, pwd);
			sendHttpMessage (appCfg.LoginUrl, loginMsg, OnhttpLoginReturn);
        }
        else
        {
			string accountid = cookie.Accountid;
			string token = cookie.AccountToken;
			int srvid = cookie.SrvId;
			string tokentimestamp = cookie.TokenTimestamp.ToString();
			string loginfo = "save cookie info(accountid:" + accountid + ","
				+ "accountToken:" + token + ","
				+ "srvid:" + srvid + ","
				+ "tokenTimestamp:" + tokentimestamp + ")";
            logReport.OnLogReport( loginfo );
			Msg msg = new Msg( AccountSrvCodeMap.Cmd.CMD_ACCOUNT_GET_SERVER_INFO);
            msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_SRV_VERSION, appCfg.Version);
            msg.AddParam( AccountSrvCodeMap.Param.ACCOUNT_SRV_ID , cookie.SrvId );

			sendHttpMessage( appCfg.LoginUrl , msg , srvInfoReturn );
        }
    }
	public void srvInfoReturn( Msg msg )
	{
		int rscode = ( int ) msg.GetParam( AccountSrvCodeMap.Param.RS_CODE );
		if ( rscode == AccountSrvCodeMap.RsCode.SUCCESS )
		{
			srvIp = ( string ) msg.GetParam( AccountSrvCodeMap.Param.ACCOUNT_SRV_IP );
			srvPort = ( int ) msg.GetParam( AccountSrvCodeMap.Param.ACCOUNT_SRV_PORT );
			Cookie cookie = CookieData.Load();
			OpenSoketConnect( cookie );
		}
		else
			OnSocketLoginReturn( msg );
	}
    public void ReqVerificationCode(string mobilePhone,int verifyType,CmdCallback callback)
    {
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_REQ_VERIFICATION_CODE);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, mobilePhone);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_VERIFY_CODE_TYPE, verifyType);
        sendHttpMessage(appCfg.LoginUrl, msg, callback);
    }
    public void VerifiyCode(string mobilePhone, string verificationCode, CmdCallback callback)
    {
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_VERIFY_CODE);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, mobilePhone);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_VERIFICATION_CODE, verificationCode);
        sendHttpMessage(appCfg.LoginUrl, msg, callback);
    }
    public void UpdatePwd(string mobilePhone, string verificationCode,string pwd, CmdCallback callback)
    {
        Msg msg = new Msg(AccountSrvCodeMap.Cmd.CMD_ACCOUNT_UPDATE_PWD);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_OPEN_ID, mobilePhone);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_VERIFICATION_CODE, verificationCode);
        msg.AddParam(AccountSrvCodeMap.Param.ACCOUNT_PWD, pwd);
        sendHttpMessage(appCfg.LoginUrl, msg, callback);
    }
    public void sendHttpMessage( string url , Msg msg , CmdCallback callback)
	{
		HttpClient httpClient = null;
		if(httpClients.ContainsKey(url))
			httpClient = httpClients [url];
		if (httpClient==null) 
		{
			httpClient = new HttpClient(url);
			httpClient.Decoder = httpDecoder;
			httpClient.Encoder = httpEncoder;
			httpClients.Add (url, httpClient);
		}
		httpClient.Send(msg, callback);
	}
    public void OnhttpLoginReturn(Msg msg)
    {
        int rsCode = (int)msg.GetParam(BaseCodeMap.BaseParam.RS_CODE);
        switch (rsCode)
        {
            case BaseCodeMap.BaseRsCode.SUCCESS:
				srvIp = ( string ) msg.GetParam( AccountSrvCodeMap.Param.ACCOUNT_SRV_IP );
			    srvPort = ( int ) msg.GetParam( AccountSrvCodeMap.Param.ACCOUNT_SRV_PORT );
				logReport.OnLogReport("loginHttpReqSucess->ip:"+srvIp+",port:"+srvPort);
                if(accountSrvComplete != null)
                    accountSrvComplete(true, msg);
                OpenSoketConnect(msg);
                break;
            default:
                logReport.OnWarningReport("loginHttpReqFailAndReconnect->rscode:" + rsCode);
                if (accountSrvComplete != null)
                    accountSrvComplete(false, msg);
                OnSocketLoginReturn(msg);
                break;
        }
    }
    public void OpenSoketConnect(Msg msg)//openSoket
    {
        Cookie cookie = new Cookie();
		cookie.Accountid = (string)msg.GetParam(AccountSrvCodeMap.Param.ACCOUNT_UID);
        cookie.AccountToken = (string)msg.GetParam(AccountSrvCodeMap.Param.ACCOUNT_TOKEN);
        cookie.SrvId = (int)msg.GetParam(AccountSrvCodeMap.Param.ACCOUNT_SRV_ID);
        cookie.TokenTimestamp = (long)msg.GetParam(AccountSrvCodeMap.Param.ACCOUNT_TOKEN_TIMESTAMP);
		cookie.IsBind = (bool)msg.GetParam (AccountSrvCodeMap.Param.ACCOUNT_IS_BIND);
        CookieData.Save(cookie);

        OpenSoketConnect(cookie);
    }
    public void OpenSoketConnect(Cookie cookie)//openSoket
    {
        accountid = cookie.Accountid;
        accountToken = cookie.AccountToken;
        srvId = cookie.SrvId;
        tokenTimestamp = cookie.TokenTimestamp;
        Open();
    }
    public void Open()
    {
        Msg loginPackage = new Msg(GameSrvCodeMap.Cmd.CMD_USER_LOG_IN);
        loginPackage.AddParam(GameSrvCodeMap.Param.UID, accountid);
        loginPackage.AddParam(GameSrvCodeMap.Param.TOKEN, accountToken);
        loginPackage.AddParam(GameSrvCodeMap.Param.SRVID, srvId);
        loginPackage.AddParam(GameSrvCodeMap.Param.TOKEN_TIMESTAMP, tokenTimestamp);
        loginPackage.AddParam(GameSrvCodeMap.Param.VERSION, appCfg.Version);
        gameSrvClient.LoginPackage = loginPackage;
        gameSrvClient.OnLoginReturn = OnSocketLoginReturn;
        gameSrvClient.Connect(srvIp, srvPort,new GameSrvConnectionProcessor(gameSrvClient, GameSrvBeginReconnect, onClosed));
    }
    private void OnSocketLoginReturn(Msg msg)
    {
        int rsCode = (int)msg.GetParam(BaseCodeMap.BaseParam.RS_CODE);
        switch (rsCode)
        {
			case BaseCodeMap.BaseRsCode.SUCCESS:
			    playerid = (string)msg.GetParam (GameSrvCodeMap.Param.PLAYER_ID);
			    testLoginTimes = DEFAULT_TEST_LOGIN_TIME;
                isLogined = true;
                GameSrvConnectSuccess();
				logReport.OnLogReport ("loginSuccess->pid:"+playerid+",uid:"+msg.GetParam(GameSrvCodeMap.Param.UID)+",srvid:"+msg.GetParam(GameSrvCodeMap.Param.SRVID));
                break;
			case GameSrvCodeMap.RsCode.ERRO_CODE_FORCE_UPDATE_VERSION:
				logReport.OnWarningReport ("versionNeedUpdate");
                Close();
				break;
            case GameSrvCodeMap.RsCode.ERRO_CODE_TOKEN_EXPIRED:
                logReport.OnWarningReport("loginTokenExpiredAndRelogin.");
                testLoginTimes--;
                CookieData.clear();
                if (testLoginTimes > 0)
                    login(ConnectSocketComplete, this.accountSrvComplete);
			    Close();
                break;
            case GameSrvCodeMap.RsCode.ERRO_CODE_INVALIDE_TOKEN:
                logReport.OnWarningReport("login token invalid,relogin.");
                CookieData.clear();
			    Close();
                break;
            case GameSrvCodeMap.RsCode.ERR_CODE_SRV_ERRO:
                logReport.OnWarningReport("login fail.srv erro");
				Close();
                break;
            case BaseCodeMap.BaseRsCode.TIME_OUT:
                logReport.OnWarningReport("login fail.time out");
				Close();
                onClosed();
                break;
			default:
			    logReport.OnWarningReport( "login fail,code:" + rsCode );
				Close();
				break;
        };
        if (ConnectSocketComplete != null)
        {
			ConnectSocketComplete(Check(),msg);
            ConnectSocketComplete = null;
        }
    }
}