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
using Chaotic.Resources;

namespace Chaotic.Tasks.Chaos.Class
{
    public class Soulfist : ChaosClass
    {
        public Soulfist(UserSettings settings, UserCharacter character, ApplicationResources r, KeyboardUtility kb, MouseUtility mouse, AppLogger logger) : base(settings, character, r, kb, mouse, logger)
        {

        }

        public override void StartUp()
        {
        }

        public override void UseCharacterSpecificAbilities()
        {

            var special = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("class/soulfist.png", _settings.Resolution), CharacterIconRegion, .65);
            if (!special.Found)
                _kb.Press(Key.Z, 200);
        }
    }
}
