using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UHFReader288MP
{
    public struct ResultStruct
    {
        string tag;
        string size;
        int type;
        int color;
        string pic;
        string in_money;
        string out_money;
        int supplier_id;
        int warehouse_id;
        string type_name;
        string color_name;
        string supplier_name;
        string warehouse_name;
    }
    /// <summary>
    /// url: /api/linen/getLinenDetail
    /// get
    /// epc_id:string
    /// {
    /// "code": 200,
    /// "data": {
    /// "tag": "544KSJSJL42",
    /// "size": "100*100",
    /// "type": 14,
    /// "color": 26,
    /// "pic": "/storage/uploads/1/image/common/20220916/10f941dc5007df1436621ecb21b6ee2a.jpg",
    /// "in_money": "10.00",
    /// "out_money": "25.00",
    /// "supplier_id": 3,
    /// "warehouse_id": 7,
    /// "type_name": "towels",
    /// "color_name": "blue",
    /// "supplier_name": "供应商3",
    /// "warehouse_name": "仓库2"
    /// },
    /// "msg": "operation succeeded"
    /// }
    /// </summary>
    public class HttpHelper
    {
        public static string url = "/api/linen/getLinenDetail";

        public static bool GetLabelProperty(string epc, out ResultStruct result)
        {
            result = new ResultStruct();
            string str;
            string cmd = "{\"epc_id\"=\"" + epc + "\"}";
            int res = HttpGet(url, cmd, out str);

            if (res != 0)
            {
                return false;
            }

            return true;
        }
        
        public static int HttpGet(string url, string sendData, out string reslut)
        {
            reslut = "";
            try
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(sendData);
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);  // 制备web请求
                wbRequest.Proxy = null;     //现场测试注释掉也可以上传
                wbRequest.Method = "Get";
                wbRequest.ContentType = "application/json";
                wbRequest.ContentLength = data.Length;

                //#region //【1】获得请求流，OK
                //Stream newStream = wbRequest.GetRequestStream();
                //newStream.Write(data, 0, data.Length);
                //newStream.Close();//关闭流
                //newStream.Dispose();//释放流所占用的资源
                //#endregion

                #region //【2】将创建Stream流对象的过程写在using当中，会自动的帮助我们释放流所占用的资源。OK
                using (Stream wStream = wbRequest.GetRequestStream())         //using(){}作为语句，用于定义一个范围，在此范围的末尾将释放对象。
                {
                    wStream.Write(data, 0, data.Length);
                }
                #endregion

                //获取响应
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream, Encoding.UTF8))      //using(){}作为语句，用于定义一个范围，在此范围的末尾将释放对象。
                    {
                        reslut = sReader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                reslut = e.Message;     //输出捕获到的异常，用OUT关键字输出
                return -1;              //出现异常，函数的返回值为-1
            }
            return 0;
        }


    }
}
