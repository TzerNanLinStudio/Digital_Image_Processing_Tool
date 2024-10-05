/*
 * To Use:
 * -----------------------
 * Step 1: Make a copy from this file
 * Step 2: Modify "Filename" => ex. "CustomConfig_Example_Parameters.cs" to "CustomConfig_MyProjectName.cs"
 * Step 3: Modify Class Name of "Parameters Class" => ex.  "CustomConfig_Example_Parameters" to "CustomConfig_MyProjectName_Parameters"
 * Step 4: Modify Class Name of "Main Class" => "CustomConfig_Example" to "CustomConfig_MyProjectName"
 * Step 5: Setting variables for "Parameters Class"
 * -----------------------
 * Note: There are some type of variables can use.  (Such as: TCP/IP) Locate to "BaseConfig.cs"
 * -----------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_sharp
{
    public class CustomConfig_Example_Parameters
    {
        public CustomConfig_Example_Parameters()
        {
            m_CH1_Value = -1;
            m_CH2_Value = -1;
            m_CH3_Value = -1;
        }

        private int m_CH1_Value;
        private int m_CH2_Value;
        private int m_CH3_Value;

        public int CH1_Value
        {
            set
            {
                m_CH1_Value = (value < 0 || value > 100) ? -1 : value;
            }

            get
            {
                return (m_CH1_Value == -1) ? 0 : m_CH1_Value;
            }
        }
        public int CH2_Value
        {
            set
            {
                m_CH2_Value = (value < 0 || value > 100) ? -1 : value;
            }

            get
            {
                return (m_CH2_Value == -1) ? 0 : m_CH2_Value;
            }
        }
        public int CH3_Value
        {
            set
            {
                m_CH3_Value = (value < 0 || value > 100) ? -1 : value;
            }

            get
            {
                return (m_CH3_Value == -1) ? 0 : m_CH3_Value;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class CustomConfig_Example : BaseConfig<CustomConfig_Example>
    {
        //private string _version = "FinaljudgeListConfig_201912031520";
        private string _version = "LightingConfig_202004081500";

        public CustomConfig_Example_Parameters parameters;

        public CustomConfig_Example()
        {
            version = _version;

            parameters = new CustomConfig_Example_Parameters();
            // Default Path
            RecipeFullPath = "./recipt.bat";
        }

        public CustomConfig_Example(string fileFullPath)
        {
            parameters = new CustomConfig_Example_Parameters();

            RecipeFullPath = fileFullPath;
        }

        protected override bool Write_Object(CustomConfig_Example config)
        {
            parameters = config.parameters.Clone() as CustomConfig_Example_Parameters;

            return (this._version == config._version);
        }
    }
}

