using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TextAnalyser.Model;
using TextAnalyser.Resources;
using Utilities;

namespace TextAnalyser.ViewModels
{
    internal class MainViewModel : DependencyObject
    {
        #region File
        private ICommand? readFileCommand;
        public ICommand ReadFileCommand => readFileCommand ?? (readFileCommand = new CommandHandler(ReadFile, () => true));

        private void ReadFile()
        {
            string fileMessage; ;

            Read(out fileMessage);
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
            DependencyProperty.Register(nameof(FileMessage), typeof(string), typeof(MainViewModel), new PropertyMetadata("Read a file or edit."));

        public string FileMessage
        {
            get { return (string)GetValue(FileMessageProperty); }
            set { SetValue(FileMessageProperty, value); }
        }
        #endregion

        #region Text
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(MainViewModel));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion

        #region Analysis
        private ICommand? analyseCommand;
        public ICommand AnalyseCommand => analyseCommand ?? (analyseCommand = new CommandHandler(Analyse, () => !String.IsNullOrEmpty(Text)));

        private void Analyse()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Addresses
        public static readonly DependencyProperty AdressesProperty =
            DependencyProperty.Register(nameof(Adresses), typeof(ObservableCollection<Address>), typeof(MainViewModel), new PropertyMetadata(new ObservableCollection<Address>()));

        public ObservableCollection<Address> Adresses
        {
            get { return (ObservableCollection<Address>)GetValue(AdressesProperty); }
            set { SetValue(AdressesProperty, value); }
        }
        #endregion
    }
}