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
    public partial class ucOutputBox : UserControl
    {
        public string ReceviedText { get; set; }

        public ucOutputBox(string ReceviedText)
        {
            InitializeComponent();
            this.txtBox.Text = ReceviedText;
        }


        public ucOutputBox(CommandLog commandLog)
        {
            InitializeComponent();
        
            if (commandLog.CommandDirection == CommandDirection.OUTPUT)
            {
                if (commandLog.CommandType == CommandType.BreakCase)
                {
                    breakCaseTxt.Text = commandLog.Text;
                    txtBox.Text = commandLog.Response;
                }
                else if (commandLog.CommandType == CommandType.BreakCaseError)
                {
                    txtBox.Text = "[][]No Responose From Server[][]";
                }
            }
        }


    }
}
