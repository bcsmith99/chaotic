﻿using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Chaotic.Resources;

namespace Chaotic.Tasks.Chaos.Class
{
    public class Destroyer : ChaosClass
    {
        public Destroyer(UserSettings settings, UserCharacter character, ApplicationResources r, KeyboardUtility kb, MouseUtility mouse, AppLogger logger) : base(settings, character, r, kb, mouse, logger)
        {
            SkillConfidence = .85;
        }
    }
}
