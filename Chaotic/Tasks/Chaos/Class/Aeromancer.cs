﻿using Chaotic.Resources;
using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic.Tasks.Chaos.Class
{
    public class Aeromancer : ChaosClass
    {
        public Aeromancer(UserSettings settings, UserCharacter character, ApplicationResources r, KeyboardUtility kb, MouseUtility mouse, AppLogger logger) : base(settings, character, r, kb, mouse, logger)
        {

        }

        public override void StartUp()
        {
        }

        public override void UseCharacterSpecificAbilities()
        {
            var special = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("class/aeromancer.png", _settings.Resolution), CharacterIconRegion, .85);
            if (special.Found)
                _kb.Press(Key.Z, 200);
        }
    }
}
