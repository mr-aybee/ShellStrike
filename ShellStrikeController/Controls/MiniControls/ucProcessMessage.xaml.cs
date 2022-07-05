using ShellStrike;
using ShellStrike.Card;
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

namespace ShellStrikeController.Controls.MiniControls
{
    /// <summary>
    /// Interaction logic for SendMessageBox.xaml
    /// </summary>
    public partial class ucProcessBox : UserControl
    {
        //public string MiddleText { get; set; }

        public ucProcessBox(string MiddleText)
        {
            InitializeComponent();
            txtBox.Text = MiddleText;
            //this.Height = ScalarFunctions.GetTextBoxSizeByTextEnters(MiddleText);
        }

        public ucProcessBox(CommandLog commandLog)
        {
            InitializeComponent();
            txtBox.Text = commandLog.Text;
        }
    }
}
