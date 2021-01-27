using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuipuGmbHTest
{
    public class UrlContent : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public int CountOfTags { get; set; }

        private bool isHighest;
        public bool IsHighest
        {
            get => isHighest; 
            set
            {
                isHighest = value;
                OnPropertyChanged(nameof(IsHighest));
            }
        }

        public UrlContent()
        {

        }
        public UrlContent(string name, int countOfTags, bool isHighest) : this(name, countOfTags)
        {
            this.IsHighest = isHighest;
        }

        public UrlContent(string name, int countOfTags)
        {
            this.Name = name;
            this.CountOfTags = countOfTags;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
