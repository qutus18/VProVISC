using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.ImageFile;
using System.IO;
using System;
using System.Windows.Threading;

namespace VProCheckRingVISC
{
    class VisionAppObject : ObservableObject
    {
        // Đường dẫn tool
        private string urlCameraJob = Settings.Default.UrlCameraJob;
        // Khai báo tool Camera
        private CogAcqFifoEditV2 cogAcqFifoEdit;
        public CogAcqFifoEditV2 CogAcqFifoEdit
        {
            get { return cogAcqFifoEdit; }
            set { cogAcqFifoEdit = value; }
        }
        // Khai báo Tool Image File
        private CogImageFileEditV2 cogImageFileTool;
        public CogImageFileEditV2 CogImageFileTool
        {
            get
            {
                return cogImageFileTool;
            }
            set
            {
                cogImageFileTool = value;
            }
        }
        // Image Input Mode : 0 - Camera, 1 - Imagefile
        private int imageInputMode = 0;
        public int ImageInputMode
        {
            get { return imageInputMode; }
            set
            {
                imageInputMode = value;
                OnPropertyChanged("ImageInputMode");
                UpdateBindingInputImage();
            }
        }
        public object ImageInputTool
        {
            get
            {
                switch (imageInputMode)
                {
                    case 0:
                        return cogAcqFifoEdit;
                    case 1:
                        return cogImageFileTool;
                    default:
                        return cogAcqFifoEdit;
                }
            }
            private set { }
        }
        // Khai báo Tool Fixture
        private CogFixtureEditV2 cogFixtureTool;
        public CogFixtureEditV2 CogFixtureTool
        {
            get { return cogFixtureTool; }
            set { cogFixtureTool = value; }
        }
        // Khai báo tool Align
        private CogPMAlignEditV2 pmAlignToolEdit = null;
        public CogPMAlignEditV2 PmAlignToolEdit
        {
            get { return pmAlignToolEdit; }
            set { pmAlignToolEdit = value; }
        }
        // Khai báo tool Sharpness
        private CogImageSharpnessEditV2 sharpnessToolEdit1 = null;
        public CogImageSharpnessEditV2 SharpnessToolEdit1
        {
            get { return sharpnessToolEdit1; }
            set { sharpnessToolEdit1 = value; }
        }
        private CogImageSharpnessEditV2 sharpnessToolEdit2 = null;
        public CogImageSharpnessEditV2 SharpnessToolEdit2
        {
            get { return sharpnessToolEdit2; }
            set { sharpnessToolEdit2 = value; }
        }
        private double minSharpness1;
        public double MinSharpness1
        {
            get { return minSharpness1; }
            set { minSharpness1 = value; OnPropertyChanged("MinSharpness1"); }
        }
        private double maxSharpness1;
        public double MaxSharpness1
        {
            get { return maxSharpness1; }
            set { maxSharpness1 = value; OnPropertyChanged("MaxSharpness1"); }
        }
        private double minSharpness2;
        public double MinSharpness2
        {
            get { return minSharpness2; }
            set { minSharpness2 = value; OnPropertyChanged("MinSharpness2"); }
        }
        private double maxSharpness2;
        public double MaxSharpness2
        {
            get { return maxSharpness2; }
            set { maxSharpness2 = value; OnPropertyChanged("MaxSharpness2"); }
        }
        // Khai báo tool Display
        private CogRecordDisplay cogDisplayMain = null;
        public CogRecordDisplay CogDisplayMain
        {
            get { return cogDisplayMain; }
            set { cogDisplayMain = value; }
        }
        public delegate void JobEventHandle();
        public event JobEventHandle JobDoneNotice;

        public VisionAppObject()
        {
            // Khởi tạo Acq
            cogAcqFifoEdit = new CogAcqFifoEditV2();
            cogAcqFifoEdit.Subject = new CogAcqFifoTool();
            // Khởi tạo Image file Tool
            cogImageFileTool = new CogImageFileEditV2();
            cogImageFileTool.Subject = new CogImageFileTool();
            // Khởi tạo Fixture Tool
            cogFixtureTool = new CogFixtureEditV2();
            cogFixtureTool.Subject = new CogFixtureTool();
            // Khởi tạo PM Align Tool
            pmAlignToolEdit = new CogPMAlignEditV2();
            pmAlignToolEdit.Subject = new CogPMAlignTool();
            // Khởi tạo Sharpness Tool
            sharpnessToolEdit1 = new CogImageSharpnessEditV2();
            sharpnessToolEdit1.Subject = new CogImageSharpnessTool();
            sharpnessToolEdit2 = new CogImageSharpnessEditV2();
            sharpnessToolEdit2.Subject = new CogImageSharpnessTool();
            // Giá trị max, min Sharpness mặc định
            minSharpness1 = Settings.Default.MinSharpness1;
            maxSharpness1 = Settings.Default.MaxSharpness1;
            minSharpness2 = Settings.Default.MinSharpness2;
            maxSharpness2 = Settings.Default.MaxSharpness2;
            // Khởi tạo Cogdisplay
            cogDisplayMain = new CogRecordDisplay();
            // Load Old Job
            LoadCameraJobFromUrl();
            // Xóa Binding cũ
            if (cogFixtureTool.Subject.DataBindings.Contains("InputImage")) cogFixtureTool.Subject.DataBindings.Remove("InputImage");
            if (pmAlignToolEdit.Subject.DataBindings.Contains("InputImage")) pmAlignToolEdit.Subject.DataBindings.Remove("InputImage");
            if (sharpnessToolEdit1.Subject.DataBindings.Contains("InputImage")) sharpnessToolEdit1.Subject.DataBindings.Remove("InputImage");
            if (sharpnessToolEdit2.Subject.DataBindings.Contains("InputImage")) sharpnessToolEdit2.Subject.DataBindings.Remove("InputImage");
            cogDisplayMain.DataBindings.Clear();
            /// Khởi tạo Binding Image
            /// 
            cogFixtureTool.Subject.DataBindings.Add("InputImage", cogAcqFifoEdit.Subject, "OutputImage");
            pmAlignToolEdit.Subject.DataBindings.Add("InputImage", cogAcqFifoEdit.Subject, "OutputImage");
            sharpnessToolEdit1.Subject.DataBindings.Add("InputImage", cogFixtureTool.Subject, "OutputImage");
            sharpnessToolEdit2.Subject.DataBindings.Add("InputImage", cogFixtureTool.Subject, "OutputImage");
            cogDisplayMain.DataBindings.Add("Image", cogFixtureTool.Subject, "OutputImage", true);
            cogDisplayMain.BackColor = System.Drawing.Color.Gray;
        }

        private void LoadCameraJobFromUrl()
        {
            cogAcqFifoEdit.Subject = CogSerializer.LoadObjectFromFile(urlCameraJob + "\\AcqTool.vpp") as CogAcqFifoTool;
            cogImageFileTool.Subject = CogSerializer.LoadObjectFromFile(urlCameraJob + "\\ImageFileTool.vpp") as CogImageFileTool;
            pmAlignToolEdit.Subject = CogSerializer.LoadObjectFromFile(urlCameraJob + "\\AlignTool.vpp") as CogPMAlignTool;
            cogFixtureTool.Subject = CogSerializer.LoadObjectFromFile(urlCameraJob + "\\FixtureTool.vpp") as CogFixtureTool;
            sharpnessToolEdit1.Subject = CogSerializer.LoadObjectFromFile(urlCameraJob + "\\SharpnessTool1.vpp") as CogImageSharpnessTool;
            sharpnessToolEdit2.Subject = CogSerializer.LoadObjectFromFile(urlCameraJob + "\\SharpnessTool2.vpp") as CogImageSharpnessTool;
        }

        public void SaveCameraJobToUrl()
        {
            CogSerializer.SaveObjectToFile(cogAcqFifoEdit.Subject, urlCameraJob + "\\AcqTool.vpp");
            CogSerializer.SaveObjectToFile(cogImageFileTool.Subject, urlCameraJob + "\\ImageFileTool.vpp");
            CogSerializer.SaveObjectToFile(pmAlignToolEdit.Subject, urlCameraJob + "\\AlignTool.vpp");
            CogSerializer.SaveObjectToFile(cogFixtureTool.Subject, urlCameraJob + "\\FixtureTool.vpp");
            CogSerializer.SaveObjectToFile(sharpnessToolEdit1.Subject, urlCameraJob + "\\SharpnessTool1.vpp");
            CogSerializer.SaveObjectToFile(sharpnessToolEdit2.Subject, urlCameraJob + "\\SharpnessTool2.vpp");
            SaveSharpnessSettings();
        }

        /// <summary>
        /// Lưu giá trị cài đặt ngưỡng độ nét
        /// </summary>
        private void SaveSharpnessSettings()
        {
            // Giá trị max, min Sharpness mặc định
            Settings.Default.MinSharpness1 = minSharpness1;
            Settings.Default.MaxSharpness1 = maxSharpness1;
            Settings.Default.MinSharpness2 = minSharpness2;
            Settings.Default.MaxSharpness2 = maxSharpness2;
            Settings.Default.Save();
        }

        /// <summary>
        /// Cập nhật nguồn ảnh cho tool Align khi Mode Input Image thay đổi
        /// </summary>
        private void UpdateBindingInputImage()
        {
            pmAlignToolEdit.Subject.DataBindings.Remove("InputImage");
            cogFixtureTool.Subject.DataBindings.Remove("InputImage");
            switch (imageInputMode)
            {
                case 0:
                    pmAlignToolEdit.Subject.DataBindings.Add("InputImage", (ImageInputTool as CogAcqFifoEditV2).Subject, "OutputImage");
                    cogFixtureTool.Subject.DataBindings.Add("InputImage", (ImageInputTool as CogAcqFifoEditV2).Subject, "OutputImage");
                    break;
                case 1:
                    pmAlignToolEdit.Subject.DataBindings.Add("InputImage", (ImageInputTool as CogImageFileEditV2).Subject, "OutputImage");
                    cogFixtureTool.Subject.DataBindings.Add("InputImage", (ImageInputTool as CogImageFileEditV2).Subject, "OutputImage");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Chạy lần lượt các tool
        /// </summary>
        public bool RunJob()
        {
            cogAcqFifoEdit.Subject.Run();
            cogImageFileTool.Subject.Run();
            // PM Align
            pmAlignToolEdit.Subject.Run();
            if (pmAlignToolEdit.Subject.Results?.Count > 0)
            {
                cogFixtureTool.Subject.RunParams.UnfixturedFromFixturedTransform = pmAlignToolEdit.Subject.Results[0].GetPose();
            }
            else return false;
            cogFixtureTool.Subject.Run();
            sharpnessToolEdit1.Subject.Run();
            sharpnessToolEdit2.Subject.Run();
            cogDisplayMain.Fit();
            JobDoneNotice?.Invoke();
            return true;
        }

        /// <summary>
        /// Kiểm tra kết quả Tool Sharpness có trong khoảng cho phép không, nếu không trả về false
        /// </summary>
        /// <returns></returns>
        public bool CheckSharpnessRange()
        {
            if (sharpnessToolEdit1.Subject.Score > 0)
            {
                double temp = sharpnessToolEdit1.Subject.Score;
                if (!((temp > minSharpness1) && (temp < maxSharpness1))) return false;
            }
            if (sharpnessToolEdit2.Subject.Score > 0)
            {
                double temp = sharpnessToolEdit2.Subject.Score;
                if ((temp > minSharpness2) && (temp < maxSharpness2)) return true;
                else return false;
            }
            return false;
        }
    }
}
