namespace tensorflow.keras {
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    class ImageTools {
        public static float[,,] Coord(int width, int height) {
            var result = new float[width, height, 2];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) {
                    result[x, y, 0] = x * 2f / width - 1;
                    result[x, y, 1] = y * 2f / height - 1;
                }

            return result;
        }

        public static dynamic NormalizeChannelValue(dynamic value) => value / 128f - 1f;

        public static unsafe byte[,,] ToBytesHWC(Bitmap bitmap) {
            byte[,,] result = new byte[bitmap.Height, bitmap.Width, 4];
            int rowBytes = bitmap.Width * 4;
            var rect = new Rectangle(default, new Size(bitmap.Width, bitmap.Height));
            var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try {
                fixed (byte* dest = result) {
                    for (int y = 0; y < bitmap.Height; y++) {
                        var source = data.Scan0 + data.Stride * y;
                        Buffer.MemoryCopy((byte*)source, destination: &dest[rowBytes * y], rowBytes, rowBytes);
                    }
                }
            } finally {
                bitmap.UnlockBits(data);
            }

            return result;
        }
    }
}
