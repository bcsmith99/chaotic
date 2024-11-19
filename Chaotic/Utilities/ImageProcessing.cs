using OpenCvSharp;
using System.IO;
using System.Windows;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;

namespace Chaotic.Utilities
{
    public class ScreenSearchResult()
    {
        public bool Found { get; set; }
        public int CenterX { get; set; }
        public int CenterY { get; set; }
        public double MaxConfidence { get; set; }

        public List<ImageMatch> Matches { get; set; } = new List<ImageMatch>();

        public Point Center { get { return new Point(CenterX, CenterY); } }
    }

    public class ImageMatchResult()
    {
        public double MaxConfidence { get; set; }
        public List<ImageMatch> Matches { get; set; } = new List<ImageMatch>();
    }
    public class ImageMatch()
    {
        public double Confidence { get; set; }
        public OpenCvSharp.Rect Match { get; set; }
        public Point Center { get; set; }
    }

    public class ImageProcessing
    {
        public static bool SHOW_DEBUG_IMAGES = false;
        public static bool SAVE_DEBUG_IMAGES = false;

        public static System.Drawing.Bitmap CaptureScreen(Rect rect)
        {
            return CaptureScreen(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static System.Drawing.Bitmap CaptureScreen(int x, int y, int width, int height)
        {
            var bitmap = new System.Drawing.Bitmap(width, height);
            using (var g = System.Drawing.Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(x, y, 0, 0, bitmap.Size, System.Drawing.CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }

        public static System.Drawing.Bitmap CaptureScreen(/*ResourceHelper rh*/)
        {
            //var width = rh.GetInt("ScreenX");
            //var height = rh.GetInt("ScreenY");
            var width = Convert.ToInt32(System.Windows.SystemParameters.PrimaryScreenWidth);
            var height = Convert.ToInt32(System.Windows.SystemParameters.PrimaryScreenHeight);
            return CaptureScreen(0, 0, width, height);
        }

        public static List<Point> ConvertPointArray(string points)
        {
            var results = new List<Point>();
            string[] lines = points.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.TrimEntries);
            foreach (var line in lines)
            {
                var currentPoint = line.Split(",");
                if (currentPoint.Length == 2)
                    results.Add(new Point(Int32.Parse(currentPoint[0]), Int32.Parse(currentPoint[1])));
            }

            return results;
        }

        public static Point GetPointFromStringCoords(string coords)
        {
            var positions = coords.Split(',');
            return new Point(Int32.Parse(positions[0]), Int32.Parse(positions[1]));
        }

        public static Rect ConvertStringCoordsToRect(string region)
        {
            var coords = region.Split(",");
            if (coords.Length != 4)
                throw new ArgumentOutOfRangeException("Provided region must be 4 comma-separated numbers.");

            return new Rect(Int32.Parse(coords[0]), Int32.Parse(coords[1]), Int32.Parse(coords[2]), Int32.Parse(coords[3]));
        }

        public static ScreenSearchResult LocateCenterOnScreen(string filePath, Rect coords, double confidence = .999, bool useGrayscale = false)
        {
            return LocateCenterOnScreen(filePath, coords.X, coords.Y, coords.Width, coords.Height, confidence, useGrayscale);
        }

        public static ScreenSearchResult LocateCenterOnScreen(MemoryStream findImage, Rect coords, double confidence = .999, bool useGrayscale = false)
        {
            return LocateCenterOnScreen(findImage, coords.X, coords.Y, coords.Width, coords.Height, confidence, useGrayscale);
        }

        public static ScreenSearchResult LocateCenterOnScreen(string filePath, int x = 0, int y = 0, int? width = null, int? height = null, double confidence = .999, bool useGrayscale = false)
        {
            using (var fs = File.OpenRead(filePath))
            {
                return LocateCenterOnScreen(fs, x, y, width, height, confidence, useGrayscale);
            }
        }
        public static ScreenSearchResult LocateCenterOnScreen(Stream findImage, int x = 0, int y = 0, int? width = null, int? height = null, double confidence = .999, bool useGrayscale = false)
        {
            var result = new ScreenSearchResult();
            var imResult = LocateOnScreen(findImage, x, y, width, height, confidence, useGrayscale);
            result.MaxConfidence = imResult.MaxConfidence;
            result.Matches = imResult.Matches;

            if (result.Matches.Any())
            {
                result.Found = true;
                var top = result.Matches.OrderByDescending(x => x.Confidence).First();

                var center = GetMatchCenter(top.Match, x, y);
                result.CenterX = center.X;
                result.CenterY = center.Y;
                //result.CenterX = x + (top.Match.X + .5 * top.Match.Width);
                //result.CenterY = y + (top.Match.Y + .5 * top.Match.Height);
            }
            else
                result.Found = false;

            return result;
        }

        public static Point GetMatchCenter(Rect rect, int x, int y)
        {
            var centerX = x + (rect.X + .5 * rect.Width);
            var centerY = y + (rect.Y + .5 * rect.Height);
            return new Point(centerX, centerY);
        }

        public static ImageMatchResult LocateOnScreen(String filePath, Rect region, double confidence = .999, bool useGrayscale = false)
        {
            return LocateOnScreen(filePath, region.X, region.Y, region.Width, region.Height, confidence, useGrayscale);
        }

        public static ImageMatchResult LocateOnScreen(String filePath, int x = 0, int y = 0, int? width = null, int? height = null, double confidence = .999, bool useGrayscale = false)
        {
            using (var fs = File.OpenRead(filePath))
            {
                return LocateOnScreen(fs, x, y, width, height, confidence, useGrayscale);
            }
        }

        public static ImageMatchResult LocateOnScreen(Stream findImage, int x = 0, int y = 0, int? width = null, int? height = null, double confidence = .999, bool useGrayscale = false)
        {

            if (!width.HasValue)
                width = Convert.ToInt32(System.Windows.SystemParameters.PrimaryScreenWidth);
            if (!height.HasValue)
                height = Convert.ToInt32(System.Windows.SystemParameters.PrimaryScreenHeight);

            using (var screenshot = CaptureScreen(x, y, width.Value, height.Value))
            {
                return Locate(findImage, screenshot, x, y, confidence, useGrayscale);
            }
        }

        public static void RunTemplateMatch(string reference, string template)
        {
            using (Mat refMat = new Mat(reference))
            using (Mat tplMat = new Mat(template))
            using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
            {
                Cv2.ImShow("Reference", refMat);
                Cv2.ImShow("Template", tplMat);

                //Convert input images to gray
                Mat gref = refMat.CvtColor(ColorConversionCodes.BGR2GRAY);
                Mat gtpl = tplMat.CvtColor(ColorConversionCodes.BGR2GRAY);

                Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        //Setup the rectangle to draw
                        OpenCvSharp.Rect r = new OpenCvSharp.Rect(new OpenCvSharp.Point(maxloc.X, maxloc.Y), new OpenCvSharp.Size(tplMat.Width, tplMat.Height));
                        Console.WriteLine($"MinVal={minval.ToString()} MaxVal={maxval.ToString()} MinLoc={minloc.ToString()} MaxLoc={maxloc.ToString()} Rect={r.ToString()}");

                        //Draw a rectangle of the matching area
                        Cv2.Rectangle(refMat, r, Scalar.LimeGreen, 2);

                        //Fill in the res Mat so you don't find the same area again in the MinMaxLoc
                        OpenCvSharp.Rect outRect;
                        Cv2.FloodFill(res, maxloc, new Scalar(0), out outRect, new Scalar(0.1), new Scalar(1.0));
                    }
                    else
                        break;
                }

                Cv2.ImShow("Matches", refMat);
                Cv2.WaitKey();
            }
        }

        //int x, int y, int width, int height,
        public static ImageMatchResult Locate(Stream toFind, System.Drawing.Bitmap src, int x, int y, double confidence = .999, bool grayScale = false)
        {
            var result = new ImageMatchResult() { MaxConfidence = 0 };

            var findImg = Mat.FromStream(toFind, ImreadModes.Color);
            var srcImg = src.ToMat();


            if (SHOW_DEBUG_IMAGES)
                Cv2.ImShow("Find Image", findImg);
            //Cv2.WaitKey();
            if (SHOW_DEBUG_IMAGES)
                Cv2.ImShow("Source Image", srcImg);

            using (Mat res = new Mat(srcImg.Rows - findImg.Rows + 1, srcImg.Cols - findImg.Cols + 1, MatType.CV_32FC1))
            {
                Mat gFind = findImg.CvtColor(grayScale ? ColorConversionCodes.RGB2GRAY : ColorConversionCodes.RGB2BGR);
                Mat gSrc = srcImg.CvtColor(grayScale ? ColorConversionCodes.RGB2GRAY : ColorConversionCodes.RGB2BGR);

                if (SHOW_DEBUG_IMAGES)
                    Cv2.ImShow("Converted Find Image", gFind);
                //Cv2.WaitKey();
                if (SHOW_DEBUG_IMAGES)
                    Cv2.ImShow("Converted Source Image", gSrc);
                //Cv2.WaitKey();

                Cv2.MatchTemplate(gFind, gSrc, res, TemplateMatchModes.CCoeffNormed);
                var foundThreshold = Cv2.Threshold(res, res, confidence, 1.0, ThresholdTypes.Tozero);

                //Cv2.ImShow("Res Image", res);
                while (true)
                {
                    double minVal, maxVal, threshold = confidence;
                    OpenCvSharp.Point minLoc, maxLoc;
                    Cv2.MinMaxLoc(res, out minVal, out maxVal, out minLoc, out maxLoc);

                    result.MaxConfidence = Math.Max(result.MaxConfidence, maxVal);

                    if (maxVal >= threshold)
                    {
                        ////Setup the rectangle to draw
                        OpenCvSharp.Rect r = new OpenCvSharp.Rect(new OpenCvSharp.Point(maxLoc.X, maxLoc.Y), new OpenCvSharp.Size(findImg.Width, findImg.Height));
                        //Console.WriteLine($"MinVal={minVal.ToString()} MaxVal={maxVal.ToString()} MinLoc={minLoc.ToString()} MaxLoc={maxLoc.ToString()} Rect={r.ToString()}");

                        ////Draw a rectangle of the matching area
                        Cv2.Rectangle(srcImg, r, Scalar.LimeGreen, 2);

                        ////Fill in the res Mat so you don't find the same area again in the MinMaxLoc
                        OpenCvSharp.Rect outRect;
                        Cv2.FloodFill(res, maxLoc, new Scalar(0), out outRect, new Scalar(0.1), new Scalar(1.0));

                        result.Matches.Add(new ImageMatch() { Confidence = maxVal, Match = r, Center = GetMatchCenter(r, x, y) });
                    }
                    else
                        break;
                }
                //Cv2.ImShow("Res", res);
            }

            if (result.Matches.Count > 0 && SAVE_DEBUG_IMAGES)
            {
                findImg.SaveImage($"C:\\DebugPics\\findImg{DateTime.Now.ToString("mm-dd-yyyy-HH-ss-fff")}.png");
                srcImg.SaveImage($"C:\\DebugPics\\srcImg{DateTime.Now.ToString("mm-dd-yyyy-HH-ss-fff")}.png");
            }


            //Cv2.ImShow("Matches", srcImg);
            //Cv2.WaitKey();

            return result;
        }
    }
}
