using System.Windows;
using System.Windows.Media;
using Google.Cloud.Vision.V1;


namespace GoogleCloudVision
{
    internal class BoundingBox : Observable
    {
        public object Inner;
        public PointCollection Points { get; } = new PointCollection();
        public Brush Stroke { get; }
        public string Type { get; }

        private bool _IsChecked = true;
        public bool IsChecked { get => _IsChecked; set { Set(ref _IsChecked, value); OnPropertyChanged(nameof(IsVisible)); } }

        private bool _IsSelected;
        public bool IsSelected { get => _IsSelected; set { Set(ref _IsSelected, value); OnPropertyChanged(nameof(IsVisible)); } }

        public bool IsVisible => IsChecked || IsSelected;


        public BoundingBox(Block block) : this(block, block.BoundingBox) => Stroke = Brushes.Red;
        public BoundingBox(Paragraph paragraph) : this(paragraph, paragraph.BoundingBox) => Stroke = Brushes.Green;
        public BoundingBox(Word word) : this(word, word.BoundingBox) => Stroke = Brushes.Blue;

        private BoundingBox(object obj, BoundingPoly poly)
        {
            Inner = obj;
            Type = obj.GetType().Name;
            foreach(var p in poly.Vertices)
                Points.Add(new Point { X = p.X, Y = p.Y });
        }
    }
}
