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
        private readonly ResourceHelper _r;
        private readonly AppLogger _logger;

        public UITasks(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ResourceHelper r, AppLogger logger)
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
                //TODO: Parameterize Coordinates for Resolution
                //,
                var inTownButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("chevron.png", _settings.Resolution), IP.ConvertStringCoordsToRect(_r["Chevron_Region"]), confidence: .65, useGrayscale: true);
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

        public void ToggleArkPassive(bool disable)
        {
            Sleep.SleepMs(500, 500);
            _mouse.ClickCenterScreen(_r);

            _mouse.ClickPosition(_r["CharacterMenu"], 500);
            _mouse.ClickPosition(_r["CharacterProfileMenu"], 1000);

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
            Sleep.SleepMs(300, 400);
            var ongoingButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ongoing_quests.png", _settings.Resolution), IP.ConvertStringCoordsToRect(_r["OngoingQuest_Region"]), .85, true);
            if (ongoingButton.Found)
            {
                Sleep.SleepMs(100, 200);
                _mouse.ClickPosition(ongoingButton.CenterX, ongoingButton.CenterY, 1000);
                var completeButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("complete_button.png", _settings.Resolution), confidence: .95);
                while (completeButton.Found)
                {
                    Sleep.SleepMs(100, 200);
                    _mouse.ClickPosition(completeButton.CenterX, completeButton.CenterY, 1000);
                    completeButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("complete_button.png", _settings.Resolution), confidence: .95);
                }
            }
        }

        public bool MoveGems()
        {
            var inventoryRegion = IP.ConvertStringCoordsToRect(_r["Inventory_Region"]);
            var success = true;
            var gemDirectory = Utility.ResourceLocation(_settings.Resolution, "gems");

            var moveGemLevel = _settings.GemLevelThreshold;
            var currentLevel = 9;

            while (currentLevel >= moveGemLevel)
            {
                var filePrefixes = new List<string>() { "t3_dmg_", "t3_cd_", "t4_dmg_", "t4_cd_" };

                foreach (var prefix in filePrefixes)
                {
                    var fileName = $"{gemDirectory}{prefix}{currentLevel}.png";
                    if (File.Exists(fileName))
                    {
                        var loopCount = 0;
                        var gems = IP.LocateOnScreen(fileName, inventoryRegion, .95);
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
            var inventoryRegion = IP.ConvertStringCoordsToRect(_r["Inventory_Region"]);
            var success = true;
            var honingMatsDirectory = Utility.ResourceLocation(_settings.Resolution, "mats");

            var materialFileNames = Directory.GetFiles(honingMatsDirectory).Select(x => Path.GetFileName(x));

            foreach (var materialFile in materialFileNames)
            {
                var loopCount = 0;

                var material = IP.LocateOnScreen($"{honingMatsDirectory}{materialFile}", inventoryRegion, .85);
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

            Sleep.SleepMs(200, 300);

            return success;
        }

        public bool OpenInventoryManagement()
        {
            bool success = true;
            OpenPetMenu();

            var remoteStorageButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("remotestorage_button.png", _settings.Resolution), confidence: .85);
            if (remoteStorageButton.Found)
                _mouse.ClickPosition(remoteStorageButton.CenterX, remoteStorageButton.CenterY, 1000);
            else
                success = false;

            return success;
        }

        public bool CloseInventoryManagement()
        {
            bool success = true;

            //IP.SHOW_DEBUG_IMAGES = true; 
            var exitButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("exit_menu.png", _settings.Resolution), IP.ConvertStringCoordsToRect(_r["BottomRight_Exit"]), confidence: .4, useGrayscale: true);
            //IP.SHOW_DEBUG_IMAGES = false;
            if (exitButton.Found)
                _mouse.ClickPosition(exitButton.CenterX, exitButton.CenterY, 1000);
            else
                _kb.Press(Key.Escape, 1000);


            var closeButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("x.png", _settings.Resolution), confidence: .8);
            if (closeButton.Found)
                _mouse.ClickPosition(closeButton.CenterX, closeButton.CenterY, 1000);
            else
            {
                _mouse.ClickTopScreen(_r);
                _kb.Press(Key.Escape, 1000);
            }


            if (GameMenuOpen())
                CloseGameMenu();

            return success;
        }

        private void OpenPetMenu()
        {
            _mouse.ClickPosition(_r["GuideMenu"], 500);
            _mouse.ClickPosition(_r["GuidePetMenu"], 1000);
        }

        public bool GameMenuOpen()
        {
            var gameMenu = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("game_menu.png", _settings.Resolution), confidence: .7);
            return gameMenu.Found;
        }

        public void CloseGameMenu()
        {
            _mouse.ClickTopScreen(_r);
            _kb.Press(Key.Escape);
        }

        public bool AuraRepair()
        {
            bool success = true;
            OpenPetMenu();

            var repairButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("repair_button.png", _settings.Resolution), confidence: .85);
            if (repairButton.Found)
            {
                _mouse.ClickPosition(repairButton.CenterX, repairButton.CenterY, 1000);
                var repairAllButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("repairall_button.png", _settings.Resolution), confidence: .95);

                Debug.Print($"Repair All Confidence: {repairAllButton.MaxConfidence}");
                if (repairAllButton.Found)
                    _mouse.ClickPosition(repairAllButton.CenterX, repairAllButton.CenterY, 500);

                _kb.Press(Key.Escape, 500);
            }
            else
                success = false;

            _kb.Press(Key.Escape, 500);

            return success;
        }


        public bool SwapCharacters(UserCharacter character)
        {
            _mouse.ClickPosition(_r["ServicesMenu"], 500);
            _mouse.ClickPosition(_r["GameMenu"], 1000);
            _mouse.ClickPosition(_r["SwitchCharacterButton"], 500);

            _mouse.SetPosition(_r["CenterScreen"]);

            _mouse.Scroll(MouseUtility.ScrollDirection.Up, 5);

            var index = character.CharacterIndex - 1;
            var row = index / 3;
            int col = index % 3;

            if (row > 2)
            {
                var numScrolls = row - 2;
                row = 2;
                _mouse.Scroll(MouseUtility.ScrollDirection.Down, numScrolls);
            }

            _mouse.ClickPosition(_r[$"CharSelectCol{col}"], _r[$"CharSelectRow{row}"], 500);
            _mouse.ClickPosition(_r["ConnectButton"], 1000);

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
