using BurcatProtocol;
using BurcatProtocol.Annotations;
using FFMpegCore;
using FFMpegCore.Pipes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace Burcat.API
{
    [BurcatIdentity("ef10c94a-3577-407c-976a-17b49d107fb8")]
    public class Video : BurcatObject, IInterfaceObject
    {
        [NotBurcatInvokable]
        public static string VideosPath { get; set; } = "C:/Media/Videos/";

        public static bool ValidateVideo(byte[] data)
        {
            bool isMP4 = data[4] == 'f' && data[5] == 't' && data[6] == 'y' && data[7] == 'p';

            return isMP4;
        }

        public BurcatIdentifier<Member> Creator { get; }
        public BurcatIdentifier<Politeness> Politeness { get; set; }

        public Video(BurcatIdentifier<Member> creator, BurcatIdentifier<Politeness> politeness) { Creator = creator; Politeness = politeness; }

        public void SetVideo(byte[] data)
        {
            if (ValidateVideo(data)) File.WriteAllBytes($"{VideosPath}{Identifier}.mp4", data);
            else throw new ArgumentException("The data is not from a MP4 video", nameof(data));
        }

        public virtual int GetVideoLength()
        {
            if (File.Exists($"{VideosPath}{Identifier}.mp4")) return FFProbe.Analyse($"{VideosPath}{Identifier}.mp4").Duration.Seconds;
            else throw new FileNotFoundException("The video does not exist");
        }

        public virtual byte[] GetVideo(int fromSecond, int toSecond)
        {
            if (!File.Exists($"{VideosPath}{Identifier}.mp4")) throw new FileNotFoundException("The video does not exist");
            else if (toSecond <= fromSecond) throw new ArgumentException("End must be greater than start");
            else
            {
                MemoryStream outputStream = new();
                StreamPipeSource inputPipe = new(new MemoryStream(File.ReadAllBytes($"{VideosPath}{Identifier}.mp4")));
                StreamPipeSink outputPipe = new(outputStream);

                FFMpegArguments
                    .FromPipeInput(inputPipe, options => options.Seek(TimeSpan.FromSeconds(fromSecond)))
                    .OutputToPipe(outputPipe, options => options
                        .WithDuration(TimeSpan.FromSeconds(toSecond - fromSecond))
                        .WithVideoCodec("copy") // <-- no re-encoding
                        .WithAudioCodec("copy")) // <-- no re-encoding
                    .ProcessSynchronously();

                return outputStream.ToArray();
            }
        }

        public virtual byte[] GetVideo()
        {
            if (File.Exists($"{VideosPath}{Identifier}.png")) return File.ReadAllBytes($"{VideosPath}{Identifier}.mp4");
            else throw new FileNotFoundException("The video does not exist");
        }

        bool IInterfaceObject.ShouldManage(BurcatIdentifier<Member>? member) => Creator == member;
        public override object?[] GetBurcatConstructionValues() => [Creator, Politeness];
    }
}
