namespace CameraSetting
{
    partial class UserControlOfCameraSetting
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Test001 = new System.Windows.Forms.Button();
            this.label_Exposure = new System.Windows.Forms.Label();
            this.label_FPS = new System.Windows.Forms.Label();
            this.label_Gain = new System.Windows.Forms.Label();
            this.label_FPSValue = new System.Windows.Forms.Label();
            this.label_GainValue = new System.Windows.Forms.Label();
            this.label_ExposureValue = new System.Windows.Forms.Label();
            this.groupBox_CameraImformation = new System.Windows.Forms.GroupBox();
            this.numericUpDown_Exposure = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_Gain = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_FPS = new System.Windows.Forms.NumericUpDown();
            this.groupBox_CameraImformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Exposure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Gain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_FPS)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Test001
            // 
            this.button_Test001.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_Test001.Location = new System.Drawing.Point(327, 107);
            this.button_Test001.Name = "button_Test001";
            this.button_Test001.Size = new System.Drawing.Size(120, 40);
            this.button_Test001.TabIndex = 0;
            this.button_Test001.Text = "Test";
            this.button_Test001.UseVisualStyleBackColor = true;
            this.button_Test001.Click += new System.EventHandler(this.button_Test001_Click);
            // 
            // label_Exposure
            // 
            this.label_Exposure.AutoSize = true;
            this.label_Exposure.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_Exposure.Location = new System.Drawing.Point(6, 29);
            this.label_Exposure.Name = "label_Exposure";
            this.label_Exposure.Size = new System.Drawing.Size(113, 25);
            this.label_Exposure.TabIndex = 1;
            this.label_Exposure.Text = "Exposure : ";
            // 
            // label_FPS
            // 
            this.label_FPS.AutoSize = true;
            this.label_FPS.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_FPS.Location = new System.Drawing.Point(6, 107);
            this.label_FPS.Name = "label_FPS";
            this.label_FPS.Size = new System.Drawing.Size(106, 25);
            this.label_FPS.TabIndex = 2;
            this.label_FPS.Text = "FPS           :";
            // 
            // label_Gain
            // 
            this.label_Gain.AutoSize = true;
            this.label_Gain.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_Gain.Location = new System.Drawing.Point(6, 70);
            this.label_Gain.Name = "label_Gain";
            this.label_Gain.Size = new System.Drawing.Size(110, 25);
            this.label_Gain.TabIndex = 3;
            this.label_Gain.Text = "Gain         : ";
            // 
            // label_FPSValue
            // 
            this.label_FPSValue.AutoSize = true;
            this.label_FPSValue.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_FPSValue.Location = new System.Drawing.Point(125, 107);
            this.label_FPSValue.Name = "label_FPSValue";
            this.label_FPSValue.Size = new System.Drawing.Size(50, 25);
            this.label_FPSValue.TabIndex = 4;
            this.label_FPSValue.Text = "Null";
            // 
            // label_GainValue
            // 
            this.label_GainValue.AutoSize = true;
            this.label_GainValue.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_GainValue.Location = new System.Drawing.Point(125, 70);
            this.label_GainValue.Name = "label_GainValue";
            this.label_GainValue.Size = new System.Drawing.Size(50, 25);
            this.label_GainValue.TabIndex = 5;
            this.label_GainValue.Text = "Null";
            // 
            // label_ExposureValue
            // 
            this.label_ExposureValue.AutoSize = true;
            this.label_ExposureValue.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_ExposureValue.Location = new System.Drawing.Point(125, 33);
            this.label_ExposureValue.Name = "label_ExposureValue";
            this.label_ExposureValue.Size = new System.Drawing.Size(50, 25);
            this.label_ExposureValue.TabIndex = 6;
            this.label_ExposureValue.Text = "Null";
            // 
            // groupBox_CameraImformation
            // 
            this.groupBox_CameraImformation.Controls.Add(this.label_Gain);
            this.groupBox_CameraImformation.Controls.Add(this.label_Exposure);
            this.groupBox_CameraImformation.Controls.Add(this.label_FPS);
            this.groupBox_CameraImformation.Controls.Add(this.label_FPSValue);
            this.groupBox_CameraImformation.Controls.Add(this.label_GainValue);
            this.groupBox_CameraImformation.Controls.Add(this.label_ExposureValue);
            this.groupBox_CameraImformation.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox_CameraImformation.Location = new System.Drawing.Point(3, 3);
            this.groupBox_CameraImformation.Name = "groupBox_CameraImformation";
            this.groupBox_CameraImformation.Size = new System.Drawing.Size(249, 144);
            this.groupBox_CameraImformation.TabIndex = 8;
            this.groupBox_CameraImformation.TabStop = false;
            this.groupBox_CameraImformation.Text = "Camera Imformation";
            // 
            // numericUpDown_Exposure
            // 
            this.numericUpDown_Exposure.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numericUpDown_Exposure.Location = new System.Drawing.Point(327, 3);
            this.numericUpDown_Exposure.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_Exposure.Name = "numericUpDown_Exposure";
            this.numericUpDown_Exposure.Size = new System.Drawing.Size(120, 34);
            this.numericUpDown_Exposure.TabIndex = 9;
            this.numericUpDown_Exposure.ValueChanged += new System.EventHandler(this.numericUpDown_Exposure_ValueChanged);
            // 
            // numericUpDown_Gain
            // 
            this.numericUpDown_Gain.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numericUpDown_Gain.Location = new System.Drawing.Point(327, 39);
            this.numericUpDown_Gain.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_Gain.Name = "numericUpDown_Gain";
            this.numericUpDown_Gain.Size = new System.Drawing.Size(120, 34);
            this.numericUpDown_Gain.TabIndex = 10;
            // 
            // numericUpDown_FPS
            // 
            this.numericUpDown_FPS.Font = new System.Drawing.Font("微軟正黑體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numericUpDown_FPS.Location = new System.Drawing.Point(327, 74);
            this.numericUpDown_FPS.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_FPS.Name = "numericUpDown_FPS";
            this.numericUpDown_FPS.Size = new System.Drawing.Size(120, 34);
            this.numericUpDown_FPS.TabIndex = 11;
            // 
            // UserControlOfCameraSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.numericUpDown_Exposure);
            this.Controls.Add(this.numericUpDown_Gain);
            this.Controls.Add(this.numericUpDown_FPS);
            this.Controls.Add(this.groupBox_CameraImformation);
            this.Controls.Add(this.button_Test001);
            this.Name = "UserControlOfCameraSetting";
            this.Size = new System.Drawing.Size(450, 150);
            this.groupBox_CameraImformation.ResumeLayout(false);
            this.groupBox_CameraImformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Exposure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Gain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_FPS)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button button_Test001;
        public System.Windows.Forms.Label label_Exposure;
        public System.Windows.Forms.Label label_FPS;
        public System.Windows.Forms.Label label_Gain;
        public System.Windows.Forms.Label label_FPSValue;
        public System.Windows.Forms.Label label_GainValue;
        public System.Windows.Forms.Label label_ExposureValue;
        public System.Windows.Forms.GroupBox groupBox_CameraImformation;
        public System.Windows.Forms.NumericUpDown numericUpDown_Exposure;
        public System.Windows.Forms.NumericUpDown numericUpDown_FPS;
        public System.Windows.Forms.NumericUpDown numericUpDown_Gain;
    }
}
