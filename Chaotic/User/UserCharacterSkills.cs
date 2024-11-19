using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Chaotic.User
{
    public class UserCharacterSkills
    {
        public UserCharacterSkills()
        {
            QSkill = new UserCharacterSkill() { SkillKey = "Q", SkillType = "Normal" };
            WSkill = new UserCharacterSkill() { SkillKey = "W", SkillType = "Normal" };
            ESkill = new UserCharacterSkill() { SkillKey = "E", SkillType = "Normal" };
            RSkill = new UserCharacterSkill() { SkillKey = "R", SkillType = "Normal" };
            ASkill = new UserCharacterSkill() { SkillKey = "A", SkillType = "Normal" };
            SSkill = new UserCharacterSkill() { SkillKey = "S", SkillType = "Normal" };
            DSkill = new UserCharacterSkill() { SkillKey = "D", SkillType = "Normal" };
            FSkill = new UserCharacterSkill() { SkillKey = "F", SkillType = "Normal" };
            HyperSkill = new UserCharacterSkill() { SkillKey = "T", SkillType = "Normal" };
            Awakening = new UserCharacterSkill() { SkillKey = "V", SkillType = "Normal", IsAwakening = true };
        }

        public UserCharacterSkill QSkill { get; set; }
        public UserCharacterSkill WSkill { get; set; }
        public UserCharacterSkill ESkill { get; set; }
        public UserCharacterSkill RSkill { get; set; }
        public UserCharacterSkill ASkill { get; set; }
        public UserCharacterSkill SSkill { get; set; }
        public UserCharacterSkill DSkill { get; set; }
        public UserCharacterSkill FSkill { get; set; }
        public UserCharacterSkill Awakening { get; set; }
        public UserCharacterSkill HyperSkill { get; set; }

        [JsonIgnore]
        public List<UserCharacterSkill> AllSkills
        {
            get
            {
                return new List<UserCharacterSkill>()
                {
                    QSkill,
                    WSkill,
                    ESkill,
                    RSkill,
                    ASkill,
                    SSkill,
                    DSkill,
                    FSkill,
                    HyperSkill,
                    Awakening,
                };
            }
        }
    }

    public class UserCharacterSkill : INotifyPropertyChanged
    {
        public UserCharacterSkill()
        {
            SkillKey = "";
            SkillType = "Cast";
            Duration = 500;
            IsDirectional = true;
            IsShortCast = true;
            Priority = 1;
            SkillImageEncoded = String.Empty;
            IsAwakening = false;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public string SkillKey { get; set; }
        public string SkillType { get; set; }
        public int Duration { get; set; }
        public bool IsDirectional { get; set; }
        public bool IsShortCast { get; set; }
        public int Priority { get; set; }
        public bool IsAwakening { get; set; }

        [JsonIgnore]
        private string _skillImageEncoded;
        public string SkillImageEncoded
        {
            get { return _skillImageEncoded; }
            set
            {
                _skillImageEncoded = value;
                OnPropertyChanged("SkillImage");
            }
        }

        [JsonIgnore]
        public BitmapImage? SkillImage
        {
            get
            {
                if (String.IsNullOrWhiteSpace(SkillImageEncoded))
                    return null;
                else
                {
                    var byteArray = Convert.FromBase64String(SkillImageEncoded);

                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = new MemoryStream(byteArray);
                    bi.EndInit();

                    return bi;
                }
            }
        }
    }
}