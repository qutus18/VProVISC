using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.CalibFix;
using System.IO;

namespace VProCheckRingVISC
{
    class VisionAppObject
    {
        // Khai báo tool Camera
        private CogAcqFifoEditV2 cogAcqFifoEdit;
        public CogAcqFifoEditV2 CogAcqFifoEdit
        {
            get { return cogAcqFifoEdit; }
            set { cogAcqFifoEdit = value; }
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
        private CogImageSharpnessEditV2 sharpnessToolEdit = null;
        public CogImageSharpnessEditV2 SharpnessToolEdit
        {
            get { return sharpnessToolEdit; }
            set { sharpnessToolEdit = value; }
        }
        private double minSharpness;
        public double MinSharpness
        {
            get { return minSharpness; }
            set { minSharpness = value; }
        }
        private double maxSharpness;
        public double MaxSharpness
        {
            get { return maxSharpness; }
            set { maxSharpness = value; }
        }
        // Khai báo tool Display
        private CogDisplay cogDisplayMain = null;
        public CogDisplay CogDisplayMain
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
            // Khởi tạo Fixture Tool
            cogFixtureTool = new CogFixtureEditV2();
            cogFixtureTool.Subject = new CogFixtureTool();
            // Khởi tạo PM Align Tool
            pmAlignToolEdit = new CogPMAlignEditV2();
            pmAlignToolEdit.Subject = new CogPMAlignTool();
            // Khởi tạo Sharpness Tool
            sharpnessToolEdit = new CogImageSharpnessEditV2();
            sharpnessToolEdit.Subject = new CogImageSharpnessTool();
            // Giá trị max, min Sharpness mặc định
            minSharpness = 0;
            maxSharpness = 0;
            // Khởi tạo Cogdisplay
            cogDisplayMain = new CogDisplay();
            /// Khởi tạo Binding Image
            /// 
            cogFixtureTool.Subject.DataBindings.Add("InputImage", cogAcqFifoEdit.Subject, "OutputImage");
            pmAlignToolEdit.Subject.DataBindings.Add("InputImage", cogAcqFifoEdit.Subject, "OutputImage");
            sharpnessToolEdit.Subject.DataBindings.Add("InputImage", cogFixtureTool.Subject, "OutputImage");
        }

        /// <summary>
        /// Chạy lần lượt các tool
        /// </summary>
        public void RunJob()
        {
            cogAcqFifoEdit.Subject.Run();
            cogFixtureTool.Subject.Run();
            pmAlignToolEdit.Subject.Run();
            sharpnessToolEdit.Subject.Run();
            JobDoneNotice?.Invoke();
        }

        /// <summary>
        /// Kiểm tra kết quả Tool Sharpness có trong khoảng cho phép không, nếu không trả về false
        /// </summary>
        /// <returns></returns>
        public bool CheckSharpnessRange()
        {
            if (sharpnessToolEdit.Subject.Score > 0)
            {
                double temp = sharpnessToolEdit.Subject.Score;
                if ((temp > minSharpness) && (temp < maxSharpness)) return true;
                else return false;
            }
            return false;
        }
    }
}
