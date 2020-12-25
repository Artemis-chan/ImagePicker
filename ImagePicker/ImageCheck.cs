using System;
using System.IO;

public static class ImageCheck
{

    private const int BUFFER_SIZE = 11;

    //taken from https://stackoverflow.com/a/9359622/12651549
    private static Func<byte[], bool>[] imageChecks = {
        //JPEG
        (headerBytes) =>
        {
            return (headerBytes[0] == 0xFF &&//FF D8
                    headerBytes[1] == 0xD8 &&
                    (
                        (headerBytes[6] == 0x4A &&//'JFIF'
                        headerBytes[7] == 0x46 &&
                        headerBytes[8] == 0x49 &&
                        headerBytes[9] == 0x46)
                        ||
                        (headerBytes[6] == 0x45 &&//'EXIF'
                        headerBytes[7] == 0x78 &&
                        headerBytes[8] == 0x69 &&
                        headerBytes[9] == 0x66)
                    ) &&
                    headerBytes[10] == 00);
        },
        //PNG
        (headerBytes) =>
        {
            return (headerBytes[0] == 0x89 && //89 50 4E 47 0D 0A 1A 0A
                    headerBytes[1] == 0x50 &&
                    headerBytes[2] == 0x4E &&
                    headerBytes[3] == 0x47 &&
                    headerBytes[4] == 0x0D &&
                    headerBytes[5] == 0x0A &&
                    headerBytes[6] == 0x1A &&
                    headerBytes[7] == 0x0A);
        },
        //BMP
        (headerBytes) =>
        {
            return (headerBytes[0] == 0x42 &&//42 4D
                    headerBytes[1] == 0x4D);
        }
    };

    public static bool IsImage(string path)
    {
        byte[] bytes = new byte[BUFFER_SIZE];
        using (var file = File.OpenRead(path))
        {
            file.Read(bytes, 0, BUFFER_SIZE - 1);
        }

        foreach (var Check in imageChecks)
        {
            if (Check(bytes))
                return true;
        }

        return false;
    }
}