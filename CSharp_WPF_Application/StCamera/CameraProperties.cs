using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// EmguCV
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows;
using Sentech.StApiDotNET;

//Config
using Config_sharp;

//Log
using LOGRECORDER;

namespace CameraDevice
{
    public class CameraProperties
    {
	}

	public class Component
	{
		public int DataStreamIndex;
		public int FPS;
		public int Gain;
		public int Exposure;
		public String FullName;
		public String Name;
		public String Mac;
		public IStDeviceInfo DeviceInfo;
		public Image<Bgr, byte> GrabImage;
		public Bitmap OneBitmap;
		private CustomConfig_Camera CameraConfig;
		private string m_RecipeDirectoryPath = System.Environment.CurrentDirectory + @"\Appendix\Config\";
		private string m_RecipeFilename = "Camera";
		private string m_RecipeSubtitle = ".dat";
		private string m_RecipeFullPath;

		public Component()
		{
			DataStreamIndex = -1;
			Name = "";
			Mac = "";
			GrabImage = new Image<Bgr, byte>(2592, 1944);
			OneBitmap = null;
		}

		public void CameraConfigInitial()
		{
			// Append "RecipeFilename" to "DeviceName"
			m_RecipeFilename = m_RecipeFilename + "_" + Name;

			m_RecipeFullPath = m_RecipeDirectoryPath + m_RecipeFilename + m_RecipeSubtitle;

			CameraConfig = new CustomConfig_Camera(m_RecipeFullPath);
		}

		public void CameraConfigLoad()
        {
			if (CameraConfig.Load() == false) return;

			FPS = Convert.ToInt32(CameraConfig.parameters.FPS_Value);
			Gain = Convert.ToInt32(CameraConfig.parameters.Gain_Value);
			Exposure = Convert.ToInt32(CameraConfig.parameters.Exposure_Value);
		}

		public void CameraConfigSave()
		{
            Console.WriteLine("FPS:" + FPS);
            Console.WriteLine("Gain:" + Gain);
            Console.WriteLine("Exposure:" + Exposure);

            CameraConfig.parameters.FPS_Value = FPS;
			CameraConfig.parameters.Gain_Value = Gain;
			CameraConfig.parameters.Exposure_Value = Exposure;

			CameraConfig.Save();			
		}
	};
}
