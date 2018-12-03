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
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace VProCheckRingVISC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VisionAppObject cameraMainJob = null;
        private ICommand p_F3Command;

        public MainWindow()
        {
            InitializeComponent();
            InitialCameraProgram();
        }

        private void InitialCameraProgram()
        {
            cameraMainJob = new VisionAppObject();
            cameraMainJob.JobDoneNotice += ProcessCameraJobDone;
            SSmallSharpness1.DataContext = cameraMainJob;
            SSmallSharpness2.DataContext = cameraMainJob;
            wfDisplayMain.Child = cameraMainJob.CogDisplayMain;
        }

        private void ProcessCameraJobDone()
        {
            if (cameraMainJob.CheckSharpnessRange()) lblResultMain.Content = "OK";
            else lblResultMain.Content = "NG";
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            // Lưu ảnh
            if (!Directory.Exists(Settings.Default.ImageDBUrl)) Directory.CreateDirectory(Settings.Default.ImageDBUrl);
            string tempUrl = Settings.Default.ImageDBUrl + "\\Image_" + DateTime.Now.ToString("yyMMdd_hhmmss") + ".bmp";
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(tempUrl, FileMode.Create, FileAccess.ReadWrite))
                {
                    cameraMainJob.CogFixtureTool.Subject.OutputImage.ToBitmap().Save(memory, ImageFormat.Bmp);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }

        private void btnSetting_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "btnSettingCameraInitial":
                    wfSettingPanel.Child = cameraMainJob.ImageInputTool as System.Windows.Forms.Control;
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
                case "btnSettingSharpness1":
                    wfSettingPanel.Child = cameraMainJob.SharpnessToolEdit1;
                    // Testing
                    ChangeSettingsSmallGrid("Sharpness1");
                    break;
                case "btnSettingSharpness2":
                    wfSettingPanel.Child = cameraMainJob.SharpnessToolEdit2;
                    // Testing
                    ChangeSettingsSmallGrid("Sharpness2");
                    break;
                case "btnSettingFinish":
                    wfSettingPanel.Child = null;
                    cameraMainJob.SaveCameraJobToUrl();
                    MessageBox.Show("Save Job Done!");
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

        /// <summary>
        /// Lựa chọn mode Input Image từ button Radio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioModeImagebtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as RadioButton).Content.ToString().IndexOf("0") > 0) cameraMainJob.ImageInputMode = 0;
            else cameraMainJob.ImageInputMode = 1;
            wfSettingPanel.Child = cameraMainJob.ImageInputTool as System.Windows.Forms.Control;
        }

        /// <summary>
        /// Nút nhấn chạy Job Camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemRunJob_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cameraMainJob.RunJob();
        }

        /// <summary>
        /// Lưu giá trị cài đặt độ nét 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveSharpnessSettings1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Default.MinSharpness1 = cameraMainJob.MinSharpness1;
            Settings.Default.MaxSharpness1 = cameraMainJob.MaxSharpness1;
            Settings.Default.Save();
        }

        /// <summary>
        /// Lưu giá trị cài đặt độ nét 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveSharpnessSettings2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Default.MinSharpness2 = cameraMainJob.MinSharpness2;
            Settings.Default.MaxSharpness2 = cameraMainJob.MaxSharpness2;
            Settings.Default.Save();
        }

        private void BtnTriggerMain_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            lblResultMain.Content = "";
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            cameraMainJob.RunJob();
        }
    }
}
