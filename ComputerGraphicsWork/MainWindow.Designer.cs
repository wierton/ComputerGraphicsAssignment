using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace ComputerGraphicsWork
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.commandHolder = new System.Windows.Forms.ToolStrip();
            this.buttonDrawPoint = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawLine = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawCircle = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawEllipse = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawNothing = new System.Windows.Forms.ToolStripButton();
            this.commandHolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // commandHolder
            // 
            this.commandHolder.Dock = System.Windows.Forms.DockStyle.Left;
            this.commandHolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonDrawPoint,
            this.buttonDrawLine,
            this.buttonDrawCircle,
            this.buttonDrawEllipse,
            this.buttonDrawNothing});
            this.commandHolder.Location = new System.Drawing.Point(0, 0);
            this.commandHolder.Name = "commandHolder";
            this.commandHolder.Size = new System.Drawing.Size(33, 381);
            this.commandHolder.Stretch = true;
            this.commandHolder.TabIndex = 0;
            this.commandHolder.Text = "commandHolder";
            this.commandHolder.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.commandHolder_ItemClicked);
            // 
            // buttonDrawPoint
            // 
            this.buttonDrawPoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawPoint.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawPoint.Image")));
            this.buttonDrawPoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawPoint.Name = "buttonDrawPoint";
            this.buttonDrawPoint.Size = new System.Drawing.Size(30, 20);
            this.buttonDrawPoint.Text = "buttonDrawPoint";
            this.buttonDrawPoint.Click += new System.EventHandler(this.buttonDrawPoint_Click);
            // 
            // buttonDrawLine
            // 
            this.buttonDrawLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawLine.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawLine.Image")));
            this.buttonDrawLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawLine.Name = "buttonDrawLine";
            this.buttonDrawLine.Size = new System.Drawing.Size(30, 20);
            this.buttonDrawLine.Text = "buttonDrawLine";
            this.buttonDrawLine.ToolTipText = "buttonDrawLine";
            this.buttonDrawLine.Click += new System.EventHandler(this.buttonDrawLine_Click);
            // 
            // buttonDrawCircle
            // 
            this.buttonDrawCircle.AutoSize = false;
            this.buttonDrawCircle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawCircle.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawCircle.Image")));
            this.buttonDrawCircle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawCircle.Name = "buttonDrawCircle";
            this.buttonDrawCircle.Size = new System.Drawing.Size(32, 32);
            this.buttonDrawCircle.Text = "buttonDrawCircle";
            this.buttonDrawCircle.Click += new System.EventHandler(this.buttonDrawCircle_Click);
            // 
            // buttonDrawEllipse
            // 
            this.buttonDrawEllipse.AutoSize = false;
            this.buttonDrawEllipse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawEllipse.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawEllipse.Image")));
            this.buttonDrawEllipse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawEllipse.Name = "buttonDrawEllipse";
            this.buttonDrawEllipse.Size = new System.Drawing.Size(32, 32);
            this.buttonDrawEllipse.Text = "buttonDrawEllipse";
            this.buttonDrawEllipse.ToolTipText = "buttonDrawEllipse";
            this.buttonDrawEllipse.Click += new System.EventHandler(this.buttonDrawEllipse_Click);
            // 
            // buttonDrawNothing
            // 
            this.buttonDrawNothing.AutoSize = false;
            this.buttonDrawNothing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawNothing.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawNothing.Image")));
            this.buttonDrawNothing.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawNothing.Name = "buttonDrawNothing";
            this.buttonDrawNothing.Size = new System.Drawing.Size(32, 32);
            this.buttonDrawNothing.Text = "buttonDrawNothing";
            this.buttonDrawNothing.Click += new System.EventHandler(this.buttonDrawNothing_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(635, 381);
            this.Controls.Add(this.commandHolder);
            this.KeyPreview = true;
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_OverideLoad);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainWindow_Paint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainWindow_KeyPress);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseUp);
            this.commandHolder.ResumeLayout(false);
            this.commandHolder.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip commandHolder;
        private System.Windows.Forms.ToolStripButton buttonDrawLine;
        private System.Windows.Forms.ToolStripButton buttonDrawCircle;
        private System.Windows.Forms.ToolStripButton buttonDrawEllipse;
        private System.Windows.Forms.ToolStripButton buttonDrawNothing;
        private System.Windows.Forms.ToolStripButton buttonDrawPoint;
    }
}