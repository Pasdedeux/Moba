using System;
using UnityEngine;
namespace common.gameData
{
    class BaseDao
    {
        private UnityLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        public void Save(Po po)
        {
            byte[] bytes = po.GetBinData();
            string info = Convert.ToBase64String(bytes);
            string tableField = po.GetTableField().ToString();
            PlayerPrefs.SetString(tableField, info);
            logReport.OnLogReport("save tableFiled:"+ tableField + " data to local,data size:"+bytes.Length+",saveInfo:"+info);
        }
        public byte[] Load(int tableField)
        {
            string info = PlayerPrefs.GetString(tableField.ToString(), null);
            byte[] bytes = null;
            if (info != null)
                bytes = Convert.FromBase64String(info);
            logReport.OnLogReport("load tableFiled:" + tableField + " ,data size:" + (bytes == null?0:bytes.Length)+",saveInfo:"+info);
            return bytes;
        }
    }
}