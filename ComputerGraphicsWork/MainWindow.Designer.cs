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
            this.buttonSaveBitmap = new System.Windows.Forms.ToolStripButton();
            this.buttonMoveGraphics = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawRotation = new System.Windows.Forms.ToolStripButton();
            this.buttonAdjustGraphics = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawPoint = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawLine = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawCircle = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawEllipse = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawPolygon = new System.Windows.Forms.ToolStripButton();
            this.commandHolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // commandHolder
            // 
            this.commandHolder.Dock = System.Windows.Forms.DockStyle.Left;
            this.commandHolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSaveBitmap,
            this.buttonMoveGraphics,
            this.buttonDrawRotation,
            this.buttonAdjustGraphics,
            this.buttonDrawPoint,
            this.buttonDrawLine,
            this.buttonDrawCircle,
            this.buttonDrawEllipse,
            this.buttonDrawPolygon});
            this.commandHolder.Location = new System.Drawing.Point(0, 0);
            this.commandHolder.Name = "commandHolder";
            this.commandHolder.Size = new System.Drawing.Size(33, 381);
            this.commandHolder.Stretch = true;
            this.commandHolder.TabIndex = 0;
            this.commandHolder.Text = "commandHolder";
            this.commandHolder.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.commandHolder_ItemClicked);
            // 
            // buttonSaveBitmap
            // 
            this.buttonSaveBitmap.AutoSize = false;
            this.buttonSaveBitmap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSaveBitmap.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveBitmap.Image")));
            this.buttonSaveBitmap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveBitmap.Name = "buttonSaveBitmap";
            this.buttonSaveBitmap.Size = new System.Drawing.Size(32, 32);
            this.buttonSaveBitmap.Text = "buttonSaveBitmap";
            this.buttonSaveBitmap.ToolTipText = "save";
            this.buttonSaveBitmap.Click += new System.EventHandler(this.buttonSaveBitmap_Click);
            // 
            // buttonMoveGraphics
            // 
            this.buttonMoveGraphics.AutoSize = false;
            this.buttonMoveGraphics.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveGraphics.Image = ((System.Drawing.Image)(resources.GetObject("buttonMoveGraphics.Image")));
            this.buttonMoveGraphics.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveGraphics.Name = "buttonMoveGraphics";
            this.buttonMoveGraphics.Size = new System.Drawing.Size(32, 32);
            this.buttonMoveGraphics.Text = "buttonMoveGraphics";
            this.buttonMoveGraphics.ToolTipText = "move";
            this.buttonMoveGraphics.Click += new System.EventHandler(this.buttonMoveGraphics_Click);
            // 
            // buttonDrawRotation
            // 
            this.buttonDrawRotation.AutoSize = false;
            this.buttonDrawRotation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawRotation.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawRotation.Image")));
            this.buttonDrawRotation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawRotation.Name = "buttonDrawRotation";
            this.buttonDrawRotation.Size = new System.Drawing.Size(32, 32);
            this.buttonDrawRotation.Text = "buttonDrawRotation";
            this.buttonDrawRotation.ToolTipText = "rotation";
            this.buttonDrawRotation.Click += new System.EventHandler(this.buttonDrawRotation_Click);
            // 
            // buttonAdjustGraphics
            // 
            this.buttonAdjustGraphics.AutoSize = false;
            this.buttonAdjustGraphics.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAdjustGraphics.Image = ((System.Drawing.Image)(resources.GetObject("buttonAdjustGraphics.Image")));
            this.buttonAdjustGraphics.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAdjustGraphics.Name = "buttonAdjustGraphics";
            this.buttonAdjustGraphics.Size = new System.Drawing.Size(32, 32);
            this.buttonAdjustGraphics.Text = "buttonAdjustGraphics";
            this.buttonAdjustGraphics.ToolTipText = "adjust";
            this.buttonAdjustGraphics.Click += new System.EventHandler(this.buttonAdjustGraphics_Click);
            // 
            // buttonDrawPoint
            // 
            this.buttonDrawPoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawPoint.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawPoint.Image")));
            this.buttonDrawPoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawPoint.Name = "buttonDrawPoint";
            this.buttonDrawPoint.Size = new System.Drawing.Size(30, 20);
            this.buttonDrawPoint.Text = "buttonDrawPoint";
            this.buttonDrawPoint.ToolTipText = "point";
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
            this.buttonDrawLine.ToolTipText = "line";
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
            this.buttonDrawCircle.ToolTipText = "circle";
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
            this.buttonDrawEllipse.ToolTipText = "ellipse";
            this.buttonDrawEllipse.Click += new System.EventHandler(this.buttonDrawEllipse_Click);
            // 
            // buttonDrawPolygon
            // 
            this.buttonDrawPolygon.AutoSize = false;
            this.buttonDrawPolygon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawPolygon.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawPolygon.Image")));
            this.buttonDrawPolygon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawPolygon.Name = "buttonDrawPolygon";
            this.buttonDrawPolygon.Size = new System.Drawing.Size(32, 32);
            this.buttonDrawPolygon.Text = "buttonDrawPolygon";
            this.buttonDrawPolygon.ToolTipText = "polygon";
            this.buttonDrawPolygon.Click += new System.EventHandler(this.buttonDrawPolygon_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(635, 381);
            this.Controls.Add(this.commandHolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_OverideLoad);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainWindow_Paint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainWindow_KeyPress);
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
        private System.Windows.Forms.ToolStripButton buttonMoveGraphics;
        private System.Windows.Forms.ToolStripButton buttonDrawPoint;
        private System.Windows.Forms.ToolStripButton buttonSaveBitmap;
        private System.Windows.Forms.ToolStripButton buttonAdjustGraphics;
        private System.Windows.Forms.ToolStripButton buttonDrawRotation;
        private System.Windows.Forms.ToolStripButton buttonDrawPolygon;
    }
}