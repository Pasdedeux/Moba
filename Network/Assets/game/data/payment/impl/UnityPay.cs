//using IAPCustom;

namespace common.gameData.payment.impl
{
    public class UnityPay : SDKHandler
    {
        public void DoPay(string orderid, int sdkCode, int productId, string sdkProuctId, int productNum, OnSDKPayReturn onSDKPayReturn)
        {
            /*UnityPurchaser.Instance.setCurrentBuyInfo( orderid, sdkCode, productId, sdkProuctId, productNum, onSDKPayReturn);
            UnityPurchaser.Instance.BuyProductID(sdkProuctId);*/
        }
    }
}
