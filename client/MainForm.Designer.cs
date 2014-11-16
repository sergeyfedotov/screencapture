namespace Screencapture
{
    partial class MainForm
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
            this.Overlay = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Overlay)).BeginInit();
            this.SuspendLayout();
            // 
            // Overlay
            // 
            this.Overlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Overlay.Location = new System.Drawing.Point(0, 0);
            this.Overlay.Name = "Overlay";
            this.Overlay.Size = new System.Drawing.Size(284, 262);
            this.Overlay.TabIndex = 0;
            this.Overlay.TabStop = false;
            this.Overlay.Paint += new System.Windows.Forms.PaintEventHandler(this.Overlay_Paint);
            this.Overlay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Overlay_MouseDown);
            this.Overlay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Overlay_MouseMove);
            this.Overlay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Overlay_MouseUp);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.Overlay);
            this.Name = "MainForm";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.Overlay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Overlay;
    }
}

