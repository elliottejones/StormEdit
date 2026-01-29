using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace StormEdit.Views
{
    public partial class MainWindow : Window
    {
        private TextEditor _editor;
        private TextEditor _minifiedPreview;
        public MainWindow()
        {
            InitializeComponent();

            _editor = this.FindControl<TextEditor>("Editor");
            _minifiedPreview = this.FindControl<TextEditor>("MinifiedPreview");

            SetLuaGrammar(_editor);
            SetLuaGrammar(_minifiedPreview);

        }

        private void SetLuaGrammar(TextEditor editor)
        {
            var registryOptions = new RegistryOptions(ThemeName.DarkPlus);

            var textMateInstallation = editor.InstallTextMate(registryOptions);

            textMateInstallation.SetGrammar(editor.Document.TextLength == 0
                ? registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".lua").Id)
                : registryOptions.GetScopeByLanguageId("lua"));
        }
    }
}