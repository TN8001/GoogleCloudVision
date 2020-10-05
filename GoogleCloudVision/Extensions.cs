using System.Linq;
using Google.Cloud.Vision.V1;


namespace GoogleCloudVision
{
    internal static class Extensions
    {
        public static string DebugString(this Page @this) => string.Join("\\n", @this.Blocks.Select(x => x.DebugString()));
        public static string DebugString(this Block @this) => string.Join(" ", @this.Paragraphs.Select(x => x.DebugString()));
        public static string DebugString(this Paragraph @this) => string.Concat(@this.Words.Select(x => x.DebugString()));
        public static string DebugString(this Word @this) => string.Concat(@this.Symbols.Select(x => x.Text));
    }
}
