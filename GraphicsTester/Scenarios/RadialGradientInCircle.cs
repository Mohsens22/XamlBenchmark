﻿using Microsoft.Maui.Graphics;

namespace GraphicsTester.Scenarios
{
    public class RadialGradientInCircle : AbstractScenario
    {
        public RadialGradientInCircle() : base(720, 1024)
        {
        }

        public override void Draw(ICanvas canvas)
        {
            canvas.SaveState();
            
            var paint = new RadialGradientPaint
            {
                StartColor = Colors.White,
                EndColor = Colors.Black
            };

            canvas.SetFillPaint(paint, new RectangleF(200, 200, 300, 200));
            canvas.FillEllipse(100, 100, 200, 200);

            canvas.SetFillPaint(paint, new RectangleF(250, 500, 100, 500));
            canvas.FillEllipse(100, 400, 200, 200);

            canvas.RestoreState();
        }
    }
}