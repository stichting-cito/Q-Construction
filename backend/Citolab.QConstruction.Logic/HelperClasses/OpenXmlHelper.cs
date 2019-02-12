using System;
using System.Drawing;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace Citolab.QConstruction.Logic.HelperClasses
{
    public static class OpenXmlHelper
    {
        public static void AddText(this Body body, string text)
        {
            var para = body.AppendChild(new Paragraph());
            var run = para.AppendChild(new Run());
            run.AppendChild(new Text(text));
        }

        public static void InsertAPicture(this MainDocumentPart mainPart, byte[] image, string filename)
        {
            var imagePart = mainPart.AddImagePart(GetImagePartTypeByExtension(Path.GetExtension(filename)));

            using (var stream = new MemoryStream(image))
            {
                imagePart.FeedData(stream);
            }

            AddImageToBody(mainPart, mainPart.GetIdOfPart(imagePart), filename, image);

        }

        private static ImagePartType GetImagePartTypeByExtension(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return ImagePartType.Jpeg;
                case ".bmp":
                    return ImagePartType.Bmp;
                case ".png":
                    return ImagePartType.Png;
                default: return ImagePartType.Jpeg;
            }
        }
        private static void AddImageToBody(MainDocumentPart mainPart, string relationshipId, string fileName, byte[] image)
        {
            var img = Image.FromStream(new MemoryStream(image));
            var iWidth = (int)Math.Round((decimal)img.Width * 9525);
            var iHeight = (int)Math.Round((decimal)img.Height * 9525);
            // Define the reference of the image.
            var element =
                new Drawing(
                    new DW.Inline(
                        new DW.Extent() { Cx = iWidth, Cy = iHeight },
                        new DW.EffectExtent()
                        {
                            LeftEdge = 0L,
                            TopEdge = 0L,
                            RightEdge = 0L,
                            BottomEdge = 0L
                        },
                        new DW.DocProperties()
                        {
                            Id = (UInt32Value)1U,
                            Name = Path.GetFileNameWithoutExtension(fileName)
                        },
                        new DW.NonVisualGraphicFrameDrawingProperties(
                            new A.GraphicFrameLocks() { NoChangeAspect = true }),
                        new A.Graphic(
                            new A.GraphicData(
                                    new PIC.Picture(
                                        new PIC.NonVisualPictureProperties(
                                            new PIC.NonVisualDrawingProperties()
                                            {
                                                Id = (UInt32Value)0U,
                                                Name = Path.GetFileName(fileName)
                                            },
                                            new PIC.NonVisualPictureDrawingProperties()),
                                        new PIC.BlipFill(
                                            new A.Blip(
                                                new A.BlipExtensionList(
                                                    new A.BlipExtension()
                                                    {
                                                        Uri =
                                                            "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                    })
                                            )
                                            {
                                                Embed = relationshipId,
                                                CompressionState =
                                                    A.BlipCompressionValues.Print
                                            },
                                            new A.Stretch(
                                                new A.FillRectangle())),
                                        new PIC.ShapeProperties(
                                            new A.Transform2D(
                                                new A.Offset() { X = 0L, Y = 0L },
                                                new A.Extents() { Cx = iWidth, Cy = iHeight }),
                                            new A.PresetGeometry(
                                                    new A.AdjustValueList()
                                                )
                                            { Preset = A.ShapeTypeValues.Rectangle }))
                                )
                                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                    )
                        {
                            DistanceFromTop = (UInt32Value)0U,
                            DistanceFromBottom = (UInt32Value)0U,
                            DistanceFromLeft = (UInt32Value)0U,
                            DistanceFromRight = (UInt32Value)0U,
                            EditId = "50D07946"
                        });

            // Append the reference to body, the element should be in a Run.
            mainPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }


    }
}
