using LT.DigitalOffice.UserService.Mappers.Helper.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Helper
{
  public class ResizeImageHelper : IResizeImageHelper
  {
    private readonly Dictionary<string, IImageFormat> imageFormats = new()
    {
      { ".jpg", JpegFormat.Instance },
      { ".jpeg", JpegFormat.Instance },
      { ".png", PngFormat.Instance },
      { ".bmp", BmpFormat.Instance },
      { ".gif", GifFormat.Instance },
      { ".tga", TgaFormat.Instance }
    };

    public string Resize(string inputBase64, string extension)
    {
      try
      {
        byte[] byteString = Convert.FromBase64String(inputBase64);
        Image image = Image.Load(byteString);

        if (image.Width > 150 && image.Height > 150)
        {
          var minSize = Math.Min(image.Width, image.Height);
          var offsetX = (image.Width - minSize) / 2;
          var offsetY = (image.Height - minSize) / 2;

          image.Mutate(x => x.Crop(new Rectangle(offsetX, offsetY, minSize, minSize)));

          image.Mutate(x => x.Resize(150, 150));

          return image.ToBase64String(imageFormats[extension]).Split(',')[1];
        }
        else
        {
          return null;
        }
      }
      catch
      {
        return null;
      }
    }
  }
}