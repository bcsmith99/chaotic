using Chaotic.User;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Resources
{
    public abstract class ApplicationResources : IApplicationResources
    {
        protected string FileName { get; set; } = "";

        public Rect BifrostOk { get; set; }
        public Rect BossMobHealth { get; set; }
        public Rect BottomRightExit { get; set; }
        public Rect ChaosLeave { get; set; }
        public Rect ChaosOk { get; set; }
        public Rect CharacterIcon { get; set; }
        public Rect Chevron { get; set; }
        public Rect ClaimAll { get; set; }
        public Rect ClickableRegion { get; set; }
        public Rect Complete { get; set; }
        public Rect GoldDonate { get; set; }
        public Rect GuildSupport { get; set; }
        public Rect GuildSupportCancel { get; set; }
        public Rect HealthBar { get; set; }
        public Rect Inventory { get; set; }
        public Rect KurzanMap1PreferredArea { get; set; }
        public Rect KurzanMap2PreferredArea { get; set; }
        public Rect KurzanMap3PreferredArea { get; set; }
        public Rect KurzanMap3StickingPoint { get; set; }
        public Rect Minimap { get; set; }
        public Rect OngoingQuests { get; set; }
        public Rect SilverDonate { get; set; }
        public Rect Skill_A { get; set; }
        public Rect Skill_D { get; set; }
        public Rect Skill_E { get; set; }
        public Rect Skill_F { get; set; }
        public Rect Skill_HyperV { get; set; }
        public Rect Skill_Q { get; set; }
        public Rect Skill_R { get; set; }
        public Rect Skill_S { get; set; }
        public Rect Skill_T { get; set; }
        public Rect Skill_V { get; set; }
        public Rect Skill_W { get; set; }
        public Rect Timeout { get; set; }
        public Rect UnaDailyRegion { get; set; }
        public Rect UnaWeeklyRegion { get; set; }
        public Rect VoldisLeapClose { get; set; }
        public Rect VoldisLeapNpc { get; set; }
        public Point AdventureMenu { get; set; }
        public Point Bifrost1 { get; set; }
        public Point Bifrost2 { get; set; }
        public Point Bifrost3 { get; set; }
        public Point Bifrost4 { get; set; }
        public Point Bifrost5 { get; set; }
        public Point Bifrost6 { get; set; }
        public Point BifrostMenu { get; set; }
        public Point CenterScreen { get; set; }
        public Point ChaosDungeon_1415 { get; set; }
        public Point ChaosDungeon_1445 { get; set; }
        public Point ChaosDungeon_1475 { get; set; }
        public Point ChaosDungeon_1490 { get; set; }
        public Point ChaosDungeon_1520 { get; set; }
        public Point ChaosDungeon_1540 { get; set; }
        public Point ChaosDungeon_1560 { get; set; }
        public Point ChaosDungeon_1580 { get; set; }
        public Point ChaosDungeon_1600 { get; set; }
        public Point ChaosDungeon_1610 { get; set; }
        public Point ChaosDungeon_Elgacia { get; set; }
        public Point ChaosDungeon_RightArrow { get; set; }
        public Point ChaosDungeon_Shortcut { get; set; }
        public Point ChaosDungeon_Vern { get; set; }
        public Point ChaosDungeon_Voldis { get; set; }
        public Point CharacterMenu { get; set; }
        public Point CharacterProfileMenu { get; set; }
        public Point ClickableArea { get; set; }
        public Point CommunityMenu { get; set; }
        public Point ConnectButton { get; set; }
        public Point GameMenu { get; set; }
        public Point GuidePetMenu { get; set; }
        public Point GuildMenu { get; set; }
        public Point GuildNormalSupport { get; set; }
        public Point GuildSupportOkButton { get; set; }
        public Point Kurzan_1640 { get; set; }
        public Point Kurzan_1660 { get; set; }
        public Point MinimapCenter { get; set; }
        public Point ServicesMenu { get; set; }
        public Point SwitchCharacterButton { get; set; }
        public Point UnaDaily { get; set; }
        public Point UnaDailyDropdown { get; set; }
        public Point UnaDailyDropdownFavorite { get; set; }
        public Point UnaTask { get; set; }
        public Point UnaWeekly { get; set; }
        public Point UnaWeeklyDropdown { get; set; }
        public Point UnaWeeklyDropdownFavorite { get; set; }

        
        public Point CubeNextFloor { get; set; }
        public Point CubeMiddleFloor { get; set; }
        public Point CubeInitialMove { get; set; }

        public List<Point> ElgaciaShardClickpoints { get; set; }
        public List<Point> KurzanMap1Start { get; set; }
        public List<Point> KurzanMap2Start { get; set; }
        public List<Point> KurzanMap3Start { get; set; }
        public List<Point> PlecciaShardClickpoints { get; set; }
        public List<Point> SouthKurzanLeapClickpoints { get; set; }
        public List<Point> UnaLopangRoute { get; set; }
        public int CharSelectCol0 { get; set; }
        public int CharSelectCol1 { get; set; }
        public int CharSelectCol2 { get; set; }
        public int CharSelectRow0 { get; set; }
        public int CharSelectRow1 { get; set; }
        public int CharSelectRow2 { get; set; }
        public Point ClickableOffset { get; set; }
        public int ScreenX { get; set; }
        public int ScreenY { get; set; }
        public int VoldisLeapX { get; set; }
        public int YDistance { get; set; }
        public Point GuideMenu { get; set; }

        protected void Save()
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (JsonTextWriter jw = new(sw))
                {
                    JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                    serializer.Serialize(jw, this);
                }
            }

            File.WriteAllText(GetFullPath(), sb.ToString());
        }

        protected string GetFullPath()
        {
            return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Chaotic\\{FileName}";
        }

        protected void CheckFileExists()
        {
            var path = GetFullPath();

            if (!File.Exists(path) || ConfigurationManager.AppSettings["ForceUpgradeResources"] == "true")
            {
                Save();

                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["ForceUpgradeResources"].Value = "false";
                config.Save();
            }
        }

        public ApplicationResources Read()
        {
            try
            {
                using (StreamReader sr = new StreamReader(GetFullPath()))
                {
                    using (var tr = new JsonTextReader(sr))
                    {
                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<ApplicationResources>(tr);
                    }
                }
            }
            catch (FileNotFoundException fnfe)
            {
                this.Save();
                return this.Read();
            }
        }

        public static ApplicationResources Create(string resolution)
        {
            switch (resolution)
            {
                case "3440x1440":
                    return new AppResources3440x1440();
                case "2560x1440":
                    return new AppResources2560x1440();
                default:
                    throw new NotImplementedException("The provided resolution is not supported");
            }
        }
    }
}
