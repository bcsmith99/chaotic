using Chaotic.Extensions;
using Chaotic.Resources;
using Chaotic.Tasks.Chaos.Class;
using Chaotic.User;
using Chaotic.Utilities;
using OpenCvSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic.Tasks
{
    public class CubeTasks
    {
        private UserSettings _settings;
        private MouseUtility _mouse;
        private KeyboardUtility _kb;
        private ApplicationResources _r;
        private UITasks _uiTasks;
        private AppLogger _logger;

        public CubeTasks(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, UITasks uiTasks, AppLogger logger)
        {
            _settings = settings;
            _mouse = mouse;
            _kb = kb;
            _r = r;
            _uiTasks = uiTasks;
            _logger = logger;

            CenterScreen = _r.CenterScreen;
            MoveToPoint = CenterScreen;
            ClickableRegion = _r.ClickableRegion;
            ClickableOffset = _r.ClickableOffset;
        }

        public Point MoveToPoint { get; private set; }
        public Rect ClickableRegion { get; }
        public Point CenterScreen { get; }
        public Point ClickableOffset { get; }

        public ChaosClass CurrentCharacter { get; set; }

        public bool RunCube(UserCharacter character)
        {
            BackgroundProcessing.ProgressCheck();
            var success = true;

            if (character == null)
                return true;

            CurrentCharacter = ChaosClass.Create(_settings, character, _r, _kb, _mouse, _logger);

            return EnterCubeStart();
        }

        private bool ReEnterCube(ScreenSearchResult button)
        {
            Sleep.SleepMs(5000, 10000);

            var reEnter = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_reentry_button.png", _settings.Resolution, "cube"), confidence: .9, useGrayscale: true);
            if (reEnter.Found)
            {
                _logger.Log(LogDetailLevel.Debug, $"Attempting to Re-Enter Cube(Updated Loc), Clicking ({reEnter.CenterX},{reEnter.CenterY})");
                _mouse.ClickPosition(reEnter.Center, 1000, MouseButtons.Left);
            }

            else
            {
                _logger.Log(LogDetailLevel.Debug, $"Attempting to Re-Enter Cube(Old Loc), Clicking ({button.CenterX},{button.CenterY})");
                _mouse.ClickPosition(button.Center, 1000, MouseButtons.Left);
            }

            _mouse.ClickPosition(button.Center, 1000, MouseButtons.Left);

            var accept = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("accept_check_button.png", _settings.Resolution), confidence: .7, useGrayscale: true);

            if (accept.Found)
            {
                _mouse.ClickPosition(accept.Center, 15000, MouseButtons.Left);
                return EnterCubeStart();
            }
            else
            {
                var okButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), confidence: .85);
                if (okButton.Found)
                {
                    _mouse.ClickPosition(okButton.Center, 1000, MouseButtons.Left);
                    var exitButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("exit_chaos.png", _settings.Resolution), confidence: .65);
                    if (exitButton.Found)
                    {
                        _mouse.ClickPosition(exitButton.Center, 5000);
                        if (_uiTasks.InAreaCheck())
                        {
                            _logger.Log(LogDetailLevel.Info, "Succesfully Completed all Cube Tickets and Exited");
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                return false;
            }
        }

        private bool EnterCubeStart()
        {
            BackgroundProcessing.ProgressCheck();
            Sleep.SleepMs(2000, 2500);

            _mouse.ClickPosition(_r.CubeInitialMove, 2000, MouseButtons.Right);

            var start = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_start_alt.png", _settings.Resolution, "cube"), confidence: .6, useGrayscale: true);

            while (!start.Found)
            {
                Sleep.SleepMs(1000, 1500);
                start = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_start_alt.png", _settings.Resolution, "cube"), confidence: .6, useGrayscale: true);
            }

            _logger.Log(LogDetailLevel.Debug, $"Cube Start Location Found, Moving to it - {start.MaxConfidence}");
            Sleep.SleepMs(1000, 1000);
            _mouse.ClickPosition(CenterScreen.Add(new Point(100, 100)), 1000, MouseButtons.Right);

            Sleep.SleepMs(5000, 7000);

            var floorType = CheckCubeRoom();

            while (true)
            {
                BackgroundProcessing.ProgressCheck();
                var floorComplete = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_floor_complete.png", _settings.Resolution, "cube"), confidence: .7, useGrayscale: true);

                var reEnter = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_reentry_button.png", _settings.Resolution, "cube"), confidence: .7, useGrayscale: true);

                if (floorComplete.Found)
                    floorType = MoveToNextFloor();

                if (reEnter.Found)
                {
                    _logger.Log(LogDetailLevel.Debug, "Re-Enter Button initial found, going into re-enter loop");
                    return ReEnterCube(reEnter);
                }

                if (floorType == CubeFloors.Normal)
                    RunNormalFloor();
                else
                    RunGoldFloor();

                Sleep.SleepMs(200, 400);
            }
        }

        private void RunNormalFloor()
        {
            var mob_health = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("mob_health_bar.png", _settings.Resolution), confidence: .95, breakAfterFirst: true);

            if (mob_health.Found)
            {
                _logger.Log(LogDetailLevel.Debug, "Found Mob Health Bar");
                CurrentCharacter.UseAbilities(mob_health.Center.Add(new Point(50, 150)), 1, true);
            }

            else
                CurrentCharacter.UseAbilities(ClickableRegion.RandomPoint());
        }

        private void RunGoldFloor()
        {
            var mob_health = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("mob_health_bar.png", _settings.Resolution), confidence: .95, breakAfterFirst: true);

            if (mob_health.Found)
            {
                _logger.Log(LogDetailLevel.Debug, "Found Mob Health Bar");
                CurrentCharacter.UseAbilities(mob_health.Center.Add(new Point(50, 150)), 1, true);
            }

            else
                CurrentCharacter.UseAbilities(ClickableRegion.RandomPoint());
        }

        private CubeFloors MoveToNextFloor()
        {
            Sleep.SleepMs(7000, 11000);
            _mouse.ClickPosition(_r.CubeNextFloor, 10000, MouseButtons.Right);
            _mouse.ClickPosition(_r.CubeMiddleFloor, 2000, MouseButtons.Right);

            var floorType = CheckCubeRoom();
            _logger.Log(LogDetailLevel.Debug, $"Cube Floor Type Set to {floorType.ToString()}");
            return floorType;
        }

        private CubeFloors CheckCubeRoom()
        {
            var floor = CubeFloors.Normal;

            var cube = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_fortune.png", _settings.Resolution, "cube"), confidence: .9, breakAfterFirst: true);
            if (cube.Found)
            {
                floor = CubeFloors.Gold;

                var cube_treasure = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_treasure.png", _settings.Resolution, "cube"), confidence: .9, breakAfterFirst: true);
                if (cube_treasure.Found)
                    return CubeFloors.GoldTreasure;
                var cube_tooki = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_tooki.png", _settings.Resolution, "cube"), confidence: .9, breakAfterFirst: true);
                if (cube_tooki.Found)
                    return CubeFloors.GoldTooki;
                var cube_tuturi = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_tuturi.png", _settings.Resolution, "cube"), confidence: .9, breakAfterFirst: true);
                if (cube_tuturi.Found)
                    return CubeFloors.GoldTuturi;
                var cube_dragon = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_dragon.png", _settings.Resolution, "cube"), confidence: .9, breakAfterFirst: true);
                if (cube_dragon.Found)
                    return CubeFloors.GoldDragon;
                var cube_flyer = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_flyer.png", _settings.Resolution, "cube"), confidence: .9, breakAfterFirst: true);
                if (cube_flyer.Found)
                    return CubeFloors.GoldDragon;
                var cube_slime = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_slime.png", _settings.Resolution, "cube"), confidence: .9, breakAfterFirst: true);
                if (cube_slime.Found)
                    return CubeFloors.GoldSlime;
            }

            return floor;
        }

        private enum CubeFloors
        {
            Normal,
            Gold,
            GoldTreasure,
            GoldTooki,
            GoldTuturi,
            GoldDragon,
            GoldSlime
        }
    }



}
