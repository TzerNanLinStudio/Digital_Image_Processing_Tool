using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_sharp
{
    public class PLCParameters
    {
        //Parameters For Station K  
        // --------------------------------------------------
        private bool m_IsOpen;
        private bool[][] m_StatusFlag;
        private string m_StstionIP;
        private string m_StstionName;        
        private int m_StstionPort;
        private int[] m_ReceiveValue;
        // --------------------------------------------------


        public PLCParameters()
        {
            //Parameters For Station K  
            // --------------------------------------------------
            m_IsOpen = false;
            m_StstionIP = "";
            m_StstionName = "";
            m_StstionPort = 0;
            

            m_StatusFlag = new bool[3][];
            for(int i = 0; i < 3; i++)
            {
                m_StatusFlag[i] = new bool[16];
                for (int j = 0; j < 3; j++)
                {
                    m_StatusFlag[i][j] = false;
                }
            }

            m_ReceiveValue = new int[3];
            for (int i = 0; i < 3; i++)
            {
                m_ReceiveValue[i] = 0;
            }
            // --------------------------------------------------
        }

        #region "Parameters For Station K"
        public bool IsOpen
        {
            set
            {
                m_IsOpen = value;
            }

            get
            {
                return m_IsOpen;
            }
        }

        public bool[][] StatusFlag
        {
            set
            {
                m_StatusFlag = value;
            }

            get
            {
                return m_StatusFlag;
            }
        }

        public string StstionIP
        {
            set
            {
                m_StstionIP = value;
            }

            get
            {
                return m_StstionIP;
            }
        }

        public string StstionName
        {
            set
            {
                m_StstionName = value;
            }

            get
            {
                return m_StstionName;
            }
        }

        public int StstionPort
        {
            set
            {
                m_StstionPort = value;
            }

            get
            {
                return m_StstionPort;
            }
        }

        public int[] ReceiveValue
        {
            set
            {
                m_ReceiveValue = value;
            }

            get
            {
                return m_ReceiveValue;
            }
        }
        #endregion


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class CustomConfig_PLCParameters : BaseConfig<CustomConfig_PLCParameters>
    {
        //private string _version = "FinaljudgeListConfig_201912031520";
        private string _version = "PLCConfig_202004081500";

        public PLCParameters parameters;

        public CustomConfig_PLCParameters()
        {
            version = _version;

            parameters = new PLCParameters();
            // Default Path
            RecipeFullPath = "./recipt.bat";

            ConfigName = "PLC";
        }

        public CustomConfig_PLCParameters(string fileFullPath)
        {
            parameters = new PLCParameters();

            RecipeFullPath = fileFullPath;
        }

        protected override bool Write_Object(CustomConfig_PLCParameters config)
        {
            parameters = config.parameters.Clone() as PLCParameters;

            return (this._version == config._version);
        }
    }
}

