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
    public class KurzanMap2 : KurzanBase
    {
        public KurzanMap2(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ResourceHelper rh, AppLogger logger) : base("KurzanMap2", settings, mouse, kb, rh, logger)
        {
            PreferredMovementArea = IP.ConvertStringCoordsToRect(_rh["KurzanMap2_PreferredArea"]);
        }

        public override void StartMapMove()
        {
            _logger.Log(LogDetailLevel.Debug, "Kurzan Map 2 Initial Moves");
            var basePoint = IP.GetPointFromStringCoords(_rh["KurzanMap2_Start"]);
            _mouse.ClickPosition(basePoint, 2000, MouseButtons.Right);
            _mouse.ClickPosition(basePoint, 2000, MouseButtons.Right);
            _mouse.ClickPosition(basePoint, 2000, MouseButtons.Right);
        }
    }
}
