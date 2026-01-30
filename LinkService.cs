using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia.Metadata;

namespace StormEdit
{
    public class LinkService
    {
        public LinkService()
        {
            
        }

        public List<Creation>? GetExplorerTree(string gamePath, bool includeScriptless)
        {
            List<Creation> explorerTree = new();

            string vehicleFolderPath = ValidateGamePath(gamePath);

            if (vehicleFolderPath == null)
            {
                return null;
            }

            foreach (string file in Directory.EnumerateFiles(vehicleFolderPath, "*.xml"))
            {
                StreamReader reader = new StreamReader(file);
                string fileContent = reader.ReadToEnd();
                
                if (!includeScriptless && !fileContent.Contains("script="))
                {
                    continue;
                }

                explorerTree.Add(ConstructCreationData(file, fileContent));
            }

            return explorerTree;

        }

        private Creation ConstructCreationData(string filePath, string fileContent)
        {
            Creation creation = new Creation(Path.GetFileNameWithoutExtension(filePath));

            for (int i = 0; i < fileContent.Length; i++)
            {
                int scriptIndex = fileContent.IndexOf("script=", i, StringComparison.OrdinalIgnoreCase);
                if (scriptIndex == -1)
                {
                    break;
                }
                int startQuoteIndex = fileContent.IndexOf('"', scriptIndex);
                int endQuoteIndex = fileContent.IndexOf('"', startQuoteIndex + 1);
                if (startQuoteIndex == -1 || endQuoteIndex == -1)
                {
                    break;
                }

                string scriptName = "Unnamed Script";
                int titleIndex = fileContent.IndexOf("title{");
                if (titleIndex != -1)
                {
                    int closeBraceIndex = fileContent.IndexOf("}title");
                    scriptName = fileContent[(titleIndex + 6 )..closeBraceIndex];
                }
                LuaScript luaScript = new LuaScript(creation.Scripts.Count + 1, scriptName);
                creation.Scripts.Add(luaScript);
                i = endQuoteIndex;
            }

            return creation;
        }

        private string ValidateGamePath(string path)
        { 
            string dataFolderPath = Path.Combine(path, "data");
            // check if data folder exists
            if (!Directory.Exists(dataFolderPath))
            {
                return null;
            }

            string vehicleFolderPath = Path.Combine(dataFolderPath, "vehicles");
            // check if vehicles folder exists
            if (!Directory.Exists(vehicleFolderPath))
            {
                return null;
            }

            return vehicleFolderPath;
        }

    }
}
