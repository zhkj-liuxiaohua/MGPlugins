/*
 * 由SharpDevelop创建。
 * 用户： BDSNetRunner
 * 日期: 12/20/2020
 * 时间: 04:01
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CSRLore
{
	/// <summary>
	/// Lore配置文件编辑框
	/// </summary>
	public partial class LoreEditForm : Form
	{
		private OnBtCb cb;
		private Dictionary<string, object> cfg;
		
		public LoreEditForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
		}
		
		public void setDataAndListeners(Dictionary<string, object> cdata, OnBtCb mcb) {
			cb = mcb;
			cfg = cdata;
			
			if (cfg != null) {
				try {
					btlevel.Checked = cfg["cost"] as string == "level";
					btscore.Checked = !btlevel.Checked;
					scorename.Text = cfg["costname"] as string;
					count.Text = "" + Convert.ToInt32(cfg["count"]);
					var tip = cfg["tips"] as Dictionary<string, object>;
					tipsok.Text = tip["ok"] as string;
					tipserr.Text = tip["error"] as string;
					tipscostly.Text = tip["costly"] as string;
					tipshelp.Text = tip["help"] as string;
					var its = cfg["applyitems"] as ArrayList;
					if (its != null && its.Count > 0) {
						var sits = (string[])its.ToArray(typeof(string));
						var s = string.Join("\r\n", sits);
						items.Text = s;
					}
				} catch{}
			}
		}
		
		void BtcancelClick(object sender, EventArgs e)
		{
			if (cb != null) {
				if (cb.onBtCancel != null)
					cb.onBtCancel();
			}
		}
		void BtokClick(object sender, EventArgs e)
		{
			if (cb != null) {
				if (cb.onBtOk != null) {
					var od = new Dictionary<string, object>();
					od["cost"] = btlevel.Checked ? "level" : "scoreboard";
					od["costname"] = scorename.Text;
					od["count"] = int.Parse(count.Text);
					var tips = new Dictionary<string, object>();
					tips["ok"] = tipsok.Text.Replace("\r", "");
					tips["error"] = tipserr.Text.Replace("\r", "");
					tips["costly"] = tipscostly.Text.Replace("\r", "");
					tips["help"] = tipshelp.Text.Replace("\r", "");
					tips["helpex"] = tipshelpex.Text.Replace("\r", "");
					od["tips"] = tips;
					var al = new ArrayList(items.Text.Split('\n'));
					for (int i = al.Count; i > 0; --i) {
						var s = al[i - 1] as string;
						s = (s != null) ? s.Trim().Trim('\r') : s;
						if (string.IsNullOrEmpty(s)) {
							al.RemoveAt(i - 1);
						} else {
							al[i - 1] = s;
						}
					}
					od["applyitems"] = al;
					cb.onBtOk(od);
				}
			}
		}
		void onScChecked(object sender, EventArgs e)
		{
			scorename.Enabled = btscore.Checked;
		}
		/// <summary>
		/// 按钮回调
		/// </summary>
		public class OnBtCb {
			public delegate void ONBTOK(Dictionary<string, object> data);
			public delegate void ONBTCANCEL();
			/// <summary>
			/// 按下确定按钮后回调
			/// </summary>
			public ONBTOK onBtOk;
			/// <summary>
			/// 按下取消按钮回调
			/// </summary>
			public ONBTCANCEL onBtCancel;
		}
	}
}
