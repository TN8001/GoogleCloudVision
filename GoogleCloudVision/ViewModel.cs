using System.Collections.ObjectModel;


namespace GoogleCloudVision
{
    internal class ViewModel : Observable
    {
        private object _Pages;
        public object Pages { get => _Pages; set => Set(ref _Pages, value); }

        private string _ImagePath;
        public string ImagePath { get => _ImagePath; set => Set(ref _ImagePath, value); }

        public ObservableCollection<BoundingBox> BoundingBoxes { get; } = new ObservableCollection<BoundingBox>();
    }
}
