using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Threading;
using System.Net;

namespace Config
{
    public class BaseConfig<T>
    {
        public string version;
        public string RecipeFullPath = null;
        private string m_ConfigName = "";

        public bool IsRecipeExist { get { return File.Exists(RecipeFullPath); } }

        // Rijndael 演算法的 Managed,
        // Managed version of the Rijndael Algorithm to encrypt and decrypt data
        private static readonly RijndaelManaged KeyManaged = new RijndaelManaged();

        // 加密器
        // Transform interface for "Encryptor"
        private ICryptoTransform Encryptor = KeyManaged.CreateEncryptor(
            System.Text.ASCIIEncoding.ASCII.GetBytes("dikj9517IJYD19cj"),
            System.Text.ASCIIEncoding.ASCII.GetBytes("87ds13IUNC23id56"));

        // 解密器
        // Transform interface for "Decryptor"
        private ICryptoTransform Decryptor = KeyManaged.CreateDecryptor(
            System.Text.ASCIIEncoding.ASCII.GetBytes("dikj9517IJYD19cj"),
            System.Text.ASCIIEncoding.ASCII.GetBytes("87ds13IUNC23id56"));

        // Xml序列化器
        // Serializes and deserializes object into and from XML documents.
        private readonly XmlSerializer Serializer = new XmlSerializer(typeof(T));

        protected string ConfigName { get => m_ConfigName; set { m_ConfigName = (value == "") ? "MyConfigName" : value; } }

        protected virtual bool Write_Object(T Config_Class)
        {
            return false;
        }

        public bool Save()
        {
            try
            {
                //FileFolderSelector();

                DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(RecipeFullPath));

                if (dirInfo.Exists == false)
                {
                    DialogResult result = MessageBox.Show("Saving folder is not exist. Do you want to create new folder?", ConfigName + " Config Save Warrning", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        dirInfo.Create();
                    }

                    return false;
                }

                if (dirInfo.Exists == true)
                {
                    WriteConfig(RecipeFullPath);

                    return true;
                }
                else
                {
                    MessageBox.Show( "There is no config file in \n\"" + Path.GetDirectoryName(Path.GetFullPath(RecipeFullPath)) + "\\\"", ConfigName + " Config Save Warrning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Config Function Error Occurred.", "Save() Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public bool Load()
        {
            try
            {
                //FileFolderSelector();

                if (IsRecipeExist)
                {
                    ReadConfig(RecipeFullPath);
                    return true;
                }
                else
                {
                    MessageBox.Show("Config file: \"" + Path.GetFileName(RecipeFullPath)+"\" is not exit in:\n\"" + Path.GetDirectoryName(Path.GetFullPath(RecipeFullPath)) + "\\\"", 
                        "Config Load Warrning", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Config Function Error Occurred.", "Load() Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private bool FileFolderSelector()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Multiselect = false,
                    Filter = "Dat files (*.dat)|*.dat|All files (*.*)|*.*",
                    InitialDirectory = Directory.GetCurrentDirectory(),
                    FileName = Path.GetFileName(RecipeFullPath),
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    RecipeFullPath = openFileDialog.FileName;
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private void WriteConfig(string recipeFullPath)
        {
            try
            {
                // 文件流
                // Read "Recipe File" as "File Stream" buffer
                FileStream fileStream = new FileStream(recipeFullPath, FileMode.Create);

                // 加密流(將文件流(目標資料流)加密成加密流)
                // Encrypt "File Stream" into "Encrypted Stream" buffer
                CryptoStream encryptedStream = new CryptoStream(fileStream, Encryptor, CryptoStreamMode.Write);

                // 記憶體資料流
                // A backing store memory stream buffer be used for xml serializing
                MemoryStream memoryStream = new MemoryStream();


                // 序列化這個物件進記憶體流
                // Serializes "Class Object" into "Memory Stream"
                Serializer.Serialize(memoryStream, this);

                // 從記憶體流寫入目標資料流
                // Writes the "Memory Stream" to "Encrypted Streaem"
                encryptedStream.Write(memoryStream.ToArray(), 0, Convert.ToInt32(memoryStream.Length));

                // Flush and close
                // Update "Encrypted Stream" data with the "MemoryStream", then clears the buffer
                encryptedStream.FlushFinalBlock();

                // 用完記得關或使用using(python的with...as)
                // Close and release any resouces of current streams
                //encryptedStream.Close();
                //memoryStream.Close();
                //fileStream.Close();

                // By Dispose(), Releases all resources used by the Stream object.
                encryptedStream.Dispose();
                memoryStream.Dispose();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ReadConfig(string recipeFullPath)
        {
            try
            {
                // 文件流
                // Read "Recipe File" as "File Stream" buffer
                FileStream fileStream = new FileStream(recipeFullPath, FileMode.Open);

                // 加密流(將文件流加密至加密流)
                // Decrypt "File Stream" into "Decrypted Stream" buffer
                CryptoStream decryptStream = new CryptoStream(fileStream, Decryptor, CryptoStreamMode.Read);

                // XML讀取器
                // Loading "Decrypted Stream" into "XML Buffer" (a buffer that is forward-only access)
                XmlReader xmlReader = new XmlTextReader(decryptStream);

                // 泛型類
                // Class-based generic object
                T customObject;

                // 反序列化
                // Deserializes "XML Buffer" into "Class Object"
                customObject = (T)(Serializer.Deserialize(xmlReader));

                //用完記得關或使用using(python的with...as)
                // Close and release any resouces of current streams                
                //decryptStream.Close();
                //fileStream.Close();

                // By Dispose(), Releases all resources used by the Stream object.
                decryptStream.Dispose();
                fileStream.Dispose();

                if (this.Write_Object(customObject) == false)
                {
                    this.WriteConfig(recipeFullPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }


    }

    public class TCP_IPv4
    {
        private string m_IP;
        private string m_Port;
        private string m_ProtocolName;

        public string IP
        {
            get { return m_IP; }
            set
            {
                try
                {
                    m_IP = IPAddress.Parse(value).ToString();
                }
                catch (ArgumentNullException e)
                {
                    MessageBox.Show(
                        e.Message, m_ProtocolName + " IP Address Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                catch (FormatException e)
                {
                    MessageBox.Show(
                        e.Message, m_ProtocolName + " IP Address Format Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                catch (Exception e)
                {
                    MessageBox.Show(
                        "Source: " + e.Source + "\nMessag:e" + e.Message, m_ProtocolName + " IP Address Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public string Port
        {
            get => m_Port;
            set
            {
                try
                {
                    if (Convert.ToInt32(value) < 0 || Convert.ToInt32(value) > 65532) return;
                    m_Port = value;
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        m_ProtocolName + " Port Number Error!", m_ProtocolName + " Setting Error!", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public string ProtocolName { get => m_ProtocolName; }

        public TCP_IPv4()
        {
            this.IP = "127.0.0.1";
            this.Port = "21";
            this.m_ProtocolName = "TCPIP";
        }

        public TCP_IPv4(string ip, string port)
        {
            this.IP = ip;
            this.Port = port;
            SetProtocolName();
        }

        public void SetProtocolName()
        {
            //  Protocol name set as TCPIP as not set.
            SetProtocolName("");
        }

        public void SetProtocolName(string opt)
        {
            switch (opt)
            {
                case "FTP":
                    m_ProtocolName = opt;
                    break;
                case "PLC":
                    m_ProtocolName = opt;
                    break;
                case "AI":
                    m_ProtocolName = opt;
                    break;
                default:
                    m_ProtocolName = "TCPIP";
                    break;
            }
        }
    }


    public class Predict_Config
    {
        public string modelPath;
        public string ImagePath;
        public string OutputPath;
        public string ConfidenceLevel;
        public string Class_Name;
        public string Class_Judge_setting;

        public Predict_Config()
        {
            this.modelPath = null;
            this.ImagePath = null;
            this.OutputPath = null;
            this.ConfidenceLevel = null;
            this.Class_Name = null;
            this.Class_Judge_setting = null;
        }
    }


    public class Capture_Rect
    {
        public string x_Capture_Rect;
        public string y_Capture_Rect;
        public string width_Capture_Rect;
        public string height_Capture_Rect;

        public Capture_Rect()
        {
            this.x_Capture_Rect = null;
            this.y_Capture_Rect = null;
            this.width_Capture_Rect = null;
            this.height_Capture_Rect = null;
        }
    }
}
