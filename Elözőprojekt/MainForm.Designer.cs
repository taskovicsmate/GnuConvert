/*namespace könyvelőprogram
{
    partial class Könyvelőprogram
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Könyvelőprogram));
            Uploadbutton2 = new System.Windows.Forms.Button();
            VersionNumber = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            button3 = new System.Windows.Forms.Button();
            panel2 = new System.Windows.Forms.Panel();
            checkBox1 = new System.Windows.Forms.CheckBox();
            button6 = new System.Windows.Forms.Button();
            label7 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            button5 = new System.Windows.Forms.Button();
            button4 = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            panel3 = new System.Windows.Forms.Panel();
            button8 = new System.Windows.Forms.Button();
            button7 = new System.Windows.Forms.Button();
            label9 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // Uploadbutton2
            // 
            Uploadbutton2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            Uploadbutton2.BackColor = System.Drawing.SystemColors.ButtonFace;
            Uploadbutton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
            Uploadbutton2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            Uploadbutton2.Location = new System.Drawing.Point(728, 34);
            Uploadbutton2.Margin = new System.Windows.Forms.Padding(0);
            Uploadbutton2.Name = "Uploadbutton2";
            Uploadbutton2.Padding = new System.Windows.Forms.Padding(10);
            Uploadbutton2.Size = new System.Drawing.Size(194, 90);
            Uploadbutton2.TabIndex = 0;
            Uploadbutton2.Text = "Konvertálás";
            Uploadbutton2.UseVisualStyleBackColor = false;
            Uploadbutton2.Click += button1_Click;
            // 
            // VersionNumber
            // 
            VersionNumber.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            VersionNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            VersionNumber.Location = new System.Drawing.Point(10, 685);
            VersionNumber.Margin = new System.Windows.Forms.Padding(0);
            VersionNumber.Name = "VersionNumber";
            VersionNumber.ReadOnly = true;
            VersionNumber.Size = new System.Drawing.Size(73, 19);
            VersionNumber.TabIndex = 1;
            VersionNumber.Text = "V0.6";
            // 
            // label1
            // 
            label1.AutoEllipsis = true;
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
            label1.Location = new System.Drawing.Point(38, 34);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(333, 20);
            label1.TabIndex = 2;
            label1.Text = "1. Ki kell választni a konvertálni kívánt .csv fájlt.";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
            label2.Location = new System.Drawing.Point(38, 118);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(512, 20);
            label2.TabIndex = 3;
            label2.Text = "2. Konvertálás után egy új excel fájlba be kell importálni a konvertált fájlt.";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
            label3.Location = new System.Drawing.Point(38, 201);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(378, 20);
            label3.TabIndex = 4;
            label3.Text = "3. A beimportált fájlt el kell menteni .csv formátumba.";
            // 
            // panel1
            // 
            panel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(Uploadbutton2);
            panel1.Location = new System.Drawing.Point(13, 70);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(980, 495);
            panel1.TabIndex = 5;
            panel1.Paint += panel1_Paint;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(13, 6);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(212, 60);
            button1.TabIndex = 6;
            button1.Text = "Partner ZOO";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(231, 6);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(200, 60);
            button2.TabIndex = 7;
            button2.Text = "06-os Bank";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(437, 6);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(258, 60);
            button3.TabIndex = 8;
            button3.Text = "MYPOS Bankártyás";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // panel2
            // 
            panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel2.Controls.Add(checkBox1);
            panel2.Controls.Add(button6);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(label6);
            panel2.Controls.Add(button5);
            panel2.Controls.Add(button4);
            panel2.Controls.Add(label5);
            panel2.Location = new System.Drawing.Point(13, 70);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(980, 495);
            panel2.TabIndex = 2;
            panel2.Visible = false;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new System.Drawing.Point(665, 124);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(209, 24);
            checkBox1.TabIndex = 16;
            checkBox1.TabStop = false;
            checkBox1.Text = "Sikeresen ki lett választva";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Location = new System.Drawing.Point(719, 54);
            button6.Name = "button6";
            button6.Size = new System.Drawing.Size(182, 64);
            button6.TabIndex = 15;
            button6.Text = "Kiválasztás";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // label7
            // 
            label7.Location = new System.Drawing.Point(25, 62);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(462, 56);
            label7.TabIndex = 14;
            label7.Text = "1. Válazd ki a kiexportált pénzügyi nyilvántartást.";
            // 
            // label6
            // 
            label6.Location = new System.Drawing.Point(25, 396);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(660, 69);
            label6.TabIndex = 13;
            label6.Text = "Ezzel megtörténik a konvertálás.";
            // 
            // button5
            // 
            button5.Location = new System.Drawing.Point(903, 225);
            button5.Name = "button5";
            button5.Size = new System.Drawing.Size(8, 8);
            button5.TabIndex = 12;
            button5.Text = "button5";
            button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new System.Drawing.Point(719, 340);
            button4.Name = "button4";
            button4.Size = new System.Drawing.Size(182, 68);
            button4.TabIndex = 11;
            button4.Text = "Konvertálás";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label5
            // 
            label5.Location = new System.Drawing.Point(25, 340);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(688, 68);
            label5.TabIndex = 10;
            label5.Text = "2. Válazd ki a 06-os Bankhoz tartozó banktörténet fájlt. Minta:HISTORY_00372237_20241231 (9) ";
            // 
            // panel3
            // 
            panel3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel3.AutoSize = true;
            panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel3.Controls.Add(label8);
            panel3.Controls.Add(label4);
            panel3.Controls.Add(label9);
            panel3.Controls.Add(button8);
            panel3.Controls.Add(button7);
            panel3.Location = new System.Drawing.Point(13, 70);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(980, 493);
            panel3.TabIndex = 3;
            panel3.Visible = false;
            panel3.Paint += panel3_Paint;
            // 
            // button8
            // 
            button8.Location = new System.Drawing.Point(775, 320);
            button8.Name = "button8";
            button8.Size = new System.Drawing.Size(126, 61);
            button8.TabIndex = 2;
            button8.Text = "Konvertálás";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button7
            // 
            button7.Location = new System.Drawing.Point(775, 62);
            button7.Name = "button7";
            button7.Size = new System.Drawing.Size(126, 64);
            button7.TabIndex = 1;
            button7.Text = "Kiválasztás";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // label9
            // 
            label9.Location = new System.Drawing.Point(25, 70);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(462, 56);
            label9.TabIndex = 15;
            label9.Text = "1. Válazd ki a kiexportált pénzügyi nyilvántartást.";
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(21, 340);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(688, 68);
            label4.TabIndex = 16;
            label4.Text = "2. Válazd ki a 06-os Bankhoz tartozó banktörténet fájlt. Minta:HISTORY_00372237_20241231 (9) ";
            // 
            // label8
            // 
            label8.Location = new System.Drawing.Point(21, 408);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(660, 69);
            label8.TabIndex = 17;
            label8.Text = "Ezzel megtörténik a konvertálás.";
            // 
            // Könyvelőprogram
            // 
            ClientSize = new System.Drawing.Size(1117, 590);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(panel1);
            Controls.Add(VersionNumber);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "Könyvelőprogram";
            Padding = new System.Windows.Forms.Padding(10);
            Text = "Könyvelőprogram";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button Uploadbutton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button Uploadbutton2;
        private System.Windows.Forms.TextBox VersionNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button6;
        public System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
    }
}
*/
