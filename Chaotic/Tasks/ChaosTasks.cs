﻿using Chaotic.Tasks.Chaos.Class;
using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenCvSharp;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;
using System.CodeDom.Compiler;
using Accessibility;
using System.Windows.Documents;
using System.Drawing.Printing;
using System.DirectoryServices;
using Chaotic.Tasks.Chaos;
using Chaotic.Tasks.Chaos.Kurzan;
using System.Linq.Expressions;

namespace Chaotic.Tasks
{
    public class ChaosTasks
    {
        private readonly UserSettings _settings;
        private readonly MouseUtility _mouse;
        private readonly KeyboardUtility _kb;
        private readonly ResourceHelper _r;
        private readonly UITasks _uiTasks;
        private readonly AppLogger _logger;

        private Point MoveToPoint { get; set; }
        public int MoveTime { get; set; }
        private ChaosStates CurrentState { get; set; }
        private bool BossBarLocated { get; set; } = false;
        public DateTime LastHealthPotUsed { get; set; } = DateTime.MinValue;

        public Rect ClickableRegion { get; set; }
        public Point CenterScreen { get; set; }

        public Point ClickableOffset { get; set; }

        public enum ChaosStates
        {
            Loading,
            InCity,
            KurzanMap1,
            KurzanMap2,
            KurzanMap3,
            Floor1,
            Floor2,
            Floor3,
            Cleared
        }

        public ChaosTasks(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ResourceHelper r, UITasks uiTasks, AppLogger logger)
        {
            _settings = settings;
            _mouse = mouse;
            _kb = kb;
            _r = r;
            _uiTasks = uiTasks;
            _logger = logger;

            var centerScreen = IP.GetPointFromStringCoords(_r["CenterScreen"]);

            MoveToPoint = new Point(centerScreen.X, centerScreen.Y);
            ClickableRegion = IP.ConvertStringCoordsToRect(_r["Clickable_Region"]);
            CenterScreen = _r.Point("CenterScreen");
            ClickableOffset = _r.Point("Clickable_Offset");
        }


        public bool RunChaos(UserCharacter character, int chaosCount = 2)
        {
            if (character == null || !character.RunChaos)
                return true;

            var cc = ChaosClass.Create(_settings, character, _r, _kb, _mouse, _logger);

            if (character.ChaosLevel < 1640)
            {
                for (int i = 0; i < chaosCount; i++)
                {
                    if (EnterChaosDungeon(character))
                    {
                        var result = TaskOutcomes.Failure;
                        DateTime chaosStart = DateTime.Now;
                        DateTime? floor1Start = null, floor2Start = null, floor3Start = null,
                            floor1End = null, floor2End = null, floor3End = null, chaosEnd = null;

                        _logger.Log(LogDetailLevel.Debug, "Made it into actual processing space");
                        floor1Start = DateTime.Now;
                        RunChaosFloorOne(cc);
                        floor1End = DateTime.Now;
                        chaosEnd = DateTime.Now;
                        _logger.Log(LogDetailLevel.Info, $"Floor 1 Complete - {floor1End.Value.Subtract(floor1Start.Value).TotalSeconds.ToString("0.##")}s elapsed");
                        if (TimeoutCheck())
                        {
                            result = TaskOutcomes.Timeout;
                            QuitChaos(true);
                            AddChaosTaskStatistic(result, character, chaosStart, chaosEnd, floor1Start, floor1End, floor2Start, floor2End, floor3Start, floor3End);
                            return false;
                        }
                        WaitForLoading();
                        floor2Start = DateTime.Now;
                        RunChaosFloorTwo(cc);
                        floor2End = DateTime.Now;
                        chaosEnd = DateTime.Now;
                        _logger.Log(LogDetailLevel.Info, $"Floor 2 Complete - {floor2End.Value.Subtract(floor2Start.Value).TotalSeconds.ToString("0.##")}s elapsed");
                        if (TimeoutCheck())
                        {
                            result = TaskOutcomes.Timeout;
                            QuitChaos(true);
                            AddChaosTaskStatistic(result, character, chaosStart, chaosEnd, floor1Start, floor1End, floor2Start, floor2End, floor3Start, floor3End);
                            return false;
                        }
                        WaitForLoading();
                        floor3Start = DateTime.Now;
                        if (!RunChaosFloorThree(cc))
                        {
                            floor3End = DateTime.Now;
                            chaosEnd = DateTime.Now;
                            _logger.Log(LogDetailLevel.Debug, "Failed to make it out of floor 3");
                            AddChaosTaskStatistic(result, character, chaosStart, chaosEnd, floor1Start, floor1End, floor2Start, floor2End, floor3Start, floor3End);
                            return false;
                        }

                        if (TimeoutCheck())
                        {
                            floor3End = DateTime.Now;
                            chaosEnd = DateTime.Now;
                            result = TaskOutcomes.Timeout;
                            AddChaosTaskStatistic(result, character, chaosStart, chaosEnd, floor1Start, floor1End, floor2Start, floor2End, floor3Start, floor3End);
                            return QuitChaos(true);
                        }
                        else
                        {
                            floor3End = DateTime.Now;
                            chaosEnd = DateTime.Now;
                            result = TaskOutcomes.Success;
                            _logger.Log(LogDetailLevel.Info, $"Floor 3 Complete - {floor3End.Value.Subtract(floor3Start.Value).TotalSeconds.ToString("0.##")}s elapsed");
                            _logger.Log(LogDetailLevel.Summary, $"Chaos Dungeon Complete on {character.ClassName}, Total Elapsed: {chaosEnd.Value.Subtract(chaosStart).ToString(@"mm\:ss")}");
                            AddChaosTaskStatistic(result, character, chaosStart, chaosEnd, floor1Start, floor1End, floor2Start, floor2End, floor3Start, floor3End);
                        }
                    }
                    else if (CheckAuraExpended())
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (character.DisableArkPassive)
                    _uiTasks.ToggleArkPassive(true);

                if (EnterKurzanFront(character))
                {
                    var outcome = TaskOutcomes.Failure;
                    var startTime = DateTime.Now;
                    DateTime endTime;
                    _logger.Log(LogDetailLevel.Summary, $"Entering Kurzan Front on {character.ClassName}");
                    var km = KurzanBase.CreateMap(CurrentState, _settings, _mouse, _kb, _r, _logger);

                    ClearQuests();

                    km.StartMapMove();
                    cc.StartUp();

                    if (WatchForMobs(cc, km))
                    {
                        endTime = DateTime.Now;
                        if (QuitChaos())
                        {
                            outcome = TaskOutcomes.Success;
                            _logger.Log(LogDetailLevel.Summary, $"Kurzan Front completed on {character.ClassName}, Total Elapsed: {endTime.Subtract(startTime).ToString(@"mm\:ss")}");
                        }
                    }
                    else
                        endTime = DateTime.Now;

                    if (TimeoutCheck())
                    {
                        QuitChaos(true);
                        outcome = TaskOutcomes.Timeout;
                        _logger.Log(LogDetailLevel.Summary, $"Kurzan Front timed out on {character.ClassName}, Total Elapsed: {endTime.Subtract(startTime).TotalSeconds.ToString(@"mm\:ss")}");
                    }

                    AddKurzanTaskStatistic(outcome, character, km, startTime, endTime);

                    if (character.DisableArkPassive)
                        _uiTasks.ToggleArkPassive(false);
                }
                else if (CheckAuraExpended())
                {
                    return true;
                }
            }

            return true;
        }

        private void AddKurzanTaskStatistic(string outcome, UserCharacter character, KurzanBase map, DateTime startTime, DateTime endTime)
        {
            TimeSpan totalTime = endTime.Subtract(startTime);

            _logger.AddStatisticEntry(new KurzanTaskStatistic()
            {
                CharacterIdentifier = character.Identifier,
                TaskOutcome = outcome,
                StartDate = startTime,
                TotalDuration = totalTime,
                ChaosLevel = character.ChaosLevel,
                Class = character.ClassName,
                Map = map.MapName
            });
        }

        private void AddChaosTaskStatistic(string result, UserCharacter character, DateTime chaosStart, DateTime? chaosEnd, DateTime? floor1Start, DateTime? floor1End, DateTime? floor2Start, DateTime? floor2End, DateTime? floor3Start, DateTime? floor3End)
        {
            TimeSpan totalTime = TimeSpan.Zero;
            TimeSpan floor1Time = TimeSpan.Zero;
            TimeSpan floor2Time = TimeSpan.Zero;
            TimeSpan floor3Time = TimeSpan.Zero;

            if (chaosEnd.HasValue)
                totalTime = chaosEnd.Value.Subtract(chaosStart);
            if (floor1Start.HasValue && floor1End.HasValue)
                floor1Time = floor1End.Value.Subtract(floor1Start.Value);
            if (floor2Start.HasValue && floor2End.HasValue)
                floor2Time = floor2End.Value.Subtract(floor2Start.Value);
            if (floor3Start.HasValue && floor3End.HasValue)
                floor3Time = floor3End.Value.Subtract(floor3Start.Value);

            _logger.AddStatisticEntry(new ChaosTaskStatistic()
            {
                CharacterIdentifier = character.Identifier,
                TaskOutcome = result,
                StartDate = chaosStart,
                TotalDuration = totalTime,
                ChaosLevel = character.ChaosLevel,
                Class = character.ClassName,
                Floor1Duration = floor1Time,
                Floor2Duration = floor2Time,
                Floor3Duration = floor3Time,
            });
        }

        public bool CheckAuraExpended()
        {
            var auraGone = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("aura_done.png", _settings.Resolution), ClickableRegion, .7);
            if (auraGone.Found)
            {
                _kb.Press(Key.Escape, 700);
                _kb.Press(Key.Escape, 700);
                _kb.Press(Key.Escape, 700);

                return true;
            }
            return false;
        }

        public bool WatchForMobs(ChaosClass cc, KurzanBase map)
        {
            var currentTime = DateTime.Now;
            var maxTime = currentTime.AddMinutes(7);

            var minimapRegion = IP.ConvertStringCoordsToRect(_r["MinimapRegion"]);
            while (currentTime < maxTime)
            {
                try
                {
                    HealthCheck(cc);
                    DeathCheck();

                    //1500, 1000, 500, 400,
                    var ok_chaos = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_chaos.png", _settings.Resolution), IP.ConvertStringCoordsToRect(_r["ChaosOk_Region"]), confidence: .70);

                    if (ok_chaos.Found)
                    {
                        _logger.Log(LogDetailLevel.Info, "Ok Button found - quitting chaos");
                        return true;
                    }

                    var boss_mob = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("boss_mob_middle.png", _settings.Resolution), minimapRegion, .7);
                    var gold_mob = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("gold_mob_middle.png", _settings.Resolution), minimapRegion, .75);
                    var elite_mob = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("elite_mob_middle.png", _settings.Resolution), minimapRegion, .86);

                    if (CurrentState == ChaosStates.KurzanMap1)
                    {

                    }
                    map.PerformSpecialChecks();

                    //check map sticking point 
                    var stuck = map.CheckIfStuck();

                    if (stuck.Found)
                    {
                        _logger.Log(LogDetailLevel.Debug, "Stuck on map.  Moving to get outta it");
                        MoveOnScreen(stuck.CenterX, stuck.CenterY, 1500, 2000);
                    }

                    if (boss_mob.Found)
                    {
                        _logger.Log(LogDetailLevel.Debug, $"Boss Mob Found - {boss_mob.MaxConfidence}");
                        MoveToMinimapPos(boss_mob.CenterX, boss_mob.CenterY, 1000, 1500);
                        cc.UseAwakening(MoveToPoint);
                        cc.UseAbilities(MoveToPoint, 2);
                    }
                    else if (elite_mob.Found)
                    {
                        _logger.Log(LogDetailLevel.Debug, $"Elite Mob Found, Confidence: {elite_mob.MaxConfidence}");
                        MoveToMinimapPos(elite_mob.CenterX, elite_mob.CenterY, 500, 1000);
                        cc.UseAbilities(MoveToPoint, 2);
                    }
                    else if (gold_mob.Found)
                    {
                        _logger.Log(LogDetailLevel.Debug, $"Gold Mob Found - {gold_mob.MaxConfidence}");
                        MoveToMinimapPos(gold_mob.CenterX, gold_mob.CenterY, 1000, 1500);
                        cc.UseAbilities(MoveToPoint, 2);
                    } 
                    else
                    {
                        var point = map.PreferredRandomPoint();
                        RandomMove(500, 500, point, 60);
                        cc.UseAbilities(MoveToPoint, 1);
                    }

                    Sleep.SleepMs(50, 150);
                    currentTime = DateTime.Now;
                }
                catch (Exception)
                {
                    break;
                }
            }
            return false;
        }

        public void DetectMapImage()
        {
            //Look for jump portal and take it.
            var jump_pad = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("kurzan_map1_jumppoint.png", _settings.Resolution), confidence: .6);
            if (jump_pad.Found)
            {
                while (true)
                {
                    _logger.Log(LogDetailLevel.Debug, $"Jump pad found, Confidence: {jump_pad.MaxConfidence}");
                    _mouse.ClickPosition(jump_pad.CenterX, jump_pad.CenterY, 1500, MouseButtons.Right);
                    _kb.Press(Key.G, 1000);

                    jump_pad = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("kurzan_map1_jumppoint.png", _settings.Resolution), confidence: .6);
                    if (!jump_pad.Found)
                        break;
                }
            }
        }

        private void WaitForLoading()
        {
            var startTime = DateTime.Now;
            var maxWait = 120000;
            while (true)
            {
                _logger.Log(LogDetailLevel.Debug, "Waiting for loading");
                var currentTime = DateTime.Now;
                if (currentTime.Subtract(startTime).TotalMilliseconds > maxWait)
                {
                    throw new Exception("Waited too long to enter, crashing out of execution");
                }

                var leaveButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("exit_chaos.png", _settings.Resolution), IP.ConvertStringCoordsToRect(_r["ChaosLeave_Region"]), .7, true);

                if (leaveButton.Found)
                    return;
                Sleep.SleepMs(450, 550);
            }
        }

        public void TestChaos()
        {
            var minimapCenter = _r.Point("MinimapCenter");
            SetMoveToPoint(minimapCenter.X + 50, minimapCenter.Y + 50);
            CalcMinimapPos(MoveToPoint.X, MoveToPoint.Y);
        }

        public void RunChaosFloorOne(ChaosClass cc)
        {
            CurrentState = ChaosStates.Floor1;
            ClearQuests();
            Sleep.SleepMs(500, 600);

            AuraRepair();

            _mouse.ClickPosition(_r["CenterScreen"], 500, MouseButtons.Right);

            if (OfflineCheck() || GameCrashCheck())
                return;

            while (true)
            {
                if (OfflineCheck() || GameCrashCheck() || TimeoutCheck())
                    return;

                UseChaosDungeonAbilities(cc);
                _logger.Log(LogDetailLevel.Info, "Floor 1 cleared");

                if (CurrentState == ChaosStates.Floor2)
                    return;
                //CalcMinimapPos(MoveToPoint.X, MoveToPoint.Y);
                if (EnterPortal(cc))
                    break;
            }

            if (OfflineCheck() || GameCrashCheck() || TimeoutCheck())
                return;
            CurrentState = ChaosStates.Floor2;
            return;
        }

        public void RunChaosFloorTwo(ChaosClass cc)
        {
            BossBarLocated = false;
            ClearQuests();
            Sleep.SleepMs(500, 600);

            AuraRepair();

            var screenCenter = IP.GetPointFromStringCoords(_r["CenterScreen"]);

            _mouse.ClickPosition(screenCenter, 800, MouseButtons.Right);

            while (true)
            {
                if (OfflineCheck() || GameCrashCheck() || TimeoutCheck())
                    return;

                UseChaosDungeonAbilities(cc);

                _logger.Log(LogDetailLevel.Info, "Floor 2 cleared");
                if (CurrentState == ChaosStates.Floor3)
                {
                    _logger.Log(LogDetailLevel.Debug, "Floor 3 state already set on floor 2");
                    return;
                }

                _logger.Log(LogDetailLevel.Debug, "Cleared 2 and moving to portal");
                //CalcMinimapPos(MoveToPoint.X, MoveToPoint.Y);

                Sleep.SleepMs(500, 800);

                if (EnterPortal(cc))
                    break;
            }

            if (OfflineCheck() || GameCrashCheck() || TimeoutCheck())
                return;

            CurrentState = ChaosStates.Floor3;
            return;
        }

        public bool RunChaosFloorThree(ChaosClass cc)
        {
            CurrentState = ChaosStates.Floor3;

            ClearQuests();
            Sleep.SleepMs(500, 600);

            AuraRepair();

            var screenCenter = IP.GetPointFromStringCoords(_r["CenterScreen"]);

            _mouse.ClickPosition(screenCenter, 800, MouseButtons.Right);

            UseChaosDungeonAbilities(cc);

            _logger.Log(LogDetailLevel.Info, "Chaos Dungeon Full Cleared");
            return QuitChaos();
        }

        public bool QuitChaos(bool skipOk = false)
        {
            //1500, 1000, 500, 400,
            var ok_chaos = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_chaos.png", _settings.Resolution), confidence: .55);

            if (ok_chaos.Found || skipOk)
            {
                if (ok_chaos.Found)
                    _mouse.ClickPosition(ok_chaos.CenterX, ok_chaos.CenterY, 1000);

                //190, 350, 170, 50,
                var exit_chaos = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("exit_chaos.png", _settings.Resolution), confidence: .65);

                if (exit_chaos.Found)
                {
                    _mouse.ClickPosition(exit_chaos.CenterX, exit_chaos.CenterY, 1000);
                    //IP.SHOW_DEBUG_IMAGES = true;
                    //1550, 750, 250, 75,
                    var ok_button = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), confidence: .7);
                    if (ok_button.Found)
                    {
                        _logger.Log(LogDetailLevel.Debug, $"Found ok button to quit, confidence: {ok_button.MaxConfidence}");
                        _mouse.ClickPosition(ok_button.CenterX, ok_button.CenterY, 5000);
                        var exited = _uiTasks.InAreaCheck(30);
                        if (exited)
                            return true;
                    }
                }
            }

            return false;
        }

        private bool EnterPortal(ChaosClass cc)
        {
            CheckPortal();
            Sleep.SleepMs(1100, 1200);
            if (MoveTime > 550)
            {
                _mouse.ClickPosition(MoveToPoint.X, MoveToPoint.Y, 150, MouseButtons.Right);
            }
            var maxTimeMs = 6000;
            var enterTime = DateTime.Now;
            var screenCenter = IP.GetPointFromStringCoords(_r["CenterScreen"]);

            Rect minimapRegion = IP.ConvertStringCoordsToRect(_r["MinimapRegion"]);
            while (true)
            {
                var minimap = IP.CaptureScreen(minimapRegion);
                var c = minimap.GetPixel(minimap.Width / 2, minimap.Height / 2);
                if (c.R + c.G + c.B < 40)
                {
                    _logger.Log(LogDetailLevel.Debug, "Portal Entered");
                    _mouse.SetPosition(screenCenter.X, screenCenter.Y);
                    return true;
                }

                var currentTime = DateTime.Now;

                if (currentTime.Subtract(enterTime).TotalMilliseconds > maxTimeMs)
                {
                    cc.UseAbilities(MoveToPoint, 2);
                    return EnterPortal(cc);
                }

                if (MoveToPoint.X == screenCenter.X && MoveToPoint.Y == screenCenter.Y)
                    for (int i = 0; i < 10; i++)
                    {
                        _kb.Press(Key.G, 100);
                        Sleep.SleepMs(40, 60);
                        _logger.Log(LogDetailLevel.Debug, "Finished Enter Portal Loop");
                    }

                else
                {
                    _kb.Press(Key.G, 50);
                    _mouse.ClickPosition(MoveToPoint, 300, MouseButtons.Right);
                    _kb.Press(Key.G, 50);
                }
            }
        }

        private void UseChaosDungeonAbilities(ChaosClass cc)
        {
            while (true)
            {
                DeathCheck();
                HealthCheck(cc);

                if (GameCrashCheck() || OfflineCheck() || TimeoutCheck())
                    return;

                if (CurrentState == ChaosStates.Floor1 && CheckEliteMob())
                {
                    _logger.Log(LogDetailLevel.Debug, "Accidentally entered floor 2");
                    CurrentState = ChaosStates.Floor2;
                    return;
                }
                else if (CurrentState == ChaosStates.Floor2 && CheckTower())
                {
                    _logger.Log(LogDetailLevel.Debug, "Accidentally entered floor 3");
                    CurrentState = ChaosStates.Floor3;
                    return;
                }

                if (CurrentState == ChaosStates.Floor2 && !CheckEliteMob() && CheckRedMob())
                {
                    MoveOnScreen(MoveToPoint.X, MoveToPoint.Y, 400, 500, false);
                }
                else if (CurrentState == ChaosStates.Floor2 && !CheckEliteMob() && !CheckRedMob())
                {
                    _logger.Log(LogDetailLevel.Debug, "No mob on floor 2, random move");
                    RandomMove();
                }
                else if (CurrentState == ChaosStates.Floor2 && CheckBossMob())
                {
                    MoveOnScreen(MoveToPoint.X, MoveToPoint.Y, 500, 600, false);
                    if (CheckBossMobHealthBar())
                        cc.UseAwakening(MoveToPoint);
                }

                else if (CurrentState == ChaosStates.Floor1 && !CheckRedMob())
                {
                    _logger.Log(LogDetailLevel.Debug, "No mob on floor 1, random move");
                    RandomMove();
                }
                else if (CurrentState == ChaosStates.Floor3 && CheckEliteMob())
                {
                    MoveOnScreen(MoveToPoint.X, MoveToPoint.Y, 200, 300, false);
                    cc.UseAwakening(MoveToPoint);
                }

                if (CurrentState == ChaosStates.Floor3 && CheckChaosFinished())
                {
                    _logger.Log(LogDetailLevel.Info, "Floor 3 Cleared and Chaos Finished");
                    return;
                }

                if (CheckPortal() && (CurrentState == ChaosStates.Floor1 || CurrentState == ChaosStates.Floor2 || CurrentState == ChaosStates.Floor3))
                {
                    _mouse.SetPosition(_r["CenterScreen"]);
                    CheckPortal();
                    return;
                }

                if (CurrentState == ChaosStates.Floor3)
                    ClickChaosTower();

                if (CurrentState == ChaosStates.Floor1 && CheckRedMob())
                {
                    MoveOnScreen(MoveToPoint.X, MoveToPoint.Y, 400, 600);
                }

                else if (CurrentState == ChaosStates.Floor2)
                {

                    if (CheckBossMob())
                    {
                        MoveOnScreen(MoveToPoint.X, MoveToPoint.Y, 950, 1050, true);
                        if (CheckBossMobHealthBar())
                            cc.UseAwakening(MoveToPoint);
                    }
                    else if (CheckEliteMob())
                    {
                        MoveOnScreen(MoveToPoint.X, MoveToPoint.Y, 750, 850, false);
                    }
                }

                else if (CurrentState == ChaosStates.Floor3 && CheckTower())
                {
                    MoveOnScreen(MoveToPoint.X, MoveToPoint.Y, 1200, 1300, true);
                    ClickChaosTower();
                    if (!CheckEliteMob() && !CheckRedMob())
                    {
                        RandomMove(500, 1500, biasPoint: MoveToPoint, biasPercent: 75);
                        if (CheckTower())
                            ClickChaosTower();
                    }
                }
                else if (CurrentState == ChaosStates.Floor3 && CheckRedMob())
                {
                    MoveOnScreen(MoveToPoint.X, MoveToPoint.Y, 200, 300, false);
                    cc.UseAwakening(MoveToPoint);
                }
                else if (CurrentState == ChaosStates.Floor3 && CheckBossMob())
                {
                    DeathCheck();
                    MoveOnScreen(MoveToPoint.X, MoveToPoint.Y, 800, 900, false);
                }

                cc.UseAbilities(MoveToPoint, 4);

                if (CurrentState == ChaosStates.Floor3 && !CheckEliteMob() && !CheckBossMob())
                {
                    _logger.Log(LogDetailLevel.Debug, "Floor 3 random move");
                    RandomMove();
                }

            }
        }

        private List<System.Drawing.Point> _minimapSpiralized = null;
        public List<System.Drawing.Point> MinimapSpiralized
        {
            get
            {
                if (_minimapSpiralized == null)
                {
                    Rect minimapRegion = IP.ConvertStringCoordsToRect(_r["MinimapRegion"]);

                    _minimapSpiralized = GetPixelsSpiralized(Math.Max(minimapRegion.Width, minimapRegion.Height)).ToList();
                    return _minimapSpiralized;
                }
                else
                    return _minimapSpiralized;
            }
        }

        public IEnumerable<System.Drawing.Point> GetPixelsSpiralized(int size)
        {
            // Take a square size and spiral it from center outward.
            System.Drawing.Point point = new System.Drawing.Point(size / 2, size / 2);

            yield return point;
            int sign = 1;
            for (int row = 1; row < size; row++)
            {
                // move right/left by row, and then up/down by row
                for (int k = 0; k < row; k++)
                {
                    point.Offset(sign * 1, 0);
                    yield return point;
                }
                for (int k = 0; k < row; k++)
                {
                    point.Offset(0, -sign * 1);
                    yield return point;
                }
                sign *= -1;
            }
            // last leg to finish filling the area
            for (int k = 0; k < size - 1; k++)
            {
                point.Offset(sign * 1, 0);
                yield return point;
            }
        }

        //private List<Point> SpiralSearch(int rows, int cols, int rowStart, int colStart)
        //{
        //    var result = new List<Point>();
        //    var end = rows * cols;

        //    int i = rowStart, i1 = rowStart, i2 = rowStart;
        //    int j = colStart, j1 = colStart, j2 = colStart;

        //    while (true)
        //    {
        //        j2++;
        //        while (j < j2)
        //        {
        //            if (i <= j && j < cols && 0 <= i)
        //                result.Add(new Point(i, j));
        //            j++;
        //            if (0 > i)
        //                j = j2;
        //        }
        //        i2++;
        //        while (i < i2)
        //        {
        //            if (0 <= i && i < rows && j < cols)
        //                result.Add(new Point(i, j));
        //            i++;
        //            if (j >= cols)
        //                i = i2;
        //        }
        //        j1--;
        //        while (j > j1)
        //        {
        //            if (0 <= j && j < cols && i < rows)
        //                result.Add(new Point(i, j));
        //            j--;
        //            if (i >= rows)
        //                j = j1;
        //        }
        //        i1--;
        //        while (i > i1)
        //        {
        //            if (0 <= i && i < rows && 0 <= j)
        //                result.Add(new Point(i, j));
        //            i--;
        //            if (0 > j)
        //                i = i1;
        //        }
        //        if (result.Count == end)
        //            return result;
        //    }
        //}

        private void ClickChaosTower()
        {
            var riftCore = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("rift_core1.png", _settings.Resolution), confidence: .6);
            if (riftCore.Found)
            {
                if (riftCore.CenterY > 650 || riftCore.CenterX < 400 || riftCore.CenterY > 1500)
                    return;

                MoveToPoint = new Point(riftCore.CenterX, riftCore.CenterY + 190);
                _mouse.ClickPosition(MoveToPoint.X, MoveToPoint.Y, 500, MouseButtons.Right);
                _kb.Press(Key.C, 300);
                _kb.Press(Key.C, 300);
                _kb.Press(Key.C, 300);
                _kb.Press(Key.C, 300);
            }
        }

        public bool CheckChaosFinished()
        {
            //IP.SHOW_DEBUG_IMAGES = true;
            //1600, 1250, 200, 75, 
            var ok_chaos = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_chaos.png", _settings.Resolution), confidence: .55);

            if (ok_chaos.Found)
                _logger.Log(LogDetailLevel.Debug, "Ok Button found - quitting chaos");

            //IP.SHOW_DEBUG_IMAGES = false;
            return ok_chaos.Found;
        }

        private void SetMoveToPoint(int x, int y)
        {
            if (x < ClickableRegion.Left || y < ClickableRegion.Top || x > ClickableRegion.Right || y > ClickableRegion.Bottom)
                _logger.Log(LogDetailLevel.Debug, $"Coordinates out of bounds! Region: {ClickableRegion.Left}, {ClickableRegion.Top}, {ClickableRegion.Right}, {ClickableRegion.Bottom} - {x},{y}");
            MoveToPoint = new Point(x, y);
        }

        private bool CheckPortal()
        {
            var portalImages = new List<string>()
            {
                "portal.png",
                "portal_top.png",
                "portal_bot.png"
            };
            Rect minimapRegion = IP.ConvertStringCoordsToRect(_r["MinimapRegion"]);

            ScreenSearchResult portal = new ScreenSearchResult();

            foreach (var image in portalImages)
            {
                //IP.SAVE_DEBUG_IMAGES = true;
                portal = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation(image, _settings.Resolution), minimapRegion, confidence: .8);
                if (portal.Found)
                {
                    //IP.SAVE_DEBUG_IMAGES = false;
                    break;
                }
            }
            //IP.SAVE_DEBUG_IMAGES = false;

            if (portal.Found)
            {
                _logger.Log(LogDetailLevel.Debug, $"Portal Image Found, Confidence: {portal.MaxConfidence}");
                CalcMinimapPos(portal.CenterX, portal.CenterY);

                return true;
            }
            _logger.Log(LogDetailLevel.Debug, "Portal not found");
            return false;
        }

        private bool CheckBossMobHealthBar()
        {
            _logger.Log(LogDetailLevel.Debug, "Checking Boss Health Bar");
            Rect bossRegion = IP.ConvertStringCoordsToRect(_r["BossMobHealth_Region"]);
            var boss = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("boss_bar.png", _settings.Resolution), bossRegion, confidence: .75);
            if (boss.Found)
            {
                _logger.Log(LogDetailLevel.Debug, $"Found Boss  Health, Max Confidence: {boss.MaxConfidence}, Position: X: {boss.CenterX}, Y: {boss.CenterY}");
                return true;
            }
            return false;
        }

        private bool CheckBossMob()
        {
            Rect minimapRegion = IP.ConvertStringCoordsToRect(_r["MinimapRegion"]);
            var boss = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("boss_mob_middle.png", _settings.Resolution), minimapRegion, confidence: .7);
            if (boss.Found)
            {
                _logger.Log(LogDetailLevel.Debug, $"Found Boss, Max Confidence: {boss.MaxConfidence}, Position: X: {boss.CenterX}, Y: {boss.CenterY}");
                CalcMinimapPos(boss.CenterX, boss.CenterY);
                return true;
            }
            return false;
        }

        private bool CheckRedMob()
        {
            Rect minimapRegion = IP.ConvertStringCoordsToRect(_r["MinimapRegion"]);
            var minimap = IP.CaptureScreen(minimapRegion);

            foreach (var entry in MinimapSpiralized)
            {
                if (entry.X >= minimap.Width || entry.Y >= minimap.Height)
                    continue;

                var c = minimap.GetPixel(entry.X, entry.Y);
                bool inRange = (Enumerable.Range(206, 211).Contains(c.R) && Enumerable.Range(22, 27).Contains(c.G) && Enumerable.Range(22, 27).Contains(c.B));
                if (inRange)
                {
                    CalcMinimapPos(minimapRegion.Left + entry.X, minimapRegion.Top + entry.Y);
                    return true;
                }
            }
            return false;
        }

        private bool CheckEliteMob()
        {
            Rect minimapRegion = IP.ConvertStringCoordsToRect(_r["MinimapRegion"]);
            var elite = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("elite_mob_middle.png", _settings.Resolution), minimapRegion, confidence: .9);
            if (elite.Found)
            {
                _logger.Log(LogDetailLevel.Debug, $"Found Elite, Max Confidence: {elite.MaxConfidence}, Position: X: {elite.CenterX}, Y: {elite.CenterY}");
                CalcMinimapPos(elite.CenterX, elite.CenterY);
                _logger.Log(LogDetailLevel.Debug, $"New Elite MoveToPoint: X: {MoveToPoint.X}, Y:{MoveToPoint.Y}");
                return true;
            }
            return false;
        }

        private bool CheckTower()
        {
            var towerImages = new List<string>()
            {
                "tower.png",
                "tower_top.png",
                "tower_bot.png"
            };
            Rect minimapRegion = IP.ConvertStringCoordsToRect(_r["MinimapRegion"]);

            ScreenSearchResult tower = new ScreenSearchResult();

            foreach (var image in towerImages)
            {
                tower = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation(image, _settings.Resolution), minimapRegion, confidence: .75);
                if (tower.Found)
                    break;
            }

            if (tower.Found)
            {
                _logger.Log(LogDetailLevel.Debug, $"Tower Found Confidence: {tower.MaxConfidence}");
                CalcMinimapPos(tower.CenterX, tower.CenterY);


                if (MoveToPoint.Y < CenterScreen.Y && MoveToPoint.Y > ClickableRegion.Top + ClickableOffset.Y)
                {
                    var newRandomY = Random.Shared.Next(ClickableRegion.Top, ClickableRegion.Top + ClickableOffset.Y);
                    SetMoveToPoint(MoveToPoint.X, newRandomY);
                }

                if (MoveToPoint.Y > CenterScreen.Y && MoveToPoint.Y < ClickableRegion.Bottom - ClickableOffset.Y)
                {
                    var newRandomY = Random.Shared.Next(ClickableRegion.Bottom - ClickableOffset.Y, ClickableRegion.Bottom);
                    SetMoveToPoint(MoveToPoint.X, newRandomY);
                }


                return true;
            }

            return false;
        }


        private void HealthCheck(ChaosClass cc)
        {
            var currentTime = DateTime.Now;
            if (currentTime.Subtract(LastHealthPotUsed).TotalSeconds < 15)
                return;

            var healthBarRegion = IP.ConvertStringCoordsToRect(_r["HealthBar_Region"]);
            var percentToPot = _settings.UsePotionPercent;
            var healthPotKey = _settings.HealthPotionKey;

            if (percentToPot <= 0 || percentToPot > 100 || String.IsNullOrWhiteSpace(healthPotKey))
                return;

            var y = healthBarRegion.Height / 2;
            var x = healthBarRegion.Width * percentToPot / 100;

            if (cc.GetType() == typeof(Berserker))
                x = (int)(healthBarRegion.Width * .25) * percentToPot / 100;

            var healthBar = IP.CaptureScreen(healthBarRegion);

            var pixel1 = healthBar.GetPixel(x, y);
            var pixel2 = healthBar.GetPixel(x + 3, y);
            var pixel3 = healthBar.GetPixel(x - 3, y);

            if (pixel1.R < 30 || pixel2.R < 30 || pixel3.R < 30)
            {
                _kb.Press(healthPotKey, 50);
                _logger.Log(LogDetailLevel.Debug, "Pressed health pot key");
                LastHealthPotUsed = DateTime.Now;
            }
        }


        private void DeathCheck()
        {
            var dead = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("dead.png", _settings.Resolution), confidence: .75);

            if (dead.Found)
            {
                _logger.Log(LogDetailLevel.Debug, "You ded.");
                return;
            }
        }

        public bool TimeoutCheck()
        {
            var timeoutRegion = IP.ConvertStringCoordsToRect(_r["Timeout_Region"]);
            var timeout = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("timeout1.png", _settings.Resolution), timeoutRegion, confidence: .9);
            if (timeout.Found)
                _logger.Log(LogDetailLevel.Info, $"Timeout1 found. Confidence : {timeout.MaxConfidence}");
            else
            {
                timeout = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("timeout2.png", _settings.Resolution), timeoutRegion, confidence: .9);
                if (timeout.Found)
                    _logger.Log(LogDetailLevel.Info, $"Timeout2 found. Confidence : {timeout.MaxConfidence}");
            }

            return timeout.Found;
        }

        private bool OfflineCheck()
        {
            return false;
        }

        private bool GameCrashCheck()
        {
            return false;
        }

        private void AuraRepair()
        {

        }

        private void ClearQuests()
        {
            var quest = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("quest.png", _settings.Resolution), confidence: .85);
            if (quest.Found)
            {
                _mouse.ClickPosition(quest.CenterX, quest.CenterY, 1500);
                _kb.Press(Key.Escape, 1500);
            }
        }

        private bool EnterChaosDungeon(UserCharacter character)
        {
            var retVal = true;
            _mouse.ClickCenterScreen(_r);

            retVal = SelectChaosDungeon(character);

            if (retVal)
            {
                var enterButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("enter_button.png", _settings.Resolution), confidence: .95);

                if (enterButton.Found)
                {
                    _mouse.ClickPosition(enterButton.CenterX, enterButton.CenterY, 1000);
                    //ImageProcessing.SHOW_DEBUG_IMAGES = true;
                    var acceptButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("accept_check_button.png", _settings.Resolution), confidence: .95);
                    //ImageProcessing.SHOW_DEBUG_IMAGES = false;
                    if (acceptButton.Found)
                    {
                        _logger.Log(LogDetailLevel.Debug, "Accept Button Found");
                        _mouse.ClickPosition(acceptButton.CenterX, acceptButton.CenterY, 5000);
                        if (_uiTasks.InAreaCheck())
                        {
                            CurrentState = ChaosStates.Floor1;

                            _logger.Log(LogDetailLevel.Debug, "Made it into Chaos Dungeon");
                        }
                        else
                            return false;
                    }
                    else
                    {
                        _logger.Log(LogDetailLevel.Debug, $"Accept Not Found, Highest Confidence: {acceptButton.MaxConfidence}");
                        return false;
                    }

                }
                else
                    return false;

            }

            return true;
        }

        private bool EnterKurzanFront(UserCharacter character)
        {
            CurrentState = ChaosStates.InCity;
            _mouse.ClickCenterScreen(_r);
            _kb.AltPress(Key.Q, 1000);
            _mouse.ClickPosition(_r[$"Kurzan{character.ChaosLevel}"], 300);

            var cc = ChaosClass.Create(_settings, character, _r, _kb, _mouse, _logger);

            var enterButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("enter_button.png", _settings.Resolution), confidence: .95);

            if (enterButton.Found)
            {
                _mouse.ClickPosition(enterButton.CenterX, enterButton.CenterY, 1000);
                //ImageProcessing.SHOW_DEBUG_IMAGES = true;
                var acceptButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("accept_check_button.png", _settings.Resolution), confidence: .95);
                //ImageProcessing.SHOW_DEBUG_IMAGES = false;
                if (acceptButton.Found)
                {
                    _mouse.ClickPosition(acceptButton.CenterX, acceptButton.CenterY, 5000);
                    if (_uiTasks.InAreaCheck())
                    {
                        //_logger.Log(LogDetailLevel.Debug, "Made it into Chaos Dungeon");
                        DetectKurzanMap();
                    }
                    else
                        return false;
                }
                else
                {
                    _logger.Log(LogDetailLevel.Debug, $"Accept Not Found, Highest Confidence: {acceptButton.MaxConfidence}");
                    return false;
                }

            }
            else
                return false;

            return true;
        }

        public void DetectKurzanMap()
        {
            var tries = 1;
            var maxTries = 20;
            var minimapRegion = ImageProcessing.ConvertStringCoordsToRect(_r["MinimapRegion"]);

            while (tries <= maxTries)
            {
                var breakLoop = false;
                var map1 = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("kurzan_map1.png", _settings.Resolution), minimapRegion, .75);
                var map2 = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("kurzan_map2.png", _settings.Resolution), minimapRegion, .75);
                var map3 = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("kurzan_map3.png", _settings.Resolution), minimapRegion, .75);

                if (map1.Found)
                {
                    CurrentState = ChaosStates.KurzanMap1;
                    breakLoop = true;
                }

                if (map2.Found)
                {
                    CurrentState = ChaosStates.KurzanMap2;
                    breakLoop = true;
                }
                if (map3.Found)
                {
                    CurrentState = ChaosStates.KurzanMap3;
                    breakLoop = true;
                }

                if (breakLoop)
                    break;
            }
        }

        private bool SelectChaosDungeon(UserCharacter character)
        {

            //Todo remove alt-q and go through adventure menu. 
            _kb.AltPress(Key.Q, 1000);
            _mouse.ClickPosition(_r["ChaosDungeon_Shortcut"], 500);

            var claimAll = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("claimall_button.png", _settings.Resolution), IP.ConvertStringCoordsToRect(_r["ClaimAll_Region"]), .95);

            if (claimAll.Found)
            {
                _mouse.ClickPosition(claimAll.Center, 500);
                var okButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), ClickableRegion, .75);
                if (okButton.Found)
                    _mouse.ClickPosition(okButton.Center, 500);
            }

            _mouse.ClickPosition(_r["ChaosDungeon_RightArrow"], 300);
            _mouse.ClickPosition(_r["ChaosDungeon_RightArrow"], 300);
            _mouse.ClickPosition(_r["ChaosDungeon_RightArrow"], 300);

            _mouse.ClickPosition(GetChaosDungeonTabCoords(character.ChaosLevel), 500);
            _mouse.ClickPosition(_r[$"ChaosDungeon_{character.ChaosLevel}"], 500);

            return true;
        }

        private string GetChaosDungeonTabCoords(int itemLevel)
        {
            if (itemLevel >= 1415 && itemLevel < 1580)
                return _r["ChaosDungeon_Vern"];
            else if (itemLevel >= 1580 && itemLevel < 1610)
                return _r["ChaosDungeon_Elgacia"];
            else if (itemLevel >= 1610 && itemLevel < 1640)
                return _r["ChaosDungeon_Voldis"];
            else
                throw new ArgumentOutOfRangeException("Unknown Chaos Dungeon Entry");
        }



        private void MoveToMinimapPos(int x, int y, int minTime, int maxTime, bool blink = false)
        {
            var pos = CalcMinimapPos(x, y);
            MoveOnScreen(pos.X, pos.Y, minTime, maxTime, blink);
        }

        public void MoveOnScreen(int x, int y, int minTime, int maxTime, bool blink = false)
        {
            var centerScreen = _r.Point("CenterScreen");
            if (x == centerScreen.X && x == centerScreen.Y)
                return;

            if (MoveTime < 50)
                return;

            int wait = MoveTime / 2;

            if (wait < minTime)
                wait = minTime;
            else if (wait > maxTime)
                wait = maxTime;

            _mouse.ClickPosition(x, y, wait, MouseButtons.Right);

            _mouse.ClickPosition(centerScreen.X, centerScreen.Y, 50, MouseButtons.Right);
            return;
        }

        private Point GetRandomPointFromRegion(int xMult, int yMult, Rect region, int xOffsset, int yOffset)
        {
            int x, y;
            if (xMult < 0)
                x = Random.Shared.Next(region.Left, region.Left + xOffsset);
            else if (xMult == 0)
                x = Random.Shared.Next(CenterScreen.X - xOffsset, CenterScreen.X + xOffsset);
            else
                x = Random.Shared.Next(region.Right - xOffsset, region.Right);
            if (yMult < 0)
                y = Random.Shared.Next(region.Top, region.Top + yOffset);
            else if (yMult == 0)
                y = Random.Shared.Next(CenterScreen.Y - yOffset, CenterScreen.Y + yOffset);
            else
                y = Random.Shared.Next(region.Bottom - yOffset, region.Bottom);

            return new Point(x, y);
        }

        public void RandomMove(int minTime = 500, int maxTime = 1000, Point? biasPoint = null, int? biasPercent = null)
        {

            var randX = Random.Shared.Next(0, 100);
            int xMult, yMult;
            if (randX < 40)
                xMult = -1;
            else if (randX >= 40 && randX < 60)
                xMult = 0;
            else
                xMult = 1;

            var randY = Random.Shared.Next(0, 100);
            if (randY < 40)
                yMult = -1;
            else if (randY >= 40 && randY < 60)
                yMult = 0;
            else
                yMult = 1;

            Point randomPoint;

            if (biasPoint.HasValue)
            {
                var random = Random.Shared.Next(0, 100);
                if (random <= biasPercent)
                {
                    randomPoint = new Point(biasPoint.Value.X, biasPoint.Value.Y);
                    _logger.Log(LogDetailLevel.Debug, $"Bias Point Random Move: X: {randomPoint.X}, Y: {randomPoint.Y}");
                }
                else
                {
                    randomPoint = GetRandomPointFromRegion(xMult, yMult, ClickableRegion, 200, 150);
                    _logger.Log(LogDetailLevel.Debug, $"Random Move To Point: X: {randomPoint.X}, Y: {randomPoint.Y}");
                }
            }
            else
            {
                randomPoint = GetRandomPointFromRegion(xMult, yMult, ClickableRegion, 200, 150);
                _logger.Log(LogDetailLevel.Debug, $"Random Move To Point: X: {randomPoint.X}, Y: {randomPoint.Y}");
            }

            SetMoveToPoint(randomPoint.X, randomPoint.Y);
            if (CurrentState == ChaosStates.Floor1
                || CurrentState == ChaosStates.Floor2)
            {
                Sleep.SleepMs(200, 250);
                _kb.Press(Key.Space);
                Sleep.SleepMs(200, 250);
                return;
            }

            _mouse.ClickPosition(randomPoint, Random.Shared.Next(minTime, maxTime), MouseButtons.Right);
            //_mouse.ClickPosition(randomPoint, Random.Shared.Next(minTime, maxTime), MouseButtons.Right);
        }

        public Point CalcMinimapPos(int x, int y)
        {
            var minimapCenter = _r.Point("MinimapCenter");
            var screenCenter = _r.Point("CenterScreen");
            var clickableArea = _r.Point("ClickableArea");

            x = x - minimapCenter.X;
            y = y - minimapCenter.Y;

            var pointDistance = Math.Sqrt(x * x + y * y);
            MoveTime = (int)pointDistance * 8;

            var dist = 200;
            int newX, newY;
            if (y < 0)
                dist *= -1;

            if (x == 0)
            {
                newY = y < 0 ? y - Math.Abs(dist) : y + Math.Abs(dist);
                MoveToPoint = new Point(screenCenter.X, newY + screenCenter.Y);
                return MoveToPoint;
            }

            if (y == 0)
            {
                newX = x < 0 ? x - Math.Abs(dist) : x + Math.Abs(dist);
                MoveToPoint = new Point(newX + screenCenter.X, screenCenter.Y);
                return MoveToPoint;
            }

            double k = (double)y / (double)x;
            newY = y + dist;
            newX = (int)((newY - y) / k + x);

            if (newX < 0 && Math.Abs(newX) > clickableArea.X)
            {
                newX = clickableArea.X * -1;
                if (newY < 0)
                    newY = (int)(newY + Math.Abs(dist) * .25);
                else
                    newY = (int)(newY - Math.Abs(dist) * .25);
            }
            else if (newX > 0 && Math.Abs(newX) > clickableArea.X)
            {
                newX = clickableArea.X;
                if (newY < 0)
                    newY = (int)(newY + Math.Abs(dist) * .25);
                else
                    newY = (int)(newY - Math.Abs(dist) * .25);
            }

            if (newY < 0 && Math.Abs(newY) > clickableArea.Y)
            {
                newY = clickableArea.Y * -1;
                if (newX < 0)
                    newX = (int)(newX + Math.Abs(dist) * .7);
                else
                    newX = (int)(newX - Math.Abs(dist) * .7);
            }
            else if (newY > 0 && Math.Abs(newY) > clickableArea.Y)
            {
                newY = clickableArea.Y;
                if (newX < 0)
                    newX = (int)(newX + Math.Abs(dist) * .7);
                else
                    newX = (int)(newX - Math.Abs(dist) * .7);
            }


            MoveToPoint = new Point(newX + screenCenter.X, newY + screenCenter.Y);
            return MoveToPoint;
        }
    }
}