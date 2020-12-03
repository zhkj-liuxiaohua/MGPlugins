/*
 * 由SharpDevelop创建。
 * 用户： 梦之故里
 * 日期: 2020/8/29
 * 时间: 23:41
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
 
 // 调试开关
 //#define _DEBUG_OFF
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using CSR;

namespace DtOnLineScore
{
	/// <summary>
	/// 怀旧服收集并统计积分<br/>
	/// 注：本插件已过期，停止维护
	/// </summary>
	public class MyClass
	{
		static MCCSAPI mapi;
		//////////////////////// 通用方法定义区域 ////////////////////////
		// 服务器域名
		const string _dtonline_url = "http://localhost:81/";
		// 服务器注册密码
		const string _dtonline_pw = "DREAMTOWN";

		// 服务器名注册接口
		const string _dtapi_register = "register.ashx";
		// 激活
		const string _dtapi_live = "live.ashx";
		// 售出
		const string _dtapi_sell = "sell.ashx";
		// 参数关键字
		const string _dtSERVERNAME = "梦之故里";

		const string _dtKEY_SESSION = "session";
		const string _dtKEY_NAME = "name";
		const string _dtKEY_KEY = "KEY";
		const string _dtKEY_XUID = "xuid";
		const string _dtKEY_COUNT = "count";

		//const bool _DEBUG_OFF = true;

		// 通信用关键字
		static string _dtparam_KEY = "";
		// 怀旧服所有已登录玩家临时存储的积分，每分钟统计一次
		static Hashtable _dtonline_counts = new Hashtable();
		// 怀旧服登录玩家信息，以名字为键
		static Hashtable _dtonline_players = new Hashtable();
		
		// 打印调试信息
		static void _DT_PR(string e) {
			#if _DEBUG_OFF
			Console.WriteLine("{" + e);
			#endif
		}
		
		// 更新在线玩家列表
		static void _dt_refreshPlayers() {
			var strlist = mapi.getOnLinePlayers();
			if (!string.IsNullOrEmpty(strlist)) {
				_dtonline_players = new Hashtable();	// 重新更新玩家列表
				var ser = new JavaScriptSerializer();
				var list = ser.Deserialize<ArrayList>(strlist);
				if (list != null && list.Count > 0)
					for (int i = 0, l = list.Count; i < l; i++) {
					var p = (Dictionary<string, object>)list[i];
					_dtonline_players[p["playername"].ToString()] = p["xuid"].ToString();
				}
			}
		}
		
		// 请求回调
		public delegate void ReqCb(string result);
		
		// 从http地址获取网页内容
		public static string getHttpData(string uri) {
			Uri url = new Uri(uri);	// 初始化uri
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);		// 初始化请求
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();	// 得到响应
			Stream stream = response.GetResponseStream();	// 获取响应的主体
			StreamReader reader = new StreamReader(stream);	// 初始化读取器
			string result = reader.ReadToEnd();	// 读取，存储结果
			reader.Close();	// 关闭读取器，释放资源
			stream.Close();	// 关闭流，释放资源
			return result;
		}
		
		/// <summary>
		/// 指定Post地址使用Get 方式获取全部字符串
		/// </summary>
		/// <param name="url">请求后台地址</param>
		/// <param name="content">Post提交数据内容(utf-8编码的)</param>
		/// <returns></returns>
		public static string Post(string url, string content)
		{
			string result = "";
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			#region 添加Post 参数
			byte[] data = Encoding.UTF8.GetBytes(content);
			req.ContentLength = data.Length;
			using (Stream reqStream = req.GetRequestStream()) {
				reqStream.Write(data, 0, data.Length);
				reqStream.Close();
			}
			#endregion
			HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
			Stream stream = resp.GetResponseStream();
			//获取响应内容
			using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
				result = reader.ReadToEnd();
			}
			return result;
		}
		
		// 发起一个网络请求
		static void request(string url, string method, string param, ReqCb r) {
			if (method == "GET") {
				if (!string.IsNullOrEmpty(param)) {
					url += "?";
					url += param;
				}
				new Thread(() => {
					string ret = null;
					try {
						ret = getHttpData(url);
					} catch (Exception e) {
						Console.WriteLine(e.StackTrace);
					}
					r(ret);
				}).Start();
				
			} else {
				new Thread(() => {
					string ret = null;
					try {
						ret = Post(url, param);
					} catch (Exception e) {
						Console.WriteLine(e.StackTrace);
					}
					r(ret);
				}).Start();
			}
		}
		
		// 激活用户
		static void _dt_translive(string xuid, ReqCb f) {
			var p = _dtonline_counts[xuid];
			if (p != null) {
				var po = (Dictionary<string, object>)p;
				var url = _dtonline_url + _dtapi_live;
				var Params = _dtKEY_KEY + "=" + _dtparam_KEY + "&" + _dtKEY_XUID + "=" + po["xuid"].ToString();
				_DT_PR("发起激活请求：" + url + '?' + Params);
				request(url, "GET", Params, x => {
					_DT_PR("激活结果：" + x);
					if (f != null) {
						f(x);
					}
				});
			}
		}
		
		// 取积分中信息并上传服务器 使用延时方式调用此方法
		static void _dt_transSell(string xuid, ReqCb f) {
			var p = _dtonline_counts[xuid];
			if (p != null) {
				var po = (Dictionary<string, object>)p;
				object ocount;
				if (po.TryGetValue("count", out ocount))
				if ((int)ocount > 0) {
					var url = _dtonline_url + _dtapi_sell;
					var Params = _dtKEY_KEY + "=" + _dtparam_KEY + "&" + _dtKEY_XUID + "=" + po["xuid"].ToString() + "&" +
					           _dtKEY_COUNT + "=" + ocount;
					_DT_PR("发起请求：" + url + '?' + Params);
					request(url, "GET", Params, e => {
						_DT_PR("积分上传结果：" + e);
						var str = e.Split(',');
						if (str[0] == "True") {
							// 已售出
							int oldc = (int)po["count"];
							po["count"] = oldc - (int)ocount;	// 减少当前玩家手头持有的积分
						}
						if (f != null)
							f(e);	// 回调
					});
				}
			}
		}
		
		// 设置一个定时上传器
		static void _dt_startLinsten(string xuid, int count) {
			_DT_PR("即将上传：xuid=" + xuid);
			var p = _dtonline_counts[xuid];
			if (p != null) {
				var po = (Dictionary<string, object>)p;
				object ocount;
				po.TryGetValue("count", out ocount);
				object oisOnLine;
				po.TryGetValue("isOnLine", out oisOnLine);
				if (ocount != null && oisOnLine != null) {
					var oldcount = (int)ocount;
					po["count"] = oldcount + ((bool)oisOnLine ? count : 0);	// 如果在线，增加指定个数积分
				}
				if (oisOnLine != null && (bool)oisOnLine) {
					_dt_translive(xuid, e => {
						var str = e.Split(',');
						if (str[0] == "True") {
							// 激活成功，发送售出请求
							_dt_transSell(xuid, null);	// 因内部已处理，暂时不用回调
						}
					});
				}
				if (oisOnLine != null && (bool)oisOnLine) {
					po["isListened"] = true;
					new Thread(() => {
						Thread.Sleep(60000);
						_dt_startLinsten(xuid, 60);
					}).Start();	// 一分钟上传一次，并一次累加60积分
				} else {
					po["isListened"] = false;
				}
			}
		}
		
		// 根据玩家名查找该玩家的xuid
		static string _dt_getXuidByName(string e) {
			var xuid = _dtonline_players[e];
			if (xuid == null) {
				_dt_refreshPlayers();	// 名称不对应的情况 重载玩家列表
				xuid = _dtonline_players[e];
			}
			return (string)xuid;
		}
		
		//////////////////////// 监听器设置区 ////////////////////////
		
		// 主程序入口
		public static void init(MCCSAPI api) {
			mapi = api;
			// 注册服务器到远程商店
			new Thread(() => {
				Thread.Sleep(1000);
				request(_dtonline_url + _dtapi_register, "POST", _dtKEY_SESSION + "=" + _dtonline_pw + "&" +
				_dtKEY_NAME + "=" + _dtSERVERNAME, e => {
					_DT_PR("注册结果：" + e);
					_dtparam_KEY = e;	// 保存通信用关键字
				});
			}).Start();
			
			// 方块放置监听
			api.addAfterActListener(EventKey.onPlacedBlock, x => {
				var e = BaseEvent.getFrom(x) as PlacedBlockEvent;
				if (e != null && e.RESULT) {
					var xuid = _dt_getXuidByName(e.playername);	// 直接查询
					var p = _dtonline_counts[xuid];
					if (p != null) {
						// 此处添加积分，放置方块+1分
						var po = (Dictionary<string, object>)p;
						po["count"] = (int)po["count"] + 1;
						_DT_PR(e.playername + "放置方块，积分+1");
					}
				}
				return true;
			});
			
			// 方块破坏监听
			api.addAfterActListener(EventKey.onDestroyBlock, x => {
				var e = BaseEvent.getFrom(x) as DestroyBlockEvent;
				if (e != null && e.RESULT) {
					var xuid = _dt_getXuidByName(e.playername);	// 直接查询
					var p = _dtonline_counts[xuid];
					if (p != null) {
						// 此处添加积分，破坏方块+1分
						var po = (Dictionary<string, object>)p;
						po["count"] = (int)po["count"] + 1;
						_DT_PR(e.playername + "破坏方块，积分+1");
					}
				}
				return true;
			});
			// 生物死亡监听
			api.addAfterActListener(EventKey.onMobDie, x => {
				var e = BaseEvent.getFrom(x) as MobDieEvent;
				if (!string.IsNullOrEmpty(e.srcname)) {
					var xuid = _dt_getXuidByName(e.srcname);	// 直接查询
					if (xuid != null) {
						// 击杀生物，临时存储积分+1
						var p = _dtonline_counts[xuid];
						if (p != null) {
							var po = (Dictionary<string, object>)p;
							po["count"] = (int)po["count"] + 1;
							_DT_PR(e.srcname + "击杀生物，积分+1");
						}
					}
				}
				return true;
			});
			
			// 装载名字回调
			api.addAfterActListener(EventKey.onLoadName, x => {
				var e = BaseEvent.getFrom(x) as LoadNameEvent;
				var xuid = e.xuid;
				var p = _dtonline_counts[xuid];
				Dictionary<string, object> po;
				if (p == null) {
					// 第一次创建重新构筑玩家信息
					po = new Dictionary<string, object>();
					po["isListened"] = false;
					_dtonline_counts[xuid] = po;
				} else {
					po = (Dictionary<string, object>)p;
				}
				po["count"] = 0;
				po["xuid"] = xuid;
				po["isOnLine"] = true;
				_dtonline_players[e.playername] = xuid;
				if (!(bool)po["isListened"])
					_dt_startLinsten(xuid, 1);	// 未监听状态，初始值为1，开始计时
				Console.WriteLine("{" + e.playername + "已装载入游戏，xuid=" + e.xuid);
				return true;
			});
			
			// 玩家离开游戏回调
			api.addAfterActListener(EventKey.onPlayerLeft, x => {
				var e = BaseEvent.getFrom(x) as PlayerLeftEvent;
				var xuid = e.xuid;
				var p = _dtonline_counts[xuid];
				if (p != null) {
					var po = (Dictionary<string, object>)p;
					po["isOnLine"] = false;	// 离线状态已设置
				}
				Console.WriteLine("{" + e.playername + "已离开游戏，xuid=" + e.xuid);
				return true;
			});
		}
	}
}


namespace CSR {
	partial class Plugin {
		private static MCCSAPI mapi = null;
		/// <summary>
		/// 静态api对象
		/// </summary>
		public static MCCSAPI api { get { return mapi; } }
		public static int onServerStart(string pathandversion) {
			string path = null, version = null;
			bool commercial = false;
			string [] pav = pathandversion.Split(',');
			if (pav.Length > 1) {
				path = pav[0];
				version = pav[1];
				commercial = (pav[pav.Length - 1] == "1");
				mapi = new MCCSAPI(path, version, commercial);
				if (mapi != null) {
					onStart(mapi);
					GC.KeepAlive(mapi);
					return 0;
				}
			}
			Console.WriteLine("Load failed.");
			return -1;
		}

		~Plugin()
        {
			//Console.WriteLine("[CSR Plugin] Ref released.");
        }

		#region 必要接口 onStart ，由用户实现
		public static void onStart(MCCSAPI api) {
			DtOnLineScore.MyClass.init(api);
			Console.WriteLine("[DtOnLineScore] 梦故积分已启用。");
		}
		#endregion
	}
}
