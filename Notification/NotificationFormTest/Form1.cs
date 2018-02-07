using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Notification;
namespace NotificationFormTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            new NotificationForm("Nouveau produit en ripture de stock bonata",10000, IconStyle.Error).Show();
            new NotificationForm("Nouveau produit en ripture de stock bonata", 10000, IconStyle.Warning).Show();
            new NotificationForm("Nouveau produit en ripture de stock bonata", 10000, IconStyle.Information).Show();
            new NotificationForm("Nouveau produit en ripture de stock bonata", 10000, IconStyle.Help).Show();
            new NotificationForm("Nouveau produit en ripture de stock bonata", 10000, IconStyle.Success).Show();
            new NotificationForm("Nouveau produit en ripture de stock bonata", 10000, IconStyle.Notification).Show();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            Form1_Shown(null, null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
          
        }
    }
}
