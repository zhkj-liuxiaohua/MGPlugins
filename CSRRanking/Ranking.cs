﻿/*
 * 由SharpDevelop创建。
 * 用户： 梦之故里
 * 日期: 2020/7/28
 * 时间: 0:45
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web.Script.Serialization;
using System.IO;
using CSR;

namespace CSRRanking
{
	/// <summary>
	/// Ranking<br/>
	/// 排行榜<br/>
	/// 总计四项轮换：挖掘榜，放置榜，击杀榜，阵亡榜
	/// </summary>
	public static class Ranking
	{
		private static MCCSAPI mapi = null;
		
		/// <summary>
		/// 固定数据文件所在路径
		/// </summary>
		static readonly string RANKINGCONFIG = @"CSR\configs\ranking.json";
		
		const string KEY_DIGMAP = "DIGMAP";
		const string KEY_PLACEMAP = "PLACEMAP";
		const string KEY_KILLMAP = "KILLMAP";
		const string KEY_DIEMAP = "DIEMAP";
		
		/// <summary>
		/// 数据内容读写
		/// </summary>
		public static string RANKINGS {
			get {
				string t = null;
				try {
					t = File.ReadAllText(RANKINGCONFIG);
				} catch{}
				return t;
			}
			set {
				try {
					string dir = Path.GetDirectoryName(RANKINGCONFIG);
					if (!Directory.Exists(dir))
						Directory.CreateDirectory(dir);
					File.WriteAllText(RANKINGCONFIG, value);
				} catch{}
			}
		}
		
		// 排行列表集
		public static Dictionary<string, object> rankinglists;
		// 挖掘榜
		public static Dictionary<string, object> digmap;
		// 放置榜
		public static Dictionary<string, object> placemap;
		// 击杀榜
		public static Dictionary<string, object> killmap;
		// 阵亡榜
		public static Dictionary<string, object> diemap;
		
		public static JavaScriptSerializer ser = null;
		
		// 登录信息
		public static volatile Hashtable players = new Hashtable();
		
		// 重载所有数据
		public static void reloadData() {
			string t = RANKINGS;
			rankinglists = new Dictionary<string, object>();
			digmap = new Dictionary<string, object>();
			placemap = new Dictionary<string, object>();
			killmap = new Dictionary<string, object>();
			diemap = new Dictionary<string, object>();
			if (string.IsNullOrEmpty(t)) {
				Console.WriteLine("[Ranking] 默认数据为空，重新计数。");
			} else {
				ser = ser ?? new JavaScriptSerializer();
				try {
					rankinglists = ser.Deserialize<Dictionary<string, object>>(t);
					object o = null;
					rankinglists.TryGetValue(KEY_DIGMAP, out o);
					digmap = ((Dictionary<string, object>)o ?? digmap);
					o = null;
					rankinglists.TryGetValue(KEY_PLACEMAP, out o);
					placemap = ((Dictionary<string, object>)o ?? placemap);
					o = null;
					rankinglists.TryGetValue(KEY_KILLMAP, out o);
					killmap = ((Dictionary<string, object>)o ?? killmap);
					o = null;
					rankinglists.TryGetValue(KEY_DIEMAP, out o);
					diemap = ((Dictionary<string, object>)o ?? diemap);
				} catch{}
			}
		}
		
		// 保存所有排名数据
		public static void saveData() {
			ser = ser ?? new JavaScriptSerializer();
			rankinglists = rankinglists ?? new Dictionary<string, object>();
			rankinglists[KEY_DIGMAP] = digmap;
			rankinglists[KEY_PLACEMAP] = placemap;
			rankinglists[KEY_KILLMAP] = killmap;
			rankinglists[KEY_DIEMAP] = diemap;
			RANKINGS = ser.Serialize(rankinglists);
		}
		
		/// <summary>
		/// 榜单数据
		/// </summary>
		public class AData {
			public string name;
			public int score;
		}
		
		// 根据获取关键字对应的指定表
		public static Dictionary<string, object> getMap(string key) {
			Dictionary<string, object> map = null;
			switch(key) {
				case KEY_DIGMAP:
					map = digmap;
					break;
				case KEY_PLACEMAP:
					map = placemap;
					break;
				case KEY_KILLMAP:
					map = killmap;
					break;
				case KEY_DIEMAP:
					map = diemap;
					break;
				default:
					// do nothing
					map = null;
					break;
			}
			if (map == null) {
				object om;
				if (rankinglists.TryGetValue(key, out om))
					map = (Dictionary<string, object>)om;
			}
			return map;
		}
		
		public static Hashtable oldRanklists = new Hashtable();	// 旧榜单
		public static Hashtable oldRankTimes = new Hashtable();	// 上次计算榜单时间
		
		/// <summary>
		/// 排名显示数据
		/// </summary>
		public class RankViewData {
			/// <summary>
			/// 排序前十名表
			/// </summary>
			public string rankinglist;
			/// <summary>
			/// 是否需要追加显示
			/// </summary>
			public bool needmodify;
			/// <summary>
			/// 追加显示用的指令
			/// </summary>
			public string modifycmd;
		}
		
		// 在已有前十排行榜里添加自己的信息
		static string findOrAddColor(AData[] clist, AData x) {
			if (clist == null)
				return "";
			bool finded = false;
			var ranklst = new ArrayList();
			for (int i = 0, l = clist.Length; i < 10 && i < l; i++) {
				var d = clist[i];
				var nd = new AData(){name = d.name, score = d.score};
				if (x != null) {
					if (d.name == x.name) {
						finded = true;
						nd.name = "§d" + nd.name;
					}
				}
				ranklst.Add("" + nd.name + ", §e" + nd.score);
			}
			if (!finded) {
				if (x != null) {
					ranklst.Add("§d" + x.name + ", §e" + x.score);
				}
			}
			return ser.Serialize(ranklst);;
		}
		
		// 表转榜单
		public static string makeRankingList(string key, string pname) {
			Dictionary<string, object> map = getMap(key);
			if (map == null)
				return null;
			if (oldRankTimes[key] != null) {
				DateTime ot = (DateTime)oldRankTimes[key];
				if (ot.AddSeconds(5) > DateTime.Now)	// 未超时，使用临时榜单
				{
					var psc = map[pname];
					var oldstr = oldRanklists[key] as AData[];
					if (psc != null)
						return findOrAddColor(oldstr, new AData(){name = pname, score = Convert.ToInt32(psc)});
					return findOrAddColor(oldstr, null);
				}
			}
			ArrayList al = new ArrayList();
			AData p = null;
			lock(map) {
				foreach(string k in map.Keys) {
					AData d = new AData(){name = k, score = Convert.ToInt32(map[k])};
					al.Add(d);
					if (k == pname)
						p = d;
				}
			}
			AData[] rls = (AData[])al.ToArray(typeof(AData));
			Array.Sort(rls, (x, y) => y.score - x.score);
			if (rls.Length > 0) {
				oldRanklists[key] = rls;
				oldRankTimes[key] = DateTime.Now;
				return findOrAddColor(rls, p);
			}
			return "";
		}
		
		// 增加指定表中一个分数
		public static void addCount(string key, string name) {
			Dictionary<string, object> map = getMap(key);
			if (map != null) {
				lock(map){
					object os;
					int score;
					score = map.TryGetValue(name, out os) ? Convert.ToInt32(os) : 0;
					++score;
					map[name] = score;
				}
			}
		}
		
		public static Hashtable dietms = new Hashtable();	// 死亡时间记录
		// 检测是否重复死亡
		public static bool checkdielistOK(IntPtr ptr) {
			if (dietms[ptr] == null) {
				if (dietms.Count > 5000)
					dietms.Clear();
				dietms[ptr] = DateTime.Now;
				return true;
			} else {
				DateTime dt = (DateTime)dietms[ptr];
				if (dt.AddSeconds(5) < DateTime.Now) {	// 已超时
					if (dietms.Count > 5000)
						dietms.Clear();
					dietms[ptr] = DateTime.Now;
					return true;
				}
			}
			return false;
		}
		
		public static Hashtable rankTasks = new Hashtable();	// 排行榜任务列表
		
		public static readonly string[] TITLES = {"--- 挖掘榜 ---", "~~~ 放置榜 ~~~", "*** 击杀榜 ***", "=== 阵亡榜 ==="};
		public static readonly string[] KEYSETS = {KEY_DIGMAP, KEY_PLACEMAP, KEY_KILLMAP, KEY_DIEMAP};
		
		public class RankThread {
			IntPtr mp;
			Thread t;
			bool flag;
			int startbar;
			long msec;
			public void start(IntPtr p) {
				mp = p;
				flag = true;
				startbar = 0;
				msec = 0;
				t = new Thread(() => {
				               	while (flag) {
				               		// TODO 此处设置玩家侧边栏
				               		startbar = startbar % 4;				// 总计4项榜单
				               		string title = TITLES[startbar];
				               		try {
										var le = players[p] as LoadNameEvent;
										if (le == null)
											return;
										var name = le.playername;
										var uuid = le.uuid;
										var str = makeRankingList(KEYSETS[startbar], name);
										mapi.setPlayerSidebar(uuid, title, str);
				               		}catch (AccessViolationException) {
				               			Console.WriteLine("An AccessViolationException err, exit task.");
				               			return;	// 发生指针读取异常时，直接结束任务
				               		}catch (InvalidOperationException) {
				               			Console.WriteLine("An InvalidOperationException err.");
				               			//return; // 发生数据读取异常时，跳过本次任务
				               		}
				               		Thread.Sleep(5000);
				               		msec += 5000;
				               		if (msec > 30000) {
				               			msec -= 30000;
				               			++startbar;
				               		}
				               	}
				               });
				t.Start();
			}
			public void switchToAnother() {
				++startbar;
			}
			public void stop() {
				flag = false;
			}
		}
		
		// 开始一个排行榜显示任务，每隔5秒更新一次侧边栏，每隔30秒切换一次榜单类型
		public static void startRankTask(IntPtr p) {
			if (rankTasks[p] != null) {
				RankThread rt = (RankThread)rankTasks[p];
				rt.switchToAnother();
				return;
			}
			RankThread r = new RankThread();
			rankTasks[p] = r;
			r.start(p);
		}
		// 结束一个排行榜显示任务
		public static void stopRankTask(IntPtr p) {
			if (rankTasks[p] != null) {
				RankThread rt = (RankThread)rankTasks[p];
				rt.stop();
				rankTasks.Remove(p);
			}
		}
		
		// 开机自启任务，每隔一分钟保存一次榜单
		public static void autoStartSaveData() {
			new Thread(() => {
			           	while (true) {
			           		Thread.Sleep(60000);
			           		saveData();
			           	}
			           }).Start();
		}
		
		// 放置方块标签
		static Hashtable placedflag = new Hashtable();
		
		// 主入口实现
		public static void init(MCCSAPI api) {
			mapi = api;
			// 此处载入初始化配置文件
			Console.WriteLine("[Ranking]即将载入配置文件，位置位于 {0}", RANKINGCONFIG);
			reloadData();
			api.addAfterActListener(EventKey.onDestroyBlock, x => {
			                        	// TODO 破坏方块记录操作
			                        	var e = BaseEvent.getFrom(x) as DestroyBlockEvent;
			                        	if (e != null) {
			                        		addCount(KEY_DIGMAP, e.playername);
			                        	}
			                        	return true;
			                        });
			api.addAfterActListener(EventKey.onPlacedBlock, x => {
			                        	// TODO 放置方块记录操作
			                        	var e = BaseEvent.getFrom(x) as PlacedBlockEvent;
			                        	if (e != null && e.RESULT) {
			                        		placedflag[e.playerPtr] = true;
			                        	}
			                        	return true;
			                        });
			api.addAfterActListener(EventKey.onUseItem, x => {
			                        	// TODO 放置方块记录操作2
			                        	var e = BaseEvent.getFrom(x) as UseItemEvent;
			                        	if (e != null && e.RESULT) {
			                        		if (placedflag[e.playerPtr] != null) {
			                        			addCount(KEY_PLACEMAP, e.playername);
			                        			placedflag.Remove(e.playerPtr);
			                        		}
			                        	}
			                        	return true;
			                        });
			api.addAfterActListener(EventKey.onMobDie, x => {
			                        	// TODO 生物死亡事件记录操作，可能出现重复情况
			                        	var e = BaseEvent.getFrom(x) as MobDieEvent;
			                        	if (e != null) {
			                        		if (e.srctype == "entity.player.name") {
			                        			if (!string.IsNullOrEmpty(e.srcname)) {
			                        				if (checkdielistOK(e.mobPtr))
			                        					addCount(KEY_KILLMAP, e.srcname);
			                        			}
			                        		}
			                        		if (e.mobtype == "entity.player.name") {
			                        			addCount(KEY_DIEMAP, e.playername);
			                        		}
			                        	}
			                        	return true;
			                        });
			api.addBeforeActListener(EventKey.onInputCommand, x => {
			                         	// TODO 玩家输入指令监听
			                         	var e = BaseEvent.getFrom(x) as InputCommandEvent;
			                         	if (e != null) {
			                         		string icmd = e.cmd.Trim();
			                         		if (icmd == "/ranking") {
			                         			// TODO 开启循环播报侧边栏
			                         			startRankTask(e.playerPtr);
			                         			return false;
			                         		} else if (icmd == "/ranking hide") {
			                         			// TODO 关闭侧边栏显示
			                         			stopRankTask(e.playerPtr);
			                         			var le = players[e.playerPtr] as LoadNameEvent;
			                         			if (le != null)
				                         			api.removePlayerSidebar(le.uuid);
			                         			return false;
			                         		}
			                         	}
			                         	return true;
			                         });
			api.addAfterActListener(EventKey.onLoadName, x => {
			                        	// 玩家进入游戏监听，记录信息
			                        	var e = BaseEvent.getFrom(x) as LoadNameEvent;
			                        	if (e != null) {
			                        		players[e.playerPtr] = e;
			                        	}
			                        	return true;
			                        });
			api.addBeforeActListener(EventKey.onPlayerLeft, x => {
			                        	// TODO 玩家离开游戏监听
			                        	var e = BaseEvent.getFrom(x) as PlayerLeftEvent;
			                        	if (e != null) {
			                        		players.Remove(e.playerPtr);
			                        		stopRankTask(e.playerPtr);
			                        	}
			                        	return true;
			                        });
			api.setCommandDescribe("ranking", "显示排行榜");
			api.setCommandDescribe("ranking hide", "隐藏排行榜");
			autoStartSaveData();
		}
	}
}

namespace CSR {
	partial class Plugin {
		
		/// <summary>
		/// 通用调用接口，需用户自行实现
		/// </summary>
		/// <param name="api">MC相关调用方法</param>
		public static void onStart(MCCSAPI api) {
			// TODO 此接口为必要实现
			if (api.COMMERCIAL) {
				CSRRanking.Ranking.init(api);
				Console.WriteLine("[Ranking] 排行榜插件已装载。");
			} else {
				Console.WriteLine("[Ranking] 暂不适用于社区版。");
			}
		}
	}
}