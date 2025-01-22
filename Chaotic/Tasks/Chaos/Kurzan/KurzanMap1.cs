using Chaotic.Resources;
using Chaotic.Tasks.Chaos.Class;
using Chaotic.User;
using Chaotic.Utilities;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic.Tasks.Chaos.Kurzan
{
    public class KurzanMap1 : KurzanBase
    {
        public Rect ClickableRegion { get; }

        public KurzanMap1(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ApplicationResources rh, AppLogger logger) : base("KurzanMap1", settings, mouse, kb, rh, logger)
        {
            PreferredMovementArea = _r.KurzanMap1PreferredArea;
            ClickableRegion = _r.ClickableRegion;

        }
        public override void StartMapMove(ChaosClass cc)
        {
            _logger.Log(LogDetailLevel.Debug, "Kurzan Map 1 Initial Move");
            var startPoints = _r.KurzanMap1Start;

            _mouse.ClickPosition(startPoints[0], 2500, MouseButtons.Right);
            _mouse.ClickPosition(startPoints[1], 2000, MouseButtons.Right);

            cc.StartUp();
            //Use abilities here for a minute before proceeding - Go NE then SW then back NE. 
            var newPoint = new Point(CenterScreen.X + 200, CenterScreen.Y - 100);
            cc.UseAbilities(newPoint, 4);

            newPoint = new Point(CenterScreen.X - 200, CenterScreen.Y + 100);
            cc.UseAbilities(newPoint, 4);

            newPoint = new Point(CenterScreen.X + 200, CenterScreen.Y - 100);
            cc.UseAbilities(newPoint, 4);
        }

        public override void PerformSpecialChecks(DateTime startTime)
        {
            var currentTime = DateTime.Now;

            //Don't look for jump pad until 1.5mins in
            if (currentTime.Subtract(startTime).TotalSeconds < 90)
                return;
            //Don't update the movement area until after we have hopefully jumped.
            if (currentTime.Subtract(startTime).TotalSeconds >= 180)
                PreferredMovementArea = ClickableRegion;

            var jumpPadImages = new List<string>()
            {
                "kurzan_map1_jumppoint.png",
                "kurzan_map1_jumppoint1.png",
                "kurzan_map1_jumppoint2.png"
            };
            ScreenSearchResult jumpPad = new ScreenSearchResult();

            foreach (var image in jumpPadImages)
            {
                //IP.SAVE_DEBUG_IMAGES = true;
                jumpPad = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation(image, _settings.Resolution), confidence: .82);
                if (jumpPad.Found)
                {
                    //IP.SAVE_DEBUG_IMAGES = false;
                    break;
                }
            }
            //Look for jump portal and take it.
            if (jumpPad.Found)
            {
                var clickX = jumpPad.CenterX;
                var clickY = jumpPad.CenterY;

                if (clickX < ClickableRegion.Left)
                    clickX = ClickableRegion.Left;
                if (clickX > ClickableRegion.Right)
                    clickX = ClickableRegion.Right;
                if (clickY < ClickableRegion.Top)
                    clickY = ClickableRegion.Top;
                if (clickY > ClickableRegion.Bottom)
                    clickY = ClickableRegion.Bottom;

                _logger.Log(LogDetailLevel.Debug, $"Kurzan Jump Pad Found - {jumpPad.MaxConfidence}");
                _mouse.ClickPosition(clickX, clickY + 50, 200, MouseButtons.Right);
                for (int i = 0; i < 10; i++)
                {
                    _kb.Press(Key.G, 200);
                }
            }
        }

        public override ScreenSearchResult CheckMapRoute(DateTime startTime)
        {
            var images = new List<string>()
            {
                "kf_map1_preferredarea1.png",
                "kf_map1_preferredarea2.png",
                "kf_map1_preferredarea3.png",
            };
            var currentTime = DateTime.Now;

            var delta = currentTime.Subtract(startTime);

            ScreenSearchResult prefArea = new ScreenSearchResult() { Found = false };
            if (delta.TotalSeconds < 90)
            {
                prefArea = IP.LocateCenterOnScreen(Utility.ImageResourceLocation(images[0], _settings.Resolution), TopMinimapRegion, confidence: .8);
                if (prefArea.Found)
                    _logger.Log(LogDetailLevel.Debug, "Found Kurzan Map 1 Preferred Point 1");
            }
            else if (delta.TotalSeconds >= 90 && delta.TotalSeconds < 120)
            {
                //IP.SAVE_DEBUG_IMAGES = true;
                prefArea = IP.LocateCenterOnScreen(Utility.ImageResourceLocation(images[1], _settings.Resolution), TopMinimapRegion, confidence: .8);
                if (prefArea.Found)
                    _logger.Log(LogDetailLevel.Debug, "Found Kurzan Map 1 Preferred Point 2");
                //IP.SAVE_DEBUG_IMAGES = false;
            }
            else if (delta.TotalSeconds >= 120)
            {
                //IP.SAVE_DEBUG_IMAGES = true;
                prefArea = IP.LocateCenterOnScreen(Utility.ImageResourceLocation(images[2], _settings.Resolution), TopMinimapRegion, confidence: .8);
                if (prefArea.Found)
                    _logger.Log(LogDetailLevel.Debug, "Found Kurzan Map 1 Preferred Point 3");
                //IP.SAVE_DEBUG_IMAGES = false;
            }


            return prefArea;
        }
    }
}
