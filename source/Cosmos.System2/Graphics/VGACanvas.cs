using static Cosmos.HAL.Drivers.Video.VGADriver;
using Cosmos.HAL.Drivers.Video;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace Cosmos.System.Graphics
{
    /// <summary>
    /// Defines a VGA canvas implementation.
    /// </summary>
    public class VGACanvas : Canvas
    {
        bool enabled;
        readonly VGADriver driver;

        /// <summary>
        /// Available VGA supported video modes.
        /// <para>
        /// Low color pallete:
        /// <list type="bullet">
        /// <item>320x200x4.</item>
        /// <item>640x480x2.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Normal colors:
        /// <list type="bullet">
        /// <item>320x200x8.</item>
        /// <item>640x480x4.</item>
        /// <item>720x480x16.</item>
        /// </list>
        /// </para>
        /// </summary>
        static readonly List<Mode> availableModes = new()
        {
            new Mode(640, 480, ColorDepth.ColorDepth4),
            new Mode(720, 480, ColorDepth.ColorDepth4),
            new Mode(320, 200, ColorDepth.ColorDepth8)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="VGACanvas"/> class
        /// with the given display mode.
        /// </summary>
        public VGACanvas(Mode mode) : base(mode)
        {
            driver = new VGADriver();
            driver.SetGraphicsMode(ModeToScreenSize(mode), (VGADriver.ColorDepth)(int)mode.ColorDepth);
            Mode = mode;
            Enabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VGACanvas"/> class
        /// with the default display mode.
        /// </summary>
        public VGACanvas() : base()
        {
            Enabled = true;
            Mode = DefaultGraphicsMode;
            driver = new VGADriver();
            driver.SetGraphicsMode(ModeToScreenSize(DefaultGraphicsMode), (VGADriver.ColorDepth)(int)DefaultGraphicsMode.ColorDepth);
        }

        public override string Name() => "VGACanvas";

        public override Mode Mode { get; set; }

        public override void Clear(int aColor)
        {
            driver.DrawFilledRectangle(0, 0, driver.PixelWidth, driver.PixelHeight, (uint)aColor);
        }

        public override void Clear(Color aColor)
        {
            var paletteIndex = driver.GetClosestColorInPalette(aColor);
            driver.DrawFilledRectangle(0, 0, driver.PixelWidth, driver.PixelHeight, paletteIndex);
        }

        public override void Disable()
        {
            if (Enabled)
            {
                Enabled = false;
            }
        }

        public override void DrawFilledRectangle(Color aColor, int aXStart, int aYStart, int aWidth, int aHeight, bool preventOffBoundPixels = true)
        {
            if (preventOffBoundPixels)
            {
                aWidth = Math.Min(aWidth, (int)Mode.Width - aXStart);
                aHeight = Math.Min(aHeight, (int)Mode.Height - aYStart);
            }
            driver.DrawFilledRectangle(aXStart, aYStart, aWidth, aHeight, driver.GetClosestColorInPalette(aColor));
        }

        public override void DrawRectangle(Color color, int x, int y, int width, int height)
        {
            int rawColor = color.ToArgb();

            /* Draw the top edge (A to B) */
            for (int posX = x; posX < x + width; posX++)
            {
                DrawRawPoint((uint)rawColor, posX, y);
            }

            /* Draw the bottom edge (C to D) */
            int newY = y + height;
            for (int posX = x; posX < x + width; posX++)
            {
                DrawRawPoint((uint)rawColor, posX, newY);
            }

            /* Draw the left edge (A to C) */
            for (int posY = y; posY < y + height; posY++)
            {
                DrawRawPoint((uint)rawColor, x, posY);
            }

            /* Draw the right edge (B to D) */
            int newX = x + width;
            for (int posY = y; posY < y + height; posY++)
            {
                DrawRawPoint((uint)rawColor, newX, posY);
            }
        }

        public override void DrawPoint(Color aColor, int aX, int aY)
        {
            driver.SetPixel((uint)aX, (uint)aY, aColor);
        }

        public void DrawPoint(uint aColor, int aX, int aY)
        {
            driver.SetPixel((uint)aX, (uint)aY, aColor);
        }

        public override void DrawRawPoint(uint aColor, int aX, int aY)
        {
            driver.SetPixel((uint)aX, (uint)aY, aColor);
        }

        public override List<Mode> AvailableModes => availableModes;

        public override Color GetPointColor(int aX, int aY)
        {
            return Color.FromArgb((int)driver.GetPixel((uint)aX, (uint)aY));
        }

        public override Mode DefaultGraphicsMode => new Mode(640, 480, ColorDepth.ColorDepth4);

        /// <summary>
        /// Whether the canvas is active, and the display is currently in VGA
        /// graphics mode.
        /// </summary>
        public bool Enabled { get => enabled; private set => enabled = value; }

        private static ScreenSize ModeToScreenSize(Mode aMode)
        {
            if (aMode.Width == 320 && aMode.Height == 200)
            {
                return ScreenSize.Size320x200;
            }
            else if (aMode.Width == 640 && aMode.Height == 480)
            {
                return ScreenSize.Size640x480;
            }
            else if (aMode.Width == 720 && aMode.Height == 480)
            {
                return ScreenSize.Size720x480;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override void Display()
        {

        }
    }
}
