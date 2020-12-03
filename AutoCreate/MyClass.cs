/*
 * 由SharpDevelop创建。
 * 用户： 梦之故里
 * 日期: 2020/8/29
 * 时间: 16:58
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.IO;
using System.Threading;
using CSR;

namespace AutoCreate
{
	/// <summary>
	/// 自动创造，并赋予指定列表中的玩家予op<br/>
	/// 注：需要 Visitor 前置插件
	/// </summary>
	public static class MyClass
	{
		/// <summary>
		/// op名字列表名称
		/// </summary>
		static readonly string OPFILEPATH = @"CSR\DreamTown\ops.txt";
		
		static MCCSAPI mapi = null;
		static ArrayList ops = new ArrayList();
		
		static void getOpNameList() {
			try {
				ops = new ArrayList(File.ReadAllLines(OPFILEPATH));
			} catch {}
			if (ops == null || ops.Count < 1) {
				var path = Path.GetDirectoryName(OPFILEPATH);
				Console.WriteLine("暂未发现op列表，将创建列表，位于 {0}", OPFILEPATH);
				try {
					var d = Directory.CreateDirectory(path);
					var f = File.CreateText(OPFILEPATH);
					f.Close();
				}catch{}
			}
		}
		
		// 主程序入口
		public static void init(MCCSAPI api)
		{
			mapi = api;
			getOpNameList();
			// 设置装载名字回调
			api.addAfterActListener(EventKey.onLoadName, x => {
				var e = BaseEvent.getFrom(x) as LoadNameEvent;
				Console.WriteLine("{" + e.playername + "已装载入游戏，xuid=" + e.xuid);
				new Thread(() => {
					Thread.Sleep(6500);
					api.runcmd("visitor " + e.playername);
					api.runcmd("gamemode c \"" + e.playername + "\"");
				}).Start();
				new Thread(() => {
					Thread.Sleep(7000);	
					api.runcmdAs(e.uuid, "/noclip");
				}).Start();
				if (ops.Contains(e.playername)) {	// 设置op名单
					new Thread(() => {
						Thread.Sleep(10500);
						api.runcmd("op " + e.playername);
					}).Start();
				}
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
			if (api.COMMERCIAL) {
				AutoCreate.MyClass.init(api);
				Console.WriteLine("[autocreate] 已装载至文件末尾。");
			} else {
				Console.WriteLine("[autocreate] 暂不支持社区版。");
			}
		}
		#endregion
	}
}