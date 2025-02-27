using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Resources
{
    public class AppResources1920x1080 : ApplicationResources
    {

        protected string _fileName = "resources_1920x1080.json";
        public AppResources1920x1080()
        {
            FileName = _fileName;

            AdventureMenu = new Point(1750, 920);
            Bifrost1 = new Point(1350, 430);
            Bifrost2 = new Point(1350, 490);
            Bifrost3 = new Point(1350, 550);
            Bifrost4 = new Point(1350, 650);
            Bifrost5 = new Point(1350, 710);
            Bifrost6 = new Point(1350, 765);
            BifrostMenu = new Point(1700, 730);
            BifrostOk = new Rect(880, 600, 100, 50);
            BossMobHealth = new Rect(660, 150, 100, 100);
            BottomRightExit = new Rect(1770, 880, 110, 50);
            CenterScreen = new Point(960, 520);
            ChaosDungeon_1415 = new Point(630,405);
            ChaosDungeon_1445 = new Point(630,460);
            ChaosDungeon_1475 = new Point(630,510);
            ChaosDungeon_1490 = new Point(630,560);
            ChaosDungeon_1520 = new Point(630,620);
            ChaosDungeon_1540 = new Point(630,670);
            ChaosDungeon_1560 = new Point(630,720);
            ChaosDungeon_1580 = new Point(630,405);
            ChaosDungeon_1600 = new Point(630,460);
            ChaosDungeon_1610 = new Point(630,405);

            ChaosDungeon_Elgacia = new Point(1300,315);
            ChaosDungeon_RightArrow = new Point(1575,315);
            ChaosDungeon_Shortcut = new Point(885,350);
            ChaosDungeon_Vern = new Point(1160,315);
            ChaosDungeon_Voldis = new Point(1450,315);

            ChaosLeave = new Rect(70, 300, 130, 75);
            ChaosOk = new Rect(890, 840, 150, 40);
            CharacterIcon = new Rect(895, 820, 125, 125);
            CharacterMenu = new Point(1700, 920);
            CharacterProfileMenu = new Point(1650, 850);
            CharSelectCol0 = 765;
            CharSelectCol1 = 970;
            CharSelectCol2 = 1160;
            CharSelectRow0 = 440;
            CharSelectRow1 = 530;
            CharSelectRow2 = 620;

            Chevron = new Rect(1880, 140, 25, 25);
            ClaimAll = new Rect(330,770,160,40);
            ClickableOffset = new Point(200, 150);
            ClickableRegion = new Rect(220, 225, 1300, 550);
            ClickableArea = new Point(500, 280);
            CommunityMenu = new Point(1845, 920);
            Complete = new Rect(215, 680, 110, 30);
            ConnectButton = new Point(1030, 700);
            CubeNextFloor = new Point();
            CubeMiddleFloor = new Point();
            CubeInitialMove = new Point();
            ElgaciaShardClickpoints = new List<Point>()
            {
                new Point(1100,785),
                new Point(800,200),
                new Point(900,360)
            };

            GameMenu = new Point(1850, 850);
            GoldDonate = new Rect(710, 525, 100, 50);
            GuideMenu = new Point(1800, 920);
            GuidePetMenu = new Point(1750, 755);
            GuildMenu = new Point(1800, 850);
            FriendsMenu = new Point(1800, 875);
            FriendsStatusChevron = new Point(1870, 387);
            FriendsOfflineOption = new Point(1820, 452);
            GuildNormalSupport = new Point(850, 520);
            GuildSupport = new Rect(1160, 450, 120, 40);
            GuildSupportCancel = new Rect(960, 690, 100, 40);
            GuildSupportOkButton = new Point(920, 705);

            GuildBloodstoneShopMenu = new Point(1275, 270);
            GuildBloodstoneTicketMenu = new Point(1100, 305);
            GuildBloodstoneDropdown = new Point(775, 345);
            GuildBloodstoneViewAll = new Point(775, 375);
            GuildExchangeX = 1465;

            HealthBar = new Rect(705, 855, 180, 15);
            Inventory = new Rect(1090, 300, 410, 460);
            Kurzan_1640 = new Point(830, 675);
            Kurzan_1660 = new Point(965, 595);
            Kurzan_1680 = new Point(1140, 450);
            KurzanMap1PreferredArea = new Rect(900, 225, 600, 400);
            KurzanMap1Start = new List<Point>
            {
                new Point(400,333),
                new Point(1030,200)
            };
            KurzanMap2PreferredArea = new Rect(225,400,500,300);
            KurzanMap2Start = new List<Point>
            {
                new Point(800,185),
                new Point(550,275)
            };
            KurzanMap3PreferredArea = new Rect(225,230,800,350);
            KurzanMap3Start = new List<Point>
            {
                new Point(600,215)
            };
            KurzanMap3StickingPoint = new Rect(920,130,500,170);


            MinimapCenter = new Point(1788, 260);
            Minimap = new Rect(1675, 165, 225, 195);//1900,360
            OngoingQuests = new Rect(1670, 420, 110, 25);
            PlecciaShardClickpoints = new List<Point>()
            {
                new Point(1250,390)
            };
            ScreenX = 1920;
            ScreenY = 1080;
            ServicesMenu = new Point(1895, 920);
            SilverDonate = new Rect(705, 525, 120, 40);
            Skill_A = new Rect(774, 910, 22, 22);
            Skill_D = new Rect(844, 910, 22, 22);
            Skill_E = new Rect(827, 874, 22, 22);
            Skill_F = new Rect(879, 910, 22, 22);
            Skill_HyperV = new Rect(660, 888, 30, 30);
            Skill_Q = new Rect(755, 874, 22, 22);
            Skill_R = new Rect(862, 874, 22, 22);
            Skill_S = new Rect(809, 910, 22, 22);
            Skill_T = new Rect(716, 893, 25, 25);
            Skill_V = new Rect(705, 888, 30, 30);
            Skill_W = new Rect(792, 874, 22, 22);

            SouthKurzanLeapClickpoints = new List<Point>()
            {
                new Point(900,190),
                new Point(350,400)
            };

            SwitchCharacterButton = new Point(530, 700);
            Timeout = new Rect(200, 150, 220, 75);
            UnaDaily = new Point(555, 255);
            UnaDailyDropdown = new Point(555, 320);
            UnaDailyDropdownFavorite = new Point(555, 406);
            UnaDailyRegion = new Rect(1165, 375, 100, 350);
            UnaLopangRoute = new List<Point>()
            {
                new Point(),
                new Point(),
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

            UnaTask = new Point(1700, 850);
            UnaWeekly = new Point(670, 255);
            UnaWeeklyDropdown = new Point(600, 285);
            UnaWeeklyDropdownFavorite = new Point(600, 332);
            UnaWeeklyRegion = new Rect(1165, 350, 100, 350);
            VoldisLeapClose = new Rect(800, 200, 500, 250);
            VoldisLeapNpc = new Rect(1350,350,275,300);
            SoloModeExchangeX = 990;
            SoloModeMax = new Point(860, 525);
            SoloModeShareRewards = new Point(760, 270);
            VoldisLeapX = 450;
            YDistance = 200;

            CheckFileExists();
        }
    }
}

//voldis leap npc region

