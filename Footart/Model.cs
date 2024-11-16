using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Footart;

namespace Footart
{
    public class Model : INotifyPropertyChanged
    {
        string _path;
        string[] _files;
        bool _showFiles = true, _showFolders = false;
        public string Path { get { return _path; } set { _path = value; OnPropertyChanged("Path"); } }
        public string[] Files { get { return _files; } set { _files = value; OnPropertyChanged("Files"); } }
        public bool ShowFiles { get { return _showFiles; } set { _showFiles = value; OnPropertyChanged("ShowFiles"); } }
        public bool ShowFolders { get { return _showFolders; } set { _showFolders = value; OnPropertyChanged("ShowFolders"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        MainWindow _view;

        public Model(MainWindow view)
        {
            _view = view;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(
                    this,
                    new PropertyChangedEventArgs(propertyName)
                    );
            }

            //Lazy =D
            if (propertyName == "Path" || propertyName == "ShowFiles" || propertyName == "ShowFolders")
            {
                _view.ClearCache();
                List<string> folderAndFiles = new List<string>();
                if (ShowFolders) folderAndFiles.AddRange(Directory.GetDirectories(Path).ToArray());
                if (ShowFiles) folderAndFiles.AddRange(Directory.GetFiles(Path).ToArray());

                Files = folderAndFiles.ToArray();
            }
        }
    }


}

