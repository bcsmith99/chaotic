using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;
using Chaotic.Resources;

namespace Chaotic.Tasks.Chaos.Class
{
    public class Summoner : ChaosClass
    {
        public Summoner(UserSettings settings, UserCharacter character, ApplicationResources r, KeyboardUtility kb, MouseUtility mouse, AppLogger logger) : base(settings, character, r, kb, mouse, logger)
        {

        }

        private bool PhoenixSet { get; set; } = false;

        public override void StartUp()
        {
            int maxTries = 20;
            int current = 0;
            while (current < maxTries)
            {
                var phoenix = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("class/summoner_phoenix.png", _settings.Resolution), CharacterIconRegion, .95);
                var phoenix_ready = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("class/summoner_ready.png", _settings.Resolution), CharacterIconRegion, .95);
                if (phoenix.Found || phoenix_ready.Found)
                    break;

                _kb.Press(Key.X, 300);
                current++;
            }
        }

        public override void UseCharacterSpecificAbilities()
        {
            var special = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("class/summoner_ready.png", _settings.Resolution), CharacterIconRegion, .8);
            if (special.Found)
                _kb.Press(Key.Z, 200);
        }
    }
}
