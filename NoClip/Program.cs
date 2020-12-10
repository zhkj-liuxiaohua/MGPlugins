using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using CSR;

// 文件名：noclip.csr.dll
// 文件功能：开启/关闭穿墙能力
// 前置：BDSNetRunner商业版
// 指令：#穿墙 /noclip
namespace NoClip
{
	class Program
	{
		static private MCCSAPI mapi;
		
		//////////////////////// 预处理设置区 ////////////////////////
		static JavaScriptSerializer ser = new JavaScriptSerializer();
		
		// 发送一个tellraw
		public static void tellraw(string pname, string msg) {
			var rawtxt = new Dictionary<string, object>();
			var rawtext = new ArrayList();
			var text = new Dictionary<string, object>();
			text["text"] = msg;
			rawtext.Add(text);
			rawtxt["rawtext"] = rawtext;
			mapi.runcmd("tellraw \"" + pname + "\" " + ser.Serialize(rawtxt));
		}
		
		// 测试输入文本是否为固定指令集合
		static bool testNoclip(CsPlayer p, string t) {
			var ct = t.Trim();
			var ret = true;
			if (ct == "#穿墙" || ct == "/noclip") {
				ret = false;    // 命中，即将执行指令
				var uuid = p.Uuid;
				if (uuid != null) {
					var abilities = mapi.getPlayerAbilities(uuid);
					if (!string.IsNullOrEmpty(abilities)) {
						var ja = ser.Deserialize<Dictionary<string, object>>(abilities);
						object jnoc;
						if (ja.TryGetValue("noclip", out jnoc))
						if ((bool)jnoc) {
							// 即将停止穿墙能力
							var cja = new Dictionary<string, object>();
							cja["noclip"] = false;
							mapi.setPlayerAbilities(uuid, ser.Serialize(cja));
							tellraw(p.getName(), "您已取消穿墙模式。能力指令 #穿墙 或 /noclip");
						} else {
							// 即将启用穿墙能力
							var cja = new Dictionary<string, object>();
							cja["noclip"] = true;
							mapi.setPlayerAbilities(uuid, ser.Serialize(cja));
							tellraw(p.getName(), "您已开启穿墙模式。能力指令 #穿墙 或 /noclip");
						}
					}
				}
			}
			return ret;
		}
		
		//////////////////////// 监听器设置区 ////////////////////////
		
		// 主程序入口
		public static void init(MCCSAPI api)
		{
			mapi = api;
			api.addBeforeActListener(EventKey.onInputText, x => {
				var e = BaseEvent.getFrom(x) as InputTextEvent;
				if (e != null) {
					var p = new CsPlayer(api, e.playerPtr);
					if (!testNoclip(p, e.msg))
						return false;       // 直接拦截
				}
				return true;
			});
			api.addBeforeActListener(EventKey.onInputCommand, x => {
				var e = BaseEvent.getFrom(x) as InputCommandEvent;
				if (e != null) {
					var p = new CsPlayer(api, e.playerPtr);
					if (!testNoclip(p, e.cmd)) {
						return false;
					}// 直接拦截
				}
				return true;
			});
			api.setCommandDescribe("noclip", "启用或关闭穿墙。同 #穿墙 命令");
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
			if (api.COMMERCIAL) {
				NoClip.Program.init(api);
				Console.WriteLine("[noclip] noclip 指令已加载。用法(仅限玩家): #穿墙 或 /noclip");
			} else {
				Console.WriteLine("[noclip] 暂不适用于社区版。");
			}
		}
		#endregion
	}
}