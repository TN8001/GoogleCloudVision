using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Google.Cloud.Vision.V1;


namespace GoogleCloudVision
{
    public partial class MainWindow : Window
    {
        private readonly ViewModel vm = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop, true)
                ? DragDropEffects.Copy
                : DragDropEffects.None;

            e.Handled = true;
        }

        private async void Window_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                if(files.FirstOrDefault() is string file)
                {
                    Cursor = Cursors.Wait;
                    await DetectDocumentTextAsync(file);
                    Cursor = null;
                }
            }
        }

        private async Task DetectDocumentTextAsync(string path)
        {
            vm.BoundingBoxes.Clear();
            vm.ImagePath = path;

            var image = Image.FromFile(path);
            var client = ImageAnnotatorClient.Create();
            var response = await client.DetectDocumentTextAsync(image);
            //var res = await client.DetectTextAsync(image);

            vm.Pages = response.Pages;

            Debug.WriteLine($"Text: {response.Text}");

            //var w = response.Pages.SelectMany(x => x.Blocks.SelectMany(x => x.Paragraphs.SelectMany(x => x.Words)));
            //var g = w.GroupBy(x => Math.Round(x.BoundingBox.Vertices[0].Y / 10.0)).OrderBy(x => x.Key);
            //var p = g.Select(x => string.Join(" ", x.Select(x => x.DebugString()))).ToList();

            var words = new List<Word>();
            foreach(var page in response.Pages)
            {
                foreach(var block in page.Blocks)
                {
                    vm.BoundingBoxes.Add(new BoundingBox(block));
                    foreach(var paragraph in block.Paragraphs)
                    {
                        vm.BoundingBoxes.Add(new BoundingBox(paragraph));
                        foreach(var word in paragraph.Words)
                        {
                            vm.BoundingBoxes.Add(new BoundingBox(word));
                            words.Add(word);
                        }
                    }
                }
            }

            var groups = new SortedDictionary<int, List<Word>>();
            foreach(var word in words)
            {
                var round = (int)Math.Round(word.BoundingBox.Vertices[0].Y / 12.5
                    );
                if(groups.ContainsKey(round))
                    groups[round].Add(word);
                else
                    groups.Add(round, new List<Word> { word });
            }

            var Paragraphs = new List<string>();
            foreach(var list in groups.Values)
            {
                var s = "";
                foreach(var word in list)
                    s += word.DebugString() + " ";
                Paragraphs.Add(s.TrimEnd());
            }

            aa(Paragraphs);
            //Debug.WriteLine(string.Join("\n", Paragraphs));
        }

        private void aa(List<string> Paragraphs)
        {
            // 商品のとこだけ
            var n = Paragraphs
                .SkipWhile(x => !x.Contains("領"))
                .Skip(1)
                .TakeWhile(x => !x.Contains("( 商品"));

            var str = "";
            foreach(var word in n)
            {
                if(word.Contains("¥"))
                    str += word + "¥";
                else
                    str += word;
            }

            var m = str.Split("¥");
            foreach(var w in m)
            {
                Debug.WriteLine(w);
            }
            //もち 麦鮭 わかめ むすび
            // 130
            //コールスロー サラダ
            // 203
            //値 引 額 - 50亀田 ハッピー ターン
            // 192
            //7 プレミアム むぎ 茶 600ml
            // 100
            //厳選 米 おむすび いくら
            // 150
            //銀 しゃり むすび 塩 むすび@ 100x2
            // 200
            //冷し とろろ 蕎麦
            // 430
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is ToggleButton toggleButton)
            {
                var check = toggleButton.IsChecked.Value;
                var type = toggleButton.Content as string;

                foreach(var box in vm.BoundingBoxes.Where(x => x.Type == type))
                    box.IsChecked = check;
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            foreach(var box in vm.BoundingBoxes)
                box.IsSelected = e.NewValue == box.Inner;
        }
    }
}
