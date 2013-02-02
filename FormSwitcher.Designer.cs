using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AudioSwitch.CoreAudioApi;

namespace AudioSwitch
{
    partial class FormSwitcher
    {
        private CustomListView listView1;
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
            this.components = new Container();
            this.notifyIcon1 = new NotifyIcon(this.components);
            this.pictureBox1 = new PictureBox();
            this.timer1 = new Timer(this.components);
            this.ledRight = new LedBar();
            this.ledLeft = new LedBar();
            this.Volume = new VolumeBar();
            this.listView1 = new CustomListView();
            ((ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "AudioSwitch";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDown += new MouseEventHandler(this.notifyIcon1_MouseDown);
            this.notifyIcon1.MouseMove += new MouseEventHandler(this.notifyIcon1_MouseMove);
            this.notifyIcon1.MouseUp += new MouseEventHandler(this.notifyIcon1_MouseUp);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = AnchorStyles.Bottom;
            this.pictureBox1.Location = new Point(0, 191);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(223, 44);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new PaintEventHandler(this.pictureBox1_Paint);
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new EventHandler(this.timer1_Tick);
            // 
            // ledRight
            // 
            this.ledRight.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.ledRight.BackColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ledRight.Location = new Point(12, 219);
            this.ledRight.Name = "ledRight";
            this.ledRight.Size = new Size(196, 5);
            this.ledRight.TabIndex = 9;
            this.ledRight.TabStop = false;
            // 
            // ledLeft
            // 
            this.ledLeft.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.ledLeft.BackColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ledLeft.Location = new Point(12, 203);
            this.ledLeft.Name = "ledLeft";
            this.ledLeft.Size = new Size(196, 5);
            this.ledLeft.TabIndex = 8;
            this.ledLeft.TabStop = false;
            // 
            // Volume
            // 
            this.Volume.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.Volume.Location = new Point(12, 209);
            this.Volume.Name = "Volume";
            this.Volume.Size = new Size(196, 10);
            this.Volume.TabIndex = 7;
            this.Volume.TabStop = false;
            // 
            // listView1
            // 
            this.listView1.BorderStyle = BorderStyle.None;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new Point(1, -1);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new Size(222, 437);
            this.listView1.TabIndex = 0;
            this.listView1.TileSize = new Size(222, 40);
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = View.Tile;
            this.listView1.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            this.listView1.KeyDown += new KeyEventHandler(this.listView1_KeyDown);
            this.listView1.KeyUp += new KeyEventHandler(this.listView1_KeyUp);
            // 
            // FormSwitcher
            // 
            this.ClientSize = new Size(220, 234);
            this.ControlBox = false;
            this.Controls.Add(this.ledRight);
            this.Controls.Add(this.ledLeft);
            this.Controls.Add(this.Volume);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listView1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSwitcher";
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.Deactivate += new EventHandler(this.FormSwitcher_Deactivate);
            this.Load += new EventHandler(this.FormSwitcher_Load);
            ((ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox pictureBox1;
        private Timer timer1;
        private VolumeBar Volume;
        private LedBar ledLeft;
        private LedBar ledRight;
    }
}