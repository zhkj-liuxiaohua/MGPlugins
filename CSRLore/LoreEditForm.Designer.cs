/*
 * 由SharpDevelop创建。
 * 用户： BDSNetRunner
 * 日期: 12/20/2020
 * 时间: 04:01
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
namespace CSRLore
{
	partial class LoreEditForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button btok;
		private System.Windows.Forms.Button btcancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton btlevel;
		private System.Windows.Forms.RadioButton btscore;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox scorename;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox count;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tipserr;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tipsok;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tipshelp;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox items;
		private System.Windows.Forms.TextBox tipscostly;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox tipshelpex;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.btok = new System.Windows.Forms.Button();
			this.btcancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btlevel = new System.Windows.Forms.RadioButton();
			this.btscore = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.scorename = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.count = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tipshelpex = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.tipshelp = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.tipscostly = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.tipserr = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tipsok = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.items = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btok
			// 
			this.btok.Location = new System.Drawing.Point(247, 518);
			this.btok.Name = "btok";
			this.btok.Size = new System.Drawing.Size(75, 23);
			this.btok.TabIndex = 0;
			this.btok.Text = "确定";
			this.btok.UseVisualStyleBackColor = true;
			this.btok.Click += new System.EventHandler(this.BtokClick);
			// 
			// btcancel
			// 
			this.btcancel.Location = new System.Drawing.Point(397, 518);
			this.btcancel.Name = "btcancel";
			this.btcancel.Size = new System.Drawing.Size(75, 23);
			this.btcancel.TabIndex = 0;
			this.btcancel.Text = "取消";
			this.btcancel.UseVisualStyleBackColor = true;
			this.btcancel.Click += new System.EventHandler(this.BtcancelClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(133, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "消耗项：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btlevel
			// 
			this.btlevel.Checked = true;
			this.btlevel.Location = new System.Drawing.Point(177, 8);
			this.btlevel.Name = "btlevel";
			this.btlevel.Size = new System.Drawing.Size(116, 24);
			this.btlevel.TabIndex = 2;
			this.btlevel.TabStop = true;
			this.btlevel.Text = "等级";
			this.btlevel.UseVisualStyleBackColor = true;
			// 
			// btscore
			// 
			this.btscore.Location = new System.Drawing.Point(299, 8);
			this.btscore.Name = "btscore";
			this.btscore.Size = new System.Drawing.Size(104, 24);
			this.btscore.TabIndex = 2;
			this.btscore.Text = "计分板";
			this.btscore.UseVisualStyleBackColor = true;
			this.btscore.CheckedChanged += new System.EventHandler(this.onScChecked);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(133, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "计分板名称：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// scorename
			// 
			this.scorename.Enabled = false;
			this.scorename.Location = new System.Drawing.Point(167, 38);
			this.scorename.Name = "scorename";
			this.scorename.Size = new System.Drawing.Size(236, 25);
			this.scorename.TabIndex = 3;
			this.scorename.Text = "money";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 75);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(133, 23);
			this.label3.TabIndex = 1;
			this.label3.Text = "消耗值：";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// count
			// 
			this.count.Location = new System.Drawing.Point(167, 73);
			this.count.Name = "count";
			this.count.Size = new System.Drawing.Size(236, 25);
			this.count.TabIndex = 3;
			this.count.Text = "1";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tipshelpex);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.tipshelp);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.tipscostly);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.tipserr);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.tipsok);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(12, 104);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(758, 290);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "命令提示";
			// 
			// tipshelpex
			// 
			this.tipshelpex.Location = new System.Drawing.Point(155, 236);
			this.tipshelpex.Multiline = true;
			this.tipshelpex.Name = "tipshelpex";
			this.tipshelpex.Size = new System.Drawing.Size(597, 47);
			this.tipshelpex.TabIndex = 1;
			this.tipshelpex.Text = "[帮助] 输入 loreraw [JSON] 指令，消耗一定经验或金钱以添加您装备的注释。\r\n注：JSON中，信息集应包含在texts关键字对应的集合中。";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(21, 239);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(100, 23);
			this.label9.TabIndex = 0;
			this.label9.Text = "ex指令帮助：";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tipshelp
			// 
			this.tipshelp.Location = new System.Drawing.Point(155, 183);
			this.tipshelp.Multiline = true;
			this.tipshelp.Name = "tipshelp";
			this.tipshelp.Size = new System.Drawing.Size(597, 47);
			this.tipshelp.TabIndex = 1;
			this.tipshelp.Text = "[帮助] 输入 lore [注释信息] 指令，消耗一定经验或金钱以添加您装备的注释。";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(21, 186);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 23);
			this.label7.TabIndex = 0;
			this.label7.Text = "指令帮助：";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tipscostly
			// 
			this.tipscostly.Location = new System.Drawing.Point(155, 130);
			this.tipscostly.Multiline = true;
			this.tipscostly.Name = "tipscostly";
			this.tipscostly.Size = new System.Drawing.Size(597, 47);
			this.tipscostly.TabIndex = 1;
			this.tipscostly.Text = "[失败] 过于昂贵，添加注释失败";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(21, 133);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 23);
			this.label6.TabIndex = 0;
			this.label6.Text = "过于昂贵：";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tipserr
			// 
			this.tipserr.Location = new System.Drawing.Point(155, 77);
			this.tipserr.Multiline = true;
			this.tipserr.Name = "tipserr";
			this.tipserr.Size = new System.Drawing.Size(597, 47);
			this.tipserr.TabIndex = 1;
			this.tipserr.Text = "[出错] 装备添加注释失败。请检查命令是否正确，装备是否符合品级。";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(21, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 23);
			this.label5.TabIndex = 0;
			this.label5.Text = "执行出错：";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tipsok
			// 
			this.tipsok.Location = new System.Drawing.Point(155, 24);
			this.tipsok.Multiline = true;
			this.tipsok.Name = "tipsok";
			this.tipsok.Size = new System.Drawing.Size(597, 47);
			this.tipsok.TabIndex = 1;
			this.tipsok.Text = "[提示] 您的装备已添加注释成功。";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(21, 27);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 23);
			this.label4.TabIndex = 0;
			this.label4.Text = "执行成功：";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(12, 409);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(133, 88);
			this.label8.TabIndex = 0;
			this.label8.Text = "适用物品列表：\r\n（注：一行一个，清空此列表，则表示适用所有非空物品）";
			// 
			// items
			// 
			this.items.Location = new System.Drawing.Point(167, 409);
			this.items.Multiline = true;
			this.items.Name = "items";
			this.items.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.items.Size = new System.Drawing.Size(597, 88);
			this.items.TabIndex = 1;
			this.items.Text = "minecraft:diamond_sword\r\nminecraft:netherite_sword\r\nminecraft:shield\r\nminecraft:e" +
	"lytra";
			// 
			// LoreEditForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(782, 553);
			this.Controls.Add(this.items);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.count);
			this.Controls.Add(this.scorename);
			this.Controls.Add(this.btscore);
			this.Controls.Add(this.btlevel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btcancel);
			this.Controls.Add(this.btok);
			this.Name = "LoreEditForm";
			this.Text = "Lore设置窗口";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
