namespace WFA_LM_test1
{
    partial class LeapZephyros
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.txt_circle = new System.Windows.Forms.TextBox();
            this.txt_move = new System.Windows.Forms.TextBox();
            this.txt_grab = new System.Windows.Forms.TextBox();
            this.txt_wrist_Z = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_wrist_Y = new System.Windows.Forms.TextBox();
            this.txt_wrist_X = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.create_server_btn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.Location = new System.Drawing.Point(42, 510);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(320, 37);
            this.button1.TabIndex = 4;
            this.button1.Text = "Thread Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.groupBox11);
            this.groupBox2.Controls.Add(this.create_server_btn);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Font = new System.Drawing.Font("굴림", 20F);
            this.groupBox2.Location = new System.Drawing.Point(1505, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(387, 664);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "[Right Arm]";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(778, 35);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(703, 666);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 16;
            this.pictureBox2.TabStop = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox1.Location = new System.Drawing.Point(44, 596);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(212, 31);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Text = "Sending Data";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.txt_circle);
            this.groupBox11.Controls.Add(this.txt_move);
            this.groupBox11.Controls.Add(this.txt_grab);
            this.groupBox11.Controls.Add(this.txt_wrist_Z);
            this.groupBox11.Controls.Add(this.label4);
            this.groupBox11.Controls.Add(this.txt_wrist_Y);
            this.groupBox11.Controls.Add(this.txt_wrist_X);
            this.groupBox11.Controls.Add(this.label2);
            this.groupBox11.Controls.Add(this.label3);
            this.groupBox11.Location = new System.Drawing.Point(18, 27);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(363, 225);
            this.groupBox11.TabIndex = 12;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "wrist";
            // 
            // txt_circle
            // 
            this.txt_circle.BackColor = System.Drawing.SystemColors.Window;
            this.txt_circle.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_circle.ForeColor = System.Drawing.Color.Blue;
            this.txt_circle.Location = new System.Drawing.Point(236, 111);
            this.txt_circle.Name = "txt_circle";
            this.txt_circle.Size = new System.Drawing.Size(100, 39);
            this.txt_circle.TabIndex = 7;
            this.txt_circle.Text = "100";
            this.txt_circle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_move
            // 
            this.txt_move.BackColor = System.Drawing.SystemColors.Window;
            this.txt_move.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_move.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.txt_move.Location = new System.Drawing.Point(130, 111);
            this.txt_move.Name = "txt_move";
            this.txt_move.Size = new System.Drawing.Size(100, 39);
            this.txt_move.TabIndex = 6;
            this.txt_move.Text = "100";
            this.txt_move.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_grab
            // 
            this.txt_grab.BackColor = System.Drawing.SystemColors.Window;
            this.txt_grab.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_grab.ForeColor = System.Drawing.Color.Red;
            this.txt_grab.Location = new System.Drawing.Point(24, 111);
            this.txt_grab.Name = "txt_grab";
            this.txt_grab.Size = new System.Drawing.Size(100, 39);
            this.txt_grab.TabIndex = 5;
            this.txt_grab.Text = "100";
            this.txt_grab.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_wrist_Z
            // 
            this.txt_wrist_Z.BackColor = System.Drawing.SystemColors.Window;
            this.txt_wrist_Z.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_wrist_Z.ForeColor = System.Drawing.Color.Blue;
            this.txt_wrist_Z.Location = new System.Drawing.Point(236, 67);
            this.txt_wrist_Z.Name = "txt_wrist_Z";
            this.txt_wrist_Z.Size = new System.Drawing.Size(100, 39);
            this.txt_wrist_Z.TabIndex = 3;
            this.txt_wrist_Z.Text = "100";
            this.txt_wrist_Z.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(268, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 27);
            this.label4.TabIndex = 4;
            this.label4.Text = "Z";
            // 
            // txt_wrist_Y
            // 
            this.txt_wrist_Y.BackColor = System.Drawing.SystemColors.Window;
            this.txt_wrist_Y.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_wrist_Y.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.txt_wrist_Y.Location = new System.Drawing.Point(130, 67);
            this.txt_wrist_Y.Name = "txt_wrist_Y";
            this.txt_wrist_Y.Size = new System.Drawing.Size(100, 39);
            this.txt_wrist_Y.TabIndex = 1;
            this.txt_wrist_Y.Text = "100";
            this.txt_wrist_Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_wrist_X
            // 
            this.txt_wrist_X.BackColor = System.Drawing.SystemColors.Window;
            this.txt_wrist_X.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_wrist_X.ForeColor = System.Drawing.Color.Red;
            this.txt_wrist_X.Location = new System.Drawing.Point(24, 67);
            this.txt_wrist_X.Name = "txt_wrist_X";
            this.txt_wrist_X.Size = new System.Drawing.Size(100, 39);
            this.txt_wrist_X.TabIndex = 1;
            this.txt_wrist_X.Text = "100";
            this.txt_wrist_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 27);
            this.label2.TabIndex = 2;
            this.label2.Text = "X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(162, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 27);
            this.label3.TabIndex = 2;
            this.label3.Text = "Y";
            // 
            // create_server_btn
            // 
            this.create_server_btn.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.create_server_btn.Location = new System.Drawing.Point(42, 553);
            this.create_server_btn.Name = "create_server_btn";
            this.create_server_btn.Size = new System.Drawing.Size(322, 37);
            this.create_server_btn.TabIndex = 15;
            this.create_server_btn.Text = "Create Server";
            this.create_server_btn.UseVisualStyleBackColor = true;
            this.create_server_btn.Click += new System.EventHandler(this.create_server_btn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(6, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(754, 666);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button2.Location = new System.Drawing.Point(6, 889);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 48);
            this.button2.TabIndex = 14;
            this.button2.Text = "Connect";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button3.Location = new System.Drawing.Point(164, 889);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(181, 48);
            this.button3.TabIndex = 15;
            this.button3.Text = "Disconnect";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox2);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Font = new System.Drawing.Font("굴림", 20F);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1487, 937);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "[Video view]";
            // 
            // LeapZephyros
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 961);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "LeapZephyros";
            this.Text = "Leap Motion with Zephyros";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox11;
        public System.Windows.Forms.TextBox txt_wrist_Z;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox txt_wrist_Y;
        public System.Windows.Forms.TextBox txt_wrist_X;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txt_grab;
        private System.Windows.Forms.Button create_server_btn;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox txt_move;
        public System.Windows.Forms.TextBox txt_circle;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

