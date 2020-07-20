﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

namespace Hive.IO.Plots
{
    /// <summary>
    /// MenuButtons are clickable regions that can render themselves to to a canvas.
    /// They are managed by the PlotSelector class.
    /// </summary>
    public class MenuButton: IVisualizerControl
    {
        private Pen _borderPen = new Pen(Color.FromArgb(217, 217, 217));
        private string _text;
        private Font _font;
        private Brush _textBrush = new SolidBrush(Color.Black);

        public MenuButton(string text): this(text, GH_FontServer.Standard)
        {
        }

        public MenuButton(string text, Font font)
        {
            _text = text;
            _font = font;
        }

        public void Render(Results results, Graphics graphics, RectangleF bounds)
        {
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            graphics.DrawString(_text, _font, _textBrush, bounds, format);
            graphics.DrawRectangleF(_borderPen, bounds);
        }

        public bool Contains(PointF location)
        {
            throw new NotImplementedException();
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
