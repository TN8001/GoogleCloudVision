using System;
using System.Globalization;
using System.Windows.Data;
using Google.Cloud.Vision.V1;


namespace GoogleCloudVision
{
    internal sealed class VisionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Page page) return "Page:" + page.DebugString();
            if(value is Block block) return "Block:" + block.DebugString();
            if(value is Paragraph paragraph) return "Paragraph:" + paragraph.DebugString();
            if(value is Word word) return "Word:" + word.DebugString();

            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
