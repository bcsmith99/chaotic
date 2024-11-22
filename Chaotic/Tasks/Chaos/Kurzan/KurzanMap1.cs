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

        public KurzanMap1(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ResourceHelper rh, AppLogger logger) : base("KurzanMap1", settings, mouse, kb, rh, logger)
        {
            PreferredMovementArea = IP.ConvertStringCoordsToRect(_rh["KurzanMap1_PreferredArea"]);
            ClickableRegion = IP.ConvertStringCoordsToRect(_rh["Clickable_Region"]);
        }
        public override void StartMapMove()
        {
            _logger.Log(LogDetailLevel.Debug, "Kurzan Map 1 Initial Move");
            var startPoints = IP.ConvertPointArray(_rh["KurzanMap1_Start"]);

            _mouse.ClickPosition(startPoints[0], 2500, MouseButtons.Right);
            _mouse.ClickPosition(startPoints[1], 2000, MouseButtons.Right);

        }
        public override void PerformSpecialChecks()
        {
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
                jumpPad = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation(image, _settings.Resolution), confidence: .85);
                if (jumpPad.Found)
                {
                    //IP.SAVE_DEBUG_IMAGES = false;
                    break;
                }
            }
            //Look for jump portal and take it.
            if (jumpPad.Found)
            {
                _logger.Log(LogDetailLevel.Debug, $"Kurzan Jump Pad Found - {jumpPad.MaxConfidence}");
                _mouse.ClickPosition(jumpPad.CenterX, jumpPad.CenterY + 50, 200, MouseButtons.Right);
                for (int i = 0; i < 8; i++)
                {
                    _kb.Press(Key.G, 200);
                }

                PreferredMovementArea = IP.ConvertStringCoordsToRect(_rh["Clickable_Region"]);
            }
        }
    }
}
