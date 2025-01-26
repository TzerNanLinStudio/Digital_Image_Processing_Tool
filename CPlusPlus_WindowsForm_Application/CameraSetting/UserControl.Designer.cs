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
            this.label_Exposure = new System.Windows.Forms.Label();
            this.label_FPS = new System.Windows.Forms.Label();
            this.label_Gain = new System.Windows.Forms.Label();
            this.label_FPSValue = new System.Windows.Forms.Label();
            this.label_GainValue = new System.Windows.Forms.Label();
            this.label_ExposureValue = new System.Windows.Forms.Label();
            this.groupBox_CameraImformation = new System.Windows.Forms.GroupBox();
            this.groupBox_CameraImformation.SuspendLayout();
            this.SuspendLayout();
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
            this.groupBox_CameraImformation.Size = new System.Drawing.Size(224, 144);
            this.groupBox_CameraImformation.TabIndex = 8;
            this.groupBox_CameraImformation.TabStop = false;
            this.groupBox_CameraImformation.Text = "Camera Imformation";
            // 
            // UserControlOfCameraSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.groupBox_CameraImformation);
            this.Name = "UserControlOfCameraSetting";
            this.Size = new System.Drawing.Size(240, 150);
            this.groupBox_CameraImformation.ResumeLayout(false);
            this.groupBox_CameraImformation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label label_Exposure;
        public System.Windows.Forms.Label label_FPS;
        public System.Windows.Forms.Label label_Gain;
        public System.Windows.Forms.Label label_FPSValue;
        public System.Windows.Forms.Label label_GainValue;
        public System.Windows.Forms.Label label_ExposureValue;
        public System.Windows.Forms.GroupBox groupBox_CameraImformation;
    }
}
