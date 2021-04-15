/*
 * 由SharpDevelop创建。
 * 用户： 梦之故里
 * 日期: 2021/2/17
 * 时间: 14:09
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CSR;
using System.Web.Script.Serialization;

namespace PlayerDataBackup
{
	/// <summary>
	/// 导入导出玩家数据
	/// </summary>
	public class MyClass
	{
		static MCCSAPI mapi;
		const string PLAYERDATAFILE = "players.json";
		static JavaScriptSerializer ser = new JavaScriptSerializer();
		
		public static void init(MCCSAPI api) {
			mapi = api;
			api.addBeforeActListener(EventKey.onServerCmd, x => {
			                        	var e = BaseEvent.getFrom(x) as ServerCmdEvent;
			                        	if (e != null) {
			                        		if (e.cmd.IndexOf("exportplayers") == 0) {
			                        			if (e.cmd == "exportplayers")
			                        			{
				                        			try {
				                        				File.WriteAllText(PLAYERDATAFILE, api.exportPlayersData());
				                        				api.logout("[PlayerDataBackup] 玩家数据已全部导出至" + PLAYERDATAFILE + "文件中。");
				                        			} catch{}
			                        			} else {
			                        				try {
			                        					string fname = e.cmd.Substring(14).Trim();
			                        					File.WriteAllText(fname, api.exportPlayersData());
				                        				api.logout("[PlayerDataBackup] 玩家数据已全部导出至" + fname + "文件中。");
			                        				} catch{}
			                        			}
			                        			return false;
			                        		}/* else if (e.cmd.IndexOf("importplayers") == 0) {
			                        			if (e.cmd == "importplayers") {
			                        				try {
			                        					if (api.importPlayersData(File.ReadAllText(PLAYERDATAFILE))) {
			                        						api.logout("[PlayerDataBackup] 玩家数据已从" + PLAYERDATAFILE + "文件中全部导出。");
			                        					}
			                        				}catch{}
			                        			} else {
			                        				try {
			                        					string fname = e.cmd.Substring(14).Trim();
			                        					string pdatstr = File.ReadAllText(fname);
			                        					var data = ser.Deserialize<Dictionary<string, object>>(pdatstr);
			                        					if (data != null) {
			                        						foreach(string k in data.Keys) {
			                        							var actornbt = data[k] as Dictionary<string, object>;
			                        							var attr = actornbt["tv"] as ArrayList;
			                        							foreach (Dictionary<string, object> attrck in attr) {
			                        								if ((attrck["ck"] as string) == "DimensionId") {
			                        									// 自动换至主世界
			                        									(attrck["cv"] as Dictionary<string, object>)["tv"] = 0;
			                        									break;
			                        								}
			                        							}
			                        						}
			                        						if (api.importPlayersData(ser.Serialize(data))) {
			                        							api.logout("[PlayerDataBackup] 玩家数据已从" + fname + "文件中全部导出并重置为主世界。");
			                        						}
			                        					}
			                        				} catch(Exception ex) {Console.WriteLine(ex.StackTrace);}
			                        			}
			                        			return false;
			                        		}
			                        		*/
			                        	}
			                        	return true;
			                        });
			Console.WriteLine("[PlayerDataBackup] 玩家数据备份已加载。用法：\n" +
			                  "\t导出玩家数据：（后台）exportplayers [Filename]");/* +
			                  "\t导入玩家数据：（后台）importplayers [Filename]");*/
		}
	}
}

namespace CSR {
	partial class Plugin{
		public static void onStart(MCCSAPI api) {
			if (api.COMMERCIAL) {
				PlayerDataBackup.MyClass.init(api);
			} else {
				Console.WriteLine("[PlayerDataBackup] 暂不适用于社区版。");
			}
		}
	}
}