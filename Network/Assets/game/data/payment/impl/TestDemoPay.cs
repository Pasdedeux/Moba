using System;

namespace common.gameData.payment.impl
{
    class TestDemoPay : SDKHandler
    {
        public void DoPay(string orderid, int sdkCode, int productId, string sdkProuctId, int productNum, OnSDKPayReturn onSDKPayReturn)
        {
            onSDKPayReturn(orderid, sdkCode,productId,productNum, "CNY", new PayRs(true, "xxxxx"));
        }
    }
}
