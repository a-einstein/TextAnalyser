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
        public static readonly DependencyProperty ClearFirstProperty =
            DependencyProperty.Register("ClearFirst", typeof(bool), typeof(MainViewModel), new PropertyMetadata(true));

        public bool ClearFirst
        {
            get { return (bool)GetValue(ClearFirstProperty); }
            set { SetValue(ClearFirstProperty, value); }
        }

        private ICommand? analyseCommand;
        public ICommand AnalyseCommand => analyseCommand ?? (analyseCommand = new CommandHandler(Analyse, () => !String.IsNullOrEmpty(Text)));

        private void Analyse()
        {
            if (ClearFirst)
                Addresses.Clear();

            var streetExpr = @"(?<street>(\w|[\-])+)";
            var spaceExpr = @"\s+";
            var numberExpr = @"(?<number>\d+)";
            var connectorExpr = @"([-]|\s)*";
            var additionExpr = @"(?<addition>[a-z]*)";
            var separatorExpr = @"([,;]|\s)+";
            var codeExpr = @"(?<code>\d{4}\s*[a-z]{2})";
            var townExpr = @"(?<town>(\w|[\-])+)";

            var addressExpression = new Regex($"{streetExpr}{spaceExpr}{numberExpr}{connectorExpr}{additionExpr}{separatorExpr}{codeExpr}{separatorExpr}{townExpr}", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            using (StringReader reader = new StringReader(Text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var match = addressExpression.Match(line);

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