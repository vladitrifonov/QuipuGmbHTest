using GalaSoft.MvvmLight;
using Microsoft.Win32;
using QuipuGmbHTest.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace QuipuGmbHTest
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IMessageBoxService messageBoxService;
        private string filePath;
        private readonly string searchTag = "<a";
        Action<int, int> UpdateGeneralInfo;
        Action<string, int> UpdateUrlInfo;

        public MainWindowViewModel(IMessageBoxService messageBoxService)
        {
            this.messageBoxService = messageBoxService;

            UpdateGeneralInfo += UpdateProgressBar;
            UpdateGeneralInfo += UpdateTextProgress;
          
            UpdateUrlInfo += AddItem;
            UpdateUrlInfo += UpdateHighestUri;
        }

        public RelayCommand ChooseFile => new RelayCommand(obj =>
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                filePath = openFileDialog.FileName;
        });

        private bool IsProcessing;
        private Thread thread;
        public RelayCommand Start => new RelayCommand(obj =>
        {
            if (IsProcessing)
            {
                return;
            }

            if (filePath == null)
            {
                messageBoxService.ShowMessage("Wasn't selected file", "Error");
            }
            else
            {
                TagsCollection = new ObservableCollection<UrlContent>();

                Task.Run(() =>
                {
                    thread = Thread.CurrentThread;
                    IsProcessing = true;
                    IFetch fetchService = new FileFetch(filePath);
                    var urls = fetchService.Fetch();

                    using (var client = new WebClient())
                    {
                        var counter = 1;
                        foreach (var url in urls)
                        {
                            if (IsUrlValid(url))
                            {
                                var htmlCode = DownloadString(client, url);

                                var countOfTags = Regex.Matches(htmlCode, searchTag).Count;

                                   UpdateUrlInfo?.Invoke(url, countOfTags);
                                UpdateGeneralInfo?.Invoke(counter++, urls.Count);                             
                            }
                        }
                    }

                    IsProcessing = false;
                    messageBoxService.ShowMessage("Complete", "Info");
                });
            }
        });


        public RelayCommand Stop => new RelayCommand(obj =>
        {
            if (IsProcessing)
            {
                thread.Abort();
                IsProcessing = false;
                thread = null;
                HighestUrl = new UrlContent();
                messageBoxService.ShowMessage("Operation was stopped", "Info");
            }
        });
              

        private ObservableCollection<UrlContent> tagsCollection = new ObservableCollection<UrlContent>();
        public ObservableCollection<UrlContent> TagsCollection
        {
            get => tagsCollection;
            set
            {
                tagsCollection = value;
                RaisePropertyChanged(nameof(TagsCollection));
            }
        }
        private string DownloadString(WebClient client, string url)
        {
            try
            {
                return client.DownloadString(url);
            }
            catch (Exception ex)
            {
                //handle the exception, logging or whatever               
                return "";
            }
        }

        private void UpdateProgressBar(int counter, int countOfColl)
        {
            ProgressBarValue = (int)((double)counter++ / countOfColl * 100);
        }

        private void UpdateTextProgress(int counter, int countOfColl)
        {
            TextProgress = $"Completed {counter} from {countOfColl}";
        }

        private void AddItem(string url, int countOfTags)
        {           
            App.Current.Dispatcher.Invoke(() =>
            {
                if (countOfTags > HighestUrl?.CountOfTags)
                {
                    TagsCollection.ToList().ForEach(x => x.IsHighest = false);
                    TagsCollection.Add(new UrlContent(url, countOfTags, true));
                    RaisePropertyChanged(nameof(TagsCollection));
                }
                else
                {
                    TagsCollection.Add(new UrlContent(url, countOfTags));
                }
              
            });

        }

        private void UpdateHighestUri(string url, int countOfTags)
        {
            if (countOfTags > HighestUrl?.CountOfTags)
            {               
                HighestUrl = new UrlContent(url, countOfTags);                
            }
        }

        private UrlContent _highestUrl = new UrlContent();
        public UrlContent HighestUrl
        {
            get => _highestUrl;
            set
            {
                _highestUrl = value;
                RaisePropertyChanged(nameof(HighestUrl));
            }
        }

        private int _progressBarValue;
        public int ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                _progressBarValue = value;
                RaisePropertyChanged(nameof(ProgressBarValue));
            }
        }

        private string _textProgress;
        public string TextProgress
        {
            get => _textProgress;
            set
            {
                _textProgress = value;
                RaisePropertyChanged(nameof(TextProgress));
            }
        }
        private bool IsUrlValid(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }


    }
}
