using Chaotic.Resources;
using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chaotic.Tasks.Una
{
    [UnaTask(UnaTaskNames.VoldisShard)]
    public class VoldisShard : UnaTask
    {
        public VoldisShard(UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, UserSettings settings, AppLogger logger)
            : base(uiTask, mouse, kb, r, settings, logger)
        {

        }

        public override void ExecuteTask()
        {
            _kb.Press(Key.G, 5000);
            _kb.Press(Key.G, 5000);
            _kb.Press(Key.G, 7000);

            //Press both F5 and F6 in case an adventure quest is hijacking the key.
            _kb.Press(Key.F5, 1000);
            _kb.Press(Key.F6, 1000);

            _uiTask.ClearOngoingQuests();
        }
    }
}
