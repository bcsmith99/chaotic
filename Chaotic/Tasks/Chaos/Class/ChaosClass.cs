using Accessibility;
using Chaotic.User;
using Chaotic.Utilities;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using OpenCvSharp;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;
using System.Diagnostics;

namespace Chaotic.Tasks.Chaos.Class
{
    public class ChaosClass
    {
        protected readonly UserCharacter _char;
        protected readonly ResourceHelper _r;
        protected readonly KeyboardUtility _kb;
        protected readonly KeyConverter _kc;
        protected readonly MouseUtility _mouse;
        protected readonly UserSettings _settings;
        protected readonly AppLogger _logger;

        public ChaosClass(UserSettings settings, UserCharacter character, ResourceHelper r, KeyboardUtility kb, MouseUtility mouse, AppLogger logger)
        {
            _char = character;
            _r = r;
            _kb = kb;
            _kc = new KeyConverter();
            _mouse = mouse;
            _settings = settings;
            _logger = logger;

            CharacterIconRegion = IP.ConvertStringCoordsToRect(_r["CharacterIcon_Region"]);
            ScreenCenter = IP.GetPointFromStringCoords(_r["CenterScreen"]);
        }

        public double SkillConfidence { get; protected set; } = .95;

        protected Rect CharacterIconRegion { get; private set; }
        public Point ScreenCenter { get; private set; }

        public void UseAwakening(Point screenPoint)
        {
            var awakening = _char.Skills.Awakening;
            var skillFound = CheckSkillAvailable(awakening, .8);

            if (awakening.Priority > 0 && skillFound.Found)
            {
                CastSkill(awakening, screenPoint);
            }

        }

        public virtual void StartUp()
        {

        }

        public virtual void UseCharacterSpecificAbilities()
        {

        }

        private Point GetShortCastPos(Point point, Point center)
        {
            var yModifier = center.Y > point.Y ? -1 : 1;
            var xModifier = center.X > point.X ? -1 : 1;


            var xCoord = (int)(center.X + (xModifier * Math.Abs(center.X - point.X) * .33));
            var yCoord = (int)(center.Y + (yModifier * Math.Abs(center.Y - point.Y) * .33));
            return new Point(xCoord, yCoord);
        }

        public void UseAbilities(Point screenPoint, int maxCasts = 0)
        {
            _logger.Log(LogDetailLevel.Debug, $"Abilities cycle - {maxCasts} casts");
            StartUp();

            UseCharacterSpecificAbilities();

            int currentCasts = 0;
            var abilities = _char.Skills.AllSkills.Where(x => !x.IsAwakening && x.Priority > 0).OrderBy(x => x.Priority);

            foreach (var ability in abilities)
            {
                if (maxCasts > 0 && currentCasts >= maxCasts)
                    break;

                if (ability.SkillKey == "T" && _char.ChaosLevel < 1640)
                    continue;

                var skillAvailable = CheckSkillAvailable(ability);
                if (skillAvailable.Found)
                {
                    CastSkill(ability, screenPoint);
                    currentCasts++;
                }

            }
        }

        public ScreenSearchResult CheckSkillAvailable(UserCharacterSkill skill)
        {
            return CheckSkillAvailable(skill, SkillConfidence);
        }

        public ScreenSearchResult CheckSkillAvailable(UserCharacterSkill skill, double confidence)
        {
            using (var ms = new MemoryStream(Convert.FromBase64String(skill.SkillImageEncoded)))
            {
                var skillCoords = _r[$"Skill_{skill.SkillKey}"];
                if (skill.IsAwakening && _char.HasHyperSkill)
                    skillCoords = _r[$"Skill_Hyper{skill.SkillKey}"];

                var skillFound = IP.LocateCenterOnScreen(ms, IP.ConvertStringCoordsToRect(skillCoords), confidence);
                return skillFound;
            }
        }
        private void CastSkill(UserCharacterSkill skill, Point screenPoint)
        {
            if (skill.IsDirectional)
            {
                var newPoint = screenPoint;
                if (skill.IsShortCast)
                {
                    newPoint = GetShortCastPos(screenPoint, ScreenCenter);
                }
                _mouse.SetPosition(newPoint.X, newPoint.Y);
            }
            else
                _mouse.SetPosition(ScreenCenter.X, ScreenCenter.Y);


            if (skill.SkillType == "Normal")
                _kb.Press((Key)_kc.ConvertFromString(skill.SkillKey), 300);
            else if (skill.SkillType == "Hold")
            {
                _logger.Log(LogDetailLevel.Debug, "Attempting to hold a key");
                _kb.Hold((Key)_kc.ConvertFromString(skill.SkillKey), skill.Duration);
            }

            else if (skill.SkillType == "Cast")
            {
                int clickDelay = 100;
                int numTimes = skill.Duration / clickDelay;
                for (int i = 0; i < numTimes; i++)
                {
                    _kb.Press((Key)_kc.ConvertFromString(skill.SkillKey), clickDelay);
                }
            }
        }


        public static ChaosClass Create(UserSettings settings, UserCharacter character, ResourceHelper r, KeyboardUtility kb, MouseUtility mouse, AppLogger logger)
        {
            switch (character.ClassName)
            {
                case ClassNames.Berserker:
                    return new Berserker(settings, character, r, kb, mouse, logger);
                case ClassNames.Paladin:
                    return new Paladin(settings, character, r, kb, mouse, logger);
                case ClassNames.Sharpshooter:
                    return new Sharpshooter(settings, character, r, kb, mouse, logger);
                case ClassNames.Artist:
                    return new Artist(settings, character, r, kb, mouse, logger);
                case ClassNames.Aeromancer:
                    return new Aeromancer(settings, character, r, kb, mouse, logger);
                case ClassNames.Arcanist:
                    return new Arcanist(settings, character, r, kb, mouse, logger);
                case ClassNames.Scrapper:
                    return new Scrapper(settings, character, r, kb, mouse, logger);
                case ClassNames.Souleater:
                    return new Souleater(settings, character, r, kb, mouse, logger);
                case ClassNames.Deathblade:
                    return new Deathblade(settings, character, r, kb, mouse, logger);
                case ClassNames.Breaker:
                    return new Breaker(settings, character, r, kb, mouse, logger);
                case ClassNames.Summoner:
                    return new Summoner(settings, character, r, kb, mouse, logger);
                case ClassNames.Gunlancer:
                    return new Gunlancer(settings, character, r, kb, mouse, logger);
                case ClassNames.Slayer:
                    return new Slayer(settings, character, r, kb, mouse, logger);
                case ClassNames.Destroyer:
                    return new Destroyer(settings, character, r, kb, mouse, logger);
                case ClassNames.Unset:
                default:
                    return new ChaosClass(settings, character, r, kb, mouse, logger);
            }
        }
    }
}
