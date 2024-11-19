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
    public class Berserker : ChaosClass
    {
        public Berserker(UserSettings settings, UserCharacter character, ResourceHelper r, KeyboardUtility kb, MouseUtility mouse, AppLogger logger) : base(settings, character, r, kb, mouse, logger)
        {

        }

        public override void StartUp()
        {
            var mayhem = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("class/berserker.png", _settings.Resolution), CharacterIconRegion, .7);
            if (mayhem.Found)
                _kb.Press(Key.Z, 200);
        }
    }
}
