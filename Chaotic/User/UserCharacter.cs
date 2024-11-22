using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chaotic.User
{
    public static class ClassNames
    {
        public const string Unset = "Unset";
        public const string Berserker = "Berserker";
        public const string Paladin = "Paladin";
        public const string Sharpshooter = "Sharpshooter";
        public const string Aeromancer = "Aeromancer";
        public const string Artist = "Artist";
        public const string Souleater = "Souleater";
        public const string Scrapper = "Scrapper";
        public const string Arcanist = "Arcanist";
        public const string Breaker = "Breaker";
        public const string Destroyer = "Destroyer";
        public const string Glaivier = "Glaivier";
        public const string Machinist = "Machinist";
        public const string Deathblade = "Deathblade";
        public const string Gunlancer = "Gunlancer";
        public const string Slayer = "Slayer";
        public const string Gunslinger = "Gunslinger";
        public const string Sorceress = "Sorceress";
        public const string Artillerist = "Artillerist";
        public const string Deadeye = "Deadeye";
        public const string Wardancer = "Wardancer";
        public const string Reaper = "Reaper";
        public const string Summoner = "Summoner";
    }

    public class UserCharacter
    {
        public UserCharacter()
        {
            Identifier = Guid.NewGuid(); 

            FirstUnaTask = new UserUnaTask() { UnaName = "Not Set", BifrostPosition = 1 };
            SecondUnaTask = new UserUnaTask() { UnaName = "Not Set", BifrostPosition = 1 };
            ThirdUnaTask = new UserUnaTask() { UnaName = "Not Set", BifrostPosition = 1 };

            Skills = new UserCharacterSkills();
        }

        [JsonIgnore]
        public static List<string> AllClasses = new List<string>()
        {
            ClassNames.Unset,
            ClassNames.Berserker,
            ClassNames.Paladin,
            ClassNames.Sharpshooter,
            ClassNames.Aeromancer,
            ClassNames.Artist,
            ClassNames.Souleater,
            ClassNames.Scrapper,
            ClassNames.Arcanist,
            ClassNames.Breaker,
            ClassNames.Destroyer,
            ClassNames.Glaivier,
            ClassNames.Machinist,
            ClassNames.Deathblade,
            ClassNames.Gunlancer,
            ClassNames.Slayer,
            ClassNames.Gunslinger,
            ClassNames.Sorceress,
            ClassNames.Artillerist,
            ClassNames.Deadeye,
            ClassNames.Wardancer,
            ClassNames.Reaper,
            ClassNames.Summoner
        };

        [JsonIgnore]
        public bool IsCharSelected { get; set; } = true;

        public Guid Identifier { get; set; }

        public bool IsMain { get; set; }

        public string ClassName { get; set; }
        public int CharacterIndex { get; set; }
        public int ChaosLevel { get; set; }

        public bool RunChaos { get; set; }
        public bool DisableArkPassive { get; set; }
        public bool RunLopangUnas { get; set; }
        public bool RunLopangShushireOnly { get; set; }

        public bool RunUnas { get; set; }
        public bool GuildDonation { get; set; }
        public bool GuildDonationSilver { get; set; }
        public bool GuildDonationGold { get; set; }

        public UserUnaTask FirstUnaTask { get; set; }
        public UserUnaTask SecondUnaTask { get; set; }
        public UserUnaTask ThirdUnaTask { get; set; }

        public UserCharacterSkills Skills { get; set; }

        public bool HasHyperSkill
        {
            get
            {
                return ChaosLevel >= 1640;
            }
        }

    }
}
