using Chaotic.Resources;
using Chaotic.User;
using Chaotic.Utilities;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic.Tasks
{
    public class UITasks
    {
        private UserSettings _settings;
        private readonly MouseUtility _mouse;
        private readonly KeyboardUtility _kb;
        private readonly ApplicationResources _r;
        private readonly AppLogger _logger;

        public UITasks(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, AppLogger logger)
        {
            _settings = settings;
            _mouse = mouse;
            _kb = kb;
            _r = r;
            _logger = logger;
        }

        public bool CrashCheck()
        {
            return false;
        }

        public bool OfflineCheck()
        {
            return false;
        }

        public bool InAreaCheck(int maxTries = 30)
        {
            double maxConfidence = 0;
            while (maxTries > 0)
            {
                BackgroundProcessing.ProgressCheck();
                //TODO: Parameterize Coordinates for Resolution
                //,
                var inTownButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("chevron.png", _settings.Resolution), _r.Chevron, confidence: .65, useGrayscale: true);
                maxConfidence = Math.Max(maxConfidence, inTownButton.MaxConfidence);
                if (inTownButton.Found)
                {
                    _logger.Log(LogDetailLevel.Debug, $"In Area Found - {inTownButton.MaxConfidence}");
                    return true;
                }

                maxTries--;
                Sleep.SleepMs(900, 1100);
            }

            _logger.Log(LogDetailLevel.Debug, $"In Town Check Failed - Max Conf: {maxConfidence}");

            return false;
        }

        public void GoOffline()
        {
            Sleep.SleepMs(500, 600);
            BackgroundProcessing.ProgressCheck();

            _logger.Log(LogDetailLevel.Debug, "Going to Offline Mode");
            _mouse.ClickPosition(_r.CommunityMenu, 500);
            _mouse.ClickPosition(_r.FriendsMenu, 1000);
            _mouse.ClickPosition(_r.FriendsStatusChevron, 500);
            _mouse.ClickPosition(_r.FriendsOfflineOption, 500);
            _kb.Press(Key.Escape, 1000);
        }

        public void ToggleArkPassive(bool disable)
        {
            BackgroundProcessing.ProgressCheck();
            Sleep.SleepMs(500, 500, _settings.PerformanceMultiplier);
            _mouse.ClickCenterScreen(_r.CenterScreen);

            _mouse.ClickPosition(_r.CharacterMenu, 500);
            _mouse.ClickPosition(_r.CharacterProfileMenu, 1000);

            var arkPassive = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("arkpassive_button.png", _settings.Resolution), confidence: .9);

            if (arkPassive.Found)
            {
                _mouse.ClickPosition(arkPassive.CenterX, arkPassive.CenterY, 1000);

                if (disable)
                {
                    var disableButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("arkpassive_disable.png", _settings.Resolution), confidence: .9);
                    if (disableButton.Found)
                    {
                        _mouse.ClickPosition(disableButton.CenterX, disableButton.CenterY, 500);
                        var okButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), confidence: .9);
                        while (okButton.Found)
                        {
                            _mouse.ClickPosition(okButton.CenterX, okButton.CenterY, 500);
                            okButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), confidence: .9);
                        }
                    }
                    else
                        _kb.Press(Key.Escape, 500);
                }
                else
                {
                    var enableButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("arkpassive_enable.png", _settings.Resolution), confidence: .9);
                    if (enableButton.Found)
                    {
                        _mouse.ClickPosition(enableButton.CenterX, enableButton.CenterY, 500);
                        var okButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), confidence: .9);
                        while (okButton.Found)
                        {
                            _mouse.ClickPosition(okButton.CenterX, okButton.CenterY, 500);
                            okButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), confidence: .9);
                        }
                    }
                    else
                        _kb.Press(Key.Escape, 500);
                }

                _kb.Press(Key.Escape, 500);
            }

        }

        public void ClearOngoingQuests()
        {
            Sleep.SleepMs(300, 400, _settings.PerformanceMultiplier);
            BackgroundProcessing.ProgressCheck();
            var ongoingButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("ongoing_quests.png", _settings.Resolution), _r.OngoingQuests, .85, true);
            if (ongoingButton.Found)
            {
                Sleep.SleepMs(100, 200, _settings.PerformanceMultiplier);
                _mouse.ClickPosition(ongoingButton.CenterX, ongoingButton.CenterY, 1000);
                var completeButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("complete_button.png", _settings.Resolution), confidence: .95);
                while (completeButton.Found)
                {
                    Sleep.SleepMs(100, 200, _settings.PerformanceMultiplier);
                    _mouse.ClickPosition(completeButton.CenterX, completeButton.CenterY, 1000);
                    completeButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("complete_button.png", _settings.Resolution), confidence: .95);
                }
            }
        }

        public bool MoveGems()
        {
            _logger.Log(LogDetailLevel.Debug, "Moving gems to storage");
            var inventoryRegion = _r.Inventory;
            var success = true;
            var gemDirectory = Utility.ResourceLocation(_settings.Resolution, "gems");

            var moveGemLevel = _settings.GemLevelThreshold;
            var currentLevel = 9;

            BackgroundProcessing.ProgressCheck();
            while (currentLevel >= moveGemLevel)
            {
                var filePrefixes = new List<string>() { "t3_dmg_", "t3_cd_", "t4_dmg_", "t4_cd_" };

                foreach (var prefix in filePrefixes)
                {
                    var fileName = $"{gemDirectory}{prefix}{currentLevel}.png";
                    if (File.Exists(fileName))
                    {
                        var loopCount = 0;
                        var gems = IP.LocateOnScreen(fileName, inventoryRegion, .9);
                        if (gems.Matches.Count > 0)
                        {
                            foreach (var match in gems.Matches)
                            {
                                _kb.Hold(Key.LeftAlt);
                                _mouse.ClickPosition(match.Center, 100, MouseButtons.Right);
                                _kb.Release(Key.LeftAlt);
                                Sleep.SleepMs(50, 150);
                            }
                        }

                    }
                }
                currentLevel--;
            }

            return success;
        }

        public bool MoveHoningMaterials()
        {
            _logger.Log(LogDetailLevel.Debug, "Moving Honing Materials to Storage");
            var inventoryRegion = _r.Inventory;
            var success = true;
            var honingMatsDirectory = Utility.ResourceLocation(_settings.Resolution, "mats");

            var materialFileNames = Directory.GetFiles(honingMatsDirectory).Select(x => Path.GetFileName(x));

            BackgroundProcessing.ProgressCheck();
            foreach (var materialFile in materialFileNames)
            {
                var loopCount = 0;

                var material = IP.LocateOnScreen($"{honingMatsDirectory}{materialFile}", inventoryRegion, .8);
                if (material.Matches.Count > 0)
                {
                    foreach (var match in material.Matches)
                    {
                        _kb.Hold(Key.LeftAlt);
                        _mouse.ClickPosition(match.Center, 100, MouseButtons.Right);
                        _kb.Release(Key.LeftAlt);
                        Sleep.SleepMs(50, 150);
                    }
                }
            }

            Sleep.SleepMs(200, 300, _settings.PerformanceMultiplier);

            return success;
        }

        public bool OpenInventoryManagement()
        {
            _logger.Log(LogDetailLevel.Debug, "Opening Inventory Management");
            BackgroundProcessing.ProgressCheck();
            bool success = true;
            OpenPetMenu();

            var remoteStorageButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("remotestorage_button.png", _settings.Resolution), confidence: .85);
            if (remoteStorageButton.Found)
                _mouse.ClickPosition(remoteStorageButton.CenterX, remoteStorageButton.CenterY, 1000);
            else
            {
                _logger.Log(LogDetailLevel.Debug, "Did not find remote storage button on inventory management, aborting.");
                success = false;
            }


            return success;
        }

        public void DeathCheck()
        {
            var dead = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("dead.png", _settings.Resolution), confidence: .75);

            if (dead.Found)
            {
                _logger.Log(LogDetailLevel.Debug, "You ded.");
                Sleep.SleepMs(5000, 7000);
                int i = 0;
                while (i < 10)
                {
                    var revive_button = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("revive.png", _settings.Resolution), confidence: .7);
                    if (revive_button.Found)
                    {
                        _mouse.ClickPosition(revive_button.Center, 1000);
                        return;
                    }
                    Sleep.SleepMs(500, 700);
                    i++;
                }
                _logger.Log(LogDetailLevel.Debug, "Unable to find revive button to press in death check.");
            }
            return;
        }

        public bool BifrostToPoint(int bifrost)
        {
            _mouse.ClickPosition(_r.AdventureMenu, 1000);
            _mouse.ClickPosition(_r.BifrostMenu, 1500);
            var bifrostPoint = (OpenCvSharp.Point)_r.GetType().GetProperty($"Bifrost{bifrost}").GetValue(_r);
            _mouse.ClickPosition(bifrostPoint, 1000);

            var bifrostOkRegion = _r.BifrostOk;
            var okButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), bifrostOkRegion, .9);
            if (okButton.Found)
            {
                _mouse.ClickPosition(okButton.CenterX, okButton.CenterY, 5000);
                return InAreaCheck();
            }

            return false;
        }

        public bool BuyGuildShop(UserCharacter character)
        {
            BackgroundProcessing.ProgressCheck();
            var success = true;

            Thread.Sleep(2000);
            _mouse.ClickPosition(_r.CommunityMenu, 500);
            _mouse.ClickPosition(_r.GuildMenu, 3200);
            _mouse.ClickPosition(_r.GuildBloodstoneShopMenu, 1000);
            _mouse.ClickPosition(_r.GuildBloodstoneTicketMenu, 1000);
            _mouse.ClickPosition(_r.GuildBloodstoneDropdown, 1000);
            _mouse.ClickPosition(_r.GuildBloodstoneViewAll, 1000);

            bool buyMilitia = character.BuyGuildMilitia;
            bool buyKnights = character.BuyGuildKnights;
            bool buyTarunian = character.BuyGuildTarunian;
            bool buyLazenith = character.BuyGuildLazenith;
            bool buySage = character.BuyGuildSage;
            bool buyAllied = character.BuyGuildAllied;
            bool buyAllied2 = character.BuyGuildAllied2;

            int currentScroll = 0;
            int maxScroll = 3;

            BackgroundProcessing.ProgressCheck();

            _mouse.ClickPosition(_r.CenterScreen, 1000);

            while ((buyMilitia || buyKnights || buyTarunian || buyLazenith || buySage || buyAllied) && currentScroll < maxScroll)
            {
                if (buyMilitia)
                    success = success && BuyGuildTickets("militia_chest");
                if (buyKnights)
                    success = success && BuyGuildTickets("knights_chest");
                if (buyTarunian)
                    success = success && BuyGuildTickets("tarunian_chest");
                if (buyLazenith)
                    success = success && BuyGuildTickets("lazenith_chest");
                if (buySage)
                    success = success && BuyGuildTickets("sage_chest");
                if (buyAllied)
                    success = success && BuyGuildTickets("allied_chest");
                if (buyAllied2)
                    success = success && BuyGuildTickets("allied2_chest");

                _mouse.Scroll(MouseUtility.ScrollDirection.Down, 7);
                currentScroll++;
            }

            _kb.Press(Key.Escape, 1500);

            return success;
        }

        public bool BuySoloModeShop(UserCharacter character)
        {
            BackgroundProcessing.ProgressCheck();
            bool success = true;

            if (BifrostToPoint(character.SoloModeBifrost))
            {
                bool buyOreha = character.BuySoloOreha;
                bool buySuperiorOreha = character.BuySoloSuperiorOreha;
                bool buyPrimeOreha = character.BuySoloPrimeOreha;
                bool buyMarvelousLeap = character.BuySoloMarvelousLeaps;
                bool buyRadiantLeap = character.BuySoloRadiantLeaps;
                bool buyHonorShards = character.BuySoloHonorShards;
                bool buyRefinedProtection = character.BuySoloRefinedProtection;
                bool buyRefinedObliteration = character.BuySoloRefinedObliteration;

                int maxScrollAttempts = 30;
                int currentScroll = 0;

                _mouse.ClickPosition(_r.CenterScreen);
                _kb.Press(Key.G, 1500);

                _mouse.Scroll(MouseUtility.ScrollDirection.Down, 30, 200);

                BackgroundProcessing.ProgressCheck();

                while ((buyOreha || buySuperiorOreha || buyPrimeOreha || buyMarvelousLeap || buyRadiantLeap || buyHonorShards || buyRefinedObliteration || buyRefinedProtection) && currentScroll < maxScrollAttempts)
                {
                    BackgroundProcessing.ProgressCheck();
                    if (buyOreha && BuySoloItem("oreha"))
                        buyOreha = false;
                    if (buySuperiorOreha && BuySoloItem("superior_oreha"))
                        buySuperiorOreha = false;
                    if (buyPrimeOreha && BuySoloItem("prime_oreha"))
                        buyPrimeOreha = false;
                    if (buyMarvelousLeap && BuySoloItem("marvelous_leap"))
                        buyMarvelousLeap = false;
                    if (buyRadiantLeap && BuySoloItem("radiant_leap"))
                        buyRadiantLeap = false;
                    if (buyHonorShards && BuySoloItem("honor_shard"))
                        buyHonorShards = false;
                    if (buyRefinedObliteration && BuySoloItem("refined_obliteration"))
                        buyRefinedObliteration = false;
                    if (buyRefinedProtection && BuySoloItem("refined_protection"))
                        buyRefinedProtection = false;

                    _mouse.Scroll(MouseUtility.ScrollDirection.Up, 1, 500);
                    currentScroll++;
                }
                Sleep.SleepMs(500, 700);
                success = success && BuySoloSharedRewards(character);

                _kb.Press(Key.Escape, 1500);
            }

            return success;
        }

        private bool BuySoloSharedRewards(UserCharacter character)
        {
            BackgroundProcessing.ProgressCheck();
            bool success = true;

            bool buyAkkanEyes = character.BuyAkkanEyes;
            bool buyEchidnaEyes = character.BuyEchidnaEyes;
            bool buyThaemineFire = character.BuyThaemineFire;

            int maxScrollAttempts = 10;
            int currentScroll = 0;

            if (buyAkkanEyes || buyEchidnaEyes || buyThaemineFire)
            {
                _mouse.ClickPosition(_r.SoloModeShareRewards, 1000);
                _mouse.SetPosition(_r.CenterScreen);
                _mouse.Scroll(MouseUtility.ScrollDirection.Down, 10, 200);

                BackgroundProcessing.ProgressCheck();

                while ((buyAkkanEyes || buyEchidnaEyes || buyThaemineFire) && currentScroll < maxScrollAttempts)
                {
                    BackgroundProcessing.ProgressCheck();
                    if (buyAkkanEyes && BuySoloItem("akkan_eye"))
                        buyAkkanEyes = false;
                    if (buyEchidnaEyes && BuySoloItem("echidna_eye"))
                        buyEchidnaEyes = false;
                    if (buyThaemineFire && BuySoloItem("thaemine_fire"))
                        buyThaemineFire = false;

                    _mouse.Scroll(MouseUtility.ScrollDirection.Up, 1, 500);
                    currentScroll++;
                }
            }

            return success;
        }

        private bool BuyGuildTickets(string itemImg)
        {
            var success = true;
            var results = IP.LocateOnScreen(Utility.ImageResourceLocation($"{itemImg}.png", _settings.Resolution, "guild"), confidence: .9);
            foreach (var match in results.Matches)
            {
                _mouse.ClickPosition(_r.GuildExchangeX, match.Center.Y, 1000);
                var okButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation($"ok_button.png", _settings.Resolution), confidence: .8, breakAfterFirst: true);
                if (okButton.Found)
                {
                    _logger.Log(LogDetailLevel.Debug, $"Item {itemImg} OK Button found.  Confidence: {okButton.MaxConfidence}");
                    _mouse.ClickPosition(okButton.Center, 1000);
                }
            }

            return success;
        }

        private bool BuySoloItem(string itemImg)
        {
            bool itemBought = false;

            var item = IP.LocateCenterOnScreen(Utility.ImageResourceLocation($"{itemImg}.png", _settings.Resolution, "soloshop"), confidence: .9, breakAfterFirst: true);

            if (item.Found)
            {
                _logger.Log(LogDetailLevel.Debug, $"Item {itemImg} found.  Confidence: {item.MaxConfidence}");
                _mouse.ClickPosition(_r.SoloModeExchangeX, item.CenterY, 1000);
                _mouse.ClickPosition(_r.SoloModeMax, 1000);

                var okButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation($"ok_button.png", _settings.Resolution), confidence: .8, breakAfterFirst: true);
                if (okButton.Found)
                {
                    _logger.Log(LogDetailLevel.Debug, $"Item {itemImg} OK Button found.  Confidence: {okButton.MaxConfidence}");
                    _mouse.ClickPosition(okButton.Center, 1000);
                }
                itemBought = true;
            }
            return itemBought;
        }


        public bool CloseInventoryManagement()
        {
            _logger.Log(LogDetailLevel.Debug, "Closing Inventory Management");
            BackgroundProcessing.ProgressCheck();
            bool success = true;

            //IP.SHOW_DEBUG_IMAGES = true; 
            var exitButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("exit_menu.png", _settings.Resolution), _r.BottomRightExit, confidence: .4, useGrayscale: true);
            //IP.SHOW_DEBUG_IMAGES = false;
            if (exitButton.Found)
                _mouse.ClickPosition(exitButton.CenterX, exitButton.CenterY, 1000);
            else
            {
                _logger.Log(LogDetailLevel.Debug, "Inventory Management Exit button not found, clicking escape");
                _kb.Press(Key.Escape, 1000);
            }



            var closeButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("x.png", _settings.Resolution), confidence: .8);
            if (closeButton.Found)
                _mouse.ClickPosition(closeButton.CenterX, closeButton.CenterY, 1000);
            else
            {
                _mouse.ClickTopScreen(_r.CenterScreen);
                _kb.Press(Key.Escape, 1000);
            }


            if (GameMenuOpen())
                CloseGameMenu();

            return success;
        }

        private void OpenPetMenu()
        {

            _mouse.ClickPosition(_r.GuideMenu, 800);
            _mouse.ClickPosition(_r.GuidePetMenu, 1500);
        }

        public bool GameMenuOpen()
        {
            var gameMenu = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("game_menu.png", _settings.Resolution), confidence: .7);
            return gameMenu.Found;
        }

        public void CloseGameMenu()
        {
            _mouse.ClickTopScreen(_r.CenterScreen);
            _kb.Press(Key.Escape);
        }

        public bool AuraRepair()
        {
            _logger.Log(LogDetailLevel.Debug, "Attempting Aura Repair");
            BackgroundProcessing.ProgressCheck();
            bool success = true;
            OpenPetMenu();

            var repairButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("repair_button.png", _settings.Resolution), confidence: .85);
            if (repairButton.Found)
            {
                _mouse.ClickPosition(repairButton.CenterX, repairButton.CenterY, 1000);
                var repairAllButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("repairall_button.png", _settings.Resolution), confidence: .80);

                //Debug.Print($"Repair All Confidence: {repairAllButton.MaxConfidence}");
                if (repairAllButton.Found)
                    _mouse.ClickPosition(repairAllButton.CenterX, repairAllButton.CenterY, 500);

                _kb.Press(Key.Escape, 500);
            }
            else
            {
                _logger.Log(LogDetailLevel.Debug, "Unable to find repair button while repairing gear, failing.");
                //success = false;
            }


            _kb.Press(Key.Escape, 500);

            return success;
        }

        public bool ExitGame()
        {
            _mouse.ClickCenterScreen(_r.CenterScreen);
            for (int i = 0; i < 60; i++)
            {
                if (!GameMenuOpen())
                {
                    _kb.Press(Key.Escape);
                    Sleep.SleepMs(700, 1000, _settings.PerformanceMultiplier);
                }
                else
                    break;
            }

            var quitButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("quit_button.png", _settings.Resolution), confidence: .9);
            if (quitButton.Found)
            {
                _mouse.ClickPosition(quitButton.Center, 1000);
                var okButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), confidence: .9);

                if (okButton.Found)
                {
                    _mouse.ClickPosition(okButton.Center, 1000);

                    return true;
                }
            }

            return false;
        }


        public bool SwapCharacters(UserCharacter character)
        {
            BackgroundProcessing.ProgressCheck();
            _mouse.ClickPosition(_r.ServicesMenu, 500);
            _mouse.ClickPosition(_r.GameMenu, 1000);
            _mouse.ClickPosition(_r.SwitchCharacterButton, 500);

            _mouse.SetPosition(_r.CenterScreen);

            _mouse.Scroll(MouseUtility.ScrollDirection.Up, 9);

            var index = character.CharacterIndex - 1;
            var row = index / 3;
            int col = index % 3;

            if (row > 2)
            {
                var numScrolls = row - 2;
                row = 2;
                _mouse.Scroll(MouseUtility.ScrollDirection.Down, numScrolls);
            }
            var charColumn = (int)_r.GetType().GetProperty($"CharSelectCol{col}").GetValue(_r);
            var charRow = (int)_r.GetType().GetProperty($"CharSelectRow{row}").GetValue(_r);

            _mouse.ClickPosition(charColumn, charRow, 500);
            _mouse.ClickPosition(_r.ConnectButton, 1000);

            //1550, 775, 300, 75,
            var okButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), confidence: .95);

            if (okButton.Found)
            {
                _mouse.ClickPosition(okButton.CenterX, okButton.CenterY, 15000);
                return true;
            }
            else
            {
                _logger.Log(LogDetailLevel.Debug, $"Button not found - max confidence: {okButton.MaxConfidence}");
                return false;
            }

        }
    }
}
