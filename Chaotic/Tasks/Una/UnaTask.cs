using Accessibility;
using Chaotic.Resources;
using Chaotic.User;
using Chaotic.Utilities;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Tasks.Una
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UnaTaskAttribute : Attribute
    {
        public string Name;
        public UnaTaskAttribute(string name)
        {
            Name = name;
        }
    }

    public class UnaTaskNames
    {
        public const string NotSet = "Not Set";
        public const string ElgaciaLeap = "Elgacia-Leap";
        public const string ElgaciaShard = "Elgacia-Shard";
        public const string GhostShipLeap = "GhostShip-Leap";
        public const string MokoMarketLeap = "MokoMarket-Leap";
        public const string VoldisLeap = "Voldis-Leap";
        public const string VoldisShard = "Voldis-Shard";
        public const string SKurzanLeap = "SKurzan-Leap";
        public const string NKurzanShard = "NKurzan-Shard";
        public const string PlecciaShard = "Pleccia-Shard";
        public const string ElneadSolar = "Elnead-Solars";
        public const string PlecciaSolar = "Pleccia-Solars";
        public const string PraeteriaSolar = "Praeteria-Solars";
        public const string Lopang = "Lopang";
    }

    public abstract class UnaTask
    {
        protected readonly UITasks _uiTask;
        protected readonly MouseUtility _mouse;
        protected readonly KeyboardUtility _kb;
        protected readonly ApplicationResources _r;
        protected readonly UserSettings _settings;
        protected readonly AppLogger _logger;

        protected Point CenterScreen { get; }

        protected UnaTask(UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, UserSettings settings, AppLogger logger)
        {
            _uiTask = uiTask;
            _mouse = mouse;
            _kb = kb;
            _r = r;
            _settings = settings;
            _logger = logger;

            CenterScreen = _r.CenterScreen;
        }

        public static UnaTask Create(string unaTaskName, UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, UserSettings settings, AppLogger logger)
        {
            UnaTask task;
            switch (unaTaskName)
            {
                case UnaTaskNames.PraeteriaSolar:
                    task = new PraeteriaSolar(uiTask, mouse, kb, r, settings, logger);
                    break;
                case UnaTaskNames.PlecciaSolar:
                    task = new PlecciaSolar(uiTask, mouse, kb, r, settings, logger);
                    break;
                case UnaTaskNames.ElneadSolar:
                    task = new ElneadSolar(uiTask, mouse, kb, r, settings, logger);
                    break;
                case UnaTaskNames.PlecciaShard:
                    task = new PlecciaShard(uiTask, mouse, kb, r, settings, logger);
                    break;
                case UnaTaskNames.ElgaciaShard:
                    task = new ElgaciaShard(uiTask, mouse, kb, r, settings, logger);
                    break;
                case UnaTaskNames.VoldisShard:
                    task = new VoldisShard(uiTask, mouse, kb, r, settings, logger);
                    break;
                case UnaTaskNames.VoldisLeap:
                    task = new VoldisLeap(uiTask, mouse, kb, r, settings, logger);
                    break;
                case UnaTaskNames.SKurzanLeap:
                    task = new SKurzanLeap(uiTask, mouse, kb, r, settings, logger);
                    break;
                case UnaTaskNames.GhostShipLeap:
                    task = new GhostShipLeap(uiTask, mouse, kb, r, settings, logger);
                    break;
                case UnaTaskNames.Lopang:
                    task = new Lopang(uiTask, mouse, kb, r, settings, logger);
                    break;
                default:
                    task = new NotSet(uiTask, mouse, kb, r, settings, logger);
                    break;
            }
            return task;
        }
        public void RunUna(int bifrost)
        {
            if (_uiTask.BifrostToPoint(bifrost))
            {
                _mouse.ClickCenterScreen(CenterScreen);
                ExecuteTask();
            }
        }

        public abstract void ExecuteTask();


    }
}
