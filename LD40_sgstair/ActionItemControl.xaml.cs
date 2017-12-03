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

namespace LD40_sgstair
{
    /// <summary>
    /// Interaction logic for ActionItemControl.xaml
    /// </summary>
    public partial class ActionItemControl : UserControl
    {
        public ActionItemControl()
        {
            InitializeComponent();
            MouseLeftButtonDown += ActionItemControl_MouseLeftButtonDown;
            MouseLeftButtonUp += ActionItemControl_MouseLeftButtonUp;
            MouseLeave += ActionItemControl_MouseLeave;
        }
        Brush defaultBrush = new SolidColorBrush(Color.FromArgb(0x99, 255, 255, 255));
        Brush cancelBrush = new SolidColorBrush(Color.FromArgb(0x99, 255, 208, 218));
        Brush clickBrush = new SolidColorBrush(Color.FromArgb(0xDD, 230, 255, 230));

        Brush baseBrush;

        internal RoundAction Action;
        internal void BindAction(RoundAction possibleAction)
        {
            Action = possibleAction;

            LabelHeader.Content = Action.Action.Title;
            LabelDescription.Content = Action.Action.Description;

            LabelCost.Content = Action.CostString == null ? "" : "Cost: " + Action.CostString;
            LabelRisk.Content = Action.RiskString == null ? "" : "Risk: " + Action.RiskString;
            LabelBenefit.Content = Action.BenefitString == null ? "" : "Benefit: " + Action.BenefitString;

            baseBrush = Action.Action.CancelTaskFor == null ? defaultBrush : cancelBrush;
            UpdateBackground();
        }

        internal event Action<RoundAction> ActionClicked;


        private void ActionItemControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(clicking)
            {
                ActionClicked?.Invoke(Action);
                // Clicked.
                clicking = false;
                UpdateBackground();
            }
        }

        bool clicking = false;

        private void ActionItemControl_MouseLeave(object sender, MouseEventArgs e)
        {
            clicking = false;
            UpdateBackground();
        }

        private void ActionItemControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clicking = true;
            UpdateBackground();
        }

        void UpdateBackground()
        {
            RectBackground.Fill = clicking ? clickBrush : baseBrush;
        }
    }
}
