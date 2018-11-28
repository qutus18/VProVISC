using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using wform = System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VProCheckRingVISC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VisionAppObject cameraMainJob = null;
        public MainWindow()
        {
            InitializeComponent();
            InitialCameraProgram();
        }

        private void InitialCameraProgram()
        {
            cameraMainJob = new VisionAppObject();
        }

        private void btnSetting_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "btnSettingCameraInitial":
                    wfSettingPanel.Child = cameraMainJob.CogAcqFifoEdit;
                    break;
                case "btnSettingAlign":
                    wfSettingPanel.Child = cameraMainJob.PmAlignToolEdit;
                    break;
                case "btnSettingFixture":
                    wfSettingPanel.Child = cameraMainJob.CogFixtureTool;
                    break;
                case "btnSettingSharpness":
                    wfSettingPanel.Child = cameraMainJob.SharpnessToolEdit;
                    break;
                case "btnSettingFinish":
                    wfSettingPanel.Child = null;
                    break;
                default:
                    break;
            }
        }
    }
}
