using System.Collections;

public class CodeMap {

	public static class Filed
	{
		//系统字段
		/// <summary>
		/// IMEI
		/// </summary>
		public const int Filed_IMEI = 0;
		/// <summary>
		/// SDK渠道号
		/// </summary>
		public const int Filed_SDK_CODE = 1;
		/// <summary>
		/// 扩展字段1
		/// </summary>
		public const int FIELDE_EXT1 = 2;
		//cookie字段
		public const int FIELED_COOKIE = 100;
		/// <summary>
		/// 账号id
		/// </summary>
		public const int FIELED_COOKIE_ACCOUNT_ID = FIELED_COOKIE + 1;
		/// <summary>
		/// TOKEN.
		/// </summary>
		public const int FIELED_COOKIE_ACCOUNT_TOKEN = FIELED_COOKIE + 2;
		/// <summary>
		/// 服务器id
		/// </summary>
		public const int FIELED_COOKIE_ACCOUNT_SRV_ID = FIELED_COOKIE + 3;
		/// <summary>
		/// TOKE + TIMESTAM.
		/// </summary>
		public const int FIELED_COOKIE_ACCOUNT_TOKEN_TIMESTAMP = FIELED_COOKIE + 4;
		/// <summary>
		/// 账号是否绑定
		/// </summary>
		public const int FIELED_COOKIE_ACCOUNT_IS_BIND = FIELED_COOKIE + 5;
        /// <summary>
        /// 支付字段
        /// </summary>
        public const int FIELED_PAYMENT = 200;
        /// <summary>
        /// 支付数据
        /// </summary>
        public const int FIELED_PAYMENT_DATA = FIELED_PAYMENT + 1;

    }
}
