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
            this.buttonOpenFile = new System.Windows.Forms.ToolStripButton();
            this.buttonSaveBitmap = new System.Windows.Forms.ToolStripButton();
            this.buttonMoveGraphics = new System.Windows.Forms.ToolStripButton();
            this.buttonRotation = new System.Windows.Forms.ToolStripButton();
            this.buttonZoomGraphics = new System.Windows.Forms.ToolStripButton();
            this.buttonAdjustGraphics = new System.Windows.Forms.ToolStripButton();
            this.buttonColoring = new System.Windows.Forms.ToolStripButton();
            this.buttonInternalColoring = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawPoint = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawLine = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawRectangle = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawCircle = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawEllipse = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawPolygon = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawBezier = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawBStyleCurve = new System.Windows.Forms.ToolStripButton();
            this.buttonTrim = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawCube = new System.Windows.Forms.ToolStripButton();
            this.commandHolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // commandHolder
            // 
            this.commandHolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonOpenFile,
            this.buttonSaveBitmap,
            this.buttonMoveGraphics,
            this.buttonRotation,
            this.buttonZoomGraphics,
            this.buttonAdjustGraphics,
            this.buttonColoring,
            this.buttonInternalColoring,
            this.buttonDrawPoint,
            this.buttonDrawLine,
            this.buttonDrawRectangle,
            this.buttonDrawCircle,
            this.buttonDrawEllipse,
            this.buttonDrawPolygon,
            this.buttonDrawBezier,
            this.buttonDrawBStyleCurve,
            this.buttonTrim,
            this.buttonDrawCube});
            this.commandHolder.Location = new System.Drawing.Point(0, 0);
            this.commandHolder.Name = "commandHolder";
            this.commandHolder.Size = new System.Drawing.Size(635, 35);
            this.commandHolder.Stretch = true;
            this.commandHolder.TabIndex = 0;
            this.commandHolder.Text = "commandHolder";
            this.commandHolder.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.commandHolder_ItemClicked);
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.AutoSize = false;
            this.buttonOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonOpenFile.Image")));
            this.buttonOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(32, 32);
            this.buttonOpenFile.Text = "buttonOpenFile";
            this.buttonOpenFile.ToolTipText = "open cg file";
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
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
            // buttonRotation
            // 
            this.buttonRotation.AutoSize = false;
            this.buttonRotation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRotation.Image = ((System.Drawing.Image)(resources.GetObject("buttonRotation.Image")));
            this.buttonRotation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRotation.Name = "buttonRotation";
            this.buttonRotation.Size = new System.Drawing.Size(32, 32);
            this.buttonRotation.Text = "buttonRotation";
            this.buttonRotation.ToolTipText = "rotation";
            this.buttonRotation.Click += new System.EventHandler(this.buttonRotation_Click);
            // 
            // buttonZoomGraphics
            // 
            this.buttonZoomGraphics.AutoSize = false;
            this.buttonZoomGraphics.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonZoomGraphics.Image = ((System.Drawing.Image)(resources.GetObject("buttonZoomGraphics.Image")));
            this.buttonZoomGraphics.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonZoomGraphics.Name = "buttonZoomGraphics";
            this.buttonZoomGraphics.Size = new System.Drawing.Size(32, 32);
            this.buttonZoomGraphics.Text = "zoom";
            this.buttonZoomGraphics.Click += new System.EventHandler(this.buttonZoomGraphics_Click);
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
            // buttonColoring
            // 
            this.buttonColoring.AutoSize = false;
            this.buttonColoring.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonColoring.Image = ((System.Drawing.Image)(resources.GetObject("buttonColoring.Image")));
            this.buttonColoring.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonColoring.Name = "buttonColoring";
            this.buttonColoring.Size = new System.Drawing.Size(32, 32);
            this.buttonColoring.Text = "coloring";
            this.buttonColoring.Click += new System.EventHandler(this.buttonColoring_Click);
            // 
            // buttonInternalColoring
            // 
            this.buttonInternalColoring.AutoSize = false;
            this.buttonInternalColoring.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonInternalColoring.Image = ((System.Drawing.Image)(resources.GetObject("buttonInternalColoring.Image")));
            this.buttonInternalColoring.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonInternalColoring.Name = "buttonInternalColoring";
            this.buttonInternalColoring.Size = new System.Drawing.Size(32, 32);
            this.buttonInternalColoring.Text = "internal coloring";
            this.buttonInternalColoring.Click += new System.EventHandler(this.buttonInternalColoring_Click);
            // 
            // buttonDrawPoint
            // 
            this.buttonDrawPoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawPoint.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawPoint.Image")));
            this.buttonDrawPoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawPoint.Name = "buttonDrawPoint";
            this.buttonDrawPoint.Size = new System.Drawing.Size(23, 32);
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
            this.buttonDrawLine.Size = new System.Drawing.Size(23, 32);
            this.buttonDrawLine.Text = "buttonDrawLine";
            this.buttonDrawLine.ToolTipText = "line";
            this.buttonDrawLine.Click += new System.EventHandler(this.buttonDrawLine_Click);
            // 
            // buttonDrawRectangle
            // 
            this.buttonDrawRectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawRectangle.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawRectangle.Image")));
            this.buttonDrawRectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawRectangle.Name = "buttonDrawRectangle";
            this.buttonDrawRectangle.Size = new System.Drawing.Size(23, 32);
            this.buttonDrawRectangle.Text = "buttonDrawRectangle";
            this.buttonDrawRectangle.ToolTipText = "rectangle";
            this.buttonDrawRectangle.Click += new System.EventHandler(this.buttonDrawRectangle_Click);
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
            // buttonDrawBezier
            // 
            this.buttonDrawBezier.AutoSize = false;
            this.buttonDrawBezier.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawBezier.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawBezier.Image")));
            this.buttonDrawBezier.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawBezier.Name = "buttonDrawBezier";
            this.buttonDrawBezier.Size = new System.Drawing.Size(32, 32);
            this.buttonDrawBezier.Text = "buttonDrawBezier";
            this.buttonDrawBezier.ToolTipText = "bezier";
            this.buttonDrawBezier.Click += new System.EventHandler(this.buttonDrawBezier_Click);
            // 
            // buttonDrawBStyleCurve
            // 
            this.buttonDrawBStyleCurve.AutoSize = false;
            this.buttonDrawBStyleCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawBStyleCurve.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawBStyleCurve.Image")));
            this.buttonDrawBStyleCurve.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawBStyleCurve.Name = "buttonDrawBStyleCurve";
            this.buttonDrawBStyleCurve.Size = new System.Drawing.Size(32, 32);
            this.buttonDrawBStyleCurve.Text = "buttonDrawBStyleCurve";
            this.buttonDrawBStyleCurve.ToolTipText = "B spline curve";
            this.buttonDrawBStyleCurve.Click += new System.EventHandler(this.buttonDrawBStyleCurve_Click);
            // 
            // buttonTrim
            // 
            this.buttonTrim.AutoSize = false;
            this.buttonTrim.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonTrim.Image = ((System.Drawing.Image)(resources.GetObject("buttonTrim.Image")));
            this.buttonTrim.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTrim.Name = "buttonTrim";
            this.buttonTrim.Size = new System.Drawing.Size(32, 32);
            this.buttonTrim.Text = "buttonTrim";
            this.buttonTrim.ToolTipText = "trim";
            this.buttonTrim.Click += new System.EventHandler(this.buttonTrim_Click);
            // 
            // buttonDrawCube
            // 
            this.buttonDrawCube.AutoSize = false;
            this.buttonDrawCube.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawCube.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawCube.Image")));
            this.buttonDrawCube.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawCube.Name = "buttonDrawCube";
            this.buttonDrawCube.Size = new System.Drawing.Size(32, 32);
            this.buttonDrawCube.Text = "buttonDrawCube";
            this.buttonDrawCube.ToolTipText = "cube";
            this.buttonDrawCube.Click += new System.EventHandler(this.buttonDrawCube_Click);
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
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
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
        private System.Windows.Forms.ToolStripButton buttonMoveGraphics;
        private System.Windows.Forms.ToolStripButton buttonDrawPoint;
        private System.Windows.Forms.ToolStripButton buttonSaveBitmap;
        private System.Windows.Forms.ToolStripButton buttonAdjustGraphics;
        private System.Windows.Forms.ToolStripButton buttonRotation;
        private System.Windows.Forms.ToolStripButton buttonDrawPolygon;
        private System.Windows.Forms.ToolStripButton buttonColoring;
        private System.Windows.Forms.ToolStripButton buttonZoomGraphics;
        private System.Windows.Forms.ToolStripButton buttonDrawBezier;
        private System.Windows.Forms.ToolStripButton buttonTrim;
        private System.Windows.Forms.ToolStripButton buttonDrawRectangle;
        private System.Windows.Forms.ToolStripButton buttonDrawBStyleCurve;
        private System.Windows.Forms.ToolStripButton buttonInternalColoring;
        private System.Windows.Forms.ToolStripButton buttonOpenFile;
        private System.Windows.Forms.ToolStripButton buttonDrawCube;
    }
}