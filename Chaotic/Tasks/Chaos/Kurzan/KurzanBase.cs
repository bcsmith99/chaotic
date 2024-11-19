using Accessibility;
using Chaotic.User;
using Chaotic.Utilities;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chaotic.Tasks.ChaosTasks;

namespace Chaotic.Tasks.Chaos.Kurzan
{
    public abstract class KurzanBase
    {
        protected readonly UserSettings _settings;
        protected readonly MouseUtility _mouse;
        protected readonly KeyboardUtility _kb;
        protected readonly ResourceHelper _rh;
        protected readonly AppLogger _logger;
        public string MapName { get; set; }

        public abstract void StartMapMove();

        protected KurzanBase(string mapName, UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ResourceHelper rh, AppLogger logger)
        {
            _settings = settings;
            _mouse = mouse;
            _kb = kb;
            _rh = rh;
            _logger = logger;
            MapName = mapName; 
        }

        public Rect PreferredMovementArea { get; protected set; }


        public Point PreferredRandomPoint()
        {
            var x = Random.Shared.Next(PreferredMovementArea.Left, PreferredMovementArea.Right);
            var y = Random.Shared.Next(PreferredMovementArea.Top, PreferredMovementArea.Bottom);

            return new Point(x, y);
        }

        public virtual ScreenSearchResult CheckIfStuck()
        {
            return new ScreenSearchResult() { Found = false };
        }

        public virtual void PerformSpecialChecks()
        {

        }

        public static KurzanBase CreateMap(ChaosStates state, UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ResourceHelper rh, AppLogger logger)
        {
            switch (state)
            {
                case ChaosStates.KurzanMap1:
                    return new KurzanMap1(settings, mouse, kb, rh, logger);
                case ChaosStates.KurzanMap2:
                    return new KurzanMap2(settings, mouse, kb, rh, logger);
                case ChaosStates.KurzanMap3:
                    return new KurzanMap3(settings, mouse, kb, rh, logger);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
