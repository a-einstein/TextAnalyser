using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TextAnalyser.Resources;
using Utilities;

namespace TextAnalyser.ViewModels
{
    internal class MainViewModel : DependencyObject
    {
        private ICommand? readFileCommand;
        public ICommand ReadFileCommand => readFileCommand ?? (readFileCommand = new CommandHandler(ReadFile, () => true));

        private bool fileRead;
        private bool FileRead
        {
            get { return fileRead; }
            set
            {
                fileRead = value;
            }
        }

        private void ReadFile()
        {
            string fileMessage; ;

            FileRead = Read(out fileMessage);
            FileMessage = fileMessage;
        }

        public bool Read(out string message)
        {
            var initialDirectory = Path.GetFullPath(Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\Texts");

            var fileDialog = new OpenFileDialog
            {
                Title = Texts.FileReadTitle,
                Filter = "TXT files|*.txt",
                InitialDirectory = initialDirectory
            };

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var filePath = fileDialog.FileName;

                message = string.Format(Texts.MessageFileName_name, Path.GetFileName(filePath));

                Text = File.ReadAllText(filePath);

                return true;
            }
            else
            {
                message = "No file read.";
                return false;
            }
        }

        public static readonly DependencyProperty FileMessageProperty =
            DependencyProperty.Register(nameof(FileMessage), typeof(string), typeof(MainViewModel), new PropertyMetadata("No file yet"));

        public string FileMessage
        {
            get { return (string)GetValue(FileMessageProperty); }
            set { SetValue(FileMessageProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(MainViewModel), new PropertyMetadata("Read a file or edit."));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
