using Chaotic.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chaotic
{
    /// <summary>
    /// Interaction logic for SkillControl.xaml
    /// </summary>
    public partial class SkillControl : UserControl
    {



        //public UserCharacter Character
        //{
        //    get { return (UserCharacter)GetValue(CharacterProperty); }
        //    set { SetValue(CharacterProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Character.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CharacterProperty =
        //    DependencyProperty.Register("Character", typeof(UserCharacter), typeof(SkillControl), new PropertyMetadata(new UserCharacter()));



        public UserCharacterSkill Skill
        {
            get { return (UserCharacterSkill)GetValue(SkillProperty); }
            set { SetValue(SkillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkillProperty =
            DependencyProperty.Register("Skill", typeof(UserCharacterSkill), typeof(SkillControl), new PropertyMetadata(new UserCharacterSkill()));


        public SkillControl()
        {
            InitializeComponent();
        }
    }
}
