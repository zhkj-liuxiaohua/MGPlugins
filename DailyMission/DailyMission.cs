/*
 * 由SharpDevelop创建。
 * 用户： classmates
 * 日期: 2021/4/19
 * 时间: 23:30
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;
using CSR;

namespace DailyMission
{
	/// <summary>
	/// 每日任务
	/// </summary>
	public class DailyMission
	{
		const string KEY_LOGIN = "LOGIN";
		const string KEY_KILL = "KILL";
		const string KEY_TIME = "TIME";
		const string KEY_COMPLETE = "COMPLETE";
		
		const string SB_NAME_CUP = "cup";
		
		const string MISSION_DIR = @"CSR/DailyMission";
		static readonly string MISSIONDATA = @"CSR\DailyMission\mdata.json";
		
		static JavaScriptSerializer ser = null;
		
		static MCCSAPI mapi = null;
		
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
		
		static Hashtable olpls = new Hashtable();
		static Hashtable olplinfos = new Hashtable();
		
		static Dictionary<string, object> missondata = null;
		
		// 判断今日任务是否完成
		static bool checkMissionComplete(Dictionary<string, object> ddata, LoadNameEvent e) {
			if (ddata != null) {
				object oislogined, okilled, ooltime, ocompleted;
				ddata.TryGetValue(KEY_LOGIN, out oislogined);
				ddata.TryGetValue(KEY_KILL, out okilled);
				ddata.TryGetValue(KEY_TIME, out ooltime);
				ddata.TryGetValue(KEY_COMPLETE, out ocompleted);
				if (oislogined != null && (bool)oislogined) {
					if (okilled != null && Convert.ToInt32(okilled) >= 30) {
						if (ooltime != null && Convert.ToInt32(ooltime) >= 30) {
							if (ocompleted == null || !(bool)ocompleted) {
								ddata[KEY_COMPLETE] = true;
								// TODO 此处发放奖励并提示
								mapi.setscoreboard(e.uuid, SB_NAME_CUP,
								                   mapi.getscoreboard(e.uuid, SB_NAME_CUP) + 1);
								mapi.sendText(e.uuid, "[每日] §e恭喜，您今天的每日任务已全部完成！");
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		
		// 保存任务数据
		static bool saveMdata() {
			if (missondata != null && missondata.Count > 0) {
				try {
					var s = ser.Serialize(missondata);
					Directory.CreateDirectory(MISSION_DIR);
					File.WriteAllText(MISSIONDATA, s);
					return true;
				} catch{}
			}
			return false;
		}
		// 读取任务数据
		static bool loadMData() {
			try {
				Directory.CreateDirectory(MISSION_DIR);
				missondata = ser.Deserialize<Dictionary<string, object>>(File.ReadAllText(MISSIONDATA));
				return true;
			} catch{}
			return false;
		}
		
		// 取玩家对应当日任务数据
		static Dictionary<string, object> getPlayersMdata(string xuid) {
			object xd;
			missondata.TryGetValue(xuid, out xd);
			if (xd == null)
				missondata[xuid] = new Dictionary<string, object>();
			var pmdata = missondata[xuid] as Dictionary<string, object>;
			var day = DateTime.Now.Date.ToShortDateString();
			xd = null;
			pmdata.TryGetValue(day, out xd);
			if (xd == null)
				pmdata[day] = new Dictionary<string, object>();
			return pmdata[day] as Dictionary<string, object>;
		}
		
		// 完成每日登录任务
		public static void addMissonLogin(IntPtr p) {
			var e = olplinfos[p] as LoadNameEvent;
			if (e != null) {
				string xuid = e.xuid;
				var ddata = getPlayersMdata(xuid);
				if (ddata != null) {
					object oislogined = null;
					ddata.TryGetValue(KEY_LOGIN, out oislogined);
					if (oislogined == null || !(bool)oislogined) {
						ddata[KEY_LOGIN] = true;
						saveMdata();
					}
					if (checkMissionComplete(ddata, e))
						saveMdata();
				}
			}
		}
		
		static Hashtable timemissions = new Hashtable();
		
		// 增加每日在线时长一分
		public static bool addMissonTime(IntPtr p) {
			bool ret = false;
			var e = olplinfos[p] as LoadNameEvent;
			if (e != null) {
				string xuid = e.xuid;
				var ddata = getPlayersMdata(xuid);
				object ooltime = null;
				ddata.TryGetValue(KEY_TIME, out ooltime);
				if (ooltime == null)
					ddata[KEY_TIME] = 0;
				int oltime = Convert.ToInt32(ddata[KEY_TIME]);
				if (oltime < 30) {
					++oltime;
					ddata[KEY_TIME] = oltime;
					saveMdata();
					if (oltime % 10 == 0) {
						mapi.sendText(e.uuid, "[每日] 您已累计在线" + oltime + "分钟。");
					}
					ret = true;
				}
				if (checkMissionComplete(ddata, e))
					saveMdata();
			}
			return ret;
		}
		
		// 计时类
		class TimerMData {
			public DateTime stime;
			public IntPtr player;
			public Thread t;
			private bool stopedflag;
			public void start(IntPtr p) {
				player = p;
				stime = DateTime.Now;
				stopedflag = false;
				t = new Thread(() => {
				               	do {
				               		Thread.Sleep(10000);
				               		if (!stopedflag) {
				               			var dn = DateTime.Now;
				               			var d = dn.Subtract(stime);
				               			if (d.Seconds == 0) {
				               				var added = addMissonTime(p);
				               				if (!added)
				               					stopedflag = true;
											stime = dn;
				               			}
				               		}
				               	} while(!stopedflag);
				               });
				t.Start();
			}
			public void stop() {
				stopedflag = true;
			}
		}
		
		// 停止计时
		static void stopTimeListen(IntPtr p) {
			var tm = timemissions[p] as TimerMData;
			if (tm != null) {
				tm.stop();
				timemissions.Remove(p);
			}
		}
		// 开始计时
		static void startTimeListen(IntPtr p) {
			TimerMData tm = new TimerMData();
			timemissions[p] = tm;
			tm.start(p);
		}
		
		// 统计击杀分数
		static void addMissonKill(string pname) {
			var p = (IntPtr)olpls[pname];
			if (p != IntPtr.Zero) {
				var e = olplinfos[p] as LoadNameEvent;
				var ddata = getPlayersMdata(e.xuid);
				object okilled = null;
				ddata.TryGetValue(KEY_KILL, out okilled);
				if (okilled == null)
					ddata[KEY_KILL] = 0;
				int olkill = Convert.ToInt32(ddata[KEY_KILL]);
				if (olkill < 30) {
					++olkill;
					ddata[KEY_KILL] = olkill;
					saveMdata();
					if (olkill % 10 == 0) {
						mapi.sendText(e.uuid, "[每日] 您已累计获得" + olkill + "击杀分数。");
					}
				}
				if (checkMissionComplete(ddata, e))
					saveMdata();
			}
		}
		
		static Hashtable firstInGame = new Hashtable();
		// 每日任务进度提示
		static void missiontips(IntPtr p) {
			var e = olplinfos[p] as LoadNameEvent;
			if (e != null) {
				var ddata = getPlayersMdata(e.xuid);
				object oislogined, okilled, ooltime;
				ddata.TryGetValue(KEY_LOGIN, out oislogined);
				ddata.TryGetValue(KEY_KILL, out okilled);
				ddata.TryGetValue(KEY_TIME, out ooltime);
				bool islogined = oislogined != null ? (bool)oislogined : false;
				int killed = okilled != null ? Convert.ToInt32(okilled) : 0;
				int oltime = ooltime != null ? Convert.ToInt32(ooltime) : 0;
				mapi.sendText(e.uuid, "[每日] 您的每日任务完成进度：\n1. 每日登录（" + 
				              (islogined ? "已完成" : "未完成") + "）\n2. 每日击杀（" + killed +
				              "/30，" + (killed >= 30 ? "已完成" : "未完成") + "）\n3. 每日在线（" + oltime +
				              "/30，" + (oltime >= 30 ? "已完成" : "未完成") + "）");
			}
		}
		
		// 主入口
		public static void init(MCCSAPI api) {
			mapi = api;
			ser = new JavaScriptSerializer();
			ser.MaxJsonLength = 500 * 1024 * 1024;
			loadMData();
			missondata = missondata ?? new Dictionary<string, object>();
			api.addAfterActListener(EventKey.onLoadName, x => {
			                        	var e = BaseEvent.getFrom(x) as LoadNameEvent;
			                        	olpls[e.playername] = e.playerPtr;
			                        	olplinfos[e.playerPtr] = e;
			                        	addMissonLogin(e.playerPtr);
			                        	startTimeListen(e.playerPtr);
			                        	firstInGame[e.playerPtr] = true;
			                        	return true;
			                        });
			api.addAfterActListener(EventKey.onRespawn, x => {
			                        	var e = BaseEvent.getFrom(x) as RespawnEvent;
			                        	if (firstInGame[e.playerPtr] != null && (bool)firstInGame[e.playerPtr]) {
			                        		missiontips(e.playerPtr);
			                        		firstInGame[e.playerPtr] = false;
			                        	}
			                        	return true;
			                        });
			api.addAfterActListener(EventKey.onMobDie, x => {
			                        	var e = BaseEvent.getFrom(x) as MobDieEvent;
			                        	if (e.srctype == "entity.player.name") {
			                        		if (!string.IsNullOrEmpty(e.srcname)) {
			                        			if (checkdielistOK(e.mobPtr))
			                        				addMissonKill(e.srcname);
			                        		}
			                        	}
			                        	return true;
			                        });
			api.addBeforeActListener(EventKey.onPlayerLeft, x => {
			                         	var e = BaseEvent.getFrom(x) as PlayerLeftEvent;
			                         	olpls.Remove(e.playername);
			                         	olplinfos.Remove(e.playerPtr);
			                         	stopTimeListen(e.playerPtr);
			                         	firstInGame.Remove(e.playerPtr);
			                         	return true;
			                         });
			api.addBeforeActListener(EventKey.onInputCommand, x => {
			                        	var e = BaseEvent.getFrom(x) as InputCommandEvent;
			                        	var cmd = e.cmd.Trim();
			                        	if (cmd == "/mission") {
			                        		missiontips(e.playerPtr);
			                        		var linfo = olplinfos[e.playerPtr] as LoadNameEvent;
			                        		if (linfo != null) {
			                        			var uuid = linfo.uuid;
			                        			api.sendText(uuid, "[每日] 您已累计获得" + api.getscoreboard(uuid, SB_NAME_CUP) + "个每日之星。");
			                        		}
			                        		return false;
			                        	}
			                        	return true;
			                        });
			api.setCommandDescribe("mission", "查询当日的每日进度");
			Console.WriteLine("[DailyMission] 每日任务已加载。用法：/mission");
		}
	}
}

namespace CSR {
	partial class Plugin {
		public static void onStart(MCCSAPI api) {
			DailyMission.DailyMission.init(api);
		}
	}
}