namespace Segments
{
    partial class SegmentsForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AfterRB = new System.Windows.Forms.RadioButton();
            this.BeforeRB = new System.Windows.Forms.RadioButton();
            this.NumOfSegmentsLabel = new System.Windows.Forms.Label();
            this.NumOfSegmentsTB = new System.Windows.Forms.TextBox();
            this.StartButton = new System.Windows.Forms.Button();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.zedGraphControl2 = new ZedGraph.ZedGraphControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.AfterRB);
            this.panel1.Controls.Add(this.BeforeRB);
            this.panel1.Controls.Add(this.NumOfSegmentsLabel);
            this.panel1.Controls.Add(this.NumOfSegmentsTB);
            this.panel1.Controls.Add(this.StartButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(886, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(366, 680);
            this.panel1.TabIndex = 1;
            // 
            // AfterRB
            // 
            this.AfterRB.AutoSize = true;
            this.AfterRB.Checked = true;
            this.AfterRB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AfterRB.Location = new System.Drawing.Point(79, 98);
            this.AfterRB.Name = "AfterRB";
            this.AfterRB.Size = new System.Drawing.Size(195, 28);
            this.AfterRB.TabIndex = 4;
            this.AfterRB.TabStop = true;
            this.AfterRB.Text = "После разделения";
            this.AfterRB.UseVisualStyleBackColor = true;
            this.AfterRB.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // BeforeRB
            // 
            this.BeforeRB.AutoSize = true;
            this.BeforeRB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BeforeRB.Location = new System.Drawing.Point(79, 64);
            this.BeforeRB.Name = "BeforeRB";
            this.BeforeRB.Size = new System.Drawing.Size(165, 28);
            this.BeforeRB.TabIndex = 3;
            this.BeforeRB.Text = "До разделения";
            this.BeforeRB.UseVisualStyleBackColor = true;
            this.BeforeRB.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // NumOfSegmentsLabel
            // 
            this.NumOfSegmentsLabel.AutoSize = true;
            this.NumOfSegmentsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumOfSegmentsLabel.Location = new System.Drawing.Point(29, 321);
            this.NumOfSegmentsLabel.Name = "NumOfSegmentsLabel";
            this.NumOfSegmentsLabel.Size = new System.Drawing.Size(211, 24);
            this.NumOfSegmentsLabel.TabIndex = 2;
            this.NumOfSegmentsLabel.Text = "Количество отрезков:";
            // 
            // NumOfSegmentsTB
            // 
            this.NumOfSegmentsTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumOfSegmentsTB.Location = new System.Drawing.Point(246, 318);
            this.NumOfSegmentsTB.Name = "NumOfSegmentsTB";
            this.NumOfSegmentsTB.Size = new System.Drawing.Size(74, 29);
            this.NumOfSegmentsTB.TabIndex = 1;
            this.NumOfSegmentsTB.Text = "10";
            // 
            // StartButton
            // 
            this.StartButton.AutoSize = true;
            this.StartButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StartButton.Location = new System.Drawing.Point(114, 379);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(145, 66);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "СТАРТ";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.zedGraphControl1.IsShowPointValues = true;
            this.zedGraphControl1.Location = new System.Drawing.Point(0, 0);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(879, 680);
            this.zedGraphControl1.TabIndex = 0;
            // 
            // zedGraphControl2
            // 
            this.zedGraphControl2.IsShowPointValues = true;
            this.zedGraphControl2.Location = new System.Drawing.Point(0, 0);
            this.zedGraphControl2.Name = "zedGraphControl2";
            this.zedGraphControl2.ScrollGrace = 0D;
            this.zedGraphControl2.ScrollMaxX = 0D;
            this.zedGraphControl2.ScrollMaxY = 0D;
            this.zedGraphControl2.ScrollMaxY2 = 0D;
            this.zedGraphControl2.ScrollMinX = 0D;
            this.zedGraphControl2.ScrollMinY = 0D;
            this.zedGraphControl2.ScrollMinY2 = 0D;
            this.zedGraphControl2.Size = new System.Drawing.Size(879, 680);
            this.zedGraphControl2.TabIndex = 5;
            this.zedGraphControl2.Visible = false;
            // 
            // SegmentsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1252, 680);
            this.Controls.Add(this.zedGraphControl2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.zedGraphControl1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MaximizeBox = false;
            this.Name = "SegmentsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Segments";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label NumOfSegmentsLabel;
        private System.Windows.Forms.TextBox NumOfSegmentsTB;
        private System.Windows.Forms.Button StartButton;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.RadioButton AfterRB;
        private System.Windows.Forms.RadioButton BeforeRB;
        private ZedGraph.ZedGraphControl zedGraphControl2;
    }
}

