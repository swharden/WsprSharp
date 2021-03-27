using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WsprInspector
{
    public static class WsprImage
    {
        /// <summary>
        /// Generate a Bitmap showing a simulated spectrogram of the given levels
        /// </summary>
        /// <param name="levels">array of tone levels (numbers 0-3)</param>
        /// <param name="scaleX">each tone will be this number of horizontal pixels</param>
        /// <param name="scaleY">each tone will be this number of vertical pixels</param>
        public static Bitmap MakeSpectrogram(byte[] levels, int scaleX = 4, int scaleY = 10)
        {
            if (levels is null || levels.Length == 0)
                throw new ArgumentException("Levels must contain multiple values");

            if (levels.Min() < 0 || levels.Max() > 3)
                throw new ArgumentException("Levels must be values from 0 through 3");

            int width = levels.Length * scaleX;
            int height = 4 * scaleY;
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (Graphics gfx = Graphics.FromImage(bmp))
            using (Brush squareFill = new SolidBrush(Color.Yellow))
            using (Pen squareOutline = new Pen(Color.FromArgb(100, Color.Black)))
            {
                gfx.Clear(Color.Navy);

                for (int i = 0; i < levels.Length; i++)
                {
                    int levelPixelY = (3 - levels[i]) * scaleY;
                    Rectangle rect = new Rectangle(i * scaleX, levelPixelY, scaleX, scaleY);
                    gfx.FillRectangle(squareFill, rect);
                    gfx.DrawLine(squareOutline, rect.X, rect.Y, rect.X, rect.Y + scaleY);
                }
            }
            return bmp;
        }
    }
}
