/*
 * 由SharpDevelop创建。
 * 用户：BDSNetRunner
 * 日期: 2020/12/15
 * 时间: 9:32
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using CSR;

namespace SplashDamage
{
	/// <summary>
	/// 溅射伤害
	/// </summary>
	public class MyClass
	{
		static MCCSAPI mapi;
		static Hashtable swords = new Hashtable();
		static JavaScriptSerializer ser = new JavaScriptSerializer();
		
		// 高版本 用标识符检测
		static void initSwordIds() {
			swords["wooden_sword"] = true;
			swords["stone_sword"] = true;
			swords["iron_sword"] = true;
			swords["diamond_sword"] = true;
			swords["golden_sword"] = true;
			swords["netherite_sword"] = true;
		}
		
		public static void init(MCCSAPI api) {
			mapi = api;
			initSwordIds();
			api.addAfterActListener(EventKey.onAttack, x => {
				var e = BaseEvent.getFrom(x) as AttackEvent;
				if (e != null) {
					CsPlayer p = new CsPlayer(api, e.playerPtr);
					CsActor a = new CsActor(api, e.attackedentityPtr);
					var hand = ser.Deserialize<ArrayList>(p.HandContainer);
					if (hand != null && hand.Count > 0) {
						var mainhand = hand[0] as Dictionary<string, object>;
						if (mainhand != null) {
							object oid;
							if (mainhand.TryGetValue("rawnameid", out oid)) {
								string rid = oid as string;		// 剑
								var oisSword = swords[rid];
								if (oisSword != null && (bool)oisSword) {
									// TODO 此处执行溅射伤害操作
									var pdata = a.Position;
									var aXYZ = ser.Deserialize<Vec3>(a.Position);
									var list = CsActor.getsFromAABB(api, a.DimensionId, aXYZ.x - 2, aXYZ.y - 1, aXYZ.z - 2,
										                               aXYZ.x + 2, aXYZ.y + 1, aXYZ.z + 2);
									if (list != null && list.Count > 0) {
										int count = 0;
										foreach (IntPtr aptr in list) {
											if (aptr != e.attackedentityPtr) {
												CsActor spa = new CsActor(api, aptr);
												if (((spa.TypeId & 0x100) == 0x100)) {	// 具有Mob标识
													spa.hurt(e.playerPtr, ActorDamageCause.EntityAttack, 1, true, false);
													++count;
												}
											}
											if (count >= 10) {	// 溅射最大个数：10
												break;
											}
										}
									}
								}
							}
						}
					}                		
				}
				return true;
			});
			Console.WriteLine("[SplashDamage] 武器溅射伤害已应用。");
		}
	}
}

namespace CSR {
	partial class Plugin {

		#region 必要接口 onStart ，由用户实现
		public static void onStart(MCCSAPI api) {
				SplashDamage.MyClass.init(api);
		}
		#endregion
	}
}