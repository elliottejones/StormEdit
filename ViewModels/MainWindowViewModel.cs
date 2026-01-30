using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLua;
using StormEdit.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace StormEdit.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            AddTestCreation();
            AddTestCreation();

            // manually refresh ui when script is edited or minified
            _script.TextChanged += (s, e) => {
                OnPropertyChanged(nameof(EditorScriptLength));
                OnPropertyChanged(nameof(EditorScriptString));
            };
            _minifiedScript.TextChanged += (s, e) => {
                OnPropertyChanged(nameof(MinifiedScriptLength));
                OnPropertyChanged(nameof(MinifiedScriptString));
            };
        }

        // SERVICES AND HELPERS

        private LinkService _linkService = new LinkService();

        // SCRIPT LENGTH AND CONTENT BINDINGS

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EditorScriptString))]
        [NotifyPropertyChangedFor(nameof(EditorScriptLength))]
        private TextDocument _script = new();
        public string EditorScriptString => Script.Text;
        public int EditorScriptLength => Script.Text.Length;


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MinifiedScriptString))]
        [NotifyPropertyChangedFor(nameof(MinifiedScriptLength))]
        private TextDocument _minifiedScript = new();
        public string MinifiedScriptString => MinifiedScript.Text;
        public int MinifiedScriptLength => MinifiedScript.Text.Length;
        [ObservableProperty]
        private bool _isMinifyOpen = false;

        // SETTINGS BINDINGS

        [ObservableProperty]
        private bool _isSettingsOpen = false;

        [ObservableProperty]
        private int _fontSize = 16;

        [ObservableProperty]
        private string _gamePath = "";
        private bool gamePathValid = false;

        // DIALOGUE BINDINGS

        [ObservableProperty]
        private bool _isDialogueOpen = false;
        [ObservableProperty]
        private string _dialogueMessage = "";
        [ObservableProperty]
        private string _dialogueTitle = "";
        [ObservableProperty]
        private bool _isExitDialogue = false;

        [RelayCommand]
        private void IncorrectGamePath()
        {
            DialogueMessage = "Incorrect Stormworks game file - please check path and try again.";
            DialogueTitle = "❌ Error";
            IsDialogueOpen = true;
            IsExitDialogue = false;
        }

        [RelayCommand]
        private void CloseDialogue()
        {
            IsDialogueOpen = false;
        }

        // EXPLORER BINDINGS
        [ObservableProperty]
        private bool _includeScriptlessCreations = true;

        [RelayCommand]
        private void RefreshExplorer()
        {
            List<Creation>? entries = _linkService.GetExplorerTree(_gamePath, IncludeScriptlessCreations);

            if (entries == null)
            {
                IncorrectGamePath();
                return;
            }

            ExplorerEntries.Clear();

            foreach (Creation creation in entries)
            {
                ExplorerEntries.Add(creation);
            }
        }

        [RelayCommand]
        public async Task OpenGameFolder(Object? root)
        {
            var topLevel = TopLevel.GetTopLevel(root as Control);

            var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Open Stormworks Folder",
                AllowMultiple = false
            });

            if (folder.Count == 0)
                return;

            IStorageFolder gameFolder = folder[0];
            GamePath = gameFolder.Path.ToString().Substring(8);
        }

        public ObservableCollection<Creation> ExplorerEntries { get; set; } = new();

        public void AddCreation(Creation creation)
        {
            ExplorerEntries.Add(creation);
        }

        public void AddTestCreation()
        {
            Creation test = new("Test Creation");
            test.Scripts.Add(new LuaScript(424,"Test Script 1"));
            test.Scripts.Add(new LuaScript(425,"Test Script 2"));
            AddCreation(test);
        }

        
        [RelayCommand]
        private void Minify()
        {
            IsMinifyOpen = true;
            MinifiedScript.Text = RunMinifyScript(EditorScriptString);

        }

        private string RunMinifyScript(string script)
        {
            using (Lua lua = new())
            {
                string minifyScript = LoadMinifyScript();

                lua.DoString(minifyScript);

                var minifyFunction = lua["minify"] as LuaFunction;

                if (minifyFunction == null)
                {
                    throw new Exception("Could not find global function 'minify' in Lua script.");
                }

                var result = minifyFunction.Call(script);

                if (result == null || result.Length == 0 || result[0] == null)
                {
                    string errorMsg = (result.Length > 1 && result[1] != null)
                                      ? result[1].ToString()
                                      : "Unknown Minification Error";

                    System.Diagnostics.Debug.WriteLine(errorMsg);
                    return script; 
                }

                return result[0].ToString();
            }
        }

        public string LoadMinifyScript()
        {
            var assembly = Assembly.GetExecutingAssembly();

            string resourceName = "StormEdit.Scripts.MinifyLogic.lua";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [RelayCommand]
        private void CloseMinify()
        {
            IsMinifyOpen = false;
        }

        [RelayCommand]
        private void OpenSettings()
        {
            IsSettingsOpen = true;
        }

        [RelayCommand]
        private void CloseSettings()
        {
            IsSettingsOpen = false;
        }
    }
}
