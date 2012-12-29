using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AudioSwitch.CoreAudioApi;

namespace AudioSwitch
{
    partial class FormSwitcher
    {
        private Label label1;
        private ListView listView1;
        private NotifyIcon notifyIcon1;

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
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tbMaster = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.pgRight = new AudioSwitch.VerticalProgressBar();
            this.pgLeft = new AudioSwitch.VerticalProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaster)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDown);
            this.notifyIcon1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseMove);
            this.notifyIcon1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NotifyIconMouseUp);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Location = new System.Drawing.Point(181, 204);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Exit";
            this.label1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ExitOnMouseClick);
            this.label1.MouseEnter += new System.EventHandler(this.ExitOnMouseEnter);
            this.label1.MouseLeave += new System.EventHandler(this.ExitOnMouseLeave);
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(1, -1);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(222, 380);
            this.listView1.TabIndex = 0;
            this.listView1.TileSize = new System.Drawing.Size(222, 40);
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Tile;
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.DeviceListOnSelectionChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.pictureBox1.Location = new System.Drawing.Point(0, 191);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(223, 44);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tbMaster
            // 
            this.tbMaster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbMaster.Location = new System.Drawing.Point(0, 192);
            this.tbMaster.Maximum = 100;
            this.tbMaster.Name = "tbMaster";
            this.tbMaster.Size = new System.Drawing.Size(174, 45);
            this.tbMaster.TabIndex = 1;
            this.tbMaster.TickFrequency = 10;
            this.tbMaster.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbMaster.Scroll += new System.EventHandler(this.tbMaster_Scroll);
            this.tbMaster.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbMaster_MouseUp);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(200, 500);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 32);
            this.label2.TabIndex = 6;
            // 
            // pgRight
            // 
            this.pgRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pgRight.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pgRight.Location = new System.Drawing.Point(208, 500);
            this.pgRight.Name = "pgRight";
            this.pgRight.Size = new System.Drawing.Size(6, 30);
            this.pgRight.Step = 1;
            this.pgRight.TabIndex = 4;
            // 
            // pgLeft
            // 
            this.pgLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pgLeft.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pgLeft.Location = new System.Drawing.Point(201, 500);
            this.pgLeft.Name = "pgLeft";
            this.pgLeft.Size = new System.Drawing.Size(6, 30);
            this.pgLeft.Step = 1;
            this.pgLeft.TabIndex = 5;
            // 
            // FormSwitcher
            // 
            this.ClientSize = new System.Drawing.Size(220, 234);
            this.ControlBox = false;
            this.Controls.Add(this.pgRight);
            this.Controls.Add(this.pgLeft);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbMaster);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listView1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSwitcher";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.OnDeactivated);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaster)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pictureBox1;
        private Timer timer1;
        private VerticalProgressBar pgRight;
        private TrackBar tbMaster;
        private VerticalProgressBar pgLeft;
        private Label label2;
    }
}