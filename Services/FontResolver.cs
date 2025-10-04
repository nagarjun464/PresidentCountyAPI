using System.Reflection;
using PdfSharp.Fonts;

namespace PresidentCountyAPI.Services
{
    public class CustomFontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            string resourceName = faceName.ToLower() switch
            {
                "dejavusans" => "PresidentCountyAPI.Fonts.DejaVuSans.ttf",
                "dejavusans-bold" => "PresidentCountyAPI.Fonts.DejaVuSans-Bold.ttf",
                _ => null
            };

            if (resourceName == null)
                throw new Exception($"Unknown font: {faceName}");

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception($"Font not found: {resourceName}");

            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("DejaVuSans", StringComparison.OrdinalIgnoreCase))
            {
                if (isBold) return new FontResolverInfo("dejavusans-bold");
                return new FontResolverInfo("dejavusans");
            }

            // Default fallback
            return new FontResolverInfo("dejavusans");
        }
    }
}
