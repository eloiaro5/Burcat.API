using BurcatProtocol;
using BurcatProtocol.Annotations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace Burcat.API
{
    [BurcatIdentity("5c0ea179-74df-4391-bb47-473fc9229bca")]
    public class Image : BurcatObject, IInterfaceObject
    {
        [NotBurcatInvokable]
        public static string ImagesPath { get; set; } = "C:/Media/Images/";

        public static byte[] ResizeImage(byte[] data, int width, int height)
        {
            using var image = SixLabors.ImageSharp.Image.Load([.. data]);
            image.Mutate(x => x.Resize(width, height));

            using var ms = new MemoryStream();
            image.SaveAsPng(ms);

            return ms.ToArray();
        }

        public BurcatIdentifier<Member> Creator { get; }
        public BurcatIdentifier<Politeness> Politeness { get; set; }

        public Image(BurcatIdentifier<Member> creator, BurcatIdentifier<Politeness> politeness) { Creator = creator; Politeness = politeness; }

        public virtual void SetImage([BurcatCustomValidation(typeof(DataValidator), nameof(DataValidator.ValidateImageData))] byte[] data) => File.WriteAllBytes($"{ImagesPath}{Identifier}.png", ResizeImage(data, 512, 512));
        public virtual void DeleteImage()
        {
            if (File.Exists($"{ImagesPath}{Identifier}.png"))
                File.Delete($"{ImagesPath}{Identifier}.png");
        }

        public virtual byte[] GetImage()
        {
            if (File.Exists($"{ImagesPath}{Identifier}.png")) return [.. File.ReadAllBytes($"{ImagesPath}{Identifier}.png")];
            else throw new FileNotFoundException("The image does not exist");
        }

        public bool ShouldCreate(BurcatIdentifier<Member>? member) => ShouldManage(member);
        public bool ShouldSelect(BurcatIdentifier<Member>? member) => true;
        public bool ShouldManage(BurcatIdentifier<Member>? member) => Creator == member;

        public override object?[] GetBurcatConstructionValues() => [Creator, Politeness];
    }
}
