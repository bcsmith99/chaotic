using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IP = Chaotic.Utilities.ImageProcessing;


namespace Chaotic.Tasks.Chaos.Kurzan
{
    internal class KurzanMap3 : KurzanBase
    {
        public KurzanMap3(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ResourceHelper rh, AppLogger logger) : base("KurzanMap3", settings, mouse, kb, rh, logger)
        {
            PreferredMovementArea = IP.ConvertStringCoordsToRect(_rh["KurzanMap3_PreferredArea"]);
        }

        public override void StartMapMove()
        {
            _logger.Log(LogDetailLevel.Debug, "Kurzan Map 3 Initial Move");
            var basePoint = IP.GetPointFromStringCoords(_rh["KurzanMap3_Start"]);
            _mouse.ClickPosition(basePoint, 2000, MouseButtons.Right);
            _mouse.ClickPosition(basePoint, 2000, MouseButtons.Right);
        }

        public override ScreenSearchResult CheckIfStuck()
        {
            var region = IP.ConvertStringCoordsToRect(_rh["KurzanMap3_Stickpoint_Region"]);
            var stickPoint = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("kurzan_map3_stickpoint.png", _settings.Resolution), region, .65);
            return stickPoint;

        }
    }
}
