using System;
using System.Numerics;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Controls;
using Emgu.CV;
using Emgu.CV.Flann;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Config;
//using LOGRECORDER;

namespace ImageProcessing
{
    public class BasicAlgorithm
    {
        /// <summary>
        /// Applies dilation to the given grayscale image.
        /// </summary>
        /// <param name="originImage">The input grayscale image.</param>
        /// <param name="kernelSize">Size of the structuring element (default is 3).</param>
        /// <param name="iterations">Number of times dilation is applied (default is 1).</param>
        /// <returns>The dilated image.</returns>
        public static Image<Gray, byte> ToDilation(Image<Gray, byte> originImage, int kernelSize = 3, int iterations = 1)
        {
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(kernelSize, kernelSize), new Point(-1, -1));
            Image<Gray, byte> result = new Image<Gray, byte>(originImage.Size);
            CvInvoke.Dilate(originImage, result, kernel, new Point(-1, -1), iterations, BorderType.Default, new MCvScalar(0));
            return result;
        }

        /// <summary>
        /// Applies erosion to the given grayscale image.
        /// </summary>
        /// <param name="originImage">The input grayscale image.</param>
        /// <param name="kernelSize">Size of the structuring element (default is 3).</param>
        /// <param name="iterations">Number of times erosion is applied (default is 1).</param>
        /// <returns>The eroded image.</returns>
        public static Image<Gray, byte> ToErosion(Image<Gray, byte> originImage, int kernelSize = 3, int iterations = 1)
        {
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(kernelSize, kernelSize), new Point(-1, -1));
            Image<Gray, byte> result = new Image<Gray, byte>(originImage.Size);
            CvInvoke.Erode(originImage, result, kernel, new Point(-1, -1), iterations, BorderType.Default, new MCvScalar(0));
            return result;
        }

        /// <summary>
        /// Converts a grayscale image to an RGB image.
        /// </summary>
        /// <param name="originImage">The input grayscale image.</param>
        /// <returns>The converted RGB image.</returns>
        public static Image<Bgr, byte> ToBGR(Image<Gray, byte> originImage)
        {
            return originImage.Convert<Bgr, byte>();
        }

        /// <summary>
        /// Converts an RGB image to a grayscale image.
        /// </summary>
        /// <param name="originImage">The input RGB image.</param>
        /// <returns>The converted grayscale image.</returns>
        public static Image<Gray, byte> ToGray(Image<Bgr, byte> originImage)
        {
            return originImage.Convert<Gray, byte>();
        }

        /// <summary>
        /// Applies binary thresholding to a grayscale image.
        /// </summary>
        /// <param name="originImage">The input grayscale image.</param>
        /// <param name="thresholdValue">The threshold value for binarization.</param>
        /// <returns>The binarized image.</returns>
        public static Image<Gray, byte> ToBinarization(Image<Gray, byte> originImage, Gray thresholdValue)
        {
            return originImage.ThresholdBinary(thresholdValue, new Gray(255));
        }

        /// <summary>
        /// Rotates an RGB image by a given angle.
        /// </summary>
        /// <param name="modelImage">The input RGB image.</param>
        /// <param name="degree">The rotation angle in degrees.</param>
        /// <param name="fillColor">The background color for uncovered areas.</param>
        /// <returns>The rotated image.</returns>
        public static Image<Bgr, byte> RotateImage(Image<Bgr, byte> modelImage, int degree, Bgr fillColor)
        {
            Image<Bgr, byte> cloneImage = modelImage.Clone();
            double angle = degree * Math.PI / 180;
            double sinAngle = Math.Sin(angle), cosAngle = Math.Cos(angle);
            int width = modelImage.Width;
            int height = modelImage.Height;
            int newWidth = Convert.ToInt32(height * Math.Abs(sinAngle) + width * Math.Abs(cosAngle));
            int newHeight = Convert.ToInt32(width * Math.Abs(sinAngle) + height * Math.Abs(cosAngle));

            Matrix<float> rotationMatrix = new Matrix<float>(2, 3);
            PointF center = new PointF(width / 2, height / 2);
            CvInvoke.GetRotationMatrix2D(center, degree, 1.0, rotationMatrix);
            rotationMatrix[0, 2] += (newWidth - width) / 2;
            rotationMatrix[1, 2] += (newHeight - height) / 2;

            Image<Bgr, byte> rotatedImage = new Image<Bgr, byte>(newWidth, newHeight, fillColor);
            CvInvoke.WarpAffine(cloneImage, rotatedImage, rotationMatrix, new Size(newWidth, newHeight), Inter.Nearest, Warp.Default, BorderType.Transparent, new MCvScalar());
            return rotatedImage;
        }

        /// <summary>
        /// Moves an RGB image by a given offset.
        /// </summary>
        /// <param name="modelImage">The input RGB image.</param>
        /// <param name="x">The horizontal translation.</param>
        /// <param name="y">The vertical translation.</param>
        /// <param name="fillColor">The background color for uncovered areas.</param>
        /// <returns>The translated image.</returns>
        public static Image<Bgr, byte> MoveImage(Image<Bgr, byte> modelImage, int x, int y, Bgr fillColor)
        {
            Image<Bgr, byte> cloneImage = modelImage.Clone();
            Matrix<float> translationMatrix = new Matrix<float>(2, 3);
            PointF center = new PointF(modelImage.Width / 2, modelImage.Height / 2);
            CvInvoke.GetRotationMatrix2D(center, 0, 1.0, translationMatrix);
            translationMatrix[0, 2] += x;
            translationMatrix[1, 2] += y;

            Image<Bgr, byte> movedImage = new Image<Bgr, byte>(modelImage.Width, modelImage.Height, fillColor);
            CvInvoke.WarpAffine(cloneImage, movedImage, translationMatrix, new Size(modelImage.Width, modelImage.Height), Inter.Nearest, Warp.Default, BorderType.Transparent, new MCvScalar());
            return movedImage;
        }

        /// <summary>
        /// Applies a custom 3x3 filter to a grayscale image (compatible with most Emgu CV versions).
        /// </summary>
        /// <param name="sourceImage">The input grayscale image.</param>
        /// <param name="kernel">The 3x3 filter kernel to apply.</param>
        /// <param name="normalizeFactor">Optional factor to normalize the result. If 0, sum of kernel values is used.</param>
        /// <param name="offset">Optional value to add to each pixel after filtering.</param>
        /// <returns>The filtered grayscale image.</returns>
        public static Image<Gray, byte> ApplyCustomFilter(Image<Gray, byte> sourceImage, float[,] kernel, float normalizeFactor = 0, float offset = 0)
        {
            // Validate parameters
            if (sourceImage == null)
                throw new ArgumentNullException(nameof(sourceImage));

            if (kernel == null)
                throw new ArgumentNullException(nameof(kernel));

            if (kernel.GetLength(0) != 3 || kernel.GetLength(1) != 3)
                throw new ArgumentException("Kernel must be a 3x3 matrix.", nameof(kernel));

            // Calculate the normalization factor if not provided
            if (normalizeFactor == 0)
            {
                float sum = 0;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        sum += kernel[i, j];
                    }
                }
                normalizeFactor = sum != 0 ? sum : 1;
            }

            // Create the kernel matrix directly
            Image<Gray, float> kernelImage = new Image<Gray, float>(3, 3);

            // Fill the kernel values and apply normalization
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    kernelImage[i, j] = new Gray(kernel[i, j] / normalizeFactor);
                }
            }

            // Create the output image
            Image<Gray, byte> result = sourceImage.Clone();

            // Apply the filter using convolution
            CvInvoke.Filter2D(sourceImage, result, kernelImage, new Point(-1, -1), offset, BorderType.Reflect);

            // Clean up
            kernelImage.Dispose();

            return result;
        }

        /// <summary>
        /// Extracts a Region of Interest (ROI) from the input image.
        /// </summary>
        /// <typeparam name="TColor">The color type of the image (e.g., Bgr, Gray).</typeparam>
        /// <typeparam name="TDepth">The pixel depth type of the image (e.g., byte).</typeparam>
        /// <param name="inputImage">The input image.</param>
        /// <param name="roi">The rectangular region of interest.</param>
        /// <returns>A new image containing the selected ROI.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the input image is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the ROI is out of bounds.</exception>
        public static Image<TColor, TDepth> GetROI<TColor, TDepth>(Image<TColor, TDepth> inputImage, Rectangle roi)
            where TColor : struct, IColor
            where TDepth : new()
        {
            if (inputImage == null)
                throw new ArgumentNullException(nameof(inputImage), "Input image cannot be null."); 

            if (roi.X < 0 || roi.Y < 0 || roi.Right > inputImage.Width || roi.Bottom > inputImage.Height)
                throw new ArgumentException("ROI is out of bounds of the input image.");

            return inputImage.GetSubRect(roi);
        }
    }
 }
