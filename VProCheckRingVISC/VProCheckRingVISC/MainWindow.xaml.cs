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
            SSmallSharpness.DataContext = cameraMainJob;
        }

        private void btnSetting_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "btnSettingCameraInitial":
                    wfSettingPanel.Child = cameraMainJob.CogAcqFifoEdit;
                    ChangeSettingsSmallGrid("Camera");
                    break;
                case "btnSettingAlign":
                    wfSettingPanel.Child = cameraMainJob.PmAlignToolEdit;
                    ChangeSettingsSmallGrid("Align");
                    break;
                case "btnSettingFixture":
                    wfSettingPanel.Child = cameraMainJob.CogFixtureTool;
                    ChangeSettingsSmallGrid("Fixture");
                    break;
                case "btnSettingSharpness":
                    wfSettingPanel.Child = cameraMainJob.SharpnessToolEdit;
                    // Testing
                    ChangeSettingsSmallGrid("Sharpness");
                    break;
                case "btnSettingFinish":
                    wfSettingPanel.Child = null;
                    // Testing
                    ChangeSettingsSmallGrid("Finish");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Đổi tab Grid Settings nhỏ theo nút nhấn, lựa chọn từ tên grid truyền vào
        /// </summary>
        /// <param name="v"></param>
        private void ChangeSettingsSmallGrid(string name)
        {
            foreach (var item in SettingsSmallGrid.Children)
            {
                if ((item as Grid).Name.IndexOf(name) > 0) Grid.SetRow(item as Grid, 0);
                else Grid.SetRow(item as Grid, 1);
            }
        }

        /// <summary>
        /// Đổi tab Grid Main nhỏ theo nút nhấn, lựa chọn từ tên grid truyền vào
        /// </summary>
        /// <param name="v"></param>
        private void ChangeMainGrid(string name)
        {
            foreach (var item in MainGrid.Children)
            {
                if ((item as Grid).Name.IndexOf(name) > 0)
                {
                    Grid.SetColumn(item as Grid, 0);
                }
                else
                {
                    Grid.SetColumn(item as Grid, 1);
                }
            }
        }

        private void btnMenuSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch ((sender as MenuItem).Header)
            {
                case "Display":
                    ChangeMainGrid("Display");
                    break;
                case "Settings":
                    ChangeMainGrid("Settings");
                    break;
                default:
                    break;
            }
        }
    }
}
