﻿using Chaotic.Resources;
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
    public class KurzanMap2 : KurzanBase
    {
        public KurzanMap2(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ApplicationResources rh, AppLogger logger) : base("KurzanMap2", settings, mouse, kb, rh, logger)
        {
            PreferredMovementArea = _r.KurzanMap2PreferredArea;
        }

        public override void StartMapMove(ChaosClass cc)
        {
            _logger.Log(LogDetailLevel.Debug, "Kurzan Map 2 Initial Moves");
            var startPoints = _r.KurzanMap2Start;
            _mouse.ClickPosition(startPoints[0], 2000, MouseButtons.Right);
            _mouse.ClickPosition(startPoints[0], 2000, MouseButtons.Right);
            _mouse.ClickPosition(startPoints[0], 2000, MouseButtons.Right);
            _mouse.ClickPosition(startPoints[1], 1200, MouseButtons.Right);
        }
    }
}
