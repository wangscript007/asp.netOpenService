﻿using liemei.Common;
using liemei.Common.common;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace liemei.WeChat.Model
{
    public class JsApiPay
    {
        /// <summary>
        /// openid用于调用统一下单接口
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// access_token用于获取收货地址js函数入口参数
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 商品金额，用于统一下单,单位分
        /// </summary>
        public int total_fee { get; set; }

        /// <summary>
        /// 统一下单接口返回结果
        /// </summary>
        public WxPayData unifiedOrderResult { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 附加信息
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 在线订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 商品标记,使用代金券或立减优惠功能时需要的参数
        /// </summary>
        public string goods_tag { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string product_id { get; set; }
        //public JsApiPay(Page page)
        //{
        //    this.page = page;
        //}

        public JsApiPay()
        {
            goods_tag = "test";
        }
        /**
        * 
        * 网页授权获取用户基本信息的全部过程
        * 详情请参看网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
        * 第一步：利用url跳转获取code
        * 第二步：利用code去获取openid和access_token
        * 
        */
        //public void GetOpenidAndAccessToken()
        //{
        //    if (!string.IsNullOrEmpty(HttpContext.Request.QueryString["code"]))
        //    {
        //        //获取code码，以获取openid和access_token
        //        string code = HttpContext.Request.QueryString["code"];
        //        ClassLoger.DEBUG(this.GetType().ToString(), "Get code : " + code);
        //        GetOpenidAndAccessTokenFromCode(code);
        //    }
        //    else
        //    {
        //        //构造网页授权获取code的URL
        //        string host = HttpContext.Request.Url.Host;
        //        string path = HttpContext.Request.Path;
        //        string redirect_uri = HttpUtils.Ins.UrlEncode("http://" + host + path);
        //        WxPayData data = new WxPayData();
        //        data.SetValue("appid", SystemSet.Serviceappid);
        //        data.SetValue("redirect_uri", redirect_uri);
        //        data.SetValue("response_type", "code");
        //        data.SetValue("scope", "snsapi_base");
        //        data.SetValue("state", "STATE" + "#wechat_redirect");
        //        string url = "https://open.weixin.qq.com/connect/oauth2/authorize?" + data.ToUrl();
        //        ClassLoger.DEBUG(this.GetType().ToString(), "Will Redirect to URL : " + url);
        //        try
        //        {
        //            //触发微信返回code码         
        //            HttpContext.Response.Redirect(url);//Redirect函数会抛出ThreadAbortException异常，不用处理这个异常
        //        }
        //        catch (System.Threading.ThreadAbortException ex)
        //        {
        //        }
        //    }
        //}


        /**
	    * 
	    * 通过code换取网页授权access_token和openid的返回数据，正确时返回的JSON数据包如下：
	    * {
	    *  "access_token":"ACCESS_TOKEN",
	    *  "expires_in":7200,
	    *  "refresh_token":"REFRESH_TOKEN",
	    *  "openid":"OPENID",
	    *  "scope":"SCOPE",
	    *  "unionid": "o6_bmasdasdsad6_2sgVt7hMZOPfL"
	    * }
	    * 其中access_token可用于获取共享收货地址
	    * openid是微信支付jsapi支付接口统一下单时必须的参数
        * 更详细的说明请参考网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
        * @失败时抛异常WxPayException
	    */
        public void GetOpenidAndAccessTokenFromCode(string code)
        {
            try
            {
                //构造获取openid及access_token的url
                WxPayData data = new WxPayData();
                data.SetValue("appid", SystemSet.Serviceappid);
                data.SetValue("secret", SystemSet.Serviceappsecret);
                data.SetValue("code", code);
                data.SetValue("grant_type", "authorization_code");
                string url = "https://api.weixin.qq.com/sns/oauth2/access_token?" + data.ToUrl();

                //请求url以获取数据
                string result = HttpUtils.Ins.GET(url);

                ClassLoger.DEBUG(this.GetType().ToString(), "GetOpenidAndAccessTokenFromCode response : " + result);

                //保存access_token，用于收货地址获取
                JsonData jd = JsonMapper.ToObject(result);
                access_token = (string)jd["access_token"];

                //获取用户openid
                openid = (string)jd["openid"];

                ClassLoger.DEBUG(this.GetType().ToString(), "Get openid : " + openid);
                ClassLoger.DEBUG(this.GetType().ToString(), "Get access_token : " + access_token);
            }
            catch (Exception ex)
            {
                ClassLoger.Error(this.GetType().ToString(), ex.ToString());
                throw new WxPayException(ex.ToString());
            }
        }

        /**
         * 调用统一下单，获得下单结果
         * @return 统一下单结果
         * @失败时抛异常WxPayException
         */
        public WxPayData GetUnifiedOrderResult()
        {
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", body);
            data.SetValue("attach", attach);
            data.SetValue("out_trade_no", out_trade_no);
            data.SetValue("total_fee", total_fee);
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", "test");
            data.SetValue("trade_type", trade_type);
            data.SetValue("openid", openid);
            data.SetValue("product_id", product_id);
            WxPayData result = WxPayApi.UnifiedOrder(data);
            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                ClassLoger.Error(this.GetType().ToString(), "UnifiedOrder response error!");
                throw new WxPayException("UnifiedOrder response error!");
            }

            unifiedOrderResult = result;
            return result;
        }

        /**
        *  
        * 从统一下单成功返回的数据中获取微信浏览器调起jsapi支付所需的参数，
        * 微信浏览器调起JSAPI时的输入参数格式如下：
        * {
        *   "appId" : "wx2421b1c4370ec43b",     //公众号名称，由商户传入     
        *   "timeStamp":" 1395712654",         //时间戳，自1970年以来的秒数     
        *   "nonceStr" : "e61463f8efa94090b1f366cccfbbb444", //随机串     
        *   "package" : "prepay_id=u802345jgfjsdfgsdg888",     
        *   "signType" : "MD5",         //微信签名方式:    
        *   "paySign" : "70EA570631E4BB79628FBCA90534C63FF7FADD89" //微信签名 
        * }
        * @return string 微信浏览器调起JSAPI时的输入参数，json格式可以直接做参数用
        * 更详细的说明请参考网页端调起支付API：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_7
        * 
        */
        public string GetJsApiParameters()
        {
            ClassLoger.DEBUG(this.GetType().ToString(), "JsApiPay::GetJsApiParam is processing...");

            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("appId", unifiedOrderResult.GetValue("appid"));
            jsApiParam.SetValue("timeStamp", WxPayApi.GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", WxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + unifiedOrderResult.GetValue("prepay_id"));
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign());

            string parameters = jsApiParam.ToJson();

            ClassLoger.DEBUG(this.GetType().ToString(), "Get jsApiParam : " + parameters);
            return parameters;
        }


        /**
	    * 
	    * 获取收货地址js函数入口参数,详情请参考收货地址共享接口：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_9
	    * @return string 共享收货地址js函数需要的参数，json格式可以直接做参数使用
	    */
        //public string GetEditAddressParameters()
        //{
        //    string parameter = "";
        //    try
        //    {
        //        string host = HttpContext.Request.Url.Host;
        //        string path = HttpContext.Request.Path;
        //        string queryString = HttpContext.Request.Url.Query;
        //        //这个地方要注意，参与签名的是网页授权获取用户信息时微信后台回传的完整url
        //        string url = "http://" + host + path + queryString;

        //        //构造需要用SHA1算法加密的数据
        //        WxPayData signData = new WxPayData();
        //        signData.SetValue("appid", SystemSet.Serviceappid);
        //        signData.SetValue("url", url);
        //        signData.SetValue("timestamp", WxPayApi.GenerateTimeStamp());
        //        signData.SetValue("noncestr", WxPayApi.GenerateNonceStr());
        //        signData.SetValue("accesstoken", access_token);
        //        string param = signData.ToUrl();

        //        ClassLoger.DEBUG(this.GetType().ToString(), "SHA1 encrypt param : " + param);
        //        //SHA1加密
        //        string addrSign = FormsAuthentication.HashPasswordForStoringInConfigFile(param, "SHA1");
        //        ClassLoger.DEBUG(this.GetType().ToString(), "SHA1 encrypt result : " + addrSign);

        //        //获取收货地址js函数入口参数
        //        WxPayData afterData = new WxPayData();
        //        afterData.SetValue("appId", SystemSet.Serviceappid);
        //        afterData.SetValue("scope", "jsapi_address");
        //        afterData.SetValue("signType", "sha1");
        //        afterData.SetValue("addrSign", addrSign);
        //        afterData.SetValue("timeStamp", signData.GetValue("timestamp"));
        //        afterData.SetValue("nonceStr", signData.GetValue("noncestr"));

        //        //转为json格式
        //        parameter = afterData.ToJson();
        //        ClassLoger.DEBUG(this.GetType().ToString(), "Get EditAddressParam : " + parameter);
        //    }
        //    catch (Exception ex)
        //    {
        //        ClassLoger.Error(this.GetType().ToString(), ex.ToString());
        //        throw new WxPayException(ex.ToString());
        //    }

        //    return parameter;
        //}
    }
}
