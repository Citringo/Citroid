namespace CitroidGui
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.label1 = new System.Windows.Forms.Label();
			this.button3 = new System.Windows.Forms.Button();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.button4 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.Color.Red;
			this.button1.ForeColor = System.Drawing.Color.White;
			this.button1.Location = new System.Drawing.Point(55, 129);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(225, 36);
			this.button1.TabIndex = 0;
			this.button1.Text = "記憶を消す";
			this.button1.UseVisualStyleBackColor = false;
			// 
			// button2
			// 
			this.button2.BackColor = System.Drawing.Color.DeepSkyBlue;
			this.button2.Location = new System.Drawing.Point(286, 54);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(144, 69);
			this.button2.TabIndex = 0;
			this.button2.Text = "記憶させる";
			this.button2.UseVisualStyleBackColor = false;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(12, 12);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(268, 111);
			this.textBox1.TabIndex = 1;
			// 
			// treeView1
			// 
			this.treeView1.Location = new System.Drawing.Point(12, 171);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(268, 163);
			this.treeView1.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 156);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "脳みそ";
			// 
			// button3
			// 
			this.button3.BackColor = System.Drawing.Color.Transparent;
			this.button3.Location = new System.Drawing.Point(286, 272);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(144, 62);
			this.button3.TabIndex = 4;
			this.button3.Text = "スキャン";
			this.button3.UseVisualStyleBackColor = false;
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.BackColor = System.Drawing.Color.Transparent;
			this.checkBox1.ForeColor = System.Drawing.Color.White;
			this.checkBox1.Location = new System.Drawing.Point(282, 171);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(78, 16);
			this.checkBox1.TabIndex = 5;
			this.checkBox1.Text = "リプライのみ";
			this.checkBox1.UseVisualStyleBackColor = false;
			// 
			// trackBar1
			// 
			this.trackBar1.BackColor = System.Drawing.Color.White;
			this.trackBar1.LargeChange = 10;
			this.trackBar1.Location = new System.Drawing.Point(311, 221);
			this.trackBar1.Maximum = 100;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(104, 45);
			this.trackBar1.TabIndex = 6;
			this.trackBar1.TickFrequency = 10;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.ForeColor = System.Drawing.Color.White;
			this.label2.Location = new System.Drawing.Point(294, 206);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "反応しやすさ";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Location = new System.Drawing.Point(413, 231);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 12);
			this.label3.TabIndex = 3;
			this.label3.Text = "100%";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.ForeColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(288, 231);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(17, 12);
			this.label4.TabIndex = 3;
			this.label4.Text = "0%";
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(286, 12);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(140, 36);
			this.button4.TabIndex = 7;
			this.button4.Text = "Slack から学習する";
			this.button4.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(147)))), ((int)(((byte)(147)))));
			this.BackgroundImage = global::CitroidGui.Properties.Resources.Citroid_2016;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ClientSize = new System.Drawing.Size(445, 346);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.trackBar1);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "Form1";
			this.Text = "Citroid for Slack";
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button4;
	}
}

