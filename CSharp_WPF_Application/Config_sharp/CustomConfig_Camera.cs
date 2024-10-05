using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_sharp
{
    public class CameraParameters
    {
        public CameraParameters()
        {
            FPS = 14;
            Gain = 0;
            Exposure = 71100;
        }

        private int FPS;
        private int Gain;
        private int Exposure;

        public int FPS_Value
        {
            set
            {
                FPS = (value < 10 || value > 350) ? 14 : value;
            }

            get
            {
                return FPS;
            }
        }
        public int Gain_Value
        {
            set
            {
                Gain = (value < 0 || value > 40) ? 0 : value;
            }

            get
            {
                return Gain;
            }
        }
        public int Exposure_Value
        {
            set
            {
                Exposure = (value < 50 || value > 16000000) ? 71100: value;
            }

            get
            {
                return Exposure;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    
    public class CustomConfig_Camera : BaseConfig<CustomConfig_Camera>
    {
        //private string _version = "FinaljudgeListConfig_201912031520";
        private string _version = "CameraConfig_202004081500";

        public CameraParameters parameters;

        public CustomConfig_Camera()
        {
            version = _version;

            parameters = new CameraParameters();
            // Default Path
            RecipeFullPath = "./recipt.bat";

            ConfigName = "Camera";
        }

        public CustomConfig_Camera(string fileFullPath)
        {
            parameters = new CameraParameters();

            RecipeFullPath = fileFullPath;
        }

        protected override bool Write_Object(CustomConfig_Camera config)
        {
            parameters = config.parameters.Clone() as CameraParameters;

            return (this._version == config._version);
        }
    }
}

