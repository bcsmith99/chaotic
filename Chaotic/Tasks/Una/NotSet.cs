﻿using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Tasks.Una
{
    [UnaTask(UnaTaskNames.NotSet)]
    public class NotSet : UnaTask
    {
        public NotSet(UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ResourceHelper r, UserSettings settings, AppLogger logger)
            : base(uiTask, mouse, kb, r, settings, logger)
        {

        }

        protected override void ExecuteTask()
        {
            throw new NotImplementedException();
        }
    }
}