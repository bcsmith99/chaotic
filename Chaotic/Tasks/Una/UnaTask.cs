using Accessibility;
using Chaotic.User;
using Chaotic.Utilities;
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
        public const string Lopang = "Lopang";
    }

    public abstract class UnaTask
    {
        protected readonly UITasks _uiTask;
        protected readonly MouseUtility _mouse;
        protected readonly KeyboardUtility _kb;
        protected readonly ResourceHelper _r;
        protected readonly UserSettings _settings;
        protected readonly AppLogger _logger; 

        protected UnaTask(UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ResourceHelper r, UserSettings settings, AppLogger logger)
        {
            _uiTask = uiTask;
            _mouse = mouse;
            _kb = kb;
            _r = r;
            _settings = settings;
            _logger = logger;
        }

        public static UnaTask Create(string unaTaskName, UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ResourceHelper r, UserSettings settings, AppLogger logger)
        {
            UnaTask task;
            switch (unaTaskName)
            {
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
            if (BifrostToPoint(bifrost))
            {
                _mouse.ClickCenterScreen(_r);
                ExecuteTask();
            }
        }

        protected abstract void ExecuteTask();

        protected bool BifrostToPoint(int bifrost)
        {
            _mouse.ClickPosition(_r["AdventureMenu"], 1000);
            _mouse.ClickPosition(_r["BifrostMenu"], 1500);
            _mouse.ClickPosition(_r[$"Bifrost{bifrost}"], 1000);

            var bifrostOkRegion = ImageProcessing.ConvertStringCoordsToRect(_r["BifrostOk_Region"]);
            var okButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), bifrostOkRegion,  .95);
            if (okButton.Found)
            {
                _mouse.ClickPosition(okButton.CenterX, okButton.CenterY, 5000);
                return _uiTask.InAreaCheck();
            }

            return false;
        }
    }
}
