using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Notification
{
    public class NotificationForm : Form
    {
        const int MARGIN_TOP = 5; //Top margin of the form
        private Panel _leftPanel;
        private Panel _rightPanel;
        private Label _lblClose;
        private Label _MessageContainer;
        private int _height;
        private static int _lastY = 0;
        private static int _lastX = 0;
        private static List<Form> _openedFormPosition=new List<Form>(); //Contient la position de tous les formulaires ouverts
        private Point _mouseOffset;
        private Timer _opacityChanger;
        private Timer _displayer;


        private int CurrentDuration;
        public int InitialDuration { get; set; }
        public NotificationForm(string message = "",int duration=10000, IconStyle style = IconStyle.Notification)
        {
            this.Size = new System.Drawing.Size(400, 70);
           
            this.FormBorderStyle = FormBorderStyle.None;
            this.Font = new System.Drawing.Font("Calibri", 13, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.BackColor = System.Drawing.Color.FromArgb(42, 42, 42);
            this.ForeColor = System.Drawing.Color.White;
            //Positionnement
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            this.ClientSize = this.Size;
            int left = workingArea.Width - this.Width-_lastX;
            int top = workingArea.Height  - _lastY;
            _openedFormPosition.Add(this);
            _lastY += this.Height + MARGIN_TOP;
            if(_lastY>=(workingArea.Height-this.Height))
            {
                _lastY = 0;
                _lastX += this.Width+ MARGIN_TOP;
            }
            if(_lastX+this.Width>=workingArea.Width)
            {
                _lastX = 0;
            }
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(left, top);
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Text = "Notification";
            this.TopMost = true;
            this.Shown += new EventHandler(this.frm_Shown);

            //Left panel
            _leftPanel = new Panel();
            _leftPanel.Size = new Size(70, 70);
            _leftPanel.BackColor = Color.Transparent;
            _leftPanel.Dock = DockStyle.Left;
            _leftPanel.BackgroundImageLayout = ImageLayout.Stretch;
            _leftPanel.BackgroundImage = getIconImage(style);
            this.Controls.Add(_leftPanel);

            #region Right Panel
            //Right Panel
            _rightPanel = new Panel();
            _rightPanel.Size = new System.Drawing.Size(this.Width - _leftPanel.Width, this.Height);
            _rightPanel.BackColor = System.Drawing.Color.Transparent;
            _rightPanel.Dock = DockStyle.Right;
            this.Controls.Add(_rightPanel);
            //Label MessageContainer
            _MessageContainer = new Label();
            _MessageContainer.Text = message;
            _MessageContainer.Dock = DockStyle.Fill;
            _MessageContainer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _MessageContainer.Padding = new Padding(20, 20, 10, 10);
            _MessageContainer.AutoEllipsis = true;
            _rightPanel.Controls.Add(_MessageContainer);
            #endregion

            //Close Label
            _lblClose = new Label();
            _lblClose.Text = "x";
            _lblClose.ForeColor = System.Drawing.Color.White;
            _lblClose.Click += new EventHandler(this.lblClose_Click);
           _lblClose.BackColor = System.Drawing.Color.Transparent;

            _MessageContainer.Controls.Add(_lblClose);
            _lblClose.Location = new System.Drawing.Point(_MessageContainer.Width - 20, 0);
            //Timer
            _opacityChanger = new Timer();
            _opacityChanger.Interval = 100;
            _opacityChanger.Enabled = false;
            _opacityChanger.Tick += new EventHandler(this.opacityChanger_Tick);

            _displayer = new Timer();
            _displayer.Interval = 2;
            _displayer.Enabled = false;
            _displayer.Tick += new EventHandler(this.displayer_Tick);

            //Duration setting
            InitialDuration = duration;//in millisecond
            CurrentDuration = InitialDuration;
            //Form Moving
            _MessageContainer.MouseDown += new MouseEventHandler(frm_mouseDown);
            _MessageContainer.MouseMove += new MouseEventHandler(frm_mouseMove);
            //Mouse hoving on the form
            _MessageContainer.MouseHover += new EventHandler(this.frm_mouseHover);
            _rightPanel.MouseHover += new EventHandler(this.frm_mouseHover);
            _MessageContainer.MouseLeave += new EventHandler(this.frm_mouseLeave);
            _rightPanel.MouseLeave += new EventHandler(this.frm_mouseLeave);
        }

        private void frm_mouseHover(object sender, EventArgs e)
        {
            CurrentDuration = InitialDuration;
            this.Opacity = 1F;
            _opacityChanger.Enabled = false;
        }
        private void frm_mouseLeave(object sender, EventArgs e)
        {          
            _opacityChanger.Enabled = true;
        }
        private void frm_Shown(object sender, EventArgs e)
        {
            _height = this.Height;
            _displayer.Enabled = true;
        }
        private void displayer_Tick(object sender, EventArgs e)
        {

            Top -= 5;
            _height -= 5;
            if (_height <= 0)
            {
                _displayer.Enabled = false;
                _opacityChanger.Enabled = true;                
            }
        }
        private void opacityChanger_Tick(object sender, EventArgs e)
        {
            if(CurrentDuration<=5000)
            {
                this.Opacity -= 0.02F;
            }
            CurrentDuration -= 100;
            if (CurrentDuration == 0)
            {
                _opacityChanger.Enabled = false;
                int min = Screen.PrimaryScreen.WorkingArea.Height - this.Width- MARGIN_TOP; //Position minimale
                for (int i = 0; i < _openedFormPosition.Count; i++)
                {
                    
                    if (_openedFormPosition[i].Location.X <= min)
                    {
                        _lastY = i  * MARGIN_TOP;
                        if (_openedFormPosition[i].Left <= _lastX)
                        {
                            _lastX -= _openedFormPosition[i].DesktopLocation.X - this.Width - MARGIN_TOP;
                        }
                        break;
                    }
                }
                _openedFormPosition.Remove(this);
                this.Dispose(); //CLean memory and close form
            }
        }

        #region Form Moving 
        private void frm_mouseDown(object sender, MouseEventArgs e)
        {
            _mouseOffset = new Point(-e.X - _leftPanel.Width, -e.Y);
        }

        private void frm_mouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point MousePosition = Control.MousePosition;
                 MousePosition.Offset(_mouseOffset.X, _mouseOffset.Y);
                this.Location = MousePosition;
            }
        }
        #endregion


        private void lblClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private System.Drawing.Image getIconImage(IconStyle style)
        {
            System.Drawing.Image img=null;
            switch(style)
            {
                case IconStyle.Error: img = Properties.Resources.error_icon; break;
                case IconStyle.Help: img = Properties.Resources.help_icon; break;
                case IconStyle.Information: img = Properties.Resources.information_icon; break;
                case IconStyle.Notification: img = Properties.Resources.notification_icon; break;
                case IconStyle.Success: img = Properties.Resources.success_icon; break;
                case IconStyle.Warning: img = Properties.Resources.warning_icon; break;
            }
            return img;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // NotificationForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "NotificationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }
    }
    public enum IconStyle
    {
        Notification = 0,
        Information,
        Success,
        Error,
        Warning,
        Help
    }

}

