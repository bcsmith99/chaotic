using Accessibility;
using Chaotic.User;
using Chaotic.Utilities;
using DeftSharp.Windows.Input.Keyboard;
using DeftSharp.Windows.Input.Mouse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic.Tasks
{
    public class GuildTasks
    {
        private UserSettings _settings;
        private readonly MouseUtility _mouse;
        private readonly KeyboardUtility _kb;
        private readonly ResourceHelper _r;
        private readonly AppLogger _logger;
        private readonly ManualResetEvent _busy;

        public GuildTasks(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ResourceHelper r,AppLogger logger, ManualResetEvent busy)
        {
            _settings = settings;
            _mouse = mouse;
            _kb = kb;
            _r = r;
            _logger = logger;
            _busy = busy; 
        }
        public bool PerformGuildTasks(UserCharacter character)
        {
            var retVal = true;
            if (!character.GuildDonation)
                return retVal;

            Thread.Sleep(2000);
            _mouse.ClickPosition(_r["CommunityMenu"], 500);
            _mouse.ClickPosition(_r["GuildMenu"], 3200);

            //1630, 710, 200, 60,
            var ok = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("ok_button.png", _settings.Resolution), confidence: .95);

            if (ok.Found)
            {
                _logger.Log(LogDetailLevel.Debug, $"X: {ok.CenterX}, Y: {ok.CenterY}");
                _mouse.ClickPosition(ok.CenterX, ok.CenterY, 1000);
            }

            if (character.GuildDonationGold || character.GuildDonationSilver)
            {
                //2500, 370, 200, 60,
                var donateButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("donate_button.png", _settings.Resolution), confidence: .95);

                if (donateButton != null)
                {
                    _logger.Log(LogDetailLevel.Debug, $"Donate - X: {donateButton.CenterX}, Y: {donateButton.CenterY}");
                    _mouse.ClickPosition(donateButton.CenterX, donateButton.CenterY, 1500);
                    if (character.GuildDonationSilver)
                    {
                        // 1250, 680, 250, 100,
                        var silverDono = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("donate_silver.png", _settings.Resolution), IP.ConvertStringCoordsToRect(_r["SilverDonate_Region"]), confidence: .95);
                        if (silverDono != null)
                        {
                            _logger.Log(LogDetailLevel.Debug, $"Silver Donate - X: {silverDono.CenterX}, Y: {silverDono.CenterY}");

                            _mouse.ClickPosition(silverDono.CenterX, silverDono.CenterY, 1000);
                        }
                    }
                    if (character.GuildDonationGold)
                    {
                        //1600, 680, 250, 100,
                        var goldDono = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("donate_silver.png", _settings.Resolution), IP.ConvertStringCoordsToRect(_r["GoldDonate_Region"]), confidence: .95);
                        if (goldDono != null)
                        {
                            _logger.Log(LogDetailLevel.Debug, $"Gold Donate - X: {goldDono.CenterX}, Y: {goldDono.CenterY}");
                            _mouse.ClickPosition(goldDono.CenterX, goldDono.CenterY, 1000);
                            //Thread.Sleep(2000);
                        }
                    }

                    _kb.Press(Key.Escape, 500);
                }
            }

            var supportButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("support_button.png", _settings.Resolution),
                IP.ConvertStringCoordsToRect(_r["GuildSupport_Region"]), .85);
            if (supportButton.Found)
            {
                _mouse.ClickPosition(supportButton.CenterX, supportButton.CenterY, 1000);
                _mouse.ClickPosition(_r["GuildNormalSupport"], 500);
                _mouse.ClickPosition(_r["GuildSupportOk_Button"], 1000);

                var cancelButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("cancel_button.png", _settings.Resolution),
                    IP.ConvertStringCoordsToRect(_r["GuildSupportCancel_Region"]), .7);
                if (cancelButton.Found)
                    _mouse.ClickPosition(cancelButton.CenterX, cancelButton.CenterY, 500);
            }

            _kb.Press(Key.Escape, 1000);

            return retVal;
        }
    }
}
