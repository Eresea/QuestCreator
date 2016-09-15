namespace DialogueCreator3
{
    partial class Preview
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
            this.NPCNameLabel = new System.Windows.Forms.Label();
            this.Answer1Button = new System.Windows.Forms.Button();
            this.Answer2Button = new System.Windows.Forms.Button();
            this.Answer3Button = new System.Windows.Forms.Button();
            this.Answer4Button = new System.Windows.Forms.Button();
            this.Answer5Button = new System.Windows.Forms.Button();
            this.QuestionrTxtBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // NPCNameLabel
            // 
            this.NPCNameLabel.AutoSize = true;
            this.NPCNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NPCNameLabel.Location = new System.Drawing.Point(12, 17);
            this.NPCNameLabel.Name = "NPCNameLabel";
            this.NPCNameLabel.Size = new System.Drawing.Size(154, 33);
            this.NPCNameLabel.TabIndex = 0;
            this.NPCNameLabel.Text = "NPCName";
            // 
            // Answer1Button
            // 
            this.Answer1Button.BackColor = System.Drawing.Color.DarkGray;
            this.Answer1Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Answer1Button.Location = new System.Drawing.Point(12, 161);
            this.Answer1Button.Name = "Answer1Button";
            this.Answer1Button.Size = new System.Drawing.Size(1254, 32);
            this.Answer1Button.TabIndex = 1;
            this.Answer1Button.Text = "Answer1";
            this.Answer1Button.UseVisualStyleBackColor = false;
            this.Answer1Button.Click += new System.EventHandler(this.Answer1Button_Click);
            // 
            // Answer2Button
            // 
            this.Answer2Button.BackColor = System.Drawing.Color.DarkGray;
            this.Answer2Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Answer2Button.Location = new System.Drawing.Point(12, 199);
            this.Answer2Button.Name = "Answer2Button";
            this.Answer2Button.Size = new System.Drawing.Size(1254, 32);
            this.Answer2Button.TabIndex = 2;
            this.Answer2Button.Text = "Answer2";
            this.Answer2Button.UseVisualStyleBackColor = false;
            this.Answer2Button.Click += new System.EventHandler(this.Answer2Button_Click);
            // 
            // Answer3Button
            // 
            this.Answer3Button.BackColor = System.Drawing.Color.DarkGray;
            this.Answer3Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Answer3Button.Location = new System.Drawing.Point(12, 237);
            this.Answer3Button.Name = "Answer3Button";
            this.Answer3Button.Size = new System.Drawing.Size(1254, 32);
            this.Answer3Button.TabIndex = 3;
            this.Answer3Button.Text = "Answer3";
            this.Answer3Button.UseVisualStyleBackColor = false;
            this.Answer3Button.Click += new System.EventHandler(this.Answer3Button_Click);
            // 
            // Answer4Button
            // 
            this.Answer4Button.BackColor = System.Drawing.Color.DarkGray;
            this.Answer4Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Answer4Button.Location = new System.Drawing.Point(12, 275);
            this.Answer4Button.Name = "Answer4Button";
            this.Answer4Button.Size = new System.Drawing.Size(1254, 32);
            this.Answer4Button.TabIndex = 4;
            this.Answer4Button.Text = "Answer4";
            this.Answer4Button.UseVisualStyleBackColor = false;
            this.Answer4Button.Click += new System.EventHandler(this.Answer4Button_Click);
            // 
            // Answer5Button
            // 
            this.Answer5Button.BackColor = System.Drawing.Color.DarkGray;
            this.Answer5Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Answer5Button.Location = new System.Drawing.Point(12, 313);
            this.Answer5Button.Name = "Answer5Button";
            this.Answer5Button.Size = new System.Drawing.Size(1254, 32);
            this.Answer5Button.TabIndex = 5;
            this.Answer5Button.Text = "Answer5";
            this.Answer5Button.UseVisualStyleBackColor = false;
            this.Answer5Button.Click += new System.EventHandler(this.Answer5Button_Click);
            // 
            // QuestionrTxtBox
            // 
            this.QuestionrTxtBox.BackColor = System.Drawing.Color.Gray;
            this.QuestionrTxtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.QuestionrTxtBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.QuestionrTxtBox.Location = new System.Drawing.Point(12, 69);
            this.QuestionrTxtBox.Name = "QuestionrTxtBox";
            this.QuestionrTxtBox.ReadOnly = true;
            this.QuestionrTxtBox.Size = new System.Drawing.Size(1254, 86);
            this.QuestionrTxtBox.TabIndex = 6;
            this.QuestionrTxtBox.Text = "";
            // 
            // Preview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::DialogueCreator3.Properties.Resources.DialogBox;
            this.ClientSize = new System.Drawing.Size(1278, 357);
            this.Controls.Add(this.QuestionrTxtBox);
            this.Controls.Add(this.Answer5Button);
            this.Controls.Add(this.Answer4Button);
            this.Controls.Add(this.Answer3Button);
            this.Controls.Add(this.Answer2Button);
            this.Controls.Add(this.Answer1Button);
            this.Controls.Add(this.NPCNameLabel);
            this.Name = "Preview";
            this.Text = "Preview";
            this.Load += new System.EventHandler(this.Preview_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NPCNameLabel;
        private System.Windows.Forms.Button Answer1Button;
        private System.Windows.Forms.Button Answer2Button;
        private System.Windows.Forms.Button Answer3Button;
        private System.Windows.Forms.Button Answer4Button;
        private System.Windows.Forms.Button Answer5Button;
        private System.Windows.Forms.RichTextBox QuestionrTxtBox;
    }
}