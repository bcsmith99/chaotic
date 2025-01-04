using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        public const string Soulfist = "Soulfist";
        public const string Shadowhunter = "Shadowhunter";
        public const string Striker = "Striker";
        public const string Bard = "Bard"; 
    }

    public class UserCharacter : INotifyPropertyChanged
    {
        public UserCharacter()
        {
            Identifier = Guid.NewGuid();

            FirstUnaTask = new UserUnaTask() { UnaName = "Not Set", BifrostPosition = 1 };
            SecondUnaTask = new UserUnaTask() { UnaName = "Not Set", BifrostPosition = 1 };
            ThirdUnaTask = new UserUnaTask() { UnaName = "Not Set", BifrostPosition = 1 };

            AllResolutionSkills.Add("3440x1440", new UserCharacterSkills());
            AllResolutionSkills.Add("2560x1440", new UserCharacterSkills());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
            ClassNames.Summoner,
            ClassNames.Soulfist,
            ClassNames.Shadowhunter, 
            ClassNames.Bard,
            ClassNames.Shadowhunter
        }.OrderBy(x => x).ToList();


        private bool _isCharSelected = false;
        public bool IsCharSelected
        {
            get { return _isCharSelected; }
            set
            {
                _isCharSelected = value;
                OnPropertyChanged();
            }
        }

        public Guid Identifier { get; set; }

        public bool IsMain { get; set; }

        public string ClassName { get; set; } = "";
        public int CharacterIndex { get; set; }
        public int ChaosLevel { get; set; }

        public bool RunChaos { get; set; }
        public bool DisableArkPassive { get; set; }
        public bool RunLopangUnas { get; set; }
        public bool RunLopangShushireOnly { get; set; }

        public bool RunUnas { get; set; }
        private bool _guildDonation;
        public bool GuildDonation
        {
            get { return _guildDonation; }
            set
            {
                _guildDonation = value;
                OnPropertyChanged();
            }
        }

        private bool _BuySoloMode;
        public bool BuySoloMode
        {
            get { return _BuySoloMode; }
            set
            {
                _BuySoloMode = value;
                OnPropertyChanged();
            }
        }

        private int _SoloModeBifrost = 0;
        public int SoloModeBifrost
        {
            get { return _SoloModeBifrost; }
            set
            {
                _SoloModeBifrost = value;
                OnPropertyChanged();
            }
        }

        public bool BuySoloHonorShards { get; set; }
        public bool BuySoloMarvelousLeaps { get; set; }
        public bool BuySoloRadiantLeaps { get; set; }
        public bool BuySoloRefinedObliteration { get; set; }
        public bool BuySoloRefinedProtection { get; set; }

        public bool BuySoloOreha { get; set; }
        public bool BuySoloSuperiorOreha { get; set; }
        public bool BuySoloPrimeOreha { get; set; }


        public bool GuildDonationSilver { get; set; }
        public bool GuildDonationGold { get; set; }

        public UserUnaTask FirstUnaTask { get; set; }
        public UserUnaTask SecondUnaTask { get; set; }
        public UserUnaTask ThirdUnaTask { get; set; }

        public Dictionary<string, UserCharacterSkills> AllResolutionSkills { get; set; } = new Dictionary<string, UserCharacterSkills>();

        [JsonIgnore]
        public bool HasHyperSkill
        {
            get
            {
                return ChaosLevel >= 1640;
            }
        }

    }
}
