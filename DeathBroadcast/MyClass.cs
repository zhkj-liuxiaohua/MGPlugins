/*
 * 由SharpDevelop创建。
 * 用户： 梦之故里
 * 日期: 2020/12/13
 * 时间: 18:54
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using CSR;

namespace DeathBroadcast
{
	/// <summary>
	/// 死亡信息推送广播
	/// </summary>
	public static class MyClass
	{
		static MCCSAPI mapi;
		public static void init(MCCSAPI api) {
			mapi = api;
			api.addAfterActListener(EventKey.onMobDie, x => {
			                        	var e = BaseEvent.getFrom(x) as MobDieEvent;
			                        	if (e != null) {
			                        		if (e.mobtype == "entity.player.name") {
			                        			var p = new CsPlayer(api, e.mobPtr);
			                        			string []tips = {"倒在", "亡命于","悲剧在","呜呼于"};
			                        			int tid = new Random().Next(4);
			                        			string dtip = tips[tid];
			                        			string cmd = string.Format("me §e>> 我{0} {1}的({2}, {3}, {4})位置 <<",
			                        			                           dtip, e.dimension, (int)e.XYZ.x, (int)e.XYZ.y, (int)e.XYZ.z);
			                        			api.runcmdAs(p.Uuid, cmd);
			                        		}
			                        	}
			                        	return true;
			                        });
			Console.WriteLine("[DeathBroadcast] 死亡播报已加载。");
		}
	}
}


namespace CSR {
	partial class Plugin {

		#region 必要接口 onStart ，由用户实现
		public static void onStart(MCCSAPI api) {
			DeathBroadcast.MyClass.init(api);
		}
		#endregion
	}
}