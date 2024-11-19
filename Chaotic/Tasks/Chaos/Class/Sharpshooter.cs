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
    public class Sharpshooter : ChaosClass
    {
        public Sharpshooter(UserSettings settings, UserCharacter character, ResourceHelper r, KeyboardUtility kb, MouseUtility mouse, AppLogger logger) : base(settings, character, r, kb, mouse, logger)
        {

        }

        public override void StartUp()
        {
        }

        public override void UseCharacterSpecificAbilities()
        {

            var special = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("class/sharpshooter.png", _settings.Resolution), CharacterIconRegion, .8);
            if (special.Found)
                _kb.Press(Key.Z, 200);


            var special_bird = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("class/sharpshooter_bird.png", _settings.Resolution), CharacterIconRegion, .8);
            if (special_bird.Found)
                _kb.Press(Key.Z, 200);
        }
    }
}
