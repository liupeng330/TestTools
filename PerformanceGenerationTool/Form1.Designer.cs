namespace PerformanceGenerationTool
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DBTableSelection = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.startDate_dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.endDate_dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.generate_button = new System.Windows.Forms.Button();
            this.randomStart_textBox = new System.Windows.Forms.TextBox();
            this.randomEnd_textBox = new System.Windows.Forms.TextBox();
            this.dbfilelocatioin_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.accountId_textBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // DBTableSelection
            // 
            this.DBTableSelection.FormattingEnabled = true;
            this.DBTableSelection.Items.AddRange(new object[] {
            "tblConvFacebookAccount_daily",
            "tblConvFacebookAdGroup_daily",
            "tblConvFacebookCampaign_daily",
            "tblConvStatsFacebookAdGroup",
            "tblConvStatsFacebookCampaign",
            "tblPerfFacebookAccount_daily",
            "tblPerfFacebookAdGroup_daily",
            "tblPerfFacebookCampaign_daily"});
            this.DBTableSelection.Location = new System.Drawing.Point(28, 25);
            this.DBTableSelection.Name = "DBTableSelection";
            this.DBTableSelection.Size = new System.Drawing.Size(185, 21);
            this.DBTableSelection.TabIndex = 0;
            this.DBTableSelection.SelectedIndexChanged += new System.EventHandler(this.DBTableSelection_SelectedIndexChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(28, 297);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(517, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // startDate_dateTimePicker
            // 
            this.startDate_dateTimePicker.Location = new System.Drawing.Point(19, 50);
            this.startDate_dateTimePicker.Name = "startDate_dateTimePicker";
            this.startDate_dateTimePicker.Size = new System.Drawing.Size(200, 20);
            this.startDate_dateTimePicker.TabIndex = 3;
            // 
            // endDate_dateTimePicker
            // 
            this.endDate_dateTimePicker.Location = new System.Drawing.Point(19, 107);
            this.endDate_dateTimePicker.Name = "endDate_dateTimePicker";
            this.endDate_dateTimePicker.Size = new System.Drawing.Size(200, 20);
            this.endDate_dateTimePicker.TabIndex = 4;
            // 
            // generate_button
            // 
            this.generate_button.Location = new System.Drawing.Point(28, 338);
            this.generate_button.Name = "generate_button";
            this.generate_button.Size = new System.Drawing.Size(75, 23);
            this.generate_button.TabIndex = 5;
            this.generate_button.Text = "Generate";
            this.generate_button.UseVisualStyleBackColor = true;
            this.generate_button.Click += new System.EventHandler(this.generate_button_Click);
            // 
            // randomStart_textBox
            // 
            this.randomStart_textBox.Location = new System.Drawing.Point(40, 50);
            this.randomStart_textBox.Name = "randomStart_textBox";
            this.randomStart_textBox.Size = new System.Drawing.Size(100, 20);
            this.randomStart_textBox.TabIndex = 6;
            // 
            // randomEnd_textBox
            // 
            this.randomEnd_textBox.Location = new System.Drawing.Point(40, 107);
            this.randomEnd_textBox.Name = "randomEnd_textBox";
            this.randomEnd_textBox.Size = new System.Drawing.Size(100, 20);
            this.randomEnd_textBox.TabIndex = 7;
            // 
            // dbfilelocatioin_textBox
            // 
            this.dbfilelocatioin_textBox.Location = new System.Drawing.Point(28, 84);
            this.dbfilelocatioin_textBox.Name = "dbfilelocatioin_textBox";
            this.dbfilelocatioin_textBox.Size = new System.Drawing.Size(517, 20);
            this.dbfilelocatioin_textBox.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "DB table:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "From";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "To";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Min value";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Max value";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.randomStart_textBox);
            this.groupBox1.Controls.Add(this.randomEnd_textBox);
            this.groupBox1.Location = new System.Drawing.Point(338, 122);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(207, 159);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Random data range";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.endDate_dateTimePicker);
            this.groupBox2.Controls.Add(this.startDate_dateTimePicker);
            this.groupBox2.Location = new System.Drawing.Point(28, 122);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(235, 159);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Date time range";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "DB file location";
            // 
            // accountId_textBox
            // 
            this.accountId_textBox.Location = new System.Drawing.Point(378, 26);
            this.accountId_textBox.Name = "accountId_textBox";
            this.accountId_textBox.Size = new System.Drawing.Size(100, 20);
            this.accountId_textBox.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(378, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Account Id";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 372);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.accountId_textBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dbfilelocatioin_textBox);
            this.Controls.Add(this.generate_button);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.DBTableSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "PerformanceGenerator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox DBTableSelection;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DateTimePicker startDate_dateTimePicker;
        private System.Windows.Forms.DateTimePicker endDate_dateTimePicker;
        private System.Windows.Forms.Button generate_button;
        private System.Windows.Forms.TextBox randomStart_textBox;
        private System.Windows.Forms.TextBox randomEnd_textBox;
        private System.Windows.Forms.TextBox dbfilelocatioin_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox accountId_textBox;
        private System.Windows.Forms.Label label7;
    }
}

