using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLua;
using System;
using System.IO;
using System.Reflection;

namespace StormEdit.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
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
        private bool _isSettingsOpen = false;

        [ObservableProperty]
        private int _fontSize = 16;

        [ObservableProperty]
        private bool _isMinifyOpen = false;

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
