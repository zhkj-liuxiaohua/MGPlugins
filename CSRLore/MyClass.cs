/*
 * 由SharpDevelop创建。
 * 用户： BDSNetRunner
 * 日期: 2020/12/7
 * 时间: 19:31
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

namespace CSRLore
{
	/// <summary>
	/// 自定义Lore注释
	/// </summary>
	public static class MyClass
	{
		static MCCSAPI mapi;
		
		const string CONFIG_DIR = @"CSR/lore";
		const string CONFIG_PATH = @"CSR/lore/loreconfig.json";
		
		static Dictionary<string, object> config = new Dictionary<string, object>();
		static string costtype = "level";
		static string costsbname = "money";
		static int costcount = 1;
		static string tipsok = "[提示] 您的装备已添加注释成功。";
		static string tipserr = "[出错] 装备添加注释失败。请检查命令是否正确，装备是否符合品级。";
		static string tipscostly = "[失败] 过于昂贵，添加注释失败";
		static string tipshelp = "[帮助] 输入 lore [注释信息] 指令，消耗一定经验或金钱以添加您装备的注释。";
		static string tipshelpex = "[帮助] 输入 loreraw [JSON] 指令，消耗一定经验或金钱以添加您装备的注释。\n注：JSON中，信息集应包含在texts关键字对应的集合中。";
		static ArrayList applyitems = new ArrayList();
		
		static LoreEditForm lform;
		
		static readonly JavaScriptSerializer ser = new JavaScriptSerializer();
		
		const string djsonstr = "{\"cost\": \"level\",\n" +
  "\"costname\": \"money\",\n" +
  "\"count\": 1,\n" +
  "\"tips\": {\n" +
    "\"ok\": \"[提示] 您的装备已添加注释成功。\",\n"+
    "\"error\": \"[出错] 装备添加注释失败。请检查命令是否正确，装备是否符合品级。\",\n"+
    "\"costly\": \"[失败] 过于昂贵，添加注释失败\",\n"+
    "\"help\": \"[帮助] 输入 lore [注释信息] 指令，消耗一定经验或金钱以添加您装备的注释。\",\n"+
	"\"helpex\": \"[帮助] 输入 loreraw [JSON] 指令，消耗一定经验或金钱以添加您装备的注释。\\n注：JSON中，信息集应包含在texts关键字对应的集合中。\"\n" +
  "},\n"+
  "\"applyitems\": [\n"+
	"\"minecraft:diamond_sword\",\n" +
	"\"minecraft:netherite_sword\",\n" +
	"\"minecraft:shield\",\n" +
	"\"minecraft:elytra\"\n" +
  "]\n"+
"}";
		
		// 检查配置文件完整性
		static bool checkConfigOK() {
			object ot;
			if (config.TryGetValue("cost", out ot)) {
				costtype = ot as string;
				if (costtype == null)
					return false;
				costtype = costtype.ToLower() == "level" ? "level" : "scoreboard";
				object ona;
				if (config.TryGetValue("costname", out ona)) {
					costsbname = ona as string;
					if (costsbname == null)
						return false;
					object occ;
					if (config.TryGetValue("count", out occ)) {
						costcount = Convert.ToInt32(occ);
						object otips;
						if (config.TryGetValue("tips", out otips)) {
							var tips = otips as Dictionary<string, object>;
							if (tips == null)
								return false;
							object otok;
							if (tips.TryGetValue("ok", out otok)) {
								tipsok = otok as string;
								object oterr;
								if (tips.TryGetValue("error", out oterr)) {
									tipserr = oterr as string;
									object otcostly;
									if (tips.TryGetValue("costly", out otcostly)) {
										tipscostly = otcostly as string;
										object othelp;
										if (tips.TryGetValue("help", out othelp)) {
											tipshelp = othelp as string;
											object othelpex;
											if (tips.TryGetValue("helpex", out othelpex)) {
												tipshelpex = othelpex as string;
												object oits;
												if (config.TryGetValue("applyitems", out oits)) {
													applyitems = oits as ArrayList;
													return applyitems != null;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return false;
		}
		
		static bool loadconfig() {
			try {
				Directory.CreateDirectory(CONFIG_DIR);
				config = ser.Deserialize<Dictionary<string, object>>(File.ReadAllText(CONFIG_PATH));
			}catch{}
			if (config == null || config.Count < 1 || !checkConfigOK()) {
				Console.WriteLine("[Lore] 配置文件读取失败，使用默认配置。");
				try {
					config = ser.Deserialize<Dictionary<string, object>>(djsonstr);
					Directory.CreateDirectory(CONFIG_DIR);
					File.WriteAllText(CONFIG_PATH, djsonstr);
				}catch{}
				return checkConfigOK();
			}
			return true;
		}
		
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
		
		// 检查是否可支付注释费用
		static bool checkCanpay(CsPlayer p) {
			bool can = false;
			switch(costtype) {
				case "level":
					try {
							var plvl = ser.Deserialize<Dictionary<string, object>>(mapi.getPlayerAttributes(p.Uuid));
							can = (Convert.ToInt32(plvl["level"])) - costcount >= 0;
					} catch{}
					break;
				case "scoreboard":
					try {
						int d = mapi.getscoreboard(p.Uuid, costsbname);
						can = d - costcount >= 0;
					}catch{}
					break;
			}
			return can;
		}
		// 支付等级或计分板数值
		static bool pay(CsPlayer p) {
			bool can = false;
			switch(costtype) {
				case "level":
					try {
							var plvl = ser.Deserialize<Dictionary<string, object>>(mapi.getPlayerAttributes(p.Uuid));
							can = (Convert.ToInt32(plvl["level"])) - costcount >= 0;
							if (can) {
								var lvl = new Dictionary<string, object>();
								lvl["level"] = (Convert.ToInt32(plvl["level"])) - costcount;
								return mapi.setPlayerTempAttributes(p.Uuid, ser.Serialize(lvl));
							}
					} catch{}
					break;
				case "scoreboard":
					try {
						int d = mapi.getscoreboard(p.Uuid, costsbname);
						int nd = d - costcount;
						can = nd >= 0;
						if (can) {
							return mapi.runcmd("scoreboard players set \"" + p.getName() + "\" \"" + costsbname + "\" " + nd);
						}
					}catch{}
					break;
			}
			return can;
		}
		
		/// <summary>
		/// 给玩家当前手持装备添加注释
		/// </summary>
		/// <param name="p">玩家类的实例</param>
		/// <param name="txts">待注释文本集合</param>
		/// <returns></returns>
		public static int lore(CsPlayer p, string [] txts) {
			// TODO 此处检查装备是否允许添加注释
			int retcode = 0;		// 返回值：0 - err，1 - ok，2 - too costly
			Dictionary<string, object> curitem = null;
			try {
				curitem = ser.Deserialize<Dictionary<string, object>>(mapi.getPlayerSelectedItem(p.Uuid));
			} catch {}
			if (curitem != null && curitem.Count > 0) {
				bool isitemok = false;
				try {
					var data = LoreApi.getOrAppand<Dictionary<string, object>>(curitem, "selecteditem", "", "");
					string nid = LoreApi.getItemType(data);
					isitemok = !string.IsNullOrEmpty(nid) && 
								(applyitems.Count < 1 || (applyitems.Count > 0 && applyitems.Contains(nid)));
				} catch {}
				if (!isitemok) {
					return 0;
				}
				// TODO 此处进行扣分检查操作
				if (!checkCanpay(p)) {
					return 2;
				}
				// TODO 此处进行注释操作，成功后进行扣分操作
				bool loreok = false;
				try {
					var data = LoreApi.getOrAppand<Dictionary<string, object>>(curitem, "selecteditem", "", "");
					// test
//					var oldlore = LoreApi.getLores(data);
//					if (oldlore != null) {
//						Console.WriteLine("[LOG] old lores =" + ser.Serialize(oldlore));
//					} else {
//						Console.WriteLine("[LOG] null old lores.");
//					}
					// test end
					loreok = LoreApi.setLores(data, txts);
				} catch {}
				if (loreok) {
					if (pay(p)) {
						var data = LoreApi.getOrAppand<Dictionary<string, object>>(curitem, "selecteditem", "", "");
						int slot = Convert.ToInt32(curitem["selectedslot"]);
						var nal = LoreApi.getOrAppand<ArrayList>(data, "tv", "tt", 10);
						var slotdata = LoreApi.findOrAppandCK<Dictionary<string, object>>(nal, "Slot");	// 添加背包中Slot标签以符合背包列表格式
						slotdata["tt"] = 1;
						slotdata["tv"] = slot;
						var nitmdata = new Dictionary<string, object>();
						var d = LoreApi.getOrAppand<ArrayList>(nitmdata, "tv", "tt", 9);
						d.Add(data);
						var nitms = new Dictionary<string, object>();
						nitms["Inventory"] = nitmdata;
						if (mapi.setPlayerItems(p.Uuid, ser.Serialize(nitms))) {
							retcode = 1;
						}
					}
				}
			}
			return retcode;
		}
		
		// 根据返回码打印提示消息
		static void tips(CsPlayer p, int code) {
			switch(code) {
				case 0:
					tellraw(p.getName(), tipserr);
					break;
				case 1:
					tellraw(p.getName(), tipsok);
					break;
				case 2:
					tellraw(p.getName(), tipscostly);
					break;
				default:
					// do nothing
					break;
			}
		}
		
		// 插件主入口
		public static void init(MCCSAPI api) {
			mapi = api;
			if (!loadconfig()) {
				Console.WriteLine("[Lore] 配置文件读写失败，请检查插件是否配置正确。");
				return;
			}
			api.addBeforeActListener(EventKey.onInputCommand, x => {
			                         	var e = BaseEvent.getFrom(x) as InputCommandEvent;
			                         	var mcmd = e.cmd.Trim();
			                         	if (string.IsNullOrEmpty(mcmd))
			                         		return false;
			                         	if (mcmd == "/lore") {
			                         		tellraw(e.playername, tipshelp);
			                         		return false;
			                         	}
			                         	if (mcmd.IndexOf("/lore ") == 0) {
			                         		CsPlayer p = new CsPlayer(api, e.playerPtr);
			                         		int infocode = 0;
			                         		var txt = mcmd.Substring(6);
			                         		if (!string.IsNullOrEmpty(txt)) {
			                         			// TODO 此处检查装备是否允许添加注释
			                         			txt = "\n" + txt;
			                         			infocode = lore(p, new string[] {txt});
			                         		}
			                         		tips(p, infocode);
			                         		return false;
			                         	}
			                         	if (mcmd == "/loreraw") {
			                         		tellraw(e.playername, tipshelpex);
			                         		return false;
			                         	}
			                         	if (mcmd.IndexOf("/loreraw ") == 0) {
			                         		CsPlayer p = new CsPlayer(api, e.playerPtr);
			                         		int infocode = 0;
			                         		var txts = new ArrayList();
			                         		var jsonstr = mcmd.Substring(9).Trim();
			                         		try {
			                         			var data = ser.Deserialize<Dictionary<string, object>>(jsonstr);
			                         			if (data != null) {
			                         				object ot;
			                         				if (data.TryGetValue("texts", out ot)) {
			                         					txts = ot as ArrayList;
			                         				}
			                         			}
			                         		} catch{}
			                         		if (txts != null) {
			                         			// TODO 此处检查装备是否允许添加注释
			                         			try {
			                         				infocode = lore(p, (string[])txts.ToArray(typeof(string)));
			                         			}catch{}
			                         		}
			                         		tips(p, infocode);
			                         		return false;
			                         	}
			                         	return true;
			                         });
			api.addBeforeActListener(EventKey.onServerCmd, x => {
				var e = BaseEvent.getFrom(x) as ServerCmdEvent;
				var mcmd = e.cmd.Trim();
				if (mcmd == "lore") {
					api.logout("[Lore] 用法（玩家）： /lore [说明]，/loreraw [说明集JSON]，（后台）：lore [reload|edit]");
					return false;
				}
				if (mcmd.IndexOf("lore ") == 0) {
					var para = mcmd.Substring(5);
					if (!string.IsNullOrEmpty(para)) {
						if (para.Trim().ToLower() == "reload") {
							// TODO 此处重载配置文件
							if (!loadconfig()) {
								api.logout("[Lore] 配置文件读写失败，请检查插件是否配置正确。");
							} else {
								api.logout("[Lore] 已成功重载配置文件。");
							}
						} else if (para.Trim().ToLower() == "edit") {
							api.logout("[Lore] >> 即将进入windows窗口界面编辑环境 <<");
							new Thread(() => {
								try {
									if (lform == null) {
										lform = new LoreEditForm();
										lform.setDataAndListeners(config, new LoreEditForm.OnBtCb() {
											onBtOk = (data) => {
												try {
													config = data;
													Directory.CreateDirectory(CONFIG_DIR);
													File.WriteAllText(CONFIG_PATH, ser.Serialize(config));
												} catch {}
												if (loadconfig()) {
													api.logout("[Lore] 配置文件已更新。");
												}
												lform.Close();
												lform = null;
											},
											onBtCancel = () => {
												api.logout("[Lore] 已取消配置文件表单。");
												lform.Close();
												lform = null;
											}
										});
										lform.TopMost = true;
										System.Windows.Forms.Application.Run(lform);
									} else {
										if (!lform.Visible) {
											lform.Visible = true;
											lform.Show();
										}
									}	
								} catch {}
							}).Start();
						}
					}
					return false;
				}
				return true;
			});
			api.setCommandDescribe("lore", "自定义装备注释");
			api.setCommandDescribe("loreraw", "自定义装备注释(JSON格式)");
			Console.WriteLine("[Lore] 自定义装备注释指令已加载。用法（玩家）： /lore [说明]，/loreraw [说明集JSON]，（后台）：lore [reload|edit]");
		}
	}
}

namespace CSR {
	partial class Plugin{
		public static void onStart(MCCSAPI api) {
			if (api.COMMERCIAL) {
				CSRLore.MyClass.init(api);
			} else {
				Console.WriteLine("[Lore] 暂不支持社区版。");
			}
		}
	}
}