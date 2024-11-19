using DeftSharp.Windows.Input.Keyboard;
using DeftSharp.Windows.Input.Mouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chaotic.Utilities
{
    public class KeyboardUtility
    {
        private readonly KeyboardManipulator _kb;
        private readonly KeyboardListener _kl;
        private readonly KeyConverter _kc;

        public KeyboardUtility()
        {
            this._kb = new KeyboardManipulator();
            this._kl = new KeyboardListener();
            this._kc = new KeyConverter();
        }

        public void Listen(Key key, Action action)
        {
            _kl.Subscribe(key, action);
        }

        public void StopListening(Key key)
        {
            _kl.Unsubscribe(key);
        }

        public void ComboListen(IEnumerable<Key> keys, Action action)
        {
            _kl.SubscribeCombination(keys, action);
        }

        public void Hold(Key key)
        {
            _kb.Simulate(key, KeyboardSimulateOption.KeyDown);
            Sleep.SleepMs(10, 30);
        }

        public void Hold(Key key, int duration = 0)
        {
            Hold(key);
            if (duration > 0)
            {
                Sleep.SleepMs(duration, duration + 10);
                Release(key);
            }
        }

        public void Release(Key key)
        {
            _kb.Simulate(key, KeyboardSimulateOption.KeyUp);
            Sleep.SleepMs(10, 30);
        }

        private void ModifierPress(Key modifier, Key key, int waitAfter = 0)
        {
            Hold(modifier);
            Press(key, waitAfter);
            Release(modifier);
        }

        public void AltPress(Key key, int waitAfter = 0)
        {
            ModifierPress(Key.LeftAlt, key, waitAfter);
        }

        public void ShiftPress(Key key, int waitAfter = 0)
        {
            ModifierPress(Key.LeftShift, key, waitAfter);
        }

        public void ControlPress(Key key, int waitAfter = 0)
        {
            ModifierPress(Key.LeftCtrl, key, waitAfter);
        }


        public void Press(string key, int waitAfter = 0)
        {
            var keyToPress = (Key)_kc.ConvertFromString(key);
            Press(keyToPress, waitAfter);
        }

        public void Press(Key key, int waitAfter = 0)
        {
            _kb.Press(key);
            if (waitAfter > 0)
                Sleep.SleepMs(waitAfter, waitAfter + 50);
        }

        public void Press(params Key[] keys)
        {
            _kb.Press(keys);
        }

        public void TypeString(string val)
        {
            foreach (var key in val)
            {
                Key current = (Key)_kc.ConvertFromString(key.ToString());
                _kb.Press(current);
                Sleep.SleepMs(20, 50);
            }
        }
    }
}
