/*
 * 由SharpDevelop创建。
 * 用户： classmates
 * 日期: 2020/12/4
 * 时间: 4:53
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using CSR;

namespace Transfer
{
	/// <summary>
	/// 跨服传送
	/// </summary>
	public static class MyClass
	{
		static MCCSAPI api;
		public static JavaScriptSerializer ser = new JavaScriptSerializer();
		
		static string getPlayerUuidByName(string name) {
			try {
				var udata = ser.Deserialize<ArrayList>(api.getOnLinePlayers());
				if (udata != null) {
					foreach (Dictionary<string, object> ou in udata) {
						object oname;
						object ouuid;
						if (ou.TryGetValue("playername", out oname)) {
							if (oname.ToString() == name) {
								ou.TryGetValue("uuid", out ouuid);
								if (ouuid != null) {
									return ouuid.ToString();
								}
							}
						}
					}
				}
			} catch{}
			return null;
		}
		
		public static void init(MCCSAPI mapi)
		{
			// 初始化
			api = mapi;
			api.addAfterActListener(EventKey.onChat, x => {
			                        	var e = BaseEvent.getFrom(x) as ChatEvent;
			                        	if (e != null) {
			                        		var msg = e.msg;
			                        		if (msg.IndexOf('#') < 0) {
			                        			return true;
			                        		}
			                        		var args = msg.Split(' ');
			                        		var send = e.playername;
			                        		if (send == "Server" || send == "服务器" || send == "!§r") {
			                        			string uuid = getPlayerUuidByName(e.target);
			                        			if (!string.IsNullOrEmpty(uuid)) {
			                        				api.transferserver(uuid, args[1], int.Parse(args[2]));
			                        			}
			                        		}
			                        	}
			                        	return true;
			                        });
		}
	}
}


namespace CSR
{
	partial class Plugin
	{
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
		
		/// <summary>
		/// 通用调用接口，需用户自行实现
		/// </summary>
		/// <param name="api">MC相关调用方法</param>
		public static void onStart(MCCSAPI api) {
			// TODO 此接口为必要实现
			if (api.COMMERCIAL) {
				Transfer.MyClass.init(api);
				Console.WriteLine("[Transfer] 跨服传送插件已装载。用法：tell [player] [#transferserver] [address] [port]");
			} else {
				Console.WriteLine("[Transfer] 暂不适用于社区版。");
			}
		}
	}
}