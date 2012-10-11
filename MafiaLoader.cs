using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectSound;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Mafia
{
    /// <summary>
    /// リソースをStreamを介して読み込む。
    /// </summary>
    public class MafiaLoader
    {
        private static MafiaLoader defaultLoader;

        private string resourceName;

        static MafiaLoader()
        {
            defaultLoader = new MafiaLoader(Mafia.RESOURCE_NAME);
        }

        public MafiaLoader(string resourceName)
        {
            this.resourceName = resourceName;
        }

        public static MafiaLoader DefaultLoader
        {
            get
            {
                return defaultLoader;
            }
        }

        public FileStream GetFileStream(string fileName)
        {
            if (fileName.IndexOf(':') != -1)
            {
                return new FileStream(fileName, FileMode.Open);
            }
            return new FileStream(Application.StartupPath + "\\" + resourceName + "\\" + fileName, FileMode.Open);
        }

        public Icon GetIcon(string fileName)
        {
            return new Icon(GetFileStream(fileName));
        }

        public Texture GetTexture(Microsoft.DirectX.Direct3D.Device device, string fileName)
        {
            Stream stream = GetFileStream(fileName);
            // Texture texture = Texture.FromStream(device, stream, Usage.None, Pool.Managed);
            Texture texture;
            unchecked
            {
                texture = TextureLoader.FromStream(device, stream, 0, 0, 0, Usage.None, Format.Unknown, Pool.Default, Filter.None, Filter.None, (int)0xFF800080);
            }
            stream.Close();
            stream.Dispose();
            return texture;
        }

        public SecondaryBuffer GetBuffer(Microsoft.DirectX.DirectSound.Device device, string fileName)
        {
            BufferDescription bd = new BufferDescription();
            bd.ControlEffects = false;
            bd.ControlPan = true;
            bd.ControlVolume = true;
            return new SecondaryBuffer(GetFileStream(fileName), bd, device);
        }

        public Stage GetStage(string fileName)
        {
            Stream stream = GetFileStream(fileName);
            StreamReader reader = new StreamReader(stream);
            string title = null;
            int numRows = 0;
            int numCols = 0;
            try
            {
                title = reader.ReadLine();
                numRows = int.Parse(reader.ReadLine());
                numCols = int.Parse(reader.ReadLine());
            }
            catch (FormatException)
            {
                throw new Exception(fileName + " の書式に致命的なミスがあるっぽいです＞＜");
            }
            catch (OverflowException)
            {
                throw new Exception(fileName + " で何か途方もなく巨大な面を作ろうとしてませんか＞＜");
            }
            string[] source = new string[numRows];
            for (int row = 0; row < numRows; row++)
            {
                source[row] = reader.ReadLine();
            }
            reader.Close();
            reader.Dispose();
            stream.Close();
            stream.Dispose();
            return new Stage(fileName, title, numRows, numCols, source);
        }
    }
}
