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
            var basePoint = IP.GetPointFromStringCoords(_rh["KurzanMap1_Start"]);
            _mouse.ClickPosition(basePoint, 2000, MouseButtons.Right);

        }
        public override void PerformSpecialChecks()
        {
            //Look for jump portal and take it.
            var jump_pad = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("kurzan_map1_jumppoint.png", _settings.Resolution), ClickableRegion, confidence: .8);
            if (jump_pad.Found)
            {
                _logger.Log(LogDetailLevel.Debug, $"Kurzan Jump Pad Found - {jump_pad.MaxConfidence}");
                _mouse.ClickPosition(jump_pad.CenterX, jump_pad.CenterY + 50, 200, MouseButtons.Right);
                for (int i = 0; i < 8; i++)
                {
                    _kb.Press(Key.G, 200);
                }

                PreferredMovementArea = IP.ConvertStringCoordsToRect(_rh["Clickable_Region"]);
            }
        }
    }
}
