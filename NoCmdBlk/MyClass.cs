/*
 * 由SharpDevelop创建。
 * 用户： 梦之故里
 * 日期: 2020/8/29
 * 时间: 23:24
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using CSR;

namespace NoCmdBlk
{
	/// <summary>
	/// 禁用命令块
	/// </summary>
	public class MyClass
	{
		//////////////////////// 监听器设置区 ////////////////////////
		
		// 主程序入口
		public static void init(MCCSAPI api){
			// 玩家修改命令块回调，直接拦截
			api.addBeforeActListener(EventKey.onCommandBlockUpdate, x => false);
			// 命令方块指令回调，直接拦截
			api.addBeforeActListener(EventKey.onBlockCmd, x => false);
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
				NoCmdBlk.MyClass.init(api);
				Console.WriteLine("[nocmdblk] 命令方块已禁用。");
			} else {
				Console.WriteLine("[NoCmdBlk] 暂不适用于社区版。");
			}
		}
		#endregion
	}
}
