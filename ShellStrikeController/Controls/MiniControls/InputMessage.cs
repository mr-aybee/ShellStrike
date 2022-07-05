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
    public partial class ucInputMessage : UserControl
    {

        public ucInputMessage(string SentText)
        {
            InitializeComponent();
            this.txtBox.Text = SentText;
        }


        public ucInputMessage(CommandLog commandLog)
        {
            InitializeComponent();

            if (commandLog.CommandType == CommandType.SQLCondition)
            {
                this.txtBox.Text = commandLog.Text + "[][]" + commandLog.Response;

            }
            else if (commandLog.CommandType == CommandType.SQLCommand)
            {
                this.txtBox.Text = commandLog.Text + "[][]" + commandLog.Response;
            }
            else if (commandLog.CommandType == CommandType.CommandText)
            {
                this.txtBox.Text = commandLog.Text;
            }
        }

    }
}
