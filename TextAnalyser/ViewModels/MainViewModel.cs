using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
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
        string initialDirectory = Path.GetFullPath(Directory.GetCurrentDirectory() + @"\..\..\..\..\Texts");
        string fileFilter = "TXT files|*.txt";

        private ICommand? readFileCommand;
        public ICommand ReadFileCommand => readFileCommand ??= new CommandHandler(ReadFile, () => true);

        private void ReadFile()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = Texts.FileDialogTitle,
                Filter = fileFilter,
                InitialDirectory = initialDirectory
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = fileDialog.FileName;

                FileMessage = string.Format(Texts.MessageFileName_name, Path.GetFileName(filePath));

                Text = File.ReadAllText(filePath);
            }
            else
            {
                FileMessage = Texts.MessageNoFile;
            }
        }

        private ICommand? saveFileCommand;
        public ICommand SaveFileCommand => saveFileCommand ??= new CommandHandler(SaveFile, () => !String.IsNullOrEmpty(Text));

        private void SaveFile()
        {
            var fileDialog = new SaveFileDialog
            {
                Title = Texts.FileDialogTitle,
                Filter = fileFilter,
                InitialDirectory = initialDirectory
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = fileDialog.FileName;

                File.WriteAllText(filePath, Text);

                FileMessage = string.Format(Texts.MessageFileName_name, Path.GetFileName(filePath));
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
        public static readonly DependencyProperty ClearFirstProperty =
            DependencyProperty.Register("ClearFirst", typeof(bool), typeof(MainViewModel), new PropertyMetadata(true));

        public bool ClearFirst
        {
            get { return (bool)GetValue(ClearFirstProperty); }
            set { SetValue(ClearFirstProperty, value); }
        }

        private ICommand? analyseCommand;
        public ICommand AnalyseCommand => analyseCommand ??= new CommandHandler(Analyse, () => !String.IsNullOrEmpty(Text));

        private void Analyse()
        {
            if (ClearFirst)
                Addresses.Clear();

            var streetExpr = @"(?<street>(\w|[\-])+)";
            var spaceExpr = @"\s+";
            var numberExpr = @"(?<number>\d+)";
            var connectorExpr = @"([-]|\s)*"; // Optional connector.
            var additionExpr = @"(?<addition>[a-z]*)"; // Optional addition.
            var breakExpr = @"([,;]|\s|\r?$)+"; // Enables line breaks, effectively 2 line addresses.
            var codeExpr = @"(?<code>\d{4}\s*[a-z]{2})";
            var separatorExpr = @"([,;]|\s)+"; // Some required separation.
            var townExpr = @"(?<town>(\w|[\-])+)";

            // Note this assumes a COMPLETE address with some tolerance in the format.
            var addressExpression = new Regex(
                $"{streetExpr}{spaceExpr}{numberExpr}{connectorExpr}{additionExpr}{breakExpr}{codeExpr}{separatorExpr}{townExpr}",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            var matches = addressExpression.Matches(Text);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var groups = match.Groups;

                    var streetGroup = groups["street"];
                    var numberGroup = groups["number"];
                    var additionGroup = groups["addition"];
                    var codeGroup = groups["code"];
                    var townGroup = groups["town"];

                    var address = new Address()
                    {
                        Street = streetGroup.Success ? streetGroup.Value : default,
                        Number = numberGroup.Success ? numberGroup.Value : default,
                        Addition = additionGroup.Success ? additionGroup.Value.ToUpper() : default,
                        Code = codeGroup.Success ? Regex.Replace(codeGroup.Value, spaceExpr, string.Empty) : default,
                        Town = townGroup.Success ? townGroup.Value : default
                    };

                    Addresses.Add(address);
                }
            }

            AnalysisMessage = string.Format(Texts.AnalysisResult_number, Addresses.Count);
        }

        public static readonly DependencyProperty AnalysisMessageProperty =
            DependencyProperty.Register(nameof(AnalysisMessage), typeof(string), typeof(MainViewModel));

        public string AnalysisMessage
        {
            get { return (string)GetValue(AnalysisMessageProperty); }
            set { SetValue(AnalysisMessageProperty, value); }
        }
        #endregion

        #region Addresses
        public static readonly DependencyProperty AdressesProperty =
            DependencyProperty.Register(nameof(Addresses), typeof(ObservableCollection<Address>), typeof(MainViewModel), new PropertyMetadata(new ObservableCollection<Address>()));

        public ObservableCollection<Address> Addresses
        {
            get { return (ObservableCollection<Address>)GetValue(AdressesProperty); }
            set { SetValue(AdressesProperty, value); }
        }
        #endregion
    }
}