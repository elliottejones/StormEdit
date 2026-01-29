using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using System.Net.NetworkInformation;
using TextMateSharp.Grammars;

namespace StormEdit.Views
{
    public partial class MainWindow : Window
    {
        private TextEditor _editor;
        public MainWindow()
        {
            InitializeComponent();

            _editor = this.FindControl<TextEditor>("Editor");

            var registryOptions = new RegistryOptions(ThemeName.DarkPlus);

            var textMateInstallation = _editor.InstallTextMate(registryOptions);

            textMateInstallation.SetGrammar(_editor.Document.TextLength == 0
                ? registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".lua").Id)
                : registryOptions.GetScopeByLanguageId("lua"));
        }
    }
}