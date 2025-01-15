using Accessibility;
using Chaotic.Extensions;
using Chaotic.Resources;
using Chaotic.Tasks.Chaos.Class;
using Chaotic.User;
using Chaotic.Utilities;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chaotic.Tasks.ChaosTasks;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic.Tasks.Chaos.Kurzan
{
    public abstract class KurzanBase
    {
        protected readonly UserSettings _settings;
        protected readonly MouseUtility _mouse;
        protected readonly KeyboardUtility _kb;
        protected readonly ApplicationResources _r;
        protected readonly AppLogger _logger;
        public string MapName { get; set; }
        protected Point CenterScreen { get; }
        public Rect MinimapRegion { get; }
        public Rect TopMinimapRegion { get; }

        public abstract void StartMapMove(ChaosClass cc);

        protected KurzanBase(string mapName, UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, AppLogger logger)
        {
            _settings = settings;
            _mouse = mouse;
            _kb = kb;
            _r = r;
            _logger = logger;
            MapName = mapName;
            CenterScreen = _r.CenterScreen;// _r.Point("CenterScreen");
            MinimapRegion = _r.Minimap; // IP.ConvertStringCoordsToRect(_r["MinimapRegion"]);
            TopMinimapRegion = MinimapRegion.TopRegion();
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

        public virtual void PerformSpecialChecks(DateTime startTime)
        {

        }

        public virtual ScreenSearchResult CheckMapRoute(DateTime startTime)
        {
            return new ScreenSearchResult() { Found = false };
        }

        public static KurzanBase CreateMap(ChaosStates state, UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ApplicationResources rh, AppLogger logger)
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
