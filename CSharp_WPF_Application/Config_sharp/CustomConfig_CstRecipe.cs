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
using System.Windows;
using System.Windows.Shapes;

namespace Config_sharp
{
    public class CustomConfig_CstRecipe_Parameters
    {
        public EntityRecipe[] entityRecipes;

        public CustomConfig_CstRecipe_Parameters()
        {
            entityRecipes = new EntityRecipe[100];
            for (int i = 0; i < 100; i++)
            {
                entityRecipes[i] = new EntityRecipe(i);
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class CustomConfig_CstRecipe : BaseConfig<CustomConfig_CstRecipe>
    {
        //private string _version = "FinaljudgeListConfig_201912031520";
        private string _version = "CstConfig_202004081500";

        public CustomConfig_CstRecipe_Parameters parameters;

        public CustomConfig_CstRecipe()
        {
            version = _version;

            parameters = new CustomConfig_CstRecipe_Parameters();
            // Default Path
            RecipeFullPath = "./recipt.bat";
        }

        public CustomConfig_CstRecipe(string fileFullPath)
        {
            parameters = new CustomConfig_CstRecipe_Parameters();

            RecipeFullPath = fileFullPath;
        }

        protected override bool Write_Object(CustomConfig_CstRecipe config)
        {
            parameters = config.parameters.Clone() as CustomConfig_CstRecipe_Parameters;

            return (this._version == config._version);
        }
    }


    public class EntityRecipe
    {
        public int RecipeNumber = 0;

        public EdgeDetectionSet SetA;
        public EdgeDetectionSet SetB;
        public EdgeDetectionSet SetC;
        public EdgeDetectionSet SetD;
        public EdgeDetectionSet SetE;
        public DateTimeRecorgnizeSet SetF;

        public EntityRecipe()
        {
            RecipeNumber = 1;
            SetA = new EdgeDetectionSet();
            SetB = new EdgeDetectionSet();
            SetC = new EdgeDetectionSet();
            SetD = new EdgeDetectionSet();
            SetE = new EdgeDetectionSet();
            SetF = new DateTimeRecorgnizeSet();
        }
        public EntityRecipe(int number)
        {
            RecipeNumber = number;
            SetA = new EdgeDetectionSet();
            SetB = new EdgeDetectionSet();
            SetC = new EdgeDetectionSet();
            SetD = new EdgeDetectionSet();
            SetE = new EdgeDetectionSet();
            SetF = new DateTimeRecorgnizeSet();
        }

        ~EntityRecipe()
        {

        }
    }

    public class EdgeDetectionSet
    {
        public LightingSet LightingSet;
        public EdgeDetectionParameters IP_Paras;

        public EdgeDetectionSet()
        {
            IP_Paras = new EdgeDetectionParameters();
            LightingSet = new LightingSet();
        }
    }

    public class DateTimeRecorgnizeSet
    {
        public LightingSet LightingSet;
        public int CircleType;
        public ArrowDetectionParameters IP_Paras;

        public DateTimeRecorgnizeSet()
        {
            IP_Paras = new ArrowDetectionParameters();
            LightingSet = new LightingSet();
        }
    }

    public class LightingSet
    {
        public int deviceIndex;
        public int channel;
        public int Timeout;
        public int Lum;

        public LightingSet()
        {
            deviceIndex = 0;
            channel = 1;
            Timeout = 0;
            Lum = 0;
        }
        ~LightingSet()
        {

        }
    }

    public class EdgeDetectionParameters
    {
        // ROI Setting
        // --------------------------------------------------
        private Rectangle m_ImgROI;
        public int ImgROI_X;
        public int ImgROI_Y;
        public int ImgROI_Width;
        public int ImgROI_Height;
        // --------------------------------------------------

        // Binarization
        // --------------------------------------------------
        public int BinaryTh;
        public int ClosingIter;
        // --------------------------------------------------

        // Canny
        // --------------------------------------------------
        public int CannyTh1;
        public int CannyTh2;
        public int ApertureSize;
        // --------------------------------------------------

        // Contoures
        // --------------------------------------------------
        public string Orientation;
        public int MinLength;
        // --------------------------------------------------

        // Point Result
        // --------------------------------------------------
        public List<Point> edge_result;
        // --------------------------------------------------


        public EdgeDetectionParameters()
        {
            ImgROI_Width = 0;
            ImgROI_Height = 0;
            ImgROI_X = 0;
            ImgROI_Y = 0;

            Orientation = "H";

            m_ImgROI = new Rectangle();

            BinaryTh = 0;
            ClosingIter = 0;
            CannyTh1 = 0;
            CannyTh2 = 0;
            ApertureSize = 0;
            MinLength = 0;

            edge_result = new List<Point>();
        }

        ~EdgeDetectionParameters()
        {

        }

        public List<System.Windows.Point> edge_ressult { get; set; }
        //public Rectangle ImgROI { get => m_ImgROI; set => m_ImgROI = value; }
    }



    public class ArrowDetectionParameters
    {
        private int m_CircleType;
        private Size m_DigitalArrow;
        private int  m_OutterNumberOffset;
        private Size m_DigitalRL;
        private Size m_DigitalRL_Offset;

        public int CircleType { get => m_CircleType; set => m_CircleType = value; }
        public int DigitalArrowWidth { get => (int)m_DigitalArrow.Width; set => m_DigitalArrow.Width = value; }
        public int DigitalArrowHeight { get => (int)m_DigitalArrow.Height; set => m_DigitalArrow.Height = value; }
        public int OutterNumberOffset { get => m_OutterNumberOffset; set => m_OutterNumberOffset = value; }
        public int DigitalRLWidth { get => (int)m_DigitalRL.Width; set => m_DigitalRL.Width = value; }
        public int DigitalRLHeight { get => (int)m_DigitalRL.Height; set => m_DigitalRL.Height = value; }
        public int DigitalRL_OffsetWidth { get => (int)m_DigitalRL_Offset.Width; set => m_DigitalRL_Offset.Width = value; }
        public int DigitalRL_OffsetHeight { get => (int)m_DigitalRL_Offset.Height; set => m_DigitalRL_Offset.Height = value; }

        public ArrowDetectionParameters()
        {
            DigitalArrowWidth = 0;
            DigitalArrowHeight = 0;
            OutterNumberOffset = 0;
            DigitalRLWidth = 0;
            DigitalRLHeight = 0;
            DigitalRL_OffsetWidth = 0;
            DigitalRL_OffsetHeight = 0;
        }
        ~ArrowDetectionParameters()
        {

        }
    }

    public class CircleDetectionParameters
    {

    }

    public class OCRParameters
    {

    }
}

