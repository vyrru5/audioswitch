using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AudioSwitch.CoreAudioApi;

namespace AudioSwitch
{
    partial class FormSwitcher
    {
        private CustomListView listDevices;
        private NotifyIcon notifyIcon;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.pictureItemsBack = new System.Windows.Forms.PictureBox();
            this.VolBar = new VolumeBar();
            this.ledLeft = new LedBar();
            this.ledRight = new LedBar();
            this.listDevices = new CustomListView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureItemsBack)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "AudioSwitch";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDown);
            this.notifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseMove);
            this.notifyIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseUp);
            // 
            // pictureItemsBack
            // 
            this.pictureItemsBack.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.pictureItemsBack.Location = new System.Drawing.Point(0, 190);
            this.pictureItemsBack.Name = "pictureItemsBack";
            this.pictureItemsBack.Size = new System.Drawing.Size(221, 43);
            this.pictureItemsBack.TabIndex = 2;
            this.pictureItemsBack.TabStop = false;
            this.pictureItemsBack.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // VolBar
            // 
            this.VolBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.VolBar.Location = new System.Drawing.Point(13, 209);
            this.VolBar.Name = "VolBar";
            this.VolBar.Size = new System.Drawing.Size(195, 9);
            this.VolBar.TabIndex = 7;
            this.VolBar.TabStop = false;
            // 
            // ledLeft
            // 
            this.ledLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ledLeft.BackColor = System.Drawing.SystemColors.Control;
            this.ledLeft.Location = new System.Drawing.Point(13, 203);
            this.ledLeft.Name = "ledLeft";
            this.ledLeft.Size = new System.Drawing.Size(197, 4);
            this.ledLeft.TabIndex = 8;
            this.ledLeft.TabStop = false;
            // 
            // ledRight
            // 
            this.ledRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ledRight.BackColor = System.Drawing.SystemColors.Control;
            this.ledRight.Location = new System.Drawing.Point(13, 220);
            this.ledRight.Name = "ledRight";
            this.ledRight.Size = new System.Drawing.Size(197, 4);
            this.ledRight.TabIndex = 9;
            this.ledRight.TabStop = false;
            // 
            // listDevices
            // 
            this.listDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listDevices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listDevices.FullRowSelect = true;
            this.listDevices.HideSelection = false;
            this.listDevices.Location = new System.Drawing.Point(1, -1);
            this.listDevices.MultiSelect = false;
            this.listDevices.Name = "listDevices";
            this.listDevices.Size = new System.Drawing.Size(220, 437);
            this.listDevices.TabIndex = 0;
            this.listDevices.TileSize = new System.Drawing.Size(222, 40);
            this.listDevices.UseCompatibleStateImageBehavior = false;
            this.listDevices.View = System.Windows.Forms.View.Tile;
            this.listDevices.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listDevices_KeyDown);
            this.listDevices.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listDevices_KeyUp);
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormSwitcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 234);
            this.ControlBox = false;
            this.Controls.Add(this.ledRight);
            this.Controls.Add(this.ledLeft);
            this.Controls.Add(this.VolBar);
            this.Controls.Add(this.pictureItemsBack);
            this.Controls.Add(this.listDevices);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSwitcher";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.FormSwitcher_Deactivate);
            ((System.ComponentModel.ISupportInitialize)(this.pictureItemsBack)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox pictureItemsBack;
        private Timer timer1;
        private VolumeBar VolBar;
        private LedBar ledLeft;
        private LedBar ledRight;
    }
}