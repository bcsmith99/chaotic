using Chaotic.Resources;
using Chaotic.Tasks.Chaos.Class;
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
        public KurzanMap3(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ApplicationResources rh, AppLogger logger) : base("KurzanMap3", settings, mouse, kb, rh, logger)
        {
            PreferredMovementArea = _r.KurzanMap3PreferredArea; // IP.ConvertStringCoordsToRect(_r["KurzanMap3_PreferredArea"]);
        }

        public override void StartMapMove(ChaosClass cc)
        {
            _logger.Log(LogDetailLevel.Debug, "Kurzan Map 3 Initial Move");
            var startPoints = _r.KurzanMap3Start;// IP.GetPointFromStringCoords(_r["KurzanMap3_Start"]);
            _mouse.ClickPosition(startPoints[0], 2000, MouseButtons.Right);
            _mouse.ClickPosition(startPoints[0], 2000, MouseButtons.Right);
        }

        public override ScreenSearchResult CheckIfStuck()
        {
            var region = _r.KurzanMap3StickingPoint; // IP.ConvertStringCoordsToRect(_r["KurzanMap3_Stickpoint_Region"]);
            var stickPoint = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("kurzan_map3_stickpoint.png", _settings.Resolution), region, .65);
            return stickPoint;

        }
    }
}
