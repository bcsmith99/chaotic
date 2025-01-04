using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Resources
{
    public class AppResources2560x1440 : ApplicationResources
    {

        protected string _fileName = "resources_2560x1440.json";
        public AppResources2560x1440()
        {
            FileName = _fileName;

            AdventureMenu = new Point(2330, 1225);
            Bifrost1 = new Point(1800, 570);
            Bifrost2 = new Point(1800, 650);
            Bifrost3 = new Point(1800, 730);
            Bifrost4 = new Point(1800, 860);
            Bifrost5 = new Point(1800, 940);
            Bifrost6 = new Point(1800, 1025);
            BifrostMenu = new Point(2275, 975);
            BifrostOk = new Rect(1175, 800, 100, 50);
            BossMobHealth = new Rect(400, 200, 1000, 400);
            BottomRightExit = new Rect(2350, 1175, 150, 75);
            CenterScreen = new Point(1280, 710);
            ChaosDungeon_1415 = new Point(850, 540);
            ChaosDungeon_1445 = new Point(850, 610);
            ChaosDungeon_1475 = new Point(850, 680);
            ChaosDungeon_1490 = new Point(850, 750);
            ChaosDungeon_1520 = new Point(850, 820);
            ChaosDungeon_1540 = new Point(850, 890);
            ChaosDungeon_1560 = new Point(850, 960);
            ChaosDungeon_1580 = new Point(850, 540);
            ChaosDungeon_1600 = new Point(850, 610);
            ChaosDungeon_1610 = new Point(850, 540);

            ChaosDungeon_Elgacia = new Point(1725, 420);
            ChaosDungeon_RightArrow = new Point(2100, 420);
            ChaosDungeon_Shortcut = new Point(1175, 465);
            ChaosDungeon_Vern = new Point(1540, 420);
            ChaosDungeon_Voldis = new Point(1930, 420);

            ChaosLeave = new Rect(0, 450, 250, 50);
            ChaosOk = new Rect(1200, 1100, 150, 75);
            CharacterIcon = new Rect(1200, 1115, 150, 130);
            CharacterMenu = new Point(2265, 1220);
            CharacterProfileMenu = new Point(2200, 1130);
            CharSelectCol0 = 1020;
            CharSelectCol1 = 1285;
            CharSelectCol2 = 1550;
            CharSelectRow0 = 600;
            CharSelectRow1 = 710;
            CharSelectRow2 = 825;

            Chevron = new Rect(2505, 185, 30, 30);
            ClaimAll = new Rect(450, 1025, 200, 50);
            ClickableOffset = new Point(200, 150);
            ClickableRegion = new Rect(450, 325, 1625, 700);
            ClickableArea = new Point(800, 400);
            ClickableArea = new Point(800, 400);
            CommunityMenu = new Point(2460, 1225);
            Complete = new Rect(300, 900, 200, 75);
            ConnectButton = new Point(1375, 930);
            CubeNextFloor = new Point(1260, 825);
            CubeMiddleFloor = new Point(1650, 1025);
            CubeInitialMove = new Point(1830, 325);
            ElgaciaShardClickpoints = new List<Point>()
            {
                new Point(1500,1100),
                new Point(1050,250),
                new Point(1225,500)
            };

            GameMenu = new Point(2460, 1135);
            GoldDonate = new Rect(1215, 700, 125, 50);
            GuideMenu = new Point(2400, 1225);
            GuidePetMenu = new Point(2340, 1005);
            GuildMenu = new Point(2400, 1135);
            FriendsMenu = new Point(2400, 1165);
            FriendsStatusChevron = new Point(2490, 515);
            FriendsOfflineOption = new Point(2425, 600);
            GuildNormalSupport = new Point(1130, 700);
            GuildSupport = new Rect(1550, 600, 150, 50);
            GuildSupportCancel = new Rect(1280, 920, 100, 50);
            GuildSupportOkButton = new Point(1225, 940);
            HealthBar = new Rect(935, 1140, 250, 15);
            Inventory = new Rect(1450, 400, 530, 600);
            Kurzan_1640 = new Point(1105, 930);
            Kurzan_1660 = new Point(1285, 820);
            KurzanMap1PreferredArea = new Rect(1050, 250, 1000, 550);
            KurzanMap1Start = new List<Point>
            {
                new Point(550,350),
                new Point(1400,500)
            };
            KurzanMap2PreferredArea = new Rect(300, 625, 500, 275);
            KurzanMap2Start = new List<Point>
            {
                new Point(1000,250),
                new Point(600,350)
            };
            KurzanMap3PreferredArea = new Rect(300, 400, 700, 450);
            KurzanMap3Start = new List<Point>
            {
                new Point(800,250)
            };
            KurzanMap3StickingPoint = new Rect(1280, 180, 600, 300);


            MinimapCenter = new Point(2385, 348);
            Minimap = new Rect(2230, 220, 300, 250);
            OngoingQuests = new Rect(2210, 550, 150, 40);
            PlecciaShardClickpoints = new List<Point>()
            {
                new Point(1715,520)
            };
            ScreenX = 2560;
            ScreenY = 1440;
            ServicesMenu = new Point(2525, 1220);
            SilverDonate = new Rect(950, 700, 150, 50);
            Skill_A = new Rect(1025, 1210, 40, 40);
            Skill_D = new Rect(1120, 1210, 40, 40);
            Skill_E = new Rect(1100, 1162, 40, 40);
            Skill_F = new Rect(1168, 1210, 40, 40);
            Skill_HyperV = new Rect(880, 1185, 40, 40);
            Skill_Q = new Rect(1005, 1162, 40, 40);
            Skill_R = new Rect(1145, 1162, 40, 40);
            Skill_S = new Rect(1075, 1210, 40, 40);
            Skill_T = new Rect(950, 1185, 40, 40);
            Skill_V = new Rect(940, 1185, 40, 40);
            Skill_W = new Rect(1050, 1162, 40, 40);

            SouthKurzanLeapClickpoints = new List<Point>()
            {
                new Point(1200,220),
                new Point(375,600)
            };

            SwitchCharacterButton = new Point(710, 930);
            Timeout = new Rect(250, 200, 750, 150);
            UnaDaily = new Point(740, 335);
            UnaDailyDropdown = new Point(750, 420);
            UnaDailyDropdownFavorite = new Point(700, 540);
            UnaDailyRegion = new Rect(1550, 500, 100, 450);
            UnaLopangRoute = new List<Point>()
            {
                new Point(400,640),
                new Point(540,1000),
                new Point(850,245),
                new Point(1570,225),
                new Point(1860,350),
                new Point(1890,430),
                new Point(470,1030),
                new Point(500,1100),
                new Point(575,1090),
                new Point(580,285),
                new Point(750,250)
            };

            PraeteriaSolarClickpoints = new List<Point>()
            {
                new Point(),
                new Point(),
                new Point(),
                new Point(),
                new Point(),
                new Point()
            };

            PlecciaSolarClickpoints = new List<Point>()
            {
                new Point(),
                new Point(),
                new Point(),
                new Point(),
                new Point(),
                new Point(),
                new Point(),
                new Point(),
                new Point()
            };

            UnaTask = new Point(2250, 1135);
            UnaWeekly = new Point(900, 335);
            UnaWeeklyDropdown = new Point(800, 380);
            UnaWeeklyDropdownFavorite = new Point(800, 440);
            UnaWeeklyRegion = new Rect(1550, 470, 100, 550);
            VoldisLeapClose = new Rect(1250, 220, 700, 350);
            VoldisLeapNpc = new Rect(1830, 450, 450, 200);
            SoloModeExchangeX = 1320;
            SoloModeMax = new Point(1150, 700);
            VoldisLeapX = 600;
            YDistance = 300;

            CheckFileExists();
        }
    }
}
