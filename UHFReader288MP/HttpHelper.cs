using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UHFReader288MPDemo;

namespace UHFReader288MP
{
    public struct ResultStruct
    {
        public string tag;
        public string size;
        public string type;
        public string color;
        public string pic;
        public string in_money;
        public string out_money;
        public string supplier_id;
        public string warehouse_id;
        public string type_name;
        public string color_name;
        public string supplier_name;
        public string warehouse_name;
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
        public static string url = "http://smarts.zhuyy.cn/api/linen/getLinenDetail?epc_id=";

        public static bool GetLabelProperty(string epc, out ResultStruct result)
        {
            result = new ResultStruct();
            string cmd = url + ConvertEPC(epc);
            cmd = "http://smarts.zhuyy.cn/api/linen/getLinenDetail?epc_id=E2806995000050043565C00E";
            string str;
            try
            {
                str = Get(cmd);
                if (str == null)

                {
                    Console.WriteLine("StartCollection not get responce");
                    return false;
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str);
                int code = int.Parse(jo["code"].ToString());
                string message = jo["msg"].ToString();
                if (code != 200)
                {
                    Log.WriteError(message);
                    return false;
                }

                JObject data = (JObject)JsonConvert.DeserializeObject(jo["data"].ToString());
                result.tag = data["tag"].ToString();
                result.size = data["size"].ToString();
                //result.type = data["type"].ToString();
                //result.color = data["color"].ToString();
                result.pic = "http://smarts.zhuyy.cn" + data["pic"].ToString();
                result.in_money = data["in_money"].ToString();
                result.out_money = data["out_money"].ToString();
                //result.supplier_id = data["supplier_id"].ToString();
                //result.warehouse_id = data["warehouse_id"].ToString();
                result.type_name = data["type_name"].ToString();
                result.color_name = data["color_name"].ToString();
                result.supplier_name = data["supplier_name"].ToString();
                result.warehouse_name = data["warehouse_name"].ToString();
            }
            catch(Exception ex)
            {
                Log.WriteError(ex.Message);
                return false;
            }

            return true;
        }

        public static string ConvertEPC(string epc)
        {
            string res = "";
            
            Dictionary<char, char> pwd_map = new Dictionary<char, char>();
            pwd_map.Add('0', '3');
            pwd_map.Add('1', '9');
            pwd_map.Add('2', '0');
            pwd_map.Add('3', '5');
            pwd_map.Add('4', '1');
            pwd_map.Add('5', '7');
            pwd_map.Add('6', '2');
            pwd_map.Add('7', '4');
            pwd_map.Add('8', '6');
            pwd_map.Add('9', '8');

            pwd_map.Add('A', 'W');
            pwd_map.Add('B', 'N');
            pwd_map.Add('C', 'L');
            pwd_map.Add('D', 'T');
            pwd_map.Add('E', 'K');
            pwd_map.Add('F', 'V');
            pwd_map.Add('G', 'R');
            pwd_map.Add('H', 'A');
            pwd_map.Add('I', 'D');
            pwd_map.Add('J', 'Z');
            pwd_map.Add('K', 'Q');
            pwd_map.Add('L', 'U');
            pwd_map.Add('M', 'H');
            pwd_map.Add('N', 'X');
            pwd_map.Add('O', 'E');
            pwd_map.Add('P', 'C');
            pwd_map.Add('Q', 'F');
            pwd_map.Add('R', 'G');
            pwd_map.Add('S', 'Y');
            pwd_map.Add('T', 'M');
            pwd_map.Add('U', 'J');
            pwd_map.Add('V', 'B');
            pwd_map.Add('W', 'P');
            pwd_map.Add('X', 'I');
            pwd_map.Add('Y', 'O');
            pwd_map.Add('Z', 'S');

            pwd_map.Add('a', 'w');
            pwd_map.Add('b', 'n');
            pwd_map.Add('c', 'l');
            pwd_map.Add('d', 't');
            pwd_map.Add('e', 'k');
            pwd_map.Add('f', 'v');
            pwd_map.Add('g', 'r');
            pwd_map.Add('h', 'a');
            pwd_map.Add('i', 'd');
            pwd_map.Add('j', 'z');
            pwd_map.Add('k', 'q');
            pwd_map.Add('l', 'u');
            pwd_map.Add('m', 'h');
            pwd_map.Add('n', 'x');
            pwd_map.Add('o', 'e');
            pwd_map.Add('p', 'c');
            pwd_map.Add('q', 'f');
            pwd_map.Add('r', 'g');
            pwd_map.Add('s', 'y');
            pwd_map.Add('t', 'm');
            pwd_map.Add('u', 'j');
            pwd_map.Add('v', 'b');
            pwd_map.Add('w', 'p');
            pwd_map.Add('x', 'i');
            pwd_map.Add('y', 'o');
            pwd_map.Add('z', 's');

            char[] arrayChars = new char[epc.Length];
            for (int i = 0; i < epc.Length; ++i)
            {
                arrayChars[i] = pwd_map[epc[i]];
            }

            res = new string(arrayChars);

            return res;
        }

        public static string Get(string Url, int timeOut = 2)
        {
            //System.GC.Collect();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Timeout = timeOut * 1000;
            request.ReadWriteTimeout = timeOut * 1000;
            request.Proxy = null;
            request.KeepAlive = false;
            request.Method = "GET";
            request.ContentType = "application/json; charset=UTF-8";
            request.AutomaticDecompression = DecompressionMethods.GZip;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();

            if (response != null)
            {
                response.Close();
            }
            if (request != null)
            {
                request.Abort();
            }

            return retString;
        }

        public static int HttpGet(string url, out string reslut)
        {
            reslut = "";
            try
            {
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
                wbRequest.Proxy = null;
                wbRequest.Method = "GET";
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream))
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
